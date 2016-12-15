using System;

namespace JSAssist
{
	[Serializable]
	public class NoticePopupData
	{
		public string message;

		public float startDelay;

		public float startDelayElapsed;

		public float repeatDelay;

		public float repeatDelayElapsed;

		public bool isEnabled;

		public NoticePopupData()
		{
		}
	}
}