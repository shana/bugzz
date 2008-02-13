using System;
using System.Collections.Generic;
using System.Xml;

namespace Bugzz.Bugzilla
{
	internal class ResponseParser
	{
		Dictionary <string, Bugzz.Bug> bugs;
		
		public ResponseParser (string input)
		{
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
						break;
				}
			} catch (BugzillaException ex) {
				throw;
			} catch (Exception ex) {
				throw new BugzillaException ("Error parsing the response.", ex);
			}
		}

		void ParseRDF (XmlNode top)
		{
			bugs = new Dictionary <string, Bugzz.Bug> ();
			XmlNamespaceManager ns = new XmlNamespaceManager (new NameTable ());

			ns.AddNamespace ("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
			ns.AddNamespace ("bz", "http://www.bugzilla.org/rdf#");
			ns.AddNamespace ("nc", "http://home.netscape.com/NC-rdf#");
			
			XmlNodeList nodes = top.SelectNodes ("//bz:bug[string-length (@rdf:about) > 0]", ns);
			XmlAttributeCollection attrs;
			
			foreach (XmlNode node in nodes) {
				attrs = node.Attributes;
				
			}
		}

		void ParseXML (XmlNode top)
		{
		}
	}
}
