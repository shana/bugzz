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
		string cookiejar;

		public CookieManager ()
		{
			uris = new List<Uri> ();
			cookiejar = Path.Combine (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "Bugzz"), "jar");
			if (!Directory.Exists (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "Bugzz")))
				Directory.CreateDirectory (Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "Bugzz"));
			Load ();
		}

		public void AddUri (Uri uri)
		{
			if (!uris.Contains (uri))
				uris.Add (uri);
		}

		public void Load ()
		{
			if (File.Exists (cookiejar)) {
				using (StreamReader sr = new StreamReader(File.OpenRead (cookiejar))) {
					Cookie cookie = null;
					while (!sr.EndOfStream) {
						string line = sr.ReadLine ();
						if (String.IsNullOrEmpty (line))
							continue;
						string start = line.Substring (0, line.IndexOf (":"));
						string end = line.Substring (line.IndexOf (":") + 1);
						if (String.IsNullOrEmpty (end))
							continue;
						switch (start) {
							case "Uri":
								if (cookie != null) {
									this.Add (cookie);
								}
								cookie = new Cookie ();
								AddUri (new Uri (end));
								break;
							case "Name":
								if (cookie == null) continue;
								cookie.Name = end;
								break;
							case "Value":
								if (cookie == null) continue;
								cookie.Value = end;
								break;
							case "Domain":
								if (cookie == null) continue;
								cookie.Domain = end;
								break;
							case "Path":
								if (cookie == null) continue;
								cookie.Path = end;
								break;
							case "Port":
								if (cookie == null) continue;
								cookie.Port = end;
								break;
							case "Secure":
								if (cookie == null) continue;
								cookie.Secure = bool.Parse (end);
								break;
							case "Issued":
								if (cookie == null) continue;
								//cookie.TimeStamp = end;
								break;
							case "Expires":
								if (cookie == null) continue;
								cookie.Expires = DateTime.Parse (end);
								break;
							case "Expired":
								if (cookie == null) continue;
								cookie.Expired = bool.Parse (end);
								break;
							case "Discard":
								if (cookie == null) continue;
								cookie.Discard = bool.Parse (end);
								break;
							case "Comment":
								if (cookie == null) continue;
								cookie.Comment = end;
								break;
							case "CommentUri":
								if (cookie == null) continue;
								cookie.CommentUri = new Uri (end);
								break;
							case "Version":
								if (cookie == null) continue;
								cookie.Version = (end == "2109" ? 1 : 2); //?!?
								break;
						}
					}
				}
			}
		}

		public void Save ()
		{
			StreamWriter sw = new StreamWriter (File.Open (cookiejar, FileMode.Create, FileAccess.Write));
			sw.WriteLine (this.ToString ());
			sw.Close ();
		}

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder ();
			foreach (Uri uri in uris) {
				foreach (Cookie cook in GetCookies (uri)) {
					sb.Append ("Uri:" + uri.ToString());
					sb.AppendLine ();
					sb.Append ("Name:" + cook.Name);
					sb.AppendLine ();
					sb.Append ("Value:" + cook.Value);
					sb.AppendLine ();
					sb.Append ("Domain:" + cook.Domain);
					sb.AppendLine ();
					sb.Append ("Path:" + cook.Path);
					sb.AppendLine ();
					sb.Append ("Port:" + cook.Port);
					sb.AppendLine ();
					sb.Append ("Secure:" + cook.Secure);
					sb.AppendLine ();

					sb.Append ("TimeSpamp:" + cook.TimeStamp);
					sb.AppendLine ();
					sb.Append ("Expires:" + cook.Expires);
					sb.AppendLine ();
					sb.Append ("Expired:" + cook.Expired);
					sb.AppendLine ();
					sb.Append ("Discard:" + cook.Discard);
					sb.AppendLine ();
					sb.Append ("Comment:" + cook.Comment);
					sb.AppendLine ();
					sb.Append ("CommentUri:" + cook.CommentUri);
					sb.AppendLine ();
					sb.Append ("Version:" + (cook.Version == 1 ? "2109" : "2965"));
					sb.AppendLine ();

					// Show the string representation of the cookie.
					sb.Append ("String:" + cook.ToString ());
					sb.AppendLine ();
				}
			}
			return sb.ToString ();
		}
	}
}
