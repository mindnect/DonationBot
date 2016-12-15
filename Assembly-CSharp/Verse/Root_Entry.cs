using RimWorld;
using System;

namespace Verse
{
	public class Root_Entry : Root
	{
		public MusicManagerEntry musicManagerEntry;

		public override void Start()
		{
			base.Start();
			this.musicManagerEntry = new MusicManagerEntry();
		}

		public override void Update()
		{
			base.Update();
			if (LongEventHandler.ShouldWaitForEvent)
			{
				return;
			}
			this.musicManagerEntry.MusicManagerEntryUpdate();
			if (Find.World != null)
			{
				Find.World.WorldUpdate();
			}
		}
	}
}
