using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace Bugzz.Network
{
	internal class CookieManager: CookieContainer
	{
		List<Uri> uris;

		public CookieManager ()
		{
			uris = new List<Uri> ();
		}

		public void AddUri (Uri uri)
		{
			if (!uris.Contains (uri))
				uris.Add (uri);
		}

		public void Load ()
		{
			string cookiejar = Path.Combine (global::Bugzz.Constants.DataDirectory, "jar");
			if (File.Exists (cookiejar)) {
				
			}
		}

		public void Save ()
		{
			string cookiejar = Path.Combine (global::Bugzz.Constants.DataDirectory, "jar");
			StreamWriter sw = new StreamWriter (File.Open (cookiejar, FileMode.OpenOrCreate, FileAccess.Write));
			sw.WriteLine (this.ToString ());
			sw.Close ();
		}

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder ();
			foreach (Uri uri in uris) {
				foreach (Cookie cook in GetCookies (uri)) {
					sb.Append ("Cookie:");
					sb.AppendLine ();
					sb.AppendFormat ("{0} = {1}", cook.Name, cook.Value);
					sb.AppendLine ();
					sb.AppendFormat ("Domain: {0}", cook.Domain);
					sb.AppendLine ();
					sb.AppendFormat ("Path: {0}", cook.Path);
					sb.AppendLine ();
					sb.AppendFormat ("Port: {0}", cook.Port);
					sb.AppendLine ();
					sb.AppendFormat ("Secure: {0}", cook.Secure);
					sb.AppendLine ();

					sb.AppendFormat ("When issued: {0}", cook.TimeStamp);
					sb.AppendLine ();
					sb.AppendFormat ("Expires: {0} (expired? {1})",
						cook.Expires, cook.Expired);
					sb.AppendLine ();
					sb.AppendFormat ("Don't save: {0}", cook.Discard);
					sb.AppendLine ();
					sb.AppendFormat ("Comment: {0}", cook.Comment);
					sb.AppendLine ();
					sb.AppendFormat ("Uri for comments: {0}", cook.CommentUri);
					sb.AppendLine ();
					sb.AppendFormat ("Version: RFC {0}", cook.Version == 1 ? "2109" : "2965");
					sb.AppendLine ();

					// Show the string representation of the cookie.
					sb.AppendFormat ("String: {0}", cook.ToString ());
					sb.AppendLine ();
				}
			}
			return sb.ToString ();
		}
	}
}
