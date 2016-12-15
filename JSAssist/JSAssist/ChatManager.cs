using System;
using System.Collections.Generic;

namespace JSAssist
{
	internal class ChatManager
	{
		public ChatTwitch chatTwitch;

		public ChatTVPot chatTVPot;

		public ChatYoutube chatYoutube;

		public bool isEnable;

		public bool isTestChatEnable;

		private Random rand;

		private int errCountTwitch;

		private int errCountYoutube;

		private int errCountTVPot;

		public ChatManager()
		{
			this.isEnable = true;
			this.isTestChatEnable = false;
			this.chatTwitch = new ChatTwitch();
			this.chatTwitch.Initialize(this);
			this.chatTVPot = new ChatTVPot();
			this.chatTVPot.Initialize(this);
			this.chatYoutube = new ChatYoutube();
			this.chatYoutube.Initialize(this);
			Guid guid = Guid.NewGuid();
			this.rand = new Random(guid.GetHashCode());
			this.errCountTwitch = 0;
			this.errCountYoutube = 0;
			this.errCountTVPot = 0;
		}

		private void SendChat(ChatBase chatBase)
		{
			List<ChatData> newChatList = chatBase.GetNewChatList();
			if (this.isEnable)
			{
				foreach (ChatData chatDatum in newChatList)
				{
					if (chatDatum.message == null || chatDatum.message == "")
					{
						continue;
					}
					Program.server.SendChat(chatDatum);
					FormCoreWindow.AddChat(chatDatum.user.platform, chatDatum.user.username, chatDatum.message);
				}
			}
			newChatList.Clear();
		}

		private void SendChatAll()
		{
			try
			{
				this.SendChat(this.chatTwitch);
				this.SendChat(this.chatTVPot);
				this.SendChat(this.chatYoutube);
			}
			catch
			{
				Console.WriteLine("SendChat Error");
			}
			this.SendTestChat();
		}

		private void SendTestChat()
		{
			if (this.isTestChatEnable)
			{
				if (this.rand.Next(0, 100) >= 30)
				{
					return;
				}
				int num = this.rand.Next(0, 3);
				int num1 = this.rand.Next(0, 4);
				int num2 = this.rand.Next(0, 6);
				ChatData chatDatum = new ChatData()
				{
					user = new UserData()
				};
				if (num == 0)
				{
					chatDatum.user.platform = "twitch";
				}
				else if (num != 1)
				{
					chatDatum.user.platform = "tvpot";
				}
				else
				{
					chatDatum.user.platform = "youtube";
				}
				if (num1 == 0)
				{
					chatDatum.user.username = "짓쑤";
				}
				else if (num1 == 1)
				{
					chatDatum.user.username = "수상한 아저씨";
				}
				else if (num1 == 2)
				{
					chatDatum.user.username = "철없는 아이";
				}
				else if (num1 == 3)
				{
					chatDatum.user.username = "마음의 소리";
				}
				if (num2 == 0)
				{
					chatDatum.message = "배가 너무 고파서 아무것도 못하겠네..";
				}
				else if (num2 == 1)
				{
					chatDatum.message = "안녕하세요~";
				}
				else if (num2 == 2)
				{
					chatDatum.message = "아~~ 지루하고 재미없어.";
				}
				else if (num2 == 3)
				{
					chatDatum.message = "오늘 바깥 날씨가 매우 좋아요. 우리 함께 놀러가지 않을래요?";
				}
				else if (num2 == 4)
				{
					chatDatum.message = "맙소사, 내 방송에 사람이 들어왔어!";
				}
				else if (num2 == 5)
				{
					chatDatum.message = "당신은 저를 볼 수 없어요.";
				}
				Program.server.SendChat(chatDatum);
				FormCoreWindow.AddChat(chatDatum.user.platform, chatDatum.user.username, chatDatum.message);
			}
		}

		public void Update()
		{
			try
			{
				this.chatTwitch.Update();
				this.errCountTwitch = 0;
			}
			catch
			{
				this.errCountTwitch = this.errCountTwitch + 1;
				if (this.errCountTwitch >= 5)
				{
					this.chatTwitch.Reset();
					Console.WriteLine("Too many error on twitch document. reset");
				}
			}
			try
			{
				this.chatYoutube.Update();
				this.errCountYoutube = 0;
			}
			catch
			{
				this.errCountYoutube = this.errCountYoutube + 1;
				if (this.errCountYoutube >= 5)
				{
					this.chatYoutube.Reset();
					Console.WriteLine("Too many error on youtube document. reset");
					this.errCountYoutube = 0;
				}
			}
			try
			{
				this.chatTVPot.Update();
				this.errCountTVPot = 0;
			}
			catch
			{
				this.errCountTVPot = this.errCountTVPot + 1;
				if (this.errCountTVPot >= 5)
				{
					this.chatTVPot.Reset();
					Console.WriteLine("Too many error on TVpot. reset");
					this.errCountTVPot = 0;
				}
			}
			this.SendChatAll();
		}
	}
}