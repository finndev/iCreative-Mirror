using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace LeeSin
{
    public static class Util
    {
        public static float ExtraAaRange = 120f;
        public static AIHeroClient MyHero { get { return Player.Instance; } }
        public static Vector3 MousePos { get { return Game.CursorPos; } }
        public static bool IsValidAlly(this AttackableUnit unit, float range = float.MaxValue)
        {
            return unit != null && unit.IsValid && !unit.IsDead && MyHero.Distance(unit, true) <= Math.Pow(range, 2);
        }
        public static bool IsInAutoAttackRange(this Obj_AI_Base source, Obj_AI_Base target)
        {
            if (Combo.IsActive || Harass.IsActive)
            {
                return source != null && target != null && source.IsValid && target.IsValid && !source.IsDead && !target.IsDead && Math.Pow(source.BoundingRadius + target.BoundingRadius + source.AttackRange + ExtraAaRange, 2) >= Extensions.Distance(source, target, true);
            }
            return source != null && target != null && source.IsValid && target.IsValid && !source.IsDead && !target.IsDead && Math.Pow(source.BoundingRadius + target.BoundingRadius + source.AttackRange, 2) >= Extensions.Distance(source, target, true);
        }

        public static bool HasBuff2(this Obj_AI_Base target, string buffName)
        {
            if (target == null || !target.IsValid)
            {
                return false;
            }
            return target.Buffs.Any(m => m.Name.Equals(buffName));
        }

        public static BuffInstance GetBuff2(this Obj_AI_Base target, string buffName)
        {
            if (target == null || !target.IsValid)
            {
                return null;
            }
            return target.Buffs.FirstOrDefault(m => m.Name.Equals(buffName));
        }

        public static int GetPriority(this AIHeroClient hero)
        {
            string championName = hero.ChampionName;
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
