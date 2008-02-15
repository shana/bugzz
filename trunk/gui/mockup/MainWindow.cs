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
		
		bool suspendLayout;
		
		public MainWindow (): base (Gtk.WindowType.Toplevel)
		{
		
			Build ();
			
			settings = new Settings ();
			Loader.Load (settings);
			Bugzz.LoginData loginData = new LoginData();
			Loader.Load (loginData);
			
			if (settings.Online) {
				Console.WriteLine (1);
				bugzzManager = new BugzzManager (settings.Url, loginData);
				this.actOnline.Label =  Mono.Unix.Catalog.GetString("Online");
			}
			else {
				Console.WriteLine (2);
				bugzzManager = new BugzzManager (loginData);
				this.actOnline.Label =  Mono.Unix.Catalog.GetString("Offline");
			}
			ShowAll ();
			
		}
		
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		protected virtual void OnToggleSettings (object sender, System.EventArgs e)
		{
			if (suspendLayout) return;
			if (settingsWidget == null) {
				settingsWidget = new Widgets.Settings (settings);
				//vbox1.PackStart (settingsWidget, true, true, 0);
				ShowFull (settingsWidget);
				
			} else {			
				if (settingsWidget.Visible) {
					settingsWidget.Cancel ();
					Hide (settingsWidget);
				} else {
					ShowFull (settingsWidget);
				}
			}
			ShowAll ();
		}

		protected virtual void OnToggleSearch (object sender, System.EventArgs e)
		{
			if (suspendLayout) return;
			if (searchWidget == null) {
				searchWidget = new Widgets.Search (this);
				//vbox1.PackStart (searchWidget, true, true, 0);
				ShowTop (searchWidget);				
			} else {			
				if (searchWidget.Visible) {
					Hide (searchWidget);
				} else {
					ShowTop (searchWidget);
				}
			}
			ShowAll ();
		}

		protected virtual void OnToggleList (object sender, System.EventArgs e)
		{
			if (suspendLayout) return;
			ToggleList ();
		}
		
		public void ToggleList ()
		{
			if (bugsWidget == null) {
				bugsWidget = new Widgets.BugList (bugzzManager);
				//vbox1.PackStart (bugsWidget, true, true, 0);
				ShowBottom (bugsWidget);
			} else {						
				if (bugsWidget.Visible) {
					Hide (bugsWidget);
				} else {
					ShowBottom (bugsWidget);
				}
			}
			ShowAll ();
		}
		
		public void Search (Query query)
		{
			if (bugsWidget == null || !bugsWidget.Visible)
				ToggleList ();
			
			bugsWidget.Load (query);
		}

		protected virtual void OnToggleDetail (object sender, System.EventArgs e)
		{
			if (suspendLayout) return;
			if (detailWidget == null) {
				detailWidget = new Widgets.Detail ();
				//vbox1.PackStart (detailWidget, true, true, 0);
				ShowBottom (detailWidget);
			} else {			
				if (detailWidget.Visible) {
					detailWidget.Cancel ();
					Hide (detailWidget);
				} else {
					ShowBottom (detailWidget);				
				}
			}
			ShowAll ();
		}

		protected virtual void OnToggleOnline (object sender, System.EventArgs e)
		{
			if (suspendLayout) return;
			if (settings.Online) {
				settings.Online = false;
				this.actOnline.Label =  Mono.Unix.Catalog.GetString("Offline");
			} else {
				settings.Online = true;
				this.actOnline.Label = Mono.Unix.Catalog.GetString("Online");
			}
		}
		
		private void Hide (Gtk.Widget widget)
		{
			vpane.Remove (widget);
			widget.Hide ();
			UpdateToggles ();
		}
		
		private void ShowBottom (Gtk.Widget widget)
		{
			Gtk.Widget pre = vpane.Child2;
			if (pre != null) {
				pre.Hide ();
				vpane.Remove (pre);
			}
			vpane.Add2 (widget);
			widget.Show ();
			if (vpane.Child1 != null)
				vpane.Position = vpane.Child1.SizeRequest ().Height;
			UpdateToggles ();
		}
		
		private void ShowTop (Gtk.Widget widget)
		{
			Gtk.Widget pre = vpane.Child1;
			if (pre != null) {
				pre.Hide ();
				vpane.Remove (pre);
			}
			vpane.Add1 (widget);
			widget.Show ();
			vpane.Position = widget.SizeRequest ().Height;
			UpdateToggles ();
		}
		
		private void ShowFull (Gtk.Widget widget)
		{
			Gtk.Widget pre = vpane.Child1;
			if (pre != null) {
				pre.Hide ();
				vpane.Remove (pre);
			}

			pre = vpane.Child2;
			if (pre != null) {
				pre.Hide ();
				vpane.Remove (pre);
			}
			
			vpane.Add (widget);
			widget.Show ();
			vpane.Position = widget.SizeRequest ().Height;
			UpdateToggles ();
		}
		
		private void UpdateToggles ()
		{
			suspendLayout = true;
			actBugDetail.Active = detailWidget != null ? detailWidget.Visible : false;
			actBugList.Active = bugsWidget != null ? bugsWidget.Visible : false;
			actSearch.Active = searchWidget != null ? searchWidget.Visible : false;
			actSettings.Active = settingsWidget != null ? settingsWidget.Visible : false;
			suspendLayout = false;
		}
		
	}
}