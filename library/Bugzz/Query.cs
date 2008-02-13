using System;
using System.Collections.Generic;
using System.Text;

namespace Bugzz
{
	public class Query
	{
		readonly char[] QUERY_SPLIT_CHARS = {'&', ';'};
		
		string queryPath;

		//TODO: it must be a container which allows dupes
		public Dictionary<string, string> QueryData /*{
			get;
			private set;
		}*/;
		
		public Query ()
		{
			QueryData = new Dictionary<string, string> ();
		}

		public string Email
		{
			get { return QueryData ["email1"]; }
			set {
				Dictionary<string, string> data = QueryData;

				AddQueryData ("email1", value);
				if (data.ContainsKey ("emailtype1"))
					data.Remove ("emailtype1");
				
				if (data.ContainsKey ("emailassigned_to1"))
					data.Remove ("emailassigned_to1");
				
				if (data.ContainsKey ("emailtype1"))
					data.Remove ("emailinfoprovider1");
				
				data.Add ("emailtype1", "exact");
				data.Add ("emailassigned_to1", "1");
				data.Add ("emailinfoprovider1", "1");
			}
		}

		public void AddQueryData (string fieldName, string fieldValue)
		{
			Dictionary<string, string> data = QueryData;

			if (data.ContainsKey (fieldName))
				data [fieldName] = fieldValue;
			else
				data.Add (fieldName, fieldValue);
		}

		internal void SetUrl (string url)
		{
			if (String.IsNullOrEmpty (url))
				return;

			int pos = url.IndexOf ("?");
			if (pos == -1)
				queryPath = url;
			else {
				queryPath = url.Substring (0, pos);
				string[] item;
				
				foreach (string p in url.Substring (pos + 1).Split (QUERY_SPLIT_CHARS)) {
					item = p.Split ('=');
					if (item.Length == 2)
						AddQueryData (item [0], item [1]);
					else
						AddQueryData (item [0], String.Empty);
				}
			}
		}
		
		public override string ToString ()
		{
			Dictionary<string, string> data = QueryData;
			StringBuilder ret = new StringBuilder ((queryPath ?? String.Empty) + "?");
			
			if (data != null) {
				bool first = true;
				
				foreach (KeyValuePair <string, string> kvp in data) {
					if (!first)
						ret.Append ("&");
					else
						first = false;
					
					ret.Append (kvp.Key + "=" + kvp.Value);
				}
			}

			return ret.ToString ();
		}
	}
}
