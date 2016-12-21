using System.Collections.Generic;
using Data.Tabs;
using UnityEngine;
using Verse;

namespace Data.MainTabWindow
{
    public class Donation : RimWorld.MainTabWindow
    {

        private readonly IWindowTab _eventWindowTab = new WindowTab_Event();
        private readonly IWindowTab _settingWindowTab = new WindowTab_Setting();
        private readonly IWindowTab _logWindowTab = new WindowTab_Log();

        public IWindowTab currentTab;

        public Donation()
        {
            currentTab = _eventWindowTab;
        }

        public override void DoWindowContents(Rect rect)
        {
            base.DoWindowContents(rect);

            var rect2 = rect;
            rect2.yMin += 40;
            var list = new List<TabRecord>
            {
                new TabRecord("이벤트", delegate { currentTab = _eventWindowTab; }, currentTab == _eventWindowTab),
                new TabRecord("설정", delegate { currentTab = _settingWindowTab; }, currentTab == _settingWindowTab),
                new TabRecord("후원", delegate { currentTab = _logWindowTab; }, currentTab == _logWindowTab)
            };

            TabDrawer.DrawTabs(rect2, list);
            currentTab.DoTabContents(rect2);
        }
    }
}