using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace JSAssist
{
	public class NoticePopup2 : Form
	{
		private IContainer components;

		private ListView listViewNotice;

		private TextBox textBoxNotice;

		private TextBox textBoxStartDelay;

		private TextBox textBoxRepeatDelay;

		private Button buttonPlayOnce;

		private Button buttonRepeat;

		private Button buttonRemove;

		private Label label1;

		private Label label2;

		private ColumnHeader message;

		private ColumnHeader start_delay;

		private ColumnHeader repeat_delay;

		public NoticePopup2()
		{
			this.InitializeComponent();
		}

		private void buttonPlayOnce_Click(object sender, EventArgs e)
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
			this.listViewNotice = new ListView();
			this.message = new ColumnHeader();
			this.start_delay = new ColumnHeader();
			this.repeat_delay = new ColumnHeader();
			this.textBoxNotice = new TextBox();
			this.textBoxStartDelay = new TextBox();
			this.textBoxRepeatDelay = new TextBox();
			this.buttonPlayOnce = new Button();
			this.buttonRepeat = new Button();
			this.buttonRemove = new Button();
			this.label1 = new Label();
			this.label2 = new Label();
			base.SuspendLayout();
			this.listViewNotice.Columns.AddRange(new ColumnHeader[] { this.message, this.start_delay, this.repeat_delay });
			this.listViewNotice.Location = new Point(12, 12);
			this.listViewNotice.Name = "listViewNotice";
			this.listViewNotice.Size = new System.Drawing.Size(372, 162);
			this.listViewNotice.TabIndex = 0;
			this.listViewNotice.UseCompatibleStateImageBehavior = false;
			this.listViewNotice.View = View.Details;
			this.message.Text = "Message";
			this.message.Width = 261;
			this.start_delay.Text = "Start Delay";
			this.start_delay.Width = 54;
			this.repeat_delay.Text = "Repeat Delay";
			this.repeat_delay.Width = 53;
			this.textBoxNotice.Location = new Point(390, 12);
			this.textBoxNotice.Multiline = true;
			this.textBoxNotice.Name = "textBoxNotice";
			this.textBoxNotice.Size = new System.Drawing.Size(279, 106);
			this.textBoxNotice.TabIndex = 1;
			this.textBoxStartDelay.Location = new Point(465, 124);
			this.textBoxStartDelay.Name = "textBoxStartDelay";
			this.textBoxStartDelay.Size = new System.Drawing.Size(56, 21);
			this.textBoxStartDelay.TabIndex = 3;
			this.textBoxRepeatDelay.Location = new Point(611, 124);
			this.textBoxRepeatDelay.Name = "textBoxRepeatDelay";
			this.textBoxRepeatDelay.Size = new System.Drawing.Size(58, 21);
			this.textBoxRepeatDelay.TabIndex = 4;
			this.buttonPlayOnce.Location = new Point(390, 151);
			this.buttonPlayOnce.Name = "buttonPlayOnce";
			this.buttonPlayOnce.Size = new System.Drawing.Size(75, 23);
			this.buttonPlayOnce.TabIndex = 5;
			this.buttonPlayOnce.Text = "1회 입력";
			this.buttonPlayOnce.UseVisualStyleBackColor = true;
			this.buttonPlayOnce.Click += new EventHandler(this.buttonPlayOnce_Click);
			this.buttonRepeat.Location = new Point(492, 151);
			this.buttonRepeat.Name = "buttonRepeat";
			this.buttonRepeat.Size = new System.Drawing.Size(75, 23);
			this.buttonRepeat.TabIndex = 6;
			this.buttonRepeat.Text = "반복등록";
			this.buttonRepeat.UseVisualStyleBackColor = true;
			this.buttonRemove.Location = new Point(596, 149);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(75, 23);
			this.buttonRemove.TabIndex = 7;
			this.buttonRemove.Text = "삭제";
			this.buttonRemove.UseVisualStyleBackColor = true;
			this.label1.AutoSize = true;
			this.label1.Location = new Point(392, 129);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 12);
			this.label1.TabIndex = 8;
			this.label1.Text = "시작 딜레이";
			this.label2.AutoSize = true;
			this.label2.Location = new Point(548, 129);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(57, 12);
			this.label2.TabIndex = 8;
			this.label2.Text = "반복 시간";
			base.AutoScaleDimensions = new SizeF(7f, 12f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(683, 184);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.buttonRemove);
			base.Controls.Add(this.buttonRepeat);
			base.Controls.Add(this.buttonPlayOnce);
			base.Controls.Add(this.textBoxRepeatDelay);
			base.Controls.Add(this.textBoxStartDelay);
			base.Controls.Add(this.textBoxNotice);
			base.Controls.Add(this.listViewNotice);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "NoticePopup2";
			this.Text = "NoticePopup";
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}