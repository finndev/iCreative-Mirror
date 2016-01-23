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
    public static class Combo
    {
        public static Menu Menu
        {
            get
            {
                return MenuManager.GetSubMenu("Combo");
            }
        }
        public static bool IsNormalCombo
        {
            get
            {
                return Menu.GetSliderValue("Mode") == 0;
            }
        }
        public static bool IsStarCombo
        {
            get
            {
                return Menu.GetSliderValue("Mode") == 1;
            }
        }
        public static bool IsGankCombo
        {
            get
            {
                return Menu.GetSliderValue("Mode") == 2;
            }
        }
        public static bool IsActive
        {
            get
            {
                return ModeManager.IsCombo;
            }
        }
        public static void Execute()
        {
            var target = TargetSelector.Target;
            if (target.IsValidTarget())
            {
                if (Menu.GetCheckBoxValue("Items")) { ItemManager.UseOffensiveItems(target); }
                if (Menu.GetCheckBoxValue("Smite") && SpellManager.CanUseSmiteOnHeroes) { Util.MyHero.Spellbook.CastSpell(SpellManager.Smite.Slot, target); }
                switch (Menu.GetSliderValue("Mode"))
                {
                    case 0:
                        NormalCombo.Execute();
                        break;
                    case 1:
                        StarCombo.Execute();
                        break;
                    case 2:
                        GankCombo.Execute();
                        break;
                }
            }
        }

    }
}
