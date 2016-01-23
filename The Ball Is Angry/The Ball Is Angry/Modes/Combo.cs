using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;

namespace The_Ball_Is_Angry
{
    public static class Combo
    {
        public static Menu Menu
        {
            get { return MenuManager.GetSubMenu("Combo"); }
        }
        public static bool IsActive
        {
            get { return ModeManager.IsCombo; }
        }
        
        public static bool CanUseQ;
        public static bool CanUseW;
        public static bool CanUseE;

        public static void Execute()
        {
        }

    }
}
