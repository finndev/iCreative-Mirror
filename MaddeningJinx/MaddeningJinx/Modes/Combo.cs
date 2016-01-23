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

        public static bool CanUseQ;
        public static bool CanUseW;
        public static bool CanUseE;

        public static void Execute()
        {
            var isValidTarget = MyTargetSelector.Target != null;
            if (isValidTarget)
            {
                CanUseQ = Util.MyHero.Mana >= SpellSlot.W.Mana() + (SpellSlot.E.IsLearned() ? SpellSlot.E.Mana() : 0f) + (SpellSlot.R.IsLearned() ? SpellSlot.R.Mana() : 0f);
                CanUseW = Util.MyHero.Mana >= SpellSlot.W.Mana() + (SpellSlot.R.IsLearned() ? SpellSlot.R.Mana() : 0f);
                CanUseE = Util.MyHero.Mana >= SpellSlot.E.Mana() + (SpellSlot.R.IsLearned() ? SpellSlot.R.Mana() : 0f);
                if (!MyTargetSelector.Target.IdEquals(MyTargetSelector.PowPowTarget) && CanUseW)
                {
                    SpellManager.CastW(MyTargetSelector.Target);
                }
                if (CanUseE)
                {
                    SpellManager.CastESlowed(MyTargetSelector.Target);
                    SpellManager.CastAoeE();
                }
                ItemManager.UseOffensiveItems(MyTargetSelector.FishBonesTarget);
            }
            if (!isValidTarget)
            {
                if (!KillSteal.UsingQ)
                {
                    Champion.DisableFishBones();
                }
                return;
            }
            var t = HeroesInFishBonesRange.GetBestFishBonesTarget();
            if (t.List.Count > 1 && t.CanAutoAttack())
            {
                Champion.EnableFishBones(t.Target);
            }
            else
            {
                //if (!MyTargetSelector.Target.IdEquals(MyTargetSelector.PowPowTarget) || Champion.PowPowBuffCount == 3)
                if (!MyTargetSelector.Target.IdEquals(MyTargetSelector.PowPowTarget) && CanUseQ)
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
