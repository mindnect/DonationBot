using MSHTML;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using mshtml;

namespace JSAssist
{
	internal class ChatYoutube : ChatBrowserBase
	{
		private IHTMLElement chatRoot;

		private int lastUniqueNumber;

		private bool isFirstLoad;

		public ChatYoutube()
		{
		}

		public override void Initialize(ChatManager manager)
		{
			base.Initialize(manager);
			this.lastUniqueNumber = -1;
			this.isFirstLoad = true;
		}

		protected override void OnUserAdded(UserData newUser)
		{
			base.OnUserAdded(newUser);
			newUser.platform = "youtube";
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
				if (variable.id != "all-comments")
				{
					continue;
				}
				this.chatRoot = variable;
				this.isReady = true;
				FormCoreWindow.inst.SetChatStatYoutube(ChatStat.Run);
				return;
			}
		}

		private string RemoveWhiteSpace(string str)
		{
			int num = 0;
			int num1 = 0;
			int num2 = 0;
			while (num2 < str.Length)
			{
				if (str[num2] == ' ')
				{
					num2++;
				}
				else
				{
					num = num2;
					break;
				}
			}
			int length = str.Length - 1;
			while (length >= 0)
			{
				if (str[length] == ' ')
				{
					length--;
				}
				else
				{
					num1 = length;
					break;
				}
			}
			return str.Substring(num, num1 - num + 1);
		}

		public override void Reset()
		{
			this.lastUniqueNumber = -1;
			this.isFirstLoad = true;
			base.Reset();
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
			bool flag1 = true;
			int elementUniqueNumber = 0;
			for (int i = num - 1; i >= 0; i--)
			{
				IHTMLElement variable = (IHTMLElement)((dynamic)this.chatRoot.children).Item(i);
				if (flag1)
				{
					elementUniqueNumber = base.GetElementUniqueNumber(variable);
					flag1 = false;
				}
				int elementUniqueNumber1 = base.GetElementUniqueNumber(variable);
				if (this.lastUniqueNumber >= elementUniqueNumber1)
				{
					break;
				}
				if (variable.className.Contains("comment "))
				{
					IHTMLElement variable1 = base.FindClassFromChild(variable, "content");
					if (variable1 == null)
					{
						goto Label1;
					}
					string str = base.FindClassFromChild(variable1, "byline").outerText;
					string str1 = this.RemoveWhiteSpace(str);
					str = base.FindClassFromChild(variable1, "comment-text").outerText;
					string str2 = this.RemoveWhiteSpace(str);
					ChatData chatDatum = new ChatData()
					{
						user = base.GetUserData(str1),
						message = str2
					};
					base.AddChatDataFront(chatDatum);
				}
				if (this.isFirstLoad)
				{
					this.isFirstLoad = false;
					break;
				}
			Label1:
			}
			this.lastUniqueNumber = elementUniqueNumber;
			return true;
		}
	}
}