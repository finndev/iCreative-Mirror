using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace Syndra
{
    public static class Util
    {
        public static float ExtraAaRange = 120f;
        public static AIHeroClient MyHero
        {
            get { return Player.Instance; }
        }

        public static Vector3 MousePos
        {
            get { return Game.CursorPos; }
        }

        public static bool IsValidAlly(this AttackableUnit unit, float range = float.MaxValue)
        {
            return unit != null && unit.IsValid && !unit.IsDead && MyHero.Distance(unit, true) <= Math.Pow(range, 2);
        }
        public static bool IsInAutoAttackRange(this Obj_AI_Base source, Obj_AI_Base target)
        {
            if (Combo.IsActive || Harass.IsActive)
            {
                return source != null && target != null && source.IsValid && target.IsValid && !source.IsDead && !target.IsDead && Math.Pow(source.BoundingRadius + target.BoundingRadius + source.AttackRange + ExtraAaRange, 2) >= source.Distance(target, true);
            }
            return source != null && target != null && source.IsValid && target.IsValid && !source.IsDead && !target.IsDead && Math.Pow(source.BoundingRadius + target.BoundingRadius + source.AttackRange, 2) >= source.Distance(target, true);
        }
        public static bool IsInEnemyTurret(this Obj_AI_Base unit)
        {
            if (unit == null || !unit.IsValid || unit.IsDead) return false;
            var turret = EntityManager.Turrets.Enemies.FirstOrDefault(m => m != null && m.Health > 0 && m.IsValid && unit.Distance(m, true) <= Math.Pow(750f + 80, 2));
            return turret != null;
        }
        private static Vector3 Source(this Spell.Skillshot s)
        {
            return s.SourcePosition ?? MyHero.Position;
        }
        public static Obj_AI_Base JungleClear(this Spell.Skillshot s, bool useCast = true, int numberOfHits = 1)
        {
            if (!s.IsReady() || numberOfHits <= 0) return null;
            var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(s.Source(), s.Range + s.Width).OrderBy(m => m.MaxHealth);
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
            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, s.Source(), s.Range + s.Width);
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
            return (from obj in list let info = obj.Position.To2D().ProjectOn(s.Source().To2D(), endPosition.To2D()) where info.IsOnSegment && Extensions.Distance(info.SegmentPoint, obj.Position.To2D(), true) <= s.Width select obj).Count();
        }

        public static Tuple<int, Obj_AI_Base> GetBestLineTarget(this Spell.Skillshot s, List<Obj_AI_Base> list)
        {
            Obj_AI_Base bestTarget = null;
            var bestHit = -1;
            var List = list.Where(m => m.IsValidTarget() && s.Source().Distance(m, true) <= Math.Pow(s.Range, 2)).ToList();
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
            var List = list.Where(m => m.IsValidTarget() && s.Source().Distance(m, true) <= Math.Pow(s.Range + s.Width, 2)).ToList();
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
            foreach (var minion in from minion in Orbwalker.UnKillableMinionsList let predHealth = Prediction.Health.GetPrediction(minion, (int)(1000f * s.Source().Distance(minion) / s.Speed) + s.CastDelay) where predHealth >= 0 where s.Slot.GetSpellDamage(minion) >= predHealth select minion)
            {
                s.Cast(minion);
                return minion;
            }
            /*
            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, s.Source(), s.Range + s.Width).Where(o => o.Health <= 2.0f * s.Slot.GetSpellDamage(o));
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
                        if (Orbwalker.CanMove && Orbwalker.LastTarget != null && Orbwalker.LastTarget.NetworkId != minion.NetworkId)
                        {
                            canCalculate = true;
                        }
                    }
                    else
                    {
                        if (!MyHero.IsInAutoAttackRange(minion))
                        {
                            canCalculate = true;
                        }
                        else
                        {
                            var speed = MyHero.BasicAttack.MissileSpeed;
                            time = (int)(1000 * MyHero.Distance(minion) / speed + MyHero.AttackCastDelay * 1000 + Game.Ping - 100);
                            predHealth = Prediction.Health.GetPrediction(minion, time);
                            if (predHealth <= -20)
                            {
                                canCalculate = true;
                            }
                        }
                    }
                }
                if (!canCalculate) continue;
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
                }
            }*/
            return null;
        }
        public static int GetPriority(this AIHeroClient hero)
        {
            var championName = hero.ChampionName;
            string[] p1 =
            {
                "Alistar", "Amumu", "Bard", "Blitzcrank", "Braum", "Cho'Gath", "Dr. Mundo", "Garen", "Gnar",
                "Hecarim", "Janna", "Jarvan IV", "Leona", "Lulu", "Malphite", "Nami", "Nasus", "Nautilus", "Nunu",
                "Olaf", "Rammus", "Renekton", "Sejuani", "Shen", "Shyvana", "Singed", "Sion", "Skarner", "Sona",
                "Soraka", "Taric", "Thresh", "Volibear", "Warwick", "MonkeyKing", "Yorick", "Zac", "Zyra"
            };

            string[] p2 =
            {
                "Aatrox", "Darius", "Elise", "Evelynn", "Galio", "Gangplank", "Gragas", "Irelia", "Jax",
                "Lee Sin", "Maokai", "Morgana", "Nocturne", "Pantheon", "Poppy", "Rengar", "Rumble", "Ryze", "Swain",
                "Trundle", "Tryndamere", "Udyr", "Urgot", "Vi", "XinZhao", "RekSai"
            };

            string[] p3 =
            {
                "Akali", "Diana", "Ekko", "Fiddlesticks", "Fiora", "Fizz", "Heimerdinger", "Jayce", "Kassadin",
                "Kayle", "Kha'Zix", "Lissandra", "Mordekaiser", "Nidalee", "Riven", "Shaco", "Vladimir", "Yasuo",
                "Zilean"
            };

            string[] p4 =
            {
                "Ahri", "Anivia", "Annie", "Ashe", "Azir", "Brand", "Caitlyn", "Cassiopeia", "Corki", "Draven",
                "Ezreal", "Graves", "Jinx", "Kalista", "Karma", "Karthus", "Katarina", "Kennen", "KogMaw", "Leblanc",
                "Lucian", "Lux", "Malzahar", "MasterYi", "MissFortune", "Orianna", "Quinn", "Sivir", "Syndra", "Talon",
                "Teemo", "Tristana", "TwistedFate", "Twitch", "Varus", "Vayne", "Veigar", "VelKoz", "Viktor", "Xerath",
                "Zed", "Ziggs"
            };

            if (p1.Contains(championName))
            {
                return 1;
            }
            if (p2.Contains(championName))
            {
                return 2;
            }
            if (p3.Contains(championName))
            {
                return 3;
            }
            return p4.Contains(championName) ? 4 : 1;
        }
    }
}
