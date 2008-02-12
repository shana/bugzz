using System;

namespace Bugzz.Bugzilla
{
	internal class InitialValue
	{

		private string value;
		public string Value {
			get {return value;}
			private set { this.value = value; }
		}

		private string label;
		public string Label
		{
			get {return label;}
			private set { label = value; }
		}

		public InitialValue ()
		: this (null, null)
		{
		}
		
		public InitialValue (string label, string value)
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
			InitialValue biv = that as InitialValue;

			if (biv == null)
				return false;

			return (this.Value == biv.Value && this.Label == biv.Label);
		}
	}
}
