//
// Bugzz - Multi GUI Desktop Bugzilla Client
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2008 Novell, Inc.
//
// Authors:
//	Andreia Gaita (avidigal@novell.com)
//	Marek Habersack (mhabersack@novell.com)
//
using System;
using System.Collections.Generic;
using System.Text;
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
		Dictionary <string, List <string>> mimeTypes;
		
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
			this.targetVersion = targetVersion;
		}

		public List <string> GetMimeType (string name)
		{
			if (String.IsNullOrEmpty (name))
				return null;
			
			if (!loaded)
				LoadData ();

			if (mimeTypes == null)
				return null;
			
			List <string> ret;

			if (mimeTypes.TryGetValue (name, out ret))
				return ret;

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

			nodes = versionNode.SelectNodes ("./variables/login/variable[string-length (@name) > 0 and string-length (@value) > 0]");
			if (nodes == null || nodes.Count == 0)
				throw new BugzillaException ("No login variables defined for version.");

			foreach (XmlNode node in nodes) {
				attrs = node.Attributes;
				name = attrs["name"];
				value = attrs["value"];

				bvd.AddLoginVariable (name.Value, value.Value);
			}

			nodes = versionNode.SelectNodes ("./formNames/form[string-length (@name) > 0 and string-length (@value) > 0]");
			if (nodes == null || nodes.Count == 0)
				throw new BugzillaException ("No form names defined for version.");

			foreach (XmlNode node in nodes) {
				attrs = node.Attributes;
				name = attrs ["name"];
				value = attrs ["value"];

				bvd.AddFormName (name.Value, value.Value);
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

				nodes = doc.SelectNodes ("/bugzilla/mimeTypes/type[string-length (@name) > 0]");
				if (nodes == null || nodes.Count == 0)
					return;

				mimeTypes = new Dictionary <string, List <string>> ();
				XmlNodeList tmp;
				string typeName;
				List <string> values;
				
				foreach (XmlNode node in nodes) {
					typeName = node.Attributes ["name"].Value;
					tmp = node.SelectNodes ("./string[string-length (@value) > 0]");
					if (!mimeTypes.TryGetValue (typeName, out values)) {
						values = new List <string> ();
						mimeTypes.Add (typeName, values);
					}

					foreach (XmlNode tmpNode in tmp)
						values.Add (tmpNode.Attributes ["value"].Value);
				}
			} catch (BugzzException) {
				throw;
			} catch (Exception ex) {
				throw new BugzillaException ("Failed to load data file.", ex);
			}
		}

	}
}
