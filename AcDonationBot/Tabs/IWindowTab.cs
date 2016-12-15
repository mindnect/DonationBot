using UnityEngine;

namespace ChatApp.Tabs
{
    public interface IWindowTab
    {
        void DoTabContents(Rect rect);
    }
}