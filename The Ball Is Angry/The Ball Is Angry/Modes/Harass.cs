using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;

namespace The_Ball_Is_Angry
{
    public static class Harass
    {
        public static Menu Menu
        {
            get { return MenuManager.GetSubMenu("Harass"); }
        }

        public static bool IsActive
        {
            get { return ModeManager.IsHarass; }
        }
        public static void Execute()
        {

        }
    }
}
