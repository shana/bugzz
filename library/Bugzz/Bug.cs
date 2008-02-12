using System;
using System.Collections.Generic;
using System.Text;

namespace Bugzz
{
	public class Bug
	{
		public string AssignedTo;
		public string Status;
		public string Description;

		public Bug (string assignedto, string status, string description)
		{
			AssignedTo = assignedto;
			Status = status;
			Description = description;
		}
	}
}
