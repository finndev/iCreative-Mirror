using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Syndra
{
    public static class MenuManager
    {
        public static Menu AddonMenu;
        public static Dictionary<string, Menu> SubMenu = new Dictionary<string, Menu>();
        public static void Init(EventArgs args)
        {
            var addonName = Champion.AddonName;
            var author = Champion.Author;
            AddonMenu = MainMenu.AddMenu(addonName, addonName + " by " + author + " v1.2 ");
            AddonMenu.AddLabel(addonName + " made by " + author);

            SubMenu["Prediction"] = AddonMenu.AddSubMenu("Prediction", "Prediction 2.1");
            SubMenu["Prediction"].AddGroupLabel("Q Settings");
            SubMenu["Prediction"].Add("QCombo", new Slider("Combo HitChancePercent", 65, 0, 100));
            SubMenu["Prediction"].Add("QHarass", new Slider("Harass HitChancePercent", 70, 0, 100));
            SubMenu["Prediction"].AddGroupLabel("W Settings");
            SubMenu["Prediction"].Add("WCombo", new Slider("Combo HitChancePercent", 65, 0, 100));
            SubMenu["Prediction"].Add("WHarass", new Slider("Harass HitChancePercent", 65, 0, 100));
            SubMenu["Prediction"].AddGroupLabel("QE Settings");
            SubMenu["Prediction"].Add("ECombo", new Slider("Combo HitChancePercent", 65, 0, 100));
            SubMenu["Prediction"].Add("EHarass", new Slider("Harass HitChancePercent", 70, 0, 100));


            SubMenu["Combo"] = AddonMenu.AddSubMenu("Combo", "Combo 2");
            SubMenu["Combo"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["Combo"].Add("W", new CheckBox("Use W", true));
            SubMenu["Combo"].Add("E", new CheckBox("Use E", true));
            SubMenu["Combo"].Add("QE", new CheckBox("Use QE", true));
            SubMenu["Combo"].Add("WE", new CheckBox("Use WE", true));
            SubMenu["Combo"].AddStringList("R", "Use R", new[] { "Never", "If killable", "If needed", "Always" }, 1);
            SubMenu["Combo"].Add("Zhonyas", new Slider("Use Zhonyas if HealthPercent <=", 10, 0, 100));
            SubMenu["Combo"].Add("Cooldown", new Slider("Cooldown on spells for R needed", 4, 0, 10));

            SubMenu["Harass"] = AddonMenu.AddSubMenu("Harass", "Harass");
            SubMenu["Harass"].Add("Toggle", new KeyBind("Harass toggle", false, KeyBind.BindTypes.PressToggle, 'K'));
            SubMenu["Harass"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["Harass"].Add("W", new CheckBox("Use W", false));
            SubMenu["Harass"].Add("E", new CheckBox("Use E", false));
            SubMenu["Harass"].Add("QE", new CheckBox("Use QE", false));
            SubMenu["Harass"].Add("WE", new CheckBox("Use WE", false));
            SubMenu["Harass"].Add("Turret", new CheckBox("Don't harass under enemy turret", true));
            SubMenu["Harass"].Add("Mana", new Slider("Min. Mana Percent:", 20, 0, 100));

            SubMenu["LaneClear"] = AddonMenu.AddSubMenu("LaneClear", "LaneClear");
            SubMenu["LaneClear"].Add("Q", new Slider("Use Q if Hit >=", 3, 0, 10));
            SubMenu["LaneClear"].Add("W", new Slider("Use W if Hit >=", 3, 0, 10));
            SubMenu["LaneClear"].AddGroupLabel("Unkillable minions");
            SubMenu["LaneClear"].Add("Q2", new CheckBox("Use Q", true));
            SubMenu["LaneClear"].Add("Mana", new Slider("Min. Mana Percent:", 50, 0, 100));
            
            SubMenu["JungleClear"] = AddonMenu.AddSubMenu("JungleClear", "JungleClear");
            SubMenu["JungleClear"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["JungleClear"].Add("W", new CheckBox("Use W", true));
            SubMenu["JungleClear"].Add("E", new CheckBox("Use E", true));
            SubMenu["JungleClear"].Add("Mana", new Slider("Min. Mana Percent:", 20, 0, 100));

            SubMenu["LastHit"] = AddonMenu.AddSubMenu("LastHit", "LastHit");
            SubMenu["LastHit"].AddGroupLabel("Unkillable minions");
            SubMenu["LastHit"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["LastHit"].Add("Mana", new Slider("Min. Mana Percent:", 50, 0, 100));
            
            SubMenu["KillSteal"] = AddonMenu.AddSubMenu("KillSteal", "KillSteal");
            SubMenu["KillSteal"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["KillSteal"].Add("W", new CheckBox("Use W", true));
            SubMenu["KillSteal"].Add("E", new CheckBox("Use E", true));
            SubMenu["KillSteal"].Add("R", new CheckBox("Use R", false));
            SubMenu["KillSteal"].Add("Ignite", new CheckBox("Use Ignite", true));

            SubMenu["Flee"] = AddonMenu.AddSubMenu("Flee", "Flee");
            SubMenu["Flee"].Add("Movement", new CheckBox("Disable movement", true));
            SubMenu["Flee"].Add("E", new CheckBox("Use QE/WE on enemy near mouse", true));

            SubMenu["Drawings"] = AddonMenu.AddSubMenu("Drawings", "Drawings");
            SubMenu["Drawings"].Add("Disable", new CheckBox("Disable all drawings", false));
            SubMenu["Drawings"].AddSeparator();
            SubMenu["Drawings"].Add("Q", new CheckBox("Draw Q Range", true));
            SubMenu["Drawings"].Add("W", new CheckBox("Draw W Range", false));
            SubMenu["Drawings"].Add("QE", new CheckBox("Draw QE Range", true));
            SubMenu["Drawings"].Add("R", new CheckBox("Draw R Range", false));
            SubMenu["Drawings"].Add("DamageIndicator", new CheckBox("Draw Damage Indicator", true));
            SubMenu["Drawings"].Add("Target", new CheckBox("Draw circle on target", true));
            SubMenu["Drawings"].Add("Killable", new CheckBox("Draw text if enemy is killable", true));
            SubMenu["Drawings"].Add("W.Object", new CheckBox("Draw circle on w object", true));
            SubMenu["Drawings"].Add("Harass.Toggle", new CheckBox("Draw text for harass toggle status", true));
            SubMenu["Drawings"].AddStringList("E.Lines", "Draw lines for E", new[] { "Never", "If will hit enemy", "Always" }, 1);

            SubMenu["Misc"] = AddonMenu.AddSubMenu("Misc", "Misc");
            SubMenu["Misc"].Add("GapCloser", new CheckBox("Use QE/WE to Interrupt GapClosers", true));
            SubMenu["Misc"].Add("Interrupter", new CheckBox("Use QE/WE to Interrupt Channeling Spells", true));
            SubMenu["Misc"].Add("QE.Range", new Slider("Less QE Range", 0, 0, 650));
            SubMenu["Misc"].Add("Overkill", new Slider("Overkill % for damage prediction", 10, 0, 100));
            if (EntityManager.Heroes.Enemies.Count > 0)
            {
                SubMenu["Misc"].AddGroupLabel("Don't use R on");
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    SubMenu["Misc"].Add("Dont.R." + enemy.ChampionName, new CheckBox(enemy.ChampionName, false));
                }
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

        public static void AddStringList(this Menu m, string uniqueId, string displayName, string[] values, int defaultValue)
        {
            var mode = m.Add(uniqueId, new Slider(displayName, defaultValue, 0, values.Length - 1));
            mode.DisplayName = displayName + ": " + values[mode.CurrentValue];
            mode.OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
            {
                sender.DisplayName = displayName + ": " + values[args.NewValue];
            };
        }
        public static Menu GetSubMenu(string s)
        {
            return (from t in SubMenu where t.Key.Equals(s) select t.Value).FirstOrDefault();
        }

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
    }
}
