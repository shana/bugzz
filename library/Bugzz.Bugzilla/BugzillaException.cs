using System;

using Bugzz;

namespace Bugzz.Bugzilla
{
	public class BugzillaException : BugzzException
	{
		public BugzillaException (string message)
			: base (message)
		{
		}

		public BugzillaException (string message, Exception innerException)
			: base (message, innerException)
		{
		}
	}
}
