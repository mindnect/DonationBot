using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace JSAssist
{
	public class FormSelectPotPlayer : Form
	{
		private IContainer components;

		private ListBox listBoxWindows;

		private Label label1;

		private Button buttonApply;

		private Button buttonCancel;

		private Button buttonRefresh;

		public FormSelectPotPlayer()
		{
			this.InitializeComponent();
		}

		private void buttonApply_Click(object sender, EventArgs e)
		{
			int selectedIndex = this.listBoxWindows.SelectedIndex;
			if (selectedIndex != -1)
			{
				Program.chatManager.chatTVPot.SelectPotPlayerWindow(selectedIndex);
				base.Close();
			}
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void buttonRefresh_Click(object sender, EventArgs e)
		{
			Program.chatManager.chatTVPot.RefreshPotPlayerHandleList();
			this.RefreshListBox();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void FormSelectPotPlayer_Load(object sender, EventArgs e)
		{
			Program.chatManager.chatTVPot.RefreshPotPlayerHandleList();
			this.RefreshListBox();
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FormSelectPotPlayer));
			this.listBoxWindows = new ListBox();
			this.label1 = new Label();
			this.buttonApply = new Button();
			this.buttonCancel = new Button();
			this.buttonRefresh = new Button();
			base.SuspendLayout();
			this.listBoxWindows.FormattingEnabled = true;
			this.listBoxWindows.ItemHeight = 12;
			this.listBoxWindows.Location = new Point(12, 39);
			this.listBoxWindows.Name = "listBoxWindows";
			this.listBoxWindows.Size = new System.Drawing.Size(313, 76);
			this.listBoxWindows.TabIndex = 0;
			this.listBoxWindows.SelectedIndexChanged += new EventHandler(this.listBoxWindows_SelectedIndexChanged);
			this.label1.AutoSize = true;
			this.label1.Location = new Point(12, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(193, 12);
			this.label1.TabIndex = 1;
			this.label1.Text = "연결할 팟플레이어를 선택해주세요";
			this.buttonApply.Enabled = false;
			this.buttonApply.Location = new Point(12, 121);
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.Size = new System.Drawing.Size(150, 23);
			this.buttonApply.TabIndex = 2;
			this.buttonApply.Text = "적용";
			this.buttonApply.UseVisualStyleBackColor = true;
			this.buttonApply.Click += new EventHandler(this.buttonApply_Click);
			this.buttonCancel.Location = new Point(176, 121);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(150, 23);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "취소";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new EventHandler(this.buttonCancel_Click);
			this.buttonRefresh.Location = new Point(234, 9);
			this.buttonRefresh.Name = "buttonRefresh";
			this.buttonRefresh.Size = new System.Drawing.Size(92, 23);
			this.buttonRefresh.TabIndex = 4;
			this.buttonRefresh.Text = "새로고침";
			this.buttonRefresh.UseVisualStyleBackColor = true;
			this.buttonRefresh.Click += new EventHandler(this.buttonRefresh_Click);
			base.AutoScaleDimensions = new SizeF(7f, 12f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(337, 159);
			base.ControlBox = false;
			base.Controls.Add(this.buttonRefresh);
			base.Controls.Add(this.buttonCancel);
			base.Controls.Add(this.buttonApply);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.listBoxWindows);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.Name = "FormSelectPotPlayer";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Select PotPlayer";
			base.Load += new EventHandler(this.FormSelectPotPlayer_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void listBoxWindows_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.listBoxWindows.SelectedIndex != -1)
			{
				this.buttonApply.Enabled = true;
			}
		}

		private void RefreshListBox()
		{
			this.listBoxWindows.Items.Clear();
			this.buttonApply.Enabled = false;
			WindowInfo[] windowInfoArray = Program.chatManager.chatTVPot.windowPotPlayers;
			for (int i = 0; i < (int)windowInfoArray.Length; i++)
			{
				this.listBoxWindows.Items.Add(string.Concat(new object[] { "[", windowInfoArray[i].pid, "]", windowInfoArray[i].caption }));
			}
		}
	}
}