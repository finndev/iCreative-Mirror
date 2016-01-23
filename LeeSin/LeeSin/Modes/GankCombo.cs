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
    public static class GankCombo
    {
        public static Menu Menu
        {
            get
            {
                return Combo.Menu;
            }
        }
        public static void Execute()
        {
            var target = TargetSelector.Target;
            if (target.IsValidTarget())
            {
                if (Util.MyHero.IsInAutoAttackRange(target) && Champion.PassiveStack > 2 - Menu.GetSliderValue("Gank.Stack")) { return; }
                if (Menu.GetCheckBoxValue("W") && target.IsValidTarget(900f))
                {
                    if (Menu.GetCheckBoxValue("Gank.Ward") && WardManager.CanCastWard)
                    {
                        if (Insec.IsReady && Menu.GetCheckBoxValue("Gank.R"))
                        {
                            Insec.Execute(); // C H E C K
                        }
                        else if (Extensions.Distance(Util.MyHero, target, true) > Math.Pow(450, 2) && SpellManager.CanCastW1)
                        {
                            Champion.GapCloseWithWard(target);
                        }
                    }
                    else
                    {
                        Champion.GapCloseWithoutWard(target);
                    }
                }
                if (SpellManager.CanCastW1 && !target.IsValidTarget(SpellManager.W1Range)) { return; }
                if (WardManager.IsTryingToJump) { return; }
                if (Menu.GetCheckBoxValue("E") && !SpellSlot.Q.IsReady()) { SpellManager.CastE(target); }
                if (Menu.GetCheckBoxValue("Q")) { SpellManager.CastQ(target); }
            }
        }
    }
}
