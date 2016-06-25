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

namespace Draven_Me_Crazy
{
    public static class ItemManager
    {
        private static Item[] OffensiveItem = new Item[] { new Item((int)ItemId.Bilgewater_Cutlass, 450), new Item((int)ItemId.Blade_of_the_Ruined_King, 450), new Item((int)ItemId.Hextech_Gunblade, 400), new Item((int)ItemId.Tiamat_Melee_Only, 280), new Item((int)ItemId.Ravenous_Hydra_Melee_Only, 280), new Item((int)ItemId.Youmuus_Ghostblade, 250 + Util.MyHero.GetAutoAttackRange() + 60), new Item((int)ItemId.Randuins_Omen, 500) };
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
    }
}
