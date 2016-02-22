using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Color = System.Drawing.Color;

namespace MaddeningJinx
{
    public static class DamageIndicator
    {
        private static readonly Vector2 BarOffset = new Vector2(1, 9.2f);
        private static readonly Vector2 TextOffset = new Vector2(1, -15);
        private static readonly Color SelectedColor = Color.FromArgb(170, Color.White);
        private const int BarWidth = 104;
        private const int BarHeight = 9;
        public static void Draw()
        {
            if (SpellSlot.R.IsReady() && Util.MyHero.Mana >= SpellSlot.R.Mana())
            {
                foreach (var enemy in MyTargetSelector.ValidEnemies.Where(m => m.VisibleOnScreen && KillSteal.RDamageOnEnemies.ContainsKey(m.NetworkId)))
                {
                    var health = enemy.TotalShieldHealth();
                    var maxHealth = enemy.MaxHealth + enemy.TotalShield();
                    var damage = KillSteal.RDamageOnEnemies[enemy.NetworkId];
                    var afterHealth = (health - damage);
                    var damagePercent = (afterHealth > 0f ? afterHealth : 0f) / maxHealth;
                    var currentHealthPercent = health / maxHealth;
                    var barPosition = new Vector2(enemy.HPBarPosition.X + enemy.HPBarXOffset, enemy.HPBarPosition.Y + enemy.HPBarYOffset);
                    var barPoint = barPosition + new Vector2(BarOffset.X, BarOffset.Y);
                    var startPoint = new Vector2(damagePercent * BarWidth, 0);
                    var endPoint = new Vector2(currentHealthPercent * BarWidth, 0);
                    Drawing.DrawLine(barPoint + startPoint, barPoint + endPoint, BarHeight, SelectedColor);

                    var textPoint = barPosition + new Vector2(TextOffset.X, TextOffset.Y);
                    var text = Math.Min((int)(damage / health * 100), 100) + "%";
                    Drawing.DrawText(textPoint + startPoint, SelectedColor, text, 6);
                }
            }
        }
    }
}