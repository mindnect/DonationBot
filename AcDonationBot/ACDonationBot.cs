using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AlcoholV.Event;
using Comm.Client;
using HugsLib;
using HugsLib.Settings;
using HugsLib.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Verse;

namespace AlcoholV
{
    public class AcDonationBot : ModBase
    {
        public AcDonationBot()
        {
            SocketClient.OnMessage += MessageManager.OnMessage;
            SocketClient.OnClose += OnClientLog;
            SocketClient.OnError += OnClientLog;
            SocketClient.OnOpen += OnClientLog;
            SocketClient.OnRetry += OnClientLog;
            SocketClient.Connect();
        }

        public override string ModIdentifier => "AcDonationBot";

        public override void Initialize()
        {
            //Log.Message(MethodBase.GetCurrentMethod().Name);
        }



        public override void SceneLoaded(Scene scene)
        {
            Logger.Message("SceneLoaded:" + scene.name);
        }

        public override void SettingsChanged()
        {
            Logger.Message("SettingsChanged");
        }

        private enum HandleEnum
        {
            DefaultValue,
            ValueOne,
            ValueTwo
        }

        public override void DefsLoaded()
        {
            Logger.Message("DefsLoaded");
            //var str = Settings.GetHandle("str", "String value", "", "value");
            //var spinner = Settings.GetHandle("intSpinner", "Spinner", "desc", 5, Validators.IntRangeValidator(0, 30));
            //spinner.SpinnerIncrement = 2;
            //var enumHandle = Settings.GetHandle("enumThing", "Enum setting", "", HandleEnum.DefaultValue, null, "test_enumSetting_");
            //var toggle = Settings.GetHandle("toggle", "Toggle setting", "Toggle setting", false);


            var custom = Settings.GetHandle<CustomHandleType>("customType", null, null);
            if (custom.Value == null) custom.Value = new CustomHandleType { nums = new List<int>() };
            if (custom.Value.nums.Count < 10) custom.Value.nums.Add(Rand.Range(1, 100));
            HugsLibController.SettingsManager.SaveChanges();
            Logger.Trace("Custom setting values: " + custom.Value.nums.Join(","));

            TestCustomTypeSetting();
           
        }

        private void TestCustomTypeSetting()
        {


        }

        private class CustomHandleType : SettingHandleConvertible
        {
            public List<int> nums = new List<int>();

            public override void FromString(string settingValue)
            {
                nums = settingValue.Length > 0 ? settingValue.Split('|').Select(int.Parse).ToList() : new List<int>();
            }

            public override string ToString()
            {
                return nums != null ? nums.Join("|") : "";
            }
        }



        //public override void Tick(int currentTick)
        //{
        //}

        //public override void Update()
        //{
        //}

        //public override void FixedUpdate()
        //{
        //}

        //public override void OnGUI()
        //{
        //}

        //public override void WorldLoaded()
        //{

        //}

        //public override void MapComponentsInitializing(Map map)
        //{

        //}

        //public override void MapLoaded(Map map)
        //{

        //}

        //public override void SceneLoaded(Scene scene)
        //{

        //}

        //public override void SettingsChanged()
        //{

        //}

        //public override void DefsLoaded()
        //{

        //}

        private static void OnClientLog(string s)
        {
            Log.Message(s);
        }
    }
}