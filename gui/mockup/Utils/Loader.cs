//Permission is hereby granted, free of charge, to any person obtaining
//a copy of this software and associated documentation files (the
//"Software"), to deal in the Software without restriction, including
//without limitation the rights to use, copy, modify, merge, publish,
//distribute, sublicense, and/or sell copies of the Software, and to
//permit persons to whom the Software is furnished to do so, subject to
//the following conditions:
//
//The above copyright notice and this permission notice shall be
//included in all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//Copyright (c) 2008 Novell, Inc.
//
//Authors:
//	Andreia Gaita (avidigal@novell.com)
//

using System;
using System.Xml;
using System.IO;
using System.Reflection;

namespace mockup
{
	public static class Loader
	{		
		static string dataPath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "Bugzz");			
		static object objLock = new object();
		static XmlDocument xmldoc;

		static Loader ()
		{
			lock (objLock) {
				if (!Directory.Exists (dataPath)) {
					Directory.CreateDirectory (dataPath);
					return;
				}
				
				xmldoc = new XmlDocument();
				dataPath = Path.Combine (dataPath, "config.xml");
				if (File.Exists (dataPath)) {
					xmldoc.Load (dataPath);
				}
				
				if (xmldoc.SelectSingleNode ("//root") == null) {
					XmlElement elem = xmldoc.CreateElement ("root");
					xmldoc.DocumentElement.AppendChild (elem);
				}
			}
		}

		public static void Load<T> (T instance) 
		{
			string type = typeof(T).ToString ();
			if (type.IndexOf (".") > 0)
				type = type.Substring (type.LastIndexOf (".") + 1);
			Console.WriteLine (type);
			
			XmlNode node = xmldoc.SelectSingleNode ("//root/" + type);
			if (node == null)
				return;
			
			Type t = typeof(T);
			foreach (XmlNode child in node.ChildNodes) {
				string name = child.LocalName;
				string val = child.InnerText;
				PropertyInfo prop = t.GetProperty (name, BindingFlags.Instance | BindingFlags.Public);
				if (prop != null) {
					prop.SetValue (instance, val, null);
				}
			}
		}
		
		public static void Save<T> (T instance) 
		{
			string type = typeof(T).ToString ();
			if (type.IndexOf (".") > 0)
				type = type.Substring (type.LastIndexOf (".") + 1);
			Console.WriteLine (type);
			XmlNode node = xmldoc.SelectSingleNode ("//root/" + type);
						
			if (node == null) {
				XmlElement elem = xmldoc.CreateElement (type);
				node = xmldoc.DocumentElement.FirstChild.AppendChild (elem);
			} else {
				node.RemoveAll ();
			}
			
			Type t = typeof(T);
			foreach (PropertyInfo prop  in t.GetProperties (BindingFlags.Instance | BindingFlags.Public)) {
				string name = prop.Name;
				string val = prop.GetValue (instance, null).ToString ();
				XmlElement elem = xmldoc.CreateElement (name);
				elem.InnerText = val;
				node.AppendChild (elem);
			}
			
			xmldoc.Save (dataPath);
		}
	}
}
