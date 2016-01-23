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
            get { return MenuManager.GetSubMenu("LastHit"); }
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
            if (MenuManager.Menu.GetCheckBoxValue("Farming.Q"))
            {
                if (!Combo.CanUseQ || (MyTargetSelector.Target.IsInEnemyTurret() && Util.MyHero.IsInEnemyTurret()))
                {
                    Champion.DisableFishBones();
                    return;
                }
                var t = AttackableUnits.GetBestFishBonesTarget();
                if (t.List.Count > 1 && t.CanAutoAttack())
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