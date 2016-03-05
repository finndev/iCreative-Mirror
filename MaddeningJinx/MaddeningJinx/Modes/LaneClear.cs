using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;

namespace MaddeningJinx
{
    public static class LaneClear
    {
        public static Menu Menu
        {
            get { return MenuManager.GetSubMenu("Clear"); }
        }

        public static bool IsActive
        {
            get { return ModeManager.IsLaneClear; }
        }

        public static IEnumerable<Obj_AI_Minion> Minions
        {
            get
            {
                return Orbwalker.LaneClearMinionsList.Where(m => m.IsInFishBonesRange()).Concat(LastHit.Minions);
            }
        }

        public static IEnumerable<Obj_AI_Base> AttackableUnits
        {
            get { return Minions.Concat(Combo.HeroesInFishBonesRange); }
        }

        private static bool CanAutoAttack(this Champion.BestResult result)
        {
            return (Orbwalker.LastHitMinion == null && !Orbwalker.ShouldWait) ||
                   LastHit.Minions.Contains(result.Target);
            //(Orbwalker.AlmostLasthittableMinion == null || !tuple.Item1.Intersect(Orbwalker.AlmostLasthittableMinions).Any());
        }

        public static void Execute()
        {
            if (Menu.Slider("LaneClear.Q") > 0)
            {
                if ((!ModeManager.CanUseQ || (MyTargetSelector.Target.IsInEnemyTurret() && Util.MyHero.IsInEnemyTurret())) && !Champion.ManualSwitch)
                {
                    Champion.DisableFishBones();
                    return;
                }
                var t = LastHit.AttackableUnits.GetBestFishBonesTarget();
                if (t.List.Count >= Menu.Slider("LastHit.Q") || (Champion.ManualSwitch && t.List.Count > 0))
                {
                    Champion.EnableFishBones(t.Target);
                }
                else
                {
                    t = AttackableUnits.GetBestFishBonesTarget();
                    if ((t.List.Count >= Menu.Slider("LaneClear.Q") && t.CanAutoAttack()) || (Champion.ManualSwitch && t.List.Count > 0))
                    {
                        Champion.EnableFishBones(t.Target);
                    }
                    else
                    {
                        Champion.DisableFishBones();
                    }
                }
            }
            else
            {
                Orbwalker.ForcedTarget = null;
            }
        }
    }
}