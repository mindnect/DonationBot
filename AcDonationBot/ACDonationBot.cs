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
        Instant,
        Stack,
        Cool
    }


    public class AcDonationBot : ModBase
    {
        public AcDonationBot()
        {
            Instance = this;
        }

        // ReSharper disable once InconsistentNaming
        public static AcDonationBot Instance;
        public SettingHandle<bool> adExcuteHandle;
        public SettingHandle<bool> donationHandle;

        public static bool isInitialized;

        public override string ModIdentifier => "AcDonationBot";

        public override void DefsLoaded()
        {
            // todo : 모드 언로드 구현
            if (!ModIsActive || isInitialized) return;

            Client.Start();
            MessageManager.Instance.Init();
            DataManager.Instance.Init();
            isInitialized = true;

            donationHandle = Settings.GetHandle("Spon", "후원봇", "", false);
            adExcuteHandle = Settings.GetHandle("ADExcute", "AD명령", "", false);

        }

        public override void Update()
        {
            MessageManager.Instance.Update();
        }

    }
}