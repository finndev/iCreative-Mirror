using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace MaddeningJinx
{
    public static class ItemManager
    {
        private static readonly Item[] OffensiveItem = { new Item(ItemId.Bilgewater_Cutlass, 450), new Item(ItemId.Blade_of_the_Ruined_King, 450), new Item(ItemId.Hextech_Gunblade, 400), new Item(ItemId.Tiamat_Melee_Only, 280), new Item(ItemId.Ravenous_Hydra_Melee_Only, 280), new Item(ItemId.Youmuus_Ghostblade, 250 + Util.MyHero.GetAutoAttackRange() + 60), new Item(ItemId.Randuins_Omen, 500) };
        private static readonly Item Zhonyas = new Item(ItemId.Zhonyas_Hourglass, int.MaxValue);
        public static void UseOffensiveItems(Obj_AI_Base target)
        {
            if (target != null)
            {
                foreach (var i in OffensiveItem.Where(i => i.IsReady() && Util.MyHero.IsInRange(target, i.Range)))
                {
                    i.Cast(target);
                }
            }
        }
    }
}
