using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace JSAssist
{
	public class UserInfo : Form
	{
		private IContainer components;

		private ListView listView1;

		private ColumnHeader ID;

		private ColumnHeader Nickname;

		private ColumnHeader Message;

		private Label label1;

		private Label label2;

		public UserInfo()
		{
			this.InitializeComponent();
		}

		public void AddChat(string nickname, string id, string message)
		{
			ListViewItem listViewItem = new ListViewItem(new string[] { id, nickname, message });
			this.listView1.Items.Add(listViewItem);
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserInfo));
			this.listView1 = new ListView();
			this.ID = new ColumnHeader();
			this.Nickname = new ColumnHeader();
			this.Message = new ColumnHeader();
			this.label1 = new Label();
			this.label2 = new Label();
			base.SuspendLayout();
			this.listView1.Columns.AddRange(new ColumnHeader[] { this.ID, this.Nickname, this.Message });
			this.listView1.Location = new Point(12, 51);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(481, 278);
			this.listView1.TabIndex = 3;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = View.Details;
			this.ID.Text = "ID";
			this.ID.Width = 80;
			this.Nickname.Text = "Nickname";
			this.Nickname.Width = 128;
			this.Message.Text = "Message";
			this.Message.Width = 266;
			this.label1.AutoSize = true;
			this.label1.Location = new Point(10, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(245, 12);
			this.label1.TabIndex = 4;
			this.label1.Text = "축하드려요. 숨겨진 기능 하나를 찾으셨군요.";
			this.label2.AutoSize = true;
			this.label2.Location = new Point(10, 29);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(285, 12);
			this.label2.TabIndex = 5;
			this.label2.Text = "현재 방송에서 해당 유저의 채팅을 따로 표시합니다.";
			base.AutoScaleDimensions = new SizeF(7f, 12f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(500, 341);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.listView1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "UserInfo";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "User Information";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}