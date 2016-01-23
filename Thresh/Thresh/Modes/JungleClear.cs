using EloBuddy.SDK.Menu;

namespace Thresh
{
    public static class JungleClear
    {
        public static bool IsActive
        {
            get
            {
                return ModeManager.IsJungleClear;
            }
        }
        public static Menu Menu
        {
            get
            {
                return MenuManager.GetSubMenu("JungleClear");
            }
        }
        
        public static void Execute()
        {
            if (!(Util.MyHero.ManaPercent >= Menu.GetSliderValue("Mana"))) return;
            var minion = SpellManager.E.JungleClear(false);
            if (Menu.GetCheckBoxValue("E")) { SpellManager.Pull(minion); }
            if (Menu.GetCheckBoxValue("Q")) { SpellManager.CastQ(minion); }
        }
    }
}
