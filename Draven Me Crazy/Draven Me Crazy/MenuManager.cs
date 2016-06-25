using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace Draven_Me_Crazy
{
    public static class MenuManager
    {
        public static Menu AddonMenu;
        public static Dictionary<string, Menu> SubMenu = new Dictionary<string, Menu>() { };
        public static void Init(EventArgs args)
        {
            var AddonName = Champion.AddonName;
            var Author = Champion.Author;
            AddonMenu = MainMenu.AddMenu(AddonName, AddonName + " by " + Author + " v1 ");
            AddonMenu.AddLabel(AddonName + " made by " + Author);


            SubMenu["Axes"] = AddonMenu.AddSubMenu("Axes", "Axes");
            SubMenu["Axes"].AddGroupLabel("Keys");
            SubMenu["Axes"].Add("Catch", new KeyBind("Catch Axes (Toggle)", true, KeyBind.BindTypes.PressToggle, (uint)'L'));
            SubMenu["Axes"].AddGroupLabel("Settings");
            SubMenu["Axes"].Add("Click", new CheckBox("Use double left click to disable catching the current axe", true));
            SubMenu["Axes"].AddSeparator(0);
            SubMenu["Axes"].Add("W", new CheckBox("Use W to Catch (Smart)", true));
            SubMenu["Axes"].Add("Turret", new CheckBox("Don't catch under turret", true));
            SubMenu["Axes"].Add("Delay", new Slider("% of delay to catch the axe", 100, 0, 100));
            SubMenu["Axes"].AddStringList("CatchMode", "Catch Condition", new[] { "When Orbwalking", "AutoCatch" }, 0);
            SubMenu["Axes"].AddStringList("OrbwalkMode", "Catch Mode", new[] { "My Hero in radius", "Mouse in radius" }, 0);
            SubMenu["Axes"].AddGroupLabel("Catch radius");
            SubMenu["Axes"].Add("Combo", new Slider("Combo radius", 250, 150, 600));
            SubMenu["Axes"].Add("Harass", new Slider("Harass radius", 350, 150, 600));
            SubMenu["Axes"].Add("Clear", new Slider("Clear radius", 400, 150, 800));
            SubMenu["Axes"].Add("LastHit", new Slider("LastHit radius", 400, 150, 800));

            SubMenu["Combo"] = AddonMenu.AddSubMenu("Combo", "Combo");
            SubMenu["Combo"].Add("Items", new CheckBox("Use Items", true));
            SubMenu["Combo"].Add("Q", new Slider("Use Q to have X spinning axes", 3, 0, 3));
            SubMenu["Combo"].Add("W", new CheckBox("Use W", true));
            SubMenu["Combo"].Add("E", new CheckBox("Use E", true));
            SubMenu["Combo"].Add("R", new CheckBox("Use R if killable", true));
            SubMenu["Combo"].Add("R2", new Slider("Use R if hit X enemies", 3, 1, 5));

            SubMenu["Harass"] = AddonMenu.AddSubMenu("Harass", "Harass");
            SubMenu["Harass"].Add("AA", new CheckBox("Use AA if will be lose the spin", true));
            SubMenu["Harass"].Add("Q", new Slider("Use Q to have X spinning axes", 1, 0, 2));
            SubMenu["Harass"].Add("W", new CheckBox("Use W", false));
            SubMenu["Harass"].Add("E", new CheckBox("Use E", false));
            SubMenu["Harass"].Add("Mana", new Slider("Min. Mana Percent:", 20, 0, 100));

            SubMenu["LaneClear"] = AddonMenu.AddSubMenu("LaneClear", "LaneClear");
            SubMenu["LaneClear"].Add("Q", new Slider("Use Q to have X spinning axes", 1, 0, 2));
            SubMenu["LaneClear"].Add("Mana", new Slider("Min. Mana Percent:", 20, 0, 100));

            SubMenu["LastHit"] = AddonMenu.AddSubMenu("LastHit", "LastHit");
            SubMenu["LastHit"].Add("Q", new Slider("Use Q to have X spinning axes", 1, 0, 2));
            SubMenu["LastHit"].Add("Mana", new Slider("Min. Mana Percent:", 20, 0, 100));

            SubMenu["JungleClear"] = AddonMenu.AddSubMenu("JungleClear", "JungleClear");
            SubMenu["JungleClear"].Add("Q", new Slider("Use Q to have X spinning axes", 1, 0, 2));
            SubMenu["JungleClear"].Add("W", new CheckBox("Use W", true));
            SubMenu["JungleClear"].Add("E", new CheckBox("Use E", true));
            SubMenu["JungleClear"].Add("Mana", new Slider("Min. Mana Percent:", 20, 0, 100));

            SubMenu["KillSteal"] = AddonMenu.AddSubMenu("KillSteal", "KillSteal");
            SubMenu["KillSteal"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["KillSteal"].Add("W", new CheckBox("Use W", true));
            SubMenu["KillSteal"].Add("E", new CheckBox("Use E", true));
            SubMenu["KillSteal"].Add("R", new CheckBox("Use R", true));
            SubMenu["KillSteal"].Add("Ignite", new CheckBox("Use Ignite", true));

            SubMenu["Flee"] = AddonMenu.AddSubMenu("Flee", "Flee");
            SubMenu["Flee"].Add("W", new CheckBox("Use W", true));
            SubMenu["Flee"].Add("E", new CheckBox("Use E", true));

            SubMenu["Drawings"] = AddonMenu.AddSubMenu("Drawings", "Drawings");
            SubMenu["Drawings"].Add("Disable", new CheckBox("Disable all drawings", false));
            SubMenu["Drawings"].Add("Target", new CheckBox("Draw circle on target", true));
            SubMenu["Drawings"].Add("Axes", new CheckBox("Draw catch radius", true));

            SubMenu["Misc"] = AddonMenu.AddSubMenu("Misc", "Misc");
            SubMenu["Misc"].Add("GapCloser", new CheckBox("Use E to Interrupt GapClosers", true));
            SubMenu["Misc"].Add("Interrupter", new CheckBox("Use E to Interrupt Channeling Spells", true));
            SubMenu["Misc"].Add("Overkill", new Slider("Overkill % for damage prediction", 10, 0, 100));
            SubMenu["Misc"].Add("RRange", new Slider("R Range", 1800, 300, 6000));
            SubMenu["Misc"].Add("ERange", new Slider("E Range", 1050, 100, 1050));

        }
        
        public static int GetSliderValue(this Menu m, string s)
        {
            if (m != null)
                return m[s].Cast<Slider>().CurrentValue;
            return -1;
        }
        public static bool GetCheckBoxValue(this Menu m, string s)
        {
            if (m != null)
                return m[s].Cast<CheckBox>().CurrentValue;
            return false;
        }
        public static bool GetKeyBindValue(this Menu m, string s)
        {
            if (m != null)
                return m[s].Cast<KeyBind>().CurrentValue;
            return false;
        }
        public static void AddStringList(this Menu m, string uniqueID, string DisplayName, string[] values, int defaultValue)
        {
            var mode = m.Add(uniqueID, new Slider(DisplayName, defaultValue, 0, values.Length - 1));
            mode.DisplayName = DisplayName + ": " + values[mode.CurrentValue];
            mode.OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
            {
                sender.DisplayName = DisplayName + ": " + values[args.NewValue];
            };
        }
        public static Menu GetSubMenu(string s)
        {
            foreach (KeyValuePair<string, Menu> t in SubMenu)
            {
                if (t.Key.Equals(s))
                {
                    return t.Value;
                }
            }
            return null;
        }
        public static Menu MiscMenu
        {
            get
            {
                return GetSubMenu("Misc");
            }
        }
        public static Menu PredictionMenu
        {
            get
            {
                return GetSubMenu("Prediction");
            }
        }
        public static Menu DrawingsMenu
        {
            get
            {
                return GetSubMenu("Drawings");
            }
        }
    }
}
