using JSAssist.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace JSAssist
{
	public class FormProgramInfo : Form
	{
		private IContainer components;

		private PictureBox pictureBox1;

		private Label label9;

		private PictureBox pictureBox2;

		private Label label1;

		private Label label2;

		private Button buttonApply;

		private Label label4;

		private Label label3;

		public FormProgramInfo()
		{
			this.InitializeComponent();
		}

		private void buttonApply_Click(object sender, EventArgs e)
		{
			base.Close();
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

		private void FormProgramInfo_MouseDown(object sender, MouseEventArgs e)
		{
			this.DragForm(e);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FormProgramInfo));
			this.pictureBox1 = new PictureBox();
			this.label9 = new Label();
			this.pictureBox2 = new PictureBox();
			this.label1 = new Label();
			this.label2 = new Label();
			this.buttonApply = new Button();
			this.label4 = new Label();
			this.label3 = new Label();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			((ISupportInitialize)this.pictureBox2).BeginInit();
			base.SuspendLayout();
			this.pictureBox1.Image = Resources.logo;
			this.pictureBox1.Location = new Point(170, 6);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(156, 60);
			this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 9;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.MouseDown += new MouseEventHandler(this.pictureBox1_MouseDown);
			this.label9.Font = new System.Drawing.Font("Malgun Gothic", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label9.ForeColor = Color.White;
			this.label9.Location = new Point(12, 68);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(478, 43);
			this.label9.TabIndex = 32;
			this.label9.Text = "JSAssist는 Js-Almighty 에서 배포하는 소프트웨어이며,\r\n누구나 사용에 제한 없이 무료로 사용가능합니다.";
			this.label9.TextAlign = ContentAlignment.MiddleCenter;
			this.label9.MouseDown += new MouseEventHandler(this.label9_MouseDown);
			this.pictureBox2.Image = Resources.mail;
			this.pictureBox2.Location = new Point(156, 143);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(194, 21);
			this.pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
			this.pictureBox2.TabIndex = 33;
			this.pictureBox2.TabStop = false;
			this.pictureBox2.Click += new EventHandler(this.pictureBox2_Click);
			this.label1.Font = new System.Drawing.Font("Malgun Gothic", 11.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label1.ForeColor = Color.White;
			this.label1.Location = new Point(12, 115);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(478, 28);
			this.label1.TabIndex = 34;
			this.label1.Text = "연락처";
			this.label1.TextAlign = ContentAlignment.MiddleCenter;
			this.label1.MouseDown += new MouseEventHandler(this.label1_MouseDown);
			this.label2.Font = new System.Drawing.Font("Malgun Gothic", 11.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label2.ForeColor = Color.White;
			this.label2.Location = new Point(12, 177);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(478, 28);
			this.label2.TabIndex = 35;
			this.label2.Text = "도움주기";
			this.label2.TextAlign = ContentAlignment.MiddleCenter;
			this.label2.MouseDown += new MouseEventHandler(this.label2_MouseDown);
			this.buttonApply.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonApply.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonApply.FlatAppearance.BorderSize = 2;
			this.buttonApply.FlatStyle = FlatStyle.Flat;
			this.buttonApply.Location = new Point(196, 280);
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.Size = new System.Drawing.Size(111, 23);
			this.buttonApply.TabIndex = 60;
			this.buttonApply.Text = "닫기";
			this.buttonApply.UseVisualStyleBackColor = false;
			this.buttonApply.Click += new EventHandler(this.buttonApply_Click);
			this.label4.Font = new System.Drawing.Font("Malgun Gothic", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label4.ForeColor = Color.White;
			this.label4.Location = new Point(12, 224);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(478, 25);
			this.label4.TabIndex = 61;
			this.label4.Text = "페이팔 코리아 기부기능에 문제가 있어 삭제하였습니다.";
			this.label4.TextAlign = ContentAlignment.MiddleCenter;
			this.label3.Font = new System.Drawing.Font("Malgun Gothic", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label3.ForeColor = Color.White;
			this.label3.Location = new Point(12, 202);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(478, 25);
			this.label3.TabIndex = 36;
			this.label3.Text = "JSAssist 개발 및 유지에 도움을 주실 수 있습니다.";
			this.label3.TextAlign = ContentAlignment.MiddleCenter;
			this.label3.MouseDown += new MouseEventHandler(this.label3_MouseDown);
			base.AutoScaleDimensions = new SizeF(7f, 12f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = Color.FromArgb(44, 52, 54);
			base.ClientSize = new System.Drawing.Size(502, 310);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.buttonApply);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.pictureBox2);
			base.Controls.Add(this.label9);
			base.Controls.Add(this.pictureBox1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FormProgramInfo";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "JSAssist 정보";
			base.MouseDown += new MouseEventHandler(this.FormProgramInfo_MouseDown);
			((ISupportInitialize)this.pictureBox1).EndInit();
			((ISupportInitialize)this.pictureBox2).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void label1_MouseDown(object sender, MouseEventArgs e)
		{
			this.DragForm(e);
		}

		private void label2_MouseDown(object sender, MouseEventArgs e)
		{
			this.DragForm(e);
		}

		private void label3_MouseDown(object sender, MouseEventArgs e)
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

		private void pictureBox2_Click(object sender, EventArgs e)
		{
			Process.Start("mailto:jsalmighty1989@gmail.com");
		}

		private void pictureBox3_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=jsalmighty1989%40gmail%2ecom&lc=KR&item_name=JSAssist%20%28Jisoo%2c%20Kim%29&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted");
		}
	}
}