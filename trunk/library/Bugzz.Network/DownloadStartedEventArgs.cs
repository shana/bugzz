using System;
using System.Net;

namespace Bugzz.Network
{
	public class DownloadStartedEventArgs : EventArgs
	{
		public HttpWebResponse Response {
			get;
			private set;
		}

		internal DownloadStartedEventArgs (HttpWebResponse response)
		{
			Response = response;
		}
	}
}
