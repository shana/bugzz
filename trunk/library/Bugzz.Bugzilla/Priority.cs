using System;

namespace Bugzz.Bugzilla
{
	internal sealed class Priority : InitialValue
	{
		public Priority ()
			: base ()
		{
		}
		
		public Priority (string label, string value)
			: base (label, value)
		{
		}
	}
}
