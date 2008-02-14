// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2008 Novell, Inc.
//
// Authors:
//	Andreia Gaita (avidigal@novell.com)
//


using System;
using System.Collections.Generic;
using Gtk;

namespace mockup.Widgets
{
	public partial class BugList : Gtk.Bin
	{
		Bugzz.Query query;
		ListStore store;
		Bugzz.BugzzManager bugzz;
		
		public BugList (Bugzz.BugzzManager bugzz)
		{
			this.bugzz = bugzz;
			this.Build();

			store = CreateModel ();
			
			TreeView treeView = new TreeView (store);
			treeView.RulesHint = true;
			treeView.SearchColumn = (int) Column.Description;
			scrolledwindow3.Add (treeView);

			AddColumns (treeView);

			//NodeView nodeview = new NodeView (store);
			
			
		}
		
		private void AddColumns (TreeView treeView)
		{
			// column for fixed toggles
			//CellRendererToggle rendererToggle = new CellRendererToggle ();
			//rendererToggle.Toggled += new ToggledHandler (FixedToggled);
			//TreeViewColumn column =  new TreeViewColumn ("Fixed?", rendererToggle, "active", Column.Fixed);

			// set this column to a fixed sizing (of 50 pixels)
			//column.Sizing = TreeViewColumnSizing.Fixed;
			//column.FixedWidth = 50;
			//treeView.AppendColumn (column);


			CellRendererText rendererText = new CellRendererText ();
			TreeViewColumn column = new TreeViewColumn ("Bug number", rendererText, "text", Column.Number);
			column.SortColumnId = (int) Column.Number;
			treeView.AppendColumn (column);

			// column for bug numbers
			rendererText = new CellRendererText ();
			column = new TreeViewColumn ("Assigned To", rendererText, "text", Column.AssignedTo);
			column.SortColumnId = (int) Column.AssignedTo;
			treeView.AppendColumn (column);

			// column for severities
			rendererText = new CellRendererText ();
			column = new TreeViewColumn ("Status", rendererText, "text", Column.Status);
			column.SortColumnId = (int) Column.Status;
			treeView.AppendColumn(column);

			// column for description
			rendererText = new CellRendererText ();
			column = new TreeViewColumn ("Description", rendererText, "text", Column.Description);
			column.SortColumnId = (int) Column.Description;
			treeView.AppendColumn (column);
		}
		
		private ListStore CreateModel ()
		{
			ListStore store = new ListStore (typeof(int),
							 typeof(string),
							 typeof(string),
							 typeof(string));


			return store;
		}		
		

		public void Load (Bugzz.Query query) {
			this.query = query;
			this.Refresh ();
		}
		
		public void Refresh () {
			if (this.query != null)
				return;
				
			Dictionary<string, Bugzz.Bug> bugszz = bugzz.Search (query);
		
			foreach (KeyValuePair<string, Bugzz.Bug> bug in bugszz) {
				store.AppendValues (int.Parse (bug.Value.ID),
							bug.Value.AssignedTo,
							bug.Value.Status,
							bug.Value.ShortDesc);
			}
		}
		
		private void FixedToggled (object o, ToggledArgs args)
		{
			Gtk.TreeIter iter;
			if (store.GetIterFromString (out iter, args.Path)) {
				bool val = (bool) store.GetValue (iter, 0);
				store.SetValue (iter, 0, !val);
			}
		}

		protected virtual void OnClicked (object sender, System.EventArgs e)
		{
			this.Refresh ();		
		}
		
		private enum Column
		{
			Number,
			AssignedTo,
			Status,
			Description
		}			
	}	
}
