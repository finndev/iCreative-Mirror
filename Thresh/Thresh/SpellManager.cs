using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace Thresh
{
    public static class SpellManager
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Active R;
        public static Spell.Targeted Ignite, Smite;
        public static Spell.Skillshot Flash;
        private static float _qLastCastTime;
        private static float _qArriveTime;
        public static Obj_AI_Base QTarget;
        private static bool IsQ1
        {
            get { return Q.Name.Equals("ThreshQ"); }
        }

        public static bool SmiteIsReady
        {
            get { return Smite != null && Smite.IsReady(); }
        }

        public static bool CanUseSmiteOnHeroes
        {
            get
            {
                if (!SmiteIsReady) return false;
                var name = Smite.Slot.GetSpellDataInst().SData.Name.ToLower();
                return name.Contains("smiteduel") || name.Contains("smiteplayerganker");
            }
        }

        public static bool IgniteIsReady
        {
            get { return Ignite != null && Ignite.IsReady(); }
        }

        public static bool FlashIsReady
        {
            get { return Flash != null && Flash.IsReady(); }
        }

        public static void Init(EventArgs args)
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1040, SkillShotType.Linear, 500, 1900, 60) //RealRange = 1075, RealWidth = 70.
            {
                AllowedCollisionCount = 0
            };
            W = new Spell.Skillshot(SpellSlot.W, 950, SkillShotType.Circular, 250, 1800, 300)
            {
                AllowedCollisionCount = int.MaxValue
            };
            E = new Spell.Skillshot(SpellSlot.E, 480, SkillShotType.Linear, 0, 2000, 110)
            {
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Active(SpellSlot.R, 450);
            var slot = Util.MyHero.SpellSlotFromName("summonerdot");
            if (slot != SpellSlot.Unknown)
            {
                Ignite = new Spell.Targeted(slot, 600);
            }
            slot = Util.MyHero.SpellSlotFromName("smite");
            if (slot != SpellSlot.Unknown)
            {
                Smite = new Spell.Targeted(slot, 500);
            }
            slot = Util.MyHero.SpellSlotFromName("flash");
            if (slot != SpellSlot.Unknown)
            {
                Flash = new Spell.Skillshot(slot, 400, SkillShotType.Circular);
            }
            Game.OnTick += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
            Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffLose;
        }


        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!args.Buff.Caster.IsMe) return;
            if (args.Buff.Name.Equals("ThreshQ"))
            {
                QTarget = sender;
                _qArriveTime = Game.Time;
            }
        }

        private static void Obj_AI_Base_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (!args.Buff.Caster.IsMe) return;
            if (args.Buff.Name.Equals("ThreshQ"))
            {
                QTarget = null;
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(sender is AIHeroClient) || !sender.IsAlly) return;
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.Q)
                {
                    if (args.SData.Name.Equals("ThreshQ"))
                    {
                        _qLastCastTime = Game.Time;
                    }
                }
            }
            else
            {
                if (args.SData.Name.Equals("LanternWAlly") && !IsQ1)
                {
                    Util.MyHero.Spellbook.CastSpell(SpellSlot.Q);
                }
            }
        }
        public static SpellSlot SpellSlotFromName(this AIHeroClient hero, string name)
        {
            foreach (var s in hero.Spellbook.Spells.Where(s => s.Name.ToLower().Contains(name.ToLower())))
            {
                return s.Slot;
            }
            return SpellSlot.Unknown;
        }

        public static void CastQ(Obj_AI_Base target)
        {
            if (SpellSlot.Q.IsReady() && target.IsValidTarget() && target.IsEnemy)
            {
                if (IsQ1)
                {
                    CastQ1(target);
                }
                else
                {
                    CastQ2(target);
                }
            }
        }

        public static void CastQ1(Obj_AI_Base target)
        {
            if (SpellSlot.Q.IsReady() && IsQ1 && target.IsValidTarget(Q.Range) && target.IsEnemy)
            {
                Q.Width = 70;
                var pred = Q.GetPrediction(target);
                if (pred.HitChancePercent >= Q.Slot.HitChancePercent())
                {
                    Q.Cast(pred.CastPosition);
                }
            }
        }

        public static void CastQ2(Obj_AI_Base target)
        {
            if (SpellSlot.Q.IsReady() && !IsQ1 && target.IsValidTarget() && target.IsEnemy && QTarget.Distance(Util.MyHero, true) > QTarget.Distance(target, true) && Game.Time - _qArriveTime >= 1.1f)
            {
                Util.MyHero.Spellbook.CastSpell(SpellSlot.Q);
            }
        }

        public static void CastW(Obj_AI_Base target)
        {
            if (SpellSlot.W.IsReady() && target.IsValidTarget(1300) && (target.IsAlly || target.IsMe))
            {
                var pred = W.GetPrediction(target);
                var castPosition = Util.MyHero.Position +
                                   (pred.CastPosition - Util.MyHero.Position).Normalized() *
                                   Math.Min(Util.MyHero.Distance(pred.CastPosition), W.Range);
                Util.MyHero.Spellbook.CastSpell(SpellSlot.W, castPosition);
            }
        }


        public static void Push(Obj_AI_Base target)
        {
            if (SpellSlot.E.IsReady() && target.IsValidTarget(E.Range) && target.IsEnemy)
            {
                var pred = E.GetPrediction(target);
                if (pred.HitChancePercent >= E.Slot.HitChancePercent())
                {
                    E.Cast(pred.CastPosition);
                }
            }
        }

        public static void Pull(Obj_AI_Base target)
        {
            if (SpellSlot.E.IsReady() && target.IsValidTarget(E.Range) && target.IsEnemy)
            {
                var pred = E.GetPrediction(target);
                if (pred.HitChancePercent >= E.Slot.HitChancePercent())
                {
                    Vector3? bestPosition = null;
                    if (!bestPosition.HasValue)
                    {
                        var turret = EntityManager.Turrets.Allies.FirstOrDefault(t => !t.IsDead && Util.MyHero.IsInRange(t, 1000));
                        if (turret != null)
                        {
                            bestPosition = turret.Position;
                        }
                    }
                    if (!bestPosition.HasValue)
                    {
                        if (TargetSelector.Ally != null)
                        {
                            bestPosition = TargetSelector.Ally.Position;
                        }
                    }
                    if (bestPosition.HasValue)
                    {
                        if (EntityManager.Heroes.Allies.HealthPercent(TargetSelector.Range) >= EntityManager.Heroes.Enemies.HealthPercent(TargetSelector.Range))
                        {
                            var info = Util.MyHero.Position.To2D().ProjectOn(pred.CastPosition.To2D(), bestPosition.Value.To2D());
                            var distance = info.SegmentPoint.Distance(Util.MyHero.Position.To2D());
                            if (distance <= E.Width)
                            {
                                Util.MyHero.Spellbook.CastSpell(SpellSlot.E, Util.MyHero.Position + (bestPosition.Value - pred.CastPosition).Normalized() * E.Range);
                                return;
                            }
                        }
                    }
                    var pos = Util.MyHero.Position + (Util.MyHero.Position - pred.CastPosition).Normalized() * E.Range;
                    Util.MyHero.Spellbook.CastSpell(SpellSlot.E, pos);
                }
            }
        }

        public static void CastR(AIHeroClient target)
        {
            if (SpellSlot.R.IsReady() && target.IsValidTarget(R.Range) && target.IsEnemy)
            {
                Util.MyHero.Spellbook.CastSpell(SpellSlot.R);
            }
        }

        public static float HitChancePercent(this SpellSlot s)
        {
            var slot = s.ToString().Trim();
            return Harass.IsActive ? MenuManager.PredictionMenu.GetSliderValue(slot + "Harass") : MenuManager.PredictionMenu.GetSliderValue(slot + "Combo");
        }

        public static bool IsReady(this SpellSlot slot)
        {
            return slot.GetSpellDataInst().IsReady;
        }

        public static SpellDataInst GetSpellDataInst(this SpellSlot slot)
        {
            return Util.MyHero.Spellbook.GetSpell(slot);
        }

        public static float Mana(this SpellSlot slot)
        {
            return slot.GetSpellDataInst().SData.Mana;
        }

        public static bool IsInSmiteRange(this Obj_AI_Base target)
        {
            return target.IsValidTarget() && Util.MyHero.IsInRange(target, Smite.Range + Util.MyHero.BoundingRadius + target.BoundingRadius);
        }
    }
}
