using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Bugzz
{
	public class Bug
	{
		public int ID;
		public string AssignedTo;
		public string Status;
		public string Description;

		internal Bug (XmlNode bugNode, XmlNamespaceManager ns)
		{
			XmlNode node = bugNode.SelectSingleNode ("bz:id", ns);
			ID = int.Parse (node.InnerText);
			node = bugNode.SelectSingleNode ("bz:assigned_to", ns);
			if (node != null)
				AssignedTo = node.InnerText;
			node = bugNode.SelectSingleNode ("bz:bug_status", ns);
			if (node != null)
				Status = node.InnerText;
			node = bugNode.SelectSingleNode ("bz:short_desc", ns);
			if (node != null)
				Description = node.InnerText;
		}

		public Bug (int id, string assignedto, string status, string description)
		{
			ID = id;
			AssignedTo = assignedto;
			Status = status;
			Description = description;
		}
	}
}
