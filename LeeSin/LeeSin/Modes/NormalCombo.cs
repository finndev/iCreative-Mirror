using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;

namespace LeeSin
{
    public static class NormalCombo
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
                if (Util.MyHero.HealthPercent <= Menu.GetSliderValue("Normal.W") && SpellSlot.W.IsReady())
                {
                    if (SpellSlot.W.IsFirstSpell())
                    {
                        if (target.IsInAutoAttackRange(Util.MyHero) || Util.MyHero.IsInAutoAttackRange(target)) { SpellManager.CastW1(Util.MyHero); }
                    }
                    else
                    {
                        if (Util.MyHero.IsInAutoAttackRange(target)) { SpellManager.CastW2(); }
                    }
                }
                if (Util.MyHero.IsInAutoAttackRange(target) && Champion.PassiveStack > 2 - Menu.GetSliderValue("Normal.Stack")) { return; }
                if (Menu.GetCheckBoxValue("Normal.R")) { SpellManager.CastR(target); }
                var t = _R.BestHitR(Menu.GetSliderValue("Normal.R.Hit"));
                if (Menu.GetSliderValue("Normal.R.Hit") <= t.Item1)
                {
                    SpellManager.CastR(t.Item2);
                }
                if (Menu.GetCheckBoxValue("E")) { SpellManager.CastE(target); }
                if (Menu.GetCheckBoxValue("Q") && SpellSlot.Q.IsReady())
                {
                    if (SpellSlot.Q.IsFirstSpell())
                    {
                        SpellManager.CastQ1(target);
                    }
                    else if (!target.HaveR())
                    {
                        SpellManager.CastQ2(target);
                    }
                }
                if (Menu.GetCheckBoxValue("W") && SpellSlot.W.IsReady() && !SpellSlot.W.IsFirstSpell() && Util.MyHero.IsInAutoAttackRange(target)) { SpellManager.CastW2(); }
                if (_Q.IsDashing || _Q.IsWaitingMissile || _Q.HasQ2Buff) { return; }
                if (Util.MyHero.Distance(target, true) > Math.Pow(500, 2) && Menu.GetCheckBoxValue("W") && SpellManager.CanCastW1)
                {
                    if (Menu.GetCheckBoxValue("Normal.Ward"))
                    {
                        Champion.GapCloseWithWard(target);
                    }
                    else
                    {
                        Champion.GapCloseWithoutWard(target);
                    }
                }
            }
        }
    }
}
