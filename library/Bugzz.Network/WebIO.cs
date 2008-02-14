using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

using Bugzz;
using Bugzz.Bugzilla;

namespace Bugzz.Network
{
	internal class WebIO
	{
		readonly static string userAgent;

		CookieManager cookieJar;
		DataManager dataManager;
		
		LoginData loginData;
		
		public event Bugzz.DocumentRetrieveFailureEventHandler DocumentRetrieveFailure;
		public event Bugzz.DownloadStartedEventHandler DownloadStarted;
		public event Bugzz.DownloadEndedEventHandler DownloadEnded;
		public event Bugzz.DownloadProgressEventHandler DownloadProgress;
		
		public Uri BaseUrl/* {
			get;
			private set;
		}*/;

		static WebIO ()
		{
			// TODO: construct something funnier later on
			userAgent = global::Bugzz.Constants.Package + "/" + global::Bugzz.Constants.Version;
		}
		
		public WebIO (string baseUrl, LoginData loginData, DataManager dataManager)
		{
			this.loginData = loginData;
			this.dataManager = dataManager;
			
			if (String.IsNullOrEmpty (baseUrl))
				throw new ArgumentNullException ("Base request URL must be specified.", "baseUrl");
			
			try {
				BaseUrl = new Uri (baseUrl);
			} catch (Exception ex) {
				throw new ArgumentException ("Invalid base URL.", "baseUrl", ex);
			}

			cookieJar = new CookieManager ();
		}

		void OnDocumentRetrieveFailure (HttpWebRequest req)
		{
			if (DocumentRetrieveFailure == null)
				return;

			DocumentRetrieveFailure (this, new DocumentRetrieveFailureEventArgs (req));
		}

		void OnDownloadStarted (HttpWebResponse response)
		{
			if (DownloadStarted == null)
				return;

			DownloadStarted (this, new DownloadStartedEventArgs (response));
		}
		
		void OnDownloadEnded (HttpWebResponse response)
		{
			if (DownloadEnded == null)
				return;

			DownloadEnded (this, new DownloadEndedEventArgs (response));
		}

		void OnDownloadProgress (long maxCount, long curCount)
		{
			if (DownloadProgress == null)
				return;

			DownloadProgress (this, new DownloadProgressEventArgs (maxCount, curCount));
		}
		
		// TODO: this method should retry to retrieve the document a configurable amount of
		// times before returning an error.
		public string GetDocument (string relativeUrl)
		{
			string fullUrl;
			HttpWebRequest req;
			
			try {
				UriBuilder ub = new UriBuilder (BaseUrl);
				ub.Path = relativeUrl;
				fullUrl = ub.ToString ();
				req = WebRequest.Create (new Uri (fullUrl)) as HttpWebRequest;
				cookieJar.AddUri (new Uri (fullUrl));
				req.CookieContainer = cookieJar;
			} catch (Exception ex) {
				throw new WebIOException ("Malformed relative URL.", relativeUrl, ex);
			}
			
			HttpWebResponse response;
			try {
				Console.WriteLine ("Requesting URL: {0}", fullUrl);
				
				req.UserAgent = userAgent;
				response = req.GetResponse () as HttpWebResponse;
				
				if (response.StatusCode != HttpStatusCode.OK) {
					OnDocumentRetrieveFailure (req);
					return null;
				}

				cookieJar.Save ();

				// Check for redirects to the login page
				Console.WriteLine (req.RequestUri != req.Address);
				Console.WriteLine ("LoginData: " + loginData);
				
				if (loginData != null && loginData.Url != null && req.RequestUri != req.Address) {
					var address = req.Address;
					var loginAddress = loginData.Url;
					
					if (loginAddress.Scheme == address.Scheme &&
					    loginAddress.Host == address.Host &&
					    loginAddress.AbsolutePath == address.AbsolutePath)
						if (LogIn (req))
							return GetDocument (relativeUrl);
						else
							throw new WebIOException ("Login failure.", address.ToString ());
				}
				
				StringBuilder sb = new StringBuilder ();
				Stream data = response.GetResponseStream ();
				char[] buffer = new char [4096];
				int bufferLen = buffer.Length;
				int charsRead = -1;
				long count;
				
				using (StreamReader reader = new StreamReader (data)) {
					count = 0;
					OnDownloadStarted (response);
					long contentLength = response.ContentLength;
					if (contentLength == -1)
						contentLength = Int64.MaxValue; // potentially
									       // dangerous
					
					while (count < contentLength) {
						charsRead = reader.Read (buffer, 0, bufferLen);
						if (charsRead == 0)
							break;

						count += charsRead;
						OnDownloadProgress (contentLength, count);
						sb.Append (buffer, 0, charsRead);
					}
					OnDownloadEnded (response);
				}
				
				return sb.ToString ();
			} catch (BugzzException) {
				throw;
			} catch (WebException ex) {
				HttpWebResponse exResponse = ex.Response as HttpWebResponse;
				if (exResponse != null && exResponse.StatusCode == HttpStatusCode.NotModified)
					OnDownloadEnded (exResponse);
				else
					OnDocumentRetrieveFailure (req);
				
				return null;
			} catch (Exception ex) {
				throw new WebIOException ("Error downloading document.", fullUrl, ex);
			}
		}

