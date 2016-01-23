using EloBuddy;
using EloBuddy.SDK;

namespace Syndra
{
    public static class ItemManager
    {
        private static Item[] OffensiveItem = new Item[] { new Item((int)ItemId.Bilgewater_Cutlass, 450), new Item((int)ItemId.Blade_of_the_Ruined_King, 450), new Item((int)ItemId.Hextech_Gunblade, 400), new Item((int)ItemId.Tiamat_Melee_Only, 280), new Item((int)ItemId.Ravenous_Hydra_Melee_Only, 280), new Item((int)ItemId.Youmuus_Ghostblade, 250 + Util.MyHero.GetAutoAttackRange() + 60), new Item((int)ItemId.Randuins_Omen, 500) };
        private static Item Zhonyas = new Item((int)ItemId.Zhonyas_Hourglass, int.MaxValue);
        public static void UseOffensiveItems(Obj_AI_Base target)
        {
            if (target.IsValidTarget())
            {
                foreach (Item i in OffensiveItem)
                {
                    if (i.IsReady() && target.IsValidTarget(i.Range))
                    {
                        i.Cast(target);
                    }
                }
            }
        }
        public static void UseZhonyas()
        {
            if (Zhonyas.IsReady())
            {
                Zhonyas.Cast();
            }
        }
    }
}
