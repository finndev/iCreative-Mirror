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



namespace LeeSin
{
    public static class Flee
    {
        public static Menu Menu
        {
            get
            {
                return MenuManager.GetSubMenu("Flee");
            }
        }
        public static bool IsActive
        {
            get
            {
                return ModeManager.IsFlee;
            }
        }
        public static void Execute()
        {
            if (SpellManager.CanCastW1)
            {
                var obj = Champion.GetBestObjectNearTo(Util.MousePos);
                if (obj != null && Menu.GetCheckBoxValue("W"))
                {
                    SpellManager.CastW1(obj);
                }
                else if (Menu.GetCheckBoxValue("WardJump"))
                {
                    if (WardManager.CanCastWard)
                    {
                        WardManager.CastWardTo(Util.MousePos);
                    }
                }
            }
        }
    }
}
