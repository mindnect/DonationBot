using JSAssist;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace JSAssist.Widget
{
	public class WidgetChatAddPreset : Form
	{
		public bool createDefault;

		public bool createCurrent;

		public string presetName;

		private IContainer components;

		private Panel panel1;

		private Label label1;

		private Label label9;

		private Button buttonCancel;

		private Button buttonAddPresetCurrent;

		private Button buttonAddPresetDefault;

		private TextBox textBoxPresetName;

		public WidgetChatAddPreset()
		{
			this.InitializeComponent();
			this.createDefault = false;
			this.createCurrent = false;
			this.presetName = "";
		}

		private void buttonAddPresetCurrent_Click(object sender, EventArgs e)
		{
			if (!this.ValidatePresetName())
			{
				MessageBox.Show("프리셋 이름에 특수문자를 제거하고 다시 시도해주세요.", "오류");
				return;
			}
			if (!this.CheckDuplicated())
			{
				MessageBox.Show("이미 존재하는 프리셋 이름입니다. 다른 이름을 사용해주세요.", "오류");
				return;
			}
			this.createCurrent = true;
			this.presetName = this.textBoxPresetName.Text;
			base.Close();
		}

		private void buttonAddPresetDefault_Click(object sender, EventArgs e)
		{
			if (!this.ValidatePresetName())
			{
				MessageBox.Show("프리셋 이름에 특수문자를 제거하고 다시 시도해주세요.", "오류");
				return;
			}
			if (!this.CheckDuplicated())
			{
				MessageBox.Show("이미 존재하는 프리셋 이름입니다. 다른 이름을 사용해주세요.", "오류");
				return;
			}
			this.createDefault = true;
			this.presetName = this.textBoxPresetName.Text;
			base.Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private bool CheckDuplicated()
		{
			bool flag;
			string text = this.textBoxPresetName.Text;
			List<WidgetChatPreset>.Enumerator enumerator = Program.config.listChatPreset.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.presetName != text)
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(WidgetChatAddPreset));
			this.panel1 = new Panel();
			this.label1 = new Label();
			this.label9 = new Label();
			this.buttonCancel = new Button();
			this.buttonAddPresetCurrent = new Button();
			this.buttonAddPresetDefault = new Button();
			this.textBoxPresetName = new TextBox();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.panel1.BorderStyle = BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.label9);
			this.panel1.Controls.Add(this.buttonCancel);
			this.panel1.Controls.Add(this.buttonAddPresetCurrent);
			this.panel1.Controls.Add(this.buttonAddPresetDefault);
			this.panel1.Controls.Add(this.textBoxPresetName);
			this.panel1.Dock = DockStyle.Fill;
			this.panel1.Location = new Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(306, 186);
			this.panel1.TabIndex = 72;
			this.panel1.MouseDown += new MouseEventHandler(this.panel1_MouseDown);
			this.label1.Anchor = AnchorStyles.Top;
			this.label1.Font = new System.Drawing.Font("Malgun Gothic", 9f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label1.ForeColor = Color.White;
			this.label1.Location = new Point(11, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(283, 21);
			this.label1.TabIndex = 77;
			this.label1.Text = "프리셋 이름에 일부 특수문자는 사용할 수 없습니다";
			this.label1.TextAlign = ContentAlignment.MiddleCenter;
			this.label1.MouseDown += new MouseEventHandler(this.label1_MouseDown);
			this.label9.Anchor = AnchorStyles.Top;
			this.label9.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label9.ForeColor = Color.FromArgb(0, 180, 204);
			this.label9.Location = new Point(101, 4);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(111, 21);
			this.label9.TabIndex = 76;
			this.label9.Text = "프리셋 이름";
			this.label9.TextAlign = ContentAlignment.MiddleCenter;
			this.label9.MouseDown += new MouseEventHandler(this.label9_MouseDown);
			this.buttonCancel.Anchor = AnchorStyles.Top;
			this.buttonCancel.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonCancel.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonCancel.FlatAppearance.BorderSize = 2;
			this.buttonCancel.FlatStyle = FlatStyle.Flat;
			this.buttonCancel.Location = new Point(54, 150);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(199, 23);
			this.buttonCancel.TabIndex = 75;
			this.buttonCancel.Text = "취소";
			this.buttonCancel.UseVisualStyleBackColor = false;
			this.buttonCancel.Click += new EventHandler(this.buttonCancel_Click);
			this.buttonAddPresetCurrent.Anchor = AnchorStyles.Top;
			this.buttonAddPresetCurrent.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonAddPresetCurrent.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonAddPresetCurrent.FlatAppearance.BorderSize = 2;
			this.buttonAddPresetCurrent.FlatStyle = FlatStyle.Flat;
			this.buttonAddPresetCurrent.Location = new Point(54, 121);
			this.buttonAddPresetCurrent.Name = "buttonAddPresetCurrent";
			this.buttonAddPresetCurrent.Size = new System.Drawing.Size(199, 23);
			this.buttonAddPresetCurrent.TabIndex = 74;
			this.buttonAddPresetCurrent.Text = "현재값으로 새 프리셋 추가";
			this.buttonAddPresetCurrent.UseVisualStyleBackColor = false;
			this.buttonAddPresetCurrent.Click += new EventHandler(this.buttonAddPresetCurrent_Click);
			this.buttonAddPresetDefault.Anchor = AnchorStyles.Top;
			this.buttonAddPresetDefault.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonAddPresetDefault.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonAddPresetDefault.FlatAppearance.BorderSize = 2;
			this.buttonAddPresetDefault.FlatStyle = FlatStyle.Flat;
			this.buttonAddPresetDefault.Location = new Point(54, 92);
			this.buttonAddPresetDefault.Name = "buttonAddPresetDefault";
			this.buttonAddPresetDefault.Size = new System.Drawing.Size(199, 23);
			this.buttonAddPresetDefault.TabIndex = 73;
			this.buttonAddPresetDefault.Text = "기본값으로 새 프리셋 추가";
			this.buttonAddPresetDefault.UseVisualStyleBackColor = false;
			this.buttonAddPresetDefault.Click += new EventHandler(this.buttonAddPresetDefault_Click);
			this.textBoxPresetName.Anchor = AnchorStyles.Top;
			this.textBoxPresetName.Font = new System.Drawing.Font("Malgun Gothic", 9.75f, FontStyle.Regular, GraphicsUnit.Point, 129);
			this.textBoxPresetName.Location = new Point(54, 28);
			this.textBoxPresetName.Name = "textBoxPresetName";
			this.textBoxPresetName.Size = new System.Drawing.Size(199, 25);
			this.textBoxPresetName.TabIndex = 72;
			base.AutoScaleDimensions = new SizeF(7f, 12f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = Color.FromArgb(44, 52, 54);
			base.ClientSize = new System.Drawing.Size(306, 186);
			base.Controls.Add(this.panel1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "WidgetChatAddPreset";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "프리셋 추가";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			base.ResumeLayout(false);
		}

		private void label1_MouseDown(object sender, MouseEventArgs e)
		{
			this.DragForm(e);
		}

		private void label9_MouseDown(object sender, MouseEventArgs e)
		{
			this.DragForm(e);
		}

		private void panel1_MouseDown(object sender, MouseEventArgs e)
		{
			this.DragForm(e);
		}

		private bool ValidatePresetName()
		{
			string text = this.textBoxPresetName.Text;
			if (text.Contains<char>('\u0060'))
			{
				return false;
			}
			if (text.Contains<char>('?'))
			{
				return false;
			}
			if (text.Contains<char>('~'))
			{
				return false;
			}
			if (text.Contains<char>('!'))
			{
				return false;
			}
			if (text.Contains<char>('@'))
			{
				return false;
			}
			if (text.Contains<char>('#'))
			{
				return false;
			}
			if (text.Contains<char>('$'))
			{
				return false;
			}
			if (text.Contains<char>('%'))
			{
				return false;
			}
			if (text.Contains<char>('\u005E'))
			{
				return false;
			}
			if (text.Contains<char>('&'))
			{
				return false;
			}
			if (text.Contains<char>('('))
			{
				return false;
			}
			if (text.Contains<char>(')'))
			{
				return false;
			}
			if (text.Contains<char>('+'))
			{
				return false;
			}
			if (text.Contains<char>('='))
			{
				return false;
			}
			if (text.Contains<char>('\\'))
			{
				return false;
			}
			if (text.Contains<char>('|'))
			{
				return false;
			}
			if (text.Length != 0 && !(text == ""))
			{
				return true;
			}
			return false;
		}
	}
}