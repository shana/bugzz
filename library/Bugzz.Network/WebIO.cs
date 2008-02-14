using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

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

		private Uri baseUrl;
		public Uri BaseUrl {
			get { return baseUrl; }
			set { baseUrl = value; }
		}

		public long MaxRequestAttempts;// { get; set; }
		public long MaxLoginAttempts;// { get; set; }
			
		static WebIO ()
		{
			// TODO: construct something funnier later on
			userAgent = global::Bugzz.Constants.Package + "/" + global::Bugzz.Constants.Version;
		}
		
		public WebIO (string baseUrl, LoginData loginData, DataManager dataManager)
		{
			MaxRequestAttempts = 3;
			MaxLoginAttempts = 3;
			
			this.loginData = loginData;
			this.dataManager = dataManager;
			
			if (String.IsNullOrEmpty (baseUrl))
				throw new ArgumentNullException ("Base request URL must be specified.", "baseUrl");
			
			try {
				this.baseUrl = new Uri (baseUrl);
			} catch (Exception ex) {
				throw new ArgumentException ("Invalid base URL.", "baseUrl", ex);
			}

			cookieJar = new CookieManager ();
		}

		void OnDocumentRetrieveFailure (Uri uri, HttpStatusCode status)
		{
			if (DocumentRetrieveFailure == null)
				return;

			DocumentRetrieveFailure (new DocumentRetrieveFailureEventArgs (uri, status));
		}

		void OnDownloadStarted (Uri uri, long contentLength)
		{
			if (DownloadStarted == null)
				return;

			DownloadStarted (new DownloadStartedEventArgs (uri, contentLength));
		}
		
		void OnDownloadEnded (Uri uri, long contentLength, HttpStatusCode status)
		{
			if (DownloadEnded == null)
				return;

			DownloadEnded (new DownloadEndedEventArgs (uri, contentLength, status));
		}

		void OnDownloadProgress (long maxCount, long curCount)
		{
			if (DownloadProgress == null)
				return;

			DownloadProgress (new DownloadProgressEventArgs (maxCount, curCount));
		}
		
		// TODO: this method should retry to retrieve the document a configurable amount of
		// times before returning an error.
		public string GetDocument (string relativeUrl, string expectedContentType, Regex fallbackTypeRegex)
		{
			Uri fullUrl;
			
			UriBuilder ub = new UriBuilder (BaseUrl);
			try {
				ub.Path = relativeUrl;
				fullUrl = new Uri (ub.ToString ());
			} catch (Exception ex) {
				throw new WebIOException ("Malformed relative URL.", relativeUrl, ex);
			}

			string response;
			Uri redirect;
			bool addressesMatch;
			long attempts = MaxRequestAttempts;
			long loginAttempts = MaxLoginAttempts;
			HttpStatusCode status = HttpStatusCode.BadRequest;
			bool loggedIn = false;
			
			Console.WriteLine ("Requesting URL: {0}", fullUrl);

			while (attempts > 0) {
				try {
					status = Get (fullUrl, expectedContentType, fallbackTypeRegex, out response, out addressesMatch, out redirect, false);

					if (status != HttpStatusCode.OK) {
						attempts--;
						continue;
					}
				
					if (!addressesMatch) {
						if (loginAttempts <= 0)
							throw new WebIOException ("Login failure.", redirect.ToString ());
						
						loggedIn = false;
						Uri loginAddress = loginData.Url;

						loginAttempts--;
						if (loginAddress.Scheme == redirect.Scheme &&
						    loginAddress.Host == redirect.Host &&
						    loginAddress.AbsolutePath == redirect.AbsolutePath) {
							UriBuilder uri = new UriBuilder ();
							uri.Scheme = redirect.Scheme;
							uri.Host = redirect.Host;
							uri.Path = redirect.AbsolutePath + loginData.FormActionUrl;
							Console.WriteLine ("Login attempts left: {0}", loginAttempts);
							LogIn (uri);
							continue;
						}
					} else
						loggedIn = true;
					
					if (!loggedIn)
						if (loginAttempts <= 0)
							throw new WebIOException ("Login failure.", redirect.ToString ());
						else {
							loginAttempts--;
							continue;
						}
					
					return response;
				} catch (BugzzException) {
					throw;
				} catch (Exception ex) {
					throw new WebIOException ("Error downloading document.", fullUrl.ToString (), ex);
				} finally {
					if (status != HttpStatusCode.OK)
						OnDocumentRetrieveFailure (fullUrl, status);
				}
			};
			
			return null;
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

		bool MatchingContentType (string contentType, string response, string expectedContentType, Regex fallbackTypeRegex)
		{
			Console.WriteLine ("{0}.MatchingContentType ()", this);
			Console.WriteLine ("\tcontent type: {0}", contentType);
			Console.WriteLine ("\texpected: {0}", expectedContentType);
			
			if (String.IsNullOrEmpty (contentType) && !String.IsNullOrEmpty (expectedContentType)) {
				if (fallbackTypeRegex == null || response == null)
					return false;
				return fallbackTypeRegex.IsMatch (response);
			}

			if (contentType.StartsWith (expectedContentType))
				return true;
			
			return false;
		}
		
		private HttpStatusCode Get (Uri uri, string expectedContentType, Regex fallbackTypeRegex,
					    out string response, out bool addressesMatch, out Uri redirectAddress, bool ignoreResponse)
		{
			long contentLength = -1;

			HttpStatusCode statusCode = HttpStatusCode.NotFound;
			cookieJar.AddUri (uri);
			HttpWebRequest request = WebRequest.Create (uri) as HttpWebRequest;
			HttpWebResponse resp = null;
			request.Method = "GET";
			request.UserAgent = userAgent;
			request.CookieContainer = cookieJar;

			response = null;
			
			try {
				resp = request.GetResponse () as HttpWebResponse;
				statusCode = resp.StatusCode;

				Console.WriteLine ("GET: content type == '" + resp.ContentType + "'");
				addressesMatch = (request.Address == uri);
				Console.WriteLine ("GET:addressesMatch = " + addressesMatch); 
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
						
						contentLength = resp.ContentLength;
						OnDownloadStarted (uri, contentLength);
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
						OnDownloadEnded (uri, contentLength, statusCode);
					}
				}

				//Console.WriteLine ("----GET----");
				//Console.WriteLine (sb.ToString ());
				//Console.WriteLine ("----END OF GET----");

				response = sb.ToString ();
			} catch (WebException ex) {
				Console.WriteLine ("GET WebException");
				Exception e = ex;
				while (e != null) {
					Console.WriteLine (e.Message);
					Console.WriteLine (e.StackTrace);
					e = e.InnerException;
				}

				response = null;
				redirectAddress = null;
				addressesMatch = true;
				if (ex.Response != null) {
					try {
						HttpWebResponse exResponse = ex.Response as HttpWebResponse;
						statusCode = exResponse.StatusCode;
					} catch {
						statusCode = HttpStatusCode.BadRequest;
					}
				}
				if (statusCode == HttpStatusCode.NotModified) {
					statusCode = HttpStatusCode.OK;
					OnDownloadEnded (uri, contentLength, statusCode);
				} else {
					OnDocumentRetrieveFailure (uri, statusCode);
					throw new WebIOException ("Request failed.", uri.ToString (), ex);
				}
			} catch (Exception ex) {
				Console.WriteLine ("GET Exception");
				Exception e = ex;
				while (e != null) {
					Console.WriteLine (e.Message);
					Console.WriteLine (e.StackTrace);
					e = e.InnerException;
				}
				
				response = null;
				redirectAddress = null;
				addressesMatch = false;

				throw new WebIOException ("Request failed.", uri.ToString (), ex);
			} finally {
				cookieJar.Save ();
				if (resp != null) {
					if (!MatchingContentType (resp.ContentType, response, expectedContentType, fallbackTypeRegex))
						response = null;
					
					resp.Close ();
				}
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
			
			HttpWebResponse resp = null;			
			try {
				Stream s = request.GetRequestStream ();
				s.Write (data, 0, data.Length);
				s.Close ();

				resp = request.GetResponse () as HttpWebResponse;
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
				
				response = sb.ToString ();
			} catch (Exception ex) {
				Console.WriteLine ("POST Exception");
				Exception e = ex;
				while (e != null) {
					Console.WriteLine (e.Message);
					Console.WriteLine (e.StackTrace);
					e = e.InnerException;
				}
				response = String.Empty;
				throw new WebIOException ("Request failed.", uri.ToString (), ex);
			} finally {
				cookieJar.Save ();
				if (resp != null)
					resp.Close ();
			}
			
			return statusCode;
		}
	}
}
