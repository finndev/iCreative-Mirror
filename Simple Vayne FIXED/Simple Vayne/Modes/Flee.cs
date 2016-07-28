using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using Settings = Simple_Vayne.Config.Modes.Flee;

namespace Simple_Vayne.Modes
{
    public sealed class Flee : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee);
        }

        public override void Execute()
        {
            if (Q.IsReady() && Settings.UseQ)
            {
                var position = Helpers.GetTumbleEndPos(Game.CursorPos.ExtendPlayerVector());

                if (position.Distance(Player.Instance.ServerPosition) > 200)
                {
                    Q.Cast(position);
                }
            }
            if (E.IsReady() && Settings.UseE)
            {
                var enemy = EntityManager.Heroes.Enemies.Where(index => index.IsValidTarget(E.Range)).OrderBy(index => index.Distance(Player.Instance.ServerPosition))
                    .ThenBy(index => index.HealthPercent).FirstOrDefault();

                if (enemy != null)
                {
                    E.Cast(enemy);
                }
            }
        }
    }
}