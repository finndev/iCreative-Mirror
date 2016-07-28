using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK.Enumerations;

namespace Simple_Vayne.Utility
{
    /// <summary>
    /// Interrupter class
    /// </summary>
    public class Interrupter
    {
        /// <summary>
        /// Dictionary
        /// </summary>
        public static readonly Dictionary<Champion, InterrupterInfo> Interruptible = new Dictionary<Champion, InterrupterInfo>();

        /// <summary>
        /// Interruptible spells database
        /// </summary>
        public Interrupter()
        {
            Interruptible.Add(Champion.AurelionSol, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E
            });
            Interruptible.Add(Champion.Caitlyn, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.FiddleSticks, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.Galio, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.Janna, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.Jhin, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.Karthus, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.Katarina, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.Lucian, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.Malzahar, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.MasterYi, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.W
            });
            Interruptible.Add(Champion.MissFortune, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.Nunu, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.Pantheon, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.Quinn, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.Shen, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.TahmKench, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.TwistedFate, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.Varus, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.Q
            });
            Interruptible.Add(Champion.Velkoz, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.Warwick, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.Xerath, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.R
            });
            Interruptible.Add(Champion.Zac, new InterrupterInfo
            {
                DangerLevel = DangerLevel.High,
                SpellSlot = SpellSlot.E
            });
        }

        /// <summary>
        /// InterrupterInfo class
        /// </summary>
        public class InterrupterInfo
        {
            /// <summary>
            /// Spell's danger level
            /// </summary>
            public DangerLevel DangerLevel;

            /// <summary>
            /// Spell's slot
            /// </summary>
            public SpellSlot SpellSlot;


            /// <summary>
            /// Interrupter info
            /// </summary>
            /// <param name="dangerLevel">Spell's danger level</param>
            /// <param name="spellSlot">Spell's slot</param>
            public InterrupterInfo(DangerLevel dangerLevel, SpellSlot spellSlot)
            {
                DangerLevel = dangerLevel;
                SpellSlot = spellSlot;
            }

            /// <summary>
            /// Constructor
            /// </summary>
            public InterrupterInfo()
            {
            }
        }

        /// <summary>
        /// InterrupterMenuInfo
        /// </summary>
        public class InterrupterMenuInfo
        {
            /// <summary>
            /// Champion
            /// </summary>
            public Champion Champion;

            /// <summary>
            /// Enabled or disabled
            /// </summary>
            public bool Enabled;

            /// <summary>
            /// Spell Slot
            /// </summary>
            public SpellSlot SpellSlot;

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
            /// Interrupter menu info
            /// </summary>
            public InterrupterMenuInfo(Champion champion, SpellSlot spellSlot, bool enabled, int percentHp, int enemiesNear, int delay, DangerLevel dangerLevel)
            {
                Champion = champion;
                SpellSlot = spellSlot;
                Enabled = enabled;
                PercentHp = percentHp;
                EnemiesNear = enemiesNear;
                Delay = delay;
                DangerLevel = dangerLevel;
            }

            /// <summary>
            /// Constructor
            /// </summary>
            public InterrupterMenuInfo()
            {
            }
        }
    }
}
