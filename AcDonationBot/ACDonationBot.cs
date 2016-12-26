using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using AlcoholV.Manager;
using ChatAppLib;
using ChatAppLib.Data;
using HugsLib;
using HugsLib.Settings;
using RimWorld;
using UnityEngine.SceneManagement;
using Verse;

namespace AlcoholV
{
    


    public class AcDonationBot : ModBase
    {
        public AcDonationBot()
        {
            Instance = this;
        }

        // ReSharper disable once InconsistentNaming
        public static AcDonationBot Instance;
        public static SettingHandle<bool> adCommandSetting;
        public static SettingHandle<bool> sponSetting;

        public static bool isInitialized;

        public override string ModIdentifier => "AcDonationBot";

        public override void DefsLoaded()
        {
            if (!ModIsActive || isInitialized) return;

            Client.Start();

            PacketManager.Init();
            LogManager.Init();
            DataManager.Init();

            isInitialized = true;
            sponSetting = Settings.GetHandle("Spon", "후원봇", "", false);
            adCommandSetting = Settings.GetHandle("ADExcute", "AD명령", "", false);

        }

        public override void Update()
        {
            LogManager.Update();
        }

    }
}