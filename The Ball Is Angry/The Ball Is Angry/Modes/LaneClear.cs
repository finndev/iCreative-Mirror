using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;

namespace The_Ball_Is_Angry
{
    public static class LaneClear
    {
        public static Menu Menu
        {
            get { return MenuManager.GetSubMenu("LaneClear"); }
        }

        public static bool IsActive
        {
            get { return ModeManager.IsLaneClear; }
        }
        
        public static void Execute()
        {
        }
    }
}