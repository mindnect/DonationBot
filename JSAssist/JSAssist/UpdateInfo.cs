using System;

namespace JSAssist
{
	[Serializable]
	internal class UpdateInfo
	{
		public int version;

		public FileInfo[] listFile;

		public UpdateInfo()
		{
		}
	}
}