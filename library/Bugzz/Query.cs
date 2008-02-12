using System;
using System.Collections.Generic;
using System.Text;

namespace Bugzz
{
	public class Query
	{
		private Dictionary<string, string> query;

		public Query ()
		{
			query = new Dictionary<string, string> ();
		}

		public string Email
		{
			get { return query["email1"]; }
			set {
				if (!query.ContainsKey ("email1"))
					query.Add ("email1", value);
				else
					query["email1"] = value;

				if (query.ContainsKey ("emailtype1")) query.Remove ("emailtype1");
				if (query.ContainsKey ("emailassigned_to1")) query.Remove ("emailassigned_to1");
				if (query.ContainsKey ("emailtype1")) query.Remove ("emailinfoprovider1");
				query.Add ("emailtype1", "exact");
				query.Add ("emailassigned_to1", "1");
				query.Add ("emailinfoprovider1", "1");
			}
		}

		internal string GetQuery () {
			System.Text.StringBuilder sb = new StringBuilder ();
			foreach (KeyValuePair<string, string> fields in query) {
				sb.Append ("&" + fields.Key + "=" + fields.Value);
			}
			return sb.ToString ();
		}
	}
}
