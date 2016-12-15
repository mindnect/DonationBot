using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace JSAssist
{
	public class SurveyReport : Form
	{
		private IContainer components;

		private WebBrowser webBrowser;

		public SurveyReport()
		{
			this.InitializeComponent();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SurveyReport));
			this.webBrowser = new WebBrowser();
			base.SuspendLayout();
			this.webBrowser.Dock = DockStyle.Fill;
			this.webBrowser.Location = new Point(0, 0);
			this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser.Name = "webBrowser";
			this.webBrowser.ScriptErrorsSuppressed = true;
			this.webBrowser.Size = new System.Drawing.Size(534, 561);
			this.webBrowser.TabIndex = 0;
			this.webBrowser.Url = new Uri("http://js-almighty.com/jsassist/application/survey_report.html", UriKind.Absolute);
			base.AutoScaleDimensions = new SizeF(7f, 12f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(534, 561);
			base.Controls.Add(this.webBrowser);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SurveyReport";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "JSAssist Survey & Report";
			base.ResumeLayout(false);
		}
	}
}