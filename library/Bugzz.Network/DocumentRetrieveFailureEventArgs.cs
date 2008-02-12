using System;
using System.Net;

namespace Bugzz.Network
{
	public class DocumentRetrieveFailureEventArgs : EventArgs
	{
		public HttpWebRequest Request {
			get;
			private set;
		}
		
		internal DocumentRetrieveFailureEventArgs (HttpWebRequest req)
		{
			Request = req;
		}
		
	}
}