		bool LogIn (HttpWebRequest req)
		{
			if (String.IsNullOrEmpty (loginData.Username) || String.IsNullOrEmpty (loginData.Password))
				return false;

			string usernameField = loginData.UsernameField, passwordField = loginData.PasswordField;
			VersionData bvd = null;
			
			if (String.IsNullOrEmpty (usernameField)) {
				bvd = dataManager.VersionData;
				usernameField = bvd.GetLoginVariable ("bugzilla_login");
				if (String.IsNullOrEmpty (usernameField))
					throw new BugzillaException ("Missing bugzilla login form field name 'bugzilla_login'");
			}

			if (String.IsNullOrEmpty (passwordField)) {
				if (bvd == null)
					bvd = dataManager.VersionData;
				passwordField = bvd.GetLoginVariable ("bugzilla_password");
				if (String.IsNullOrEmpty (passwordField))
					throw new BugzillaException ("Missing bugzilla login form field name 'bugzilla_password'.");
			}

			req.Abort ();
			Console.WriteLine ("Attempting to log in at '" + req.Address.ToString () + "'.");
			ASCIIEncoding ascii = new ASCIIEncoding ();
			string postData = usernameField + "=" + loginData.Username + "&" + passwordField + "=" + loginData.Password;
			foreach (KeyValuePair <string, string> kvp in loginData.ExtraData)
				postData += ("&" + kvp.Key + "=" + kvp.Value);

			Console.WriteLine ("Post data: {0}", postData);
			
			byte[] data = ascii.GetBytes (postData);

			Uri address = req.Address;
			UriBuilder formPostUri = new UriBuilder ();
			formPostUri.Scheme = address.Scheme;
			formPostUri.Host = address.Host;
			formPostUri.Path = address.AbsolutePath + loginData.FormActionUrl;
			formPostUri.Query = address.Query;
			
			Console.WriteLine ("Login POST url: {0}", formPostUri.ToString ());
			
			HttpWebRequest request = WebRequest.Create (new Uri (formPostUri.ToString ())) as HttpWebRequest;
			request.Method = "POST";
			request.ContentType="application/x-www-form-urlencoded";
			request.ContentLength = data.Length;
			cookieJar.AddUri (new Uri (request.RequestUri.ToString ()));
			request.CookieContainer = cookieJar;
			
			using (Stream s = request.GetRequestStream ()) {
				s.Write (data, 0, data.Length);
				s.Close ();
			}
			Console.WriteLine ("Data sent.");
			
			HttpWebResponse response = req.GetResponse () as HttpWebResponse;
			Console.WriteLine ("Response: {0}", response.StatusCode);
			
			if (response.StatusCode != HttpStatusCode.OK)
				return false;

			return true;
		}
	}
}
