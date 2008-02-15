//
// Bugzz - Multi GUI Desktop Bugzilla Client
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2008 Novell, Inc.
//
// Authors:
//	Andreia Gaita (avidigal@novell.com)
//	Marek Habersack (mhabersack@novell.com)
//
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
