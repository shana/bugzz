using System;

namespace Bugzz.Bugzilla
{
	internal class BugzillaClassification : BugzillaInitialValue
	{
		public BugzillaClassification ()
			: base ()
		{
		}
		
		public BugzillaClassification (string label, string value)
			: base (label, value)
		{
		}
	}
}
