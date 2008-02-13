using System;
using System.Collections.Generic;
using System.Text;

namespace Bugzz
{
	public class Query
	{
		internal class QueryDataItem
		{
			public string Name;
			public List <string> Values;

			public QueryDataItem (string name, string value)
			{
				Name = name;
				Values = new List <string> (1);
				Values.Add (value);
			}

			public override string ToString ()
			{
				StringBuilder sb = new StringBuilder ();
				bool first = true;
				
				foreach (string v in Values) {
					if (!first)
						sb.Append ("&");
					else
						first = false;

					sb.Append (Name + "=" + v);
				}

				return sb.ToString ();
			}
		}
		
		readonly char[] QUERY_SPLIT_CHARS = {'&', ';'};
		
		string queryPath;

		//TODO: it must be a container which allows dupes
		internal Dictionary <string, QueryDataItem> QueryData /*{
			get;
			private set;
		}*/;
		
		public Query ()
		{
			QueryData = new Dictionary <string, QueryDataItem> ();
		}

		public string Email
		{
			get {
				QueryDataItem qdi;

				if (QueryData.TryGetValue ("email1", out qdi)) {
					if (qdi.Values.Count > 0)
						return qdi.Values [0];
					return null;
				}
				
				return null;
			}
			
			set {
				var data = QueryData;

				AddQueryData ("email1", value);
				
				if (data.ContainsKey ("emailtype1"))
					data.Remove ("emailtype1");
				
				if (data.ContainsKey ("emailassigned_to1"))
					data.Remove ("emailassigned_to1");
				
				if (data.ContainsKey ("emailtype1"))
					data.Remove ("emailinfoprovider1");
				
				AddQueryData ("emailtype1", "exact");
				AddQueryData ("emailassigned_to1", "1");
				AddQueryData ("emailinfoprovider1", "1");
			}
		}

		public void AddQueryData (string fieldName, string fieldValue)
		{
			Dictionary <string, QueryDataItem> data = QueryData;
			QueryDataItem qdi;
			
			if (data.TryGetValue (fieldName, out qdi))
				qdi.Values.Add (fieldValue);
			else {
				qdi = new QueryDataItem (fieldName, fieldValue);
				data.Add (fieldName, qdi);
			}
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
			Dictionary <string, QueryDataItem> data = QueryData;
			StringBuilder ret = new StringBuilder ((queryPath ?? String.Empty) + "?");
			
			if (data != null) {
				bool first = true;
				
				foreach (QueryDataItem qdi in data.Values) {
					if (!first)
						ret.Append ("&");
					else
						first = false;
					
					ret.Append (qdi.ToString ());
				}
			}

			return ret.ToString ();
		}
	}
}
