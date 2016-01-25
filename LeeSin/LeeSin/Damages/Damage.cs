using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;

namespace LeeSin
{
    public static class Damage
    {
        public static float RefreshTime = 0.4f;
        private static readonly Dictionary<int, DamageResult> PredictedDamage = new Dictionary<int, DamageResult>();

        private static float Overkill
        {
            get { return (100f + MenuManager.MiscMenu.GetSliderValue("Overkill")) / 100f; }
        }

        public static float GetSpellDamage(this SpellSlot slot, Obj_AI_Base target, int stage = 1)
        {
            if (target.IsValidTarget())
            {
                if (slot == SpellSlot.Q)
                {
                    if (stage == 2)
                    {
                        if (slot.IsFirstSpell())
                        {
                            return
                                Util.MyHero.CalculateDamageOnUnit(target, DamageType.Physical,
                                    30f * slot.GetSpellDataInst().Level + 30 +
                                    0.9f * Util.MyHero.TotalAttackDamage) +
                                Util.MyHero.CalculateDamageOnUnit(target, DamageType.Physical,
                                    30f * slot.GetSpellDataInst().Level + 30 +
                                    0.9f * Util.MyHero.TotalAttackDamage + 8f * (target.MaxHealth - target.Health) / 100);
                        }
                        return Util.MyHero.CalculateDamageOnUnit(target, DamageType.Physical,
                            30f * slot.GetSpellDataInst().Level + 30 + 0.9f * Util.MyHero.FlatPhysicalReduction +
                            8f * (target.MaxHealth - target.Health) / 100);
                    }
                    if (slot.IsFirstSpell())
                    {
                        return Util.MyHero.CalculateDamageOnUnit(target, DamageType.Physical,
                            30f * slot.GetSpellDataInst().Level + 30 + 0.9f * Util.MyHero.FlatPhysicalReduction);
                    }
                    return Util.MyHero.CalculateDamageOnUnit(target, DamageType.Physical,
                        30f * slot.GetSpellDataInst().Level + 30 + 0.9f * Util.MyHero.FlatPhysicalReduction +
                        8f * (target.MaxHealth - target.Health) / 100);
                }
                if (slot == SpellSlot.W)
                {
                    return Util.MyHero.GetAutoAttackDamage(target, true) * 2;
                }
                if (slot == SpellSlot.E)
                {
                    if (stage == 2)
                    {
                        if (slot.IsFirstSpell())
                        {
                            return
                                Util.MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                                    35f * slot.GetSpellDataInst().Level + 25 + 1f * Util.MyHero.FlatPhysicalReduction) +
                                Util.MyHero.GetAutoAttackDamage(target, true);
                        }
                        return Util.MyHero.GetAutoAttackDamage(target, true);
                    }
                    if (slot.IsFirstSpell())
                    {
                        return Util.MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            35f * slot.GetSpellDataInst().Level + 25 + 1f * Util.MyHero.FlatPhysicalReduction);
                    }
                    return Util.MyHero.GetAutoAttackDamage(target, true);
                }
                if (slot == SpellSlot.R)
                {
                    return Util.MyHero.CalculateDamageOnUnit(target, DamageType.Physical,
                       200f * slot.GetSpellDataInst().Level + 2.0f * Util.MyHero.FlatPhysicalReduction);
                }
            }
            return Util.MyHero.GetSpellDamage(target, slot);
        }

        public static DamageResult GetComboDamage(this Obj_AI_Base target, bool q, bool w, bool e, bool r)
        {
            var ComboDamage = 0f;
            var ManaWasted = 0f;
            if (target.IsValidTarget())
            {
                if (q)
                {
                    ComboDamage += SpellSlot.Q.GetSpellDamage(target, 2);
                    ManaWasted += SpellSlot.Q.GetSpellDataInst().SData.Mana;
                }
                if (w)
                {
                    ComboDamage += SpellSlot.W.GetSpellDamage(target);
                    ManaWasted += SpellSlot.W.GetSpellDataInst().SData.Mana;
                }
                if (e)
                {
                    ComboDamage += SpellSlot.E.GetSpellDamage(target, 2);
                    ManaWasted += SpellSlot.E.GetSpellDataInst().SData.Mana;
                }
                if (r)
                {
                    ComboDamage += SpellSlot.R.GetSpellDamage(target);
                    ManaWasted += SpellSlot.R.GetSpellDataInst().SData.Mana;
                }
                if (SpellManager.IgniteIsReady)
                {
                    ComboDamage += Util.MyHero.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite);
                }
                if (SpellManager.SmiteIsReady)
                {
                    ComboDamage += Util.MyHero.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Smite);
                }
                ComboDamage += Util.MyHero.GetAutoAttackDamage(target, true);
            }
            ComboDamage = ComboDamage * Overkill;
            return new DamageResult(target, ComboDamage, ManaWasted);
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
                damageI2 = target.GetComboDamage(SpellSlot.Q.IsReady(), SpellSlot.W.IsReady(), SpellSlot.E.IsReady(),
                    SpellSlot.R.IsReady());
                PredictedDamage[target.NetworkId] = new DamageResult(target, damageI2.Damage, damageI2.Mana, false,
                    false, false, false, Game.Time - (Game.Ping * 2f) / 2000f);
                return target.GetBestCombo();
            }
            return new DamageResult(target, 0, 0, false, false, false, false, 0);
        }
    }
}