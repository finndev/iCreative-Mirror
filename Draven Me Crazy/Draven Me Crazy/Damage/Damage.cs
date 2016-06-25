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
    public static class Damage
    {

        public static float RefreshTime = 0.4f;
        private static readonly Dictionary<int, DamageResult> PredictedDamage = new Dictionary<int, DamageResult>() { };
        public static float Overkill
        {
            get
            {
                return ((100f + MenuManager.MiscMenu.GetSliderValue("Overkill")) / 100f);
            }
        }
        public static float GetSpellDamage(this SpellSlot slot, Obj_AI_Base target, int stage = 1)
        {
            if (target.IsValidTarget())
            {
                switch (slot)
                {
                    case SpellSlot.Q:
                        return AxesManager.AxesCount * (Util.MyHero.GetSpellDamage(target, slot) + Util.MyHero.GetAutoAttackDamage(target, true));
                    case SpellSlot.W:
                        return Util.MyHero.GetAutoAttackDamage(target, true) * 2;
                    case SpellSlot.E:
                        return Util.MyHero.CalculateDamageOnUnit(target, DamageType.Physical, (float)35 * slot.GetSpellDataInst().Level + 35 + 0.5f * Util.MyHero.TotalAttackDamage);
                    case SpellSlot.R:
                        return 2 * Util.MyHero.CalculateDamageOnUnit(target, DamageType.Physical, (float)100 * slot.GetSpellDataInst().Level + 75 + 1.1f * Util.MyHero.TotalAttackDamage);
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
                ComboDamage += 1.5f * Util.MyHero.GetAutoAttackDamage(target, true);
            }
            ComboDamage = ComboDamage * Overkill;
            return new DamageResult(target, ComboDamage, ManaWasted);
        }

        public static DamageResult GetBestCombo(this Obj_AI_Base target)
        {
            var q = SpellSlot.Q.IsReady() ? new bool[] { false, true } : new bool[] { false };
            var w = SpellSlot.W.IsReady() ? new bool[] { false, true } : new bool[] { false };
            var e = SpellSlot.E.IsReady() ? new bool[] { false, true } : new bool[] { false };
            var r = SpellSlot.R.IsReady() ? new bool[] { false, true } : new bool[] { false };
            if (target.IsValidTarget())
            {
                if (PredictedDamage.ContainsKey(target.NetworkId))
                {
                    var damageI = PredictedDamage[target.NetworkId];
                    if (Game.Time - damageI.Time <= RefreshTime)
                    {
                        return PredictedDamage[target.NetworkId];
                    }
                    else
                    {
                        bool[] best = new bool[] {
                                    SpellSlot.Q.IsReady(),
                                    SpellSlot.W.IsReady(),
                                    SpellSlot.E.IsReady(),
                                    SpellSlot.R.IsReady()
                                };
                        var bestdmg = 0f;
                        var bestmana = 0f;
                        foreach (bool r1 in r)
                        {
                            foreach (bool q1 in q)
                            {
                                foreach (bool w1 in w)
                                {
                                    foreach (bool e1 in e)
                                    {
                                        DamageResult damageI2 = target.GetComboDamage(q1, w1, e1, r1);
                                        float d = damageI2.Damage;
                                        float m = damageI2.Mana;
                                        if (Util.MyHero.Mana >= m)
                                        {
                                            if (bestdmg >= target.TotalShieldHealth())
                                            {
                                                if (d >= target.TotalShieldHealth() && (d < bestdmg || m < bestmana))
                                                {
                                                    bestdmg = d;
                                                    bestmana = m;
                                                    best = new bool[] { q1, w1, e1, r1 };
                                                }
                                            }
                                            else
                                            {
                                                if (d >= bestdmg)
                                                {
                                                    bestdmg = d;
                                                    bestmana = m;
                                                    best = new bool[] { q1, w1, e1, r1 };
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        PredictedDamage[target.NetworkId] = new DamageResult(target, bestdmg, bestmana, best[0], best[1], best[2], best[3], Game.Time);
                        return PredictedDamage[target.NetworkId];
                    }
                }
                else
                {
                    var damageI2 = target.GetComboDamage(SpellSlot.Q.IsReady(), SpellSlot.W.IsReady(), SpellSlot.E.IsReady(), SpellSlot.R.IsReady());
                    PredictedDamage[target.NetworkId] = new DamageResult(target, damageI2.Damage, damageI2.Mana, false, false, false, false, Game.Time - (Game.Ping * 2) / 2000);
                    return target.GetBestCombo();
                }
            }
            return new DamageResult(target, 0, 0, false, false, false, false, 0);
        }
    }
}
