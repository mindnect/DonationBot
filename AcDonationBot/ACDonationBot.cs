using System.Collections.Generic;
using System.Linq;
using AlcoholV.Event;
using ChatAppLib.Data;
using ChatAppLib;
using HugsLib;
using RimWorld;
using UnityEngine.SceneManagement;
using Verse;

namespace AlcoholV
{
    public enum EventExcuteType
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
            MessageManager.Init();
            Client.Start();
        }

        // Use Singleton
        public static AcDonationBot Instance { get; private set; }

        private static void OnMessage(Packet obj)
        {
        }

        #region override

        public override string ModIdentifier => "AcDonationBot";

        public override void Initialize()
        {
            //Log.MessagePacket(MethodBase.GetCurrentMethod().Name);
        }


        public override void SceneLoaded(Scene scene)
        {
            Logger.Message("SceneLoaded:" + scene.name);
        }

        public override void SettingsChanged()
        {
            Logger.Message("SettingsChanged");
        }

        public override void DefsLoaded()
        {
            var incidentList = new List<IncidentDef>();
            foreach (var current in DefDatabase<IncidentDef>.AllDefs.Select(x => x).OrderBy(d => d.defName))
            {
                var handle = Settings.GetHandle(current.defName, current.label, "", EventExcuteType.Instant, null, "");
                handle.Unsaved = true;
                handle.CustomDrawer = rect =>
                {
                    if (Widgets.ButtonText(rect, "HugsLib_setting_allNews_button".Translate()))
                        return true;
                    return false;
                };
            }
        }

        //public override void Tick(int currentTick)
        //{
        //}

        public override void Update()
        {
            //if (Current.ProgramState == ProgramState.Playing)
                MessageManager.Update();
        }

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

        #endregion
    }
}