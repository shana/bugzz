using System;

namespace Bugzz.Bugzilla
{
	internal class FoundInVersion : InitialValue
	{
		public FoundInVersion ()
			: base ()
		{
		}
		
		public FoundInVersion (string label, string value)
			: base (label, value)
		{
		}
	}
}
