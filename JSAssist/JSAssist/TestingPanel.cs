using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace JSAssist
{
	public class TestingPanel : Form
	{
		private IContainer components;

		private Label label1;

		private TextBox textBoxSponsorNickname;

		private TextBox textBoxSponsorAmount;

		private Label label2;

		private Label label3;

		private Label label4;

		private TextBox textBoxSponsorMessage;

		private Button buttonSponsor;

		private TextBox textBoxChatMessage;

		private Label label5;

		private Label label7;

		private TextBox textBoxChatNickname;

		private Label label6;

		private Label label8;

		private Button buttonChat;

		public TestingPanel()
		{
			this.InitializeComponent();
		}

		private void buttonChat_Click(object sender, EventArgs e)
		{
			string text = this.textBoxChatNickname.Text;
			string str = this.textBoxChatMessage.Text;
			if (text == null || text == "")
			{
				MessageBox.Show("닉네임을 입력해주세요.");
				return;
			}
			if (str != null && !(str == ""))
			{
				return;
			}
			MessageBox.Show("채팅 메세지를 입력해주세요.");
		}

		private void buttonSponsor_Click(object sender, EventArgs e)
		{
			string text = this.textBoxSponsorNickname.Text;
			string str = this.textBoxSponsorAmount.Text;
			string text1 = this.textBoxSponsorMessage.Text;
			if (str == null || str == "")
			{
				MessageBox.Show("후원금액을 입력해주세요.");
				return;
			}
			if (text == null || text == "")
			{
				MessageBox.Show("닉네임을 입력해주세요.");
				return;
			}
			int num = 0;
			try
			{
				num = int.Parse(str);
			}
			catch
			{
				MessageBox.Show("후원금액이 잘못되었습니다.");
				return;
			}
			if (num < 100 || num > 10000000)
			{
				MessageBox.Show("후원금액이 잘못되었습니다.");
				return;
			}
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(TestingPanel));
			this.label1 = new Label();
			this.textBoxSponsorNickname = new TextBox();
			this.textBoxSponsorAmount = new TextBox();
			this.label2 = new Label();
			this.label3 = new Label();
			this.label4 = new Label();
			this.textBoxSponsorMessage = new TextBox();
			this.buttonSponsor = new Button();
			this.textBoxChatMessage = new TextBox();
			this.label5 = new Label();
			this.label7 = new Label();
			this.textBoxChatNickname = new TextBox();
			this.label6 = new Label();
			this.label8 = new Label();
			this.buttonChat = new Button();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Gulim", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label1.Location = new Point(39, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(395, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "JSAssist Core와 Viewer의 연동 테스트를 위한 기능입니다.";
			this.textBoxSponsorNickname.Location = new Point(93, 58);
			this.textBoxSponsorNickname.Name = "textBoxSponsorNickname";
			this.textBoxSponsorNickname.Size = new System.Drawing.Size(100, 21);
			this.textBoxSponsorNickname.TabIndex = 1;
			this.textBoxSponsorAmount.Location = new Point(274, 58);
			this.textBoxSponsorAmount.Name = "textBoxSponsorAmount";
			this.textBoxSponsorAmount.Size = new System.Drawing.Size(100, 21);
			this.textBoxSponsorAmount.TabIndex = 2;
			this.textBoxSponsorAmount.TextChanged += new EventHandler(this.textBox2_TextChanged);
			this.label2.AutoSize = true;
			this.label2.Location = new Point(12, 62);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 12);
			this.label2.TabIndex = 3;
			this.label2.Text = "닉네임";
			this.label3.AutoSize = true;
			this.label3.Location = new Point(215, 62);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(53, 12);
			this.label3.TabIndex = 4;
			this.label3.Text = "후원금액";
			this.label3.Click += new EventHandler(this.label3_Click);
			this.label4.AutoSize = true;
			this.label4.Location = new Point(12, 90);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(69, 12);
			this.label4.TabIndex = 5;
			this.label4.Text = "후원 메세지";
			this.textBoxSponsorMessage.Location = new Point(93, 85);
			this.textBoxSponsorMessage.MaxLength = 30;
			this.textBoxSponsorMessage.Name = "textBoxSponsorMessage";
			this.textBoxSponsorMessage.Size = new System.Drawing.Size(281, 21);
			this.textBoxSponsorMessage.TabIndex = 6;
			this.buttonSponsor.Location = new Point(387, 84);
			this.buttonSponsor.Name = "buttonSponsor";
			this.buttonSponsor.Size = new System.Drawing.Size(75, 23);
			this.buttonSponsor.TabIndex = 7;
			this.buttonSponsor.Text = "전송";
			this.buttonSponsor.UseVisualStyleBackColor = true;
			this.buttonSponsor.Click += new EventHandler(this.buttonSponsor_Click);
			this.textBoxChatMessage.Location = new Point(93, 176);
			this.textBoxChatMessage.MaxLength = 100;
			this.textBoxChatMessage.Name = "textBoxChatMessage";
			this.textBoxChatMessage.Size = new System.Drawing.Size(281, 21);
			this.textBoxChatMessage.TabIndex = 13;
			this.label5.AutoSize = true;
			this.label5.Location = new Point(12, 181);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(69, 12);
			this.label5.TabIndex = 12;
			this.label5.Text = "채팅 메세지";
			this.label7.AutoSize = true;
			this.label7.Location = new Point(12, 153);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(41, 12);
			this.label7.TabIndex = 10;
			this.label7.Text = "닉네임";
			this.textBoxChatNickname.Location = new Point(93, 149);
			this.textBoxChatNickname.Name = "textBoxChatNickname";
			this.textBoxChatNickname.Size = new System.Drawing.Size(100, 21);
			this.textBoxChatNickname.TabIndex = 8;
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Gulim", 11.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label6.Location = new Point(11, 39);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(131, 15);
			this.label6.TabIndex = 14;
			this.label6.Text = "후원 기능 테스트";
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Gulim", 11.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label8.Location = new Point(11, 130);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(93, 15);
			this.label8.TabIndex = 15;
			this.label8.Text = "채팅 테스트";
			this.buttonChat.Location = new Point(387, 174);
			this.buttonChat.Name = "buttonChat";
			this.buttonChat.Size = new System.Drawing.Size(75, 23);
			this.buttonChat.TabIndex = 16;
			this.buttonChat.Text = "전송";
			this.buttonChat.UseVisualStyleBackColor = true;
			this.buttonChat.Click += new EventHandler(this.buttonChat_Click);
			base.AutoScaleDimensions = new SizeF(7f, 12f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(474, 212);
			base.Controls.Add(this.buttonChat);
			base.Controls.Add(this.label8);
			base.Controls.Add(this.label6);
			base.Controls.Add(this.textBoxChatMessage);
			base.Controls.Add(this.label5);
			base.Controls.Add(this.label7);
			base.Controls.Add(this.textBoxChatNickname);
			base.Controls.Add(this.buttonSponsor);
			base.Controls.Add(this.textBoxSponsorMessage);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.textBoxSponsorAmount);
			base.Controls.Add(this.textBoxSponsorNickname);
			base.Controls.Add(this.label1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "TestingPanel";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "JSAssist Testing Panel";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void label3_Click(object sender, EventArgs e)
		{
		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{
		}
	}
}