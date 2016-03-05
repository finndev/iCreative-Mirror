using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;

namespace MaddeningJinx
{
    public static class Combo
    {
        public static Menu Menu
        {
            get { return MenuManager.GetSubMenu("Combo"); }
        }
        public static bool IsActive
        {
            get { return ModeManager.IsCombo; }
        }

        public static IEnumerable<Obj_AI_Base> HeroesInFishBonesRange
        {
            get { return MyTargetSelector.ValidEnemiesInRange.Where(m => m.IsInFishBonesRange()); }
        }
        public static IEnumerable<Obj_AI_Base> HeroesInPowPowRange
        {
            get { return MyTargetSelector.ValidEnemiesInRange.Where(m => m.IsInPowPowRange()); }
        }
        private static bool CanAutoAttack(this Champion.BestResult result)
        {
            return result.List.Contains(MyTargetSelector.FishBonesTarget);
        }
        public static void Execute()
        {
            var isValidTarget = MyTargetSelector.Target != null;
            if (isValidTarget)
            {
                if (MyTargetSelector.PowPowTarget == null && ModeManager.CanUseW && Menu.CheckBox("W"))
                {
                    SpellManager.CastW(MyTargetSelector.Target);
                }
                if (ModeManager.CanUseE)
                {
                    if (Menu.CheckBox("E"))
                    {
                        SpellManager.CastESlowed(MyTargetSelector.Target);
                    }
                    if (Menu.Slider("E.Aoe") > 0)
                    {
                        SpellManager.CastAoeE(Menu.Slider("E.Aoe"));
                    }
                }
                ItemManager.UseOffensiveItems(MyTargetSelector.FishBonesTarget);
            }
            if (!isValidTarget && !Champion.ManualSwitch)
            {
                Champion.DisableFishBones();
                return;
            }
            var t = HeroesInFishBonesRange.GetBestFishBonesTarget();
            if ((t.List.Count >= Menu.Slider("Q.Aoe") && t.CanAutoAttack()) || (Champion.ManualSwitch && t.List.Count > 0))
            {
                Champion.EnableFishBones(t.Target);
            }
            else
            {
                //if (!MyTargetSelector.Target.IdEquals(MyTargetSelector.PowPowTarget) || Champion.PowPowBuffCount == 3)
                if (!MyTargetSelector.Target.IdEquals(MyTargetSelector.PowPowTarget) && Menu.CheckBox("Q") && ModeManager.CanUseQ)
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

