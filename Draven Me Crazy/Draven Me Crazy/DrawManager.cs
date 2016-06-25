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
    public static class DrawManager
    {
        public static Menu Menu
        {
            get
            {
                return MenuManager.GetSubMenu("Drawings");
            }
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
            if (MenuManager.DrawingsMenu.GetCheckBoxValue("Target") && target.IsValidTarget())
            {
                Circle.Draw(Color.Red, 120f, 5, target.Position);
            }

            if (Menu.GetCheckBoxValue("Axes") && AxesManager.CatchEnabled)
            {
                foreach (Axe a in AxesManager.Axes.Where(m => m.InTime && !m.InTurret))
                {
                    var trans = a.SourceInRadius ? 100 : 10;
                    var width = a.SourceInRadius ? 5 : 1;
                    Circle.Draw(new ColorBGRA(0, 0, 255, trans), AxesManager.CatchRadius, width, a.EndPosition);
                }
            }
        }
    }
}
