using JSAssist.Properties;
using JSAssist.Widget;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;

namespace JSAssist
{
	public class FormCoreWindow : Form
	{
		public static FormCoreWindow inst;

		private IContainer components;

		private Timer timer_update;

		private ListView listView1;

		private ColumnHeader Username;

		private ColumnHeader Message;

		private Button buttonSurvey;

		private PictureBox pictureBox1;

		private CheckBox checkBoxAutoScrollChat;

		private WebBrowser webTwitch;

		private WebBrowser webYoutube;

		private Label label1;

		private Button buttonSettingTwitch;

		private Label labelStatTwitch;

		private Label label4;

		private Label label6;

		private ImageList imgListPlatform;

		private CheckBox checkBoxChatTwitch;

		private CheckBox checkBoxChatYoutube;

		private CheckBox checkBoxChatTVPot;

		private Label label8;

		private Label label9;

		private Label label_exit;

		private Button buttonSettingYoutube;

		private Button buttonSettingTVPot;

		private Label labelStatYoutube;

		private Label labelStatTVpot;

		private Label label12;

		private Label labelVersion;

		private Button buttonExit;

		private Button buttonProgramInfo;

		private Button buttonGoSite;

		private Button buttonPatchNote;

		private Label labelOffline;

		private CheckBox checkBoxTestChat;

		private Label label_minimize;

		private Button buttonWidgetAdd;

		private Button buttonWidgetRemove;

		private Button buttonWidgetSetting;

		private ListView listViewWidgets;

		private ColumnHeader widget;

		private ColumnHeader enable;

		private Button buttonWidgetEnable;

		private Button buttonTutorial;

		static FormCoreWindow()
		{
		}

		public FormCoreWindow()
		{
			this.InitializeComponent();
			FormCoreWindow.inst = this;
			Program.chatManager.chatTwitch.BindBrowser(this.webTwitch);
			Program.chatManager.chatYoutube.BindBrowser(this.webYoutube);
			this.InitWidgetList();
			this.SetValueFromConfig();
			this.labelVersion.Text = string.Concat("build.beta.", Program.currentVersion);
			if (Program.isOfflineMode)
			{
				this.labelOffline.Visible = true;
				return;
			}
			this.labelOffline.Visible = false;
		}

		public static void AddChat(string platform, string username, string message)
		{
			ListView listView = FormCoreWindow.inst.listView1;
			ListViewItem listViewItem = new ListViewItem(new string[] { username, message });
			if (platform == "twitch")
			{
				listViewItem.ImageIndex = 1;
			}
			else if (platform == "youtube")
			{
				listViewItem.ImageIndex = 2;
			}
			else if (platform == "tvpot")
			{
				listViewItem.ImageIndex = 0;
			}
			listView.Items.Add(listViewItem);
			if (FormCoreWindow.inst.checkBoxAutoScrollChat.Checked)
			{
				listView.Items[listView.Items.Count - 1].EnsureVisible();
			}
		}

		private void button_noticePopup_Click(object sender, EventArgs e)
		{
			(new NoticePopup()).Show();
		}

		private void button4_Click(object sender, EventArgs e)
		{
		}

		private void buttonExit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void buttonGoSite_Click(object sender, EventArgs e)
		{
			Process.Start("http://www.js-almighty.com");
		}

		private void buttonPatchNote_Click(object sender, EventArgs e)
		{
			(new PatchNote()).Show();
		}

		private void buttonProgramInfo_Click(object sender, EventArgs e)
		{
			(new FormProgramInfo()).Show();
		}

		private void buttonSettingChat_Click(object sender, EventArgs e)
		{
			MessageBox.Show("아직 세부 설정 옵션이 존재하지 않습니다.", "업데이트중");
		}

