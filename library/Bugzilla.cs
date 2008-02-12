using System;
using System.IO;
using System.Xml;

using C5;

using Bugzz.Network;
using HtmlAgilityPack;

namespace Bugzz.Bugzilla
{
	public class Bugzilla
	{
		static object bugzillaDataLock = new object ();
		static BugzillaData bugzillaData;

		HashBag <BugzillaClassification> classifications = new HashBag <BugzillaClassification> ();
		HashBag <BugzillaProduct> products = new HashBag <BugzillaProduct> ();
		HashBag <BugzillaComponent> components = new HashBag <BugzillaComponent> ();
		HashBag <BugzillaFoundInVersion> foundInVersion = new HashBag <BugzillaFoundInVersion> ();
		HashBag <BugzillaFixedInMilestone> fixedInMilestone = new HashBag <BugzillaFixedInMilestone> ();
		
		WebIO webIO;
		bool initialDataLoaded;
		string targetVersion;

		public WebIO WebIO {
			get;
			private set;
		}
		
		static Bugzilla ()
		{
			LoadBugzillaData ();
		}
		
		public Bugzilla (string baseUrl)
		: this (baseUrl, null)
		{
		}

		public Bugzilla (string baseUrl, string targetVersion)
		{
			WebIO = new WebIO (baseUrl);
			this.targetVersion = targetVersion;
		}

		public void Refresh ()
		{
			LoadInitialData ();
		}		
		
		void LoadInitialData ()
		{
			if (initialDataLoaded)
				return;

			if (!LogIn ())
				throw new BugzzBugzillaException ("Login failed.");

			BugzillaVersionData bvd = null;

			if (!String.IsNullOrEmpty (targetVersion))
				bvd = bugzillaData.GetVersionData (targetVersion);
			if (bvd == null)
				bvd = bugzillaData.DefaultVersion;
			if (bvd == null)
				throw new BugzzBugzillaException ("Unable to determine bugzila version data to use.");

			string queryUrl = bvd.GetUrl ("initial");
			if (String.IsNullOrEmpty (queryUrl))
				throw new BugzzBugzillaException ("Cannot retrieve initial data - no URL given.");
			
			string query = WebIO.GetDocument (queryUrl);
			if (String.IsNullOrEmpty (query))
				throw new BugzzBugzillaException ("No document returned by server for initial data.");
			
			HtmlDocument doc = new HtmlDocument ();

			try {
				doc.LoadHtml (query);
			} catch (Exception ex) {
				throw new BugzzBugzillaException ("Failed to parse the response document.", ex);
			}

			HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes ("//select[string-length (@id) > 0]");
			if (nodes == null || nodes.Count == 0)
				throw new BugzzBugzillaException ("No initial data found.");

			// Load the "toplevel" values - that is, all possible values for all
			// the 5 Product Information selects.
			string canonicalName, id;
			foreach (HtmlNode node in nodes) {
				id = node.Attributes ["id"].Value;
				canonicalName = bvd.HasInitialVariable (id);
				if (canonicalName == null)
					continue;
				StoreSelectValues (node, canonicalName);
			}
			
			initialDataLoaded = true;
		}
		
		void StoreSelectValues (HtmlNode selectNode, string canonicalName)
		{
			HtmlNodeCollection nodes = selectNode.SelectNodes ("./option");

			switch (canonicalName) {
				case "classification":
					StoreValues <BugzillaClassification> (classifications, nodes);
					break;

				case "product":
					StoreValues <BugzillaProduct> (products, nodes);
					break;

				case "component":
					StoreValues <BugzillaComponent> (components, nodes);
					break;

				case "version":
					StoreValues <BugzillaFoundInVersion> (foundInVersion, nodes);
					break;

				case "target_milestone":
					StoreValues <BugzillaFixedInMilestone> (fixedInMilestone, nodes);
					break;
			}
		}

