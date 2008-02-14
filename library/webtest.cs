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
		loginData.AddExtraData ("nlogin_submit_btn", "Log in");
		
		Bugzz.BugzzManager bugz = new Bugzz.BugzzManager (args [0], loginData);
		bugz.AddCallback (new Bugzz.DownloadProgressEventHandler (OnDownloadProgress));
		bugz.AddCallback (new Bugzz.DownloadEndedEventHandler (OnDownloadEnded));
		bugz.AddCallback (new Bugzz.DownloadStartedEventHandler (OnDownloadStarted));
		bugz.AddCallback (new Bugzz.DocumentRetrieveFailureEventHandler (OnDocumentRetrieveFailure));
		
		Thread t = new Thread (App.StartRequest);
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
	
	static void OnDownloadProgress (object sender, Bugzz.DownloadProgressEventArgs args)
	{
		ShowProgress (args.CurrentCount, args.MaxCount);
	}

	static void OnDownloadStarted (object sender, Bugzz.DownloadStartedEventArgs args)
	{
		ShowProgress (0, args.Response.ContentLength);
	}
	
	static void OnDownloadEnded (object sender, Bugzz.DownloadEndedEventArgs args)
	{
		long end = args.Response.ContentLength;
		ShowProgress (end, end);
		
		Console.WriteLine ();
		Console.WriteLine ("Download ended. Status: {0}", args.Response.StatusCode);
	}

	static void OnDocumentRetrieveFailure (object sender, Bugzz.DocumentRetrieveFailureEventArgs args)
	{
		Console.WriteLine ();
		HttpWebResponse response = args.Request.GetResponse () as HttpWebResponse;
		
		Console.WriteLine ("Failed to download document. Status: {0}", response != null ? response.StatusCode.ToString () : "unknown");
	}
	
	static void StartRequest (object data)
	{
		Console.WriteLine ("Starting request.");
		Bugzz.BugzzManager bugz = data as Bugzz.BugzzManager;

		if (bugz == null)
			return;

		Bugzz.Query query = new Bugzz.Query ();
		query.AddQueryData ("classification", "Mono");
		query.AddQueryData ("product", "Mono: Class Libraries");
		query.AddQueryData ("component", "Sys.Web");
		query.AddQueryData ("GoAheadAndLogIn", "1");

		var results = bugz.GetBugList (query);
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
		
		var results2 = bugz.GetBugs (query);

		foreach (Bugzz.Bug bug in results2.Values) {
			Console.WriteLine (bug.ToString (true));
			Console.WriteLine ();
		}
	}
	
}
