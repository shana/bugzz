using System;
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
		
		Thread t = new Thread (App.StartRequest);
		Console.WriteLine ("Starting thread.");
		t.Start (bugz);
		Console.WriteLine ("Waiting for thread.");
		t.Join ();
		Console.WriteLine ("Refresh finished.");
	}

	static void OnDownloadProgress (object sender, DownloadProgressEventArgs args)
	{
		Console.Write ("\rCompleted: {0:F2}%", ((double)args.CurrentCount / (double)args.MaxCount) * 100);
	}

	static void OnDownloadEnded (object sender, DownloadEndedEventArgs args)
	{
		Console.WriteLine ();
		Console.WriteLine ("Download ended. Status: {0}", args.Response.StatusCode);
	}
	
	static void StartRequest (object data)
	{
		Console.WriteLine ("Starting request.");
		Bugzilla bugz = data as Bugzilla;

		if (bugz == null)
			return;
		
		bugz.Refresh ();
		Console.WriteLine ("Request ended.");
	}
	
}
