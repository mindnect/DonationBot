using JSAssist.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace JSAssist
{
	public class ChatSettingYoutube : Form
	{
		private IContainer components;

		private PictureBox pictureBox1;

		private PictureBox pictureBox2;

		private Label label_exit;

		private Label label9;

		private Label label1;

		private Label label2;

		private Label label3;

		private TextBox textBoxYoutubeID;

		private Button buttonApply;

		private Label label4;

		private Label label5;

		public ChatSettingYoutube()
		{
			this.InitializeComponent();
			string str = Program.config.chatYoutubeID;
			if (str != null)
			{
				this.textBoxYoutubeID.Text = str;
			}
		}

		private void ApplyAndClose()
		{
			string text = this.textBoxYoutubeID.Text;
			if (text == null || text == "")
			{
				MessageBox.Show("키값이 입력되지 않았습니다. Youtube 연동이 자동으로 비활성화 됩니다.", "알림");
				Program.config.chatYoutubeID = "";
				FormCoreWindow.inst.SetChatYoutube(false, true);
			}
			else
			{
				Program.config.chatYoutubeID = text;
				FormCoreWindow.inst.SetChatYoutube(true, true);
			}
			base.Close();
		}

		private void buttonApply_Click(object sender, EventArgs e)
		{
			this.ApplyAndClose();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(ChatSettingYoutube));
			this.pictureBox1 = new PictureBox();
			this.pictureBox2 = new PictureBox();
			this.label_exit = new Label();
			this.label9 = new Label();
			this.label1 = new Label();
			this.label2 = new Label();
			this.label3 = new Label();
			this.textBoxYoutubeID = new TextBox();
			this.buttonApply = new Button();
			this.label4 = new Label();
			this.label5 = new Label();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			base.SuspendLayout();
			this.pictureBox1.Image = Resources.chat_setting_youtube_1;
			this.pictureBox1.Location = new Point(54, 51);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(290, 141);
			this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 62;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.MouseDown += new MouseEventHandler(this.pictureBox1_MouseDown);
			this.pictureBox2.Image = Resources.chat_setting_youtube_2;
			this.pictureBox2.Location = new Point(12, 237);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(367, 75);
			this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
			this.pictureBox2.TabIndex = 63;
			this.pictureBox2.TabStop = false;
			this.pictureBox2.MouseDown += new MouseEventHandler(this.pictureBox2_MouseDown);
			this.label_exit.BackColor = Color.FromArgb(0, 180, 204);
			this.label_exit.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label_exit.Location = new Point(362, 3);
			this.label_exit.Name = "label_exit";
			this.label_exit.Size = new System.Drawing.Size(23, 26);
			this.label_exit.TabIndex = 66;
			this.label_exit.Text = "X";
			this.label_exit.Click += new EventHandler(this.label_exit_Click);
			this.label9.Font = new System.Drawing.Font("Malgun Gothic", 15.75f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label9.ForeColor = Color.FromArgb(0, 180, 204);
			this.label9.Location = new Point(12, 9);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(367, 30);
			this.label9.TabIndex = 65;
			this.label9.Text = "Youtube 채팅연동";
			this.label9.TextAlign = ContentAlignment.MiddleCenter;
			this.label9.MouseDown += new MouseEventHandler(this.label9_MouseDown);
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Malgun Gothic", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label1.ForeColor = Color.White;
			this.label1.Location = new Point(51, 195);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(298, 34);
			this.label1.TabIndex = 67;
			this.label1.Text = "Youtube Live 방송 화면에서 'Popout chat' 으로\r\n채팅창을 분리해주세요";
			this.label1.TextAlign = ContentAlignment.MiddleCenter;
			this.label1.MouseDown += new MouseEventHandler(this.label1_MouseDown);
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Malgun Gothic", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label2.ForeColor = Color.White;
			this.label2.Location = new Point(33, 315);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(332, 34);
			this.label2.TabIndex = 68;
			this.label2.Text = "채팅창 주소에서 채팅창의 키값을 아래에 입력해주세요\r\n( 키값은 대부분 11자 입니다 )";
			this.label2.TextAlign = ContentAlignment.MiddleCenter;
			this.label2.MouseDown += new MouseEventHandler(this.label2_MouseDown);
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label3.ForeColor = Color.White;
			this.label3.Location = new Point(119, 399);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(150, 21);
			this.label3.TabIndex = 70;
			this.label3.Text = "Youtube Chat Key";
			this.textBoxYoutubeID.Font = new System.Drawing.Font("Gulim", 12f, FontStyle.Regular, GraphicsUnit.Point, 129);
			this.textBoxYoutubeID.Location = new Point(123, 423);
			this.textBoxYoutubeID.Name = "textBoxYoutubeID";
			this.textBoxYoutubeID.Size = new System.Drawing.Size(145, 26);
			this.textBoxYoutubeID.TabIndex = 69;
			this.textBoxYoutubeID.KeyPress += new KeyPressEventHandler(this.textBoxYoutubeID_KeyPress);
			this.buttonApply.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonApply.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonApply.FlatAppearance.BorderSize = 2;
			this.buttonApply.FlatStyle = FlatStyle.Flat;
			this.buttonApply.Location = new Point(139, 463);
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.Size = new System.Drawing.Size(111, 23);
			this.buttonApply.TabIndex = 71;
			this.buttonApply.Text = "적용 및 닫기";
			this.buttonApply.UseVisualStyleBackColor = false;
			this.buttonApply.Click += new EventHandler(this.buttonApply_Click);
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Malgun Gothic", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label4.ForeColor = Color.White;
			this.label4.Location = new Point(25, 352);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(350, 17);
			this.label4.TabIndex = 72;
			this.label4.Text = "키값은 바뀌지 않기때문에 매번 설정하실 필요는 없습니다";
			this.label4.TextAlign = ContentAlignment.MiddleCenter;
			this.label4.MouseDown += new MouseEventHandler(this.label4_MouseDown);
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Malgun Gothic", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label5.ForeColor = Color.White;
			this.label5.Location = new Point(42, 369);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(310, 13);
			this.label5.TabIndex = 73;
			this.label5.Text = "( 혹시 값이 바뀌는 경우가 생기면 버그리포팅 부탁드려요..! )";
			this.label5.TextAlign = ContentAlignment.MiddleCenter;
			this.label5.MouseDown += new MouseEventHandler(this.label5_MouseDown);
			base.AutoScaleDimensions = new SizeF(7f, 12f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = Color.FromArgb(44, 52, 54);
			base.ClientSize = new System.Drawing.Size(389, 498);
			base.Controls.Add(this.label_exit);
			base.Controls.Add(this.label5);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.buttonApply);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.textBoxYoutubeID);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.label9);
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.pictureBox1);
			this.DoubleBuffered = true;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ChatSettingYoutube";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Youtube 채팅 연동";
			((ISupportInitialize)this.pictureBox1).EndInit();
			((ISupportInitialize)this.pictureBox2).EndInit();
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

		private void label2_MouseDown(object sender, MouseEventArgs e)
		{
			this.DragForm(e);
		}

		private void label4_MouseDown(object sender, MouseEventArgs e)
		{
			this.DragForm(e);
		}

		private void label5_MouseDown(object sender, MouseEventArgs e)
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

		private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
		{
			this.DragForm(e);
		}

		private void textBoxYoutubeID_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{
				this.ApplyAndClose();
			}
		}
	}
}