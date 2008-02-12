using System;
using System.Collections.Generic;

namespace Bugzz.Bugzilla
{
	internal class VersionData
	{
		Dictionary <string, string> urls;
		Dictionary <string, string> initialVariables;
		
		public string Version {
			get; private set;
		}
		
		public VersionData (string version)
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

		// Looks up web variable name using canonical variable name
		public string GetInitialVariable (string name)
		{
			string ret;

			if (initialVariables.TryGetValue (name, out ret))
				return ret;


			return null;
		}

		// Checks if we're interested for a variable with a web name 'name' and returns the
		// corresponding canonical name. It has to be implemented using a linear search
		// since there might be cases when several canonical names will map to the same web
		// name.
		public string HasInitialVariable (string name)
		{
			foreach (KeyValuePair <string, string> kvp in initialVariables) {
				if (kvp.Value == name)
					return kvp.Key;
			}

			return null;
		}
		
	}
}
