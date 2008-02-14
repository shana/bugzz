using System;

namespace Bugzz.Bugzilla
{
	internal sealed class FixedInMilestone : InitialValue
	{
		public FixedInMilestone ()
			: base ()
		{
		}
		
		public FixedInMilestone (string label, string value)
			: base (label, value)
		{
		}
	}
}
