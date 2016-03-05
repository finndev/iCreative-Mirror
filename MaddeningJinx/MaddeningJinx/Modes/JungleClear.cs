using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;

namespace MaddeningJinx
{
    public static class JungleClear
    {
        public static bool IsActive
        {
            get
            {
                return ModeManager.IsJungleClear;
            }
        }
        public static Menu Menu
        {
            get
            {
                return MenuManager.GetSubMenu("Clear");
            }
        }

        public static IEnumerable<Obj_AI_Base> Minions
        {
            get { return EntityManager.MinionsAndMonsters.Monsters.Where(m => m.IsInFishBonesRange()).OrderByDescending(o => o.MaxHealth); }
        }

        public static void Execute()
        {
            if (Menu.Slider("JungleClear.Q") > 0)
            {
                if (!ModeManager.CanUseQ && !Champion.ManualSwitch)
                {
                    Champion.DisableFishBones();
                    return;
                }
                var t = Minions.GetBestFishBonesTarget();
                if (t.List.Count >= Menu.Slider("JungleClear.Q") || (Champion.ManualSwitch && t.List.Count > 0))
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
