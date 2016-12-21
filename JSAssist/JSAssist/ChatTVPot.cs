using MSHTML;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using mshtml;

namespace JSAssist
{
	internal class ChatTVPot : ChatBase
	{
		public WindowInfo[] windowPotPlayers;

		public int selectedPotPlayerIdx;

		private int lastUniqueNumber;

		private int lastElementIdx;

		private bool waitForSelect;

		private IHTMLElement chatRoot;

		public ChatTVPot()
		{
		}

		private void FindChatRoot()
		{
			if (this.selectedPotPlayerIdx != -1)
			{
				IntPtr intPtr = this.windowPotPlayers[this.selectedPotPlayerIdx].hWnd;
				IntPtr intPtr1 = this.FindExplorerHandleByChatWindow(this.FindChatWindowHandle());
				if (intPtr1 == IntPtr.Zero)
				{
					foreach (IntPtr childWindow in Win32.GetChildWindows(intPtr))
					{
						if (Win32.GetWinClass(childWindow) != "Internet Explorer_Server")
						{
							continue;
						}
						IHTMLDocument2 document = this.GetDocument(childWindow);
						if (document == null)
						{
							continue;
						}
						IHTMLElement variable = this.FindChatRoot(document, 50);
						if (variable == null || this.chatRoot != null)
						{
							continue;
						}
						this.document = document;
						this.chatRoot = variable.parentElement;
						this.isReady = true;
						FormCoreWindow.inst.SetChatStatTVPot(ChatStat.Run);
						return;
					}
				}
				else
				{
					IHTMLDocument2 document1 = this.GetDocument(intPtr1);
					if (document1 != null)
					{
						IHTMLElement variable1 = this.FindChatRoot(document1, 50);
						if (variable1 != null)
						{
							this.document = document1;
							this.chatRoot = variable1.parentElement;
							this.isReady = true;
							FormCoreWindow.inst.SetChatStatTVPot(ChatStat.Run);
							return;
						}
					}
				}
			}
		}

		private IHTMLElement FindChatRoot(IHTMLDocument2 tempDocument, int maxAttemptCount)
		{
			int num = maxAttemptCount;
			foreach (object child in (IEnumerable)((dynamic)tempDocument.body.children))
			{
				IHTMLElement variable = (IHTMLElement)((dynamic)child);
				num--;
				if (num > 0)
				{
					if (variable.className != "wrap_chat")
					{
						continue;
					}
					int num1 = 0;
					foreach (object obj in (IEnumerable)((dynamic)variable.children))
					{
						IHTMLElement variable1 = (IHTMLElement)((dynamic)obj);
						num1++;
					}
					return variable;
				}
				else
				{
					return null;
				}
			}
			return null;
		}

		private IntPtr FindChatWindowHandle()
		{
			IntPtr zero = IntPtr.Zero;
			int num = 5;
			for (int i = 0; i < num; i++)
			{
				zero = Win32.FindWindowEx(IntPtr.Zero, zero, null, "채팅/덧글");
				if (zero == IntPtr.Zero)
				{
					break;
				}
				uint num1 = 0;
				Win32.GetWindowThreadProcessId(zero, out num1);
				if (num1 == this.windowPotPlayers[this.selectedPotPlayerIdx].pid)
				{
					return zero;
				}
			}
			return IntPtr.Zero;
		}

		private IntPtr FindExplorerHandleByChatWindow(IntPtr hWndChatRoot)
		{
			IntPtr intPtr = hWndChatRoot;
			IntPtr zero = IntPtr.Zero;
			while (true)
			{
				zero = Win32.FindWindowEx(intPtr, IntPtr.Zero, "Internet Explorer_Server", null);
				if (zero == IntPtr.Zero)
				{
					zero = Win32.FindWindowEx(intPtr, IntPtr.Zero, null, "");
					if (zero == IntPtr.Zero)
					{
						break;
					}
					intPtr = zero;
				}
				else
				{
					Console.Write("Control Found");
					break;
				}
			}
			return zero;
		}

		public void FindPotplayer()
		{
			if (this.waitForSelect)
			{
				return;
			}
			if (this.selectedPotPlayerIdx == -1)
			{
				this.RefreshPotPlayerHandleList();
				if ((int)this.windowPotPlayers.Length == 1)
				{
					this.selectedPotPlayerIdx = 0;
				}
				else if ((int)this.windowPotPlayers.Length > 1)
				{
					(new FormSelectPotPlayer()).Show();
					this.waitForSelect = true;
				}
			}
			if (this.selectedPotPlayerIdx == -1)
			{
				return;
			}
			this.FindChatRoot();
		}

		private IHTMLDocument2 GetDocument(IntPtr hWndExplorer)
		{
			int num;
			IHTMLDocument2 variable = null;
			int num1 = 0;
			num1 = Win32.RegisterWindowMessage("WM_HTML_GETOBJECT");
			if (num1 != 0)
			{
				Console.WriteLine("Get Explorer Document");
				Win32.SendMessageTimeout(hWndExplorer, num1, 0, 0, 2, 1000, out num);
				if (num != 0)
				{
					Win32.ObjectFromLresult(num, ref Win32.IID_IHTMLDocument, 0, ref variable);
					if (variable == null)
					{
						MessageBox.Show("Couldn't Found Document", "Warning");
					}
				}
			}
			return variable;
		}

