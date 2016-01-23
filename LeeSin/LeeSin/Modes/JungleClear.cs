using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;

namespace LeeSin
{
    public static class JungleClear
    {
        private static float _lastCastTime;
        public static bool IsActive
        {
            get
            {
                return ModeManager.IsJungleClear;
            }
        }
        public static Menu Menu
        {
            get
            {
                return MenuManager.GetSubMenu("JungleClear");
            }
        }

        private static bool WillEndSoon
        {
            get
            {
                if (Util.MyHero.HasBuff2(Champion.PassiveName))
                {
                    return Util.MyHero.GetBuff2(Champion.PassiveName).EndTime - Game.Time < Util.MyHero.AttackCastDelay;
                }
                return false;
            }
        }
        public static void Execute()
        {
            if (SpellManager.SmiteIsReady)
            {
                if (Menu.GetCheckBoxValue("Smite"))
                {
                    var dragon = EntityManager.MinionsAndMonsters.GetJungleMonsters(Util.MyHero.Position, SpellManager.Q2.Range).FirstOrDefault(m => m.IsInSmiteRange() && m.IsDragon());
                    if (dragon != null)
                    {
                        if (dragon.Health <= dragon.SmiteDamage())
                        {
                            Util.MyHero.Spellbook.CastSpell(SpellManager.Smite.Slot, dragon);
                        }
                    }
                }
            }
            var minion = EntityManager.MinionsAndMonsters.GetJungleMonsters(Util.MyHero.Position, SpellManager.Q1.Range).Where(m => m.IsValidTarget()).OrderBy(m => m.MaxHealth).LastOrDefault();
            if (minion != null)
            {
                if (minion.IsDragon() && SpellManager.SmiteIsReady && SpellSlot.Q.IsReady())
                {
                    if (2.5f * SpellSlot.Q.GetSpellDamage(minion, 2) + minion.SmiteDamage() >= minion.Health && SpellSlot.Q.GetSpellDamage(minion, 2) + minion.SmiteDamage() <= minion.Health)
                    {
                        return;
                    }
                }
                if (Game.Time - _lastCastTime < 0.25f)
                {
                    return;
                }
                if (Util.MyHero.IsInAutoAttackRange(minion))
                {
                    if (WillEndSoon)
                    {
                        if (SpellSlot.Q.IsReady() && !SpellSlot.Q.IsFirstSpell() && Menu.GetCheckBoxValue("Q"))
                        {
                            Util.MyHero.Spellbook.CastSpell(SpellSlot.Q, true);
                            return;
                        }
                        if (SpellSlot.W.IsReady() && !SpellSlot.W.IsFirstSpell() && Menu.GetCheckBoxValue("W"))
                        {
                            Util.MyHero.Spellbook.CastSpell(SpellSlot.W, true);
                            return;
                        }
                        if (SpellSlot.E.IsReady() && !SpellSlot.E.IsFirstSpell() && Menu.GetCheckBoxValue("E") && Extensions.Distance(minion, Util.MyHero, true) <= Math.Pow(SpellManager.ERange, 2))
                        {
                            Util.MyHero.Spellbook.CastSpell(SpellSlot.E, true);
                            return;
                        }
                    }
                    if (Champion.PassiveStack > 0)
                    {
                        return;
                    }
                }
                if (Orbwalker.CanMove)
                {
                    if (Menu.GetCheckBoxValue("W") && minion.IsInAutoAttackRange(Util.MyHero) && Util.MyHero.HealthPercent <= 40)
                    {
                        if (SpellSlot.W.IsReady() && SpellSlot.W.IsFirstSpell())
                        {
                            Util.MyHero.Spellbook.CastSpell(SpellSlot.W, Util.MyHero, true);
                            _lastCastTime = Game.Time;
                            return;
                        }
                    }
                    if (SpellSlot.Q.IsReady() && !SpellSlot.Q.IsFirstSpell() && Menu.GetCheckBoxValue("Q"))
                    {
                        Util.MyHero.Spellbook.CastSpell(SpellSlot.Q, true);
                        _lastCastTime = Game.Time;
                        return;
                    }
                    if (SpellSlot.W.IsReady() && !SpellSlot.W.IsFirstSpell() && Menu.GetCheckBoxValue("W"))
                    {
                        Util.MyHero.Spellbook.CastSpell(SpellSlot.W, true);
                        _lastCastTime = Game.Time;
                        return;
                    }
                    if (SpellSlot.E.IsReady() && !SpellSlot.E.IsFirstSpell() && Menu.GetCheckBoxValue("E") && Extensions.Distance(minion, Util.MyHero, true) <= Math.Pow(SpellManager.ERange, 2))
                    {
                        Util.MyHero.Spellbook.CastSpell(SpellSlot.E, true);
                        _lastCastTime = Game.Time;
                        return;
                    }
                    if (SpellSlot.Q.IsReady() && SpellSlot.Q.IsFirstSpell() && Menu.GetCheckBoxValue("Q"))
                    {
                        var pred = SpellManager.Q1.GetPrediction(minion);
                        if (pred.HitChancePercent >= SpellSlot.Q.HitChancePercent())
                        {
                            Util.MyHero.Spellbook.CastSpell(SpellSlot.Q, pred.CastPosition, true);
                            _lastCastTime = Game.Time;
                            return;
                        }
                    }
                    if (SpellSlot.E.IsReady() && SpellSlot.E.IsFirstSpell() && Menu.GetCheckBoxValue("E") && Extensions.Distance(minion, Util.MyHero, true) <= Math.Pow(SpellManager.ERange, 2))
                    {
                        Util.MyHero.Spellbook.CastSpell(SpellSlot.E, true);
                        _lastCastTime = Game.Time;
                        return;
                    }
                    if (Menu.GetCheckBoxValue("W") && minion.IsInAutoAttackRange(Util.MyHero))
                    {
                        if (SpellSlot.W.IsReady() && SpellSlot.W.IsFirstSpell())
                        {
                            Util.MyHero.Spellbook.CastSpell(SpellSlot.W, Util.MyHero, true);
                            _lastCastTime = Game.Time;
                            return;
                        }
                    }
                }
            }
        }
        public static bool IsDragon(this Obj_AI_Minion minion)
        {
            return minion.IsValidTarget() && (minion.Name.ToLower().Contains("baron") || minion.Name.ToLower().Contains("dragon"));
        }
    }
}
