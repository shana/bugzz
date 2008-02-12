// Main.cs created with MonoDevelop
// User: shana at 18:11 11/02/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace html2xml
{
	class MainClass : ICertificatePolicy
	{
		public bool CheckValidationResult (ServicePoint sp, X509Certificate cert, WebRequest req, int error) 
		{
			return true;
			
		}
		
		public static void Main(string[] args)
		{
			if (args.Length < 1) {
				Console.WriteLine ("Usage: mono html2xml.exe url");
				return;
			}
			
			Uri uri = new Uri (args[0]);
			Console.WriteLine ("{0} {1}", uri.Scheme + "://" + uri.Host, uri.AbsolutePath); 
			ServicePointManager.CertificatePolicy = new MainClass ();
			Bugzz.Network.WebIO io = new Bugzz.Network.WebIO (uri.Scheme + "://" + uri.Host);
			string html = io.GetDocument (uri.AbsolutePath);
			HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
			doc.LoadHtml (html);
			doc.OptionOutputAsXml = true;
			doc.Save (uri.AbsolutePath.Substring (uri.AbsolutePath.LastIndexOf ("/") + 1) + ".xml");
		}
	}
}