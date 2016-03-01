using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace LeeSin
{
    public static class _Q
    {
        public static Obj_AI_Base Target;
        public static bool IsDashing;
        private static MissileClient _missile;
        private static float _lastCastTime;
        private static Obj_AI_Minion _smiteTarget;
        public static void Init()
        {
            Game.OnUpdate += Game_OnTick;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
            Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffLose;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            GameObject.OnCreate += MissileClient_OnCreate;
            GameObject.OnDelete += MissileClient_OnDelete;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (MissileIsValid)
            {
                SpellManager.Q1.SourcePosition = _missile.Position;
                SpellManager.Q1.RangeCheckSource = _missile.Position;
                SpellManager.Q1.AllowedCollisionCount = int.MaxValue;
                SpellManager.Q1.CastDelay = 0;
            }
            else
            {
                SpellManager.Q1.SourcePosition = Util.MyHero.Position;
                SpellManager.Q1.RangeCheckSource = Util.MyHero.Position;
                SpellManager.Q1.AllowedCollisionCount = 0;
                SpellManager.Q1.CastDelay = 250;
            }
            if (IsTryingToSmite)
            {
                if (IsWaitingMissile)//
                {
                    var canSmite = false;
                    if (Game.Time - _lastCastTime <= 0.25f)
                    {
                        if ((SpellManager.Q1.Width + _smiteTarget.BoundingRadius).Pow() > Util.MyHero.Distance(_smiteTarget, true))
                        {
                            canSmite = true;
                        }
                    }
                    else if (WillHit(_smiteTarget))
                    {
                        var pred = SpellManager.Q1.GetPrediction(_smiteTarget);
                        var width = _smiteTarget.BoundingRadius + SpellManager.Q1.Width;//
                        var timeToArriveQ = (_missile.Distance(pred.CastPosition) - width) / SpellManager.Q1.Speed - SpellManager.SmiteCastDelay - (Game.Ping / 2000f + 0.1f);
                        if (timeToArriveQ <= 0)
                        {
                            canSmite = true;
                        }
                    }
                    if (canSmite && _smiteTarget.IsInSmiteRange())
                    {
                        Util.MyHero.Spellbook.CastSpell(SpellManager.Smite.Slot, _smiteTarget);
                    }
                }
            }
            if (EndTime - Game.Time <= 0.25f)
            {
                if (!ModeManager.IsNone)
                {
                    Champion.ForceQ2();
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.Q)
                {
                    if (args.SData.Name.ToLower().Contains("one"))
                    {
                        _lastCastTime = Game.Time;
                    }
                    else
                    {
                        IsDashing = true;
                    }
                }
            }
        }

        public static bool HaveQ(this Obj_AI_Base unit)
        {
            return unit.IsValidTarget() && Target != null && unit.NetworkId == Target.NetworkId;
        }
        private static void MissileClient_OnCreate(GameObject sender, EventArgs args)
        {
            if (IsWaitingMissile)
            {
                var client = sender as MissileClient;
                if (client != null)
                {
                    var missile = client;
                    if (missile.SpellCaster.IsMe)
                    {
                        if (missile.SData.Name.ToLower().Contains("blindmonkqone"))
                        {
                            _missile = missile;
                            Core.DelayAction(delegate { _missile = null; }, 1000 * (int)(2 * Extensions.Distance(_missile, _missile.EndPosition) / SpellManager.Q1.Speed));
                        }
                    }
                }
            }
        }

        private static void MissileClient_OnDelete(GameObject sender, EventArgs args)
        {
            if (MissileIsValid)
            {
                var client = sender as MissileClient;
                if (client != null)
                {
                    var missile = client;
                    if (missile.SpellCaster.IsMe)
                    {
                        if (_missile.NetworkId == missile.NetworkId)
                        {
                            _smiteTarget = null;
                            _missile = null;
                        }
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (args.Buff.Caster.IsMe)
            {
                if (!sender.IsMe)
                {
                    if (args.Buff.Name.ToLower().Contains("blindmonkqone"))
                    {
                        Target = sender;
                    }
                }
                else
                {
                    if (args.Buff.Name.ToLower().Contains("blindmonkqtwodash"))
                    {
                        IsDashing = true;
                    }
                }
            }
        }
        private static void Obj_AI_Base_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (args.Buff.Caster.IsMe)
            {
                if (!sender.IsMe)
                {
                    if (args.Buff.Name.ToLower().Contains("blindmonkqone"))
                    {
                        Target = null;
                    }
                }
                else
                {
                    if (args.Buff.Name.ToLower().Contains("blindmonkqtwodash"))
                    {
                        IsDashing = false;
                    }
                }
            }
        }
        public static void CheckSmite(Obj_AI_Base target)
        {
            if (target is AIHeroClient && SpellManager.CanCastQ1 && target.IsValidTarget())
            {
                SpellManager.Q1.AllowedCollisionCount = int.MaxValue;
                var pred = SpellManager.Q1.GetPrediction(target);
                if (pred.HitChancePercent >= SpellSlot.Q.HitChancePercent())
                {
                    var minions = pred.GetCollisionObjects<Obj_AI_Minion>().Where(m => m.IsValidTarget() && Util.MyHero.Distance(m, true) < Util.MyHero.Distance(target, true)).ToList();
                    var canSmite = (Combo.IsActive && AutoSmite.Menu.GetCheckBoxValue("Q.Combo")) || (Harass.IsActive && AutoSmite.Menu.GetCheckBoxValue("Q.Harass")) || (Insec.IsActive && AutoSmite.Menu.GetCheckBoxValue("Q.Insec"));
                    var minion = minions.FirstOrDefault();
                    if (SpellManager.SmiteIsReady && minions.Count == 1 && canSmite && minion != null)
                    {
                        if (minion.IsInSmiteRange())
                        {
                            int time = SpellManager.Q1.CastDelay +  (int)(1000 * Util.MyHero.Distance(minion) / SpellManager.Q1.Speed) + (int)SpellManager.SmiteCastDelay * 1000;
                            if (Prediction.Health.GetPrediction(minion, time) <= Util.MyHero.GetSummonerSpellDamage(minion, DamageLibrary.SummonerSpells.Smite))
                            {
                                _smiteTarget = minion;
                                SpellManager.Q1.Cast(pred.CastPosition);
                            }
                        }
                    }
                    else if (!pred.CollisionObjects.Any(m => m.IsValidTarget() && Util.MyHero.Distance(m, true) < Util.MyHero.Distance(target, true)))
                    {
                        SpellManager.Q1.Cast(pred.CastPosition);
                    }
                }
            }
        }
        public static bool WillHit(Obj_AI_Base target)
        {
            if (MissileIsValid && target.IsValidTarget())
            {
                var pred = SpellManager.Q1.GetPrediction(target);
                var info = pred.CastPosition.To2D().ProjectOn(_missile.StartPosition.To2D(), _missile.EndPosition.To2D());
                float hitchancepercent = target.Type == GameObjectType.AIHeroClient ? SpellSlot.Q.HitChancePercent() / 2f : 0;
                if (info.IsOnSegment && pred.HitChancePercent >= hitchancepercent && info.SegmentPoint.Distance(pred.CastPosition.To2D(), true) <= Math.Pow(target.BoundingRadius + SpellManager.Q1.Width, 2))
                {
                    return true;
                }
            }
            return false;
        }
        private static BuffInstance Buff
        {
            get
            {
                if (Target != null)
                {
                    return Target.Buffs.FirstOrDefault(buff => buff.Name.ToLower().Contains("blindmonkqone"));
                }
                return null;
            }
        }
        public static bool IsTryingToSmite
        {
            get
            {
                return _smiteTarget != null && _smiteTarget.IsValidTarget() && SpellManager.SmiteIsReady;
            }
        }
        public static float EndTime
        {
            get
            {
                if (Buff != null)
                {
                    return Buff.EndTime;
                }
                return 0f;
            }
        }
        public static bool IsValidTarget
        {
            get
            {
                return Target != null && Target.IsValidTarget();
            }
        }
        public static bool HasQ2Buff
        {
            get
            {
                return (SpellSlot.Q.IsReady() && !SpellSlot.Q.IsFirstSpell()) || IsValidTarget;
            }
        }
        private static bool MissileIsValid
        {
            get
            {
                return _missile != null;
            }
        }
        public static bool IsWaitingMissile
        {
            get
            {
                return MissileIsValid || Game.Time - _lastCastTime <= 0.29f;
            }
        }

    }
}
