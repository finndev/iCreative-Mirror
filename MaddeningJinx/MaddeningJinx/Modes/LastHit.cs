using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;

namespace MaddeningJinx
{
    public static class LastHit
    {
        public static Menu Menu
        {
            get { return MenuManager.GetSubMenu("Clear"); }
        }

        public static bool IsActive
        {
            get { return ModeManager.IsLastHit; }
        }

        public static IEnumerable<Obj_AI_Minion> Minions
        {
            get { return Orbwalker.LastHitMinionsList.Where(m => m.IsInFishBonesRange()); }
        }

        public static IEnumerable<Obj_AI_Base> AttackableUnits
        {
            get { return Minions.Concat(Combo.HeroesInFishBonesRange).ToList(); }
        }

        private static bool CanAutoAttack(this Champion.BestResult result)
        {
            return Minions.Contains(result.Target);
        }

        public static void Execute()
        {
            if (Menu.Slider("LastHit.Q") > 0)
            {
                if ((!ModeManager.CanUseQ || (MyTargetSelector.Target.IsInEnemyTurret() && Util.MyHero.IsInEnemyTurret())) && !Champion.ManualSwitch)
                {
                    Champion.DisableFishBones();
                    return;
                }
                var t = AttackableUnits.GetBestFishBonesTarget();
                if ((t.List.Count >= Menu.Slider("LastHit.Q") && t.CanAutoAttack()) || (Champion.ManualSwitch && t.List.Count > 0))
                {
                    Champion.EnableFishBones(t.Target);
                }
                else
                {
                    Champion.DisableFishBones();
                }
            }
            else
            {
                Orbwalker.ForcedTarget = null;
            }
        }
    }
}