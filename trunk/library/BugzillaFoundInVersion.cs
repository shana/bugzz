using System;

namespace Bugzz.Bugzilla
{
	internal class BugzillaFoundInVersion : BugzillaInitialValue
	{
		public BugzillaFoundInVersion ()
			: base ()
		{
		}
		
		public BugzillaFoundInVersion (string label, string value)
			: base (label, value)
		{
		}
	}
}
