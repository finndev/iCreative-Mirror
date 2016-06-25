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
    public static class Harass
    {
        public static Menu Menu
        {
            get
            {
                return MenuManager.GetSubMenu("Harass");
            }
        }
        public static bool IsActive
        {
            get
            {
                return ModeManager.IsHarass;
            }
        }
        public static void Execute()
        {
            if (Menu.GetSliderValue("Mana") <= Util.MyHero.ManaPercent)
            {
                var target = TargetSelector.Target;
                if (target.IsValidTarget())
                {
                    SpellManager.CastQ(target, Menu.GetSliderValue("Q"));
                    var minion = EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Minion, EntityManager.UnitTeam.Enemy, Util.MyHero.Position, SpellManager.Q.Range, true).FirstOrDefault();
                    if (minion != null && minion.IsValidTarget())
                    {
                        SpellManager.CastQ(minion, Menu.GetSliderValue("Q"));
                    }
                    if (Menu.GetCheckBoxValue("E")) { SpellManager.CastE(target); }
                    if (Menu.GetCheckBoxValue("W")) { SpellManager.CastW(target); }
                    if (Menu.GetCheckBoxValue("AA"))
                    {

                        if (Util.MyHero.HasBuff("dravenspinningattack"))
                        {
                            var buff = Util.MyHero.GetBuff("dravenspinningattack");
                            if (Orbwalker.CanAutoAttack)
                            {
                                if (buff.EndTime - Game.Time <= 1.25f + Util.MyHero.AttackCastDelay)
                                {
                                    Obj_AI_Base BestTarget = null;
                                    AIHeroClient target2 = TargetSelector.Target;
                                    if (target2 != null && target2.IsValidTarget())
                                    {
                                        BestTarget = target2;
                                    }
                                    else
                                    {
                                        Obj_AI_Minion BestMinion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsValidTarget() && Util.MyHero.IsInAutoAttackRange(m) && (Prediction.Health.GetPrediction(m, 2 * 1000 * (int)(Util.MyHero.AttackDelay + Util.MyHero.AttackCastDelay + Extensions.Distance(Util.MyHero, m) / Util.MyHero.BasicAttack.MissileSpeed - 0.07f)) > 2 * Util.MyHero.GetAutoAttackDamage(m) || Prediction.Health.GetPrediction(m, 1000 * (int)(Util.MyHero.AttackCastDelay + Extensions.Distance(Util.MyHero, m) / Util.MyHero.BasicAttack.MissileSpeed - 0.07f)) == m.Health)).OrderBy(m => m.HealthPercent).LastOrDefault();
                                        if (BestMinion != null && BestMinion.IsValidTarget())
                                        {
                                            BestTarget = BestMinion;
                                        }
                                        else
                                        {
                                            BestMinion = EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.IsValidTarget() && Util.MyHero.IsInAutoAttackRange(m)).OrderBy(m => m.HealthPercent).LastOrDefault();
                                            if (BestMinion != null && BestMinion.IsValidTarget())
                                            {
                                                BestTarget = BestMinion;
                                            }
                                        }
                                    }
                                    Orbwalker.ForcedTarget = BestTarget;
                                }
                                else
                                {
                                    Orbwalker.ForcedTarget = null;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
