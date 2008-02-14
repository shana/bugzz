using System;

namespace Bugzz.Bugzilla
{
	internal sealed class Component : InitialValue
	{
		public Component ()
			: base ()
		{
		}
		
		public Component (string label, string value)
			: base (label, value)
		{
		}
	}
}
