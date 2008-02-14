using System;
using System.Net;

namespace Bugzz
{
	public class DownloadStartedEventArgs : EventArgs
	{
		private Uri uri;
		public Uri Uri {
			get { return uri; }
		}


		long contentLength;
		public long ContentLength
		{
			get { return contentLength; }
		}


		internal DownloadStartedEventArgs (Uri uri, long contentLength)
		{
			this.uri = uri;
			this.contentLength = contentLength;
		}
	}
}
