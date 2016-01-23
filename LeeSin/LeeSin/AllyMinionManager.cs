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



namespace LeeSin
{
    public static class AllyMinionManager
    {
        public static Obj_AI_Base GetNearestTo(Vector3 position)
        {
            return EntityManager.MinionsAndMonsters.AlliedMinions.Where(m => m.IsValid && !m.IsDead && Extensions.Distance(Util.MyHero, m, true) <= Math.Pow(SpellManager.W1Range + SpellManager.WExtraRange, 2)).OrderBy(m => Extensions.Distance(m, position, true)).FirstOrDefault();
        }
        public static Obj_AI_Base GetFurthestTo(Vector3 position)
        {
            return EntityManager.MinionsAndMonsters.AlliedMinions.Where(m => m.IsValid && !m.IsDead && Extensions.Distance(Util.MyHero, m, true) <= Math.Pow(SpellManager.W1Range + SpellManager.WExtraRange, 2)).OrderBy(m => Extensions.Distance(m, position, true)).LastOrDefault();
        }
    }
}
