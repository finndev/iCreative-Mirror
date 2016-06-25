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


namespace Draven_Me_Crazy
{
    public static class Clear
    {
        public static bool IsActive
        {
            get
            {
                return ModeManager.IsClear;
            }
        }
        public static void Execute()
        {
            if (ModeManager.IsLaneClear)
            {
                LaneClear.Execute();
            }
            if (ModeManager.IsJungleClear)
            {
                JungleClear.Execute();
            }
        }
    }
}
