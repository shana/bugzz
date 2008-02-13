using System;
using SGC = System.Collections.Generic;
using System.Text;
using C5;
using Bugzz;
using Bugzz.Network;
using HtmlAgilityPack;
using System.Xml;
using System.IO;

namespace Bugzz.Bugzilla
{
	internal class DataManager
	{
		static object bugzillaDataLock = new object ();
		private Data bugzillaData;
		private bool loaded;
		private VersionData versionData;

		public VersionData VersionData
		{
			get {
				if (!loaded) {
					LoadData ();
					versionData = GetVersionData ();
				}
				return versionData;
				
			}
		}

		string targetVersion;
		public DataManager (string targetVersion)
		{
			targetVersion = targetVersion;
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

		private void LoadData ()
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

		void StoreBugzillaVersion (XmlNode versionNode)
		{
			string version = versionNode.Attributes["value"].Value;
			XmlNodeList nodes = versionNode.SelectNodes ("./urls/url[string-length (@name) > 0 and string-length (@value) > 0]");

			if (nodes == null || nodes.Count == 0)
				throw new BugzillaException ("No URLs defined for version.");

			VersionData bvd = new VersionData (version);

			XmlAttribute name, value;
			XmlAttributeCollection attrs;

			foreach (XmlNode node in nodes) {
				attrs = node.Attributes;
				name = attrs["name"];
				value = attrs["value"];

				bvd.AddUrl (name.Value, value.Value);
			}

			nodes = versionNode.SelectNodes ("./variables/initial/variable[string-length (@name) > 0 and string-length (@value) > 0]");
			if (nodes == null || nodes.Count == 0)
				throw new BugzillaException ("No initial variables defined for version.");

			foreach (XmlNode node in nodes) {
				attrs = node.Attributes;
				name = attrs["name"];
				value = attrs["value"];

				bvd.AddInitialVariable (name.Value, value.Value);
			}

			nodes = versionNode.SelectNodes ("./variables/search/variable[string-length (@name) > 0 and string-length (@value) > 0]");
			if (nodes == null || nodes.Count == 0)
				throw new BugzillaException ("No search variables defined for version.");

			foreach (XmlNode node in nodes) {
				attrs = node.Attributes;
				name = attrs["name"];
				value = attrs["value"];

				bvd.AddSearchVariable (name.Value, value.Value);
			}


			name = versionNode.Attributes["default"];
			bool isDefault = name != null ? name.Value == "true" : false;
			bugzillaData.AddVersionData (version, bvd, isDefault);
		}


		private void LoadDataFile (string dataFile)
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
						name = attrs["name"];
						label = attrs["label"];

						bugzillaData.AddSupportedVersion (name.Value, label.Value);
					}

					nodes = doc.SelectNodes ("/bugzilla/version[string-length (@value) > 0]");
					if (nodes == null || nodes.Count == 0)
						throw new BugzillaException ("No bugzilla definitions found in the data file.");

					foreach (XmlNode node in nodes)
						StoreBugzillaVersion (node);
				}

			}
			catch (BugzzException) {
				throw;
			}
			catch (Exception ex) {
				throw new BugzillaException ("Failed to load data file.", ex);
			}
		}

	}
}
