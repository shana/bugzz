using System;

namespace Bugzz.Bugzilla
{
	internal sealed class Resolution : InitialValue
	{
		public Resolution ()
			: base ()
		{
		}
		
		public Resolution (string label, string value)
			: base (label, value)
		{
		}
	}
}
