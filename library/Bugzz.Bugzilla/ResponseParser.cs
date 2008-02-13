using System;
using System.Collections.Generic;
using System.Xml;

namespace Bugzz.Bugzilla
{
	internal class ResponseParser
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
			var bugs = Bugs;
			bugs.Clear ();
			
			XmlNodeList nodes = top.SelectNodes ("//bugzilla/bug");
			Bugzz.Bug bug;
			string innerText;

			foreach (XmlNode node in nodes) {
				bug = new Bugzz.Bug ();

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
						}
					}
				}
			}
		}
	}
}