		public override void Initialize(ChatManager manager)
		{
			base.Initialize(manager);
			this.selectedPotPlayerIdx = -1;
			this.lastUniqueNumber = -1;
			this.lastElementIdx = -1;
			this.waitForSelect = false;
		}

		protected override void OnUserAdded(UserData newUser)
		{
			base.OnUserAdded(newUser);
			newUser.platform = "tvpot";
		}

		protected override void PrepareUpdate()
		{
			this.FindPotplayer();
			base.PrepareUpdate();
		}

		public void RefreshPotPlayerHandleList()
		{
			int num = 10;
			IntPtr[] intPtrArray = new IntPtr[num];
			int num1 = 0;
			IntPtr zero = IntPtr.Zero;
			for (int i = 0; i < num; i++)
			{
				zero = Win32.FindWindowEx(IntPtr.Zero, zero, "PotPlayer", null);
				if (zero == IntPtr.Zero)
				{
					break;
				}
				intPtrArray[i] = zero;
				num1++;
			}
			for (int j = num1; j < num; j++)
			{
				zero = Win32.FindWindowEx(IntPtr.Zero, zero, "PotPlayer64", null);
				if (zero == IntPtr.Zero)
				{
					break;
				}
				intPtrArray[j] = zero;
				num1++;
			}
			this.windowPotPlayers = new WindowInfo[num1];
			StringBuilder stringBuilder = new StringBuilder(260);
			for (int k = 0; k < num1; k++)
			{
				Win32.GetWindowText(intPtrArray[k], stringBuilder, 260);
				uint num2 = 0;
				Win32.GetWindowThreadProcessId(intPtrArray[k], out num2);
				this.windowPotPlayers[k] = new WindowInfo();
				this.windowPotPlayers[k].hWnd = intPtrArray[k];
				this.windowPotPlayers[k].caption = stringBuilder.ToString();
				this.windowPotPlayers[k].pid = num2;
			}
		}

		public override void Reset()
		{
			base.Reset();
			this.chatRoot = null;
			this.lastUniqueNumber = -1;
			this.lastElementIdx = -1;
			this.isReady = false;
		}

		public void SelectPotPlayerWindow(int idxWindow)
		{
			this.Reset();
			this.selectedPotPlayerIdx = idxWindow;
			this.waitForSelect = false;
		}

		public override bool Update()
		{
			if (!base.Update())
			{
				return false;
			}
			this.UpdateChat();
			return true;
		}

		private void UpdateChat()
		{
			this.UpdateChatRoom(this.chatRoot);
		}

		private bool UpdateChatRoom(IHTMLElement root)
		{
			bool flag;
			int num = 0;
			try
			{
				num = (int)((dynamic)root.children).length;
				goto Label0;
			}
			catch
			{
				this.lastElementIdx = -1;
				this.lastUniqueNumber = -1;
				this.chatRoot = null;
				this.FindChatRoot();
				flag = false;
			}
			return flag;
		Label0:
			if (this.lastElementIdx == -1)
			{
				this.lastElementIdx = num - 1;
			}
			if (this.lastElementIdx > ((dynamic)root.children).length - 1)
			{
				int num1 = 50;
				int num2 = num - 1;
				while (num2 >= num - 1 - num1)
				{
					if (base.GetElementUniqueNumber((IHTMLElement)((dynamic)root.children).Item(num2)) != this.lastUniqueNumber)
					{
						num2--;
					}
					else
					{
						this.lastElementIdx = num2 + 1;
						break;
					}
				}
			}
			for (int i = this.lastElementIdx + 1; i < num; i++)
			{
				IHTMLElement variable = (IHTMLElement)((dynamic)root.children).Item(i);
				if (variable == null)
				{
					break;
				}
				IHTMLElement variable1 = (IHTMLElement)((dynamic)variable.children).Item(0);
				int length = variable1.className.Length;
				if ((length == 9 || length == 20) && variable1.className.Contains("area_chat"))
				{
					IHTMLElement variable2 = base.FindClassFromChild(variable1, "tit_name");
					if (variable2 != null)
					{
						string str = variable2.outerText;
						int num3 = -1;
						for (int j = 0; j < str.Length; j++)
						{
							if (str[j] == '(')
							{
								num3 = j;
							}
						}
						string str1 = str.Substring(0, num3);
						if (str1[0] == 'P')
						{
							str1 = str1.Replace("PD\r\n", "");
						}
						else if (str1[0] == 'A')
						{
							str1 = str1.Replace("AD\r\n", "");
						}
						if (str1[str1.Length - 1] == ' ')
						{
							str1 = str1.Remove(str1.Length - 1);
						}
						IHTMLElement variable3 = base.FindClassFromChild(variable1, "info_words");
						if (variable3 != null)
						{
							string str2 = variable3.outerText;
							ChatData chatDatum = new ChatData()
							{
								user = base.GetUserData(str1),
								message = str2
							};
							base.AddChatData(chatDatum);
						}
					}
				}
			}
			this.lastElementIdx = num - 1;
			try
			{
				IHTMLElement variable4 = (IHTMLElement)((dynamic)root.children).Item(num - 1);
				this.lastUniqueNumber = base.GetElementUniqueNumber(variable4);
			}
			catch
			{
			}
			return false;
		}
	}
}