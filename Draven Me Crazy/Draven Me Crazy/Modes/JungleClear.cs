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
                return MenuManager.GetSubMenu("JungleClear");
            }
        }
        
        public static void Execute()
        {
            if (Util.MyHero.ManaPercent >= Menu.GetSliderValue("Mana"))
            {
                foreach (Obj_AI_Minion minion in EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Monster, EntityManager.UnitTeam.Enemy, Util.MyHero.Position, SpellManager.Q.Range, true))
                {
                    if (minion.IsValidTarget() && Util.MyHero.ManaPercent >= Menu.GetSliderValue("Mana"))
                    {
                        SpellManager.CastQ(minion, Menu.GetSliderValue("Q"));
                        if (Menu.GetCheckBoxValue("E")) { SpellManager.CastE(minion); }
                        if (Menu.GetCheckBoxValue("W")) { SpellManager.CastW(minion); }
                    }
                }
            }
        }
    }
}
