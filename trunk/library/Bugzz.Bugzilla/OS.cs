using System;

namespace Bugzz.Bugzilla
{
	internal sealed class OS : InitialValue
	{
		public OS ()
			: base ()
		{
		}
		
		public OS (string label, string value)
			: base (label, value)
		{
		}
	}
}
