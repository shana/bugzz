using System;
using SGC=System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using C5;

using Bugzz;
using Bugzz.Network;
using HtmlAgilityPack;

namespace Bugzz.Bugzilla
{
	internal class Bugzilla
	{
		bool initialDataLoaded;
		string targetVersion;
		LoginData loginData;
		Regex rdfRegexp = new Regex ("^\\s*<\\?xml.*\\?>\\s*(.|\\n)*<RDF", RegexOptions.Compiled);
		Regex xmlRegexp = new Regex ("<\\?xml.*\\?>", RegexOptions.Compiled);
		Regex htmlRegexp = new Regex ("<html(.|\\n)*>", RegexOptions.Compiled);
		
#if FALLBACK
		public HashBag <IInitialValue> Classifications;
		public HashBag <IInitialValue> Products;
		public HashBag <IInitialValue> Components;
		public HashBag <IInitialValue> FoundInVersion;
		public HashBag <IInitialValue> FixedInMilestone;
		public HashBag <IInitialValue> Status;
		public HashBag <IInitialValue> Resolution;
		public HashBag <IInitialValue> Severity;
		public HashBag <IInitialValue> Priority;
		public HashBag <IInitialValue> Hardware;
		public HashBag <IInitialValue> OS;
#else
		public HashBag <IInitialValue> Classifications { get; private set; }
		public HashBag <IInitialValue> Products {get; private set; }
		public HashBag <IInitialValue> Components {get; private set; }
		public HashBag <IInitialValue> FoundInVersion {get; private set; }
		public HashBag <IInitialValue> FixedInMilestone { get; private set; }
		public HashBag <IInitialValue> Status { get; private set; }
		public HashBag <IInitialValue> Resolution { get; private set; }
		public HashBag <IInitialValue> Severity { get; private set; }
		public HashBag <IInitialValue> Priority { get; private set; }
		public HashBag <IInitialValue> Hardware { get; private set; }
		public HashBag <IInitialValue> OS { get; private set; }
#endif
		string baseUrl;
		public string BaseUrl
		{
			get { return baseUrl; }
			set
			{
				baseUrl = value;
				if (WebIO == null) {					
					if (dataManager == null)
						dataManager = new DataManager (targetVersion);
					WebIO = new WebIO (baseUrl, loginData, dataManager);
				} else
					WebIO.BaseUrl = new Uri (baseUrl);
			}
		}

#if FALLBACK
		public WebIO WebIO;
#else
		public WebIO WebIO {get; private set; }
#endif

		private DataManager dataManager;

		public Bugzilla (LoginData loginData)
		: this (null, loginData, null)
		{
		}

		public Bugzilla (string baseUrl, LoginData loginData)
		: this (baseUrl, loginData, null)
		{
		}

		public Bugzilla (string baseUrl, LoginData loginData, string targetVersion)
		{
			this.targetVersion = targetVersion;
			this.baseUrl = baseUrl;
			this.dataManager = new DataManager (targetVersion);
			this.loginData = loginData;
			WebIO = new WebIO (baseUrl, loginData, dataManager);
			
			Classifications = new HashBag <IInitialValue> ();
			Products = new HashBag <IInitialValue> ();
			Components = new HashBag <IInitialValue> ();
			FoundInVersion = new HashBag <IInitialValue> ();
			FixedInMilestone = new HashBag <IInitialValue> ();
			Status = new HashBag <IInitialValue> ();
			Resolution = new HashBag <IInitialValue> ();
			Severity = new HashBag <IInitialValue> ();
			Priority = new HashBag <IInitialValue> ();
			Hardware = new HashBag <IInitialValue> ();
			OS = new HashBag <IInitialValue> ();
		}

		public void Refresh ()
		{
			LoadInitialData ();
		}

		public SGC.List <Bug> Search (Query q)
		{
			return null;
		}
		
		public SGC.Dictionary <string, Bug> GetBugList (Query q)
		{
			VersionData bvd = dataManager.VersionData;
			string queryUrl = bvd.GetUrl ("buglist");

			if (String.IsNullOrEmpty (queryUrl))
				throw new BugzillaException ("Cannot retrieve bug list - no URL given.");

			q.SetUrl (queryUrl);
			q.AddQueryData ("ctype", "rdf");
			
			string query = WebIO.GetDocument (q.ToString (), "rdf", rdfRegexp);
			if (String.IsNullOrEmpty (query))
				throw new BugzillaException ("No valid response retrieved.");
			
			ResponseParser rp = new ResponseParser (query);
			
			return rp.Bugs;
		}

		public SGC.Dictionary <string, Bug> GetBugs (Query q)
		{
			VersionData bvd = dataManager.VersionData;
			string queryUrl = bvd.GetUrl ("show_bug");

			if (String.IsNullOrEmpty (queryUrl))
				throw new BugzillaException ("Cannot show bugs - no URL given.");

			q.SetUrl (queryUrl);
			q.AddQueryData ("ctype", "xml");

			string query = WebIO.GetDocument (q.ToString (), "xml", xmlRegexp);
			if (String.IsNullOrEmpty (query))
				throw new BugzillaException ("No valid response retrieved.");
			
			ResponseParser rp = new ResponseParser (query);

			return rp.Bugs;
		}
		
		void LoadInitialData ()
		{
			if (initialDataLoaded)
				return;

			VersionData bvd = dataManager.VersionData;
			string queryUrl = bvd.GetUrl ("initial");
			if (String.IsNullOrEmpty (queryUrl))
				throw new BugzillaException ("Cannot retrieve initial data - no URL given.");
			
			string query = WebIO.GetDocument (queryUrl, "html", htmlRegexp);
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
					StoreValues <Classification> (Classifications, nodes);
					break;

				case "product":
					StoreValues <Product> (Products, nodes);
					break;

				case "component":
					StoreValues <Component> (Components, nodes);
					break;

				case "version":
					StoreValues <FoundInVersion> (FoundInVersion, nodes);
					break;

				case "target_milestone":
					StoreValues <FixedInMilestone> (FixedInMilestone, nodes);
					break;

				case "bug_status":
					StoreValues <Status> (Status, nodes);
					break;

				case "resolution":
					StoreValues <Resolution> (Resolution, nodes);
					break;

				case "bug_severity":
					StoreValues <Severity> (Severity, nodes);
					break;

				case "priority":
					StoreValues <Priority> (Priority, nodes);
					break;

				case "rep_platform":
					StoreValues <Hardware> (Hardware, nodes);
					break;

				case "op_sys":
					StoreValues <OS> (OS, nodes);
					break;
			}
		}

		void StoreValues <T> (HashBag <IInitialValue> bag, HtmlNodeCollection nodes) where T:InitialValue,new()
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
	}
}
