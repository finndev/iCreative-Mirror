using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;

namespace MaddeningJinx
{
    public static class Harass
    {
        public static Menu Menu
        {
            get { return MenuManager.GetSubMenu("Harass"); }
        }

        public static bool IsActive
        {
            get { return ModeManager.IsHarass; }
        }
        public static IEnumerable<Obj_AI_Base> AttackableUnits
        {
            get { return LastHit.Minions.Concat(Combo.HeroesInFishBonesRange); }
        }
        private static bool CanAutoAttack(this Champion.BestResult result)
        {
            return LastHit.Minions.Contains(result.Target) /* && result.List.Intersect(Combo.HeroesInFishBonesRange).Any()*/;
        }
        public static void Execute()
        {
            if (MyTargetSelector.Target != null && MyTargetSelector.Target.IsInEnemyTurret() && Util.MyHero.IsInEnemyTurret())
            {
                Champion.DisableFishBones();
                return;
            }
            var t = AttackableUnits.GetBestFishBonesTarget();
            if ((t.List.Count > 1 && t.CanAutoAttack()) || (Champion.ManualSwitch && t.List.Count > 0))
            {
                Champion.EnableFishBones(t.Target);
            }
            else
            {
                if (Orbwalker.LastHitMinion == null && !Orbwalker.ShouldWait && MyTargetSelector.Target != null && Util.MyHero.Distance(Util.MousePos, true) >= Util.MousePos.Distance(MyTargetSelector.Target, true) && Util.MyHero.IsInRange(MyTargetSelector.Target, MyTargetSelector.AaRange) && ModeManager.CanUseQ)
                {
                    Champion.EnableFishBones(MyTargetSelector.FishBonesTarget);
                }
                else
                {
                    Champion.DisableFishBones();
                }
            }
        }
    }
}
