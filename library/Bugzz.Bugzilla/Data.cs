using System;
using System.Collections.Generic;

namespace Bugzz.Bugzilla
{
	internal class Data
	{
		SortedDictionary <string, string> supportedVersions;
		List <VersionData> versionData;
		VersionData defaultVersion;

		public VersionData DefaultVersion {
			get {
				if (defaultVersion != null)
					return defaultVersion;

				if (versionData.Count == 0)
					return null;
				
				return versionData [0];
			}
		}
			
		public Data ()
		{
			supportedVersions = new SortedDictionary <string, string> ();
			versionData = new List <VersionData> ();
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

		public void AddVersionData (string version, VersionData data, bool isDefault)
		{
			versionData.Add (data);
			if (isDefault)
				defaultVersion = data;
		}

		public VersionData GetVersionData (string version)
		{
			if (versionData.Count == 0)
				return null;

			foreach (VersionData bvd in versionData)
				if (bvd.Version == version)
					return bvd;

			return null;
		}
	}
}
