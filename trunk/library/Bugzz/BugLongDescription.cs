using System;
using System.Text;

namespace Bugzz
{
	public class BugLongDescription
	{
		public bool IsPrivate /* { get; set; } */;
		public DateTime When /* { get; set; } */;
		public string Text /* { get; set; } */;
		
		public BugLongDescription ()
		{
		}

		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder (this.GetType ().ToString () + " [");

			sb.Append (IsPrivate ? "" : "Not " + "Private, ");
			sb.Append ("Created on: " + When.ToString () + "] Text:" + Environment.NewLine);
			sb.Append (Text);

			return sb.ToString ();
		}
	}
}
