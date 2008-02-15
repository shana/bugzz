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
