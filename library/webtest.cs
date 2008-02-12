using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

using Bugzz.Bugzilla;
using Bugzz.Network;

class App
{
	static void Main (string[] args)
	{
		Bugzz.BugzzManager bugz = new Bugzz.BugzzManager (args [0]);
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

		bugz.GetBugList (query);
		Console.WriteLine ("Request ended.");
	}
	
}
