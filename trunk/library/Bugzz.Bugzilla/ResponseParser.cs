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
using System.Xml;

namespace Bugzz.Bugzilla
{
	public sealed class ResponseParser
	{
		public Dictionary<string, Bugzz.Bug> Bugs/* { get; private set; }*/;
		
		public ResponseParser (string input)
		{
			Bugs = new Dictionary <string, Bugzz.Bug> ();
			Parse (input);
		}

		void Parse (string input)
		{
			XmlDocument doc = new XmlDocument ();

			try {
				doc.LoadXml (input);

				XmlNode docElement = doc.DocumentElement;
				if (docElement == null)
					return;

				switch (docElement.Name) {
					case "RDF":
						ParseRDF (docElement);
						break;

					case "bugzilla":
						ParseXML (docElement);
						break;

					default:
						throw new BugzillaException ("Unknown response document kind '" + docElement.Name + "'.");
				}
			} catch (BugzillaException) {
				throw;
			} catch (Exception ex) {
				throw new BugzillaException ("Error parsing the response.", ex);
			}
		}

		void ParseRDF (XmlNode top)
		{
			Dictionary<string, Bugzz.Bug> bugs = Bugs;
			bugs.Clear ();
			
			XmlNamespaceManager ns = new XmlNamespaceManager (new NameTable ());

			ns.AddNamespace ("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
			ns.AddNamespace ("bz", "http://www.bugzilla.org/rdf#");
			ns.AddNamespace ("nc", "http://home.netscape.com/NC-rdf#");
			
			XmlNodeList nodes = top.SelectNodes ("//bz:bug[string-length (@rdf:about) > 0]", ns);
			XmlAttributeCollection attrs;
			Bugzz.Bug bug;
			string innerText;
			
			foreach (XmlNode node in nodes) {
				bug = new Bugzz.Bug ();
				attrs = node.Attributes;

				bug.URL = attrs ["rdf:about"].Value;
				if (node.HasChildNodes) {
					foreach (XmlNode tmp in node.ChildNodes) {
						innerText = tmp.InnerText.Trim ();
						
						switch (tmp.Name) {
							case "bz:id":
								bug.ID = innerText;
								break;

							case "bz:bug_severity":
								bug.Severity = innerText;
								break;

							case "bz:priority":
								bug.Priority = innerText;
								break;

							case "bz:op_sys":
								bug.OpSys = innerText;
								break;

							case "bz:bug_status":
								bug.Status = innerText;
								break;

							case "bz:resolution":
								bug.Resolution = innerText;
								break;

							case "bz:short_desc":
								bug.ShortDesc = innerText;
								break;

							case "bz:assigned_to":
								bug.AssignedTo = innerText;
								break;

							default:
								bug.AddItem (tmp.Name, innerText);
								break;
						}
					}
				}

				string bugId = bug.ID;
				
				if (String.IsNullOrEmpty (bugId))
					continue;
				
				if (bugs.ContainsKey (bugId))
					continue;

				bugs.Add (bugId, bug);
			}
		}

		void ParseXML (XmlNode top)
		{
			/*var*/ Dictionary<string, Bug> bugs = Bugs;
			bugs.Clear ();
			
			XmlNodeList nodes = top.SelectNodes ("//bugzilla/bug");
			Bugzz.Bug bug;
			string innerText;
			XmlAttribute attr;
			
			foreach (XmlNode node in nodes) {
				bug = new Bugzz.Bug ();
				attr = node.Attributes ["error"];
				if (attr != null)
					bug.Error = attr.Value;
				
				if (node.HasChildNodes) {
					foreach (XmlNode tmp in node.ChildNodes) {
						innerText = tmp.InnerText.Trim ();

						switch (tmp.Name) {
							case "bug_id":
								bug.ID = innerText;
								break;

							case "alias":
								bug.Alias = innerText;
								break;

							case "creation_ts":
								try {
									bug.CreationTimeStamp = DateTime.Parse (innerText);
								} catch {
									bug.CreationTimeStamp = DateTime.MinValue;
								}
								break;

							case "short_desc":
								bug.ShortDesc = innerText;
								break;

							case "delta_ts":
								try {
									bug.DeltaTimeStamp = DateTime.Parse (innerText);
								} catch {
									bug.DeltaTimeStamp = DateTime.MinValue;
								}
								break;

							case "classification":
								bug.Classification = innerText;
								break;

							case "product":
								bug.Product = innerText;
								break;

							case "component":
								bug.Component = innerText;
								break;

							case "version":
								bug.Version = innerText;
								break;

							case "rep_platform":
								bug.Platform = innerText;
								break;

							case "op_sys":
								bug.OpSys = innerText;
								break;

							case "bug_status":
								bug.Status = innerText;
								break;

							case "resolution":
								bug.Resolution = innerText;
								break;
								
							case "priority":
								bug.Priority = innerText;
								break;

							case "bug_severity":
								bug.Severity = innerText;
								break;

							case "long_desc":
								AppendLongDesc (node, bug);
								break;

							default:
								bug.AddItem (node.Name, innerText);
								break;
						}
					}
				}
				string bugId = bug.ID;
				
				if (String.IsNullOrEmpty (bugId))
					continue;
				
				if (bugs.ContainsKey (bugId))
					continue;

				bugs.Add (bugId, bug);
			}
		}

		void AppendLongDesc (XmlNode node, Bugzz.Bug bug)
		{
			Bugzz.BugLongDescription desc = new Bugzz.BugLongDescription ();
			XmlAttribute attr = node.Attributes ["isprivate"];
			
			desc.IsPrivate = attr != null ? attr.Value != "0" : false;

			XmlNode tmp = node.SelectSingleNode ("//bug_when");
			if (tmp != null && !String.IsNullOrEmpty (tmp.InnerText))
				try {
					desc.When = DateTime.Parse (tmp.InnerText);
				} catch {
					desc.When = DateTime.MinValue;
				}

			tmp = node.SelectSingleNode ("//thetext");
			if (tmp != null)
				desc.Text = tmp.InnerText;

			bug.Comments.Add (desc);
		}
		
	}
}
