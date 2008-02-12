using System;

namespace Bugzz.Bugzilla
{
	internal class BugzillaComponent : BugzillaInitialValue
	{
		public BugzillaComponent ()
			: base ()
		{
		}
		
		public BugzillaComponent (string label, string value)
			: base (label, value)
		{
		}
	}
}
