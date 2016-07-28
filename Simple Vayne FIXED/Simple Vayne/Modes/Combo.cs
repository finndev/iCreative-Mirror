using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Condemn = Simple_Vayne.Config.CondemnMenu;
using Settings = Simple_Vayne.Config.Modes.Combo;
using Tumble = Simple_Vayne.Config.TumbleMenu;

namespace Simple_Vayne.Modes
{
    /// <summary>
    /// Combo mode
    /// </summary>
    public sealed class Combo : ModeBase
    {
        /// <summary>
        /// ShouldBeExecuted
        /// </summary>
        /// <returns><c>True</c> if combo mode is activated</returns>
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        /// <summary>
        /// Execute
        /// </summary>
        public override void Execute()
        {
            if (E.IsReady() && Condemn.Enabled && Condemn.EMode == 1)
            {
                switch (Condemn.ETargettingMode)
                {
                    case 0:
                    {
                        foreach (var enemy in EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(E.Range)))
                        {
                            if (Helpers.CanICastE(enemy, Condemn.PushDistance))
                            {
                                E.Cast(enemy);
                            }
                        }
                        break;
                    }
                    case 1:
                    {
                        if (Program.CurrentTarget != null)
                        {
                            if (Helpers.CanICastE(Program.CurrentTarget, Condemn.PushDistance))
                            {
                                E.Cast(Program.CurrentTarget);
                            }
                        }
                        break;
                    }
                    default:
                    {
                        break;
                    }
                }
            }
            if (Q.IsReady() && Settings.UseQ)
            {
                if (Tumble.AsGapcloser)
                {
                    foreach (
                        var enemy in
                            EntityManager.Heroes.Enemies.Where(
                                index =>
                                    index.IsValidTarget(1200) && index.Distance(Player.Instance) > Player.Instance.GetAutoAttackRange()+150 && index.ServerPosition.ExtendPlayerVector().IsPositionSafe() && Player.Instance.CountEnemiesInRange(1200) == 1 &&
                                    Player.Instance.GetAutoAttackDamage(index)*2 >= index.Health))
                    {
                        Q.Cast(enemy.ServerPosition.ExtendPlayerVector(200));
                    }
                }
                Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            }
            if (R.IsReady() && Settings.UseR &&
                Player.Instance.CountEnemiesInRange(800) >= Settings.UseRIfEnemiesAreNearby)
            {
                R.Cast();
            }
        }

        private void Orbwalker_OnPostAttack(AttackableUnit target, System.EventArgs args)
        {
            var hero = target as AIHeroClient;

            if (hero == null || hero.IsDead || hero.IsZombie ||
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                (Tumble.To2StacksOnly && hero.GetSilverStacks() != 1))
            {
                Orbwalker.OnPostAttack -= Orbwalker_OnPostAttack;
                return;
            }

            var unit =
                EntityManager.Heroes.Enemies.Where(
                    index => index.IsValidTarget(Player.Instance.GetAutoAttackRange() + 300))
                    .OrderBy(by => by.Distance(Player.Instance.Position.ExtendPlayerVector()))
                    .FirstOrDefault();

            if (unit == null)
            {
                Orbwalker.OnPostAttack -= Orbwalker_OnPostAttack;
                return;
            }

            if (Tumble.Mode == 1)
            {
                var polygons = Helpers.SegmentedAutoattackPolygons();
                var positions = new List<IEnumerable<Vector2>>();
                var enemies = Player.Instance.CountEnemiesInRange(1200);

                for (var i = 0; i < 4; i++)
                {
                    positions.Add(
                        polygons[i].Points.Where(
                            e =>
                                e.ToVector3().ExtendPlayerVector().IsPositionSafe() &&
                                e.ToVector3().ExtendPlayerVector().Distance(unit, true) > (enemies <= 2 ? 150*150 : 300*300)));
                }

                foreach (var points in positions)
                {
                    if (unit.Distance(Player.Instance.ServerPosition, true) < 425 * 425 && unit.Health > Player.Instance.GetAutoAttackDamage(unit) + (Player.Instance.GetAutoAttackDamage(unit) + Player.Instance.GetAutoAttackDamage(unit) * Helpers.QAdditionalDamage[Q.Level]))
                    {
                        foreach (var point in points.OrderByDescending(index => index.Distance(unit, true)))
                        {
                            if (Tumble.BlockQIfEnemyIsOutsideAaRange)
                            {
                                var polygon = new Geometry.Polygon.Circle(point.ToVector3().GetTumbleEndPos(),
                                    Player.Instance.GetAutoAttackRange());

                                if (polygon.IsInside(unit))
                                {
                                    Q.Cast(point.To3DWorld().ExtendPlayerVector(250));
                                }
                            }
                            else
                            {
                                Q.Cast(point.To3DWorld().ExtendPlayerVector(250));
                            }
                        }
                    }
                    else
                    {
                        foreach (var point in points.OrderBy(index => index.Distance(unit, true)))
                        {
                            if (Tumble.BlockQIfEnemyIsOutsideAaRange)
                            {
                                var polygon = new Geometry.Polygon.Circle(point.ToVector3().GetTumbleEndPos(),
                                    Player.Instance.GetAutoAttackRange());

                                if (polygon.IsInside(unit))
                                {
                                    Q.Cast(point.To3DWorld().ExtendPlayerVector(250));
                                }
                            }
                            else
                            {
                                Q.Cast(point.To3DWorld().ExtendPlayerVector(250));
                            }
                        }
                    }
                }
            }
            else
            {
                if (Game.CursorPos.GetTumbleEndPos().IsPositionSafe())
                {
                    Q.Cast(Game.CursorPos.ExtendPlayerVector(250));
                }
            }
            Orbwalker.OnPostAttack -= Orbwalker_OnPostAttack;
        }
    }
}