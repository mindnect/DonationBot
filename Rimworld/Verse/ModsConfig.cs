using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Verse
{
	public static class ModsConfig
	{
		private class ModsConfigData
		{
			public int buildNumber = -1;

			public List<string> activeMods = new List<string>();
		}

		private static ModsConfig.ModsConfigData data;

		public static IEnumerable<ModMetaData> ActiveModsInLoadOrder
		{
			get
			{
				ModsConfig.<>c__Iterator1EA <>c__Iterator1EA = new ModsConfig.<>c__Iterator1EA();
				ModsConfig.<>c__Iterator1EA expr_07 = <>c__Iterator1EA;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		static ModsConfig()
		{
			bool flag = false;
			ModsConfig.data = XmlLoader.ItemFromXmlFile<ModsConfig.ModsConfigData>(GenFilePaths.ModsConfigFilePath, true);
			if (ModsConfig.data.buildNumber < VersionControl.CurrentBuild)
			{
				Log.Message(string.Concat(new object[]
				{
					"Mods config data is from build ",
					ModsConfig.data.buildNumber,
					" while we are at build ",
					VersionControl.CurrentBuild,
					". Resetting."
				}));
				ModsConfig.data = new ModsConfig.ModsConfigData();
				flag = true;
			}
			ModsConfig.data.buildNumber = VersionControl.CurrentBuild;
			bool flag2 = File.Exists(GenFilePaths.ModsConfigFilePath);
			if (!flag2 || flag)
			{
				ModsConfig.data.activeMods.Add(ModContentPack.CoreModIdentifier);
				ModsConfig.Save();
			}
		}

		public static void DeactivateNotInstalledMods(Action<string> logCallback = null)
		{
			int i;
			for (i = ModsConfig.data.activeMods.Count - 1; i >= 0; i--)
			{
				if (!ModLister.AllInstalledMods.Any((ModMetaData m) => m.Identifier == ModsConfig.data.activeMods[i]))
				{
					if (logCallback != null)
					{
						logCallback("Deactivating " + ModsConfig.data.activeMods[i]);
					}
					ModsConfig.data.activeMods.RemoveAt(i);
				}
			}
		}

		public static void Reset()
		{
			ModsConfig.data.activeMods.Clear();
			ModsConfig.data.activeMods.Add(ModContentPack.CoreModIdentifier);
			ModsConfig.Save();
		}

		internal static void Reorder(int modIndex, int newIndex)
		{
			if (modIndex == newIndex)
			{
				return;
			}
			string item = ModsConfig.data.activeMods[modIndex];
			ModsConfig.data.activeMods.RemoveAt(modIndex);
			ModsConfig.data.activeMods.Insert(newIndex, item);
		}

		public static bool IsActive(ModMetaData mod)
		{
			return ModsConfig.data.activeMods.Contains(mod.Identifier);
		}

		public static void SetActive(ModMetaData mod, bool active)
		{
			ModsConfig.SetActive(mod.Identifier, active);
		}

		public static void SetActive(string modIdentifier, bool active)
		{
			if (active)
			{
				if (!ModsConfig.data.activeMods.Contains(modIdentifier))
				{
					ModsConfig.data.activeMods.Add(modIdentifier);
				}
			}
			else if (ModsConfig.data.activeMods.Contains(modIdentifier))
			{
				ModsConfig.data.activeMods.Remove(modIdentifier);
			}
		}

		public static void Save()
		{
			XmlSaver.SaveDataObject(ModsConfig.data, GenFilePaths.ModsConfigFilePath);
		}
	}
}
