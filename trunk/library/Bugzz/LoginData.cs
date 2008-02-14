using System;
using System.Collections.Generic;

namespace Bugzz
{
	public class LoginData
	{
		public Uri Url /*{ get; set; }*/;
		public string UsernameField /*{ get; set; }*/;
		public string PasswordField /*{ get; set; }*/;
		public string Username /*{ get; set; }*/;
		public string Password /*{ get; set; }*/;
		public string FormActionUrl /*{ get; set; }*/;
		
		internal Dictionary <string, string> ExtraData;
		
		public LoginData ()
		{
			ExtraData = new Dictionary <string, string> ();
		}

		public void SetUrl (string url)
		{
			Url = new Uri (url);
		}

		public void AddExtraData (string name, string value)
		{
			if (ExtraData.ContainsKey (name))
				ExtraData [name] = value;
			else
				ExtraData.Add (name, value);
		}
	}
}
