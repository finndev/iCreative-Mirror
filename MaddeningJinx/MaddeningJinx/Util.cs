using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace MaddeningJinx
{
    public static class Util
    {
        public static float ExtraAaRange = 120f;

        public static bool SmiteIsReady
        {
            get { return SpellManager.Smite != null && SpellManager.Smite.IsReady(); }
        }

        public static bool CanUseSmiteOnHeroes
        {
            get
            {
                if (!SmiteIsReady) return false;
                var name = SpellManager.Smite.Slot.GetSpellDataInst().SData.Name.ToLower();
                return name.Contains("smiteduel") || name.Contains("smiteplayerganker");
            }
        }

        public static bool IgniteIsReady
        {
            get { return SpellManager.Ignite != null && SpellManager.Ignite.IsReady(); }
        }

        public static bool FlashIsReady
        {
            get { return SpellManager.Flash != null && SpellManager.Flash.IsReady(); }
        }

        public static AIHeroClient MyHero
        {
            get { return Player.Instance; }
        }

        public static Vector3 MousePos
        {
            get { return Game.CursorPos; }
        }

        public static void Initialize()
        {
            var slot = MyHero.SpellSlotFromName("summonerdot");
            if (slot != SpellSlot.Unknown)
            {
                SpellManager.Ignite = new Spell.Targeted(slot, 600);
            }
            slot = MyHero.SpellSlotFromName("smite");
            if (slot != SpellSlot.Unknown)
            {
                SpellManager.Smite = new Spell.Targeted(slot, 500);
            }
            slot = MyHero.SpellSlotFromName("flash");
            if (slot != SpellSlot.Unknown)
            {
                SpellManager.Flash = new Spell.Skillshot(slot, 400, SkillShotType.Circular);
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

        public static float HitChancePercent(this SpellSlot s)
        {
            var slot = s.ToString().Trim();
            return Harass.IsActive
                ? MenuManager.PredictionMenu.Slider(slot + ".Harass")
                : MenuManager.PredictionMenu.Slider(slot + ".Combo");
        }

        public static bool IsReady(this SpellSlot slot)
        {
            return slot.GetSpellDataInst().State == SpellState.Ready;
        }

        public static SpellDataInst GetSpellDataInst(this SpellSlot slot)
        {
            return MyHero.Spellbook.GetSpell(slot);
        }

        public static float Mana(this SpellSlot slot)
        {
            return slot.GetSpellDataInst().SData.Mana;
        }

        public static bool IsLearned(this SpellSlot slot)
        {
            return slot.GetSpellDataInst().IsLearned;
        }

        public static bool IsInSmiteRange(this Obj_AI_Base target)
        {
            return MyHero.IsInRange(target, SpellManager.Smite.Range + MyHero.BoundingRadius + target.BoundingRadius);
        }

        public static float SmiteDamage(this Obj_AI_Base target)
        {
            if (!SmiteIsReady) return 0;
            if (target is AIHeroClient)
            {
                if (CanUseSmiteOnHeroes)
                {
                    return MyHero.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Smite);
                }
            }
            else
            {
                var level = MyHero.Level;
                return (new[] {20*level + 370, 30*level + 330, 40*level + 240, 50*level + 100}).Max();
            }
            return 0;
        }

        public static int CountMinionsInRange(this Obj_AI_Minion minion, IEnumerable<Obj_AI_Base> list, float range)
        {
            return list.Count(m => minion.IsInRange(m, range));
        }

        public static bool IsInAutoAttackRange(this Obj_AI_Base source, Obj_AI_Base target)
        {
            if (Combo.IsActive || Harass.IsActive)
            {
                return source.IsValidTarget() && target.IsValidTarget() &&
                       source.IsInRange(target,
                           source.BoundingRadius + target.BoundingRadius + source.AttackRange + ExtraAaRange);
            }
            return Extensions.IsInAutoAttackRange(source, target);
        }

        public static bool IsInPowPowRange(this AttackableUnit target)
        {
            return MyHero.IsInRange(target, Champion.GetPowPowRange(target));
        }

        public static bool IsInFishBonesRange(this AttackableUnit target)
        {
            return MyHero.IsInRange(target, Champion.GetFishBonesRange(target));
        }

        public static bool IsInEnemyTurret(this Obj_AI_Base unit)
        {
            if (unit == null || !unit.IsValid || unit.IsDead) return false;
            return
                EntityManager.Turrets.Enemies.Any(m => m.IsInAutoAttackRange(unit));
        }

        public static Vector3 Source(this Spell.Skillshot s)
        {
            return s.SourcePosition ?? MyHero.Position;
        }

        public static Obj_AI_Base JungleClear(this Spell.Skillshot s, bool useCast = true, int numberOfHits = 1)
        {
            if (!s.IsReady() || numberOfHits <= 0) return null;
            var minions =
                EntityManager.MinionsAndMonsters.GetJungleMonsters(s.Source(), s.Range + s.Width).ToList<Obj_AI_Base>();
            if (minions.Count < numberOfHits) return null;
            switch (s.Type)
            {
                case SkillShotType.Linear:
                    var t = s.GetBestLineTarget(minions);
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
                    var t2 = s.GetBestCircularTarget(minions);
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
            var minions =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, s.Source(),
                    s.Range + s.Width).ToList<Obj_AI_Base>();
            if (minions.Count < numberOfHits) return null;
            switch (s.Type)
            {
                case SkillShotType.Linear:
                    var t = s.GetBestLineTarget(minions);
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
                    var t2 = s.GetBestCircularTarget(minions);
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

        private static int CountObjectsOnLineSegment(this Spell.Skillshot s, IEnumerable<Obj_AI_Base> list,
            Vector3 endPosition)
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
            var lista = list.Where(m => s.Source().Distance(m, true) <= Math.Pow(s.Range, 2)).ToList();
            foreach (var obj in lista)
            {
                var endPosition = s.Source() + (obj.Position - s.Source()).Normalized()*s.Range;
                var hit = s.CountObjectsOnLineSegment(lista, endPosition);
                if (hit > bestHit)
                {
                    bestHit = hit;
                    bestTarget = obj;
                    if (bestHit == lista.Count)
                    {
                        break;
                    }
                }
            }
            return new Tuple<int, Obj_AI_Base>(bestHit, bestTarget);
        }

        private static int CountObjectsNearTo(this Spell.Skillshot s, IEnumerable<Obj_AI_Base> list, Obj_AI_Base target)
        {
            return list.Count(obj => target.Distance(obj, true) <= Math.Pow(s.Width, 2));
        }

        public static Tuple<int, Obj_AI_Base> GetBestCircularTarget(this Spell.Skillshot s, List<Obj_AI_Base> list)
        {
            Obj_AI_Base bestTarget = null;
            var bestHit = -1;
            var lista =
                list.Where(m => s.Source().Distance(m, true) <= Math.Pow(s.Range + s.Width, 2))
                    .ToList();
            foreach (var obj in lista)
            {
                var hit = s.CountObjectsNearTo(lista, obj);
                if (hit <= bestHit) continue;
                bestHit = hit;
                bestTarget = obj;
                if (bestHit == lista.Count)
                {
                    break;
                }
            }
            return new Tuple<int, Obj_AI_Base>(bestHit, bestTarget);
        }

        
        public static Obj_AI_Base LastHit(this Spell.Skillshot s, bool useCast = true)
        {
            if (!s.IsReady()) return null;
            foreach (var minion in from minion in Orbwalker.UnKillableMinionsList
                let predHealth =
                    Prediction.Health.GetPrediction(minion,
                        (int) (1000f*s.Source().Distance(minion)/s.Speed) + s.CastDelay)
                where predHealth >= 0
                where s.Slot.GetSpellDamage(minion) >= predHealth
                select minion)
            {
                s.Cast(minion);
                return minion;
            }
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