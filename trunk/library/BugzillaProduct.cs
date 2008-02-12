using System;

namespace Bugzz.Bugzilla
{
	internal class BugzillaProduct : BugzillaInitialValue
	{
		public BugzillaProduct ()
			: base ()
		{
		}
		
		public BugzillaProduct (string label, string value)
			: base (label, value)
		{
		}
	}
}
