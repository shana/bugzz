using System;

namespace Bugzz.Bugzilla
{
	internal class Component : InitialValue
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
