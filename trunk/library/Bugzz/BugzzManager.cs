//
// Bugzz - Multi GUI Desktop Bugzilla Client
//
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
//	Marek Habersack (mhabersack@novell.com)
//
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using C5;

namespace Bugzz
{
	public class BugzzManager
	{
		Bugzilla.Bugzilla bugzilla;

		public HashBag <IInitialValue> Classifications {
			get { return bugzilla.Classifications; }
		}

		public HashBag <IInitialValue> Products {
			get { return bugzilla.Products; }
		}
		
		public HashBag <IInitialValue> Components {
			get { return bugzilla.Components; }
		}
		
		public HashBag <IInitialValue> FoundInVersion {
			get { return bugzilla.FoundInVersion; }
		}
		
		public HashBag <IInitialValue> FixedInMilestone {
			get { return bugzilla.FixedInMilestone; }
		}

		public HashBag <IInitialValue> Status {
			get { return bugzilla.Status; }
		}
		
		public HashBag <IInitialValue> Resolution {
			get { return bugzilla.Resolution; }
		}
		
		public HashBag <IInitialValue> Severity {
			get { return bugzilla.Severity; }
		}
		
		public HashBag <IInitialValue> Priority {
			get { return bugzilla.Priority; }
		}
		
		public HashBag <IInitialValue> Hardware {
			get { return bugzilla.Hardware; }
		}
		
		public HashBag <IInitialValue> OS {
			get { return bugzilla.OS; }
		}
		
		public string Url
		{
			get { return bugzilla.BaseUrl; }
			set { bugzilla.BaseUrl = value; }
		}

		public BugzzManager (LoginData loginData)
		{
			bugzilla = new Bugzilla.Bugzilla (loginData);
		}

		public BugzzManager (string bugsiteBase)
		: this (bugsiteBase, null)
		{
		}
		
		public BugzzManager (string bugsiteBase, LoginData loginData)
		{
			bugzilla = new Bugzilla.Bugzilla (bugsiteBase, loginData);
		}

		public event DocumentRetrieveFailureEventHandler DocumentRetrieveFailure
		{
			add { bugzilla.WebIO.DocumentRetrieveFailure += value; }
			remove { bugzilla.WebIO.DocumentRetrieveFailure -= value; }
		}

		public event DownloadStartedEventHandler DownloadStarted
		{
			add { bugzilla.WebIO.DownloadStarted += value; }
			remove { bugzilla.WebIO.DownloadStarted -= value; }
		}

		public event DownloadEndedEventHandler DownloadEnded
		{
			add { bugzilla.WebIO.DownloadEnded += value; }
			remove { bugzilla.WebIO.DownloadEnded -= value; }
		}

		public event DownloadProgressEventHandler DownloadProgress
		{
			add { bugzilla.WebIO.DownloadProgress += value; }
			remove { bugzilla.WebIO.DownloadProgress -= value; }
		}

		public Dictionary <string, Bug> Search (Query query)
		{
			return bugzilla.GetBugList (query);
		}

		public Dictionary <string, Bug> GetBugList (Query query)
		{
			return bugzilla.GetBugList (query);
		}

		public Dictionary <string, Bug> GetBugs (Query query)
		{
			return bugzilla.GetBugs (query);
		}
	}
}
