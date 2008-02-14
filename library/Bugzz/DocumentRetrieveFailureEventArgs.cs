using System;
using System.Net;

namespace Bugzz
{
	public class DocumentRetrieveFailureEventArgs : EventArgs
	{

		private Uri uri;
		public Uri Uri {
			get { return uri; }
		}

		HttpStatusCode status;
		public HttpStatusCode Status
		{
			get { return status; }
		}

		internal DocumentRetrieveFailureEventArgs (Uri uri, HttpStatusCode status)
		{
			this.uri = uri;
			this.status = status;
		}
		
	}
}
