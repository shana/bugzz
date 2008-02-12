using System;

namespace Bugzz.Bugzilla
{
	internal class BugzillaInitialValue
	{
		public string Value {
			get;
			private set;
		}

		public string Label {
			get;
			private set;
		}

		public BugzillaInitialValue ()
		: this (null, null)
		{
		}
		
		public BugzillaInitialValue (string label, string value)
		{
			Set (label, value);
		}

		public void Set (string label, string value)
		{
			Label = label;
			Value = value;
		}
		
	}
}
