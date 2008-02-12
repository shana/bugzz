using System;

namespace Bugzz.Bugzilla
{
	internal class Product : InitialValue
	{
		public Product ()
			: base ()
		{
		}
		
		public Product (string label, string value)
			: base (label, value)
		{
		}
	}
}
