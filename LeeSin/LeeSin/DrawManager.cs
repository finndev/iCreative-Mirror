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

namespace LeeSin
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
        public static void Init()
        {
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Util.MyHero.IsDead) { return; }
            if (Menu.GetCheckBoxValue("Disable")) { return; }
            if (Menu.GetCheckBoxValue("Combo.Mode"))
            {
                var pos = Util.MyHero.Position.WorldToScreen();
                pos.X = pos.X - 50;
                Drawing.DrawText(pos, System.Drawing.Color.White, Combo.Menu["Mode"].Cast<Slider>().DisplayName, 15);
            }
            var target = TargetSelector.Target;
            if (MenuManager.DrawingsMenu.GetCheckBoxValue("Target") && target.IsValidTarget())
            {
                Circle.Draw(Color.Red, 120f, 5, target.Position);
            }
            if (Menu.GetCheckBoxValue("Insec.Line") && Insec.IsReady)
            {
                if (target.IsValidTarget())
                {
                    var blue = System.Drawing.Color.FromArgb(140, 0, 0, 255);
                    var startpos = target.Position;
                    var endpos = Insec.EndPosition;
                    var endpos1 = Insec.EndPosition + (startpos - endpos).To2D().Normalized().Rotated(45 * (float)Math.PI / 180).To3D() * target.BoundingRadius;
                    var endpos2 = Insec.EndPosition + (startpos - endpos).To2D().Normalized().Rotated(-45 * (float)Math.PI / 180).To3D() * target.BoundingRadius;
                    var width = 5;
                    Drawing.DrawLine(startpos.WorldToScreen(), endpos.WorldToScreen(), width, blue);
                    Drawing.DrawLine(endpos.WorldToScreen(), endpos1.WorldToScreen(), width, blue);
                    Drawing.DrawLine(endpos.WorldToScreen(), endpos2.WorldToScreen(), width, blue);
                }
            }
            var color = new ColorBGRA(255, 255, 255, 100);
            if (Menu.GetCheckBoxValue("Q") && SpellSlot.Q.IsReady())
            {
                Circle.Draw(color, SpellManager.QRange, Util.MyHero.Position);
            }
            if (Menu.GetCheckBoxValue("W") && SpellSlot.W.IsReady())
            {
                Circle.Draw(color, SpellManager.WRange, Util.MyHero.Position);
            }
            if (Menu.GetCheckBoxValue("E") && SpellSlot.E.IsReady())
            {
                Circle.Draw(color, SpellManager.ERange, Util.MyHero.Position);
            }
            if (Menu.GetCheckBoxValue("R") && SpellSlot.R.IsReady())
            {
                Circle.Draw(color, SpellManager.R.Range, Util.MyHero.Position);
            }
        }
    }
}
