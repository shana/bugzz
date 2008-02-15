using System;

namespace Bugzz.Bugzilla
{
	internal sealed class Severity : InitialValue
	{
		public Severity ()
			: base ()
		{
		}
		
		public Severity (string label, string value)
			: base (label, value)
		{
		}
	}
}
