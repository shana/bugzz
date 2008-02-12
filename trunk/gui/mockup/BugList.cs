// BugList.cs created with MonoDevelop
// User: shana at 22:31 11/02/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;
using Gtk;

namespace mockup
{
	public partial class BugList : Gtk.Bin
	{
		
		ListStore store;
		public BugList()
		{
			this.Build();

			
			Bugzz.Bugzz bugzz = new Bugzz.Bugzz ();
			Bugzz.Query query = new Bugzz.Query();
			query.Email = "avidigal@novell.com";
			List<Bugzz.Bug> bugszz =  bugzz.Search (query);

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
			CellRendererToggle rendererToggle = new CellRendererToggle ();
			rendererToggle.Toggled += new ToggledHandler (FixedToggled);
			TreeViewColumn column =  new TreeViewColumn ("Fixed?", rendererToggle, "active", Column.Fixed);

			// set this column to a fixed sizing (of 50 pixels)
			column.Sizing = TreeViewColumnSizing.Fixed;
			column.FixedWidth = 50;
			treeView.AppendColumn (column);

			// column for bug numbers
			CellRendererText rendererText = new CellRendererText ();
			column = new TreeViewColumn ("Bug number", rendererText, "text", Column.Number);
			column.SortColumnId = (int) Column.Number;
			treeView.AppendColumn (column);

			// column for severities
			rendererText = new CellRendererText ();
			column = new TreeViewColumn ("Severity", rendererText, "text", Column.Severity);
			column.SortColumnId = (int) Column.Severity;
			treeView.AppendColumn(column);

			// column for description
			rendererText = new CellRendererText ();
			column = new TreeViewColumn ("Description", rendererText, "text", Column.Description);
			column.SortColumnId = (int) Column.Description;
			treeView.AppendColumn (column);
		}
		
		private ListStore CreateModel ()
		{
			ListStore store = new ListStore (typeof(bool),
							 typeof(int),
							 typeof(string),
							 typeof(string));

			foreach (Bug bug in bugs) {
				store.AppendValues (bug.Fixed,
						    bug.Number,
						    bug.Severity,
						    bug.Description);
			}

			return store;
		}		
		
		
		private void FixedToggled (object o, ToggledArgs args)
		{
			Gtk.TreeIter iter;
			if (store.GetIterFromString (out iter, args.Path)) {
				bool val = (bool) store.GetValue (iter, 0);
				store.SetValue (iter, 0, !val);
			}
		}
		
		private enum Column
		{
			Fixed,
			Number,
			Severity,
			Description
		}		
		
		private static Bug[] bugs =
		{
			new Bug ( false, 60482, "Normal",     "scrollable notebooks and hidden tabs"),
			new Bug ( false, 60620, "Critical",   "gdk_window_clear_area (gdkwindow-win32.c) is not thread-safe" ),
			new Bug ( false, 50214, "Major",      "Xft support does not clean up correctly" ),
			new Bug ( true,  52877, "Major",      "GtkFileSelection needs a refresh method. " ),
			new Bug ( false, 56070, "Normal",     "Can't click button after setting in sensitive" ),
			new Bug ( true,  56355, "Normal",     "GtkLabel - Not all changes propagate correctly" ),
			new Bug ( false, 50055, "Normal",     "Rework width/height computations for TreeView" ),
			new Bug ( false, 58278, "Normal",     "gtk_dialog_set_response_sensitive () doesn't work" ),
			new Bug ( false, 55767, "Normal",     "Getters for all setters" ),
			new Bug ( false, 56925, "Normal",     "Gtkcalender size" ),
			new Bug ( false, 56221, "Normal",     "Selectable label needs right-click copy menu" ),
			new Bug ( true,  50939, "Normal",     "Add shift clicking to GtkTextView" ),
			new Bug ( false, 6112,  "Enhancement","netscape-like collapsable toolbars" ),
			new Bug ( false, 1,     "Normal",     "First bug :=)" )
		};		
	}
	
	public class Bug
	{
		public bool Fixed;
		public int Number;
		public string Severity;
		public string Description;

		public Bug (bool status, int number, string severity,
			    string description)
		{
			Fixed = status;
			Number = number;
			Severity = severity;
			Description = description;
		}
	}	
}