		private void buttonSettingTVPot_Click(object sender, EventArgs e)
		{
			int num = Program.chatManager.chatTVPot.selectedPotPlayerIdx;
			if (num != -1 && MessageBox.Show(string.Concat("이미 [", Program.chatManager.chatTVPot.windowPotPlayers[num].caption, "] 에 연결되어있습니다. 변경하시겠습니까?"), "다른 팟플레이어에 연결하기", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
			{
				return;
			}
			(new FormSelectPotPlayer()).Show();
		}

		private void buttonSettingTwitch_Click(object sender, EventArgs e)
		{
			(new ChatSettingTwitch()).ShowDialog();
		}

		private void buttonSettingYoutube_Click(object sender, EventArgs e)
		{
			(new ChatSettingYoutube()).ShowDialog();
		}

		private void buttonSurvey_Click(object sender, EventArgs e)
		{
			(new SurveyReport()).Show();
		}

		private void buttonTutorial_Click(object sender, EventArgs e)
		{
			Process.Start("http://js-almighty.com/jsassist/tutorial");
		}

		private void buttonWidgetAdd_Click(object sender, EventArgs e)
		{
			MessageBox.Show("새로운 위젯을 추가중입니다.", "개발중");
		}

		private void buttonWidgetEnable_Click(object sender, EventArgs e)
		{
			bool flag = !Program.config.widgetChatEnable;
			Program.config.widgetChatEnable = flag;
			Program.config.SaveFile();
			Program.chatManager.isEnable = flag;
			this.SetWidgetEnable("채팅 위젯", flag);
		}

		private void buttonWidgetRemove_Click(object sender, EventArgs e)
		{
			MessageBox.Show("채팅 위젯은 제거할 수 없습니다. 사용을 중단하시려면 비활성화를 눌러주세요", "알림");
		}

		private void buttonWidgetSetting_Click(object sender, EventArgs e)
		{
			(new WidgetChat()).Show();
		}

		private void checkBoxChat_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void checkBoxChatTVPot_CheckedChanged(object sender, EventArgs e)
		{
			this.SetChatTVPot(this.checkBoxChatTVPot.Checked, true);
		}

		private void checkBoxChatTwitch_CheckedChanged(object sender, EventArgs e)
		{
			this.SetChatTwitch(this.checkBoxChatTwitch.Checked, true);
		}

		private void checkBoxChatYoutube_CheckedChanged(object sender, EventArgs e)
		{
			this.SetChatYoutube(this.checkBoxChatYoutube.Checked, true);
		}

		private void checkBoxTestChat_CheckedChanged(object sender, EventArgs e)
		{
			Program.chatManager.isTestChatEnable = this.checkBoxTestChat.Checked;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void FormCoreWindow_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				Win32.ReleaseCapture();
				Win32.SendMessage(base.Handle, 161, 2, 0);
			}
		}

		private void FormCoreWindow_Shown(object sender, EventArgs e)
		{
			if (!Program.config.skipPatchNote)
			{
				(new PatchNote()).Show();
			}
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(FormCoreWindow));
			this.timer_update = new Timer(this.components);
			this.listView1 = new ListView();
			this.Username = new ColumnHeader();
			this.Message = new ColumnHeader();
			this.imgListPlatform = new ImageList(this.components);
			this.buttonSurvey = new Button();
			this.pictureBox1 = new PictureBox();
			this.checkBoxAutoScrollChat = new CheckBox();
			this.webTwitch = new WebBrowser();
			this.webYoutube = new WebBrowser();
			this.label1 = new Label();
			this.buttonSettingTwitch = new Button();
			this.labelStatTwitch = new Label();
			this.label4 = new Label();
			this.label6 = new Label();
			this.checkBoxChatTwitch = new CheckBox();
			this.checkBoxChatYoutube = new CheckBox();
			this.checkBoxChatTVPot = new CheckBox();
			this.label8 = new Label();
			this.label9 = new Label();
			this.label_exit = new Label();
			this.buttonSettingYoutube = new Button();
			this.buttonSettingTVPot = new Button();
			this.labelStatYoutube = new Label();
			this.labelStatTVpot = new Label();
			this.label12 = new Label();
			this.labelVersion = new Label();
			this.buttonExit = new Button();
			this.buttonProgramInfo = new Button();
			this.buttonGoSite = new Button();
			this.buttonPatchNote = new Button();
			this.labelOffline = new Label();
			this.checkBoxTestChat = new CheckBox();
			this.label_minimize = new Label();
			this.buttonWidgetAdd = new Button();
			this.buttonWidgetRemove = new Button();
			this.buttonWidgetSetting = new Button();
			this.listViewWidgets = new ListView();
			this.widget = new ColumnHeader();
			this.enable = new ColumnHeader();
			this.buttonWidgetEnable = new Button();
			this.buttonTutorial = new Button();
			((ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.timer_update.Enabled = true;
			this.timer_update.Interval = 250;
			this.timer_update.Tick += new EventHandler(this.timer_update_Tick);
			this.listView1.Columns.AddRange(new ColumnHeader[] { this.Username, this.Message });
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.Location = new Point(321, 136);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(458, 411);
			this.listView1.SmallImageList = this.imgListPlatform;
			this.listView1.TabIndex = 2;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = View.Details;
			this.listView1.SelectedIndexChanged += new EventHandler(this.listView1_SelectedIndexChanged);
			this.listView1.MouseDoubleClick += new MouseEventHandler(this.listView1_DoubleClick);
			this.Username.Text = "Username";
			this.Username.Width = 146;
			this.Message.Text = "Message";
			this.Message.Width = 288;
			this.imgListPlatform.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imgListPlatform.ImageStream");
			this.imgListPlatform.TransparentColor = Color.Transparent;
			this.imgListPlatform.Images.SetKeyName(0, "tvpot");
			this.imgListPlatform.Images.SetKeyName(1, "twitch");
			this.imgListPlatform.Images.SetKeyName(2, "youtube");
			this.buttonSurvey.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonSurvey.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonSurvey.FlatAppearance.BorderSize = 2;
			this.buttonSurvey.FlatStyle = FlatStyle.Flat;
			this.buttonSurvey.Location = new Point(434, 583);
			this.buttonSurvey.Name = "buttonSurvey";
			this.buttonSurvey.Size = new System.Drawing.Size(111, 23);
			this.buttonSurvey.TabIndex = 7;
			this.buttonSurvey.Text = "설문 / 의견";
			this.buttonSurvey.UseVisualStyleBackColor = false;
			this.buttonSurvey.Click += new EventHandler(this.buttonSurvey_Click);
			this.pictureBox1.Image = Resources.logo;
			this.pictureBox1.Location = new Point(279, 10);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(227, 71);
			this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 8;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.MouseDown += new MouseEventHandler(this.pictureBox1_MouseDown_1);
			this.checkBoxAutoScrollChat.AutoSize = true;
			this.checkBoxAutoScrollChat.Checked = true;
			this.checkBoxAutoScrollChat.CheckState = CheckState.Checked;
			this.checkBoxAutoScrollChat.Font = new System.Drawing.Font("Gulim", 9f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.checkBoxAutoScrollChat.ForeColor = Color.White;
			this.checkBoxAutoScrollChat.Location = new Point(696, 551);
			this.checkBoxAutoScrollChat.Name = "checkBoxAutoScrollChat";
			this.checkBoxAutoScrollChat.Size = new System.Drawing.Size(89, 16);
			this.checkBoxAutoScrollChat.TabIndex = 11;
			this.checkBoxAutoScrollChat.Text = "자동스크롤";
			this.checkBoxAutoScrollChat.UseVisualStyleBackColor = true;
			this.webTwitch.Location = new Point(18, 12);
			this.webTwitch.MinimumSize = new System.Drawing.Size(20, 20);
			this.webTwitch.Name = "webTwitch";
			this.webTwitch.ScriptErrorsSuppressed = true;
			this.webTwitch.Size = new System.Drawing.Size(63, 48);
			this.webTwitch.TabIndex = 12;
			this.webTwitch.Url = new Uri("", UriKind.Relative);
			this.webTwitch.Visible = false;
			this.webYoutube.Location = new Point(108, 12);
			this.webYoutube.MinimumSize = new System.Drawing.Size(20, 20);
			this.webYoutube.Name = "webYoutube";
			this.webYoutube.ScriptErrorsSuppressed = true;
			this.webYoutube.Size = new System.Drawing.Size(57, 48);
			this.webYoutube.TabIndex = 13;
			this.webYoutube.Url = new Uri("", UriKind.Relative);
			this.webYoutube.Visible = false;
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label1.ForeColor = Color.White;
			this.label1.Location = new Point(13, 135);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(98, 21);
			this.label1.TabIndex = 14;
			this.label1.Text = "Twitch 채팅";
			this.buttonSettingTwitch.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonSettingTwitch.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonSettingTwitch.FlatAppearance.BorderSize = 2;
			this.buttonSettingTwitch.FlatStyle = FlatStyle.Flat;
			this.buttonSettingTwitch.Location = new Point(270, 136);
			this.buttonSettingTwitch.Name = "buttonSettingTwitch";
			this.buttonSettingTwitch.Size = new System.Drawing.Size(43, 23);
			this.buttonSettingTwitch.TabIndex = 15;
			this.buttonSettingTwitch.Text = "설정";
			this.buttonSettingTwitch.UseVisualStyleBackColor = false;
			this.buttonSettingTwitch.Click += new EventHandler(this.buttonSettingTwitch_Click);
			this.labelStatTwitch.BackColor = Color.DimGray;
			this.labelStatTwitch.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.labelStatTwitch.ForeColor = Color.White;
			this.labelStatTwitch.Location = new Point(13, 162);
			this.labelStatTwitch.Name = "labelStatTwitch";
			this.labelStatTwitch.Size = new System.Drawing.Size(300, 25);
			this.labelStatTwitch.TabIndex = 16;
			this.labelStatTwitch.Text = "대기중";
			this.labelStatTwitch.TextAlign = ContentAlignment.TopCenter;
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label4.ForeColor = Color.White;
			this.label4.Location = new Point(13, 199);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(113, 21);
			this.label4.TabIndex = 17;
			this.label4.Text = "Youtube 채팅";
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label6.ForeColor = Color.White;
			this.label6.Location = new Point(13, 262);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(84, 21);
			this.label6.TabIndex = 20;
			this.label6.Text = "TV팟 채팅";
			this.checkBoxChatTwitch.AutoSize = true;
			this.checkBoxChatTwitch.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.checkBoxChatTwitch.ForeColor = Color.White;
			this.checkBoxChatTwitch.Location = new Point(203, 134);
			this.checkBoxChatTwitch.Name = "checkBoxChatTwitch";
			this.checkBoxChatTwitch.Size = new System.Drawing.Size(61, 25);
			this.checkBoxChatTwitch.TabIndex = 24;
			this.checkBoxChatTwitch.Text = "사용";
			this.checkBoxChatTwitch.UseVisualStyleBackColor = true;
			this.checkBoxChatTwitch.CheckedChanged += new EventHandler(this.checkBoxChatTwitch_CheckedChanged);
			this.checkBoxChatYoutube.AutoSize = true;
			this.checkBoxChatYoutube.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.checkBoxChatYoutube.ForeColor = Color.White;
			this.checkBoxChatYoutube.Location = new Point(203, 196);
			this.checkBoxChatYoutube.Name = "checkBoxChatYoutube";
			this.checkBoxChatYoutube.Size = new System.Drawing.Size(61, 25);
			this.checkBoxChatYoutube.TabIndex = 26;
			this.checkBoxChatYoutube.Text = "사용";
			this.checkBoxChatYoutube.UseVisualStyleBackColor = true;
			this.checkBoxChatYoutube.CheckedChanged += new EventHandler(this.checkBoxChatYoutube_CheckedChanged);
			this.checkBoxChatTVPot.AutoSize = true;
			this.checkBoxChatTVPot.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.checkBoxChatTVPot.ForeColor = Color.White;
			this.checkBoxChatTVPot.Location = new Point(203, 259);
			this.checkBoxChatTVPot.Name = "checkBoxChatTVPot";
			this.checkBoxChatTVPot.Size = new System.Drawing.Size(61, 25);
			this.checkBoxChatTVPot.TabIndex = 28;
			this.checkBoxChatTVPot.Text = "사용";
			this.checkBoxChatTVPot.UseVisualStyleBackColor = true;
			this.checkBoxChatTVPot.CheckedChanged += new EventHandler(this.checkBoxChatTVPot_CheckedChanged);
			this.label8.Font = new System.Drawing.Font("Malgun Gothic", 15.75f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label8.ForeColor = Color.FromArgb(0, 180, 204);
			this.label8.Location = new Point(7, 350);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(306, 30);
			this.label8.TabIndex = 30;
			this.label8.Text = "WIDGETS";
			this.label8.TextAlign = ContentAlignment.MiddleCenter;
			this.label9.Font = new System.Drawing.Font("Malgun Gothic", 15.75f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label9.ForeColor = Color.FromArgb(0, 180, 204);
			this.label9.Location = new Point(7, 95);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(306, 30);
			this.label9.TabIndex = 31;
			this.label9.Text = "CHAT INTEGRATION";
			this.label9.TextAlign = ContentAlignment.MiddleCenter;
			this.label_exit.BackColor = Color.FromArgb(0, 180, 204);
			this.label_exit.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label_exit.Location = new Point(764, 3);
			this.label_exit.Name = "label_exit";
			this.label_exit.Size = new System.Drawing.Size(23, 26);
			this.label_exit.TabIndex = 43;
			this.label_exit.Text = "X";
			this.label_exit.Click += new EventHandler(this.label_exit_Click);
			this.buttonSettingYoutube.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonSettingYoutube.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonSettingYoutube.FlatAppearance.BorderSize = 2;
			this.buttonSettingYoutube.FlatStyle = FlatStyle.Flat;
			this.buttonSettingYoutube.Location = new Point(270, 198);
			this.buttonSettingYoutube.Name = "buttonSettingYoutube";
			this.buttonSettingYoutube.Size = new System.Drawing.Size(43, 23);
			this.buttonSettingYoutube.TabIndex = 44;
			this.buttonSettingYoutube.Text = "설정";
			this.buttonSettingYoutube.UseVisualStyleBackColor = false;
			this.buttonSettingYoutube.Click += new EventHandler(this.buttonSettingYoutube_Click);
			this.buttonSettingTVPot.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonSettingTVPot.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonSettingTVPot.FlatAppearance.BorderSize = 2;
			this.buttonSettingTVPot.FlatStyle = FlatStyle.Flat;
			this.buttonSettingTVPot.Location = new Point(270, 261);
			this.buttonSettingTVPot.Name = "buttonSettingTVPot";
			this.buttonSettingTVPot.Size = new System.Drawing.Size(43, 23);
			this.buttonSettingTVPot.TabIndex = 45;
			this.buttonSettingTVPot.Text = "설정";
			this.buttonSettingTVPot.UseVisualStyleBackColor = false;
			this.buttonSettingTVPot.Click += new EventHandler(this.buttonSettingTVPot_Click);
			this.labelStatYoutube.BackColor = Color.DimGray;
			this.labelStatYoutube.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.labelStatYoutube.ForeColor = Color.White;
			this.labelStatYoutube.Location = new Point(13, 224);
			this.labelStatYoutube.Name = "labelStatYoutube";
			this.labelStatYoutube.Size = new System.Drawing.Size(300, 25);
			this.labelStatYoutube.TabIndex = 49;
			this.labelStatYoutube.Text = "대기중";
			this.labelStatYoutube.TextAlign = ContentAlignment.TopCenter;
			this.labelStatTVpot.BackColor = Color.DimGray;
			this.labelStatTVpot.Font = new System.Drawing.Font("Malgun Gothic", 12f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.labelStatTVpot.ForeColor = Color.White;
			this.labelStatTVpot.Location = new Point(13, 287);
			this.labelStatTVpot.Name = "labelStatTVpot";
			this.labelStatTVpot.Size = new System.Drawing.Size(300, 25);
			this.labelStatTVpot.TabIndex = 50;
			this.labelStatTVpot.Text = "대기중";
			this.labelStatTVpot.TextAlign = ContentAlignment.TopCenter;
			this.label12.Font = new System.Drawing.Font("Malgun Gothic", 15.75f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label12.ForeColor = Color.FromArgb(0, 180, 204);
			this.label12.Location = new Point(336, 95);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(443, 30);
			this.label12.TabIndex = 53;
			this.label12.Text = "CHAT VIEW";
			this.label12.TextAlign = ContentAlignment.MiddleCenter;
			this.labelVersion.AutoSize = true;
			this.labelVersion.Font = new System.Drawing.Font("Gulim", 9f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.labelVersion.ForeColor = Color.FromArgb(0, 180, 204);
			this.labelVersion.Location = new Point(512, 66);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(82, 12);
			this.labelVersion.TabIndex = 54;
			this.labelVersion.Text = "build.beta.1";
			this.buttonExit.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonExit.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonExit.FlatAppearance.BorderSize = 2;
			this.buttonExit.FlatStyle = FlatStyle.Flat;
			this.buttonExit.Location = new Point(668, 583);
			this.buttonExit.Name = "buttonExit";
			this.buttonExit.Size = new System.Drawing.Size(111, 23);
			this.buttonExit.TabIndex = 55;
			this.buttonExit.Text = "종료";
			this.buttonExit.UseVisualStyleBackColor = false;
			this.buttonExit.Click += new EventHandler(this.buttonExit_Click);
			this.buttonProgramInfo.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonProgramInfo.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonProgramInfo.FlatAppearance.BorderSize = 2;
			this.buttonProgramInfo.FlatStyle = FlatStyle.Flat;
			this.buttonProgramInfo.Location = new Point(551, 583);
			this.buttonProgramInfo.Name = "buttonProgramInfo";
			this.buttonProgramInfo.Size = new System.Drawing.Size(111, 23);
			this.buttonProgramInfo.TabIndex = 56;
			this.buttonProgramInfo.Text = "프로그램 정보";
			this.buttonProgramInfo.UseVisualStyleBackColor = false;
			this.buttonProgramInfo.Click += new EventHandler(this.buttonProgramInfo_Click);
			this.buttonGoSite.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonGoSite.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonGoSite.FlatAppearance.BorderSize = 2;
			this.buttonGoSite.FlatStyle = FlatStyle.Flat;
			this.buttonGoSite.Location = new Point(317, 583);
			this.buttonGoSite.Name = "buttonGoSite";
			this.buttonGoSite.Size = new System.Drawing.Size(111, 23);
			this.buttonGoSite.TabIndex = 57;
			this.buttonGoSite.Text = "사이트 방문";
			this.buttonGoSite.UseVisualStyleBackColor = false;
			this.buttonGoSite.Click += new EventHandler(this.buttonGoSite_Click);
			this.buttonPatchNote.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonPatchNote.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonPatchNote.FlatAppearance.BorderSize = 2;
			this.buttonPatchNote.FlatStyle = FlatStyle.Flat;
			this.buttonPatchNote.Location = new Point(83, 583);
			this.buttonPatchNote.Name = "buttonPatchNote";
			this.buttonPatchNote.Size = new System.Drawing.Size(111, 23);
			this.buttonPatchNote.TabIndex = 58;
			this.buttonPatchNote.Text = "패치노트 보기";
			this.buttonPatchNote.UseVisualStyleBackColor = false;
			this.buttonPatchNote.Click += new EventHandler(this.buttonPatchNote_Click);
			this.labelOffline.AutoSize = true;
			this.labelOffline.Font = new System.Drawing.Font("Gulim", 9f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.labelOffline.ForeColor = Color.Red;
			this.labelOffline.Location = new Point(512, 48);
			this.labelOffline.Name = "labelOffline";
			this.labelOffline.Size = new System.Drawing.Size(194, 12);
			this.labelOffline.TabIndex = 59;
			this.labelOffline.Text = "최신버전을 확인할 수 없습니다.";
			this.checkBoxTestChat.AutoSize = true;
			this.checkBoxTestChat.Font = new System.Drawing.Font("Gulim", 9f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.checkBoxTestChat.ForeColor = Color.White;
			this.checkBoxTestChat.Location = new Point(567, 551);
			this.checkBoxTestChat.Name = "checkBoxTestChat";
			this.checkBoxTestChat.Size = new System.Drawing.Size(107, 16);
			this.checkBoxTestChat.TabIndex = 60;
			this.checkBoxTestChat.Text = "테스트용 채팅";
			this.checkBoxTestChat.UseVisualStyleBackColor = true;
			this.checkBoxTestChat.CheckedChanged += new EventHandler(this.checkBoxTestChat_CheckedChanged);
			this.label_minimize.BackColor = Color.FromArgb(0, 180, 204);
			this.label_minimize.Font = new System.Drawing.Font("Malgun Gothic", 14.25f, FontStyle.Bold, GraphicsUnit.Point, 129);
			this.label_minimize.Location = new Point(736, 3);
			this.label_minimize.Name = "label_minimize";
			this.label_minimize.Size = new System.Drawing.Size(23, 26);
			this.label_minimize.TabIndex = 61;
			this.label_minimize.Text = "_";
			this.label_minimize.TextAlign = ContentAlignment.TopRight;
			this.label_minimize.Click += new EventHandler(this.label_minimize_Click);
			this.buttonWidgetAdd.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonWidgetAdd.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonWidgetAdd.FlatAppearance.BorderSize = 2;
			this.buttonWidgetAdd.FlatStyle = FlatStyle.Flat;
			this.buttonWidgetAdd.Location = new Point(206, 387);
			this.buttonWidgetAdd.Name = "buttonWidgetAdd";
			this.buttonWidgetAdd.Size = new System.Drawing.Size(107, 23);
			this.buttonWidgetAdd.TabIndex = 63;
			this.buttonWidgetAdd.Text = "새 위젯 추가";
			this.buttonWidgetAdd.UseVisualStyleBackColor = false;
			this.buttonWidgetAdd.Click += new EventHandler(this.buttonWidgetAdd_Click);
			this.buttonWidgetRemove.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonWidgetRemove.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonWidgetRemove.FlatAppearance.BorderSize = 2;
			this.buttonWidgetRemove.FlatStyle = FlatStyle.Flat;
			this.buttonWidgetRemove.Location = new Point(206, 416);
			this.buttonWidgetRemove.Name = "buttonWidgetRemove";
			this.buttonWidgetRemove.Size = new System.Drawing.Size(107, 23);
			this.buttonWidgetRemove.TabIndex = 64;
			this.buttonWidgetRemove.Text = "위젯 제거";
			this.buttonWidgetRemove.UseVisualStyleBackColor = false;
			this.buttonWidgetRemove.Click += new EventHandler(this.buttonWidgetRemove_Click);
			this.buttonWidgetSetting.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonWidgetSetting.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonWidgetSetting.FlatAppearance.BorderSize = 2;
			this.buttonWidgetSetting.FlatStyle = FlatStyle.Flat;
			this.buttonWidgetSetting.Location = new Point(206, 495);
			this.buttonWidgetSetting.Name = "buttonWidgetSetting";
			this.buttonWidgetSetting.Size = new System.Drawing.Size(107, 23);
			this.buttonWidgetSetting.TabIndex = 65;
			this.buttonWidgetSetting.Text = "설정";
			this.buttonWidgetSetting.UseVisualStyleBackColor = false;
			this.buttonWidgetSetting.Click += new EventHandler(this.buttonWidgetSetting_Click);
			this.listViewWidgets.BorderStyle = BorderStyle.FixedSingle;
			this.listViewWidgets.Columns.AddRange(new ColumnHeader[] { this.widget, this.enable });
			this.listViewWidgets.FullRowSelect = true;
			this.listViewWidgets.GridLines = true;
			this.listViewWidgets.Location = new Point(18, 387);
			this.listViewWidgets.MultiSelect = false;
			this.listViewWidgets.Name = "listViewWidgets";
			this.listViewWidgets.Size = new System.Drawing.Size(182, 160);
			this.listViewWidgets.TabIndex = 66;
			this.listViewWidgets.UseCompatibleStateImageBehavior = false;
			this.listViewWidgets.View = View.Details;
			this.listViewWidgets.SelectedIndexChanged += new EventHandler(this.listViewWidgets_SelectedIndexChanged);
			this.widget.Text = "위젯";
			this.widget.Width = 107;
			this.enable.Text = "활성화";
			this.enable.Width = 52;
			this.buttonWidgetEnable.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonWidgetEnable.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonWidgetEnable.FlatAppearance.BorderSize = 2;
			this.buttonWidgetEnable.FlatStyle = FlatStyle.Flat;
			this.buttonWidgetEnable.Location = new Point(206, 524);
			this.buttonWidgetEnable.Name = "buttonWidgetEnable";
			this.buttonWidgetEnable.Size = new System.Drawing.Size(107, 23);
			this.buttonWidgetEnable.TabIndex = 67;
			this.buttonWidgetEnable.Text = "비활성화";
			this.buttonWidgetEnable.UseVisualStyleBackColor = false;
			this.buttonWidgetEnable.Click += new EventHandler(this.buttonWidgetEnable_Click);
			this.buttonTutorial.BackColor = Color.FromArgb(0, 180, 204);
			this.buttonTutorial.FlatAppearance.BorderColor = Color.FromArgb(0, 116, 132);
			this.buttonTutorial.FlatAppearance.BorderSize = 2;
			this.buttonTutorial.FlatStyle = FlatStyle.Flat;
			this.buttonTutorial.Location = new Point(200, 583);
			this.buttonTutorial.Name = "buttonTutorial";
			this.buttonTutorial.Size = new System.Drawing.Size(111, 23);
			this.buttonTutorial.TabIndex = 68;
			this.buttonTutorial.Text = "사용법 보기";
			this.buttonTutorial.UseVisualStyleBackColor = false;
			this.buttonTutorial.Click += new EventHandler(this.buttonTutorial_Click);
			base.AutoScaleDimensions = new SizeF(7f, 12f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
			this.BackColor = Color.FromArgb(44, 52, 54);
			base.ClientSize = new System.Drawing.Size(791, 616);
			base.Controls.Add(this.buttonTutorial);
			base.Controls.Add(this.buttonWidgetEnable);
			base.Controls.Add(this.listViewWidgets);
			base.Controls.Add(this.buttonWidgetSetting);
			base.Controls.Add(this.buttonWidgetRemove);
			base.Controls.Add(this.buttonWidgetAdd);
			base.Controls.Add(this.label_minimize);
			base.Controls.Add(this.checkBoxTestChat);
			base.Controls.Add(this.labelOffline);
			base.Controls.Add(this.webTwitch);
			base.Controls.Add(this.buttonPatchNote);
			base.Controls.Add(this.buttonGoSite);
			base.Controls.Add(this.buttonProgramInfo);
			base.Controls.Add(this.buttonExit);
			base.Controls.Add(this.labelVersion);
			base.Controls.Add(this.label12);
			base.Controls.Add(this.labelStatTVpot);
			base.Controls.Add(this.labelStatYoutube);
			base.Controls.Add(this.buttonSettingTVPot);
			base.Controls.Add(this.buttonSettingYoutube);
			base.Controls.Add(this.label_exit);
			base.Controls.Add(this.label9);
			base.Controls.Add(this.label8);
			base.Controls.Add(this.checkBoxChatTVPot);
			base.Controls.Add(this.checkBoxChatYoutube);
			base.Controls.Add(this.checkBoxChatTwitch);
			base.Controls.Add(this.label6);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.labelStatTwitch);
			base.Controls.Add(this.buttonSettingTwitch);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.webYoutube);
			base.Controls.Add(this.checkBoxAutoScrollChat);
			base.Controls.Add(this.pictureBox1);
			base.Controls.Add(this.buttonSurvey);
			base.Controls.Add(this.listView1);
			this.DoubleBuffered = true;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Icon = (System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "FormCoreWindow";
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "JSAssist";
			base.Shown += new EventHandler(this.FormCoreWindow_Shown);
			base.MouseDown += new MouseEventHandler(this.FormCoreWindow_MouseDown);
			((ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void InitWidgetList()
		{
			this.listViewWidgets.Items.Clear();
			ListViewItem listViewItem = new ListViewItem(new string[] { "채팅 위젯", "O" });
			this.listViewWidgets.Items.Add(listViewItem);
		}

		private void label_exit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void label_minimize_Click(object sender, EventArgs e)
		{
			base.WindowState = FormWindowState.Minimized;
		}

		private void listView1_DoubleClick(object sender, MouseEventArgs e)
		{
			int index = this.listView1.FocusedItem.Index;
			Console.WriteLine(this.listView1.Items[index].SubItems[0].Text);
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		private void listViewWidgets_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				Win32.ReleaseCapture();
				Win32.SendMessage(base.Handle, 161, 2, 0);
			}
		}

		private void pictureBox1_MouseDown_1(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				Win32.ReleaseCapture();
				Win32.SendMessage(base.Handle, 161, 2, 0);
			}
		}

		private void SetChatStat(Label label, ChatStat stat)
		{
			Color dimGray = Color.DimGray;
			string str = "";
			if (stat == ChatStat.Disabled)
			{
				dimGray = Color.DimGray;
				str = "비활성화됨";
			}
			else if (stat == ChatStat.Ready)
			{
				dimGray = Color.RosyBrown;
				str = "연동 준비중";
			}
			else if (stat == ChatStat.Run)
			{
				dimGray = Color.MediumAquamarine;
				str = "연동 성공";
			}
			label.BackColor = dimGray;
			label.Text = str;
		}

		public void SetChatStatTVPot(ChatStat stat)
		{
			this.SetChatStat(this.labelStatTVpot, stat);
		}

		public void SetChatStatTwitch(ChatStat stat)
		{
			this.SetChatStat(this.labelStatTwitch, stat);
		}

		public void SetChatStatYoutube(ChatStat stat)
		{
			this.SetChatStat(this.labelStatYoutube, stat);
		}

		public void SetChatTVPot(bool isEnable, bool saveConfig = false)
		{
			Program.chatManager.chatTVPot.SetActive(isEnable);
			this.checkBoxChatTVPot.Checked = isEnable;
			if (!isEnable)
			{
				this.SetChatStatTVPot(ChatStat.Disabled);
			}
			else
			{
				this.SetChatStatTVPot(ChatStat.Ready);
			}
			if (saveConfig)
			{
				Program.config.chatTVPotEnable = isEnable;
				Program.config.SaveFile();
			}
		}

		public bool SetChatTwitch(bool isEnable, bool saveConfig = false)
		{
			if (!isEnable)
			{
				this.webTwitch.Navigate("about:blank");
			}
			else
			{
				string str = Program.config.chatTwitchID;
				if (str == null || str == "")
				{
					isEnable = false;
					MessageBox.Show("Twitch 아이디가 입력되지 않았습니다. 설정을 확인해주세요.", "설정을 확인해주세요");
				}
				else
				{
					Program.chatManager.chatTwitch.SetURL(string.Concat("http://www.twitch.tv/", str, "/chat?popout="));
				}
			}
			this.checkBoxChatTwitch.Checked = isEnable;
			Program.chatManager.chatTwitch.SetActive(isEnable);
			if (!isEnable)
			{
				this.SetChatStatTwitch(ChatStat.Disabled);
			}
			else
			{
				this.SetChatStatTwitch(ChatStat.Ready);
			}
			if (saveConfig)
			{
				Program.config.chatTwitchEnable = isEnable;
				Program.config.SaveFile();
			}
			return true;
		}

		public bool SetChatYoutube(bool isEnable, bool saveConfig = false)
		{
			if (!isEnable)
			{
				this.webYoutube.Navigate("about:blank");
			}
			else
			{
				string str = Program.config.chatYoutubeID;
				if (str == null || str == "")
				{
					isEnable = false;
					MessageBox.Show("Youtube 아이디가 입력되지 않았습니다. 설정을 확인해주세요.", "설정을 확인해주세요");
				}
				else
				{
					Program.chatManager.chatYoutube.SetURL(string.Concat("http://www.youtube.com/live_chat?is_popout=1&v=", str));
				}
			}
			if (!isEnable)
			{
				this.SetChatStatYoutube(ChatStat.Disabled);
			}
			else
			{
				this.SetChatStatYoutube(ChatStat.Ready);
			}
			this.checkBoxChatYoutube.Checked = isEnable;
			Program.chatManager.chatYoutube.SetActive(isEnable);
			if (saveConfig)
			{
				Program.config.chatYoutubeEnable = isEnable;
				Program.config.SaveFile();
			}
			return true;
		}

		private void SetValueFromConfig()
		{
			JSAssistConfig jSAssistConfig = Program.config;
			this.labelVersion.Text = string.Concat("build.beta.", jSAssistConfig.build);
			this.checkBoxChatTVPot.Checked = jSAssistConfig.chatTVPotEnable;
			this.SetWidgetEnable("채팅 위젯", jSAssistConfig.widgetChatEnable);
			this.SetChatTwitch(jSAssistConfig.chatTwitchEnable, false);
			this.SetChatYoutube(jSAssistConfig.chatYoutubeEnable, false);
			this.SetChatTVPot(jSAssistConfig.chatTVPotEnable, false);
			Program.chatManager.isEnable = jSAssistConfig.widgetChatEnable;
		}

		private void SetWidgetEnable(string widgetName, bool enable)
		{
			int num = -1;
			int num1 = 0;
			while (num1 < this.listViewWidgets.Items.Count)
			{
				if (this.listViewWidgets.Items[num1].Text != widgetName)
				{
					num1++;
				}
				else
				{
					num = num1;
					break;
				}
			}
			if (num == -1)
			{
				return;
			}
			this.listViewWidgets.Items[num].SubItems[1].Text = (enable ? "O" : "X");
			if (enable)
			{
				this.buttonWidgetEnable.Text = "비활성화";
				return;
			}
			this.buttonWidgetEnable.Text = "활성화";
		}

		private void timer_update_Tick(object sender, EventArgs e)
		{
			Program.chatManager.Update();
		}
	}
}