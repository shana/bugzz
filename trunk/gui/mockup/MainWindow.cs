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
using Bugzz;

namespace mockup {

	public partial class MainWindow: Gtk.Window
	{	

		Widgets.BugList bugsWidget;
		Widgets.Detail detailWidget;
		Widgets.Search searchWidget;
		Widgets.Settings settingsWidget;
		
		
		Settings settings;
		BugzzManager bugzzManager;
		
		public MainWindow (): base (Gtk.WindowType.Toplevel)
		{
		
			settings = new Settings ();
			Loader.Load (settings);
			Loader.Save (settings);
			Bugzz.LoginData loginData = new LoginData();
			if (settings.Online)
				bugzzManager = new BugzzManager (settings.Url);
			else
				bugzzManager = new BugzzManager (loginData);

		
			Build ();
		}
		
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		protected virtual void OnToggleSettings (object sender, System.EventArgs e)
		{
			if (settingsWidget == null) {
				settingsWidget = new Widgets.Settings (settings);
				vbox1.PackStart (settingsWidget, true, true, 0);
			} else {			
				if (settingsWidget.Visible)
					settingsWidget.Cancel ();
				else
					settingsWidget.Show ();
			}
			ShowAll ();
		}

		protected virtual void OnToggleSearch (object sender, System.EventArgs e)
		{
			if (searchWidget == null) {
				searchWidget = new Widgets.Search (this);
				vbox1.PackStart (searchWidget, true, true, 0);
			} else {			
				if (searchWidget.Visible)
					searchWidget.Hide ();
				else
					searchWidget.Show ();
			}
			ShowAll ();
		}

		protected virtual void OnToggleList (object sender, System.EventArgs e)
		{
			if (searchWidget == null)
				return;
			ToggleList (searchWidget.Query);
		}
		
		public void ToggleList (Query query)
		{
			if (bugsWidget == null) {
				bugsWidget = new Widgets.BugList (bugzzManager);
				vbox1.PackStart (bugsWidget, true, true, 0);
			} else {						
				if (bugsWidget.Visible)
					bugsWidget.Hide ();
				else
					bugsWidget.Show ();
			}
			ShowAll ();
			bugsWidget.Load (query);
		}

		protected virtual void OnToggleDetail (object sender, System.EventArgs e)
		{
			if (detailWidget == null) {
				detailWidget = new Widgets.Detail ();
				vbox1.PackStart (detailWidget, true, true, 0);
			} else {			
			
				if (detailWidget.Visible)
					detailWidget.Hide ();
				else
					detailWidget.Show ();
			}
			ShowAll ();
		}

		protected virtual void OnToggleOnline (object sender, System.EventArgs e)
		{
			if (settings.Online) {
				settings.Online = false;
				this.actOnline.Label = "Offline";
			} else {
				settings.Online = true;
				this.actOnline.Label = "Online";
			}
		}
	}
}