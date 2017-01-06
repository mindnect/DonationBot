using Ac.DonationBot.Manager;
using AlcoholV.Manager;
using ChatApp.Client;
using HugsLib;
using HugsLib.Settings;

namespace AlcoholV
{
    public class AcDonationBot : ModBase
    {
        // ReSharper disable once InconsistentNaming
        public static AcDonationBot Instance;
        public static SettingHandle<bool> adAllowedCommand;
        public static SettingHandle<bool> sponEventEnabled;

        public static bool isInitialized;

        public AcDonationBot()
        {
            Instance = this;
        }

        public override string ModIdentifier => "AcDonationBot";

        public override void DefsLoaded()
        {
            if (!ModIsActive || isInitialized) return;

            PacketManager.Init(); // 클라이언트 보다 먼저 초기화 되어야 한다.
            DataManager.Init();

            Client.Start();

            isInitialized = true;

            sponEventEnabled = Settings.GetHandle("Spon", "후원봇", "", false);
            adAllowedCommand = Settings.GetHandle("ADExcute", "AD명령", "", false);
        }

        public override void Update()
        {
            PacketManager.Update();
        }
    }
}