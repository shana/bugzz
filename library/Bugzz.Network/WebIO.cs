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
			Uri fullUrl;
			
			UriBuilder ub = new UriBuilder (BaseUrl);
			try {
				ub.Path = relativeUrl;
				fullUrl = new Uri (ub.ToString ());

			}
			catch (Exception ex) {
				throw new WebIOException ("Malformed relative URL.", relativeUrl, ex);
			}

			string response;
			Uri redirect;
			bool addressesMatch;

			Console.WriteLine ("Requesting URL: {0}", fullUrl);

			HttpStatusCode status = Get (fullUrl, out response, out addressesMatch, out redirect, false);

			if (status != HttpStatusCode.OK) {
//				OnDocumentRetrieveFailure (req);
				return null;
			}

			if (!addressesMatch) {

				Uri loginAddress = loginData.Url;

				if (loginAddress.Scheme == redirect.Scheme &&
					loginAddress.Host == redirect.Host &&
					loginAddress.AbsolutePath == redirect.AbsolutePath) {

					UriBuilder uri = new UriBuilder ();
					uri.Scheme = redirect.Scheme;
					uri.Host = redirect.Host;
					uri.Path = redirect.AbsolutePath + loginData.FormActionUrl;


					if (LogIn (uri))
						return GetDocument (relativeUrl);
					else
						throw new WebIOException ("Login failure.", redirect.ToString ());
				}
			}

			return response;


			//} catch (BugzzException) {
			//    throw;
			//} catch (WebException ex) {
			//    Console.WriteLine (ex);
			//    //HttpWebResponse exResponse = ex.Response as HttpWebResponse;
			//    //if (exResponse != null && exResponse.StatusCode == HttpStatusCode.NotModified)
			//    //    OnDownloadEnded (exResponse);
			//    //else
			//    //    OnDocumentRetrieveFailure (req);
				
			//    return null;
			//} catch (Exception ex) {
			//    Console.WriteLine (ex);
			//    throw new WebIOException ("Error downloading document.", fullUrl, ex);
			//}
		}

		bool LogIn (UriBuilder formPostUri)
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

			Console.WriteLine ("Attempting to log in ");//at '" + req.Address.ToString () + "'.");


			string postData = usernameField + "=" + loginData.Username + "&" + passwordField + "=" + loginData.Password;
			
			foreach (KeyValuePair <string, string> kvp in loginData.ExtraData)
				postData += ("&" + kvp.Key + "=" + kvp.Value);

			Console.WriteLine ("Login POST url: {0}", formPostUri.ToString ());
			Console.WriteLine ("Post data: {0}", postData);

			string response;
			HttpStatusCode status = Post (new Uri (formPostUri.ToString ()), postData, out response, true);
			if (status == HttpStatusCode.OK)
				return true;
			
			
			return false;
		}


		private HttpStatusCode Get (Uri uri, out string response, out bool addressesMatch, out Uri redirectAddress, bool ignoreResponse)
		{
			HttpStatusCode statusCode = HttpStatusCode.NotFound;
			cookieJar.AddUri (uri);
			HttpWebRequest request = WebRequest.Create (uri) as HttpWebRequest;
			request.Method = "GET";
			request.UserAgent = userAgent;
			request.CookieContainer = cookieJar;

			try {
				HttpWebResponse resp = request.GetResponse () as HttpWebResponse;
				statusCode = resp.StatusCode;

				addressesMatch = (request.RequestUri == request.Address);
				redirectAddress = request.Address;

				StringBuilder sb = new StringBuilder ();

				if (!ignoreResponse) {
					Stream d = resp.GetResponseStream ();
					char[] buffer = new char[4096];
					int bufferLen = buffer.Length;
					int charsRead = -1;
					long count;

					using (StreamReader reader = new StreamReader (d)) {
						count = 0;
//						OnDownloadStarted (response);
						long contentLength = resp.ContentLength;
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
//						OnDownloadEnded (response);
					}
				}

				cookieJar.Save ();
				resp.Close ();

				Console.WriteLine (sb.ToString ());

				response = sb.ToString ();

			}
			catch (HttpListenerException ex) {
				response = String.Empty;
				redirectAddress = null;
				addressesMatch = false;
			}
			return statusCode;
		}

		private HttpStatusCode Post (Uri uri, string postData, out string response, bool ignoreResponse)
		{
			HttpStatusCode statusCode = HttpStatusCode.NotFound;
			ASCIIEncoding ascii = new ASCIIEncoding ();
			byte[] data = ascii.GetBytes (postData);

			cookieJar.AddUri (uri);
			HttpWebRequest request = WebRequest.Create (uri) as HttpWebRequest;
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = data.Length;
			request.UserAgent = userAgent;
			request.CookieContainer = cookieJar;

			Stream s = request.GetRequestStream ();
			s.Write (data, 0, data.Length);
			s.Close ();

			try {
				HttpWebResponse resp = request.GetResponse () as HttpWebResponse;
				statusCode = resp.StatusCode;

				
				StringBuilder sb = new StringBuilder ();

				if (!ignoreResponse) {
					Stream d = resp.GetResponseStream ();
					char[] buffer = new char[4096];
					int bufferLen = buffer.Length;
					int charsRead = -1;
					long count;

					using (StreamReader reader = new StreamReader (d)) {
						count = 0;
						long contentLength = resp.ContentLength;
						if (contentLength == -1)
							contentLength = Int64.MaxValue; // potentially
						// dangerous

						while (count < contentLength) {
							charsRead = reader.Read (buffer, 0, bufferLen);
							if (charsRead == 0)
								break;

							count += charsRead;
							sb.Append (buffer, 0, charsRead);
						}
					}
				}

				cookieJar.Save ();
				resp.Close ();

				Console.WriteLine (sb.ToString ());

				response = sb.ToString ();

			}
			catch (HttpListenerException ex) {
				response = String.Empty;
			}
			return statusCode;
		}
	}
}
