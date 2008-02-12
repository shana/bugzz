using System;
using SGC=System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using C5;

using Bugzz;
using Bugzz.Network;
using HtmlAgilityPack;

namespace Bugzz.Bugzilla
{
	public class Bugzilla
	{
		static object bugzillaDataLock = new object ();
		internal static Data bugzillaData;

		HashBag <Classification> classifications = new HashBag <Classification> ();
		HashBag <Product> products = new HashBag <Product> ();
		HashBag <Component> components = new HashBag <Component> ();
		HashBag <FoundInVersion> foundInVersion = new HashBag <FoundInVersion> ();
		HashBag <FixedInMilestone> fixedInMilestone = new HashBag <FixedInMilestone> ();
		
		bool initialDataLoaded;
		string targetVersion;

		public WebIO WebIO {
			get;
			private set;
		}
		
		static Bugzilla ()
		{
			LoadData ();
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

		public SGC.List <Bug> Search (Query q)
		{
			return null;
		}
		
		public SGC.List <Bug> GetBugList (Query q)
		{
			LoadInitialData ();
			VersionData bvd = GetVersionData ();
			string queryUrl = bvd.GetUrl ("buglist");

			if (String.IsNullOrEmpty (queryUrl))
				throw new BugzillaException ("Cannot retrieve bug list - no URL given.");

			q.SetUrl (queryUrl);
			q.AddQueryData ("ctype", "rdf");
			
			string query = WebIO.GetDocument (q.ToString ());

			return null;
		}
		
		VersionData GetVersionData ()
		{
			VersionData ret = null;

			if (!String.IsNullOrEmpty (targetVersion))
				ret = bugzillaData.GetVersionData (targetVersion);
			if (ret == null)
				ret = bugzillaData.DefaultVersion;
			if (ret == null)
				throw new BugzillaException ("Unable to determine bugzila version data to use.");

			return ret;
		}
		
		void LoadInitialData ()
		{
			if (initialDataLoaded)
				return;

			VersionData bvd = GetVersionData ();
			string queryUrl = bvd.GetUrl ("initial");
			if (String.IsNullOrEmpty (queryUrl))
				throw new BugzillaException ("Cannot retrieve initial data - no URL given.");
			
			string query = WebIO.GetDocument (queryUrl);
			if (String.IsNullOrEmpty (query))
				throw new BugzillaException ("No document returned by server for initial data.");
			
			HtmlDocument doc = new HtmlDocument ();

			try {
				doc.LoadHtml (query);
			} catch (Exception ex) {
				throw new BugzillaException ("Failed to parse the response document.", ex);
			}

			HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes ("//select[string-length (@id) > 0]");
			if (nodes == null || nodes.Count == 0)
				throw new BugzillaException ("No initial data found.");

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
					StoreValues <Classification> (classifications, nodes);
					break;

				case "product":
					StoreValues <Product> (products, nodes);
					break;

				case "component":
					StoreValues <Component> (components, nodes);
					break;

				case "version":
					StoreValues <FoundInVersion> (foundInVersion, nodes);
					break;

				case "target_milestone":
					StoreValues <FixedInMilestone> (fixedInMilestone, nodes);
					break;
			}
		}

		void StoreValues <T> (HashBag <T> bag, HtmlNodeCollection nodes) where T:InitialValue,new()
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

		static void LoadData ()
		{
			string datafile = Path.Combine (global::Bugzz.Constants.DataDirectory, "bugzilla.xml");

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
			throw new BugzillaException ("Bugzilla data file not found.");
		}

		static void LoadDataFile (string dataFile)
		{
			XmlDocument doc;

			try {
				doc = new XmlDocument ();
				doc.Load (dataFile);
				
				XmlNodeList nodes = doc.SelectNodes ("/bugzilla/supportedVersions/version[string-length (@name) > 0 and string-length (@label) > 0]");
				if (nodes == null || nodes.Count == 0)
					throw new BugzillaException ("Missing supported versions information in the data file.");

				lock (bugzillaDataLock) {
					XmlAttribute name, label;
					XmlAttributeCollection attrs;
					
					bugzillaData = new Data ();
					foreach (XmlNode node in nodes) {
						attrs = node.Attributes;
						name = attrs ["name"];
						label = attrs ["label"];
						
						bugzillaData.AddSupportedVersion (name.Value, label.Value);
					}

					nodes = doc.SelectNodes ("/bugzilla/version[string-length (@value) > 0]");
					if (nodes == null || nodes.Count == 0)
						throw new BugzillaException ("No bugzilla definitions found in the data file.");

					foreach (XmlNode node in nodes)
						StoreBugzillaVersion (node);
				}
				
			} catch (BugzzException) {
				throw;
			} catch (Exception ex) {
				throw new BugzillaException ("Failed to load data file.", ex);
			}
		}

		static void StoreBugzillaVersion (XmlNode versionNode)
		{
			string version = versionNode.Attributes ["value"].Value;
			XmlNodeList nodes = versionNode.SelectNodes ("./urls/url[string-length (@name) > 0 and string-length (@value) > 0]");

			if (nodes == null || nodes.Count == 0)
				throw new BugzillaException ("No URLs defined for version.");

			VersionData bvd = new VersionData (version);
			
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
				throw new BugzillaException ("No initial variables defined for version.");

			foreach (XmlNode node in nodes) {
				attrs = node.Attributes;
				name = attrs ["name"];
				value = attrs ["value"];

				bvd.AddInitialVariable (name.Value, value.Value);
			}

			nodes = versionNode.SelectNodes ("./variables/search/variable[string-length (@name) > 0 and string-length (@value) > 0]");
			if (nodes == null || nodes.Count == 0)
				throw new BugzillaException ("No search variables defined for version.");

			foreach (XmlNode node in nodes) {
				attrs = node.Attributes;
				name = attrs ["name"];
				value = attrs ["value"];

				bvd.AddSearchVariable (name.Value, value.Value);
			}
			

			name = versionNode.Attributes ["default"];
			bool isDefault = name != null ? name.Value == "true" : false;
			bugzillaData.AddVersionData (version, bvd, isDefault);
		}
	}
}
