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
using System.Net;
using System.Threading;

using Bugzz;

class App
{
	static void Main (string[] args)
	{
		int len = args.Length;

		if (len == 0) {
			Console.Error.WriteLine ("Usage: webtest BUGZILLA_URL [LOGIN_URL [USERNAME [PASSWORD]]]");
			return;
		}
		
		LoginData loginData = new LoginData ();

		if (args.Length > 1)
			loginData.SetUrl (args [1]);
		if (args.Length > 2)
			loginData.Username = args [2];
		if (args.Length > 3)
			loginData.Password = args [3];
		loginData.UsernameField="username";
		loginData.PasswordField="password";
		loginData.FormActionUrl="auth-up";
		loginData.AddExtraData ("url", "https://bugzilla.novell.com/ichainlogin.cgi?target=query.cgi?format%3Dadvanced%26field0-0-0%3Dxyzzy%26GoAheadAndLogIn%3D1");
		//loginData.AddExtraData ("nlogin_submit_btn", "Log in");
		
		Bugzz.BugzzManager bugz = new Bugzz.BugzzManager (args [0], loginData);
		bugz.DownloadProgress += delegate (Bugzz.DownloadProgressEventArgs e) {
			ShowProgress (e.CurrentCount, e.MaxCount);
		};

		bugz.DownloadEnded += delegate (Bugzz.DownloadEndedEventArgs e) {
			ShowProgress (e.ContentLength, e.ContentLength);

			Console.WriteLine ();
			Console.WriteLine ("Download ended. Status: {0}", e.Status);
		};
		bugz.DownloadStarted += delegate (Bugzz.DownloadStartedEventArgs e) {
			ShowProgress (0, e.ContentLength);
		};
		bugz.DocumentRetrieveFailure += delegate (Bugzz.DocumentRetrieveFailureEventArgs e) {
			Console.WriteLine ();
			Console.WriteLine ("Failed to download document. Status: {0}", e.Status.ToString ());
		};
		
		Thread t = new Thread (StartRequest);
		Console.WriteLine ("Starting thread.");
		t.Start (bugz);
		Console.WriteLine ("Waiting for thread.");
		t.Join ();
		Console.WriteLine ("Refresh finished.");
	}

	static void ShowProgress (long start, long end)
	{
		Console.Write ("\rCompleted: {0:F2}%", ((double)start / (double)end) * 100);
	}

	static void StartRequest (object data)
	{
		Console.WriteLine ("Starting request.");
		Bugzz.BugzzManager bugz = data as Bugzz.BugzzManager;

		if (bugz == null)
			return;

		Bugzz.Query query = new Bugzz.Query ();
		//query.Email = "avidigal@novell.com";
		query.AddQueryData ("classification", "Mono");
		query.AddQueryData ("product", "Mono: Class Libraries");
		query.AddQueryData ("component", "Sys.Web");
		query.AddQueryData ("GoAheadAndLogIn", "1");

		Dictionary<string, Bug> results = bugz.GetBugList (query);
		if (results == null)
			return;
		
		Bugzz.Bug bug1 = null, bug2 = null;
		
		Console.WriteLine ("List of retrieved bugs:");
		foreach (Bugzz.Bug bug in results.Values) {
			if (bug1 == null)
				bug1 = bug;
			else if (bug2 == null)
				bug2 = bug;
			
			Console.WriteLine (bug);
		}

		if (bug1 == null)
			return;
		
		Console.WriteLine ("Sample bugs:");
		query = new Bugzz.Query ();
		query.AddQueryData ("id", bug1.ID);

		if (bug2 != null)
			query.AddQueryData ("id", bug2.ID);

		Dictionary<string, Bug> results2 = bugz.GetBugs (query);

		foreach (Bugzz.Bug bug in results2.Values) {
			Console.WriteLine (bug.ToString (true));
			Console.WriteLine ();
		}
	}
	
}
