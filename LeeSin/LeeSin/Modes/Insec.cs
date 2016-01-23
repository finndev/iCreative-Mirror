using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using SharpDX;


namespace LeeSin
{
    public static class Insec
    {
        // T O D O
        public static Menu Menu
        {
            get
            {
                return MenuManager.GetSubMenu("Insec");
            }
        }
        private static int Priority
        {
            get
            {
                return Menu.GetSliderValue("Priority");
            }
        }
        public static bool IsActive
        {
            get
            {
                return ModeManager.IsInsec;
            }
        }

        private static bool IsPerformingRFlash
        {
            get { return Game.Time - _lastRFlashInsec < 5 && Game.Time - _R.LastCastTime < 5; }
        }
        private static Obj_AI_Base _allySelected;
        private static Vector3 _positionSelected;
        private static float _lastGapcloseAttempt;
        private static float _lastSetPositionTime;
        private static float _lastRFlashInsec;
        private const float Offset = 80f;

        public static void Init()
        {
            Game.OnWndProc += Game_OnWndProc;
            Game.OnUpdate += Game_OnTick;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.R)
                {
                    if (Menu.GetCheckBoxValue("Flash.Return") && SpellManager.FlashIsReady && IsActive)
                    {
                        Util.MyHero.Spellbook.CastSpell(SpellManager.Flash.Slot, ExpectedEndPosition);
                    }
                    if (IsPerformingRFlash && SpellManager.FlashIsReady && TargetSelector.Target.IsValidTarget())
                    {
                        var target = TargetSelector.Target;
                        var gapclosepos = target.Position + (target.Position - ExpectedEndPosition).Normalized() * DistanceBetween;
                        Util.MyHero.Spellbook.CastSpell(SpellManager.Flash.Slot, gapclosepos);
                    }
                }
            }
        }

        public static void Execute()
        {
            var target = TargetSelector.Target;
            if (Orbwalker.CanMove && Game.Time - _lastGapcloseAttempt > 0.25f)
            {

                if (target.IsValidTarget() && Extensions.Distance(Util.MyHero, ExpectedEndPosition, true) > Extensions.Distance(target, ExpectedEndPosition, true) && IsReady)
                {
                    var gapclosepos = target.Position + (target.Position - ExpectedEndPosition).Normalized() * DistanceBetween;
                    Orbwalker.MoveTo(gapclosepos);
                }
                else
                {
                    Orbwalker.MoveTo(Util.MousePos);
                }
            }
            if (target.IsValidTarget())
            {
                if (IsReady)
                {
                    if (IsActive)
                    {
                        if (SpellManager.CanCastQ1)
                        {
                            var predtarget = SpellManager.Q1.GetPrediction(target);
                            if (Menu.GetCheckBoxValue("Object") && predtarget.CollisionObjects.Length > 1)
                            {
                                foreach (var pred in EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Both, EntityManager.UnitTeam.Enemy, Util.MyHero.Position, SpellManager.Q2.Range).Where(m => m.IsValidTarget() && SpellSlot.Q.GetSpellDamage(m) < Prediction.Health.GetPrediction(m, SpellManager.Q1.CastDelay + 1000 * (int)(Extensions.Distance(Util.MyHero, m) / SpellManager.Q1.Speed)) && Extensions.Distance(Util.MyHero, target, true) > Extensions.Distance(m, target, true) && Extensions.Distance(m, target, true) < Math.Pow(WardManager.WardRange - DistanceBetween - Offset, 2)).OrderBy(m => Extensions.Distance(target, m, true)).Select(minion => SpellManager.Q1.GetPrediction(minion)).Where(pred => pred.HitChancePercent >= SpellSlot.Q.HitChancePercent()))
                                {
                                    SpellManager.Q1.Cast(pred.CastPosition);
                                }
                                foreach (var enemy in EntityManager.Heroes.Enemies.Where(m => m.NetworkId != target.NetworkId && m.IsValidTarget(SpellManager.Q2.Range) && SpellSlot.Q.GetSpellDamage(m) < m.Health && Extensions.Distance(Util.MyHero, target, true) > Extensions.Distance(m, target, true) && Extensions.Distance(m, target, true) < Math.Pow(WardManager.WardRange - DistanceBetween - Offset, 2)).OrderBy(m => Extensions.Distance(target, m, true)))
                                {
                                    SpellManager.CastQ1(enemy);
                                }
                            }
                            SpellManager.CastQ1(target);
                        }
                        if (Util.MyHero.Distance(target, true) > Math.Pow(WardManager.WardRange - DistanceBetween, 2))
                        {
                            if (_Q.HasQ2Buff)
                            {
                                if (_Q.IsValidTarget && target.Distance(_Q.Target, true) < Math.Pow(WardManager.WardRange - DistanceBetween - Offset, 2))
                                {
                                    TargetSelector.ForcedTarget = target;
                                    Champion.ForceQ2(target);
                                }
                            }
                        }
                    }
                    if (Util.MyHero.Distance(target, true) < Math.Pow(WardManager.WardRange - DistanceBetween, 2) && !IsRecent)
                    {
                        switch (Priority)
                        {
                            case 0:
                                if (WardManager.CanWardJump)
                                {
                                    WardJump(target);
                                }
                                else if (SpellManager.FlashIsReady)
                                {
                                    Flash(target);
                                }
                                break;
                            case 1:
                                if (SpellManager.FlashIsReady)
                                {
                                    Flash(target);
                                }
                                else if (WardManager.CanWardJump)
                                {
                                    WardJump(target);
                                }
                                break;
                        }
                    }
                    CastR(target);
                }
                else
                {
                    NormalCombo.Execute();
                }
            }
        }

        private static void RFlash(AIHeroClient target)
        {
            if (SpellManager.FlashIsReady)
            {
                if (target.Distance(Util.MyHero, true) <= SpellManager.R.RangeSquared)
                {
                    _allySelected = null;
                    _positionSelected = EndPosition;
                    _lastSetPositionTime = Game.Time;
                    _lastRFlashInsec = Game.Time;
                    TargetSelector.ForcedTarget = target;
                    SpellManager.CastR(target);
                }
            }
        }

        private static void FlashR(AIHeroClient target)
        {
            if (SpellManager.FlashIsReady)
            {
                var gapclosepos = target.Position + (target.Position - ExpectedEndPosition).Normalized() * DistanceBetween;
                var flashendpos = Util.MyHero.Position + (gapclosepos - Util.MyHero.Position).Normalized() * SpellManager.Flash.Range;
                if (Extensions.Distance(gapclosepos, target, true) <= Math.Pow(SpellManager.R.Range, 2) && Extensions.Distance(target.Position, flashendpos, true) > Math.Pow(50, 2) && Extensions.Distance(flashendpos, target, true) < Extensions.Distance(flashendpos, ExpectedEndPosition, true) && Extensions.Distance(gapclosepos, target, true) < Extensions.Distance(gapclosepos, ExpectedEndPosition, true))
                {
                    if (Orbwalker.CanMove)
                    {
                        _lastGapcloseAttempt = Game.Time;
                        //Orbwalker.MoveTo(gapclosepos + (gapclosepos - ExpectedEndPosition).Normalized() * (DistanceBetween + Util.myHero.BoundingRadius / 2));
                    }
                    _allySelected = null;
                    _positionSelected = EndPosition;
                    _lastSetPositionTime = Game.Time;
                    TargetSelector.ForcedTarget = target;
                    Util.MyHero.Spellbook.CastSpell(SpellManager.Flash.Slot, gapclosepos);
                }
            }
        }

        private static void Flash(AIHeroClient target)
        {
            if (SpellManager.FlashIsReady)
            {
                switch (Menu.GetSliderValue("Flash.Priority"))
                {
                    case 0:
                        RFlash(target);
                        break;
                    case 1:
                        FlashR(target);
                        break;
                    default:
                        if (target.Distance(Util.MyHero, true) <= (SpellManager.R.Range * 1.25).Pow())
                        {
                            RFlash(target);
                        }
                        else
                        {
                            FlashR(target);
                        }

                        break;
                }
            }
        }

        private static void WardJump(AIHeroClient target)
        {
            var pred = SpellManager.W1.GetPrediction(target);
            if (WardManager.CanWardJump && pred.HitChance >= HitChance.AveragePoint)
            {
                var gapclosepos = pred.CastPosition + (pred.CastPosition - ExpectedEndPosition).Normalized() * DistanceBetween;
                if (Extensions.Distance(gapclosepos, Util.MyHero, true) <= Math.Pow(WardManager.WardRange, 2) && Extensions.Distance(gapclosepos, target, true) <= Math.Pow(SpellManager.R.Range, 2) && Extensions.Distance(gapclosepos, target, true) < Extensions.Distance(gapclosepos, ExpectedEndPosition, true))
                {
                    if (Orbwalker.CanMove)
                    {
                        _lastGapcloseAttempt = Game.Time;
                        Orbwalker.MoveTo(gapclosepos + (gapclosepos - ExpectedEndPosition).Normalized() * (DistanceBetween + Util.MyHero.BoundingRadius / 2));
                    }
                    _allySelected = null;
                    _positionSelected = EndPosition;
                    _lastSetPositionTime = Game.Time;
                    TargetSelector.ForcedTarget = target;
                    var obj = Champion.GetBestObjectNearTo(gapclosepos);
                    if (obj != null && Extensions.Distance(obj, target, true) < Extensions.Distance(obj, ExpectedEndPosition, true))
                    {
                        SpellManager.CastW1(obj);
                    }
                    else
                    {
                        WardManager.CastWardTo(gapclosepos);
                    }
                }
            }
        }
        private static void CastR(Obj_AI_Base target)
        {
            if (SpellSlot.R.IsReady() && target.IsValidTarget(SpellManager.R.Range) && Extensions.Distance(Util.MyHero, ExpectedEndPosition, true) > Extensions.Distance(target, ExpectedEndPosition, true))
            {
                var extended = ExpectedEndPosition + (ExpectedEndPosition - target.Position).Normalized() * SpellManager.RKick.Range * 0.5f;
                var realendpos = target.Position + (target.Position - Util.MyHero.Position).Normalized() * SpellManager.RKick.Range;
                var info = realendpos.To2D().ProjectOn(target.Position.To2D(), extended.To2D());
                if (info.IsOnSegment && info.SegmentPoint.Distance(ExpectedEndPosition.To2D(), true) <= Math.Pow(SpellManager.RKick.Range * 0.5f, 2))
                {
                    SpellManager.CastR(target);
                }
            }
        }
        private static bool IsValidPosition(this Vector3 position)
        {
            var target = TargetSelector.Target;
            if (target.IsValidTarget())
            {
                return target.Distance(position, true) <= Math.Pow(SpellManager.RKick.Range + 500f, 2);
            }
            return false;
        }
        private static float DistanceBetween
        {
            get
            {
                var target = TargetSelector.Target;
                if (target.IsValidTarget())
                {
                    return Math.Min((Util.MyHero.BoundingRadius + target.BoundingRadius + 50f) * (100 + Menu.GetSliderValue("DistanceBetweenPercent")) / 100, SpellManager.R.Range);
                }
                return 0;
            }
        }
        public static Vector3 EndPosition
        {
            get
            {
                var target = TargetSelector.Target;
                if (target.IsValidTarget())
                {
                    return target.Position + (ExpectedEndPosition - target.Position).Normalized() * SpellManager.RKick.Range;
                }
                return Vector3.Zero;
            }
        }
        public static Vector3 ExpectedEndPosition
        {
            get
            {
                var target = TargetSelector.Target;
                if (target.IsValidTarget())
                {
                    if (_allySelected != null && _allySelected.IsValidAlly() && _allySelected.Position.IsValidPosition())
                    {
                        return _allySelected.Position + (target.Position - _allySelected.Position).Normalized().To2D().Perpendicular().To3D() * (_allySelected.AttackRange + _allySelected.BoundingRadius + target.BoundingRadius) / 2;
                    }
                    if (_positionSelected != Vector3.Zero && _positionSelected.IsValidPosition())
                    {
                        return _positionSelected;
                    }
                    switch (Menu.GetSliderValue("Position"))
                    {
                        case 1:
                            return Util.MousePos;
                        case 2:
                            return Util.MyHero.Position;
                        default:
                            var turret = EntityManager.Turrets.Allies.Where(m => m.IsValidTarget() && m.Position.IsValidPosition() && m.Distance(target) - SpellManager.RKick.Range <= 750 + 200).OrderBy(m => Extensions.Distance(Util.MyHero, m, true)).FirstOrDefault();
                            if (turret != null)
                            {
                                return turret.Position;
                            }
                            var ally = EntityManager.Heroes.Allies.Where(m => m.IsValidTarget() && !m.IsMe && m.Position.IsValidPosition()).OrderBy(m => m.GetPriority()).LastOrDefault();
                            if (ally != null)
                                return ally.Position;
                            break;

                    }
                }
                return Util.MyHero.Position;
            }
        }
        public static bool IsReady
        {
            get
            {
                return (WardManager.CanWardJump || SpellManager.FlashIsReady || IsRecent) && SpellSlot.R.IsReady() && TargetSelector.Target != null && TargetSelector.Target.IsValidTarget();
            }
        }
        public static bool IsRecent
        {
            get
            {
                return Game.Time - SpellManager.WLastCastTime < 5 || Game.Time - SpellManager.FlashLastCastTime < 5 || Game.Time - WardManager.LastWardCreated < 5;
            }
        }
        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowMessages.LeftButtonDown)
            {
                if (IsReady)
                {
                    var target = EloBuddy.SDK.TargetSelector.GetTarget(200f, TargetSelector.damageType, Util.MousePos);
                    if (target.IsValidTarget())
                    {

                    }
                    else
                    {
                        var ally = AllyHeroManager.GetNearestTo(Util.MousePos);
                        if (ally != null && Extensions.Distance(ally, Util.MousePos) <= 200f)
                        {
                            _allySelected = ally;
                            _positionSelected = Vector3.Zero;
                            _lastSetPositionTime = Game.Time;
                        }
                        else
                        {
                            _allySelected = null;
                            _positionSelected = new Vector3(Util.MousePos.X, Util.MousePos.Y, Util.MousePos.Z);
                            _lastSetPositionTime = Game.Time;
                        }
                    }
                }
            }
        }
        private static void Game_OnTick(EventArgs args)
        {
            if (Game.Time - _lastSetPositionTime > 10 && _lastSetPositionTime > 0)
            {
                _allySelected = null;
                _positionSelected = Vector3.Zero;
                _lastSetPositionTime = 0;
                TargetSelector.ForcedTarget = null;
            }
            if (IsPerformingRFlash && SpellManager.FlashIsReady && TargetSelector.Target.IsValidTarget())
            {
                var target = TargetSelector.Target;
                var gapclosepos = target.Position + (target.Position - ExpectedEndPosition).Normalized() * DistanceBetween;
                Util.MyHero.Spellbook.CastSpell(SpellManager.Flash.Slot, gapclosepos);
            }
        }
    }
}
