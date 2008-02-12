using System;
using Bugzz;

namespace Bugzz.Network
{
	public class WebIOException : BugzzException
	{
		string fullUrl;
		
		public WebIOException () :
			base ("Bugzz Web IO exception occurred.")
		{}

		public WebIOException (string message, string fullUrl, Exception innerException)
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
