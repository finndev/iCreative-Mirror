using System.Collections.Generic;
using EloBuddy;
namespace Simple_Vayne.Utility
{
    public class DangerousSpells
    {
        public enum SpellDangerLevel
        {
            Extreme, High, Medium, Low
        }

        public static readonly Dictionary<Champion, DangerousSpell> DangerousSpellsDictionary = new Dictionary<Champion, DangerousSpell>
        {
            {Champion.Aatrox, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 1,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Ahri, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 1,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Akali, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Amumu, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.Extreme
            }},
            {Champion.Annie, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.Extreme
            }},
            {Champion.Ashe, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Blitzcrank, new DangerousSpell
            {
                SpellSlot = SpellSlot.Q,
                IfEnemiesNear = 2,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Brand, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Braum, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.Extreme
            }},
            {Champion.Caitlyn, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 2,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Cassiopeia, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.Extreme
            }},
            {Champion.Chogath, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 1,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Darius, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Diana, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Draven, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 2,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Ezreal, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 2,
                DangerLevel = SpellDangerLevel.Medium
            }},
            {Champion.FiddleSticks, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Fiora, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Fizz, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Galio, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.Extreme
            }},
            {Champion.Garen, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Gnar, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Gragas, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Graves, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 2,
                DangerLevel = SpellDangerLevel.Medium
            }},
            {Champion.Hecarim, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Illaoi, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 2,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Irelia, new DangerousSpell
            {
                SpellSlot = SpellSlot.Q,
                IfEnemiesNear = 2,
                DangerLevel = SpellDangerLevel.Medium
            }},
            {Champion.JarvanIV, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Jax, new DangerousSpell
            {
                SpellSlot = SpellSlot.E,
                IfEnemiesNear = 2,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Jhin, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 2,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Jinx, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 2,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Katarina, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Kennen, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.LeeSin, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Leona, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.Extreme
            }},
            {Champion.Lissandra, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.Extreme
            }},
            {Champion.Lucian, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 2,
                DangerLevel = SpellDangerLevel.Medium
            }},
            {Champion.Malphite, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.Extreme
            }},
            {Champion.Malzahar, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.Extreme
            }},
            {Champion.Maokai, new DangerousSpell
            {
                SpellSlot = SpellSlot.W,
                IfEnemiesNear = 1,
                DangerLevel = SpellDangerLevel.Medium
            }},
            {Champion.Morgana, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 3,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Nami, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 2,
                DangerLevel = SpellDangerLevel.Medium
            }},
            {Champion.Nautilus, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.Extreme
            }},
            {Champion.Olaf, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 2,
                DangerLevel = SpellDangerLevel.Medium
            }},
            {Champion.Orianna, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.Extreme
            }},
            {Champion.Poppy, new DangerousSpell
            {
                SpellSlot = SpellSlot.E,
                IfEnemiesNear = 3,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Riven, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 3,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Sejuani, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.Extreme
            }},
            {Champion.Sion, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Skarner, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.Extreme
            }},
            {Champion.Sona, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Syndra, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Thresh, new DangerousSpell
            {
                SpellSlot = SpellSlot.Q,
                IfEnemiesNear = 2,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Urgot, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Varus, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 2,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Veigar, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Vi, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.Extreme
            }},
            {Champion.Warwick, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.Extreme
            }},
            {Champion.MonkeyKing, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Yasuo, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 3,
                DangerLevel = SpellDangerLevel.High
            }},
            {Champion.Zed, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.Extreme
            }},
            {Champion.Zyra, new DangerousSpell
            {
                SpellSlot = SpellSlot.R,
                IfEnemiesNear = 5,
                DangerLevel = SpellDangerLevel.High
            }}
        };

        /// <summary>
        /// InterrupterMenuInfo
        /// </summary>
        public class DangerousSpell
        {
            /// <summary>
            /// Champion
            /// </summary>
            public Champion Champion;

            /// <summary>
            /// Spell Slot
            /// </summary>
            public SpellSlot SpellSlot;

            /// <summary>
            /// IfEnemiesNear
            /// </summary>
            public int IfEnemiesNear = 0;

            /// <summary>
            /// DangerLevel
            /// </summary>
            public SpellDangerLevel DangerLevel;

            /// <summary>
            /// Game Tick
            /// </summary>
            public int Tick;

            public DangerousSpell(Champion champion, SpellSlot spellSlot, int enemies, SpellDangerLevel dangerLevel, int tick)
            {
                Champion = champion;
                SpellSlot = spellSlot;
                IfEnemiesNear = enemies;
                DangerLevel = dangerLevel;
                Tick = tick;
            }

            public DangerousSpell()
            {
                
            }

        }
    }
}