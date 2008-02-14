using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Bugzz
{
	public class BugzzManager
	{
		Bugzilla.Bugzilla bugzilla;

		public string Url
		{
			get { return bugzilla.BaseUrl; }
			set { bugzilla.BaseUrl = value; }
		}

		public BugzzManager (LoginData loginData)
		{
			bugzilla = new Bugzilla.Bugzilla (loginData);
		}

		public BugzzManager (string bugsiteBase)
		: this (bugsiteBase, null)
		{
		}
		
		public BugzzManager (string bugsiteBase, LoginData loginData)
		{
			bugzilla = new Bugzilla.Bugzilla (bugsiteBase, loginData);
		}

		public event DocumentRetrieveFailureEventHandler DocumentRetrieveFailure
		{
			add { bugzilla.WebIO.DocumentRetrieveFailure += value; }
			remove { bugzilla.WebIO.DocumentRetrieveFailure -= value; }
		}

		public event DownloadStartedEventHandler DownloadStarted
		{
			add { bugzilla.WebIO.DownloadStarted += value; }
			remove { bugzilla.WebIO.DownloadStarted -= value; }
		}

		public event DownloadEndedEventHandler DownloadEnded
		{
			add { bugzilla.WebIO.DownloadEnded += value; }
			remove { bugzilla.WebIO.DownloadEnded -= value; }
		}

		public event DownloadProgressEventHandler DownloadProgress
		{
			add { bugzilla.WebIO.DownloadProgress += value; }
			remove { bugzilla.WebIO.DownloadProgress -= value; }
		}

		public Dictionary <string, Bug> Search (Query query)
		{
			return bugzilla.GetBugList (query);
		}

		public Dictionary <string, Bug> GetBugList (Query query)
		{
			return bugzilla.GetBugList (query);
		}

		public Dictionary <string, Bug> GetBugs (Query query)
		{
			return bugzilla.GetBugs (query);
		}
	}
}
