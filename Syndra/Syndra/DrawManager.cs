using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace Syndra
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
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (Util.MyHero.IsDead) { return; }
            if (Menu.GetCheckBoxValue("Disable")) { return; }
            if (Menu.GetCheckBoxValue("DamageIndicator"))
            {
                DamageIndicator.Draw();
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Util.MyHero.IsDead) { return; }
            if (Menu.GetCheckBoxValue("Disable")) { return; }
            var target = TargetSelector.Target;
            if (Menu.GetCheckBoxValue("Target") && target.IsValidTarget())
            {
                Circle.Draw(Color.Red, 120f, 5, target.Position);
            }
            if (Menu.GetSliderValue("E.Lines") > 0 && SpellSlot.E.IsReady())
            {
                switch (Menu.GetSliderValue("E.Lines"))
                {
                    case 1:
                        foreach (var b in from b in BallManager.Balls.Where(m => m.IsIdle && m.ObjectIsValid && m.EIsOnRange) from enemy in EntityManager.Heroes.Enemies.Where(b.E_WillHit) select b)
                        {
                            Drawing.DrawLine(b.Position.E_StartPosition().WorldToScreen(), b.EEndPosition.WorldToScreen(),
                                SpellManager.QE.Width, System.Drawing.Color.FromArgb(100, 255, 255, 255));
                        }
                        break;
                    case 2:
                        foreach (var b in BallManager.Balls.Where(m => m.IsIdle && m.ObjectIsValid && m.EIsOnRange))
                        {
                            Drawing.DrawLine(b.Position.E_StartPosition().WorldToScreen(), b.EEndPosition.WorldToScreen(),
                                SpellManager.QE.Width, System.Drawing.Color.FromArgb(100, 255, 255, 255));
                        }
                        break;
                }
            }
            if (Menu.GetCheckBoxValue("W.Object") && SpellManager.WObject != null)
            {
                Circle.Draw(Color.Blue, SpellManager.W_Width1, 1, SpellManager.WObject.Position);
            }
            if (Menu.GetCheckBoxValue("Killable") && SpellSlot.R.IsReady())
            {
                foreach (var enemy in EntityManager.Heroes.Enemies.Where(m => m.IsValidTarget(SpellManager.R.Range) && SpellSlot.R.GetSpellDamage(m) >= m.Health))
                {
                    Drawing.DrawText(enemy.Position.WorldToScreen(), System.Drawing.Color.Red, "R KILLABLE", 15);
                }
            }
            if (Menu.GetCheckBoxValue("Harass.Toggle"))
            {
                Drawing.DrawText(Util.MyHero.Position.WorldToScreen() - new Vector2(50, 0), System.Drawing.Color.White, "Harass Toggle: " + (Harass.Menu.GetKeyBindValue("Toggle") ? "ON": "OFF"), 15);
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
            if (Menu.GetCheckBoxValue("QE") && SpellSlot.E.IsReady())
            {
                Circle.Draw(color, SpellManager.QE.Range, Util.MyHero.Position);
            }
            if (Menu.GetCheckBoxValue("R") && SpellSlot.R.IsReady())
            {
                Circle.Draw(color, SpellManager.R.Range, Util.MyHero.Position);
            }
        }
    }
}
