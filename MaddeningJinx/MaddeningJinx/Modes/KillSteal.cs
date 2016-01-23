using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using SharpDX;

namespace MaddeningJinx
{
    public static class KillSteal
    {
        public static readonly List<Obj_AI_Base> RKillableBases = new List<Obj_AI_Base>();
        public static readonly List<Vector3> RHittableBases = new List<Vector3>();
        public static readonly Dictionary<int, float> RDamageOnEnemies = new Dictionary<int, float>();
        public static bool UsingQ;
        public static Menu Menu
        {
            get
            {
                return MenuManager.GetSubMenu("KillSteal");
            }
        }

        public static void Execute()
        {
            RKillableBases.Clear();
            RHittableBases.Clear();
            RDamageOnEnemies.Clear();
            UsingQ = false;
            if (SpellSlot.R.IsReady() && Util.MyHero.Mana >= SpellSlot.R.Mana())
            {
                foreach (var killableEnemy in MyTargetSelector.ValidEnemies)
                {
                    SpellManager.CheckRKillable(killableEnemy);
                }
                var validBases = RHittableBases.Where(v => !(Core.GameTickCount - SpellManager.WLastCastTime <= 600 || Core.GameTickCount - SpellManager.WCastSpellTime < 100 ||
                        (SpellManager.WMissile != null &&
                         Util.MyHero.Distance(SpellManager.WMissile, true) <= Util.MyHero.Distance(v, true)))).ToList();
                if (validBases.Any())
                {
                    var bestKillable = validBases.OrderBy(h => Util.MyHero.Distance(h, true)).FirstOrDefault();
                    SpellManager.R.Cast(bestKillable);
                }
            }
            if (MenuManager.Menu.GetCheckBoxValue("KillSteal"))
            {
                foreach (var enemy in MyTargetSelector.ValidEnemiesInRange.Where(m => m.HealthPercent < 40))
                {
                    var result = enemy.GetBestCombo();
                    if (result.IsKillable)
                    {
                        if (result.CanKillWith(SpellSlot.Q) && enemy.IsInFishBonesRange())
                        {
                            UsingQ = true;
                            Champion.EnableFishBones(enemy);
                        }
                        else if (result.CanKillWith(SpellSlot.W)) { SpellManager.CastW(enemy); }
                        if (result.CanKillWith(SpellSlot.E)) { SpellManager.CastESlowed(enemy); }
                    }
                }
            }
        }
    }
}
