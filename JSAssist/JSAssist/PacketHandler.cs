using System;
using System.IO;
using System.Text;

namespace JSAssist
{
	internal class PacketHandler
	{
		public PacketHandler()
		{
		}

		public virtual void ProcessPacket(int type, BinaryReader br)
		{
			if (type == 2)
			{
				string str = this.ReadString(br, 128);
				Console.WriteLine(string.Concat("Test Message Received : ", str));
			}
		}

		protected bool ReadBoolean(BinaryReader br)
		{
			if (br.ReadInt32() == 1)
			{
				return true;
			}
			return false;
		}

		protected string ReadString(BinaryReader br, int size)
		{
			byte[] numArray = br.ReadBytes(size);
			string str = Encoding.UTF8.GetString(numArray);
			int num = 0;
			while (num < size)
			{
				if (str[num] != 0)
				{
					num++;
				}
				else
				{
					str = str.Substring(0, num);
					break;
				}
			}
			return str;
		}
	}
}