		void StoreValues <T> (HashBag <T> bag, HtmlNodeCollection nodes) where T:BugzillaInitialValue,new()
		{
			HtmlAttributeCollection attrs;
			HtmlAttribute value;
			string label;
			T newItem;
			
			foreach (HtmlNode node in nodes) {
				attrs = node.Attributes;
				if (attrs != null)
					value = attrs ["value"];
				else
					value = null;

				label = node.InnerText.Trim ();
				newItem = new T ();
				newItem.Set (label, value != null ? value.Value : label);
				bag.Add (newItem);
			}
		}
		
		bool LogIn ()
		{
			return true;
		}

		static void LoadBugzillaData ()
		{
			string datafile = Path.Combine (Bugzz.Constants.DataDirectory, "bugzilla.xml");

			if (File.Exists (datafile)) {
				LoadDataFile (datafile);
				return;
			}
			
#if DEBUG
			datafile = String.Format (".{0}data{0}bugzilla.xml", Path.DirectorySeparatorChar);
			if (File.Exists (datafile)) {
				LoadDataFile (datafile);
				return;
			}
#endif
			throw new BugzzBugzillaException ("Bugzilla data file not found.");
		}

		static void LoadDataFile (string dataFile)
		{
			XmlDocument doc;

			try {
				doc = new XmlDocument ();
				doc.Load (dataFile);
				
				XmlNodeList nodes = doc.SelectNodes ("/bugzilla/supportedVersions/version[string-length (@name) > 0 and string-length (@label) > 0]");
				if (nodes == null || nodes.Count == 0)
					throw new BugzzBugzillaException ("Missing supported versions information in the data file.");

				lock (bugzillaDataLock) {
					XmlAttribute name, label;
					XmlAttributeCollection attrs;
					
					bugzillaData = new BugzillaData ();
					foreach (XmlNode node in nodes) {
						attrs = node.Attributes;
						name = attrs ["name"];
						label = attrs ["label"];
						
						bugzillaData.AddSupportedVersion (name.Value, label.Value);
					}

					nodes = doc.SelectNodes ("/bugzilla/version[string-length (@value) > 0]");
					if (nodes == null || nodes.Count == 0)
						throw new BugzzBugzillaException ("No bugzilla definitions found in the data file.");

					foreach (XmlNode node in nodes)
						StoreBugzillaVersion (node);
				}
				
			} catch (BugzzException) {
				throw;
			} catch (Exception ex) {
				throw new BugzzBugzillaException ("Failed to load data file.", ex);
			}
		}

		static void StoreBugzillaVersion (XmlNode versionNode)
		{
			string version = versionNode.Attributes ["value"].Value;
			XmlNodeList nodes = versionNode.SelectNodes ("./urls/url[string-length (@name) > 0 and string-length (@value) > 0]");

			if (nodes == null || nodes.Count == 0)
				throw new BugzzBugzillaException ("No URLs defined for version.");

			BugzillaVersionData bvd = new BugzillaVersionData (version);
			
			XmlAttribute name, value;
			XmlAttributeCollection attrs;
			
			foreach (XmlNode node in nodes) {
				attrs = node.Attributes;
				name = attrs ["name"];
				value = attrs ["value"];

				bvd.AddUrl (name.Value, value.Value);
			}

			nodes = versionNode.SelectNodes ("./variables/initial/variable[string-length (@name) > 0 and string-length (@value) > 0]");
			if (nodes == null || nodes.Count == 0)
				throw new BugzzBugzillaException ("No initial variables defined for version.");

			foreach (XmlNode node in nodes) {
				attrs = node.Attributes;
				name = attrs ["name"];
				value = attrs ["value"];

				bvd.AddInitialVariable (name.Value, value.Value);
			}
			
			name = versionNode.Attributes ["default"];
			bool isDefault = name != null ? name.Value == "true" : false;
			bugzillaData.AddVersionData (version, bvd, isDefault);
		}
	}
}
