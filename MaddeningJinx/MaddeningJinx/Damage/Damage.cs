using System;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace MaddeningJinx
{
    public static class Damage
    {
        public const float RefreshTime = 0.4f;
        private static readonly Dictionary<int, DamageResult> PredictedDamage = new Dictionary<int, DamageResult>();

        public static float Overkill
        {
            get { return 1.1f; }
        }

        public static float GetSpellDamage(this SpellSlot slot, Obj_AI_Base target, int typeOfDamage = 1)
        {
            if (target != null)
            {
                var level = Util.MyHero.Spellbook.GetSpell(slot).Level;
                switch (slot)
                {
                    case SpellSlot.Q:
                        return Util.MyHero.CalculateDamageOnUnit(target, DamageType.Physical, 1.1f * Util.MyHero.TotalAttackDamage);
                    case SpellSlot.W:
                        return Util.MyHero.CalculateDamageOnUnit(target, DamageType.Physical, 10f + 50f * level + 1.4f * Util.MyHero.TotalAttackDamage);
                    case SpellSlot.E:
                        return Util.MyHero.CalculateDamageOnUnit(target, DamageType.Magical, 25f + 55f * level + 1f * Util.MyHero.TotalMagicalDamage);
                    case SpellSlot.R:
                        var percentMod = Math.Min((int)(Vector3.Distance(target.ServerPosition, Util.MyHero.ServerPosition) / 100f) * 6f + 10f, 100f) / 100f;
                        float rawDamage;
                        if (typeOfDamage == 2)
                        {
                            rawDamage = 0.8f * percentMod * (200f + 50f * level + Util.MyHero.TotalAttackDamage + (0.25f + 0.05f * level) * (target.MaxHealth - target.Health));
                        }
                        else
                        {
                            rawDamage = percentMod * (200f + 50f * level + Util.MyHero.TotalAttackDamage + (0.25f + 0.05f * level) * (target.MaxHealth - target.Health));
                        }
                        return Util.MyHero.CalculateDamageOnUnit(target, DamageType.Physical, rawDamage);
                }
            }
            return Util.MyHero.GetSpellDamage(target, slot);
        }

        public static DamageResult GetComboDamage(this Obj_AI_Base target, bool q, bool w, bool e, bool r)
        {
            var comboDamage = 0f;
            var manaWasted = 0f;
            if (target != null)
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
                if (Util.IgniteIsReady)
                {
                    comboDamage += Util.MyHero.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite);
                }
                if (Util.SmiteIsReady)
                {
                    comboDamage += Util.MyHero.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Smite);
                }
                comboDamage += Util.MyHero.GetAutoAttackDamage(target, true);
            }
            comboDamage = comboDamage * Overkill;
            return new DamageResult(target, comboDamage, manaWasted);
        }


        public static DamageResult GetBestCombo(this Obj_AI_Base target)
        {
            var q = new[] { true };
            var w = SpellSlot.W.IsReady() ? new[] { false, true } : new[] { false };
            var e = SpellSlot.E.IsReady() ? new[] { false, true } : new[] { false };
            var r = SpellSlot.R.IsReady() ? new[] { false, true } : new[] { false };
            if (target != null)
            {
                DamageResult damageI2;
                if (PredictedDamage.ContainsKey(target.NetworkId))
                {
                    var damageI = PredictedDamage[target.NetworkId];
                    if (Game.Time - damageI.Time <= RefreshTime)
                    {
                        return PredictedDamage[target.NetworkId];
                    }
                    bool[] best = { false, false, false, false };
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
            return new DamageResult(null, 0, 0, false, false, false, false, 0);
        }
    }
}