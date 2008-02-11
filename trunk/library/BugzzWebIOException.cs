using System;
using Bugzz;

namespace Bugzz.Network
{
	public class BugzzWebIOException : BugzzException
	{
		string fullUrl;
		
		public BugzzWebIOException () :
			base ("Bugzz Web IO exception occurred.")
		{}

		public BugzzWebIOException (string message, string fullUrl, Exception innerException)
			: base (message, innerException)
		{
			this.fullUrl = fullUrl;
		}

		public virtual string FullUrl {
			get { return fullUrl; }
		}

		public override string Message {
			get {
				string fu = FullUrl;
				if (!String.IsNullOrEmpty (fu))
					return base.Message +
						Environment.NewLine +
						Locale.GetText ("URL: ") + fu;
				return base.Message;
			}
		}
	}
}
