using System;
using System.Collections.Generic;

namespace Bugzz
{
	public class LoginData
	{
#if FALLBACK
		Uri url;
		public Uri Url
		{
			get { return url; }
			set { url = value; }
		}

		string usernameField;
		public string UsernameField
		{
			get { return usernameField; }
			set { usernameField = value; }
		}

		string passwordField;
		public string PasswordField
		{
			get { return passwordField; }
			set { passwordField = value; }
		}

		string username;
		public string Username
		{
			get { return username; }
			set { username = value; }
		}

		string password;
		public string Password
		{
			get { return password; }
			set { password = value; }
		}

		string formActionUrl;
		public string FormActionUrl
		{
			get { return formActionUrl; }
			set { formActionUrl = value; }
		}

		Dictionary<string, string> extraData;
		public Dictionary<string, string> ExtraData
		{
			get { return extraData; }
			set { extraData = value; }
		}

#else
		public Uri Url { get; set; }
		public string UsernameField { get; set; }
		public string PasswordField { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string FormActionUrl { get; set; }
		public Dictionary <string, string> ExtraData { get; set; }
#endif	
				
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
