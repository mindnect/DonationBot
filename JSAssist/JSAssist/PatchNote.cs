using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace JSAssist
{
	public class PatchNote : Form
	{
		private IContainer components;

		private WebBrowser webBrowser;

		private Button buttonClose;

		private CheckBox checkBoxSkip;

		private Label label1;

		private Label labelVersion;

		public PatchNote()
		{
			this.InitializeComponent();
			this.checkBoxSkip.Checked = Program.config.skipPatchNote;
			this.labelVersion.Text = string.Concat("build.beta.", Program.currentVersion);
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			bool @checked = this.checkBoxSkip.Checked;
			Program.config.skipPatchNote = @checked;
			Program.config.SaveFile();
			base.Close();
		}

		private void checkBoxSkip_CheckedChanged(object sender, EventArgs e)
		{
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(PatchNote));
			this.webBrowser = new WebBrowser();
			this.buttonClose = new Button();
			this.checkBoxSkip = new CheckBox();
			this.label1 = new Label();
			this.labelVersion = new Label();
			base.SuspendLayout();
			this.webBrowser.Location = new Point(-1, 32);
			this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser.Name = "webBrowser";
			this.webBrowser.ScriptErrorsSuppressed = true;
			this.webBrowser.Size = new System.Drawing.Size(534, 485);
			this.webBrowser.TabIndex = 0;
			this.webBrowser.Url = new Uri("http://js-almighty.com/jsassist/application/patch_note.html", UriKind.Absolute);
			this.buttonClose.Location = new Point(229, 553);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(75, 23);
			this.buttonClose.TabIndex = 1;
			this.buttonClose.Text = "확인";
			this.buttonClose.UseVisualStyleBackColor = true;
			this.buttonClose.Click += new EventHandler(this.buttonClose_Click);
			this.checkBoxSkip.AutoSize = true;
			this.checkBoxSkip.Location = new Point(152, 529);
			this.checkBoxSkip.Name = "checkBoxSkip";
			this.checkBoxSkip.Size = new System.Drawing.Size(232, 16);
			this.checkBoxSkip.TabIndex = 2;
			this.checkBoxSkip.Text = "다음 업데이트 까지 표시하지 않습니다";
			this.checkBoxSkip.UseVisualStyleBackColor = true;
			this.checkBoxSkip.CheckedChanged += new EventHandler(this.checkBoxSkip_CheckedChanged);
			this.label1.AutoSize = true;
			this.label1.Location = new Point(8, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(133, 12);
			this.label1.TabIndex = 3;
			this.label1.Text = "실행중인 프로그램 버전";
			this.labelVersion.AutoSize = true;
			this.labelVersion.Location = new Point(147, 9);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(70, 12);
			this.labelVersion.TabIndex = 4;
			this.labelVersion.Text = "build.beta.1";
			base.AutoScaleDimensions = new SizeF(7f, 12f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = Color.White;
			base.ClientSize = new System.Drawing.Size(534, 583);
			base.Controls.Add(this.labelVersion);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.checkBoxSkip);
			base.Controls.Add(this.buttonClose);
			base.Controls.Add(this.webBrowser);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "PatchNote";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "JSAssist Patch Note";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}