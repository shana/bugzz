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
		Bugzilla bugz = new Bugzilla (args [0]);
		bugz.WebIO.DownloadProgress += new DownloadProgressEventHandler (OnDownloadProgress);
		bugz.WebIO.DownloadEnded += new DownloadEndedEventHandler (OnDownloadEnded);
		bugz.WebIO.DownloadStarted += new DownloadStartedEventHandler (OnDownloadStarted);
		bugz.WebIO.DocumentRetrieveFailure += new DocumentRetrieveFailureEventHandler (OnDocumentRetrieveFailure);
		
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
	
	static void OnDownloadProgress (object sender, DownloadProgressEventArgs args)
	{
		ShowProgress (args.CurrentCount, args.MaxCount);
	}

	static void OnDownloadStarted (object sender, DownloadStartedEventArgs args)
	{
		ShowProgress (0, args.Response.ContentLength);
	}
	
	static void OnDownloadEnded (object sender, DownloadEndedEventArgs args)
	{
		long end = args.Response.ContentLength;
		ShowProgress (end, end);
		
		Console.WriteLine ();
		Console.WriteLine ("Download ended. Status: {0}", args.Response.StatusCode);
	}

	static void OnDocumentRetrieveFailure (object sender, DocumentRetrieveFailureEventArgs args)
	{
		Console.WriteLine ();
		HttpWebResponse response = args.Request.GetResponse () as HttpWebResponse;
		
		Console.WriteLine ("Failed to download document. Status: {0}", response != null ? response.StatusCode.ToString () : "unknown");
	}
	
	static void StartRequest (object data)
	{
		Console.WriteLine ("Starting request.");
		Bugzilla bugz = data as Bugzilla;

		if (bugz == null)
			return;

		Dictionary <string, string> vars = new Dictionary <string, string> {
			{"classification", "Mono"},
			{"product", "Mono: Class Libraries"},
			{"component", "Sys.Web"}
		};

		bugz.GetBugList (vars);
		Console.WriteLine ("Request ended.");
	}
	
}
