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
                    if (Menu.GetCheckBoxValue("E") && result.CanKillWith(SpellSlot.E)) { SpellManager.CastE(enemy); }
                    if (Menu.GetCheckBoxValue("R") && SpellSlot.R.GetSpellDamage(enemy) >= enemy.TotalShieldHealth()) { SpellManager.CastR(enemy); }
                    var custom = (Menu.GetCheckBoxValue("Q") && result.CanKillWith(SpellSlot.Q)) || (Menu.GetCheckBoxValue("E") && result.CanKillWith(SpellSlot.E)) || (Menu.GetCheckBoxValue("R") && result.CanKillWith(SpellSlot.R));
                    if (Menu.GetCheckBoxValue("W") && custom)
                    {
                        if (Menu.GetCheckBoxValue("Ward"))
                        {
                            Champion.GapCloseWithWard(enemy);
                        }
                        else
                        {
                            Champion.GapCloseWithoutWard(enemy);
                        }
                    }
                }
                if (Menu.GetCheckBoxValue("Ignite") && SpellManager.IgniteIsReady && Util.MyHero.GetSummonerSpellDamage(enemy, DamageLibrary.SummonerSpells.Ignite) >= enemy.Health)
                {
                    SpellManager.Ignite.Cast(enemy);
                }
                if (Menu.GetCheckBoxValue("Smite") && SpellManager.CanUseSmiteOnHeroes && enemy.IsInSmiteRange() && Util.MyHero.GetSummonerSpellDamage(enemy, DamageLibrary.SummonerSpells.Smite) >= enemy.Health)
                {
                    Util.MyHero.Spellbook.CastSpell(SpellManager.Smite.Slot, enemy);
                }
            }
        }
    }
}
