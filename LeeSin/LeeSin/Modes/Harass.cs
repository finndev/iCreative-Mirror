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
    public static class Harass
    {
        public static Menu Menu
        {
            get
            {
                return MenuManager.GetSubMenu("Harass");
            }
        }
        public static bool IsActive
        {
            get
            {
                return ModeManager.IsHarass;
            }
        }
        public static void Execute()
        {

            var target = TargetSelector.Target;
            if (target.IsValidTarget())
            {
                if (Util.MyHero.IsInAutoAttackRange(target) && Champion.PassiveStack > 0) { return; }
                if (Menu.GetCheckBoxValue("Q")) { SpellManager.CastQ(target); }
                if (Menu.GetCheckBoxValue("E")) { SpellManager.CastE1(target); }
                if (_Q.IsDashing || _Q.IsWaitingMissile || _Q.HasQ2Buff || (SpellSlot.Q.IsReady() && SpellSlot.Q.IsFirstSpell() && Menu.GetCheckBoxValue("Q"))) { return; }
                if (Menu.GetCheckBoxValue("W"))
                {
                    var damageI = target.GetBestCombo();
                    if (target.IsInAutoAttackRange(Util.MyHero) && !damageI.IsKillable)
                    {
                        var obj = Champion.GetBestObjectFarFrom(target.Position);
                        if (obj != null && SpellManager.CanCastW1 && Extensions.Distance(Util.MyHero, target, true) < Extensions.Distance(obj, target, true))
                        {
                            SpellManager.CastW1(obj);
                        }
                    }
                }
            }
        }
    }
}
