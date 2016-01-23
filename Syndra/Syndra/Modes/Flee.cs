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
            if (Menu.GetCheckBoxValue("E"))
            {
                var target = EloBuddy.SDK.TargetSelector.GetTarget(500, DamageType.Magical, Util.MousePos);
                if (target.IsValidTarget())
                {
                    SpellManager.CastE(target);
                    SpellManager.CastQE(target);
                    SpellManager.CastWE(target);
                }
            }
        }
    }
}
