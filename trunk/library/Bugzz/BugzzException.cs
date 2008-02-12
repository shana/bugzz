using System;

namespace Bugzz
{
	public class BugzzException : Exception
	{
		public BugzzException ()
			: base ("Bugzz exception occurred.")
		{
		}

		public BugzzException (string message)
			: base (message)
		{
		}
		
		public BugzzException (string message, Exception innerException)
			: base (message, innerException)
		{
		}
	}
}
