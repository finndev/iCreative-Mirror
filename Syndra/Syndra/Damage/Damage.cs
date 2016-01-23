using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace Syndra
{
    public static class Damage
    {
        public static float RefreshTime = 0.4f;
        private static readonly Dictionary<int, DamageResult> PredictedDamage = new Dictionary<int, DamageResult>();

        public static float Overkill
        {
            get { return (100f + MenuManager.MiscMenu.GetSliderValue("Overkill")) / 100f; }
        }

        public static float GetSpellDamage(this SpellSlot slot, Obj_AI_Base target)
        {
            if (target.IsValidTarget())
            {
                switch (slot)
                {
                    case SpellSlot.Q:
                        if (target is AIHeroClient && slot.GetSpellDataInst().Level == 5)
                        {
                            return
                                Util.MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                                    45f * slot.GetSpellDataInst().Level + 5 + 0.6f * Util.MyHero.TotalMagicalDamage) *
                                1.15f;
                        }
                        return Util.MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            45f * slot.GetSpellDataInst().Level + 5 + 0.6f * Util.MyHero.TotalMagicalDamage);
                    case SpellSlot.W:
                        return Util.MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            40f * slot.GetSpellDataInst().Level + 40 + 0.7f * Util.MyHero.TotalMagicalDamage);
                    case SpellSlot.E:
                        return Util.MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            45f * slot.GetSpellDataInst().Level + 25 + 0.4f * Util.MyHero.TotalMagicalDamage);
                    case SpellSlot.R:
                        return (3 + BallManager.Balls.Count(b=> b.IsIdle)) *
                               Util.MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                                   45f * slot.GetSpellDataInst().Level + 45f + 0.2f * Util.MyHero.TotalMagicalDamage);
                }
            }
            return Util.MyHero.GetSpellDamage(target, slot);
        }

        public static DamageResult GetComboDamage(this Obj_AI_Base target, bool q, bool w, bool e, bool r)
        {
            var comboDamage = 0f;
            var manaWasted = 0f;
            if (target.IsValidTarget())
            {
                if (q)
                {
                    comboDamage += SpellSlot.Q.GetSpellDamage(target);
                    manaWasted += SpellSlot.Q.Mana();
                }
                if (w)
                {
                    comboDamage += SpellSlot.W.GetSpellDamage(target);
                    manaWasted += SpellSlot.W.Mana();
                }
                if (e)
                {
                    comboDamage += SpellSlot.E.GetSpellDamage(target);
                    manaWasted += SpellSlot.E.Mana();
                }
                if (r)
                {
                    comboDamage += SpellSlot.R.GetSpellDamage(target);
                    manaWasted += SpellSlot.R.Mana();
                }
                if (SpellManager.IgniteIsReady)
                {
                    comboDamage += Util.MyHero.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite);
                }
                if (SpellManager.SmiteIsReady)
                {
                    comboDamage += Util.MyHero.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Smite);
                }
                comboDamage += Util.MyHero.GetAutoAttackDamage(target, true);
            }
            comboDamage = comboDamage * Overkill;
            return new DamageResult(target, comboDamage, manaWasted);
        }

        public static DamageResult GetBestComboR(this Obj_AI_Base target)
        {
            if (target.IsValidTarget())
            {
                float cd = Combo.Menu.GetSliderValue("Cooldown");
                var qcd = SpellSlot.Q.GetSpellDataInst().Cooldown;
                var wcd = SpellSlot.W.GetSpellDataInst().Cooldown;
                var ecd = SpellSlot.E.GetSpellDataInst().Cooldown;
                var q1 = (SpellSlot.Q.IsReady() || qcd < cd);
                var w1 = (SpellSlot.W.IsReady() || wcd < cd);
                var e1 = (SpellSlot.E.IsReady() || ecd < cd);
                var qArray = q1 ? new[] { false, true } : new[] { false };
                var wArray = w1 ? new[] { false, true } : new[] { false };
                var eArray = e1 ? new[] { false, true } : new[] { false };
                float bestdmg = -1;
                float bestmana = 0;
                bool[] best = { false, false, false };
                if (q1 || w1 || e1)
                {
                    foreach (var qBool in qArray)
                    {
                        foreach (var wBool in wArray)
                        {
                            foreach (var eBool in eArray)
                            {
                                var damageI2 = target.GetComboDamage(qBool, wBool, eBool, false);
                                var d = damageI2.Damage;
                                var m = damageI2.Mana;
                                if (Util.MyHero.Mana >= m && d >= target.TotalShieldHealth())
                                {
                                    if (d < bestdmg || Math.Abs(bestdmg - (-1f)) < float.Epsilon)
                                    {
                                        bestdmg = d;
                                        bestmana = m;
                                        best = new[] { q1, w1, e1, false };
                                    }
                                }
                            }
                        }
                    }
                }
                return new DamageResult(target, bestdmg, bestmana, best[0], best[1], best[2], false, 0);
            }
            return new DamageResult(target, 0, 0, false, false, false, false, 0);
        }

        public static DamageResult GetBestCombo(this Obj_AI_Base target)
        {
            var q = SpellSlot.Q.IsReady() ? new[] { false, true } : new[] { false };
            var w = SpellSlot.W.IsReady() ? new[] { false, true } : new[] { false };
            var e = SpellSlot.E.IsReady() ? new[] { false, true } : new[] { false };
            var r = SpellSlot.R.IsReady() ? new[] { false, true } : new[] { false };
            if (target.IsValidTarget())
            {
                DamageResult damageI2;
                if (PredictedDamage.ContainsKey(target.NetworkId))
                {
                    var damageI = PredictedDamage[target.NetworkId];
                    if (Game.Time - damageI.Time <= RefreshTime)
                    {
                        return PredictedDamage[target.NetworkId];
                    }
                    bool[] best =
                    {
                        SpellSlot.Q.IsReady(),
                        SpellSlot.W.IsReady(),
                        SpellSlot.E.IsReady(),
                        SpellSlot.R.IsReady()
                    };
                    var bestdmg = 0f;
                    var bestmana = 0f;
                    foreach (var r1 in r)
                    {
                        foreach (var q1 in q)
                        {
                            foreach (var w1 in w)
                            {
                                foreach (var e1 in e)
                                {
                                    damageI2 = target.GetComboDamage(q1, w1, e1, r1);
                                    var d = damageI2.Damage;
                                    var m = damageI2.Mana;
                                    if (Util.MyHero.Mana >= m)
                                    {
                                        if (bestdmg >= target.TotalShieldHealth())
                                        {
                                            if (d >= target.TotalShieldHealth() && (d < bestdmg || m < bestmana))
                                            {
                                                bestdmg = d;
                                                bestmana = m;
                                                best = new[] { q1, w1, e1, r1 };
                                            }
                                        }
                                        else
                                        {
                                            if (d >= bestdmg)
                                            {
                                                bestdmg = d;
                                                bestmana = m;
                                                best = new[] { q1, w1, e1, r1 };
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    PredictedDamage[target.NetworkId] = new DamageResult(target, bestdmg, bestmana, best[0], best[1],
                        best[2], best[3], Game.Time);
                    return PredictedDamage[target.NetworkId];
                }
                damageI2 = target.GetComboDamage(SpellSlot.Q.IsReady(), SpellSlot.W.IsReady(), SpellSlot.E.IsReady(), SpellSlot.R.IsReady());
                PredictedDamage[target.NetworkId] = new DamageResult(target, damageI2.Damage, damageI2.Mana, false,
                    false, false, false, Game.Time - (Game.Ping * 2f) / 2000f);
                return target.GetBestCombo();
            }
            return new DamageResult(target, 0, 0, false, false, false, false, 0);
        }
    }
}