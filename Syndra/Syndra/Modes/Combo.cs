using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;

namespace Syndra
{
    public static class Combo
    {
        public static Menu Menu
        {
            get { return MenuManager.GetSubMenu("Combo"); }
        }

        public static bool IsActive
        {
            get { return ModeManager.IsCombo; }
        }

        public static void Execute()
        {
            var target = TargetSelector.Target;
            if (!target.IsValidTarget()) return;
            var damageI = target.GetBestCombo();
            if (Menu.GetSliderValue("Zhonyas") > 0 && Menu.GetSliderValue("Zhonyas") >= Util.MyHero.HealthPercent)
            {
                ItemManager.UseZhonyas();
            }
            if (Menu.GetCheckBoxValue("E")) { SpellManager.CastE(target); }
            if (Menu.GetCheckBoxValue("W")) { SpellManager.CastW(target); }
            if (Menu.GetCheckBoxValue("Q")) { SpellManager.CastQ(target); }
            if (Menu.GetCheckBoxValue("WE")) { SpellManager.CastWE(target); }
            if (Menu.GetCheckBoxValue("QE")) { SpellManager.CastQE(target); }
            if (Menu.GetSliderValue("R") > 0 && SpellSlot.R.IsReady())
            {
                var cd = Menu.GetSliderValue("Cooldown");
                var boolean = true;
                switch (Menu.GetSliderValue("R"))
                {
                    case 1:
                        if (damageI.IsKillable && SpellSlot.R.GetSpellDamage(target) >= target.Health)
                        {
                            var qcd = SpellSlot.Q.GetSpellDataInst().Cooldown;
                            if (SpellSlot.Q.IsReady() || qcd < cd) { boolean = false; }
                            if (boolean)
                            {
                                SpellManager.CastR(target);
                            }
                        }
                        break;
                    case 2:
                        if (damageI.IsKillable && SpellSlot.R.GetSpellDamage(target) >= target.Health)
                        {
                            if (!target.GetBestComboR().IsKillable || (Util.MyHero.HealthPercent <= target.HealthPercent && Util.MyHero.HealthPercent <= 40))
                            {
                                SpellManager.CastR(target);
                            }
                        }
                        break;
                    default:
                        SpellManager.CastR(target);
                        break;
                }
            }
        }

    }
}
