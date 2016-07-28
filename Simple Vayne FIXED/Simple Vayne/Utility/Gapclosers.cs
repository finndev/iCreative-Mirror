using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK.Enumerations;

namespace Simple_Vayne.Utility
{
    
    /// <summary>
    /// Gapclosers database class
    /// </summary>
    public class Gapclosers
    {
        public static readonly Dictionary<string, GapcloserInfo> GapcloserDangerLevel = new Dictionary
            <string, GapcloserInfo>
        {
            {"Aatrox", new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Akali", new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Alistar",  new GapcloserInfo(DangerLevel.Medium, 2, 0)},
            {"Corki",  new GapcloserInfo(DangerLevel.Low, 1, 0)},
            {"Diana",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Ekko",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Elise",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Fiora",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Fizz",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Gnar",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Gragas",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Graves",  new GapcloserInfo(DangerLevel.Medium, 3, 0)},
            {"Hecarim",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Irelia",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"JarvanIV",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Jax",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Jayce",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Kassadin",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Khazix",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"LeBlanc",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"LeeSin",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Leona",  new GapcloserInfo(DangerLevel.High, 5, 500)},
            {"Lucian",  new GapcloserInfo(DangerLevel.Low, 2, 0)},
            {"Malphite",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"MasterYi",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"MonkeyKing",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Pantheon",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Poppy",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Renekton",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Riven",  new GapcloserInfo(DangerLevel.Medium, 3, 0)},
            {"Sejuani",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Shen",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Shyvana",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Talon",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Tristana",  new GapcloserInfo(DangerLevel.Low, 2, 0)},
            {"Tryndamere",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Vi",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"XinZhao",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Yasuo",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Zac",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Zed",  new GapcloserInfo(DangerLevel.High, 5, 0)},
            {"Ziggs",  new GapcloserInfo(DangerLevel.Medium, 2, 0)}
        };

        /*
        /// <summary>
        /// Dictionary
        /// </summary>
        public static readonly Dictionary<Champion ,GapcloserInfo> Gapcloser = new Dictionary<Champion, GapcloserInfo>();

        /// <summary>
        /// Gapclosers database
        /// </summary>
        public Gapclosers()
        {
            Gapcloser.Add(Champion.Aatrox, new GapcloserInfo
            {
                SpellName = "aatroxq",
                DangerLevel = DangerLevel.Medium,
                SpellSlot = SpellSlot.Q,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Ahri, new GapcloserInfo
            {
                SpellName = "ahritumble",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Akali, new GapcloserInfo
            {
                SpellName = "akalishadowdance",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Alistar, new GapcloserInfo
            {
                SpellName = "headbutt",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.W,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.AurelionSol, new GapcloserInfo
            {
                SpellName = "xd",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Azir, new GapcloserInfo
            {
                SpellName = "xd",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Braum, new GapcloserInfo
            {
                SpellName = "xd",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.W,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Caitlyn, new GapcloserInfo
            {
                SpellName = "xd",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Corki, new GapcloserInfo
            {
                SpellName = "carpetbomb",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.W,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Diana, new GapcloserInfo
            {
                SpellName = "dianateleport",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Ekko, new GapcloserInfo
            {
                SpellName = "ekkoeattack",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Elise, new GapcloserInfo
            {
                SpellName = "ekkoe",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.Q,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Ezreal, new GapcloserInfo
            {
                SpellName = "xd",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Fiora, new GapcloserInfo
            {
                SpellName = "fioraq",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.Q,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Fizz, new GapcloserInfo
            {
                SpellName = "fizzpiercingstrike",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.Q,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Gnar, new GapcloserInfo
            {
                SpellName = "gnarbige",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Gragas, new GapcloserInfo
            {
                SpellName = "gragase",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Graves, new GapcloserInfo
            {
                SpellName = "gravesmove",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Hecarim, new GapcloserInfo
            {
                SpellName = "xd",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Illaoi, new GapcloserInfo
            {
                SpellName = "illaoiw",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.W,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Irelia, new GapcloserInfo
            {
                SpellName = "ireliagatotsu",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.Q,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.JarvanIV, new GapcloserInfo
            {
                SpellName = "jarvanivdragonstrike",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.Q,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Jax, new GapcloserInfo
            {
                SpellName = "jaxleapstrike",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.Q,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Jayce, new GapcloserInfo
            {
                SpellName = "jaycetotheskies",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.Q,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Kassadin, new GapcloserInfo
            {
                SpellName = "riftwalk",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Katarina, new GapcloserInfo
            {
                SpellName = "katarinae",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Khazix, new GapcloserInfo
            {
                SpellName = "khazixe",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Kindred, new GapcloserInfo
            {
                SpellName = "xd",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.Q,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Leblanc, new GapcloserInfo
            {
                SpellName = "leblancslide",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.W,
                Type = GapcloserInfo.SkillType.SkillShot,
                SecoundSpellName = "leblancslidem",
                SecoundSpellSlot = SpellSlot.R
            });
            Gapcloser.Add(Champion.LeeSin, new GapcloserInfo
            {
                SpellName = "blindmonkqtwo",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.Q,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Leona, new GapcloserInfo
            {
                SpellName = "leonazenithblade",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Lissandra, new GapcloserInfo
            {
                SpellName = "lissandrae",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Lucian, new GapcloserInfo
            {
                SpellName = "luciane",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Malphite, new GapcloserInfo
            {
                SpellName = "ufslash",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Maokai, new GapcloserInfo
            {
                SpellName = "xd",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.W,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.MasterYi, new GapcloserInfo
            {
                SpellName = "alphastrike",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.Q,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Nautilus, new GapcloserInfo
            {
                SpellName = "xd",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.Q,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Nidalee, new GapcloserInfo
            {
                SpellName = "pounce",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.W,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Nocturne, new GapcloserInfo
            {
                SpellName = "xd",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Pantheon, new GapcloserInfo
            {
                SpellName = "pantheon_leapbash",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.W,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Poppy, new GapcloserInfo
            {
                SpellName = "poppyheroiccharge",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Quinn, new GapcloserInfo
            {
                SpellName = "quinne",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.RekSai, new GapcloserInfo
            {
                SpellName = "xd",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Renekton, new GapcloserInfo
            {
                SpellName = "renektonsliceanddice",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Rengar, new GapcloserInfo
            {
                SpellName = "xd",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.Unknown,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Riven, new GapcloserInfo
            {
                SpellName = "rivenfeint",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Sejuani, new GapcloserInfo
            {
                SpellName = "sejuaniarcticassault",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.Q,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Shen, new GapcloserInfo
            {
                SpellName = "shenshadowdash",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Shyvana, new GapcloserInfo
            {
                SpellName = "shyvanatransformcast",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Talon, new GapcloserInfo
            {
                SpellName = "taloncutthroat",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Thresh, new GapcloserInfo
            {
                SpellName = "xd",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.Q,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Tristana, new GapcloserInfo
            {
                SpellName = "tristanaw",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.W,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Tryndamere, new GapcloserInfo
            {
                SpellName = "slashcast",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Vayne, new GapcloserInfo
            {
                SpellName = "xd",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.Q,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Vi, new GapcloserInfo
            {
                SpellName = "viq",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.Q,
                Type = GapcloserInfo.SkillType.SkillShot
            });
            Gapcloser.Add(Champion.Warwick, new GapcloserInfo
            {
                SpellName = "xd",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.MonkeyKing, new GapcloserInfo
            {
                SpellName = "monkeykingnimbus",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.XinZhao, new GapcloserInfo
            {
                SpellName = "xenzhaosweep",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Yasuo, new GapcloserInfo
            {
                SpellName = "yasuodashwrapper",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.Targeted
            });
            Gapcloser.Add(Champion.Zac, new GapcloserInfo
            {
                SpellName = "zace",
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E,
                Type = GapcloserInfo.SkillType.SkillShot
            });
        }*/

        /// <summary>
        /// GapcloserInfo class
        /// </summary>
        public class GapcloserInfo
        {
            /// <summary>
            /// Danger Level
            /// </summary>
            public DangerLevel DangerLevel;

            /// <summary>
            /// Enemies Near
            /// </summary>
            public int EnemiesNear;

            /// <summary>
            /// Delay
            /// </summary>
            public int Delay;

            /// <summary>
            /// Gapcloser info
            /// </summary>
            /// <param name="dangerLevel">Spell's danger level</param>
            /// <param name="enemiesNear">Trigger only if x or less enemies are near</param>
            /// <param name="delay">Spell's delay</param>
            public GapcloserInfo(DangerLevel dangerLevel, int enemiesNear, int delay)
            {
                DangerLevel = dangerLevel;
                EnemiesNear = enemiesNear;
                Delay = delay;
            }

            /// <summary>
            /// Constructor
            /// </summary>
            public GapcloserInfo()
            {
            }
        }

        /// <summary>
        /// InterrupterMenuInfo
        /// </summary>
        public class GapcloserMenuInfo
        {
            /// <summary>
            /// Champion
            /// </summary>
            public string Champion;

            /// <summary>
            /// Champion
            /// </summary>
            public SpellSlot SpellSlot;

            /// <summary>
            /// SpellName
            /// </summary>
            public string SpellName;

            /// <summary>
            /// Enabled or disabled
            /// </summary>
            public bool Enabled;

            /// <summary>
            /// PercentHP
            /// </summary>
            public int PercentHp;

            /// <summary>
            /// EnemiesNear
            /// </summary>
            public int EnemiesNear;

            /// <summary>
            /// Delay
            /// </summary>
            public int Delay;

            /// <summary>
            /// DangerLevel
            /// </summary>
            public DangerLevel DangerLevel;

            /// <summary>
            /// Gapcloser menu info
            /// </summary>
            public GapcloserMenuInfo(string champion, string spellName, bool enabled, int percentHp, int enemiesNear, int delay, DangerLevel dangerLevel)
            {
                Champion = champion;
                SpellName = spellName;
                Enabled = enabled;
                PercentHp = percentHp;
                EnemiesNear = enemiesNear;
                Delay = delay;
                DangerLevel = dangerLevel;
            }

            /// <summary>
            /// Constructor
            /// </summary>
            public GapcloserMenuInfo()
            {
            }
        }
    }
}