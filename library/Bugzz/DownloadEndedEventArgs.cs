using System;
using System.Net;

namespace Bugzz
{
	public class DownloadEndedEventArgs : EventArgs
	{
		private Uri uri;
		public Uri Uri {
			get { return uri; }
		}

		long contentLength;
		public long ContentLength {
			get { return contentLength; }
		}

		HttpStatusCode status;
		public HttpStatusCode Status {
			get { return status; }
		}

		internal DownloadEndedEventArgs (Uri uri, long contentLength, HttpStatusCode status)
		{
			this.uri = uri;
			this.status = status;
			this.contentLength = contentLength;
		}
	}
}
