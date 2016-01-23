using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace Thresh
{
    public static class Util
    {
        public static float ExtraAaRange = 120f;

        private static readonly string[] P1 =
        {
            "Alistar", "Amumu", "Blitzcrank", "Braum", "ChoGath", "DrMundo", "Garen",
            "Gnar", "Hecarim", "JarvanIV", "Leona", "Lulu", "Malphite", "Nasus", "Nautilus", "Nunu", "Olaf", "Rammus",
            "Renekton", "Sejuani", "Shen", "Shyvana", "Singed", "Sion", "Skarner", "TahmKench", "Taric", "Thresh",
            "Volibear", "Warwick", "MonkeyKing", "Yorick", "Zac"
        };

        private static readonly string[] P2 =
        {
            "Aatrox", "Darius", "Elise", "Evelynn", "Galio", "Gangplank", "Gragas",
            "Irelia", "Jax", "LeeSin", "Maokai", "Morgana", "Nocturne", "Pantheon", "Poppy", "Rengar", "Rumble", "Ryze",
            "Swain", "Trundle", "Tryndamere", "Udyr", "Urgot", "Vi", "XinZhao", "RekSai"
        };

        private static readonly string[] P3 =
        {
            "Akali", "Diana", "Fiddlesticks", "Fiora", "Fizz", "Heimerdinger",
            "Janna", "Jayce", "Kassadin", "Kayle", "KhaZix", "Lissandra", "Mordekaiser", "Nami", "Nidalee", "Riven",
            "Shaco", "Sona", "Soraka", "Vladimir", "Yasuo", "Zilean", "Zyra"
        };

        private static readonly string[] P4 =
        {
            "Ahri", "Anivia", "Annie", "Brand", "Cassiopeia", "Ekko", "Karma",
            "Karthus", "Katarina", "Kennen", "LeBlanc", "Lux", "Malzahar", "MasterYi", "Orianna", "Syndra", "Talon",
            "TwistedFate", "Veigar", "VelKoz", "Viktor", "Xerath", "Zed", "Ziggs"
        };

        private static readonly string[] P5 =
        {
            "Ashe", "Caitlyn", "Corki", "Draven", "Ezreal", "Graves", "Jinx",
            "Kalista", "KogMaw", "Lucian", "MissFortune", "Quinn", "Sivir", "Teemo", "Tristana", "Twitch", "Varus",
            "Vayne"
        };

        public static AIHeroClient MyHero
        {
            get { return ObjectManager.Player; }
        }

        public static Vector3 MousePos
        {
            get { return Game.CursorPos; }
        }

        public static float HealthPercent(this List<AIHeroClient> list, float range)
        {
            return list.Where(h => h.IsValidTarget(range)).Sum(h => h.HealthPercent);
        }

        public static int CountEnemiesInside(this AIHeroClient unit, float range = float.MaxValue)
        {
            return EntityManager.Heroes.Enemies.Count(h => h.IsValidTarget() && unit.IsInRange(h, range));
        }

        public static bool IsInAutoAttackRange(this Obj_AI_Base source, Obj_AI_Base target)
        {
            if (Combo.IsActive || Harass.IsActive)
            {
                return source != null && target != null && source.IsValid && target.IsValid && !source.IsDead &&
                       !target.IsDead &&
                       Math.Pow(source.BoundingRadius + target.BoundingRadius + source.AttackRange + ExtraAaRange, 2) >=
                       source.Distance(target, true);
            }
            return source != null && target != null && source.IsValid && target.IsValid && !source.IsDead &&
                   !target.IsDead &&
                   Math.Pow(source.BoundingRadius + target.BoundingRadius + source.AttackRange, 2) >=
                   source.Distance(target, true);
        }

        public static bool IsInEnemyTurret(this Obj_AI_Base unit)
        {
            if (unit == null || !unit.IsValid || unit.IsDead) return false;
            var turret =
                EntityManager.Turrets.Enemies.FirstOrDefault(
                    m => m != null && m.Health > 0 && m.IsValid && unit.Distance(m, true) <= Math.Pow(750f + 80, 2));
            return turret != null;
        }

        private static Vector3 Source(this Spell.Skillshot s)
        {
            return s.SourcePosition ?? MyHero.Position;
        }

        public static Obj_AI_Base JungleClear(this Spell.Skillshot s, bool useCast = true, int numberOfHits = 1)
        {
            if (!s.IsReady() || numberOfHits <= 0) return null;
            var minions =
                EntityManager.MinionsAndMonsters.GetJungleMonsters(s.Source(), s.Range + s.Width)
                    .OrderBy(m => m.MaxHealth);
            if (!minions.Any() || minions.Count() < numberOfHits) return null;
            switch (s.Type)
            {
                case SkillShotType.Linear:
                    var t = s.GetBestLineTarget(minions.ToList<Obj_AI_Base>());
                    if (t.Item1 >= numberOfHits)
                    {
                        if (useCast)
                        {
                            s.Cast(t.Item2);
                        }
                        return t.Item2;
                    }
                    break;
                case SkillShotType.Circular:
                    var t2 = s.GetBestCircularTarget(minions.ToList<Obj_AI_Base>());
                    if (t2.Item1 < numberOfHits) return null;
                    if (useCast)
                    {
                        s.Cast(t2.Item2);
                    }
                    return t2.Item2;
            }
            return null;
        }

        public static Obj_AI_Base LaneClear(this Spell.Skillshot s, int numberOfHits = 1, bool useCast = true)
        {
            if (!s.IsReady() || numberOfHits <= 0) return null;
            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, s.Source(),
                s.Range + s.Width);
            if (!minions.Any() || minions.Count() < numberOfHits) return null;
            switch (s.Type)
            {
                case SkillShotType.Linear:
                    var t = s.GetBestLineTarget(minions.ToList<Obj_AI_Base>());
                    if (t.Item1 >= numberOfHits)
                    {
                        if (useCast)
                        {
                            s.Cast(t.Item2);
                        }
                        return t.Item2;
                    }
                    break;
                case SkillShotType.Circular:
                    var t2 = s.GetBestCircularTarget(minions.ToList<Obj_AI_Base>());
                    if (t2.Item1 >= numberOfHits)
                    {
                        if (useCast)
                        {
                            s.Cast(t2.Item2);
                        }
                        return t2.Item2;
                    }
                    break;
            }
            return null;
        }

        private static int CountObjectsOnLineSegment(this Spell.Skillshot s, List<Obj_AI_Base> list, Vector3 endPosition)
        {
            return (from obj in list
                    let info = obj.Position.To2D().ProjectOn(s.Source().To2D(), endPosition.To2D())
                    where info.IsOnSegment && info.SegmentPoint.Distance(obj.Position.To2D(), true) <= s.Width
                    select obj).Count();
        }

        public static Tuple<int, Obj_AI_Base> GetBestLineTarget(this Spell.Skillshot s, List<Obj_AI_Base> list)
        {
            Obj_AI_Base bestTarget = null;
            var bestHit = -1;
            var List =
                list.Where(m => m.IsValidTarget() && s.Source().Distance(m, true) <= Math.Pow(s.Range, 2)).ToList();
            foreach (var obj in List)
            {
                var endPosition = s.Source() + (obj.Position - s.Source()).Normalized() * s.Range;
                var hit = s.CountObjectsOnLineSegment(List, endPosition);
                if (hit <= bestHit) continue;
                bestHit = hit;
                bestTarget = obj;
                if (bestHit == List.Count)
                {
                    break;
                }
            }
            return new Tuple<int, Obj_AI_Base>(bestHit, bestTarget);
        }

        private static int CountObjectsNearTo(this Spell.Skillshot s, List<Obj_AI_Base> list, Obj_AI_Base target)
        {
            return list.Count(obj => target.Distance(obj, true) <= Math.Pow(s.Width, 2));
        }

        public static Tuple<int, Obj_AI_Base> GetBestCircularTarget(this Spell.Skillshot s, List<Obj_AI_Base> list)
        {
            Obj_AI_Base bestTarget = null;
            var bestHit = -1;
            var List =
                list.Where(m => m.IsValidTarget() && s.Source().Distance(m, true) <= Math.Pow(s.Range + s.Width, 2))
                    .ToList();
            foreach (var obj in List)
            {
                var hit = s.CountObjectsNearTo(List, obj);
                if (hit <= bestHit) continue;
                bestHit = hit;
                bestTarget = obj;
                if (bestHit == List.Count)
                {
                    break;
                }
            }
            return new Tuple<int, Obj_AI_Base>(bestHit, bestTarget);
        }

        public static Obj_AI_Base LastHit(this Spell.Skillshot s, bool useCast = true)
        {
            if (!s.IsReady()) return null;
            var minions =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, s.Source(),
                    s.Range + s.Width).Where(o => o.Health <= 2.0f * MyHero.GetSpellDamage(o, s.Slot));
            if (!minions.Any()) return null;
            foreach (var minion in minions)
            {
                var canCalculate = false;
                int time;
                float predHealth;
                if (minion.IsValidTarget())
                {
                    if (!Orbwalker.CanAutoAttack)
                    {
                        if (Orbwalker.CanMove && Orbwalker.LastTarget != null &&
                            Orbwalker.LastTarget.NetworkId != minion.NetworkId)
                        {
                            canCalculate = true;
                        }
                    }
                    else
                    {
                        if (MyHero.IsInAutoAttackRange(minion))
                        {
                            canCalculate = true;
                        }
                        else
                        {
                            var speed = MyHero.BasicAttack.MissileSpeed;
                            time =
                                (int)
                                    (1000 * MyHero.Distance(minion) / speed + MyHero.AttackCastDelay * 1000 + Game.Ping - 100);
                            predHealth = Prediction.Health.GetPrediction(minion, time);
                            if (predHealth <= 0)
                            {
                                canCalculate = true;
                            }
                            /**
                                    if (!Orbwalker.CanBeLastHitted(minion))
                                    {
                                        CanCalculate = true;
                                    }**/
                        }
                    }
                }
                if (!canCalculate) continue;
                var dmg = MyHero.GetSpellDamage(minion, s.Slot);
                time = (int)(1000 * s.Source().Distance(minion) / s.Speed + s.CastDelay - 70);
                predHealth = Prediction.Health.GetPrediction(minion, time);
                if (time > 0 && Math.Abs(predHealth - minion.Health) < float.Epsilon)
                {
                }
                else
                {
                    if (!(dmg > predHealth) || !(predHealth > 0)) continue;
                    if (useCast)
                    {
                        s.Cast(minion);
                    }
                    return minion;
                }
            }
            return null;
        }

        public static int GetPriority(this AIHeroClient hero)
        {
            var championName = hero.ChampionName;
            if (P1.Contains(championName))
            {
                return 1;
            }
            if (P2.Contains(championName))
            {
                return 2;
            }
            if (P3.Contains(championName))
            {
                return 3;
            }
            if (P4.Contains(championName))
            {
                return 4;
            }
            return P5.Contains(championName) ? 5 : 1;
        }
    }
}