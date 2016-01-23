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


namespace Syndra
{
    public static class LaneClear
    {
        public static Menu Menu
        {
            get
            {
                return MenuManager.GetSubMenu("LaneClear");
            }
        }
        public static bool IsActive
        {
            get
            {
                return ModeManager.IsLaneClear;
            }
        }
        public static void Execute()
        {
            if (Util.MyHero.ManaPercent >= Menu.GetSliderValue("Mana"))
            {
                if (Menu.GetCheckBoxValue("Q2")) { SpellManager.Q.LastHit(); }
                SpellManager.Q.LaneClear(Menu.GetSliderValue("Q"));
                var count = SpellManager.IsW2 ? (Menu.GetSliderValue("W") - 1) : Menu.GetSliderValue("W");
                SpellManager.CastW(SpellManager.W.LaneClear(count, false));
            }
            else if (SpellManager.IsW2)
            {
                var count = SpellManager.IsW2 ? (Menu.GetSliderValue("W") - 1) : Menu.GetSliderValue("W");
                SpellManager.CastW(SpellManager.W.LaneClear(count, false));
            }
        }
    }
}
