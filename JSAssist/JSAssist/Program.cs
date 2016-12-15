using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace JSAssist
{
	internal static class Program
	{
		public static ChatManager chatManager;

		public static Server server;

		public static int currentVersion;

		public static bool isOfflineMode;

		public static string basePath;

		public static string dataPath;

		public static JSAssistConfig config;

		static Program()
		{
			Program.currentVersion = 15;
			Program.basePath = "";
			Program.dataPath = "";
		}

		public static string GetDate()
		{
			int year = DateTime.Now.Year - 2000;
			int month = DateTime.Now.Month;
			int day = DateTime.Now.Day;
			return string.Concat(year, month.ToString("00"), day.ToString("00"));
		}

		[STAThread]
		private static void Main()
		{
			bool flag = false;
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < (int)commandLineArgs.Length; i++)
			{
				if (commandLineArgs[i] == "skipupdate")
				{
					flag = true;
				}
			}
			if (!(new CheckRegistry()).Check())
			{
				return;
			}
			Program.basePath = AppDomain.CurrentDomain.BaseDirectory;
			Program.dataPath = string.Concat(Program.basePath, "data\\");
			if (!Directory.Exists(Program.dataPath))
			{
				Directory.CreateDirectory(Program.dataPath);
			}
			Program.config = new JSAssistConfig(string.Concat(Program.dataPath, "config.xml"));
			if (!flag)
			{
				Update update = new Update()
				{
					localPath = Program.basePath
				};
				update.CheckLatestVersion();
				if (!update.updateSuccess)
				{
					Program.isOfflineMode = true;
				}
				else
				{
					if (update.mainFileModified)
					{
						if (!File.Exists(string.Concat(Program.dataPath, "JSAssist_Update.exe")))
						{
							MessageBox.Show("업데이트를 진행할 수 없습니다. 문제가 지속될 경우 프로그램을 다시 받아주세요.", "업데이트 실패");
							return;
						}
						Program.config.skipPatchNote = false;
						Program.config.SaveFile();
						Process.Start(string.Concat(Program.dataPath, "JSAssist_Update.exe"));
						return;
					}
					Program.currentVersion = update.version;
					Program.config.build = Program.currentVersion;
					Program.config.SaveFile();
				}
			}
			Program.server = new Server();
			Program.server.StartServer();
			Program.chatManager = new ChatManager();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new FormCoreWindow());
			Program.server.StopServer();
		}
	}
}