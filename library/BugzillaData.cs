using System;
using System.Collections.Generic;

namespace Bugzz.Bugzilla
{
	internal class BugzillaData
	{
		SortedDictionary <string, string> supportedVersions;
		List <BugzillaVersionData> versionData;
		BugzillaVersionData defaultVersion;

		public BugzillaVersionData DefaultVersion {
			get {
				if (defaultVersion != null)
					return defaultVersion;

				if (versionData.Count == 0)
					return null;
				
				return versionData [0];
			}
		}
			
		public BugzillaData ()
		{
			supportedVersions = new SortedDictionary <string, string> ();
			versionData = new List <BugzillaVersionData> ();
		}

		public void AddSupportedVersion (string version, string label)
		{
			if (String.IsNullOrEmpty (version))
				return;

			if (supportedVersions.ContainsKey (version))
				return;

			if (String.IsNullOrEmpty (label))
				label = version;
			
			supportedVersions.Add (version, label);
		}

		public void AddVersionData (string version, BugzillaVersionData data, bool isDefault)
		{
			versionData.Add (data);
			if (isDefault)
				defaultVersion = data;
		}

		public BugzillaVersionData GetVersionData (string version)
		{
			if (versionData.Count == 0)
				return null;

			foreach (BugzillaVersionData bvd in versionData)
				if (bvd.Version == version)
					return bvd;

			return null;
		}
	}
}
