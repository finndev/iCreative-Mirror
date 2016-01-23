using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace Syndra
{
    public static class SpellManager
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot QE;
        public static Spell.Targeted R;
        public static Spell.Targeted Ignite, Smite;
        public static Spell.Skillshot Flash;
        private static float Q_LastCastTime, W_LastCastTime, W_LastSentTime;
        public static float E_LastCastTime;
        private static Vector3 Q_EndPosition, W_EndPosition = Vector3.Zero;
        private static int Q_CastDelay2 = 600;
        private static int Q_Width1 = 180;
        private static int Q_Width2 = 120;
        private static int W_CastDelay2 = 70;
        private static int W_Speed2 = 1100;
        public static int W_Width1 = 210;
        private static int W_Width2 = 160;
        public static int E_ExtraWidth = 40;
        private static int E_CastDelay1 = 300;
        private static int E_CastDelay2 = 250;
        private static int QE_Speed = 2000;
        private static float Combo_QE, Combo_WE;
        private static Obj_AI_Base _wObject;
        private static Obj_AI_Base _weObject;
        private static readonly float QE_Multiplier = 1.5f;

        public static Obj_AI_Base WObject
        {
            get
            {
                if (!IsW2) return null;
                if (_wObject != null)
                {
                    if (_wObject.IsValid && !_wObject.IsDead)
                    {
                        return _wObject;
                    }
                }
                var ball = BallManager.Balls.FirstOrDefault(m => m.IsWObject);
                if (ball != null)
                {
                    _weObject = _wObject;
                    return ball.Object;
                }
                return null;
            }
        }

        public static bool IsW2
        {
            get
            {
                return SpellSlot.W.GetSpellDataInst().SData.Name.ToLower().Equals("syndrawcast") ||
                       Util.MyHero.HasBuff("syndrawtooltip");
            }
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
            Q = new Spell.Skillshot(SpellSlot.Q, 800, SkillShotType.Circular, 620, int.MaxValue, 180)
            {
                AllowedCollisionCount = int.MaxValue
            };
            W = new Spell.Skillshot(SpellSlot.W, 950, SkillShotType.Circular, 0, 1450, 210)
            {
                AllowedCollisionCount = int.MaxValue
            };
            E = new Spell.Skillshot(SpellSlot.E, 700, SkillShotType.Cone, 300, 2500, (int)(45 * 0.5))
            {
                AllowedCollisionCount = int.MaxValue
            };
            QE = new Spell.Skillshot(SpellSlot.E, 1200, SkillShotType.Circular, 0, 2000, 60)
            {
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Targeted(SpellSlot.R, 675);
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
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
            Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffLose;
        }


        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Buff.Name.Equals("syndrawbuff"))
                {
                    _wObject = sender;
                }
            }
        }

        private static void Obj_AI_Base_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Buff.Name.Equals("syndrawbuff"))
                {
                    _wObject = null;
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 800, SkillShotType.Circular, 620, int.MaxValue, 180)
            {
                AllowedCollisionCount = int.MaxValue
            };
            W = new Spell.Skillshot(SpellSlot.W, 950, SkillShotType.Circular, 0, 1450, 210)
            {
                AllowedCollisionCount = int.MaxValue
            };
            QE = new Spell.Skillshot(SpellSlot.E, 1200 - (uint)MenuManager.MiscMenu.GetSliderValue("QE.Range"),
                SkillShotType.Circular, 0, 2000, 60)
            {
                AllowedCollisionCount = int.MaxValue
            };
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                switch (args.Slot)
                {
                    case SpellSlot.Q:
                        Q_EndPosition = args.End;
                        Q_LastCastTime = Game.Time;
                        break;
                    case SpellSlot.W:
                        if (args.SData.Name.ToLower().Equals("syndrawcast"))
                        {
                            W_EndPosition = args.End;
                            W_LastCastTime = Game.Time;
                            Core.DelayAction(delegate { _weObject = null; },
                                W.CastDelay + 1500 * (int)(Util.MyHero.Distance(args.End) / W.Speed));
                        }
                        break;
                    case SpellSlot.E:
                        E_LastCastTime = Game.Time;
                        break;
                }
            }
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.W)
            {
                W_LastSentTime = Game.Time;
            }
            if (Harass.IsActive && !Orbwalker.CanMove && Orbwalker.LastTarget.Type != Util.MyHero.Type)
            {
                args.Process = false;
            }
        }

        public static SpellSlot SpellSlotFromName(this AIHeroClient hero, string name)
        {
            foreach (var s in hero.Spellbook.Spells)
            {
                if (s.Name.ToLower().Contains(name.ToLower()))
                {
                    return s.Slot;
                }
            }
            return SpellSlot.Unknown;
        }

        public static void CastQ(Obj_AI_Base target)
        {
            if (SpellSlot.Q.IsReady() && target.IsValidTarget() && target.IsEnemy)
            {
                if (target is AIHeroClient)
                {
                    Q = new Spell.Skillshot(SpellSlot.Q, 800, SkillShotType.Circular, 620, int.MaxValue, Q_Width2)
                    {
                        AllowedCollisionCount = int.MaxValue
                    };
                }
                var pred = Q.GetPrediction(target);
                if (pred.HitChancePercent >= Q.Slot.HitChancePercent())
                {
                    Q.Cast(pred.CastPosition);
                }
            }
        }

        public static void CastW(Obj_AI_Base target)
        {
            if (SpellSlot.W.IsReady() && target.IsValidTarget(W.Range + W.Width) && target.IsEnemy &&
                Game.Time - W_LastSentTime > 0.25f)
            {
                if (target is AIHeroClient)
                {
                    W = new Spell.Skillshot(SpellSlot.W, 950, SkillShotType.Circular, 0, 1450, W_Width2)
                    {
                        AllowedCollisionCount = int.MaxValue
                    };
                }
                var pred = W.GetPrediction(target);
                if (IsW2)
                {
                    if (pred.HitChancePercent >= W.Slot.HitChancePercent())
                    {
                        Util.MyHero.Spellbook.CastSpell(W.Slot, pred.CastPosition);
                    }
                }
                else if (pred.HitChancePercent >= W.Slot.HitChancePercent() / 2)
                {
                    Obj_AI_Base best = null;
                    {
                        var minion =
                            EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                                m => m.IsValidTarget(W.Range) && m.NetworkId != target.NetworkId)
                                .OrderBy(m => Util.MyHero.Distance(m, true))
                                .FirstOrDefault();
                        if (minion.IsValidTarget())
                        {
                            best = minion;
                        }
                    }
                    if (best == null)
                    {
                        var minion =
                            EntityManager.MinionsAndMonsters.Monsters.Where(
                                m => m.IsValidTarget(W.Range) && m.NetworkId != target.NetworkId)
                                .OrderBy(m => Util.MyHero.Distance(m, true))
                                .FirstOrDefault();
                        if (minion.IsValidTarget())
                        {
                            best = minion;
                        }
                    }
                    if (best == null)
                    {
                        foreach (
                            var b in
                                BallManager.Balls.Where(
                                    m =>
                                        m.IsIdle && Util.MyHero.Distance(m.Position, true) <= Math.Pow(W.Range, 2) &&
                                        !m.EIsOnTime && m.Object.NetworkId != target.NetworkId)
                                    .OrderBy(m => Util.MyHero.Distance(m.Position, true)))
                        {
                            best = b.Object;
                            break;
                        }
                    }
                    if (best != null)
                    {
                        Util.MyHero.Spellbook.CastSpell(W.Slot, best.Position);
                    }
                }
            }
        }

        public static void CastE(Obj_AI_Base target)
        {
            if (SpellSlot.E.IsReady() && target.IsValidTarget() && target.IsEnemy)
            {
                foreach (var b in BallManager.Balls.Where(m => m.IsIdle))
                {
                    CastE2(target, b.Position);
                }
            }
        }

        public static void CastE2(Obj_AI_Base target, Vector3 position)
        {
            if (SpellSlot.E.IsReady() && target.IsValidTarget() && target.IsEnemy)
            {
                if (Util.MyHero.Distance(position, true) <= Math.Pow(E.Range + E_ExtraWidth, 2))
                {
                    var startPosition = position.E_StartPosition().To2D();
                    var endPosition = position.E_EndPosition().To2D();
                    var info = target.ServerPosition.To2D().ProjectOn(startPosition, endPosition);
                    if (info.IsOnSegment &&
                        target.ServerPosition.To2D().Distance(info.SegmentPoint, true) <=
                        Math.Pow(1.8f * (QE.Width + target.BoundingRadius), 2))
                    {
                        QE.Speed = Util.MyHero.Distance(target, true) >= Util.MyHero.Distance(position, true)
                            ? QE_Speed
                            : int.MaxValue;
                        QE.CastDelay = E.CastDelay +
                                       1000 *
                                       (int)
                                           (Math.Min(Util.MyHero.Distance(position), Util.MyHero.Distance(target)) /
                                            E.Speed);
                        QE.SourcePosition = position;
                        var pred = QE.GetPrediction(target);
                        if (pred.HitChancePercent >= QE.Slot.HitChancePercent())
                        {
                            var info2 = pred.CastPosition.To2D().ProjectOn(startPosition, endPosition);
                            if (info2.IsOnSegment &&
                                info2.SegmentPoint.Distance(pred.CastPosition.To2D(), true) <=
                                Math.Pow((QE.Width + target.BoundingRadius) * 0.8f, 2))
                            {
                                Util.MyHero.Spellbook.CastSpell(QE.Slot, pred.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        public static void CastQE(Obj_AI_Base target)
        {
            if (!SpellSlot.E.IsReady() || !target.IsValidTarget(QE.Range + QE.Width) || !target.IsEnemy) return;
            if (SpellSlot.Q.IsReady())
            {
                if (Util.MyHero.Mana >= SpellSlot.Q.Mana() + SpellSlot.E.Mana())
                {
                    if (!target.IsValidTarget(Q.Range))
                    {
                        QE.CastDelay = Q.CastDelay + E.CastDelay;
                        QE.Speed = int.MaxValue;
                        QE.SourcePosition = Util.MyHero.Position;
                        var pred1 = QE.GetPrediction(target);
                        if (pred1.HitChancePercent >= 0 && pred1.CastPosition.Distance(Util.MyHero, true) <= Math.Pow(QE.Range, 2))
                        {
                            QE.Speed = QE_Speed;
                            var startPosition = pred1.CastPosition.E_StartPosition2();
                            QE.SourcePosition = startPosition;
                            QE.CastDelay = Q_CastDelay2 + E.CastDelay +
                                           1000 * (int)(Util.MyHero.Distance(startPosition) / E.Speed);
                            var endPosition = startPosition.E_EndPosition().To2D();
                            var pred2 = QE.GetPrediction(target);
                            var info = pred2.CastPosition.To2D()
                                .ProjectOn(startPosition.To2D(), endPosition);
                            if (pred2.HitChancePercent >= QE.Slot.HitChancePercent() && pred2.CastPosition.Distance(Util.MyHero, true) <= Math.Pow(QE.Range, 2) && info.IsOnSegment && pred2.CastPosition.To2D().Distance(info.SegmentPoint, true) <= Math.Pow(QE_Multiplier * (QE.Width + target.BoundingRadius), 2))
                            {
                                var startPosition2 = Util.MyHero.Position +
                                                     (pred2.CastPosition - Util.MyHero.Position).Normalized() *
                                                     (E.Range + E_ExtraWidth);
                                Util.MyHero.Spellbook.CastSpell(Q.Slot, startPosition2);
                                Combo_QE = Game.Time;
                            }
                        }
                    }
                    else
                    {
                        if (target is AIHeroClient)
                        {
                            Q = new Spell.Skillshot(SpellSlot.Q, 800, SkillShotType.Circular, 620, int.MaxValue, Q_Width2)
                            {
                                AllowedCollisionCount = int.MaxValue
                            };
                        }
                        var pred = Q.GetPrediction(target);
                        if (pred.HitChancePercent >= SpellSlot.Q.HitChancePercent() / 2)
                        {
                            Q.Cast(pred.CastPosition);
                            Combo_QE = Game.Time;
                        }
                    }
                }
            }
            else if (Game.Time - Combo_QE <= 2f * Q.CastDelay / 1000f)
            {
                var timeToArriveQ = Q_CastDelay2 / 1000f + 0.07f + Game.Ping / 2000f - (Game.Time - Q_LastCastTime);
                if (timeToArriveQ >= 0)
                {
                    if (timeToArriveQ <= (Util.MyHero.Distance(Q_EndPosition) / E.Speed) + E_CastDelay2 / 1000f)
                    {
                        CastE2(target, Q_EndPosition);
                    }
                }
            }
        }

        public static void CastWE(Obj_AI_Base target)
        {
            if (!SpellSlot.E.IsReady() || BallManager.Balls.Count <= 0 || !target.IsValidTarget(QE.Range + QE.Width) ||
                !target.IsEnemy)
                return;
            if (SpellSlot.W.IsReady() && Game.Time - W_LastSentTime > 0.25f)
            {
                if (!IsW2 && W.Slot.Mana() + QE.Slot.Mana() <= Util.MyHero.Mana)
                {
                    var best =
                        BallManager.Balls.Where(m => m.IsIdle && !m.EIsOnTime)
                            .OrderBy(
                                m =>
                                    target.Position.To2D()
                                        .ProjectOn(m.Position.To2D(), m.EEndPosition.To2D())
                                        .SegmentPoint.Distance(target.Position.To2D(), true))
                            .LastOrDefault();
                    if (best != null)
                    {
                        Util.MyHero.Spellbook.CastSpell(W.Slot, best.Position);
                    }
                }
                else if (IsW2 && QE.Slot.Mana() <= Util.MyHero.Mana && WObject != null && WObject.IsBall())
                {
                    if (!target.IsValidTarget(W.Range))
                    {
                        QE.CastDelay = W.CastDelay + E.CastDelay;
                        QE.Speed = W.Speed;
                        QE.SourcePosition = Util.MyHero.Position;
                        var pred1 = QE.GetPrediction(target);
                        if (pred1.HitChancePercent >= 0 && pred1.CastPosition.Distance(Util.MyHero, true) <= Math.Pow(QE.Range, 2))
                        {
                            var startPosition = pred1.CastPosition.E_StartPosition2();
                            var endPosition = startPosition.E_EndPosition().To2D();
                            QE.SourcePosition = startPosition;
                            QE.CastDelay = W.CastDelay + E.CastDelay +
                                           1000 *
                                           (int)
                                               (Util.MyHero.Distance(pred1.CastPosition) / W.Speed +
                                                Util.MyHero.Distance(startPosition) / E.Speed);
                            var pred2 = QE.GetPrediction(target);
                            var info = pred2.CastPosition.To2D()
                                .ProjectOn(startPosition.To2D(), endPosition);
                            if (pred2.HitChancePercent >= QE.Slot.HitChancePercent() && pred2.CastPosition.Distance(Util.MyHero, true) <= Math.Pow(QE.Range, 2) && info.IsOnSegment && pred2.CastPosition.To2D().Distance(info.SegmentPoint, true) <= Math.Pow(QE_Multiplier * (QE.Width + target.BoundingRadius), 2))
                            {
                                var startPos = Util.MyHero.Position +
                                               (pred2.CastPosition - Util.MyHero.Position).Normalized() *
                                               (E.Range + E_ExtraWidth);
                                Util.MyHero.Spellbook.CastSpell(W.Slot, startPos);
                                Combo_WE = Game.Time;
                            }
                        }
                    }
                    else
                    {
                        if (target is AIHeroClient)
                        {
                            W = new Spell.Skillshot(SpellSlot.W, 950, SkillShotType.Circular, 0, 1450, W_Width2)
                            {
                                AllowedCollisionCount = int.MaxValue
                            };
                        }
                        var pred = W.GetPrediction(target);
                        if (pred.HitChancePercent >= SpellSlot.W.HitChancePercent() / 2)
                        {
                            Util.MyHero.Spellbook.CastSpell(W.Slot, pred.CastPosition);
                            Combo_WE = Game.Time;
                        }
                    }
                }
            }
            else if (_weObject.IsValidAlly() && _weObject.IsBall() &&
                     Game.Time - Combo_WE <= 2.0f * (W_CastDelay2 / 1000f + _weObject.Distance(W_EndPosition) / W_Speed2))
            {
                float timeToArriveW = W_CastDelay2 / 1000f + 0.07f + Game.Ping / 2000f + (_weObject.Distance(W_EndPosition) / W_Speed2) -
                                      (Game.Time - W_LastCastTime);
                if (timeToArriveW >= 0)
                {
                    if (timeToArriveW <= (W_EndPosition.Distance(Util.MyHero) / E.Speed) + E_CastDelay2 / 1000f)
                    {
                        CastE2(target, W_EndPosition);
                    }
                }
            }
        }

        public static void CastR(AIHeroClient target)
        {
            if (SpellSlot.R.IsReady() && target.IsValidTarget(R.Range) &&
                !MenuManager.MiscMenu.GetCheckBoxValue("Dont.R." + target.ChampionName))
            {
                R.Cast(target);
            }
        }

        public static float HitChancePercent(this SpellSlot s)
        {
            var slot = s.ToString().Trim();
            if (Harass.IsActive)
            {
                return MenuManager.PredictionMenu.GetSliderValue(slot + "Harass");
            }
            return MenuManager.PredictionMenu.GetSliderValue(slot + "Combo");
        }

        public static bool IsReady(this SpellSlot slot)
        {
            return slot.GetSpellDataInst().State == SpellState.Ready;
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
            return target.IsValidTarget(Smite.Range + Util.MyHero.BoundingRadius + target.BoundingRadius);
        }

        public static float SmiteDamage(this Obj_AI_Base target)
        {
            if (!target.IsValidTarget() || !SmiteIsReady) return 0;
            if (target is AIHeroClient)
            {
                if (CanUseSmiteOnHeroes)
                {
                    return Util.MyHero.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Smite);
                }
            }
            else
            {
                var level = Util.MyHero.Level;
                return (new[] { 20 * level + 370, 30 * level + 330, 40 * level + 240, 50 * level + 100 }).Max();
            }
            return 0;
        }
    }
}