using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;

namespace RimWorld
{
	public static class StorageSettingsClipboard
	{
		private static StorageSettings clipboard = new StorageSettings();

		private static bool copied = false;

		public static bool HasCopiedSettings
		{
			get
			{
				return StorageSettingsClipboard.copied;
			}
		}

		public static void Copy(StorageSettings s)
		{
			StorageSettingsClipboard.clipboard.CopyFrom(s);
			StorageSettingsClipboard.copied = true;
		}

		public static void PasteInto(StorageSettings s)
		{
			s.CopyFrom(StorageSettingsClipboard.clipboard);
		}

		[DebuggerHidden]
		public static IEnumerable<Gizmo> CopyPasteGizmosFor(StorageSettings s)
		{
			StorageSettingsClipboard.<CopyPasteGizmosFor>c__Iterator134 <CopyPasteGizmosFor>c__Iterator = new StorageSettingsClipboard.<CopyPasteGizmosFor>c__Iterator134();
			<CopyPasteGizmosFor>c__Iterator.s = s;
			<CopyPasteGizmosFor>c__Iterator.<$>s = s;
			StorageSettingsClipboard.<CopyPasteGizmosFor>c__Iterator134 expr_15 = <CopyPasteGizmosFor>c__Iterator;
			expr_15.$PC = -2;
			return expr_15;
		}
	}
}
