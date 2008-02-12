using System;
using System.Net;

namespace Bugzz
{
	public class DownloadEndedEventArgs : EventArgs
	{
		public HttpWebResponse Response  /*{
			get;
			private set;
		}*/
		   ;

		internal DownloadEndedEventArgs (HttpWebResponse response)
		{
			Response = response;
		}
	}
}
