using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace JSAssist
{
	internal class CheckRegistry
	{
		public CheckRegistry()
		{
		}

		public bool Check()
		{
			return this.CheckBrowserEmulation();
		}

		private bool CheckBrowserEmulation()
		{
			bool flag;
			bool flag1 = false;
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION");
			try
			{
				if (registryKey.GetValue("jsassist.exe").ToString() != "11001")
				{
					flag1 = true;
				}
			}
			catch
			{
				flag1 = true;
			}
			if (!flag1)
			{
				return true;
			}
			try
			{
				Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true).SetValue("jsassist.exe", "11001", RegistryValueKind.DWord);
				flag = true;
			}
			catch
			{
				MessageBox.Show("초기화에 실패하였습니다. 관리자 권한으로 다시 실행해주세요. 이 작업은 최초 실행에 필요합니다.", "초기화 실패");
				return false;
			}
			return flag;
		}
	}
}