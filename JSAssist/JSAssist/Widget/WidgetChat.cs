using JSAssist;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace JSAssist.Widget
{
	public class WidgetChat : Form
	{
		private string[] strTheme = new string[] { "기본테마" };

		private string[] strThemeValue = new string[] { "default" };

		private string[] strPlatform = new string[] { "모두보기(기본)", "Twitch", "Youtube Live", "Daum TV Pot" };

		private string[] strPlatformValue = new string[] { "all", "twitch", "youtube", "tvpot" };

		private string[] strAnimation = new string[] { "페이드", "사용안함" };

		private string[] strAnimationValue = new string[] { "fade", "none" };

		private string[] strFont = new string[] { "제주고딕", "한나체", "나눔고딕", "나눔손글씨 붓", "나눔손글씨 펜", "본고딕" };

		private string[] strFontValue = new string[] { "Jeju Gothic", "Hanna", "Nanum Gothic", "Nanum Brush Script", "Nanum Pen Script", "Noto Sans KR" };

		private WidgetChatPreset currentPreset;

		private int currentPresetIdx;

		private bool disableSend;

		private IContainer components;

		private Panel panelDetail;

		private Label label_exit;

		private ListBox listBoxPreset;

		private Button buttonPresetCreate;

		private Button buttonPresetRemove;

		private ComboBox comboBoxTheme;

		private Label label1;

		private Label label9;

		private ComboBox comboBoxPlatform;

		private Label label3;

		private CheckBox checkBoxPlatformIcon;

		private Label label2;

		private Label label6;

		private ComboBox comboBoxFont;

		private Label label5;

		private ComboBox comboBoxAnimation;

		private Label label4;

		private Label labelChatFontColor;

		private Label label11;

		private Label label12;

		private Label labelUsernameFontColor;

		private Label label7;

		private ColorDialog colorDialog;

		private Label label15;

		private Label labelBackgroundColor;

		private Label label14;

		private Label label16;

		private Label labelChatBackgroundColor;

		private Label label18;

		private NumericUpDown numChatBackgroundAlpha;

		private NumericUpDown numBackgroundAlpha;

		private NumericUpDown numChatFont;

		private NumericUpDown numUsernameFont;

		private Label label13;

		private Label label10;

		private NumericUpDown numChatFade;

		private Label label8;

		private Label label17;

		private Panel panel1;

		private Label label20;

		private Label label19;

		private TextBox textBoxChatURL;

		private Label label21;

		private Button buttonClose;

		public WidgetChat()
		{
			this.disableSend = true;
			this.InitializeComponent();
			this.InitPresetList();
			this.FillComboBox();
			this.ApplyPresetValueToControl();
			this.disableSend = false;
		}

		private void ApplyPresetValueToControl()
		{
			this.disableSend = true;
			this.comboBoxTheme.SelectedItem = this.GetStringTheme(this.currentPreset.theme);
			this.checkBoxPlatformIcon.Checked = this.currentPreset.platformIcon;
			this.comboBoxPlatform.SelectedItem = this.GetStringPlatform(this.currentPreset.platform);
			this.comboBoxAnimation.SelectedItem = this.GetStringAnimation(this.currentPreset.animation);
			this.numChatFade.Value = (int)this.currentPreset.chatFade;
			this.comboBoxFont.SelectedItem = this.GetStringFont(this.currentPreset.font);
			this.numUsernameFont.Value = (int)this.currentPreset.fontUsernameSize;
			this.labelUsernameFontColor.BackColor = this.StringToColor(this.currentPreset.fontUsernameColor);
			this.numChatFont.Value = (int)this.currentPreset.fontChatSize;
			this.labelChatFontColor.BackColor = this.StringToColor(this.currentPreset.fontChatColor);
			this.labelBackgroundColor.BackColor = this.StringToColor(this.currentPreset.backgroundColor);
			this.numBackgroundAlpha.Value = this.currentPreset.backgroundAlpha;
			this.labelChatBackgroundColor.BackColor = this.StringToColor(this.currentPreset.chatBackgroundColor);
			this.numChatBackgroundAlpha.Value = this.currentPreset.chatBackgroundAlpha;
			this.disableSend = false;
		}

		private void ApplyToChat()
		{
			if (!this.disableSend)
			{
				Program.server.SendChatPreset(this.currentPreset);
			}
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void buttonPresetCreate_Click(object sender, EventArgs e)
		{
			WidgetChatAddPreset widgetChatAddPreset = new WidgetChatAddPreset();
			widgetChatAddPreset.ShowDialog();
			if (widgetChatAddPreset.createCurrent || widgetChatAddPreset.createDefault)
			{
				WidgetChatPreset widgetChatPreset = new WidgetChatPreset();
				if (widgetChatAddPreset.createCurrent)
				{
					widgetChatPreset.CopyFrom(this.currentPreset);
				}
				widgetChatPreset.presetName = widgetChatAddPreset.presetName;
				Program.config.listChatPreset.Add(widgetChatPreset);
				this.RefreshPresetList();
				this.SelectPreset(Program.config.listChatPreset.Count - 1);
			}
		}

		private void buttonPresetRemove_Click(object sender, EventArgs e)
		{
			if (Program.config.listChatPreset.Count <= 1)
			{
				MessageBox.Show("적어도 1개의 프리셋은 남아있어야 합니다.", "오류");
				return;
			}
			int selectedIndex = this.listBoxPreset.SelectedIndex;
			WidgetChatPreset item = Program.config.listChatPreset[selectedIndex];
			Program.config.listChatPreset.RemoveAt(selectedIndex);
			this.RefreshPresetList();
			this.SelectPreset(0);
		}

		private void checkBoxPlatformIcon_CheckedChanged(object sender, EventArgs e)
		{
			this.currentPreset.platformIcon = this.checkBoxPlatformIcon.Checked;
			this.ApplyToChat();
		}

		public string ColorToString(Color c)
		{
			return string.Format("{0}, {1}, {2}", c.R, c.G, c.B);
		}

		private void comboBoxAnimation_SelectedIndexChanged(object sender, EventArgs e)
		{
			string str = this.comboBoxAnimation.SelectedItem.ToString();
			str = this.GetStringAnimation(str);
			this.currentPreset.animation = str;
			this.ApplyToChat();
		}

		private void comboBoxFont_SelectedIndexChanged(object sender, EventArgs e)
		{
			string str = this.comboBoxFont.SelectedItem.ToString();
			str = this.GetStringFont(str);
			this.currentPreset.font = str;
			this.ApplyToChat();
		}

		private void comboBoxPlatform_SelectedIndexChanged(object sender, EventArgs e)
		{
			string str = this.comboBoxPlatform.SelectedItem.ToString();
			str = this.GetStringPlatform(str);
			this.currentPreset.platform = str;
			this.ApplyToChat();
		}

		private void comboBoxTheme_SelectedIndexChanged(object sender, EventArgs e)
		{
			string str = this.comboBoxTheme.SelectedItem.ToString();
			str = this.GetStringTheme(str);
			this.currentPreset.theme = str;
			this.ApplyToChat();
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

		private void FillComboBox()
		{
			int i;
			string[] strArrays = this.strTheme;
			for (i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i];
				this.comboBoxTheme.Items.Add(str);
			}
			strArrays = this.strPlatform;
			for (i = 0; i < (int)strArrays.Length; i++)
			{
				string str1 = strArrays[i];
				this.comboBoxPlatform.Items.Add(str1);
			}
			strArrays = this.strAnimation;
			for (i = 0; i < (int)strArrays.Length; i++)
			{
				string str2 = strArrays[i];
				this.comboBoxAnimation.Items.Add(str2);
			}
			strArrays = this.strFont;
			for (i = 0; i < (int)strArrays.Length; i++)
			{
				string str3 = strArrays[i];
				this.comboBoxFont.Items.Add(str3);
			}
			this.comboBoxTheme.SelectedIndex = 0;
			this.comboBoxPlatform.SelectedIndex = 0;
			this.comboBoxAnimation.SelectedIndex = 0;
			this.comboBoxFont.SelectedIndex = 0;
		}

		private string GetOppositeString(string[] str1, string[] str2, string value)
		{
			for (int i = 0; i < (int)str1.Length; i++)
			{
				if (str1[i] == value)
				{
					return str2[i];
				}
				if (str2[i] == value)
				{
					return str1[i];
				}
			}
			return null;
		}

		public string GetStringAnimation(string str)
		{
			return this.GetOppositeString(this.strAnimation, this.strAnimationValue, str);
		}

		public string GetStringFont(string str)
		{
			return this.GetOppositeString(this.strFont, this.strFontValue, str);
		}

		public string GetStringPlatform(string str)
		{
			return this.GetOppositeString(this.strPlatform, this.strPlatformValue, str);
		}

		public string GetStringTheme(string str)
		{
			return this.GetOppositeString(this.strTheme, this.strThemeValue, str);
		}

		private void InitializeComponent()
		{
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(WidgetChat));
			this.panelDetail = new Panel();
			this.label13 = new Label();
			this.label10 = new Label();
			this.numChatFade = new NumericUpDown();
			this.label8 = new Label();
			this.numChatBackgroundAlpha = new NumericUpDown();
			this.numBackgroundAlpha = new NumericUpDown();
			this.numChatFont = new NumericUpDown();
			this.numUsernameFont = new NumericUpDown();
			this.label16 = new Label();
			this.labelChatBackgroundColor = new Label();
			this.label18 = new Label();
			this.label15 = new Label();
			this.labelBackgroundColor = new Label();
			this.label14 = new Label();
			this.labelChatFontColor = new Label();
			this.label11 = new Label();
			this.label12 = new Label();
			this.labelUsernameFontColor = new Label();
			this.label7 = new Label();
			this.label6 = new Label();
			this.comboBoxFont = new ComboBox();
			this.label5 = new Label();
			this.comboBoxAnimation = new ComboBox();
			this.label4 = new Label();
			this.comboBoxPlatform = new ComboBox();
			this.label3 = new Label();
			this.checkBoxPlatformIcon = new CheckBox();
			this.label2 = new Label();
			this.comboBoxTheme = new ComboBox();
			this.label1 = new Label();
			this.label_exit = new Label();
			this.listBoxPreset = new ListBox();
			this.buttonPresetCreate = new Button();
			this.buttonPresetRemove = new Button();
			this.label9 = new Label();
			this.colorDialog = new ColorDialog();
			this.label17 = new Label();
			this.panel1 = new Panel();
			this.buttonClose = new Button();
			this.label21 = new Label();
			this.label20 = new Label();
			this.label19 = new Label();
			this.textBoxChatURL = new TextBox();
			this.panelDetail.SuspendLayout();
			((ISupportInitialize)this.numChatFade).BeginInit();
			((ISupportInitialize)this.numChatBackgroundAlpha).BeginInit();
			((ISupportInitialize)this.numBackgroundAlpha).BeginInit();
			((ISupportInitialize)this.numChatFont).BeginInit();
			((ISupportInitialize)this.numUsernameFont).BeginInit();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.panelDetail.AutoScroll = true;
			this.panelDetail.BackColor = Color.FromArgb(71, 83, 86);
			this.panelDetail.Controls.Add(this.label13);
			this.panelDetail.Controls.Add(this.label10);
			this.panelDetail.Controls.Add(this.numChatFade);
			this.panelDetail.Controls.Add(this.label8);
			this.panelDetail.Controls.Add(this.numChatBackgroundAlpha);
			this.panelDetail.Controls.Add(this.numBackgroundAlpha);
			this.panelDetail.Controls.Add(this.numChatFont);
			this.panelDetail.Controls.Add(this.numUsernameFont);
			this.panelDetail.Controls.Add(this.label16);
			this.panelDetail.Controls.Add(this.labelChatBackgroundColor);
			this.panelDetail.Controls.Add(this.label18);
			this.panelDetail.Controls.Add(this.label15);
			this.panelDetail.Controls.Add(this.labelBackgroundColor);
			this.panelDetail.Controls.Add(this.label14);
			this.panelDetail.Controls.Add(this.labelChatFontColor);
			this.panelDetail.Controls.Add(this.label11);
			this.panelDetail.Controls.Add(this.label12);
			this.panelDetail.Controls.Add(this.labelUsernameFontColor);
			this.panelDetail.Controls.Add(this.label7);
			this.panelDetail.Controls.Add(this.label6);
			this.panelDetail.Controls.Add(this.comboBoxFont);
			this.panelDetail.Controls.Add(this.label5);
			this.panelDetail.Controls.Add(this.comboBoxAnimation);
			this.panelDetail.Controls.Add(this.label4);
			this.panelDetail.Controls.Add(this.comboBoxPlatform);
			this.panelDetail.Controls.Add(this.label3);
			this.panelDetail.Controls.Add(this.checkBoxPlatformIcon);
			this.panelDetail.Controls.Add(this.label2);
			this.panelDetail.Controls.Add(this.comboBoxTheme);
			this.panelDetail.Controls.Add(this.label1);
			this.panelDetail.Location = new Point(134, 121);
			this.panelDetail.Name = "panelDetail";
			this.panelDetail.Size = new System.Drawing.Size(296, 377);
			this.panelDetail.TabIndex = 0;
			this.label13.Anchor = AnchorStyles.Top;
			this.label13.Font = new System.Drawing.Font("Malgun Gothic", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label13.ForeColor = Color.FromArgb(0, 180, 204);
			this.label13.Location = new Point(149, 323);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(111, 30);
			this.label13.TabIndex = 105;
			this.label13.Text = "(0은 사라지지 않음)";
			this.label13.TextAlign = ContentAlignment.MiddleCenter;
			this.label10.Anchor = AnchorStyles.Top;
			this.label10.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label10.ForeColor = Color.FromArgb(0, 180, 204);
			this.label10.Location = new Point(124, 324);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(27, 30);
			this.label10.TabIndex = 104;
			this.label10.Text = "초";
			this.label10.TextAlign = ContentAlignment.MiddleCenter;
			this.numChatFade.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Regular, GraphicsUnit.Point, 129);
			this.numChatFade.Location = new Point(65, 324);
			this.numChatFade.Maximum = new decimal(new int[] { 9000, 0, 0, 0 });
			this.numChatFade.Name = "numChatFade";
			this.numChatFade.Size = new System.Drawing.Size(56, 29);
			this.numChatFade.TabIndex = 103;
			this.numChatFade.TextAlign = HorizontalAlignment.Center;
			this.numChatFade.Value = new decimal(new int[] { 10, 0, 0, 0 });
			this.numChatFade.ValueChanged += new EventHandler(this.numChatFade_ValueChanged);
			this.label8.Anchor = AnchorStyles.Top;
			this.label8.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label8.ForeColor = Color.FromArgb(0, 180, 204);
			this.label8.Location = new Point(0, 291);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(296, 30);
			this.label8.TabIndex = 102;
			this.label8.Text = "채팅 사라짐 지연시간";
			this.label8.TextAlign = ContentAlignment.MiddleCenter;
			this.numChatBackgroundAlpha.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Regular, GraphicsUnit.Point, 129);
			this.numChatBackgroundAlpha.Location = new Point(110, 1015);
			this.numChatBackgroundAlpha.Name = "numChatBackgroundAlpha";
			this.numChatBackgroundAlpha.Size = new System.Drawing.Size(56, 29);
			this.numChatBackgroundAlpha.TabIndex = 101;
			this.numChatBackgroundAlpha.TextAlign = HorizontalAlignment.Center;
			this.numChatBackgroundAlpha.ValueChanged += new EventHandler(this.numChatBackgroundAlpha_ValueChanged);
			this.numBackgroundAlpha.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Regular, GraphicsUnit.Point, 129);
			this.numBackgroundAlpha.Location = new Point(110, 862);
			this.numBackgroundAlpha.Name = "numBackgroundAlpha";
			this.numBackgroundAlpha.Size = new System.Drawing.Size(56, 29);
			this.numBackgroundAlpha.TabIndex = 100;
			this.numBackgroundAlpha.TextAlign = HorizontalAlignment.Center;
			this.numBackgroundAlpha.ValueChanged += new EventHandler(this.numBackgroundAlpha_ValueChanged);
			this.numChatFont.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Regular, GraphicsUnit.Point, 129);
			this.numChatFont.Location = new Point(110, 620);
			this.numChatFont.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
			this.numChatFont.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
			this.numChatFont.Name = "numChatFont";
			this.numChatFont.Size = new System.Drawing.Size(56, 29);
			this.numChatFont.TabIndex = 99;
			this.numChatFont.TextAlign = HorizontalAlignment.Center;
			this.numChatFont.Value = new decimal(new int[] { 14, 0, 0, 0 });
			this.numChatFont.ValueChanged += new EventHandler(this.numChatFont_ValueChanged);
			this.numUsernameFont.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Regular, GraphicsUnit.Point, 129);
			this.numUsernameFont.Location = new Point(110, 466);
			this.numUsernameFont.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
			this.numUsernameFont.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
			this.numUsernameFont.Name = "numUsernameFont";
			this.numUsernameFont.Size = new System.Drawing.Size(56, 29);
			this.numUsernameFont.TabIndex = 98;
			this.numUsernameFont.TextAlign = HorizontalAlignment.Center;
			this.numUsernameFont.Value = new decimal(new int[] { 14, 0, 0, 0 });
			this.numUsernameFont.ValueChanged += new EventHandler(this.numUsernameFont_ValueChanged);
			this.label16.Anchor = AnchorStyles.Top;
			this.label16.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label16.ForeColor = Color.FromArgb(0, 180, 204);
			this.label16.Location = new Point(0, 983);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(296, 30);
			this.label16.TabIndex = 96;
			this.label16.Text = "채팅 배경 투명도";
			this.label16.TextAlign = ContentAlignment.MiddleCenter;
			this.labelChatBackgroundColor.BackColor = Color.White;
			this.labelChatBackgroundColor.Location = new Point(118, 950);
			this.labelChatBackgroundColor.Name = "labelChatBackgroundColor";
			this.labelChatBackgroundColor.Size = new System.Drawing.Size(40, 23);
			this.labelChatBackgroundColor.TabIndex = 94;
			this.labelChatBackgroundColor.Click += new EventHandler(this.labelChatBgColor_Click);
			this.label18.Anchor = AnchorStyles.Top;
			this.label18.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label18.ForeColor = Color.FromArgb(0, 180, 204);
			this.label18.Location = new Point(0, 910);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(296, 30);
			this.label18.TabIndex = 93;
			this.label18.Text = "채팅 배경 색상";
			this.label18.TextAlign = ContentAlignment.MiddleCenter;
			this.label15.Anchor = AnchorStyles.Top;
			this.label15.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label15.ForeColor = Color.FromArgb(0, 180, 204);
			this.label15.Location = new Point(0, 829);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(296, 30);
			this.label15.TabIndex = 91;
			this.label15.Text = "배경 투명도";
			this.label15.TextAlign = ContentAlignment.MiddleCenter;
			this.labelBackgroundColor.BackColor = Color.White;
			this.labelBackgroundColor.Location = new Point(118, 796);
			this.labelBackgroundColor.Name = "labelBackgroundColor";
			this.labelBackgroundColor.Size = new System.Drawing.Size(40, 23);
			this.labelBackgroundColor.TabIndex = 89;
			this.labelBackgroundColor.Click += new EventHandler(this.labelBgColor_Click);
			this.label14.Anchor = AnchorStyles.Top;
			this.label14.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label14.ForeColor = Color.FromArgb(0, 180, 204);
			this.label14.Location = new Point(0, 756);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(296, 30);
			this.label14.TabIndex = 88;
			this.label14.Text = "배경 색상";
			this.label14.TextAlign = ContentAlignment.MiddleCenter;
			this.labelChatFontColor.BackColor = Color.White;
			this.labelChatFontColor.Location = new Point(118, 711);
			this.labelChatFontColor.Name = "labelChatFontColor";
			this.labelChatFontColor.Size = new System.Drawing.Size(40, 23);
			this.labelChatFontColor.TabIndex = 86;
			this.labelChatFontColor.Click += new EventHandler(this.labelChatFontColor_Click);
			this.label11.Anchor = AnchorStyles.Top;
			this.label11.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label11.ForeColor = Color.FromArgb(0, 180, 204);
			this.label11.Location = new Point(0, 671);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(296, 30);
			this.label11.TabIndex = 85;
			this.label11.Text = "채팅 폰트 색상";
			this.label11.TextAlign = ContentAlignment.MiddleCenter;
			this.label12.Anchor = AnchorStyles.Top;
			this.label12.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label12.ForeColor = Color.FromArgb(0, 180, 204);
			this.label12.Location = new Point(0, 587);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(296, 30);
			this.label12.TabIndex = 84;
			this.label12.Text = "채팅 폰트 크기";
			this.label12.TextAlign = ContentAlignment.MiddleCenter;
			this.labelUsernameFontColor.BackColor = Color.White;
			this.labelUsernameFontColor.Location = new Point(118, 549);
			this.labelUsernameFontColor.Name = "labelUsernameFontColor";
			this.labelUsernameFontColor.Size = new System.Drawing.Size(40, 23);
			this.labelUsernameFontColor.TabIndex = 82;
			this.labelUsernameFontColor.Click += new EventHandler(this.labelFontColor_Click);
			this.label7.Anchor = AnchorStyles.Top;
			this.label7.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label7.ForeColor = Color.FromArgb(0, 180, 204);
			this.label7.Location = new Point(0, 509);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(296, 30);
			this.label7.TabIndex = 81;
			this.label7.Text = "닉네임 폰트 색상";
			this.label7.TextAlign = ContentAlignment.MiddleCenter;
			this.label6.Anchor = AnchorStyles.Top;
			this.label6.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label6.ForeColor = Color.FromArgb(0, 180, 204);
			this.label6.Location = new Point(0, 433);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(296, 30);
			this.label6.TabIndex = 79;
			this.label6.Text = "닉네임 폰트 크기";
			this.label6.TextAlign = ContentAlignment.MiddleCenter;
			this.comboBoxFont.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxFont.FlatStyle = FlatStyle.Flat;
			this.comboBoxFont.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Regular, GraphicsUnit.Point, 129);
			this.comboBoxFont.FormattingEnabled = true;
			this.comboBoxFont.Location = new Point(78, 390);
			this.comboBoxFont.Name = "comboBoxFont";
			this.comboBoxFont.Size = new System.Drawing.Size(121, 29);
			this.comboBoxFont.TabIndex = 78;
			this.comboBoxFont.SelectedIndexChanged += new EventHandler(this.comboBoxFont_SelectedIndexChanged);
			this.label5.Anchor = AnchorStyles.Top;
			this.label5.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label5.ForeColor = Color.FromArgb(0, 180, 204);
			this.label5.Location = new Point(0, 357);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(296, 30);
			this.label5.TabIndex = 77;
			this.label5.Text = "폰트";
			this.label5.TextAlign = ContentAlignment.MiddleCenter;
			this.comboBoxAnimation.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxAnimation.FlatStyle = FlatStyle.Flat;
			this.comboBoxAnimation.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Regular, GraphicsUnit.Point, 129);
			this.comboBoxAnimation.FormattingEnabled = true;
			this.comboBoxAnimation.Location = new Point(78, 252);
			this.comboBoxAnimation.Name = "comboBoxAnimation";
			this.comboBoxAnimation.Size = new System.Drawing.Size(121, 29);
			this.comboBoxAnimation.TabIndex = 76;
			this.comboBoxAnimation.SelectedIndexChanged += new EventHandler(this.comboBoxAnimation_SelectedIndexChanged);
			this.label4.Anchor = AnchorStyles.Top;
			this.label4.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label4.ForeColor = Color.FromArgb(0, 180, 204);
			this.label4.Location = new Point(0, 219);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(296, 30);
			this.label4.TabIndex = 75;
			this.label4.Text = "애니메이션";
			this.label4.TextAlign = ContentAlignment.MiddleCenter;
			this.comboBoxPlatform.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxPlatform.FlatStyle = FlatStyle.Flat;
			this.comboBoxPlatform.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Regular, GraphicsUnit.Point, 129);
			this.comboBoxPlatform.FormattingEnabled = true;
			this.comboBoxPlatform.Location = new Point(78, 179);
			this.comboBoxPlatform.Name = "comboBoxPlatform";
			this.comboBoxPlatform.Size = new System.Drawing.Size(121, 29);
			this.comboBoxPlatform.TabIndex = 74;
			this.comboBoxPlatform.SelectedIndexChanged += new EventHandler(this.comboBoxPlatform_SelectedIndexChanged);
			this.label3.Anchor = AnchorStyles.Top;
			this.label3.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label3.ForeColor = Color.FromArgb(0, 180, 204);
			this.label3.Location = new Point(0, 146);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(296, 30);
			this.label3.TabIndex = 73;
			this.label3.Text = "플랫폼 선택";
			this.label3.TextAlign = ContentAlignment.MiddleCenter;
			this.checkBoxPlatformIcon.AutoSize = true;
			this.checkBoxPlatformIcon.FlatStyle = FlatStyle.Flat;
			this.checkBoxPlatformIcon.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.checkBoxPlatformIcon.ForeColor = Color.White;
			this.checkBoxPlatformIcon.Location = new Point(109, 111);
			this.checkBoxPlatformIcon.Name = "checkBoxPlatformIcon";
			this.checkBoxPlatformIcon.Size = new System.Drawing.Size(58, 25);
			this.checkBoxPlatformIcon.TabIndex = 72;
			this.checkBoxPlatformIcon.Text = "표시";
			this.checkBoxPlatformIcon.UseVisualStyleBackColor = true;
			this.checkBoxPlatformIcon.CheckedChanged += new EventHandler(this.checkBoxPlatformIcon_CheckedChanged);
			this.label2.Anchor = AnchorStyles.Top;
			this.label2.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label2.ForeColor = Color.FromArgb(0, 180, 204);
			this.label2.Location = new Point(0, 77);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(296, 30);
			this.label2.TabIndex = 71;
			this.label2.Text = "플랫폼 아이콘 표시";
			this.label2.TextAlign = ContentAlignment.MiddleCenter;
			this.comboBoxTheme.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboBoxTheme.FlatStyle = FlatStyle.Flat;
			this.comboBoxTheme.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Regular, GraphicsUnit.Point, 129);
			this.comboBoxTheme.FormattingEnabled = true;
			this.comboBoxTheme.Location = new Point(78, 34);
			this.comboBoxTheme.Name = "comboBoxTheme";
			this.comboBoxTheme.Size = new System.Drawing.Size(121, 29);
			this.comboBoxTheme.TabIndex = 70;
			this.comboBoxTheme.SelectedIndexChanged += new EventHandler(this.comboBoxTheme_SelectedIndexChanged);
			this.label1.Anchor = AnchorStyles.Top;
			this.label1.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label1.ForeColor = Color.FromArgb(0, 180, 204);
			this.label1.Location = new Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(296, 30);
			this.label1.TabIndex = 69;
			this.label1.Text = "테마";
			this.label1.TextAlign = ContentAlignment.MiddleCenter;
			this.label_exit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			this.label_exit.BackColor = Color.FromArgb(0, 180, 204);
			this.label_exit.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label_exit.Location = new Point(413, 5);
			this.label_exit.Name = "label_exit";
			this.label_exit.Size = new System.Drawing.Size(23, 26);
			this.label_exit.TabIndex = 44;
			this.label_exit.Text = "X";
			this.label_exit.Click += new EventHandler(this.label_exit_Click);
			this.listBoxPreset.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Regular, GraphicsUnit.Point, 129);
			this.listBoxPreset.FormattingEnabled = true;
			this.listBoxPreset.ItemHeight = 21;
			this.listBoxPreset.Items.AddRange(new object[] { "프리셋이름" });
			this.listBoxPreset.Location = new Point(11, 144);
			this.listBoxPreset.Name = "listBoxPreset";
			this.listBoxPreset.Size = new System.Drawing.Size(111, 88);
			this.listBoxPreset.TabIndex = 65;
			this.listBoxPreset.SelectedIndexChanged += new EventHandler(this.listBoxPreset_SelectedIndexChanged);
			this.buttonPresetCreate.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonPresetCreate.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonPresetCreate.FlatAppearance.BorderSize = 2;
			this.buttonPresetCreate.FlatStyle = FlatStyle.Flat;
			this.buttonPresetCreate.Location = new Point(11, 238);
			this.buttonPresetCreate.Name = "buttonPresetCreate";
			this.buttonPresetCreate.Size = new System.Drawing.Size(53, 23);
			this.buttonPresetCreate.TabIndex = 66;
			this.buttonPresetCreate.Text = "추가";
			this.buttonPresetCreate.UseVisualStyleBackColor = false;
			this.buttonPresetCreate.Click += new EventHandler(this.buttonPresetCreate_Click);
			this.buttonPresetRemove.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonPresetRemove.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonPresetRemove.FlatAppearance.BorderSize = 2;
			this.buttonPresetRemove.FlatStyle = FlatStyle.Flat;
			this.buttonPresetRemove.Location = new Point(69, 238);
			this.buttonPresetRemove.Name = "buttonPresetRemove";
			this.buttonPresetRemove.Size = new System.Drawing.Size(53, 23);
			this.buttonPresetRemove.TabIndex = 67;
			this.buttonPresetRemove.Text = "제거";
			this.buttonPresetRemove.UseVisualStyleBackColor = false;
			this.buttonPresetRemove.Click += new EventHandler(this.buttonPresetRemove_Click);
			this.label9.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label9.ForeColor = Color.FromArgb(0, 180, 204);
			this.label9.Location = new Point(11, 120);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(111, 21);
			this.label9.TabIndex = 68;
			this.label9.Text = "프리셋";
			this.label9.TextAlign = ContentAlignment.MiddleCenter;
			this.label17.Font = new System.Drawing.Font("Malgun Gothic", 15.75f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label17.ForeColor = Color.FromArgb(0, 180, 204);
			this.label17.Location = new Point(26, 5);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(380, 30);
			this.label17.TabIndex = 69;
			this.label17.Text = "채팅 위젯 설정";
			this.label17.TextAlign = ContentAlignment.MiddleCenter;
			this.label17.MouseDown += new MouseEventHandler(this.label17_MouseDown);
			this.panel1.BorderStyle = BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.buttonClose);
			this.panel1.Controls.Add(this.label21);
			this.panel1.Controls.Add(this.label20);
			this.panel1.Controls.Add(this.label19);
			this.panel1.Controls.Add(this.textBoxChatURL);
			this.panel1.Controls.Add(this.label17);
			this.panel1.Controls.Add(this.panelDetail);
			this.panel1.Controls.Add(this.label9);
			this.panel1.Controls.Add(this.label_exit);
			this.panel1.Controls.Add(this.buttonPresetRemove);
			this.panel1.Controls.Add(this.buttonPresetCreate);
			this.panel1.Controls.Add(this.listBoxPreset);
			this.panel1.Dock = DockStyle.Fill;
			this.panel1.Location = new Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(444, 541);
			this.panel1.TabIndex = 0;
			this.panel1.MouseDown += new MouseEventHandler(this.panel1_MouseDown);
			this.buttonClose.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonClose.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonClose.FlatAppearance.BorderSize = 2;
			this.buttonClose.FlatStyle = FlatStyle.Flat;
			this.buttonClose.Location = new Point(160, 508);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(127, 23);
			this.buttonClose.TabIndex = 74;
			this.buttonClose.Text = "닫기";
			this.buttonClose.UseVisualStyleBackColor = false;
			this.buttonClose.Click += new EventHandler(this.buttonClose_Click);
			this.label21.Font = new System.Drawing.Font("Malgun Gothic", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label21.ForeColor = Color.White;
			this.label21.Location = new Point(121, 83);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(189, 24);
			this.label21.TabIndex = 73;
			this.label21.Text = "프리셋 마다 주소가 다릅니다.";
			this.label21.TextAlign = ContentAlignment.MiddleCenter;
			this.label20.Font = new System.Drawing.Font("Malgun Gothic", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label20.ForeColor = Color.White;
			this.label20.Location = new Point(125, 39);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(177, 24);
			this.label20.TabIndex = 72;
			this.label20.Text = "(클릭 시 주소가 복사됩니다)";
			this.label20.TextAlign = ContentAlignment.MiddleCenter;
			this.label19.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label19.ForeColor = Color.FromArgb(0, 180, 204);
			this.label19.Location = new Point(4, 37);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(121, 24);
			this.label19.TabIndex = 71;
			this.label19.Text = "채팅 위젯 주소";
			this.label19.TextAlign = ContentAlignment.MiddleCenter;
			this.textBoxChatURL.BackColor = Color.LemonChiffon;
			this.textBoxChatURL.BorderStyle = BorderStyle.FixedSingle;
			this.textBoxChatURL.Font = new System.Drawing.Font("Gulim", 9f, FontStyle.Regular, GraphicsUnit.Point, 129);
			this.textBoxChatURL.ForeColor = SystemColors.WindowFrame;
			this.textBoxChatURL.Location = new Point(8, 62);
			this.textBoxChatURL.Name = "textBoxChatURL";
			this.textBoxChatURL.ReadOnly = true;
			this.textBoxChatURL.Size = new System.Drawing.Size(422, 21);
			this.textBoxChatURL.TabIndex = 70;
			this.textBoxChatURL.Text = "http://js-almighty.com/jsassist/web/widget_chat.html";
			this.textBoxChatURL.Click += new EventHandler(this.textBoxChatURL_Click);
			base.AutoScaleDimensions = new SizeF(7f, 12f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = Color.FromArgb(44, 52, 54);
			base.ClientSize = new System.Drawing.Size(444, 541);
			base.Controls.Add(this.panel1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "WidgetChat";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "채팅 위젯 설정";
			base.FormClosing += new FormClosingEventHandler(this.WidgetChat_FormClosing);
			this.panelDetail.ResumeLayout(false);
			this.panelDetail.PerformLayout();
			((ISupportInitialize)this.numChatFade).EndInit();
			((ISupportInitialize)this.numChatBackgroundAlpha).EndInit();
			((ISupportInitialize)this.numBackgroundAlpha).EndInit();
			((ISupportInitialize)this.numChatFont).EndInit();
			((ISupportInitialize)this.numUsernameFont).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			base.ResumeLayout(false);
		}

		private void InitPresetList()
		{
			if (Program.config.listChatPreset.Count == 0)
			{
				Console.WriteLine("Error : No WidgetChatPreset Found");
				WidgetChatPreset widgetChatPreset = new WidgetChatPreset();
				Program.config.listChatPreset.Add(widgetChatPreset);
			}
			this.RefreshPresetList();
			this.SelectPreset(0);
		}

		private void label_exit_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void label17_MouseDown(object sender, MouseEventArgs e)
		{
			this.DragForm(e);
		}

		private void labelBgColor_Click(object sender, EventArgs e)
		{
			this.ShowColorDialog(this.labelBackgroundColor);
			this.currentPreset.backgroundColor = this.ColorToString(this.labelBackgroundColor.BackColor);
			this.ApplyToChat();
		}

		private void labelChatBgColor_Click(object sender, EventArgs e)
		{
			this.ShowColorDialog(this.labelChatBackgroundColor);
			this.currentPreset.chatBackgroundColor = this.ColorToString(this.labelChatBackgroundColor.BackColor);
			this.ApplyToChat();
		}

		private void labelChatFontColor_Click(object sender, EventArgs e)
		{
			this.ShowColorDialog(this.labelChatFontColor);
			this.currentPreset.fontChatColor = this.ColorToString(this.labelChatFontColor.BackColor);
			this.ApplyToChat();
		}

		private void labelFontColor_Click(object sender, EventArgs e)
		{
			this.ShowColorDialog(this.labelUsernameFontColor);
			this.currentPreset.fontUsernameColor = this.ColorToString(this.labelUsernameFontColor.BackColor);
			this.ApplyToChat();
		}

		private void listBoxPreset_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.SelectPreset(this.listBoxPreset.SelectedIndex);
			this.ApplyPresetValueToControl();
		}

		private void numBackgroundAlpha_ValueChanged(object sender, EventArgs e)
		{
			int value = (int)this.numBackgroundAlpha.Value;
			this.currentPreset.backgroundAlpha = value;
			this.ApplyToChat();
		}

		private void numChatBackgroundAlpha_ValueChanged(object sender, EventArgs e)
		{
			int value = (int)this.numChatBackgroundAlpha.Value;
			this.currentPreset.chatBackgroundAlpha = value;
			this.ApplyToChat();
		}

		private void numChatFade_ValueChanged(object sender, EventArgs e)
		{
			int value = (int)this.numChatFade.Value;
			this.currentPreset.chatFade = (float)value;
			this.ApplyToChat();
		}

		private void numChatFont_ValueChanged(object sender, EventArgs e)
		{
			int value = (int)this.numChatFont.Value;
			this.currentPreset.fontChatSize = (float)value;
			this.ApplyToChat();
		}

		private void numUsernameFont_ValueChanged(object sender, EventArgs e)
		{
			int value = (int)this.numUsernameFont.Value;
			this.currentPreset.fontUsernameSize = (float)value;
			this.ApplyToChat();
		}

		private void panel1_MouseDown(object sender, MouseEventArgs e)
		{
			this.DragForm(e);
		}

		private void RefreshPresetList()
		{
			this.listBoxPreset.Items.Clear();
			foreach (WidgetChatPreset widgetChatPreset in Program.config.listChatPreset)
			{
				this.listBoxPreset.Items.Add(widgetChatPreset.presetName);
			}
		}

		private void SelectPreset(int idx)
		{
			this.listBoxPreset.SelectedIndex = idx;
			this.currentPreset = Program.config.listChatPreset[idx];
			this.currentPresetIdx = idx;
			this.textBoxChatURL.Text = string.Concat("http://js-almighty.com/jsassist/web/widget_chat.html?preset=", this.currentPreset.presetName);
			this.ApplyPresetValueToControl();
		}

		private void ShowColorDialog(Label label)
		{
			int value = this.panelDetail.VerticalScroll.Value;
			this.colorDialog.Color = label.BackColor;
			if (this.colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				label.BackColor = this.colorDialog.Color;
			}
			this.panelDetail.VerticalScroll.Value = value;
			this.panelDetail.PerformLayout();
		}

		public Color StringToColor(string str)
		{
			string[] strArrays = str.Split(new string[] { ", " }, StringSplitOptions.None);
			return Color.FromArgb(int.Parse(strArrays[0]), int.Parse(strArrays[1]), int.Parse(strArrays[2]));
		}

		private void textBoxChatURL_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(this.textBoxChatURL.Text);
			MessageBox.Show("주소가 복사되었습니다.", "JSAssist");
		}

		private void WidgetChat_FormClosing(object sender, FormClosingEventArgs e)
		{
			Program.config.SaveFile();
		}
	}
}