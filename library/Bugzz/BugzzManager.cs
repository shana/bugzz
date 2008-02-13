using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Bugzz
{
	public class BugzzManager
	{
		Bugzilla.Bugzilla bugzilla;

		public BugzzManager (string bugsiteBase)
		{
			bugzilla = new Bugzilla.Bugzilla (bugsiteBase);
		}

		public void AddCallback (DocumentRetrieveFailureEventHandler cb)
		{
			bugzilla.WebIO.DocumentRetrieveFailure += cb;
		}
		
		public void AddCallback (DownloadStartedEventHandler cb)
		{
			bugzilla.WebIO.DownloadStarted += cb;
		}
		
		public void AddCallback (DownloadEndedEventHandler cb)
		{
			bugzilla.WebIO.DownloadEnded += cb;
		}
		
		public void AddCallback (DownloadProgressEventHandler cb)
		{
			bugzilla.WebIO.DownloadProgress += cb;
		}
		
		public Dictionary <string, Bug> Search (Query query)
		{
			return bugzilla.GetBugList (query);
		}

		public Dictionary <string, Bug> GetBugList (Query query)
		{
			return bugzilla.GetBugList (query);
		}
	}
}
