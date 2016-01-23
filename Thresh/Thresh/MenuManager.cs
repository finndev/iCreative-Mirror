using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Thresh
{
    public static class MenuManager
    {
        public static Menu AddonMenu;
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

        public static void Init(EventArgs args)
        {
            var addonName = Champion.AddonName;
            var author = Champion.Author;
            AddonMenu = MainMenu.AddMenu(addonName, addonName + " by " + author + " v1.0000");
            AddonMenu.AddLabel(addonName + " made by " + author);

            SubMenu["Prediction"] = AddonMenu.AddSubMenu("Prediction", "Prediction 2.0");
            SubMenu["Prediction"].AddGroupLabel("Q Settings");
            SubMenu["Prediction"].Add("QCombo", new Slider("Combo HitChancePercent", 70));
            SubMenu["Prediction"].Add("QHarass", new Slider("Harass HitChancePercent", 75));
            SubMenu["Prediction"].AddGroupLabel("E Settings");
            SubMenu["Prediction"].Add("ECombo", new Slider("Combo HitChancePercent", 90));
            SubMenu["Prediction"].Add("EHarass", new Slider("Harass HitChancePercent", 95));

            SubMenu["Combo"] = AddonMenu.AddSubMenu("Combo", "Combo");
            SubMenu["Combo"].AddStringList("Q1", "Use Q1", new[] {"Never", "Only on target", "On any enemy"}, 1);
            SubMenu["Combo"].AddStringList("Q2", "Use Q2",
                new[] {"Never", "Only if hooked the target", "If hooked is near to target"}, 1);
            SubMenu["Combo"].AddStringList("W1", "Use W", new[] {"Never", "Only if target got hooked", "Always"}, 1);
            SubMenu["Combo"].Add("W2", new Slider("Use W on ally if HealthPercent <= {0}", 20));
            SubMenu["Combo"].AddStringList("E1", "E Mode", new[] {"Never", "Pull", "Push", "Based on team HealthPercent" }, 1);
            SubMenu["Combo"].AddStringList("E2", "Use E", new[] {"Never", "Only on target", "On any enemy"}, 3);
            SubMenu["Combo"].Add("R1", new Slider("Use R if HealthPercent <= {0}", 15));
            SubMenu["Combo"].Add("R2", new Slider("Use R if Enemies Inside >= {0}", 3, 0, 5));

            SubMenu["Harass"] = AddonMenu.AddSubMenu("Harass", "Harass");
            SubMenu["Harass"].AddStringList("Q1", "Use Q1", new[] {"Never", "Only on target", "On any enemy"}, 1);
            SubMenu["Harass"].AddStringList("Q2", "Use Q2",
                new[] {"Never", "On ly if hooked the target", "If hooked is near to target"}, 1);
            SubMenu["Harass"].AddStringList("W1", "Use W", new[] {"Never", "Only if target got hooked", "Always"}, 1);
            SubMenu["Harass"].Add("W2", new Slider("Use W on ally if HealthPercent <= {0}", 35));
            SubMenu["Harass"].AddStringList("E1", "E Mode", new[] {"Never", "Pull", "Push", "Based on team HealthPercent" }, 3);
            SubMenu["Harass"].AddStringList("E2", "Use E", new[] {"Never", "Only on target", "On any enemy"}, 3);
            SubMenu["Harass"].Add("Mana", new Slider("Min. Mana Percent:", 20));

            SubMenu["JungleClear"] = AddonMenu.AddSubMenu("JungleClear", "JungleClear");
            SubMenu["JungleClear"].Add("Q", new CheckBox("Use Q"));
            SubMenu["JungleClear"].Add("E", new CheckBox("Use E"));
            SubMenu["JungleClear"].Add("Mana", new Slider("Min. Mana Percent:", 20));

            SubMenu["KillSteal"] = AddonMenu.AddSubMenu("KillSteal", "KillSteal");
            SubMenu["KillSteal"].Add("Q", new CheckBox("Use Q", false));
            SubMenu["KillSteal"].Add("E", new CheckBox("Use E", false));
            SubMenu["KillSteal"].Add("R", new CheckBox("Use R", false));
            SubMenu["KillSteal"].Add("Ignite", new CheckBox("Use Ignite"));

            SubMenu["Flee"] = AddonMenu.AddSubMenu("Flee", "Flee");
            SubMenu["Flee"].Add("W", new CheckBox("Use W on ally"));
            SubMenu["Flee"].Add("E", new CheckBox("Use E to push enemies"));

            SubMenu["Drawings"] = AddonMenu.AddSubMenu("Drawings", "Drawings");
            SubMenu["Drawings"].Add("Disable", new CheckBox("Disable all drawings", false));
            SubMenu["Drawings"].AddSeparator();
            SubMenu["Drawings"].Add("Q", new CheckBox("Draw Q Range"));
            SubMenu["Drawings"].Add("W", new CheckBox("Draw W Range", false));
            SubMenu["Drawings"].Add("E", new CheckBox("Draw E Range"));
            SubMenu["Drawings"].Add("R", new CheckBox("Draw R Range", false));
            SubMenu["Drawings"].Add("Enemy.Target", new CheckBox("Draw circle on enemy target"));
            SubMenu["Drawings"].Add("Ally.Target", new CheckBox("Draw circle on ally target"));

            SubMenu["Misc"] = AddonMenu.AddSubMenu("Misc", "Misc");
            SubMenu["Misc"].Add("W", new Slider("Use W on ally if Enemies Inside >= {0}", 3, 0, 5));
            SubMenu["Misc"].Add("GapCloser.E", new CheckBox("Use E to Interrupt GapClosers (Push or Pull)"));
            SubMenu["Misc"].Add("GapCloser.Q", new CheckBox("Use Q to Interrupt Escapes"));
            SubMenu["Misc"].Add("Interrupter", new CheckBox("Use Q/E to Interrupt Channeling Spells"));
            SubMenu["Misc"].Add("Turret.Q", new CheckBox("Use Q for enemies in turret"));
            SubMenu["Misc"].Add("Turret.E", new CheckBox("Use E for enemies in turret"));
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
            int defaultValue = 0)
        {
            var mode = m.Add(uniqueId, new Slider(displayName, defaultValue, 0, values.Length - 1));
            mode.DisplayName = displayName + ": " + values[mode.CurrentValue];
            mode.OnValueChange +=
                delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
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