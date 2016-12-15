using JSAssist.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace JSAssist
{
	public class ChatSettingTwitch : Form
	{
		private IContainer components;

		private TextBox textBoxTwitchID;

		private Button buttonApply;

		private Label label2;

		private PictureBox pictureBox1;

		private Label label9;

		private Label label1;

		private Label label_exit;

		public ChatSettingTwitch()
		{
			this.InitializeComponent();
			string str = Program.config.chatTwitchID;
			if (str != null)
			{
				this.textBoxTwitchID.Text = str;
			}
		}

		private void ApplyAndClose()
		{
			string text = this.textBoxTwitchID.Text;
			if (text == null || text == "")
			{
				MessageBox.Show("아이디가 입력되지 않았습니다. Twitch 연동이 자동으로 비활성화 됩니다.", "알림");
				Program.config.chatTwitchID = "";
				FormCoreWindow.inst.SetChatTwitch(false, true);
			}
			else
			{
				Program.config.chatTwitchID = text;
				FormCoreWindow.inst.SetChatTwitch(true, true);
			}
			base.Close();
		}

		private void buttonApply_Click(object sender, EventArgs e)
		{
			this.ApplyAndClose();
		}

		private void ChatSettingTwitch_MouseDown(object sender, MouseEventArgs e)
		{
			this.DragForm(e);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void DragForm(MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				Win32.ReleaseCapture();
				Win32.SendMessage(base.Handle, 161, 2, 0);
			}
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ChatSettingTwitch));
			this.textBoxTwitchID = new TextBox();
			this.buttonApply = new Button();
			this.label2 = new Label();
			this.pictureBox1 = new PictureBox();
			this.label9 = new Label();
			this.label1 = new Label();
			this.label_exit = new Label();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.textBoxTwitchID.Font = new System.Drawing.Font("Gulim", 12f, FontStyle.Regular, GraphicsUnit.Point, 129);
			this.textBoxTwitchID.Location = new Point(89, 158);
			this.textBoxTwitchID.Name = "textBoxTwitchID";
			this.textBoxTwitchID.Size = new System.Drawing.Size(145, 26);
			this.textBoxTwitchID.TabIndex = 1;
			this.textBoxTwitchID.KeyPress += new KeyPressEventHandler(this.textBoxTwitchID_KeyPress);
			this.buttonApply.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonApply.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonApply.FlatAppearance.BorderSize = 2;
			this.buttonApply.FlatStyle = FlatStyle.Flat;
			this.buttonApply.Location = new Point(106, 199);
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.Size = new System.Drawing.Size(111, 23);
			this.buttonApply.TabIndex = 59;
			this.buttonApply.Text = "적용 및 닫기";
			this.buttonApply.UseVisualStyleBackColor = false;
			this.buttonApply.Click += new EventHandler(this.buttonApply_Click);
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label2.ForeColor = Color.White;
			this.label2.Location = new Point(121, 134);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(83, 21);
			this.label2.TabIndex = 60;
			this.label2.Text = "Twitch ID";
			this.pictureBox1.Image = Resources.chat_setting_twitch;
			this.pictureBox1.Location = new Point(48, 58);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(238, 48);
			this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 61;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.MouseDown += new MouseEventHandler(this.pictureBox1_MouseDown);
			this.label9.Font = new System.Drawing.Font("Malgun Gothic", 15.75f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label9.ForeColor = Color.FromArgb(0, 180, 204);
			this.label9.Location = new Point(12, 9);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(309, 30);
			this.label9.TabIndex = 62;
			this.label9.Text = "Twitch 채팅연동";
			this.label9.TextAlign = ContentAlignment.MiddleCenter;
			this.label9.MouseDown += new MouseEventHandler(this.label9_MouseDown);
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Malgun Gothic", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label1.ForeColor = Color.White;
			this.label1.Location = new Point(24, 109);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(288, 17);
			this.label1.TabIndex = 63;
			this.label1.Text = "자신의 Twitch ID를 입력 후 적용을 눌러주세요";
			this.label1.MouseDown += new MouseEventHandler(this.label1_MouseDown);
			this.label_exit.BackColor = Color.FromArgb(0, 180, 204);
			this.label_exit.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label_exit.Location = new Point(298, 3);
			this.label_exit.Name = "label_exit";
			this.label_exit.Size = new System.Drawing.Size(23, 26);
			this.label_exit.TabIndex = 64;
			this.label_exit.Text = "X";
			this.label_exit.Click += new EventHandler(this.label_exit_Click);
			base.AutoScaleDimensions = new SizeF(7f, 12f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = Color.FromArgb(44, 52, 54);
			base.ClientSize = new System.Drawing.Size(324, 232);
			base.Controls.Add(this.label_exit);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.label9);
			base.Controls.Add(this.pictureBox1);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.buttonApply);
			base.Controls.Add(this.textBoxTwitchID);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ChatSettingTwitch";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Twitch 채팅 연동";
			base.MouseDown += new MouseEventHandler(this.ChatSettingTwitch_MouseDown);
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void label_exit_Click(object sender, EventArgs e)
		{
			this.ApplyAndClose();
		}

		private void label1_MouseDown(object sender, MouseEventArgs e)
		{
			this.DragForm(e);
		}

		private void label9_MouseDown(object sender, MouseEventArgs e)
		{
			this.DragForm(e);
		}

		private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
		{
			this.DragForm(e);
		}

		private void textBoxTwitchID_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				this.ApplyAndClose();
			}
		}
	}
}