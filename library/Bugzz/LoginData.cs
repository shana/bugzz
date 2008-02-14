using System;

namespace Bugzz
{
	public class LoginData
	{
		public Uri Url /*{ get; set; }*/;
		public string UsernameField /*{ get; set; }*/;
		public string PasswordField /*{ get; set; }*/;
		public string Username /*{ get; set; }*/;
		public string Password /*{ get; set; }*/;

		public LoginData ()
		{
		}

		public void SetUrl (string url)
		{
			Url = new Uri (url);
		}
	}
}
