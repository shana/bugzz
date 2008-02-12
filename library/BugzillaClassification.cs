using System;
using System.Collections.Generic;

using C5;

namespace Bugzz.Bugzilla
{
	internal sealed class BugzillaClassification : BugzillaInitialValue
	{
		HashBag <BugzillaProduct> products = new HashBag <BugzillaProduct> ();
		
		public BugzillaClassification ()
			: base ()
		{
		}
		
		public BugzillaClassification (string label, string value)
			: base (label, value)
		{
		}

		public void Add (BugzillaProduct product)
		{
		}
	}
}
