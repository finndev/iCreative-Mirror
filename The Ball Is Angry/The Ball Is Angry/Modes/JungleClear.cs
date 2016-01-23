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

namespace The_Ball_Is_Angry
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

        public static IEnumerable<Obj_AI_Base> Minions
        {
            get { return EntityManager.MinionsAndMonsters.Monsters.Where(m => Util.MyHero.IsInAutoAttackRange(m)); }
        }

        public static void Execute()
        {
        }
    }
}
