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
using System.Collections.Generic;

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
					Console.WriteLine (xmldoc.OuterXml);
				}
				
				if (xmldoc.SelectSingleNode ("//root") == null) {
					XmlElement elem = xmldoc.CreateElement ("root");
					xmldoc.AppendChild (elem);
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
				object obj = val;
				if (prop != null) {
					if (prop.PropertyType == typeof(bool)) {
						obj = bool.Parse (val);
					} else 
					if (prop.PropertyType == typeof(short)) {
						obj = short.Parse (val);
					} else 
					if (prop.PropertyType == typeof(ushort)) {
						obj = ushort.Parse (val);
					} else 
					if (prop.PropertyType == typeof(int)) {
						obj = int.Parse (val);
					} else 
					if (prop.PropertyType == typeof(uint)) {
						obj = uint.Parse (val);
					} else 
					if (prop.PropertyType == typeof(byte)) {
						obj = byte.Parse (val);
					} else 
					if (prop.PropertyType == typeof(long)) {
						obj = long.Parse (val);
					} else 
					if (prop.PropertyType == typeof(ulong)) {
						obj = ulong.Parse (val);
					} else 
					if (prop.PropertyType == typeof(double)) {
						obj = double.Parse (val);
					} else 
					if (prop.PropertyType == typeof(float)) {
						obj = float.Parse (val);
					} else 
					if (prop.PropertyType == typeof(Enum)) {
						obj = Enum.Parse (prop.PropertyType, val);
					} else
					if (prop.PropertyType == typeof (Uri)) {
						obj = new Uri(val);
					} else
					if (prop.PropertyType == typeof (Dictionary<string, string>)) {
						obj = new Dictionary<string, string> ();
						foreach (string pair in val.Split ('&')) {
							string[] s = pair.Split ('=');
							if (s.Length < 2)
								continue;
							if (s.Length > 2) {
								for (int i = 2; i < s.Length; i++) {
									s[1] += "=" + s[i];
								}
							}
							((Dictionary<string, string>) obj).Add (s[0], s[1]);
						}
					} 
					try {
						prop.SetValue (instance, obj, null);
					}
					catch {
					}
					
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
				node = xmldoc.DocumentElement.AppendChild (elem);
			} else {
				node.RemoveAll ();
			}
			
			Type t = typeof(T);
			foreach (PropertyInfo prop  in t.GetProperties (BindingFlags.Instance | BindingFlags.Public)) {
				string name = prop.Name;
				object val = prop.GetValue (instance, null);
				if (val != null) {
					XmlElement elem = xmldoc.CreateElement (name);
					if (val is Dictionary<string, string>) {
						foreach (KeyValuePair<string, string> vals in val as Dictionary<string, string>) {
							elem.InnerText += vals.Key + "=" + vals.Value + "&amp;";
						}
					} else
						elem.InnerText = val.ToString ();
					node.AppendChild (elem);
				}
			}
			
			xmldoc.Save (dataPath);
		}
	}
}
