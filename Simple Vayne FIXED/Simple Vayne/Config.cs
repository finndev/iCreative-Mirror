using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using Simple_Vayne.Utility;
using CheckBox = EloBuddy.SDK.Menu.Values.CheckBox;
using ComboBox = EloBuddy.SDK.Menu.Values.ComboBox;
using Interrupter = Simple_Vayne.Utility.Interrupter;
using MainMenu = EloBuddy.SDK.Menu.MainMenu;
using Menu = EloBuddy.SDK.Menu.Menu;

namespace Simple_Vayne
{
    public static class Config
    {
        private const string MenuName = "Simple Vayne";

        private static Menu Menu;

        public static List<Interrupter.InterrupterMenuInfo> InterrupterMenuValues  = new List<Interrupter.InterrupterMenuInfo>();
        public static List<Gapclosers.GapcloserMenuInfo> AntiGapcloserMenuValues = new List<Gapclosers.GapcloserMenuInfo>();

        /// <summary>
        /// Statuc initializer
        /// </summary>
        public static void Initialize()
        {
            Menu = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Menu.AddLabel("Just a simple Vayne addon, with customizable options.");
            Menu.AddLabel("Leave feedback in forum thread if you have any sugestions.");
            Menu.AddLabel("Good luck in your game.");

            Modes.Initialize();
            InterrupterMenu.Initializer();
            GapcloserMenu.Initializer();
            TumbleMenu.Initializer();
            CondemnMenu.Initializer();
            Drawings.Initializer();
            Misc.Initializer();
        }

        /// <summary>
        /// InterrupterMenu
        /// </summary>
        public static class InterrupterMenu
        {
            private static CheckBox _enabled;
            public static int foundinterruptiblespells;

            /// <summary>
            /// Returns true if Interrupter is enabled.
            /// </summary>
            public static bool Enabled
            {
                get { return _enabled.CurrentValue; }
            }

            /// <summary>
            /// Static initializer
            /// </summary>
            public static void Initializer()
            {
                var interrupterMenu = Menu.AddSubMenu("Interrupter");
                _enabled = interrupterMenu.Add("InterrupterEnabled", new CheckBox("Enable Interrupter"));
                interrupterMenu.AddSeparator();


                interrupterMenu.AddGroupLabel("Interruptible Spells !");
                interrupterMenu.AddSeparator(5);

                var intt = new Interrupter();

                foreach (
                    var data in
                        EntityManager.Heroes.Enemies.Where(x => Interrupter.Interruptible.ContainsKey(x.Hero))
                            .Select(x => Interrupter.Interruptible.FirstOrDefault(key => key.Key == x.Hero)))
                {
                    var dangerlevel = Interrupter.Interruptible.FirstOrDefault(pair => pair.Key == data.Key);

                    interrupterMenu.AddGroupLabel(data.Key + " " + data.Value.SpellSlot);

                    interrupterMenu.Add("Enabled." + data.Key, new CheckBox("Enabled")).OnValueChange +=
                        delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = InterrupterMenuValues.FirstOrDefault(i => i.Champion == data.Key);
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.Enabled = args.NewValue;
                        };

                    interrupterMenu.Add("DangerLevel." + data.Key, new ComboBox("Danger Level", new[] { "High", "Medium", "Low" }, (int)dangerlevel.Value.DangerLevel)).OnValueChange +=
                        delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = InterrupterMenuValues.FirstOrDefault(i => i.Champion == data.Key);
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.DangerLevel = (DangerLevel)args.NewValue;
                        };

                    interrupterMenu.Add("PercentHP." + data.Key, new Slider("Only if Im below of {0} % of my HP", 100)).OnValueChange +=
                        delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = InterrupterMenuValues.FirstOrDefault(i => i.Champion == data.Key);
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.PercentHp = args.NewValue;
                        };

                    interrupterMenu.Add("EnemiesNear." + data.Key, new Slider("Only if {0} or less enemies are near", 5, 1, 5)).OnValueChange +=
                        delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = InterrupterMenuValues.FirstOrDefault(i => i.Champion == data.Key);
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.EnemiesNear = args.NewValue;
                        };

                    var rand = new Random();

