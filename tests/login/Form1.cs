using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace WindowsApplication1
{
	public class Form1 : Form
	{
		public Form1 ()
		{
			InitializeComponent ();
		}

		class CertPolicy : ICertificatePolicy
		{
			public bool CheckValidationResult (ServicePoint srvPoint,
		X509Certificate certificate, WebRequest request, int certificateProblem)
			{
				// You can do your own certificate checking.
				// You can obtain the error values from WinError.h.

				// Return true so that any certificate will work with this sample.
				return true;
			}
		}

		Bugzz.Network.CookieManager c = new Bugzz.Network.CookieManager ();
		private bool login ()
		{
			string str1 = "username=" + textBox1.Text;
			string str2 = "password=" + textBox2.Text;
			string str3 = "url=" + textBox3.Text;
			string str4 = "context=" + textBox4.Text;
			string str5 = "proxypath=" + textBox5.Text;

			ASCIIEncoding encoding = new ASCIIEncoding ();
			string postData = str1 + "&" + str2 + "&" + str3 + "&" + str4 + "&" + str5;
			byte[] data = encoding.GetBytes (postData);


			string loginurl = "https://bugzilla.novell.com/ICSLogin/auth-up";
			
			//			X509Certificate Cert = X509Certificate.CreateFromCertFile (@"D:\soft\dev\Charles\docs\charles.cer");
			//			ServicePointManager.CertificatePolicy = new CertPolicy ();

			// Prepare web request...
			HttpWebRequest myRequest =
			  (HttpWebRequest) WebRequest.Create (loginurl);
			//			myRequest.ClientCertificates.Add (Cert);

			myRequest.Method = "POST";
			myRequest.ContentType = "application/x-www-form-urlencoded";
			myRequest.ContentLength = data.Length;
			myRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.0; en-US; rv:1.8) Gecko/20051111 Firefox/1.5";
			myRequest.CookieContainer = c;

			Stream newStream = myRequest.GetRequestStream ();
			// Send the data.
			newStream.Write (data, 0, data.Length);
			newStream.Flush ();
			newStream.Close ();

			
			HttpWebResponse response = myRequest.GetResponse () as HttpWebResponse;
			c.Save ();
			response.Close ();
			
			return response.StatusCode == HttpStatusCode.OK;

		}

		private void get ()
		{
		}

		private void button1_Click (object sender, EventArgs e)
		{

			do {
				string url = "https://bugzilla.novell.com/show_bug.cgi?id=330500&GoAheadAndLogIn=1";
				HttpWebRequest myRequest = (HttpWebRequest) WebRequest.Create (url);
				myRequest.Method = "GET";
				myRequest.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.0; en-US; rv:1.8) Gecko/20051111 Firefox/1.5";
				myRequest.CookieContainer = c;

				HttpWebResponse response = myRequest.GetResponse () as HttpWebResponse;

				if (response.StatusCode == HttpStatusCode.OK) {
					if (myRequest.RequestUri != myRequest.Address) {
						c.Save ();
						response.Close ();
						if (!login ())
							return;
						continue;
					}


					StringBuilder sb = new StringBuilder ();
					Stream data1 = response.GetResponseStream ();
					char[] buffer = new char[4096];
					int bufferLen = buffer.Length;
					int charsRead = -1;
					long count;

					using (StreamReader reader = new StreamReader (data1)) {
						count = 0;
						long contentLength = response.ContentLength;
						if (contentLength == -1)
							contentLength = Int64.MaxValue; // potentially
						// dangerous

						while (count < contentLength) {
							charsRead = reader.Read (buffer, 0, bufferLen);
							if (charsRead == 0)
								break;

							count += charsRead;
							sb.Append (buffer, 0, charsRead);
						}
					}

					string ss = sb.ToString ();
					c.Save ();
					response.Close ();
					break;
				}
			} while (true);
		}



		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
			this.textBox1 = new System.Windows.Forms.TextBox ();
			this.textBox2 = new System.Windows.Forms.TextBox ();
			this.button1 = new System.Windows.Forms.Button ();
			this.textBox3 = new System.Windows.Forms.TextBox ();
			this.textBox4 = new System.Windows.Forms.TextBox ();
			this.textBox5 = new System.Windows.Forms.TextBox ();
			this.SuspendLayout ();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point (12, 21);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size (698, 20);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point (12, 47);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size (698, 20);
			this.textBox2.TabIndex = 1;
			this.textBox2.Text = "";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point (12, 153);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size (75, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler (this.button1_Click);
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point (12, 73);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size (698, 20);
			this.textBox3.TabIndex = 3;
			this.textBox3.Text = "https://bugzilla.novell.com/ichainlogin.cgi?target=query.cgi?format%3Dadvanced%26" +
				"field0-0-0%3Dxyzzy%26GoAheadAndLogIn%3D1";
			// 
			// textBox4
			// 
			this.textBox4.Location = new System.Drawing.Point (12, 100);
			this.textBox4.Name = "textBox4";
			this.textBox4.Size = new System.Drawing.Size (698, 20);
			this.textBox4.TabIndex = 4;
			this.textBox4.Text = "default";
			// 
			// textBox5
			// 
			this.textBox5.Location = new System.Drawing.Point (12, 127);
			this.textBox5.Name = "textBox5";
			this.textBox5.Size = new System.Drawing.Size (698, 20);
			this.textBox5.TabIndex = 5;
			this.textBox5.Text = "reverse";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size (817, 273);
			this.Controls.Add (this.textBox5);
			this.Controls.Add (this.textBox4);
			this.Controls.Add (this.textBox3);
			this.Controls.Add (this.button1);
			this.Controls.Add (this.textBox2);
			this.Controls.Add (this.textBox1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout (false);
			this.PerformLayout ();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.TextBox textBox5;


		[STAThread]
		static void Main ()
		{
			Application.EnableVisualStyles ();
			Application.SetCompatibleTextRenderingDefault (false);
			Application.Run (new Form1 ());
		}
	}
}