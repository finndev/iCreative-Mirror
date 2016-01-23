using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Color = System.Drawing.Color;

namespace Syndra
{
    public static class DamageIndicator
    {
        private static readonly Vector2 BarOffset = new Vector2(3, -6);
        private static readonly Vector2 TextOffset = new Vector2(3, -15);
        private static readonly Color SelectedColor = Color.DarkGoldenrod;
        private const int BarWidth = 104;
        private const int BarHeight = 9;
        public static void Draw()
        {
            foreach (var enemy in EntityManager.Heroes.Enemies.Where(m => m.IsValidTarget() && m.VisibleOnScreen))
            {
                var dmg = enemy.GetBestCombo();
                var health = enemy.Health + enemy.AllShield + enemy.AttackShield + enemy.MagicShield;
                var maxHealth = enemy.MaxHealth + enemy.AllShield + enemy.AttackShield + enemy.MagicShield;
                var afterHealth = (health - dmg.Damage / Damage.Overkill);
                var damagePercent = (afterHealth > 0f ? afterHealth : 0f) / maxHealth;
                var currentHealthPercent = health / maxHealth;
                var barPosition = new Vector2(enemy.HPBarPosition.X + enemy.HPBarXOffset, enemy.HPBarPosition.Y + enemy.HPBarYOffset);
                var barPoint = barPosition + new Vector2(BarOffset.X, BarOffset.Y);
                var startPoint = new Vector2(damagePercent * BarWidth, 0);
                var endPoint = new Vector2(currentHealthPercent * BarWidth, 0);
                Drawing.DrawLine(barPoint + startPoint, barPoint + endPoint, BarHeight, SelectedColor);

                var textPoint = barPosition + new Vector2(TextOffset.X, TextOffset.Y);
                var text = "";
                if (dmg.Q) text += "Q ";
                if (dmg.W) text += "W ";
                if (dmg.E) text += "E ";
                if (dmg.R) text += "R ";
                Drawing.DrawText(textPoint + startPoint, SelectedColor, text, 6);
            }
        }
    }
}
