using System;
using System.IO;
using System.Net;
using System.Text;

namespace Bugzz.Network
{
	public class WebIO
	{
		readonly static string userAgent;

		public event DocumentRetrieveFailureEventHandler DocumentRetrieveFailure;
		public event DownloadEndedEventHandler DownloadEnded;
		public event DownloadProgressEventHandler DownloadProgress;
		
		Uri baseUrl;

		static WebIO ()
		{
			// TODO: construct something funnier later on
			userAgent = Bugzz.Constants.Package + "/" + Bugzz.Constants.Version;
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

		void OnDocumentRetrieveFailure (HttpWebRequest req)
		{
			if (DocumentRetrieveFailure == null)
				return;

			DocumentRetrieveFailure (this, new DocumentRetrieveFailureEventArgs (req));
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
				UriBuilder ub = new UriBuilder (baseUrl);
				ub.Path = relativeUrl;
				fullUrl = ub.ToString ();
				req = WebRequest.Create (new Uri (fullUrl)) as HttpWebRequest;
			} catch (Exception ex) {
				throw new BugzzWebIOException ("Malformed relative URL.", relativeUrl, ex);
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
				
				StringBuilder sb = new StringBuilder ();
				Stream data = response.GetResponseStream ();
				char[] buffer = new char [4096];
				int bufferLen = buffer.Length;
				int charsRead;
				long count;
				
				using (StreamReader reader = new StreamReader (data)) {
					count = 0;

					while (count < response.ContentLength) {
						charsRead = reader.Read (buffer, 0, bufferLen);
						if (charsRead == 0)
							break;

						count += charsRead;
						OnDownloadProgress (response.ContentLength, count);
						sb.Append (buffer, 0, charsRead);
					}
					OnDownloadEnded (response);
				}

				return sb.ToString ();
			} catch (WebException ex) {
				HttpWebResponse exResponse = ex.Response as HttpWebResponse;
				if (exResponse != null && exResponse.StatusCode == HttpStatusCode.NotModified)
					OnDownloadEnded (exResponse);
				else
					OnDocumentRetrieveFailure (req);
				
				return null;
			} catch (Exception ex) {
				throw new BugzzWebIOException ("Error downloading document.", fullUrl, ex);
			}
		}
	}
}
