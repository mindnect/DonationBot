using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace JSAssist
{
	public class NoticePopup : Form
	{
		private static List<NoticePopupData> listNotice;

		private static string saveFileName;

		private bool isCreateNew;

		private IContainer components;

		private ListView listViewNotice;

		private TextBox textBoxNotice;

		private Button buttonPlayOnce;

		private Button buttonApply;

		private Button buttonRemove;

		private Label label1;

		private Label label2;

		private ColumnHeader message;

		private ColumnHeader start_delay;

		private ColumnHeader repeat_delay;

		private Button button1;

		private CheckBox checkBoxIsEnabled;

		private NumericUpDown numStartDelay;

		private NumericUpDown numRepeatDelay;

		private ColumnHeader enabled;

		static NoticePopup()
		{
			NoticePopup.saveFileName = "noticepopup.dat";
		}

		public NoticePopup()
		{
			this.InitializeComponent();
			this.isCreateNew = true;
			this.RefreshList();
		}

		private void buttonApply_Click(object sender, EventArgs e)
		{
			NoticePopupData item;
			if ((int)this.numRepeatDelay.Value < 5)
			{
				MessageBox.Show("반복 시간은 5초 이상으로 설정해주세요.");
				return;
			}
			if (!this.isCreateNew)
			{
				if (this.listViewNotice.FocusedItem == null)
				{
					return;
				}
				int index = this.listViewNotice.FocusedItem.Index;
				item = NoticePopup.listNotice[index];
			}
			else
			{
				item = new NoticePopupData();
			}
			item.message = this.textBoxNotice.Text;
			item.startDelay = (float)((int)this.numStartDelay.Value);
			item.repeatDelay = (float)((int)this.numRepeatDelay.Value);
			item.isEnabled = this.checkBoxIsEnabled.Checked;
			if (this.isCreateNew)
			{
				NoticePopup.listNotice.Add(item);
			}
			this.ResetInput();
			this.RefreshList();
			NoticePopup.SaveNoticeData();
		}

		private void buttonNewNotice_Click(object sender, EventArgs e)
		{
			this.ResetInput();
			this.isCreateNew = true;
		}

		private void buttonPlayOnce_Click(object sender, EventArgs e)
		{
			string text = this.textBoxNotice.Text;
		}

		private void buttonRemove_Click(object sender, EventArgs e)
		{
			if (this.listViewNotice.FocusedItem == null)
			{
				return;
			}
			int index = this.listViewNotice.FocusedItem.Index;
			NoticePopup.listNotice.RemoveAt(index);
			this.RefreshList();
			NoticePopup.SaveNoticeData();
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
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(NoticePopup));
			this.listViewNotice = new ListView();
			this.message = new ColumnHeader();
			this.start_delay = new ColumnHeader();
			this.repeat_delay = new ColumnHeader();
			this.enabled = new ColumnHeader();
			this.textBoxNotice = new TextBox();
			this.buttonPlayOnce = new Button();
			this.buttonApply = new Button();
			this.buttonRemove = new Button();
			this.label1 = new Label();
			this.label2 = new Label();
			this.button1 = new Button();
			this.checkBoxIsEnabled = new CheckBox();
			this.numStartDelay = new NumericUpDown();
			this.numRepeatDelay = new NumericUpDown();
			((ISupportInitialize)this.numStartDelay).BeginInit();
			((ISupportInitialize)this.numRepeatDelay).BeginInit();
			base.SuspendLayout();
			this.listViewNotice.Columns.AddRange(new ColumnHeader[] { this.message, this.start_delay, this.repeat_delay, this.enabled });
			this.listViewNotice.Location = new Point(12, 12);
			this.listViewNotice.Name = "listViewNotice";
			this.listViewNotice.Size = new System.Drawing.Size(372, 188);
			this.listViewNotice.TabIndex = 0;
			this.listViewNotice.UseCompatibleStateImageBehavior = false;
			this.listViewNotice.View = View.Details;
			this.listViewNotice.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
			this.message.Text = "메세지";
			this.message.Width = 207;
			this.start_delay.Text = "시작시간";
			this.start_delay.Width = 49;
			this.repeat_delay.Text = "반복시간";
			this.repeat_delay.Width = 50;
			this.enabled.Text = "활성화";
			this.textBoxNotice.Location = new Point(390, 12);
			this.textBoxNotice.Multiline = true;
			this.textBoxNotice.Name = "textBoxNotice";
			this.textBoxNotice.Size = new System.Drawing.Size(279, 106);
			this.textBoxNotice.TabIndex = 1;
			this.buttonPlayOnce.Location = new Point(390, 180);
			this.buttonPlayOnce.Name = "buttonPlayOnce";
			this.buttonPlayOnce.Size = new System.Drawing.Size(75, 23);
			this.buttonPlayOnce.TabIndex = 5;
			this.buttonPlayOnce.Text = "1회 재생";
			this.buttonPlayOnce.UseVisualStyleBackColor = true;
			this.buttonPlayOnce.Click += new EventHandler(this.buttonPlayOnce_Click);
			this.buttonApply.Location = new Point(492, 180);
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.Size = new System.Drawing.Size(75, 23);
			this.buttonApply.TabIndex = 6;
			this.buttonApply.Text = "추가/적용";
			this.buttonApply.UseVisualStyleBackColor = true;
			this.buttonApply.Click += new EventHandler(this.buttonApply_Click);
			this.buttonRemove.Location = new Point(594, 180);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(75, 23);
			this.buttonRemove.TabIndex = 7;
			this.buttonRemove.Text = "삭제";
			this.buttonRemove.UseVisualStyleBackColor = true;
			this.buttonRemove.Click += new EventHandler(this.buttonRemove_Click);
			this.label1.AutoSize = true;
			this.label1.Location = new Point(392, 129);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(75, 12);
			this.label1.TabIndex = 8;
			this.label1.Text = "시작시간(초)";
			this.label2.AutoSize = true;
			this.label2.Location = new Point(535, 129);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(75, 12);
			this.label2.TabIndex = 8;
			this.label2.Text = "반복시간(초)";
			this.button1.Location = new Point(390, 151);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(177, 23);
			this.button1.TabIndex = 9;
			this.button1.Text = "새 알림 항목 만들기";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new EventHandler(this.buttonNewNotice_Click);
			this.checkBoxIsEnabled.AutoSize = true;
			this.checkBoxIsEnabled.Checked = true;
			this.checkBoxIsEnabled.CheckState = CheckState.Checked;
			this.checkBoxIsEnabled.Location = new Point(599, 155);
			this.checkBoxIsEnabled.Name = "checkBoxIsEnabled";
			this.checkBoxIsEnabled.Size = new System.Drawing.Size(60, 16);
			this.checkBoxIsEnabled.TabIndex = 10;
			this.checkBoxIsEnabled.Text = "활성화";
			this.checkBoxIsEnabled.UseVisualStyleBackColor = true;
			this.numStartDelay.Location = new Point(468, 125);
			this.numStartDelay.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
			this.numStartDelay.Name = "numStartDelay";
			this.numStartDelay.Size = new System.Drawing.Size(58, 21);
			this.numStartDelay.TabIndex = 11;
			this.numStartDelay.ValueChanged += new EventHandler(this.numStartDelay_ValueChanged);
			this.numRepeatDelay.Location = new Point(613, 125);
			this.numRepeatDelay.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
			this.numRepeatDelay.Name = "numRepeatDelay";
			this.numRepeatDelay.Size = new System.Drawing.Size(58, 21);
			this.numRepeatDelay.TabIndex = 12;
			this.numRepeatDelay.ValueChanged += new EventHandler(this.numRepeatDelay_ValueChanged);
			base.AutoScaleDimensions = new SizeF(7f, 12f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(683, 211);
			base.Controls.Add(this.numRepeatDelay);
			base.Controls.Add(this.numStartDelay);
			base.Controls.Add(this.checkBoxIsEnabled);
			base.Controls.Add(this.button1);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.buttonRemove);
			base.Controls.Add(this.buttonApply);
			base.Controls.Add(this.buttonPlayOnce);
			base.Controls.Add(this.textBoxNotice);
			base.Controls.Add(this.listViewNotice);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "NoticePopup";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Notice Popup Editor";
			((ISupportInitialize)this.numStartDelay).EndInit();
			((ISupportInitialize)this.numRepeatDelay).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.isCreateNew = false;
			if (this.listViewNotice.FocusedItem == null)
			{
				return;
			}
			int index = this.listViewNotice.FocusedItem.Index;
			ListViewItem.ListViewSubItemCollection subItems = this.listViewNotice.Items[index].SubItems;
			string text = subItems[0].Text;
			string str = subItems[1].Text;
			string text1 = subItems[2].Text;
			bool flag = false;
			if (subItems[3].Text == "Yes")
			{
				flag = true;
			}
			this.textBoxNotice.Text = text;
			this.numStartDelay.Value = int.Parse(str);
			this.numRepeatDelay.Value = int.Parse(text1);
			this.checkBoxIsEnabled.Checked = flag;
		}

		public static void LoadNoticeData()
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			if (!File.Exists(NoticePopup.saveFileName))
			{
				NoticePopup.listNotice = new List<NoticePopupData>();
			}
			else
			{
				FileStream fileStream = File.Open(NoticePopup.saveFileName, FileMode.Open);
				NoticePopup.listNotice = (List<NoticePopupData>)binaryFormatter.Deserialize(fileStream);
				fileStream.Close();
				foreach (NoticePopupData noticePopupDatum in NoticePopup.listNotice)
				{
					noticePopupDatum.startDelayElapsed = 0f;
					noticePopupDatum.repeatDelayElapsed = 0f;
				}
			}
		}

		private void numRepeatDelay_ValueChanged(object sender, EventArgs e)
		{
		}

		private void numStartDelay_ValueChanged(object sender, EventArgs e)
		{
		}

		private void RefreshList()
		{
			this.listViewNotice.Items.Clear();
			foreach (NoticePopupData noticePopupDatum in NoticePopup.listNotice)
			{
				string[] str = new string[] { noticePopupDatum.message, noticePopupDatum.startDelay.ToString(), noticePopupDatum.repeatDelay.ToString(), null };
				str[3] = (noticePopupDatum.isEnabled ? "Yes" : "No");
				ListViewItem listViewItem = new ListViewItem(str);
				this.listViewNotice.Items.Add(listViewItem);
			}
		}

		private void ResetInput()
		{
			this.textBoxNotice.Text = "";
			this.numStartDelay.Value = decimal.Zero;
			this.numRepeatDelay.Value = decimal.Zero;
			this.checkBoxIsEnabled.Checked = true;
		}

		public static void SaveNoticeData()
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = File.Create(NoticePopup.saveFileName);
			binaryFormatter.Serialize(fileStream, NoticePopup.listNotice);
			fileStream.Close();
		}

		public static void UpdateNoticePopup(float dt)
		{
			foreach (NoticePopupData noticePopupDatum in NoticePopup.listNotice)
			{
				if (!noticePopupDatum.isEnabled)
				{
					continue;
				}
				if (noticePopupDatum.startDelayElapsed < noticePopupDatum.startDelay)
				{
					NoticePopupData noticePopupDatum1 = noticePopupDatum;
					noticePopupDatum1.startDelayElapsed = noticePopupDatum1.startDelayElapsed + dt;
				}
				else if (noticePopupDatum.repeatDelayElapsed < noticePopupDatum.repeatDelay)
				{
					NoticePopupData noticePopupDatum2 = noticePopupDatum;
					noticePopupDatum2.repeatDelayElapsed = noticePopupDatum2.repeatDelayElapsed + dt;
				}
				else
				{
					noticePopupDatum.repeatDelayElapsed = 0f;
				}
			}
		}
	}
}