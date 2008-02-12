// MainWindow.cs created with MonoDevelop
// User: shana at 21:56 11/02/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//
using System;
using Gtk;

public partial class MainWindow: Gtk.Window
{	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		
		mockup.BugList bugs = new mockup.BugList ();
		vbox1.PackStart (bugs, true, true, 0);
		ShowAll ();
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}