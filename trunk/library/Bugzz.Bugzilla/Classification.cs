using System;
using System.Collections.Generic;

using C5;

namespace Bugzz.Bugzilla
{
	internal sealed class Classification : InitialValue
	{
		public Classification ()
			: base ()
		{
		}
		
		public Classification (string label, string value)
			: base (label, value)
		{
		}
	}
}
