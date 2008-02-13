using System;
using System.Collections.Generic;
using System.Text;

namespace Bugzz
{
	public class Bug
	{
		public string ID /*{ get; set; }*/;
		public string Status /*{ get; set; }*/;
		public string Severity /*{ get; set; }*/;
		public string Priority /*{ get; set; }*/;
		public string OpSys /*{ get; set; }*/;
		public string Resolution /*{ get; set; }*/;
		public string ShortDesc /*{ get; set; }*/;
		public string URL /*{ get; set; }*/;
		public string AssignedTo /*{ get; set; }*/;
		public string Alias /*{ get; set; }*/;
		public string Classification /*{ get; set; }*/;
		public string Product /*{ get; set; }*/;
		public string Component /*{ get; set; }*/;
		public string Version /*{ get; set; }*/;
		public string Platform /*{ get; set; }*/;
		public DateTime CreationTimeStamp  /*{ get; set; }*/;
		public DateTime DeltaTimeStamp  /*{ get; set; }*/;
		public List <BugLongDescription> Comments /*{ get; private set; }*/;
		public Dictionary<string, string> Items/* { get; private set; }*/;
		
		public Bug ()
		{
			Items = new Dictionary <string, string> ();
			Comments = new List <BugLongDescription> ();
		}

		internal void AddItem (string itemName, string itemValue)
		{
			Dictionary<string, string> items = Items;

			if (items.ContainsKey (itemName))
				items [itemName] = itemValue;
			else
				items.Add (itemName, itemValue);
		}

		public override string ToString ()
		{
			return ToString (false);
		}
		
		public string ToString (bool full)
		{
			StringBuilder ret = new StringBuilder (this.GetType ().ToString ());

			if (!full) {
				ret.Append (" [");
				if (!String.IsNullOrEmpty (ID))
					ret.Append ("ID: " + ID + " ");
				if (!String.IsNullOrEmpty (Status))
					ret.Append ("Status: " + Status + " ");
				if (!String.IsNullOrEmpty (ShortDesc))
					ret.Append ("Short desc: '" + ShortDesc + "' ");
				if (!String.IsNullOrEmpty (URL))
					ret.Append ("URL: '" + URL + "' ");
				ret.Length--;
				ret.Append ("]");

				return ret.ToString ();
			}

			ret.Append (Environment.NewLine);
			AppendString (ret, "ID", ID);
			AppendString (ret, "Status", Status);
			AppendString (ret, "Severity", Severity);
			AppendString (ret, "Priority", Priority);
			AppendString (ret, "OpSys", OpSys);
			AppendString (ret, "Resolution", Resolution);
			AppendString (ret, "ShortDesc", ShortDesc);
			AppendString (ret, "URL", URL);
			AppendString (ret, "AssignedTo", AssignedTo);
			AppendString (ret, "Alias", Alias);
			AppendString (ret, "Classification", Classification);
			AppendString (ret, "Product", Product);
			AppendString (ret, "Component", Component);
			AppendString (ret, "Version", Version);
			AppendString (ret, "Platform", Platform);
			AppendString (ret, "Creation Time", CreationTimeStamp.ToString ());
			AppendString (ret, "Delta Time", DeltaTimeStamp.ToString ());

			if (Items.Count > 0) {
				ret.Append ("Items:" + Environment.NewLine);
				foreach (KeyValuePair <string, string> kvp in Items)
					ret.Append ("\t" + kvp.Key + " = " + kvp.Value + Environment.NewLine);
			}

			if (Comments.Count > 0) {
				ret.Append ("Comments:" + Environment.NewLine);
				foreach (BugLongDescription desc in Comments)
					ret.Append (desc.ToString () + Environment.NewLine);
			}
			
			return ret.ToString ();
		}

		void AppendString (StringBuilder sb, string label, string str)
		{
			if (String.IsNullOrEmpty (str))
				return;

			sb.Append (label + ": " + str + Environment.NewLine);
		}
	}
}