                    interrupterMenu.Add("Delay." + data.Key, new Slider("Humanizer delay", rand.Next(15, 50), 0, 500)).OnValueChange +=
                        delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = InterrupterMenuValues.FirstOrDefault(i => i.Champion == data.Key);
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.Delay = args.NewValue;
                        };

                    interrupterMenu.AddSeparator(5);

                    InterrupterMenuValues.Add(new Interrupter.InterrupterMenuInfo
                    {
                        Champion = data.Key,
                        SpellSlot = data.Value.SpellSlot,
                        Delay = interrupterMenu["Delay." + data.Key].Cast<Slider>().CurrentValue,
                        DangerLevel = (DangerLevel)interrupterMenu["DangerLevel." + data.Key].Cast<ComboBox>().CurrentValue,
                        Enabled = interrupterMenu["Enabled." + data.Key].Cast<CheckBox>().CurrentValue,
                        EnemiesNear = interrupterMenu["EnemiesNear." + data.Key].Cast<Slider>().CurrentValue,
                        PercentHp = interrupterMenu["PercentHP." + data.Key].Cast<Slider>().CurrentValue
                    });

                    foundinterruptiblespells++;
                }
                if (foundinterruptiblespells == 0)
                {
                    interrupterMenu.AddGroupLabel("No interruptible spells found !");
                }

            }
        }

        /// <summary>
        /// Gapcloser menu
        /// </summary>
        public static class GapcloserMenu
        {
            private static CheckBox _enabled;
            public static int foundgapclosers;

            /// <summary>
            /// Returns true if anti gapclosers is enabled.
            /// </summary>
            public static bool Enabled
            {
                get { return _enabled.CurrentValue; }
            }
            /// <summary>
            /// Static initializer
            /// </summary>
            public static void Initializer()
            {
                var gapcloserMenu = Menu.AddSubMenu("Anti-Gapcloser");
                _enabled = gapcloserMenu.Add("GapclosersEnabled", new CheckBox("Enable Anti-Gapcloser"));
                gapcloserMenu.AddSeparator(5);

                gapcloserMenu.AddGroupLabel("Enemy Gapclosers : ");
                gapcloserMenu.AddSeparator(5);

                /*
                foreach (
                    var data in
                        EntityManager.Heroes.Enemies.Where(x => Gapcloser.GapCloserList.Exists(k => k.ChampName == x.ChampionName) && x.Hero != Champion.Ziggs)
                            .Select(x => Gapcloser.GapCloserList.FirstOrDefault(key => key.ChampName == x.ChampionName)))
                {
                    var dangerlevel = Gapclosers.GapcloserDangerLevel.FirstOrDefault(pair => pair.Key == data.ChampName);

                    gapcloserMenu.AddGroupLabel(data.ChampName + " " + data.SpellSlot + " [" + data.SpellName + "]");

                    gapcloserMenu.Add("Enabled." + data.ChampName, new CheckBox("Enabled")).OnValueChange +=
                        delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = AntiGapcloserMenuValues.FirstOrDefault(i => i.Champion == data.ChampName);
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.Enabled = args.NewValue;
                        };

                    gapcloserMenu.Add("DangerLevel." + data.ChampName, new ComboBox("Danger Level", new[] { "High", "Medium", "Low" }, (int)dangerlevel.Value.DangerLevel)).OnValueChange +=
                        delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = AntiGapcloserMenuValues.FirstOrDefault(i => i.Champion == data.ChampName);
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.DangerLevel = (DangerLevel)args.NewValue;
                        };

                    gapcloserMenu.AddSeparator(5);

                    gapcloserMenu.Add("PercentHP." + data.ChampName, new Slider("Only if Im below of {0} % of my HP", 100)).OnValueChange +=
                        delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = AntiGapcloserMenuValues.FirstOrDefault(i => i.Champion == data.ChampName);
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.PercentHp = args.NewValue;
                        };

                    gapcloserMenu.Add("EnemiesNear." + data.ChampName, new Slider("Only if {0} or less enemies are near", dangerlevel.Value.EnemiesNear, 1, 5)).OnValueChange +=
                        delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = AntiGapcloserMenuValues.FirstOrDefault(i => i.Champion == data.ChampName);
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.EnemiesNear = args.NewValue;
                        };

                    gapcloserMenu.Add("Delay." + data.ChampName, new Slider("Humanizer delay", dangerlevel.Value.Delay, 0, 500)).OnValueChange +=
                        delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = AntiGapcloserMenuValues.FirstOrDefault(i => i.Champion == data.ChampName);
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.Delay = args.NewValue;
                        };

                    gapcloserMenu.AddSeparator(5);

                    AntiGapcloserMenuValues.Add(new Gapclosers.GapcloserMenuInfo
                    {
                        Champion = data.ChampName,
                        SpellSlot = data.SpellSlot,
                        SpellName = data.SpellName,
                        Delay = gapcloserMenu["Delay." + data.ChampName].Cast<Slider>().CurrentValue,
                        DangerLevel = (DangerLevel)gapcloserMenu["DangerLevel." + data.ChampName].Cast<ComboBox>().CurrentValue,
                        Enabled = gapcloserMenu["Enabled." + data.ChampName].Cast<CheckBox>().CurrentValue,
                        EnemiesNear = gapcloserMenu["EnemiesNear." + data.ChampName].Cast<Slider>().CurrentValue,
                        PercentHp = gapcloserMenu["PercentHP." + data.ChampName].Cast<Slider>().CurrentValue
                    });
                    foundgapclosers++;
                }

                if (EntityManager.Heroes.Enemies.FirstOrDefault(x => x.Hero == Champion.Nidalee) != null)
                {
                    gapcloserMenu.AddGroupLabel("Nidalee W [Pounce]");

                    gapcloserMenu.Add("Enabled.Nidalee", new CheckBox("Enabled")).OnValueChange +=
                        delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = AntiGapcloserMenuValues.FirstOrDefault(i => i.Champion == "Nidalee");
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.Enabled = args.NewValue;
                        };

                    gapcloserMenu.Add("DangerLevel.Nidalee", new ComboBox("Danger Level", new[] { "High", "Medium", "Low" }, 1)).OnValueChange +=
                        delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = AntiGapcloserMenuValues.FirstOrDefault(i => i.Champion == "Nidalee");
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.DangerLevel = (DangerLevel)args.NewValue;
                        };

                    gapcloserMenu.AddSeparator(5);

                    gapcloserMenu.Add("PercentHP.Nidalee", new Slider("Only if Im below of {0} % of my HP", 100)).OnValueChange +=
                        delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = AntiGapcloserMenuValues.FirstOrDefault(i => i.Champion == "Nidalee");
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.PercentHp = args.NewValue;
                        };

                    gapcloserMenu.Add("EnemiesNear.Nidalee", new Slider("Only if {0} or less enemies are near", 5, 1, 5)).OnValueChange +=
                        delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = AntiGapcloserMenuValues.FirstOrDefault(i => i.Champion == "Nidalee");
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.EnemiesNear = args.NewValue;
                        };

                    gapcloserMenu.Add("Delay.Nidalee", new Slider("Humanizer delay", 0, 0, 500)).OnValueChange +=
                        delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = AntiGapcloserMenuValues.FirstOrDefault(i => i.Champion == "Nidalee");
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.Delay = args.NewValue;
                        };

                    gapcloserMenu.AddSeparator(5);

                    AntiGapcloserMenuValues.Add(new Gapclosers.GapcloserMenuInfo
                    {
                        Champion = "Nidalee",
                        SpellName = "pounce",
                        Delay = gapcloserMenu["Delay.Nidalee"].Cast<Slider>().CurrentValue,
                        DangerLevel = (DangerLevel)gapcloserMenu["DangerLevel.Nidalee"].Cast<ComboBox>().CurrentValue,
                        Enabled = gapcloserMenu["Enabled.Nidalee"].Cast<CheckBox>().CurrentValue,
                        EnemiesNear = gapcloserMenu["EnemiesNear.Nidalee"].Cast<Slider>().CurrentValue,
                        PercentHp = gapcloserMenu["PercentHP.Nidalee"].Cast<Slider>().CurrentValue
                    });

                    foundgapclosers++;
                }

                if (EntityManager.Heroes.Enemies.FirstOrDefault(x => x.Hero == Champion.Rengar) != null)
                {
                    gapcloserMenu.AddGroupLabel("Rengar's passive [Leap]");

                    gapcloserMenu.Add("Enabled.Rengar", new CheckBox("Enabled")).OnValueChange +=
                        delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = AntiGapcloserMenuValues.FirstOrDefault(i => i.Champion == "Rengar");
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.Enabled = args.NewValue;
                        };

                    gapcloserMenu.Add("DangerLevel.Rengar", new ComboBox("Danger Level", new[] { "High", "Medium", "Low" })).OnValueChange +=
                        delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = AntiGapcloserMenuValues.FirstOrDefault(i => i.Champion == "Rengar");
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.DangerLevel = (DangerLevel)args.NewValue;
                        };

                    gapcloserMenu.AddSeparator(5);

                    gapcloserMenu.Add("PercentHP.Rengar", new Slider("Only if Im below of {0} % of my HP", 100)).OnValueChange +=
                        delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = AntiGapcloserMenuValues.FirstOrDefault(i => i.Champion == "Rengar");
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.PercentHp = args.NewValue;
                        };

                    gapcloserMenu.Add("EnemiesNear.Rengar", new Slider("Only if {0} or less enemies are near", 5, 1, 5)).OnValueChange +=
                        delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = AntiGapcloserMenuValues.FirstOrDefault(i => i.Champion == "Rengar");
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.EnemiesNear = args.NewValue;
                        };

                    gapcloserMenu.Add("Delay.Rengar", new Slider("Humanizer delay", 0, 0, 500)).OnValueChange +=
                        delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                        {
                            var interrupterMenuInfo = AntiGapcloserMenuValues.FirstOrDefault(i => i.Champion == "Rengar");
                            if (interrupterMenuInfo != null)
                                interrupterMenuInfo.Delay = args.NewValue;
                        };

                    gapcloserMenu.AddSeparator(5);

                    AntiGapcloserMenuValues.Add(new Gapclosers.GapcloserMenuInfo
                    {
                        Champion = "Rengar",
                        SpellName = "Rengar_LeapSound.troy",
                        Delay = gapcloserMenu["Delay.Rengar"].Cast<Slider>().CurrentValue,
                        DangerLevel = (DangerLevel)gapcloserMenu["DangerLevel.Rengar"].Cast<ComboBox>().CurrentValue,
                        Enabled = gapcloserMenu["Enabled.Rengar"].Cast<CheckBox>().CurrentValue,
                        EnemiesNear = gapcloserMenu["EnemiesNear.Rengar"].Cast<Slider>().CurrentValue,
                        PercentHp = gapcloserMenu["PercentHP.Rengar"].Cast<Slider>().CurrentValue
                    });

                    foundgapclosers++;
                }
                */
                if (foundgapclosers == 0)
                {
                    gapcloserMenu.AddGroupLabel("No gapclosers found !");
                }
            }
        }

        /// <summary>
        /// TumbleMenu
        /// </summary>
        public static class TumbleMenu
        {
            private static Menu MenuTumble;

            /// <summary>
            /// Returns Q mode from menu
            /// 0 - CursorPos
            /// 1 - Auto
            /// </summary>
            public static int Mode
            {
                get { return MenuTumble["Q.Mode"].Cast<ComboBox>().SelectedIndex; }
            }

            /// <summary>
            /// Returns Q.Gapcloser checkbox menu value
            /// </summary>
            public static bool AsGapcloser
            {
                get { return MenuTumble["Q.Gapcloser"].Cast<CheckBox>().CurrentValue; }
            }

            /// <summary>
            /// Returns Q.Backwards checkbox menu value
            /// </summary>
            public static bool Backwards
            {
                get { return MenuTumble["Q.Backwards"].Cast<CheckBox>().CurrentValue; }
            }

            /// <summary>
            /// Returns Q.DangerousSpells checkbox menu value
            /// </summary>
            public static bool DangerousSpells
            {
                get { return MenuTumble["Q.DangerousSpells"].Cast<CheckBox>().CurrentValue; }
            }

            /// <summary>
            /// Returns Q.To2StacksOnly checkbox menu value
            /// </summary>
            public static bool To2StacksOnly
            {
                get { return MenuTumble["Q.To2StacksOnly"].Cast<CheckBox>().CurrentValue; }
            }

            /// <summary>
            /// Returns Q.BlockQIfEnemyIsOutsideRange checkbox menu value
            /// </summary>
            public static bool BlockQIfEnemyIsOutsideAaRange
            {
                get { return MenuTumble["Q.BlockQIfEnemyIsOutsideRange"].Cast<CheckBox>().CurrentValue; }
            }

            /// <summary>
            /// Returns Q.IgnoreAllChecks checkbox menu value
            /// </summary>
            public static bool IgnoreAllChecks
            {
                get { return MenuTumble["Q.IgnoreAllChecks"].Cast<KeyBind>().CurrentValue; }
            }
            

            /// <summary>
            /// Static initializer
            /// </summary>
            public static void Initializer()
            {
                MenuTumble = Menu.AddSubMenu("Tumble");

                MenuTumble.AddGroupLabel("Tumble settings : ");
                MenuTumble.AddSeparator(5);

                MenuTumble.Add("Q.Mode", new ComboBox("Q Mode", new[] { "CursorPos", "Auto" }));
                MenuTumble.AddSeparator(5);
                MenuTumble.Add("Q.Gapcloser", new CheckBox("Use Q as a gapcloser if enemy is escaping", false));
                MenuTumble.AddLabel("Uses Q as a gapcloser if an enemy is escaping and is killable from an empovered autoattack");
                MenuTumble.AddSeparator(5);
                MenuTumble.Add("Q.Backwards", new CheckBox("Use Q on gapclosers", false));
                MenuTumble.AddLabel("Uses Q backwards when enemy is using a high danger level gapcloser on you");
                MenuTumble.AddSeparator(5);
                MenuTumble.Add("Q.DangerousSpells", new CheckBox("Take into account dangerous spells", false));
                MenuTumble.AddLabel("Takes into account dangerous spells / gapclosers while checking if tumble position is dangerous");
                MenuTumble.AddSeparator(5);
                MenuTumble.Add("Q.To2StacksOnly", new CheckBox("Use Q only on 2 W stacks", false));
                MenuTumble.AddLabel("Uses Q only if an enemy has 2 W stacks.");
                MenuTumble.Add("Q.BlockQIfEnemyIsOutsideRange", new CheckBox("Block Q if enemy will leave range", false));
                MenuTumble.AddLabel("Block Q if enemy is outside my AutoAttack range. Only for Auto tumble mode.");
                MenuTumble.AddSeparator(5);
                MenuTumble.Add("Q.IgnoreAllChecks", new KeyBind("Ignore all safety checks", false, KeyBind.BindTypes.PressToggle, 'H')).OnValueChange +=
                    delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                    {
                        Helpers.PrintInfoMessage(args.NewValue ? "Ignore all safety checks : <font color =\"#32BA1A\"><b>Enabled</b></font>" : "Ignore all safety checks : <font color =\"#E02007\"><b>Disabled</b></font>", false);
                    };
                MenuTumble.AddLabel("Ignores all safety checks. Casts Q to your cursor position whenever possible");
            }
        }

        /// <summary>
        /// CondemnMenu
        /// </summary>
        public static class CondemnMenu
        {
            private static Menu MenuCondemn;

            /// <summary>
            /// Returns true if condemn is enabled in menu
            /// </summary>
            public static bool Enabled
            {
                get { return MenuCondemn["Enabled"].Cast<CheckBox>().CurrentValue; }
            }

            /// <summary>
            /// Returns true if EAfterNextAuto is enabled in menu
            /// </summary>
            public static bool EAfterNextAuto
            {
                get { return MenuCondemn["Keybind.E"].Cast<KeyBind>().CurrentValue; }
                set { MenuCondemn["Keybind.E"].Cast<KeyBind>().CurrentValue = value; }
            }

            /// <summary>
            /// Returns true if FlashCondemn is enabled in menu
            /// </summary>
            public static bool FlashCondemn
            {
                get { return MenuCondemn["Keybind.FlashCondemn"].Cast<KeyBind>().CurrentValue; }
            }

            /// <summary>
            /// Returns E mode combobox value
            /// 0 - Always
            /// 1 - Only in Combo
            /// </summary>
            public static int EMode
            {
                get { return MenuCondemn["E.Mode"].Cast<ComboBox>().CurrentValue; }
            }

            /// <summary>
            /// Returns E mode combobox value
            /// 0 - Accurate
            /// 1 - Fast
            /// </summary>
            public static int EMethod
            {
                get { return MenuCondemn["E.Method"].Cast<ComboBox>().CurrentValue; }
            }

            /// <summary>
            /// Returns E mode combobox value
            /// 0 - Everyone
            /// 1 - Current Target
            /// </summary>
            public static int ETargettingMode
            {
                get { return MenuCondemn["E.Targetting"].Cast<ComboBox>().CurrentValue; }
            }

            /// <summary>
            /// Returns push distance slider value
            /// </summary>
            public static int PushDistance
            {
                get { return MenuCondemn["E.PushDistance"].Cast<Slider>().CurrentValue; }
            }

            /// <summary>
            /// Returns Execute checkbox value
            /// </summary>
            public static bool Execute
            {
                get { return MenuCondemn["E.Execute"].Cast<CheckBox>().CurrentValue; }
            }

            /// <summary>
            /// Returns E Execute mode
            /// 0 - Only after AutoAttack
            /// 1 - Always
            /// </summary>
            public static int ExecuteMode
            {
                get { return MenuCondemn["E.Execute.Mode"].Cast<ComboBox>().CurrentValue; }
            }

            /// <summary>
            /// Returns E Execute if enemies are nearby menu value
            /// </summary>
            public static int ExecuteEnemiesNearby
            {
                get { return MenuCondemn["E.Execute.Enemies"].Cast<Slider>().CurrentValue; }
            }

            /// <summary>
            /// Use E On 
            /// </summary>
            /// <param name="championName">Enemy champion name in lower case</param>
            /// <returns><c>True</c> if checkbox is true</returns>
            public static bool UseEOn(string championName)
            {
                return MenuCondemn["Enabled." + championName.ToLower()] != null && MenuCondemn["Enabled." + championName.ToLower()].Cast<CheckBox>().CurrentValue;
            }
            
            /// <summary>
            /// Static initializer
            /// </summary>
            public static void Initializer()
            {
                MenuCondemn = Menu.AddSubMenu("Condemn");

                MenuCondemn.AddGroupLabel("Condemn settings : ");
                MenuCondemn.AddSeparator(5);

                MenuCondemn.Add("Enabled", new CheckBox("Use E to stun"));
                MenuCondemn.Add("Keybind.E", new KeyBind("Use E after next autoattack", false, KeyBind.BindTypes.PressToggle, 'E'));
                MenuCondemn.Add("Keybind.FlashCondemn", new KeyBind("Flash condemn", false, KeyBind.BindTypes.HoldActive, 'G'));
                MenuCondemn.AddSeparator();
                MenuCondemn.Add("E.Mode", new ComboBox("E Mode", new[] { "Always", "Only in Combo" }));
                MenuCondemn.Add("E.Method", new ComboBox("E Method", new[] { "Accurate", "Fast" }));
                MenuCondemn.Add("E.Targetting", new ComboBox("E Targetting mode", new[] { "Everyone", "Current target" }));
                MenuCondemn.Add("E.PushDistance", new Slider("Push distance", 420, 380, 470));
                MenuCondemn.AddSeparator(5);

                MenuCondemn.AddGroupLabel("Execute settings : ");
                MenuCondemn.AddSeparator(5);

                MenuCondemn.Add("E.Execute", new CheckBox("Use E to execute"));
                MenuCondemn.Add("E.Execute.Mode", new ComboBox("Execute mode", new[] { "Only after AutoAttack", "Always" }));
                MenuCondemn.Add("E.Execute.Enemies", new Slider("Use E to execute only if {0} or less enemies are near", 2, 1, 5));
                MenuCondemn.AddSeparator(5);

                MenuCondemn.AddGroupLabel("Use condemn on : ");
                MenuCondemn.AddSeparator(5);

                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    MenuCondemn.Add("Enabled." + enemy.ChampionName.ToLower(), new CheckBox(enemy.ChampionName));
                }
            }
        }


        /// <summary>
        /// Drawings menu
        /// </summary>
        public static class Drawings
        {
            private static Menu DrawingsMenu;

            /// <summary>
            /// Returns HPBar menu value
            /// </summary>
            public static bool HpBar
            {
                get { return DrawingsMenu["HPBar"].Cast<CheckBox>().CurrentValue; }
            }

            /// <summary>
            /// Returns HPBar.FontSize menu slider value
            /// </summary>
            public static int HPBarFontSize
            {
                get { return DrawingsMenu["HPBar.FontSize"].Cast<Slider>().CurrentValue; }
            }

            /// <summary>
            /// Returns UltDuration menu value
            /// </summary>
            public static bool UltDuration
            {
                get { return DrawingsMenu["UltDuration"].Cast<CheckBox>().CurrentValue; }
            }

            /// <summary>
            /// Returns UltDuration.FontSize menu slider value
            /// </summary>
            public static int UltDurationFontSize
            {
                get { return DrawingsMenu["UltDuration.FontSize"].Cast<Slider>().CurrentValue; }
            }

            /// <summary>
            /// Returns CurrentTarget menu value
            /// </summary>
            public static bool CurrentTarget
            {
                get { return DrawingsMenu["CurrentTarget"].Cast<CheckBox>().CurrentValue; }
            }

            /// <summary>
            /// Returns DangerousSpells menu value
            /// </summary>
            public static bool DangerousSpells
            {
                get { return DrawingsMenu["DangerousSpells"].Cast<CheckBox>().CurrentValue; }
            }

            /// <summary>
            /// Returns DangerousSpells.FontSize menu slider value
            /// </summary>
            public static int DangerousSpellsFontSize
            {
                get { return DrawingsMenu["DangerousSpells.FontSize"].Cast<Slider>().CurrentValue; }
            }

            /// <summary>
            /// Returns DangerousSpells.X menu slider value
            /// </summary>
            public static int DangerousSpellsX
            {
                get { return DrawingsMenu["DangerousSpells.X"].Cast<Slider>().CurrentValue; }
            }

            /// <summary>
            /// Returns DangerousSpells.Y menu slider value
            /// </summary>
            public static int DangerousSpellsY
            {
                get { return DrawingsMenu["DangerousSpells.Y"].Cast<Slider>().CurrentValue; }
            }

            /// <summary>
            /// Returns true if DrawPermashow is enabled.
            /// </summary>
            public static bool DrawPermashow => DrawingsMenu["DrawPermashow"].Cast<CheckBox>().CurrentValue;

            /// <summary>
            /// Static initializer
            /// </summary>
            public static void Initializer()
            {
                DrawingsMenu = Menu.AddSubMenu("Drawings");

                DrawingsMenu.AddGroupLabel("Drawings settings : ");
                DrawingsMenu.AddSeparator(5);
                DrawingsMenu.Add("DrawPermashow", new CheckBox("Enable PermaShow"));
                DrawingsMenu.Add("HPBar", new CheckBox("Draw stacks on enemy"));
                DrawingsMenu.Add("HPBar.FontSize", new Slider("Font size : {0}", 15, 10, 25)).OnValueChange +=
                    delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                    {
                        sender.DisplayName = "Font size : {0} You need to F5 to make those changes take effect!";
                    };
                DrawingsMenu.AddSeparator();
                DrawingsMenu.Add("UltDuration", new CheckBox("Draw ult duration"));
                DrawingsMenu.Add("UltDuration.FontSize", new Slider("Font size : {0}", 18, 10, 25)).OnValueChange +=
                    delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                    {
                        sender.DisplayName = "Font size : {0} You need to F5 to make those changes take effect!";
                    };
                DrawingsMenu.AddSeparator();
                DrawingsMenu.Add("CurrentTarget", new CheckBox("Draw current target"));
                DrawingsMenu.Add("DangerousSpells", new CheckBox("Draw dangerous spells"));
                DrawingsMenu.AddSeparator(10);
                DrawingsMenu.Add("DangerousSpells.FontSize", new Slider("Dangerous spells font size : {0}", 15, 10, 25)).OnValueChange +=
                    delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                    {
                        sender.DisplayName = "Dangerous spells font size : {0} You need to F5 to make those changes take effect!";
                    };
                DrawingsMenu.Add("DangerousSpells.X", new Slider("DangerousSpells : X : {0}", (int)(Drawing.Width * 0.85f), 10, Drawing.Width));
                DrawingsMenu.Add("DangerousSpells.Y", new Slider("DangerousSpells : Y : {0}", (int)(Drawing.Height * 0.08f), 1, Drawing.Height));
            }
        }

        /// <summary>
        /// Misc menu
        /// </summary>
        public static class Misc
        {
            private static Menu MiscMenu;

            private static readonly string[] VanyeSkinNames =
            {
                "Classic", "Vindicator", "Aristocrat", "Dragonslayer", "Heartseeker",
                "SKT T1", "Arclight", "Green Chroma", "Red Chroma", "Chaos Silver Chroma"
            };

            public static bool NoAAWhileStealth
            {
                get { return MiscMenu["NoAAWhileStealth"].Cast<KeyBind>().CurrentValue; }
            }

            public static int NoAADelay
            {
                get { return MiscMenu["NoAADelay"].Cast<Slider>().CurrentValue; }
            }
            public static int NoAAEnemies
            {
                get { return MiscMenu["NoAAEnemies"].Cast<Slider>().CurrentValue; }
            }

            public static bool SkinHack
            {
                get { return MiscMenu["SkinHack"].Cast<CheckBox>().CurrentValue; }
            }

            public static int SkinId
            {
                get { return MiscMenu["SkinID"].Cast<ComboBox>().CurrentValue; }
            }

            public static int PermashowX => MiscMenu["Permashow.X"].Cast<Slider>().CurrentValue;
            public static int PermashowY => MiscMenu["Permashow.Y"].Cast<Slider>().CurrentValue;
            
            /// <summary>
            /// Static initializer
            /// </summary>
            public static void Initializer()
            {
                MiscMenu = Menu.AddSubMenu("Misc");

                MiscMenu.AddGroupLabel("Misc settings : ");
                MiscMenu.AddSeparator(5);

                MiscMenu.Add("NoAAWhileStealth", new KeyBind("Dont AutoAttack while stealth", false, KeyBind.BindTypes.PressToggle, 'T')).OnValueChange +=
                    delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                    {
                        Helpers.PrintInfoMessage(args.NewValue ? "Dont attack while stealth : <font color =\"#32BA1A\"><b>Enabled</b></font>" : "Dont attack while stealth : <font color =\"#E02007\"><b>Disabled</b></font>", false);
                    };
                MiscMenu.Add("NoAADelay", new Slider("Delay", 1000, 0, 1000));
                MiscMenu.Add("NoAAEnemies", new Slider("Only if {0} or more enemies are nearby", 2, 1, 5));
                MiscMenu.AddSeparator(40);

                MiscMenu.AddGroupLabel("SkinHack settings : ");
                MiscMenu.AddSeparator(5);

                MiscMenu.Add("SkinHack", new CheckBox("Use SkinHack", false)).OnValueChange +=
                    delegate (ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                    {
                        Player.SetSkin(Player.Instance.BaseSkinName, args.NewValue ? SkinId : 0);
                    };

                MiscMenu.Add("SkinID", new ComboBox("Skin : ", VanyeSkinNames)).OnValueChange +=
                    delegate
                    {
                        if (MiscMenu["SkinHack"].Cast<CheckBox>().CurrentValue)
                            Player.SetSkin(Player.Instance.BaseSkinName, SkinId);
                    };
                MiscMenu.Add("Permashow.X", new Slider("Permashow position X: ", 15, 0, Drawing.Width)).OnValueChange +=
                    delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                    {
                        PermaShow.Position[0] = args.NewValue;
                    };
                MiscMenu.Add("Permashow.Y", new Slider("Permashow position Y: ", (int)(Drawing.Height * 0.08f), 0, Drawing.Height)).OnValueChange +=
                    delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                    {
                        PermaShow.Position[1] = args.NewValue;
                    };
            }
        }

        public static class Modes
        {
            private static Menu Menu;
            
            public static void Initialize()
            {
                Menu = Config.Menu.AddSubMenu("Modes");

                Combo.Initialize();
                Menu.AddSeparator();

                Harass.Initialize();
                Menu.AddSeparator();

                Laneclear.Initialize();
                Menu.AddSeparator();

                Jungleclear.Initialize();
                Menu.AddSeparator();

                Flee.Initialize();
                Menu.AddSeparator();
            }

            public static class Combo
            {
                public static bool UseQ
                {
                    get { return Menu["comboUseQ"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseR
                {
                    get { return Menu["comboUseR"].Cast<CheckBox>().CurrentValue; }
                }
                public static int UseRIfEnemiesAreNearby
                {
                    get { return Menu["comboUseR.Enemies"].Cast<Slider>().CurrentValue; }
                }
                
                public static void Initialize()
                {
                    Menu.AddGroupLabel("Combo");
                    Menu.Add("comboUseQ", new CheckBox("Use Q"));
                    Menu.AddLabel("Q usage in ComboMode");
                    Menu.Add("comboUseR", new CheckBox("Use R", false));
                    Menu.AddLabel("R usage in ComboMode");
                    Menu.Add("comboUseR.Enemies", new Slider("Use R when {0} or more enemies are nearby", 3, 1, 5));
                }
            }

            public static class Harass
            {
                public static bool UseQ
                {
                    get { return Menu["harassUseQ"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseE
                {
                    get { return Menu["harassUseE"].Cast<CheckBox>().CurrentValue; }
                }
                public static void Initialize()
                {
                    Menu.AddGroupLabel("Harass");
                    Menu.Add("harassUseQ", new CheckBox("Use Q"));
                    Menu.AddLabel("Uses Q if enemy has 1 silver stack");
                    Menu.Add("harassUseE", new CheckBox("Use E"));
                    Menu.AddLabel("Casts E on enemy that has 2 silver stacks");
                }
            }
            public static class Laneclear
            {
                public static bool UseQ
                {
                    get { return Menu["laneclearUseQ"].Cast<CheckBox>().CurrentValue; }
                }

                public static int UseQMana
                {
                    get { return Menu["laneclearUseQMana"].Cast<Slider>().CurrentValue; }
                }
                
                public static void Initialize()
                {
                    Menu.AddGroupLabel("Laneclear");
                    Menu.Add("laneclearUseQ", new CheckBox("Use Q"));
                    Menu.AddLabel("Smart Q usage on enemy");
                    Menu.Add("laneclearUseQMana", new Slider("Min mana ({0})%", 40));
                }
            }
            public static class Jungleclear
            {
                public static bool UseQ
                {
                    get { return Menu["jungleclearUseQ"].Cast<CheckBox>().CurrentValue; }
                }

                public static int UseQMana
                {
                    get { return Menu["jungleclearUseQMana"].Cast<Slider>().CurrentValue; }
                }

                public static bool UseE
                {
                    get { return Menu["jungleclearUseE"].Cast<CheckBox>().CurrentValue; }
                }

                public static int UseEMana
                {
                    get { return Menu["jungleclearUseEMana"].Cast<Slider>().CurrentValue; }
                }
                public static void Initialize()
                {
                    Menu.AddGroupLabel("Jungleclear");
                    Menu.Add("jungleclearUseQ", new CheckBox("Use Q"));
                    Menu.AddLabel("Tries to use Q on nearest wall");
                    Menu.Add("jungleclearUseQMana", new Slider("Min mana ({0})%", 40));
                    Menu.Add("jungleclearUseE", new CheckBox("Use E"));
                    Menu.AddLabel("Casts E on big jungle monsters");
                    Menu.Add("jungleclearUseEMana", new Slider("Min mana ({0})%", 50));
                }
            }
            public static class Flee
            {
                public static bool UseQ
                {
                    get { return Menu["fleeUseQ"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseE
                {
                    get { return Menu["fleeUseE"].Cast<CheckBox>().CurrentValue; }
                }
                
                public static void Initialize()
                {
                    Menu.AddGroupLabel("Flee");
                    Menu.Add("fleeUseQ", new CheckBox("Use Q"));
                    Menu.AddLabel("Uses Q backwards");
                    Menu.Add("fleeUseE", new CheckBox("Use E"));
                    Menu.AddLabel("Casts E on closest enemy");
                }
            }
        }
    }
}