using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ChatAppLib;
using ChatAppLib.Data;
using HugsLib;
using HugsLib.Settings;
using RimWorld;
using UnityEngine.SceneManagement;
using Verse;

namespace AlcoholV
{
    public enum ExcuteType
    {
        INSTANT,
        STACK,
        COOL
    }


    public class AcDonationBot : ModBase
    {
        public AcDonationBot()
        {
            Instance = this;
        }

        // ReSharper disable once InconsistentNaming
        public static AcDonationBot Instance;
        public static SettingHandle<bool> AdCommandEnable;
        public static SettingHandle<bool> SponEnable;

        public static bool isInitialized;

        public override string ModIdentifier => "AcDonationBot";

        public override void DefsLoaded()
        {
            // todo : 모드 언로드 구현
            if (!ModIsActive || isInitialized) return;

            Client.Start();
            PacketManager.Instance.Init();
            IncidentManager.Instance.Init();
            isInitialized = true;

            SponEnable = Settings.GetHandle("Spon", "후원봇", "", false);
            AdCommandEnable = Settings.GetHandle("ADExcute", "AD명령", "", false);

        }

        public override void Update()
        {
            PacketManager.Instance.Update();
        }

    }
}