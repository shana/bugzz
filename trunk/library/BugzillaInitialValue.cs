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
			Label = label ?? String.Empty;
			Value = value ?? String.Empty;
		}

		public override string ToString ()
		{
			return String.Format ("{0} ['{1}':'{2}']", this.GetType (), Label, Value);
		}

		public override int GetHashCode ()
		{
			return this.Value.GetHashCode () ^ this.Label.GetHashCode ();
		}

		public override bool Equals (object that)
		{			
			BugzillaInitialValue biv = that as BugzillaInitialValue;

			if (biv == null)
				return false;

			return (this.Value == biv.Value && this.Label == biv.Label);
		}
	}
}
