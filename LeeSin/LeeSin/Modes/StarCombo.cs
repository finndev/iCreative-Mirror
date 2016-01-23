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
    public static class StarCombo
    {
        public static int Mode
        {
            get
            {
                return Combo.Menu.GetSliderValue("Star.Mode");
            }
        }
        public static bool IsReady
        {
            get
            {
                return SpellSlot.R.IsReady();
            }
        }
        public static Menu Menu
        {
            get
            {
                return Combo.Menu;
            }
        }
        public static void Execute()
        {

            if (IsReady || _R.IsRecentKick)
            {

                var target = TargetSelector.Target;
                if (target.IsValidTarget(WardManager.WardRange))
                {
                    if (Util.MyHero.IsInAutoAttackRange(target) && Champion.PassiveStack > 2 - Menu.GetSliderValue("Star.Stack")) { return; }
                    if (Mode == 0 && Menu.GetCheckBoxValue("W") && Menu.GetCheckBoxValue("Star.Ward") && WardManager.CanWardJump && Insec.IsReady)
                    {
                        Insec.Execute();
                    }
                    if (WardManager.IsTryingToJump) { return; }
                    if (Menu.GetCheckBoxValue("Q") && SpellSlot.Q.IsReady())
                    {
                        if (SpellSlot.Q.IsFirstSpell())
                        {
                            switch (Mode)
                            {
                                case 0:
                                    SpellManager.CastQ1(target);
                                    break;
                                case 1:
                                    if (target.HaveR())
                                    {
                                        var pred = SpellManager.Q1.GetPrediction(target);
                                        if (pred.CollisionObjects.Count() == 0)
                                        {
                                            SpellManager.Q1.Cast(pred.CastPosition);
                                        }
                                    }
                                    break;
                            }

                        }
                        else
                        {
                            if (!_R.IsRecentKick && !SpellSlot.R.IsReady())
                            {
                                SpellManager.CastQ2(target);
                            }
                        }
                    }
                    if (Menu.GetCheckBoxValue("E") && SpellSlot.E.IsReady())
                    {
                        if (!SpellSlot.R.IsReady())
                        {
                            SpellManager.CastE(target);
                        }
                    }
                    if (SpellSlot.R.IsReady())
                    {
                        switch (Mode)
                        {
                            case 0:
                                if (target.HaveQ() || _Q.WillHit(target))
                                {
                                    SpellManager.CastR(target);
                                }
                                break;
                            case 1:
                                if (SpellManager.CanCastQ1)
                                {
                                    var endpos = target.Position + (target.Position - Util.MyHero.Position).Normalized() * SpellManager.RKick.Range;
                                    SpellManager.Q1.SourcePosition = endpos;
                                    SpellManager.Q1.RangeCheckSource = endpos;
                                    var pred = SpellManager.Q1.GetPrediction(target);
                                    if (pred.HitChancePercent >= 5)
                                    {
                                        SpellManager.CastR(target);
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
                NormalCombo.Execute();
            }
        }
    }
}
