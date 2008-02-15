using System;

namespace Bugzz.Bugzilla
{
	internal sealed class Status : InitialValue
	{
		public Status ()
			: base ()
		{
		}
		
		public Status (string label, string value)
			: base (label, value)
		{
		}
	}
}
