using System;

namespace Bugzz.Bugzilla
{
	internal sealed class Hardware : InitialValue
	{
		public Hardware ()
			: base ()
		{
		}
		
		public Hardware (string label, string value)
			: base (label, value)
		{
		}
	}
}
