using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace Thresh
{
    public static class ItemManager
    {
        private static readonly Item[] OffensiveItems = { new Item((int)ItemId.Bilgewater_Cutlass, 450), new Item((int)ItemId.Blade_of_the_Ruined_King, 450), new Item((int)ItemId.Hextech_Gunblade, 400), new Item((int)ItemId.Tiamat_Melee_Only, 280), new Item((int)ItemId.Ravenous_Hydra_Melee_Only, 280), new Item((int)ItemId.Youmuus_Ghostblade, 250 + Util.MyHero.GetAutoAttackRange() + 60), new Item((int)ItemId.Randuins_Omen, 500) };
        private static readonly Item Zhonyas = new Item((int)ItemId.Zhonyas_Hourglass, int.MaxValue);
        public static void UseOffensiveItems(Obj_AI_Base target)
        {
            if (!target.IsValidTarget()) return;
            foreach (var i in OffensiveItems.Where(i => i.IsReady() && target.IsValidTarget(i.Range)))
            {
                i.Cast(target);
            }
        }

        public static void UseZhonyas()
        {
            if (Zhonyas.IsReady())
            {
                Zhonyas.Cast();
            }
        }

        public static bool OffensiveItemIsReady
        {
            get
            { return OffensiveItems.Any(i => i.IsReady()); }
        }
        public static float OffensiveItemsDamage(this AIHeroClient source, Obj_AI_Base target)
        {
            return !target.IsValidTarget() ? 0f : OffensiveItems.Where(i => i.IsReady()).Sum(i => source.GetItemDamage(target, i.Id));
        }
    }
}
