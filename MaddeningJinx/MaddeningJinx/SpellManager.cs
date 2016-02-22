using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace MaddeningJinx
{
    public static class SpellManager
    {
        private const float InitialSpeed = 1700;
        private const float ChangerSpeedDistance = 1350;
        private const float FinalSpeed = 2200;
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        public static Spell.Targeted Ignite, Smite;
        public static Spell.Skillshot Flash;
        public static readonly float QWidth = 180f;
        public static readonly float QWidthSqr = QWidth * QWidth;
        public static int WLastCastTime;
        public static int WCastSpellTime;
        public static Vector3 WStartPosition;
        public static Vector3 WEndPosition;
        public static MissileClient WMissile;
        public static int RLastCastTime;
        public static int RCastSpellTime;
        public static MissileClient RMissile;
        public const int RRange = 3500;
        public const int RRangeSqr = RRange * RRange;
        public static int QCastSpellTime;

        public static void Initialize()
        {
            W = new Spell.Skillshot(SpellSlot.W, 1500, SkillShotType.Linear, 600, 3300, 60)
            {
                AllowedCollisionCount = 0
            };
            E = new Spell.Skillshot(SpellSlot.E, 900, SkillShotType.Circular, 1200, int.MaxValue, 120)
            {
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Skillshot(SpellSlot.R, 20000, SkillShotType.Linear, 600, 1700, 140)
            {
                AllowedCollisionCount = int.MaxValue
            };
            Game.OnTick += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe)
            {
                if (args.Slot == SpellSlot.Q)
                {
                    QCastSpellTime = Core.GameTickCount;
                }
                else if (args.Slot == SpellSlot.W)
                {
                    args.Process = Core.GameTickCount - Champion.ShouldWaitTime > W.CastDelay && Orbwalker.CanMove;
                    if (args.Process)
                    {
                        WCastSpellTime = Core.GameTickCount;
                    }
                }
                else if (args.Slot == SpellSlot.R)
                {
                    if (args.Process)
                    {
                        RCastSpellTime = Core.GameTickCount;
                    }
                }
            }
        }

        internal static void CastQ()
        {
            if (SpellSlot.Q.IsReady() && Orbwalker.CanMove && Core.GameTickCount - QCastSpellTime > 5000)
            {
                Util.MyHero.Spellbook.CastSpell(SpellSlot.Q, true);
            }
        }

        internal static void CastW(Obj_AI_Base target)
        {
            if (W.IsReady() && Orbwalker.CanMove && Util.MyHero.IsInRange(target, W.Range + W.Width))
            {
                if (Core.GameTickCount - RLastCastTime <= R.CastDelay || Core.GameTickCount - RCastSpellTime < 100 || (RMissile != null && Util.MyHero.Distance(RMissile, true) <= Util.MyHero.Distance(target, true)))
                {
                    return;
                }
                if (Util.MyHero.CountEnemiesInRange(400) != 0)
                {
                    return;
                }
                var pred = W.GetPrediction(target);
                if (pred.HitChancePercent >= MenuManager.Menu.Slider("Prediction.W"))
                {
                    W.Cast(pred.CastPosition);
                }
            }
        }

        internal static void CastE(Obj_AI_Base target)
        {
            if (E.IsReady())
            {
                E.CastDelay = 1200;
                var pred = E.GetPrediction(target);
                if (pred.HitChance > HitChance.High)
                {
                    E.Cast(pred.CastPosition);
                }
            }
        }
        internal static void CastESlowed(Obj_AI_Base target)
        {
            if (E.IsReady() && Util.MyHero.IsInRange(target, E.Range + E.Width + target.BoundingRadius / 2f))
            {
                var debuffTime = target.GetMovementReducedDebuffDuration();
                if (debuffTime >= E.CastDelay / 1000f)
                {
                    var pred = E.GetPrediction(target);
                    if (pred.HitChance >= HitChance.High)
                    {
                        if (target.MoveSpeed * E.CastDelay / 1000f <= E.Width + target.BoundingRadius / 2f)
                        {
                            E.Cast(pred.CastPosition);
                        }
                    }
                }
            }
        }

        internal static void CastAoeE(int minHit = 3)
        {
            if (!E.IsReady())
            {
                return;
            }
            var enemiesInRange = MyTargetSelector.ValidEnemiesInRange.Where(client => Util.MyHero.IsInRange(client, E.Range + E.Width)).ToList();
            if (enemiesInRange.Count < minHit)
            {
                return;
            }
            var points = (from enemy in enemiesInRange let pred = E.GetPrediction(enemy) where pred.HitChance >= HitChance.High select new Tuple<Vector2, float>(pred.CastPosition.To2D(), enemy.BoundingRadius)).ToList();
            if (points.Count < minHit)
            {
                return;
            }
            var bestTuple = new Tuple<Vector2, float>(Vector2.Zero, -1f);
            foreach (var t in points)
            {
                if (points.Count(p => t.Item1.IsInRange(p.Item1, 3f * E.Width + p.Item2 + t.Item2)) >= bestTuple.Item2)
                {
                    bestTuple = t;
                    if (Math.Abs(bestTuple.Item2 - points.Count) < float.Epsilon)
                    {
                        break;
                    }
                }
            }
            if (bestTuple.Item2 >= minHit)
            {
                E.Cast(bestTuple.Item1.To3DWorld());
            }
        }
        internal static float GetUltimateTravelTime(this Obj_AI_Base target)
        {
            var distance = Vector3.Distance(Util.MyHero.ServerPosition, target.ServerPosition);
            if (distance >= ChangerSpeedDistance)
            {
                /*
                //BASEULT ++
                var missilespeed = InitialSpeed;
                var acceldifference = distance - InitialDistance;
                if (acceldifference > 150f)
                {
                    acceldifference = 150f;
                }

                var difference = distance - 1500f;
                missilespeed = (1350f * InitialSpeed + acceldifference * (InitialSpeed + Accelerationrate * acceldifference) +
                                difference * FinalSpeed) / distance;
                */
                return ChangerSpeedDistance / InitialSpeed + (distance - ChangerSpeedDistance) / FinalSpeed +
                       R.CastDelay / 1000f;
            }
            return InitialSpeed * distance + R.CastDelay / 1000f;
        }

        internal static void CheckRKillable(Obj_AI_Base target)
        {
            KillSteal.RDamageOnEnemies[target.NetworkId] = SpellSlot.R.GetSpellDamage(target);
            if (target.TotalShieldHealth() + target.HPRegenRate * 2 <= KillSteal.RDamageOnEnemies[target.NetworkId])
            {
                var distance = Vector3.Distance(Util.MyHero.Position, target.Position);
                R.Speed = (int)InitialSpeed;
                if (distance >= ChangerSpeedDistance)
                {
                    var travelTime = ChangerSpeedDistance / InitialSpeed +  (distance - ChangerSpeedDistance) / FinalSpeed;
                    R.Speed = (int)(distance / travelTime);
                }
                var pred = R.GetPrediction(target);
                var firstHit =
                    pred.GetCollisionObjects<AIHeroClient>()
                        .OrderBy(h => Util.MyHero.Distance(h, true))
                        .FirstOrDefault() ?? target;
                KillSteal.RDamageOnEnemies[target.NetworkId] = SpellSlot.R.GetSpellDamage(target,
                    firstHit.IdEquals(target) ? 1 : 2);
                if (target.TotalShieldHealth() + target.HPRegenRate * 2 <= KillSteal.RDamageOnEnemies[target.NetworkId] && Util.MyHero.Distance(target, true) <= RRangeSqr)
                {
                    KillSteal.RKillableBases.Add(target);
                    if (pred.HitChance >= HitChance.High &&
                        firstHit.Distance(target, true) <= (225 + target.BoundingRadius).Pow())
                    {
                        if ((KillSteal.Menu.CheckBox("R") && target.WillBeHittedByR() &&
                             !MyTargetSelector.PowPowTarget.IdEquals(target) && target.CountAlliesInRange(500) == 0) || MenuManager.TapKeyPressed)
                        {
                            KillSteal.RHittableBases.Add(pred.CastPosition);
                        }
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            E.CastDelay = 1200;
            if (WMissile != null)
            {
                W.CastDelay = 0;
                W.SourcePosition = WMissile.Position;
            }
            else
            {
                if (Core.GameTickCount - WLastCastTime <= 600)
                {
                    W.CastDelay = 600 - (Core.GameTickCount - WLastCastTime);
                }
                else
                {
                    W.CastDelay = 600;
                }
                W.SourcePosition = Util.MyHero.ServerPosition;
            }
            if (MenuManager.TapKeyPressed && !SpellSlot.R.IsReady())
            {
                MenuManager.TapKeyPressed = false;
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (missile != null && missile.SpellCaster != null && missile.SpellCaster.IsMe)
            {
                if (missile.SData.Name.Equals("JinxWMissile"))
                {
                    WMissile = missile;
                    Core.DelayAction(delegate { WMissile = null; }, (int)(1000 * missile.SpellCaster.Distance(missile.EndPosition) / W.Speed) + 200);
                }
                if (missile.SData.Name.Equals("JinxR"))
                {
                    RMissile = missile;
                }
            }
        }
        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (missile != null)
            {
                if (missile.IdEquals(WMissile))
                {
                    WMissile = null;
                }
                if (missile.IdEquals(RMissile))
                {
                    RMissile = null;
                }
            }
        }
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                switch (args.Slot)
                {
                    case SpellSlot.Q:
                    case SpellSlot.W:
                        if (args.SData.Name.Equals("JinxW"))
                        {
                            WLastCastTime = Core.GameTickCount;
                            WStartPosition = Util.MyHero.ServerPosition;
                            WEndPosition = args.End;
                        }
                        break;
                    case SpellSlot.E:
                    case SpellSlot.R:
                        RLastCastTime = Core.GameTickCount;
                        break;
                }
            }
        }
    }
}