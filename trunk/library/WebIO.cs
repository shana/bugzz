using System;
using System.IO;
using System.Net;

namespace Bugzz.Network
{
	public class WebIO
	{
		readonly static string userAgent;
		
		Uri baseUrl;

		static WebIO ()
		{
			// TODO: construct something funnier later on
			userAgent = "Buggz/0.0.1";
		}
		
		public WebIO (string baseUrl)
		{
			if (String.IsNullOrEmpty (baseUrl))
				throw new ArgumentNullException ("Base request URL must be specified.", "baseUrl");
			
			try {
				this.baseUrl = new Uri (baseUrl);
			} catch (Exception ex) {
				throw new ArgumentException ("Invalid base URL.", "baseUrl", ex);
			}
		}

		public string GetDocument (string relativeUrl)
		{
			WebClient wc = new WebClient ();
			wc.Headers.Add ("user-agent", userAgent);

			string fullUrl;
			
			try {
				UriBuilder ub = new UriBuilder (baseUrl);
				ub.Path = relativeUrl;
				fullUrl = ub.ToString ();
			} catch (Exception ex) {
				throw new BugzzWebIOException ("Malformed relative URL.", relativeUrl, ex);
			}
			
			try {
				using (Stream data = wc.OpenRead (fullUrl)) {
					using (StreamReader reader = new StreamReader (data)) {
						return reader.ReadToEnd ();
					}
				}
			} catch (Exception ex) {
				throw new BugzzWebIOException ("Error downloading document.", fullUrl, ex);
			}
		}
	}
}
