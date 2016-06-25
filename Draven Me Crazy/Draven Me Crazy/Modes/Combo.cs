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
    public static class Combo
    {
        public static Menu Menu
        {
            get
            {
                return MenuManager.GetSubMenu("Combo");
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
                var damageI = target.GetBestCombo();
                SpellManager.CastQ(target, Menu.GetSliderValue("Q"));
                if (Menu.GetCheckBoxValue("Items"))
                {
                    ItemManager.UseOffensiveItems(target);
                }
                if (Menu.GetCheckBoxValue("R") && damageI.IsKillable && damageI.R) { SpellManager.CastR(target); }
                if (Menu.GetCheckBoxValue("E") && damageI.Damage * Damage.Overkill.Pow() >= target.Health) { SpellManager.CastE(target); }
                if (Menu.GetCheckBoxValue("W")) { SpellManager.CastW(target); }
            }
        }

    }
}
