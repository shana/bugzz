using System;
using System.Collections.Generic;

namespace Bugzz.Bugzilla
{
	internal class BugzillaVersionData
	{
		Dictionary <string, string> urls;
		Dictionary <string, string> initialVariables;
		
		public string Version {
			get; private set;
		}
		
		public BugzillaVersionData (string version)
		{
			urls = new Dictionary <string, string> ();
			initialVariables = new Dictionary <string, string> ();
			Version = version;
		}

		public void AddUrl (string name, string value)
		{
			if (String.IsNullOrEmpty (name) || String.IsNullOrEmpty (value))
				return;
			
			if (urls.ContainsKey (name))
				return;

			urls.Add (name, value);
		}

		public void AddInitialVariable (string name, string value)
		{
			if (String.IsNullOrEmpty (name) || initialVariables.ContainsKey (name))
				return;

			initialVariables.Add (name, String.IsNullOrEmpty (value) ? name : value);
		}

		public string GetUrl (string name)
		{
			string ret;

			if (urls.TryGetValue (name, out ret))
				return ret;

			return null;
		}
	}
}
