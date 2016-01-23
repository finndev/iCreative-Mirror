using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;


namespace Thresh
{
    public static class LastHit
    {
        public static Menu Menu
        {
            get
            {
                return MenuManager.GetSubMenu("LastHit");
            }
        }
        public static bool IsActive
        {
            get
            {
                return ModeManager.IsLastHit;
            }
        }
        public static void Execute()
        {

        }
    }
}
