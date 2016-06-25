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
            if (SpellSlot.W.IsReady() && Menu.GetCheckBoxValue("W"))
            {
                Util.MyHero.Spellbook.CastSpell(SpellSlot.W);
            }
            if (SpellSlot.E.IsReady() && Menu.GetCheckBoxValue("E"))
            {
                var target = EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(SpellManager.E.Range)).OrderBy(d => Extensions.Distance(Util.MyHero, d, true)).FirstOrDefault();
                if (target.IsValidTarget())
                {
                    SpellManager.CastE(target);
                }
            }
        }
    }
}
