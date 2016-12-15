using MSHTML;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace JSAssist
{
	internal class ChatTwitch : ChatBrowserBase
	{
		private IHTMLElement chatRoot;

		private int lastUniqueNumber;

		public ChatTwitch()
		{
		}

		public override void Initialize(ChatManager manager)
		{
			base.Initialize(manager);
			this.lastUniqueNumber = -1;
		}

		protected override void OnUserAdded(UserData newUser)
		{
			base.OnUserAdded(newUser);
			newUser.platform = "twitch";
		}

		protected override void PrepareUpdate()
		{
			base.PrepareUpdate();
			if (this.document == null)
			{
				this.document = (IHTMLDocument2)this.browser.Document.DomDocument;
			}
			foreach (IHTMLElement variable in this.document.all)
			{
				if (variable.className != "chat-lines")
				{
					continue;
				}
				this.chatRoot = variable;
				this.isReady = true;
				FormCoreWindow.inst.SetChatStatTwitch(ChatStat.Run);
				return;
			}
		}

		public override void Reset()
		{
			base.Reset();
			this.lastUniqueNumber = -1;
			this.chatRoot = null;
		}

		public override bool Update()
		{
			bool flag;
			if (!base.Update())
			{
				return false;
			}
			int num = -1;
			try
			{
				num = (int)((dynamic)this.chatRoot.children).length;
				goto Label0;
			}
			catch
			{
				this.chatRoot = null;
				this.isReady = false;
				flag = false;
			}
			return flag;
		Label0:
			int num1 = 0;
			int num2 = num - 1;
			while (num2 >= 0)
			{
				IHTMLElement variable = (IHTMLElement)((dynamic)this.chatRoot.children).Item(num2);
				if (this.lastUniqueNumber != -1)
				{
					int elementUniqueNumber = base.GetElementUniqueNumber(variable);
					if (this.lastUniqueNumber >= elementUniqueNumber)
					{
						break;
					}
					num1++;
					IHTMLElement variable1 = base.FindClassFromChild(variable, "from");
					if (variable1 != null)
					{
						string str = variable1.innerText;
						if (str != null)
						{
							int num3 = str.IndexOf(" (");
							if (num3 != -1)
							{
								str = str.Substring(0, num3);
							}
							IHTMLElement variable2 = base.FindClassFromChild(variable, "message");
							if (variable2 != null)
							{
								string str1 = variable2.innerText;
								if (str1 != null)
								{
									ChatData chatDatum = new ChatData()
									{
										user = base.GetUserData(str),
										message = str1
									};
									base.AddChatDataFront(chatDatum);
									this.lastUniqueNumber = elementUniqueNumber;
								}
							}
						}
					}
					num2--;
				}
				else
				{
					this.lastUniqueNumber = base.GetElementUniqueNumber(variable);
					break;
				}
			}
			return true;
		}
	}
}