using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace Project_Zed
{
    public static class ItemManager
    {
        public static Item[] OffensiveItem = { new Item(ItemId.Bilgewater_Cutlass, 450), new Item(ItemId.Blade_of_the_Ruined_King, 450), new Item(ItemId.Hextech_Gunblade, 400), new Item(ItemId.Tiamat_Melee_Only, 280), new Item(ItemId.Ravenous_Hydra_Melee_Only, 280), new Item(ItemId.Youmuus_Ghostblade, 250 + Program.MyHero.GetAutoAttackRange() + 60), new Item(ItemId.Randuins_Omen, 500) };
        public static void UseOffensiveItems(Obj_AI_Base target)
        {
            if (!target.IsValidTarget()) return;
            foreach (var i in OffensiveItem.Where(i => i.IsReady() && target.IsValidTarget(i.Range)))
            {
                i.Cast(target);
            }
        }
    }
}
