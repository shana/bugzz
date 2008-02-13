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
		public string Alias /*{ get; set; }*/
		
		public Dictionary<string, string> Items/* { get; private set; }*/;
		
		public Bug ()
		{
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
			StringBuilder ret = new StringBuilder (this.GetType ().ToString () + "[ ");

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
	}
}
