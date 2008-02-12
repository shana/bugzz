using System;

namespace Bugzz
{
	public class DownloadProgressEventArgs : EventArgs
	{
		public long MaxCount {
			get;
			private set;
		}

		public long CurrentCount {
			get;
			private set;
		}
		
		internal DownloadProgressEventArgs (long maxCount, long currentCount)
		{
			MaxCount = maxCount;
			CurrentCount = currentCount;
		}
	}
}
