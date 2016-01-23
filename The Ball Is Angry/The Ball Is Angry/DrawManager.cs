using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;

namespace The_Ball_Is_Angry
{
    public static class DrawManager
    {
        private static readonly ColorBGRA WhiteColor = new ColorBGRA(255, 255, 255, 100);

        public static Menu Menu
        {
            get { return MenuManager.Menu; }
        }

        public static void Initialize()
        {
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (Util.MyHero.IsDead)
            {
                return;
            }
            if (Menu.GetCheckBoxValue("Drawings.Disable"))
            {
                return;
            }
            if (Menu.GetCheckBoxValue("Drawings.DamageIndicator"))
            {
                DamageIndicator.Draw();
            }
            if (Menu.GetCheckBoxValue("Drawings.R.Killable"))
            {

            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Util.MyHero.IsDead)
            {
                return;
            }
            if (Menu.GetCheckBoxValue("Drawings.Disable"))
            {
                return;
            }
            var target = MyTargetSelector.Target;
            if (Menu.GetCheckBoxValue("Drawings.Target") && target != null)
            {
                Circle.Draw(SharpDX.Color.Red, 120f, 5, target);
            }
            if (Orbwalker.DrawRange && SpellSlot.Q.IsReady())
            {
            }
            if (Menu.GetCheckBoxValue("Drawings.W") && SpellSlot.W.IsReady())
            {
                Circle.Draw(WhiteColor, SpellManager.W.Range, Util.MyHero);
            }
        }
    }
}