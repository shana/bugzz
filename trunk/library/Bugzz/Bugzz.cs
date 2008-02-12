using System;
using System.Collections.Generic;
using System.Text;

namespace Bugzz
{
	public class Bugzz
	{
		Bugzilla.Bugzilla bugzilla;
		public Bugzz ()
		{
			bugzilla = new Bugzilla.Bugzilla ("https://bugzilla.novell.com");
		}

		public List<Bug> Search (Query query)
		{
			string s = query.GetQuery ();
			string url = Bugzilla.Bugzilla.bugzillaData.DefaultVersion.GetUrl ("buglist");
			string ret = Bugzilla.Bugzilla.WebIO.GetDocument (url + s);
			return null;
		}
	}
}
