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
    public static class AutoSmite
    {
        public static Menu Menu
        {
            get
            {
                return MenuManager.GetSubMenu("Smite");
            }
        }
        private static string StealText = "";
        public static void Init()
        {
            if (SpellManager.Smite != null)
            {
                Game.OnTick += Game_OnTick;
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (SpellManager.SmiteIsReady)
            {
                if (Menu.GetCheckBoxValue("DragonSteal"))
                {
                    var minion = EntityManager.MinionsAndMonsters.Monsters.FirstOrDefault(m => m.IsInSmiteRange() && m.IsDragon());
                    if (minion != null)
                    {
                        if (minion.Health <= minion.SmiteDamage())
                        {
                            Util.MyHero.Spellbook.CastSpell(SpellManager.Smite.Slot, minion);
                        }
                    }
                }
                /*
                if (Menu.GetCheckBoxValue("KillSteal"))
                {
                    var name = SpellManager.Smite.Slot.GetSpellDataInst().SData.Name.ToLower();
                    if (name.Contains("smiteduel") || name.Contains("smiteplayerganker"))
                    {
                        foreach (AIHeroClient enemy in EntityManager.Heroes.Enemies.Where(m => m.IsValidTarget(SpellManager.Smite.Range)))
                        {
                            if (enemy.Health <= Util.myHero.GetSummonerSpellDamage(enemy, DamageLibrary.SummonerSpells.Smite))
                            {
                                Util.myHero.Spellbook.CastSpell(SpellManager.Smite.Slot, enemy);
                            }
                        }
                    }
                }*/
            }
        }
    }
}
