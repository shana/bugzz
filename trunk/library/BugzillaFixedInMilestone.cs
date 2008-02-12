using System;

namespace Bugzz.Bugzilla
{
	internal class BugzillaFixedInMilestone : BugzillaInitialValue
	{
		public BugzillaFixedInMilestone ()
			: base ()
		{
		}
		
		public BugzillaFixedInMilestone (string label, string value)
			: base (label, value)
		{
		}
	}
}
