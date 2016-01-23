using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace The_Ball_Is_Angry
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
        

        public static void Initialize()
        {
            Menu = MainMenu.AddMenu(Champion.AddonName, Champion.AddonName + " by " + Champion.Author);

            Menu.AddLabel(Champion.AddonName + " made by " + Champion.Author);

            SubMenu["Prediction"] = Menu.AddSubMenu("Prediction", "Prediction 2.1");
            SubMenu["Prediction"].AddGroupLabel("Q Settings");
            SubMenu["Prediction"].Add("QCombo", new Slider("Combo HitChancePercent", 65, 0, 100));
            SubMenu["Prediction"].Add("QHarass", new Slider("Harass HitChancePercent", 75, 0, 100));
            SubMenu["Prediction"].AddGroupLabel("W Settings");
            SubMenu["Prediction"].Add("WCombo", new Slider("Combo HitChancePercent", 70, 0, 100));
            SubMenu["Prediction"].Add("WHarass", new Slider("Harass HitChancePercent", 75, 0, 100));
            SubMenu["Prediction"].AddGroupLabel("E Settings");
            SubMenu["Prediction"].Add("ECombo", new Slider("Combo HitChancePercent", 45, 0, 100));
            SubMenu["Prediction"].Add("EHarass", new Slider("Harass HitChancePercent", 60, 0, 100));
            SubMenu["Prediction"].AddGroupLabel("R Settings");
            SubMenu["Prediction"].Add("RCombo", new Slider("Combo HitChancePercent", 65, 0, 100));
            SubMenu["Prediction"].Add("RHarass", new Slider("Harass HitChancePercent", 75, 0, 100));

            SubMenu["Combo"] = Menu.AddSubMenu("Combo", "Combo");
            SubMenu["Combo"].Add("TF", new Slider("Use TeamFight Logic if enemies near >=", 3, 1, 5));
            SubMenu["Combo"].AddGroupLabel("Common Logic");
            SubMenu["Combo"].Add("Q", new CheckBox("Use Q On Target", true));
            SubMenu["Combo"].Add("W", new CheckBox("Use W On Target", true));
            SubMenu["Combo"].Add("Shield", new CheckBox("Use Shield On Enemy Missiles", true));
            SubMenu["Combo"].Add("E", new Slider("Use E If Hit", 1, 1, 5));
            SubMenu["Combo"].Add("E2", new Slider("Use E If HealthPercent <=", 50, 0, 100));
            SubMenu["Combo"].AddGroupLabel("1 vs 1 Logic");
            SubMenu["Combo"].Add("R", new CheckBox("Use R On Target If Killable", true));
            SubMenu["Combo"].AddGroupLabel("TeamFight Logic");
            SubMenu["Combo"].Add("Q2", new Slider("Use Q If Hit", 2, 1, 5));
            SubMenu["Combo"].Add("W2", new Slider("Use W If Hit", 2, 1, 5));
            SubMenu["Combo"].Add("R2", new Slider("Use R if Hit", 3, 1, 5));

            SubMenu["Harass"] = Menu.AddSubMenu("Harass", "Harass");
            SubMenu["Harass"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["Harass"].Add("W", new CheckBox("Use W", true));
            SubMenu["Harass"].Add("Shield", new CheckBox("Use Shield On Enemy Missiles", true));
            SubMenu["Harass"].Add("E", new Slider("Use E If Hit", 1, 1, 5));
            SubMenu["Harass"].Add("E2", new Slider("Use E If HealthPercent <=", 40, 0, 100));
            SubMenu["Harass"].Add("Mana", new Slider("Min. Mana Percent:", 20, 0, 100));

            SubMenu["LaneClear"] = Menu.AddSubMenu("LaneClear", "LaneClear");
            SubMenu["LaneClear"].AddGroupLabel("LaneClear Minions");
            SubMenu["LaneClear"].Add("Q", new Slider("Use Q If Hit", 4, 0, 10));
            SubMenu["LaneClear"].Add("W", new Slider("Use W If Hit", 3, 0, 10));
            SubMenu["LaneClear"].Add("E", new Slider("Use E If Hit", 6, 0, 10));
            SubMenu["LaneClear"].AddGroupLabel("Unkillable minions");
            SubMenu["LaneClear"].Add("Q2", new CheckBox("Use Q", true));
            SubMenu["LaneClear"].Add("Mana", new Slider("Min. Mana Percent:", 50, 0, 100));

            SubMenu["LastHit"] = Menu.AddSubMenu("LastHit", "LastHit");
            SubMenu["LastHit"].AddGroupLabel("Unkillable minions");
            SubMenu["LastHit"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["LastHit"].Add("Mana", new Slider("Min. Mana Percent:", 50, 0, 100));

            SubMenu["JungleClear"] = Menu.AddSubMenu("JungleClear", "JungleClear");
            SubMenu["JungleClear"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["JungleClear"].Add("W", new CheckBox("Use W", true));
            SubMenu["JungleClear"].Add("E", new CheckBox("Use E", true));
            SubMenu["JungleClear"].Add("Mana", new Slider("Min. Mana Percent:", 20, 0, 100));

            SubMenu["KillSteal"] = Menu.AddSubMenu("KillSteal", "KillSteal");
            SubMenu["KillSteal"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["KillSteal"].Add("W", new CheckBox("Use W", true));
            SubMenu["KillSteal"].Add("E", new CheckBox("Use E", true));
            SubMenu["KillSteal"].Add("R", new CheckBox("Use R", false));
            SubMenu["KillSteal"].Add("Ignite", new CheckBox("Use Ignite", true));

            SubMenu["Flee"] = Menu.AddSubMenu("Flee", "Flee");
            SubMenu["Flee"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["Flee"].Add("W", new CheckBox("Use W", true));
            SubMenu["Flee"].Add("E", new CheckBox("Use E", true));

            SubMenu["Drawings"] = Menu.AddSubMenu("Drawings", "Drawings");
            SubMenu["Drawings"].Add("Ball", new CheckBox("Draw ball position", true));
            SubMenu["Drawings"].Add("Q", new CheckBox("Draw Q Range", true));
            SubMenu["Drawings"].Add("W", new CheckBox("Draw W Range", true));
            SubMenu["Drawings"].Add("R", new CheckBox("Draw R Range", true));

            SubMenu["Misc"] = Menu.AddSubMenu("Misc", "Misc");
            SubMenu["Misc"].Add("Overkill", new Slider("Overkill % for damage prediction", 10, 0, 100));
            SubMenu["Misc"].Add("BlockR", new CheckBox("Block R if will not hit", true));
            SubMenu["Misc"].Add("R", new CheckBox("Use R to Interrupt Channeling", true));
            SubMenu["Misc"].Add("E", new CheckBox("Use E to Initiate", true));
            SubMenu["Misc"].Add("Shield", new CheckBox("Use Shield On Enemy Missiles", false));
            SubMenu["Misc"].Add("W2", new Slider("Use W if Hit", 3, 1, 5));
            SubMenu["Misc"].Add("R2", new Slider("Use R if Hit", 4, 1, 5));
            SubMenu["Misc"].AddGroupLabel("Don't use R in:");
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                SubMenu["Misc"].Add("BlackList." + enemy.ChampionName, new CheckBox(enemy.ChampionName, false));
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