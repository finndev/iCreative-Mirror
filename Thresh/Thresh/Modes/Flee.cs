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
            if (Menu.GetCheckBoxValue("W"))
            {
                var ally = EntityManager.Heroes.Allies.Where(h => h.IsValidTarget(TargetSelector.Range) && !h.IsMe).OrderByDescending(h => h.CountEnemiesInside(600) * h.GetPriority() / h.HealthPercent).FirstOrDefault();
                if (ally != null && ally.CountEnemiesInside(600) > Util.MyHero.CountEnemiesInside(600))
                {
                    SpellManager.CastW(ally);
                }
            }
            if (Menu.GetCheckBoxValue("E"))
            {
                foreach (var h in EntityManager.Heroes.Enemies.Where(h => h.IsValidTarget(SpellManager.E.Range)))
                {
                    SpellManager.Push(h);
                }
            }
        }
    }
}
