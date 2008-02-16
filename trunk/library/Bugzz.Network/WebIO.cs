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
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using Bugzz;
using Bugzz.Bugzilla;
using HtmlAgilityPack;

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
			
			if (!String.IsNullOrEmpty (baseUrl)) {
			
				try {
					this.baseUrl = new Uri (baseUrl);
				} catch (Exception ex) {
					throw new ArgumentException ("Invalid base URL.", "baseUrl", ex);
				}
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
		
		public string PostDocument (string relativeUrl, string expectedResponseType, Regex fallbackTypeRegex)
		{
			return null;
		}
		
		public string GetDocument (string relativeUrl, string expectedContentType, Regex fallbackTypeRegex)
		{
			Uri baseUrl = BaseUrl;
			if (baseUrl == null)
				return null;
			
			Uri fullUrl;
			UriBuilder ub = new UriBuilder (baseUrl);
			
			try {
				ub.Path = relativeUrl;
				fullUrl = new Uri (ub.ToString ());
			} catch (Exception ex) {
				throw new WebIOException ("Malformed relative URL.", relativeUrl, ex);
			}

			string response;
			Uri redirect;
			bool addressesMatch, standardLogin;
			long attempts = MaxRequestAttempts;
			long loginAttempts = MaxLoginAttempts;
			HttpStatusCode status = HttpStatusCode.BadRequest;
			bool loggedIn = false;
			
			Console.WriteLine ("Requesting URL: {0}", fullUrl);

			while (attempts > 0) {
				try {
					status = Get (fullUrl, expectedContentType, fallbackTypeRegex, out response, out addressesMatch,
						      out standardLogin, out redirect, false);

					if (status != HttpStatusCode.OK) {
						attempts--;
						continue;
					}
				
					if (!addressesMatch) {
						if (loginAttempts <= 0)
							throw new WebIOException ("Login failure.", redirect.ToString ());
						
						loggedIn = false;
						Uri loginAddress = loginData.Url;
						Console.WriteLine ("loginAddress == {0}", loginAddress);
						
						loginAttempts--;
						if (redirect != null && loginAddress.Scheme == redirect.Scheme &&
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
					} else if (standardLogin) {
						Console.WriteLine ("Standard login found. Will POST to '{0}'", redirect.ToString ());
						LogIn (new UriBuilder (redirect));
					} else
						loggedIn = true;
					
					if (!loggedIn)
						if (loginAttempts <= 0)
							throw new WebIOException ("Login failure.", redirect != null ? redirect.ToString () : String.Empty);
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

			List <string> typeStrings = dataManager.GetMimeType (expectedContentType);
			Console.WriteLine ("typeStrings == {0}", typeStrings);
			
			if (String.IsNullOrEmpty (contentType) || typeStrings == null) {
				if (fallbackTypeRegex == null || response == null)
					return false;
				return fallbackTypeRegex.IsMatch (response);
			}

			foreach (string type in typeStrings) {
				Console.WriteLine ("Type: {0}", type);
				if (contentType.StartsWith (type))
					return true;
			}
			
			return false;
		}

		bool HasLoginForm (HttpWebRequest request, string response, out Uri action)
		{
			if (String.IsNullOrEmpty (response)) {
				action = null;
				return false;
			}
			
			HtmlDocument doc = new HtmlDocument ();
			try {
				doc.LoadHtml (response);
			} catch {
				// failed, not html - no form
				action = null;
				return false;
			}

			HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes ("//form[string-length (@name) > 0 and string-length (@action) > 0]");
			if (nodes == null || nodes.Count == 0) {
				action = null;
				return false;
			}
			
			VersionData bvd = dataManager.VersionData;
			string usernameField = bvd.GetLoginVariable ("bugzilla_login");
			if (String.IsNullOrEmpty (usernameField))
					throw new BugzillaException ("Missing bugzilla login form field name 'bugzilla_login'");
			
			string passwordField = bvd.GetLoginVariable ("bugzilla_password");
			if (String.IsNullOrEmpty (passwordField))
					throw new BugzillaException ("Missing bugzilla login form field name 'bugzilla_password'.");

			HtmlAttributeCollection attributes;
			foreach (HtmlNode node in nodes) {
				attributes = node.Attributes;
				if (attributes ["name"].Value != "login")
					continue;

				string actionValue = attributes ["action"].Value;
				Uri actionUri = new Uri (actionValue, UriKind.RelativeOrAbsolute);
				if (actionUri.IsAbsoluteUri) {
					action = actionUri;
					return true;
				}				

				UriBuilder ub = new UriBuilder (request.Address);
				ub.Query = null;
				ub.Fragment = null;
				
				string path = ub.Path;
				int idx;
				if (!path.EndsWith ("/")) {
					idx = path.LastIndexOf ("/");
					if (idx > -1)
						path = path.Substring (0, idx + 1);
					else
						path += "/";
				}
				
				idx = actionValue.IndexOf ("?");
				if (idx > -1)
					path += actionValue.Substring (0, idx + 1);
				else
					path += actionValue;
				
				ub.Path = path;
				action = ub.Uri;
				return true;
			}

			action = null;
			return false;
		}
		
		private HttpStatusCode Get (Uri uri, string expectedContentType, Regex fallbackTypeRegex,
					    out string response, out bool addressesMatch, out bool standardLogin, out Uri redirectAddress, bool ignoreResponse)
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
			standardLogin = false;
			
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
					Uri loginFormRedirect;
					
					if (HasLoginForm (request, response, out loginFormRedirect)) {
						standardLogin = true;
						response = null;
						redirectAddress = loginFormRedirect;
					} else if (!MatchingContentType (resp.ContentType, response, expectedContentType, fallbackTypeRegex))
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
				using (Stream s = request.GetRequestStream ()) {
					s.Write (data, 0, data.Length);
				}

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
