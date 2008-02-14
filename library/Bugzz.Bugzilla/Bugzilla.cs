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
	internal class Bugzilla
	{
		HashBag <Classification> classifications = new HashBag <Classification> ();
		HashBag <Product> products = new HashBag <Product> ();
		HashBag <Component> components = new HashBag <Component> ();
		HashBag <FoundInVersion> foundInVersion = new HashBag <FoundInVersion> ();
		HashBag <FixedInMilestone> fixedInMilestone = new HashBag <FixedInMilestone> ();
		
		bool initialDataLoaded;
		string targetVersion;
		
		public WebIO WebIO /*{
			get;
			private set;
		}*/;


		private DataManager dataManager;		
		public Bugzilla (string baseUrl, LoginData loginData)
		: this (baseUrl, loginData, null)
		{
		}

		public Bugzilla (string baseUrl, LoginData loginData, string targetVersion)
		{
			this.targetVersion = targetVersion;
			this.dataManager = new DataManager (targetVersion);
			WebIO = new WebIO (baseUrl, loginData, dataManager);
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
			
			string query = WebIO.GetDocument (q.ToString ());
			if (String.IsNullOrEmpty (query))
				return null;
			
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

			string query = WebIO.GetDocument (q.ToString ());
			if (String.IsNullOrEmpty (query))
				return null;
			
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
	}
}
