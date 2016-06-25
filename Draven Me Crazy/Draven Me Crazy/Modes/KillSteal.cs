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
    public static class KillSteal
    {
        public static Menu Menu
        {
            get
            {
                return MenuManager.GetSubMenu("KillSteal");
            }
        }
        public static void Execute()
        {
            foreach (AIHeroClient enemy in EntityManager.Heroes.Enemies.Where(m => m.IsValidTarget(TargetSelector.Range) && m.HealthPercent < 40))
            {
                var result = enemy.GetBestCombo();
                if (result.IsKillable)
                {
                    if (Menu.GetCheckBoxValue("Q") && result.CanKillWith(SpellSlot.Q)) { SpellManager.CastQ(enemy); }
                    if (Menu.GetCheckBoxValue("W") && result.CanKillWith(SpellSlot.W)) { SpellManager.CastW(enemy); }
                    if (Menu.GetCheckBoxValue("E") && result.CanKillWith(SpellSlot.E)) { SpellManager.CastE(enemy); }
                    if (Menu.GetCheckBoxValue("R") && result.CanKillWith(SpellSlot.R)) { SpellManager.CastR(enemy); }
                }
                if (Menu.GetCheckBoxValue("Ignite") && SpellManager.IgniteIsReady && Util.MyHero.GetSummonerSpellDamage(enemy, DamageLibrary.SummonerSpells.Ignite) >= enemy.Health)
                {
                    SpellManager.Ignite.Cast(enemy);
                }
            }
        }
    }
}
