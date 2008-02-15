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
			ExtraData = new Dictionary<string, string> ();
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
				Dictionary<string, QueryDataItem> data = QueryData;

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
#if FALLBACK
		Dictionary<string, string> extraData;
		public Dictionary<string, string> ExtraData
		{
			get { return extraData; }
			set { extraData = value; }
		}
#else
		public Dictionary <string, string> ExtraData { get; set; }
#endif

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
			
			bool first = true;
			if (data != null) {
				foreach (QueryDataItem qdi in data.Values) {
					if (!first)
						ret.Append ("&");
					else
						first = false;
					
					ret.Append (qdi.ToString ());
				}
			}
			
			// data on ExtraData is client-configured, fixed, and not to be repeated,
			// so we don't want to add it to the QueryData
			foreach (KeyValuePair<string, string> vals in ExtraData) {
				if (!first)
					ret.Append ("&");
				else
					first = false;
				
				ret.Append (vals.Key + "=" + vals.Value);
			}

			return ret.ToString ();
		}
	}
}
