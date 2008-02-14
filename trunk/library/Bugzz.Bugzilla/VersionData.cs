using System;
using System.Collections.Generic;

namespace Bugzz.Bugzilla
{
	public sealed class VersionData
	{
		Dictionary <string, string> urls;
		Dictionary <string, string> initialVariables;
		Dictionary <string, string> searchVariables;
		Dictionary <string, string> loginVariables;
		
		private string version;
		public string Version {
			get { return version; }
			private set { version = value; }
		}
		
		public VersionData (string version)
		{
			urls = new Dictionary <string, string> ();
			initialVariables = new Dictionary <string, string> ();
			searchVariables = new Dictionary <string, string> ();
			loginVariables = new Dictionary <string, string> ();
			
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

		private void AddVariable (Dictionary<string, string> dic, string name, string value)
		{
			if (String.IsNullOrEmpty (name) || dic.ContainsKey (name))
				return;

			dic.Add (name, String.IsNullOrEmpty (value) ? name : value);
		}


		public void AddInitialVariable (string name, string value)
		{
			AddVariable (initialVariables, name, value);
		}

		public void AddSearchVariable (string name, string value)
		{
			AddVariable (searchVariables, name, value);
		}

		public void AddLoginVariable (string name, string value)
		{
			AddVariable (loginVariables, name, value);
		}
		
		public string GetUrl (string name)
		{
			string ret;

			if (urls.TryGetValue (name, out ret))
				return ret;

			return null;
		}

		// Looks up web variable name using canonical variable name
		private string GetVariable (Dictionary<string, string> dic, string name)
		{
			string ret;

			if (dic.TryGetValue (name, out ret))
				return ret;

			return null;

		}

		public string GetInitialVariable (string name)
		{
			return GetVariable (initialVariables, name);
		}

		public string GetSearchVariable (string name)
		{
			return GetVariable (searchVariables, name);
		}

		public string GetLoginVariable (string name) 
		{
			return GetVariable (loginVariables, name);
		}
		
		// Checks if we're interested for a variable with a web name 'name' and returns the
		// corresponding canonical name. It has to be implemented using a linear search
		// since there might be cases when several canonical names will map to the same web
		// name.
		private string HasVariable (Dictionary<string, string> dic, string name)
		{
			foreach (KeyValuePair<string, string> kvp in dic) {
				if (kvp.Value == name)
					return kvp.Key;
			}

			return null;
		}

		public string HasInitialVariable (string name)
		{
			return HasVariable (initialVariables, name);
		}

		public string HasSearchVariable (string name)
		{
			return HasVariable (initialVariables, name);
		}
	}
}
