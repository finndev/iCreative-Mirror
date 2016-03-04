using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace MaddeningJinx
{
    public static class ImmobileTracker
    {
        private const int Width = 100;
        private const string BlitzcrankBuffName = "RocketGrab";
        private const string ThreshBuffName = "ThreshQ";
        private const string ThreshBuffName2 = "threshqfakeknockup";
        private const string ZhonyasBuffName = "zhonyasringshield";
        private const string BardRBuffName = "bardrstasis";
        private const int ThreshKnockupDistance = 430;
        private const string TeleportName = "global_ss_teleport_target_red.troy";
        private static GameObject _lastTeleportObject;
        private static float _lastTeleportTime;
        private static readonly Dictionary<int, float> EnemiesCastEndTime = new Dictionary<int, float>();

        private static readonly Dictionary<AIHeroClient, BuffInstance> BlitzcrankSenders =
            new Dictionary<AIHeroClient, BuffInstance>();

        private static readonly Dictionary<AIHeroClient, Tuple<BuffInstance, Vector3>> ThreshSenders =
            new Dictionary<AIHeroClient, Tuple<BuffInstance, Vector3>>();

        //VPrediction list
        public static readonly Dictionary<EloBuddy.Champion, HashSet<SpellSlot>> ChampionImmobileSlots = new Dictionary
            <EloBuddy.Champion, HashSet<SpellSlot>>
        {
            {EloBuddy.Champion.Blitzcrank, new HashSet<SpellSlot> {SpellSlot.Q, SpellSlot.R}},
            {EloBuddy.Champion.Caitlyn, new HashSet<SpellSlot> {SpellSlot.Q}},
            {EloBuddy.Champion.Cassiopeia, new HashSet<SpellSlot> {SpellSlot.R}},
            {EloBuddy.Champion.Chogath, new HashSet<SpellSlot> {SpellSlot.Q}},
            {EloBuddy.Champion.Ezreal, new HashSet<SpellSlot> {SpellSlot.R}},
            {EloBuddy.Champion.FiddleSticks, new HashSet<SpellSlot> {SpellSlot.W, SpellSlot.R}},
            {EloBuddy.Champion.Galio, new HashSet<SpellSlot> {SpellSlot.R}},
            {EloBuddy.Champion.Janna, new HashSet<SpellSlot> {SpellSlot.R}},
            {EloBuddy.Champion.Jinx, new HashSet<SpellSlot> {SpellSlot.W, SpellSlot.R}},
            {EloBuddy.Champion.Katarina, new HashSet<SpellSlot> {SpellSlot.R}},
            {EloBuddy.Champion.Lux, new HashSet<SpellSlot> {SpellSlot.R}},
            {EloBuddy.Champion.MasterYi, new HashSet<SpellSlot> {SpellSlot.W}},
            {EloBuddy.Champion.Malzahar, new HashSet<SpellSlot> {SpellSlot.R}},
            {EloBuddy.Champion.MissFortune, new HashSet<SpellSlot> {SpellSlot.R}},
            {EloBuddy.Champion.Nunu, new HashSet<SpellSlot> {SpellSlot.Q, SpellSlot.R}},
            {EloBuddy.Champion.Shen, new HashSet<SpellSlot> {SpellSlot.R}},
            {EloBuddy.Champion.Thresh, new HashSet<SpellSlot> {SpellSlot.Q, SpellSlot.R}},
            {EloBuddy.Champion.Velkoz, new HashSet<SpellSlot> {SpellSlot.R}},
            {EloBuddy.Champion.Warwick, new HashSet<SpellSlot> {SpellSlot.R}}
        };

        private static bool _containsThresh;
        private static bool _containsBard;

        public static void Initialize()
        {
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                EnemiesCastEndTime[enemy.NetworkId] = 0f;
            }
            Game.OnTick += Game_OnTick;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
            Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffLose;
            _containsThresh = EntityManager.Heroes.Allies.Any(h => h.Hero == EloBuddy.Champion.Thresh);
            _containsBard = EntityManager.Heroes.AllHeroes.Any(h => h.Hero == EloBuddy.Champion.Bard);
            AddRTracker(EloBuddy.Champion.TwistedFate, "GateMarker_red.troy", 700); //2 seconds
            AddRTracker(EloBuddy.Champion.Pantheon, "Pantheon_Base_R_indicator_red.troy", 1700); //3 seconds
            AddRTracker(EloBuddy.Champion.TahmKench, "TahmKench_Base_R_Target_Enemy.troy", 2700); //4 seconds
        }


        public static void AddRTracker(EloBuddy.Champion hero, string rObjectName, int timeToWait)
        {
            if (EntityManager.Heroes.Enemies.Any(h => h.Hero == hero))
            {
                GameObject rObject = null;
                var rObjectTime = 0;
                var allyCastedR = false;
                Game.OnTick += delegate
                {
                    if (SpellSlot.E.IsReady())
                    {
                        if (rObject != null)
                        {
                            if (rObjectTime > 0 && Core.GameTickCount - rObjectTime >= timeToWait &&
                                Core.GameTickCount - rObjectTime <= 4000)
                            {
                                if (MenuManager.GetSubMenu("Automatic").CheckBox("E.Spells") || Combo.IsActive)
                                {
                                    SpellManager.E.Cast(rObject.Position);
                                }
                            }
                        }
                    }
                };
                Obj_AI_Base.OnProcessSpellCast += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
                {
                    var senderHero = sender as AIHeroClient;
                    if (senderHero != null && senderHero.IsAlly && senderHero.Hero == hero)
                    {
                        if (args.Slot == SpellSlot.R)
                        {
                            allyCastedR = true;
                            Core.DelayAction(delegate { allyCastedR = false; }, 10000);
                        }
                    }
                };
                GameObject.OnCreate += delegate (GameObject sender, EventArgs args)
                {
                    if (sender.Name.Equals(rObjectName) && !allyCastedR)
                    {
                        rObject = sender;
                        rObjectTime = Core.GameTickCount;
                        Core.DelayAction(delegate
                        {
                            rObject = null;
                            rObjectTime = 0;
                        }, 5200);
                    }
                };
                GameObject.OnDelete += delegate (GameObject sender, EventArgs args)
                {
                    if (rObject != null && rObject.IdEquals(sender))
                    {
                        //rObject = null;
                        //rObjectTime = 0;
                    }
                };
            }
        }

        public static float GetETime()
        {
            return SpellManager.E.CastDelay / 1000f;
        }

        private static float GetWTime(Obj_AI_Base target)
        {
            return SpellManager.W.CastDelay / 1000f + Util.MyHero.Distance(target) / SpellManager.W.Speed;
        }
        

        private static void Game_OnTick(EventArgs args)
        {
            if (SpellSlot.E.IsReady())
            {
                foreach (var b in BlitzcrankSenders.ToArray().Where(b => !b.Key.IsValidTarget() || !b.Value.IsValid))
                {
                    BlitzcrankSenders.Remove(b.Key);
                }
                foreach (var b in ThreshSenders.ToArray().Where(b => !b.Key.IsValidTarget() || !b.Value.Item1.IsValid))
                {
                    ThreshSenders.Remove(b.Key);
                }
                if (MenuManager.GetSubMenu("Automatic").CheckBox("E.CC") || Combo.IsActive)
                {
                    foreach (
                        var enemy in
                            MyTargetSelector.ValidEnemiesInRange.Where(enemy => enemy.WillBeImmobileByBuff(GetETime()) && !BlitzcrankSenders.ContainsKey(enemy) && !ThreshSenders.ContainsKey(enemy)))
                    {
                        if (_containsThresh)
                        {
                            if (!enemy.HasBuff(ThreshBuffName2))
                            {
                                SpellManager.E.Cast(enemy.Position);
                            }
                        }
                        else
                        {
                            SpellManager.E.Cast(enemy.Position);
                        }
                    }
                    foreach (var blitz in BlitzcrankSenders.Values.Select(tuple => EntityManager.Heroes.Allies.FirstOrDefault(h => h.IsValidTarget() && h.ChampionName.Equals(tuple.SourceName))).Where(blitz => blitz != null))
                    {
                        SpellManager.E.Cast(blitz.ServerPosition);
                    }
                    foreach (var castPosition in from tuple in ThreshSenders let thresh = EntityManager.Heroes.Allies.FirstOrDefault(h => h.IsValidTarget() && h.ChampionName.Equals(tuple.Value.Item1.SourceName)) where thresh != null let startPosition = tuple.Value.Item2 select thresh.Position + (startPosition - thresh.Position).Normalized() * Math.Max(thresh.Distance(startPosition) - ThreshKnockupDistance, 1f))
                    {
                        SpellManager.E.Cast(castPosition);
                    }
                }
                if (MenuManager.GetSubMenu("Automatic").CheckBox("E.Spells") || Combo.IsActive)
                {
                    foreach (
                        var enemy in
                            MyTargetSelector.ValidEnemiesInRange.Where(enemy => enemy.WillBeImmobileBySpell(GetETime()))
                        )
                    {
                        SpellManager.E.Cast(enemy.Position);
                    }
                    if (_lastTeleportObject != null && _lastTeleportTime > 0 &&
                        Core.GameTickCount - _lastTeleportTime >= 2700 && Core.GameTickCount - _lastTeleportTime <= 4000)
                    //4 seconds
                    {
                        SpellManager.E.Cast(_lastTeleportObject.Position);
                    }
                    foreach (var enemy in EntityManager.Heroes.Enemies.Where(h => h.IsValid && !h.IsDead && h.HasBuff(ZhonyasBuffName)))
                    {
                        var buff = enemy.GetBuff(ZhonyasBuffName);
                        if (buff.EndTime - Game.Time <= 1.3f && buff.EndTime - buff.StartTime > 0f)
                        {
                            SpellManager.E.Cast(enemy.Position);
                        }
                    }
                    if (_containsBard)
                    {
                        foreach (var enemy in EntityManager.Heroes.Enemies.Where(h => h.IsValid && !h.IsDead && h.HasBuff(BardRBuffName)))
                        {
                            var buff = enemy.GetBuff(BardRBuffName);
                            if (buff.EndTime - Game.Time <= 1.3f && buff.EndTime - buff.StartTime > 0f)
                            {
                                SpellManager.E.Cast(enemy.Position);
                            }
                        }
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var hero = sender as AIHeroClient;
            if (hero != null && hero.IsEnemy)
            {
                if (!EnemiesCastEndTime.ContainsKey(hero.NetworkId))
                {
                    EnemiesCastEndTime[hero.NetworkId] = 0f;
                }
                var time = Game.Time + Math.Abs(args.SData.CastTime) +
                           (MenuManager.ImmobileSpellsMenu != null &&
                            MenuManager.ImmobileSpellsHashSet.Contains(hero.ChampionName + args.Slot) &&
                            MenuManager.ImmobileSpellsMenu.CheckBox(hero.ChampionName + args.Slot)
                               ? 1.2f
                               : 0f);
                if (time - Game.Time > 0)
                {
                    if (EnemiesCastEndTime[hero.NetworkId] < time)
                    {
                        EnemiesCastEndTime[hero.NetworkId] = time;
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            var senderHero = sender as AIHeroClient;
            var casterHero = args.Buff.Caster as AIHeroClient;
            if (senderHero != null && senderHero.IsEnemy)
            {
                if (casterHero != null && casterHero.IsAlly && EntityManager.Heroes.Allies.Any(h => h.ChampionName.Equals(args.Buff.SourceName)))
                {
                    if (args.Buff.SourceName.Equals("Blitzcrank") && args.Buff.DisplayName.Equals(BlitzcrankBuffName))
                    {
                        if (BlitzcrankSenders.ContainsKey(senderHero))
                        {
                            BlitzcrankSenders.Remove(senderHero);
                        }
                        BlitzcrankSenders.Add(senderHero, args.Buff);
                        Core.DelayAction(delegate
                        {
                            if (BlitzcrankSenders.ContainsKey(senderHero))
                            {
                                BlitzcrankSenders.Remove(senderHero);
                            }
                        }, (int)(1000 * (args.Buff.EndTime - args.Buff.StartTime + 0.1f)));
                    }
                    else if (args.Buff.SourceName.Equals("Thresh") && args.Buff.DisplayName.Equals(ThreshBuffName))
                    {
                        if (ThreshSenders.ContainsKey(senderHero))
                        {
                            ThreshSenders.Remove(senderHero);
                        }
                        ThreshSenders.Add(senderHero, new Tuple<BuffInstance, Vector3>(args.Buff, senderHero.ServerPosition));
                        Core.DelayAction(delegate
                        {
                            if (ThreshSenders.ContainsKey(senderHero))
                            {
                                ThreshSenders.Remove(senderHero);
                            }
                        }, (int)(1000 * (args.Buff.EndTime - args.Buff.StartTime + 0.1f)));
                    }
                }
            }
        }

        private static void Obj_AI_Base_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            var senderHero = sender as AIHeroClient;
            var casterHero = args.Buff.Caster as AIHeroClient;
            if (senderHero != null && senderHero.IsEnemy && casterHero != null && casterHero.IsAlly)
            {
                if (args.Buff.SourceName.Equals("Blitzcrank") && args.Buff.DisplayName.Equals(BlitzcrankBuffName))
                {
                    if (BlitzcrankSenders.ContainsKey(senderHero))
                    {
                        BlitzcrankSenders.Remove(senderHero);
                    }
                }
                else if (args.Buff.SourceName.Equals("Thresh") && args.Buff.DisplayName.Equals(ThreshBuffName))
                {
                    if (ThreshSenders.ContainsKey(senderHero))
                    {
                        ThreshSenders.Remove(senderHero);
                    }
                }
            }
        }
        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.Equals(TeleportName))
            {
                _lastTeleportObject = sender;
                _lastTeleportTime = Core.GameTickCount;
                Core.DelayAction(delegate
                {
                    _lastTeleportObject = null;
                    _lastTeleportTime = 0;
                }, 5200);
            }
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.IdEquals(_lastTeleportObject))
            {
                //_lastTeleportObject = null;
                //_lastTeleportTime = 0;
            }
        }

        public static bool WillBeHittedByR(this Obj_AI_Base target)
        {
            var time = target.GetUltimateTravelTime();
            return target.WillBeImmobile(time) || target.WillBeSlowed(time);
        }

        private static bool WillBeImmobileByBuff(this Obj_AI_Base target, float time)
        {
            var buffDuration = target.GetMovementBlockedDebuffDuration();
            return buffDuration > 0 && time <= buffDuration + (Width + target.BoundingRadius) / target.MoveSpeed;
        }

        private static bool WillBeImmobileBySpell(this Obj_AI_Base target, float time)
        {
            if (!EnemiesCastEndTime.ContainsKey(target.NetworkId))
            {
                EnemiesCastEndTime[target.NetworkId] = 0f;
            }
            var spellDuration = EnemiesCastEndTime[target.NetworkId] - Game.Time;
            return spellDuration > 0 && time <= spellDuration + (Width + target.BoundingRadius) / target.MoveSpeed;
        }

        private static bool WillBeImmobile(this Obj_AI_Base target, float time)
        {
            return target.WillBeImmobileByBuff(time) || target.WillBeImmobileBySpell(time);
        }

        private static bool WillBeSlowed(this Obj_AI_Base target, float time)
        {
            var buffDuration = target.GetMovementReducedDebuffDuration();
            if (buffDuration > 0)
            {
                return true;
            }
            if (Core.GameTickCount - SpellManager.WLastCastTime <= 600 || SpellManager.WMissile != null)
            {
                var pred = SpellManager.W.GetPrediction(target);
                var timeToArriveW = (pred.CastPosition.Distance(SpellManager.W.Source()) - target.BoundingRadius) /
                                    SpellManager.W.Speed + SpellManager.W.CastDelay / 1000f;
                return timeToArriveW <= time && pred.HitChancePercent >= MenuManager.Menu.Slider("Prediction.W") &&
                       pred.CastPosition.To2D()
                           .Distance(SpellManager.WStartPosition.To2D(), SpellManager.WEndPosition.To2D(), false, true) <=
                       (SpellManager.W.Width + target.BoundingRadius).Pow();
            }
            return false;
        }
    }
}