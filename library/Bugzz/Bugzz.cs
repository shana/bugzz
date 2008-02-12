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
			return null;
		}
	}
}
