using System;

namespace Bugzz
{
	public interface IInitialValue
	{
		string Value { get; }
		string Label { get; }
		void Set (string label, string value);
		string ToString ();
		int GetHashCode ();
		bool Equals (object that);
	}
}
