using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace MaddeningJinx
{
    public static class MenuManager
    {
        public static Menu Menu;
        public static Dictionary<string, Menu> SubMenu = new Dictionary<string, Menu>();

        public static Menu MiscMenu
        {
            get { return GetSubMenu("Misc"); }
        }

        public static Menu PredictionMenu
        {
            get { return GetSubMenu("Prediction"); }
        }

        public static Menu DrawingsMenu
        {
            get { return GetSubMenu("Drawings"); }
        }

        public static KeyBind TapKey;

        public static bool TapKeyPressed;

        public static Menu ImmobileSpellsMenu;

        public static Menu AntiGapcloserMenu;
        public static readonly HashSet<string> AntiGapcloserHashSet = new HashSet<string>();
        public static readonly HashSet<string> ImmobileSpellsHashSet = new HashSet<string>();

        public static void Initialize()
        {
            Menu = MainMenu.AddMenu(Champion.AddonName, Champion.AddonName + " by " + Champion.Author);

            Menu.AddGroupLabel("General Settings");
            Menu.Add("Farming.Q", new CheckBox("Use Q to Farm"));
            Menu.Add("Prediction.W", new Slider("W HitChancePercent", 75));
            TapKey = Menu.Add("R.Tap.Key", new KeyBind("R Tap Key", false, KeyBind.BindTypes.HoldActive, 'T'));
            TapKey.OnValueChange += OnValueChange;

            Menu.AddGroupLabel("Automatic Settings");
            Menu.Add("KillSteal", new CheckBox("Use Q/W/E to KillSteal"));
            Menu.Add("W.Spells", new CheckBox("Use W when enemy is escaping"));
            Menu.Add("E.CC", new CheckBox("Use E on enemies with CC"));
            Menu.Add("E.Antigapcloser", new CheckBox("Use E as antigapcloser"));
            Menu.Add("E.Spells", new CheckBox("Use E to immobile spells casted by enemy"));
            Menu.Add("R.KillSteal", new CheckBox("Use R to KillSteal (Only secure kill)"));
            Menu.Add("R.JungleSteal", new CheckBox("Use R to JungleSteal"));

            Menu.AddGroupLabel("Drawing Settings");
            Menu.Add("Drawings.Disable", new CheckBox("Disable all drawings", false));
            Menu.Add("Drawings.W", new CheckBox("Draw W Range"));
            Menu.Add("Drawings.DamageIndicator", new CheckBox("Draw R damage indicator"));
            Menu.Add("Drawings.Target", new CheckBox("Draw circle on target"));
            Menu.Add("Drawings.R.Killable", new CheckBox("Draw text if exists R killable"));

            if (EntityManager.Heroes.Enemies.Count > 0)
            {

                var hash = new HashSet<string>();
                foreach (var h in EntityManager.Heroes.Enemies)
                {
                    hash.Add(h.ChampionName);
                }
                var lastChampName = "";
                foreach (var a in Gapcloser.GapCloserList.Where(h => hash.Contains(h.ChampName)).OrderBy(h => h.ChampName).ThenBy(h => h.SpellSlot))
                {
                    if (AntiGapcloserMenu == null)
                    {
                        AntiGapcloserMenu = Menu.AddSubMenu("Antigapcloser", "Antigapcloser");
                        AntiGapcloserMenu.AddGroupLabel("Gapclose spells to cast E");
                    }
                    if (!a.ChampName.Equals(lastChampName))
                    {
                        AntiGapcloserMenu.AddLabel(a.ChampName);
                        lastChampName = a.ChampName;
                    }
                    if (!AntiGapcloserHashSet.Contains(a.ChampName + a.SpellSlot))
                    {
                        AntiGapcloserMenu.Add(a.ChampName + a.SpellSlot, new CheckBox(a.SpellSlot.ToString(), !(a.ChampName.Contains("Twisted") && a.ChampName.Contains("Pantheon"))));
                        AntiGapcloserHashSet.Add(a.ChampName + a.SpellSlot);
                    }
                }

                foreach (var t in ImmobileTracker.ChampionImmobileSlots.Where(h => EntityManager.Heroes.Enemies.Any(h2 => h2.Hero == h.Key)))
                {
                    if (ImmobileSpellsMenu == null)
                    {
                        ImmobileSpellsMenu = Menu.AddSubMenu("Immobile spells", "Immobile spells");
                        ImmobileSpellsMenu.AddGroupLabel("Enemy spells to cast E");
                    }
                    var enemy = EntityManager.Heroes.Enemies.FirstOrDefault(h => t.Key == h.Hero);
                    if (enemy != null)
                    {
                        ImmobileSpellsMenu.AddLabel(enemy.ChampionName);
                        foreach (var slot in t.Value.Where(slot => !ImmobileSpellsHashSet.Contains(enemy.ChampionName + slot)))
                        {
                            ImmobileSpellsMenu.Add(enemy.ChampionName + slot, new CheckBox(slot.ToString()));
                            ImmobileSpellsHashSet.Add(enemy.ChampionName + slot);
                        }
                    }
                }
            }

        }

        private static void OnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            if (args.NewValue && SpellSlot.R.IsReady() && KillSteal.RKillableBases.Any())
            {
                TapKeyPressed = true;
            }
        }

        public static int GetSliderValue(this Menu m, string s)
        {
            if (m != null)
                return m[s].Cast<Slider>().CurrentValue;
            return -1;
        }

        public static bool GetCheckBoxValue(this Menu m, string s)
        {
            return m != null && m[s].Cast<CheckBox>().CurrentValue;
        }

        public static bool GetKeyBindValue(this Menu m, string s)
        {
            return m != null && m[s].Cast<KeyBind>().CurrentValue;
        }

        public static void AddStringList(this Menu m, string uniqueId, string displayName, string[] values,
            int defaultValue)
        {
            var mode = m.Add(uniqueId, new Slider(displayName, defaultValue, 0, values.Length - 1));
            mode.DisplayName = displayName + ": " + values[mode.CurrentValue];
            mode.OnValueChange +=
                delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                {
                    sender.DisplayName = displayName + ": " + values[args.NewValue];
                };
        }

        public static Menu GetSubMenu(string s)
        {
            return (from t in SubMenu where t.Key.Equals(s) select t.Value).FirstOrDefault();
        }
    }
}