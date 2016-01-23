using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace Thresh
{
    public static class DrawManager
    {
        public static Menu Menu
        {
            get { return MenuManager.GetSubMenu("Drawings"); }
        }

        public static void Init(EventArgs args)
        {
            Drawing.OnDraw += Drawing_OnDraw;
        }


        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Util.MyHero.IsDead) { return; }
            if (Menu.GetCheckBoxValue("Disable")) { return; }
            var target = TargetSelector.Target;
            if (Menu.GetCheckBoxValue("Enemy.Target") && target != null)
            {
                Circle.Draw(Color.Red, 120f, 1, target.Position);
            }
            target = TargetSelector.Ally;
            if (Menu.GetCheckBoxValue("Ally.Target") && target != null)
            {
                Circle.Draw(Color.Blue, 120f, 1, target.Position);
            }
            var color = new ColorBGRA(255, 255, 255, 100);
            if (Menu.GetCheckBoxValue("Q") && SpellSlot.Q.IsReady())
            {
                Circle.Draw(color, SpellManager.Q.Range, Util.MyHero.Position);
            }
            if (Menu.GetCheckBoxValue("W") && SpellSlot.W.IsReady())
            {
                Circle.Draw(color, SpellManager.W.Range, Util.MyHero.Position);
            }
            if (Menu.GetCheckBoxValue("E") && SpellSlot.E.IsReady())
            {
                Circle.Draw(color, SpellManager.E.Range, Util.MyHero.Position);
            }
            if (Menu.GetCheckBoxValue("R") && SpellSlot.R.IsReady())
            {
                Circle.Draw(color, SpellManager.R.Range, Util.MyHero.Position);
            }
        }
    }
}
