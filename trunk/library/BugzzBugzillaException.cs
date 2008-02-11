using System;

using Bugzz;

namespace Bugzz.Bugzilla
{
	public class BugzzBugzillaException : BugzzException
	{
		public BugzzBugzillaException (string message)
			: base (message)
		{
		}

		public BugzzBugzillaException (string message, Exception innerException)
			: base (message, innerException)
		{
		}
	}
}
