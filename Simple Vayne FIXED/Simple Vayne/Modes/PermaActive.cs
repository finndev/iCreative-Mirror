using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace Simple_Vayne.Modes
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PermaActive : ModeBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool ShouldBeExecuted()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Execute()
        {
            if (E.IsReady())
            {
                if (Config.CondemnMenu.Enabled && Config.CondemnMenu.EMode == 0)
                {
                    switch (Config.CondemnMenu.ETargettingMode)
                    {
                        case 0:
                        {
                            foreach (
                                var enemy in
                                    EntityManager.Heroes.Enemies.Where(e => e.IsValidTarget(E.Range))
                                        .Where(enemy => Helpers.CanICastE(enemy, Config.CondemnMenu.PushDistance)))
                            {
                                Helpers.PrintInfoMessage("Casting condemn on <font color=\"#EB54A2\"><b>" +
                                                         enemy.BaseSkinName + "</b></font> to stun him.");
                                E.Cast(enemy);
                            }
                            break;
                        }
                        case 1:
                            {
                                if (Program.CurrentTarget != null)
                                {
                                    if (Helpers.CanICastE(Program.CurrentTarget, Config.CondemnMenu.PushDistance))
                                    {
                                        Helpers.PrintInfoMessage("Casting condemn on <font color=\"#EB54A2\"><b>" + Program.CurrentTarget.BaseSkinName + "</b></font> to stun him.");
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
                if (Config.CondemnMenu.Execute && Program.CurrentTarget != null && Config.CondemnMenu.ExecuteMode == 1 && Program.CurrentTarget.GetSilverStacks() == 2 &&
                    Player.Instance.Position.CountEnemiesInRange(1200) <= Config.CondemnMenu.ExecuteEnemiesNearby)
                {
                    if (Program.CurrentTarget.IsKillableFromSilverStacks() && !Program.CurrentTarget.IsDead)
                    {
                        Helpers.PrintInfoMessage("Casting condemn on <font color=\"#EB54A2\"><b>" +
                                                 Program.CurrentTarget.BaseSkinName + "</b></font> to execute him.");
                        E.Cast(Program.CurrentTarget);
                    }
                }

                var gapclosersQuery = from x in Program.CachedAntiGapclosers
                    where x.Sender.IsValidTarget(E.Range)
                    orderby x.DangerLevel
                    select x;

                var interruptabilespellsQuery = from x in Program.CachedInterruptibleSpells
                    where x.Sender.IsValidTarget(E.Range)
                    orderby x.DangerLevel
                    select x;

                var gapclosers = gapclosersQuery.FirstOrDefault();
                var interruptabilespells = interruptabilespellsQuery.FirstOrDefault();

                if (Config.GapcloserMenu.Enabled && gapclosers != null)
                {
                    var menudata =
                        Config.AntiGapcloserMenuValues.FirstOrDefault(x => x.Champion == gapclosers.Sender.ChampionName);

                    if (menudata == null || !menudata.Enabled)
                        return;

                    if (Player.Instance.HealthPercent <= menudata.PercentHp &&
                        Player.Instance.Position.CountEnemiesInRange(1200) <= menudata.EnemiesNear && gapclosers.Sender.IsECastableOnEnemy())
                    {
                        if (menudata.Delay <= 10)
                        {
                            Helpers.PrintInfoMessage("Casting condemn on <font color=\"#EB54A2\"><b>" +
                                                     gapclosers.Sender.BaseSkinName + "</b></font> to counter his gapcloser.");
                            E.Cast(gapclosers.Sender);
                        }
                        else
                        {
                            Core.DelayAction(delegate
                            {
                                Helpers.PrintInfoMessage("Casting condemn on <font color=\"#EB54A2\"><b>" +
                                                         gapclosers.Sender.BaseSkinName +
                                                         "</b></font> to counter his gapcloser.");
                                E.Cast(gapclosers.Sender);
                            }, menudata.Delay);
                        }
                    }
                }

                if (Config.InterrupterMenu.Enabled && interruptabilespells != null)
                {
                    var menudata =
                        Config.InterrupterMenuValues.FirstOrDefault(x => x.Champion == interruptabilespells.Sender.Hero);

                    if (menudata == null || !menudata.Enabled)
                        return;

                    if (Player.Instance.HealthPercent <= menudata.PercentHp &&
                        Player.Instance.Position.CountEnemiesInRange(1200) <= menudata.EnemiesNear && interruptabilespells.Sender.IsECastableOnEnemy())
                    {
                        if (menudata.Delay <= 10)
                        {
                            Helpers.PrintInfoMessage("Casting condemn on <font color=\"#EB54A2\"><b>" +
                                                     interruptabilespells.Sender.BaseSkinName +
                                                     "</b></font> to counter his interruptible spell.");
                            E.Cast(interruptabilespells.Sender);
                        }
                        else
                        {
                            Core.DelayAction(delegate
                            {
                                Helpers.PrintInfoMessage("Casting condemn on <font color=\"#EB54A2\"><b>" +
                                                         interruptabilespells.Sender.BaseSkinName +
                                                         "</b></font> to counter his interruptible spell.");
                                E.Cast(interruptabilespells.Sender);
                            }, menudata.Delay);
                        }
                    }
                }
            }
            if (Q.IsReady() && Config.TumbleMenu.Backwards)
            {
                var gapclosersQuery = from x in Program.CachedAntiGapclosers
                                      where x.Sender.IsValidTarget(E.Range) && x.DangerLevel == DangerLevel.High
                                      select x;

                var gapclosers = gapclosersQuery.FirstOrDefault();

                if (Config.GapcloserMenu.Enabled && gapclosers != null)
                {
                    var menudata =
                        Config.AntiGapcloserMenuValues.FirstOrDefault(x => x.Champion == gapclosers.Sender.ChampionName);

                    if (menudata == null || !menudata.Enabled)
                        return;

                    if (Player.Instance.HealthPercent <= menudata.PercentHp &&
                        Player.Instance.Position.CountEnemiesInRange(1200) <= menudata.EnemiesNear && gapclosers.Sender.ServerPosition.ExtendPlayerVector().GetTumbleEndPos().Distance(Player.Instance) > 280)
                    {
                        if (menudata.Delay <= 10)
                        {
                            if (Config.TumbleMenu.Backwards && gapclosers.Sender.ServerPosition.ExtendPlayerVector(-300).IsPositionSafe())
                            {
                                Q.Cast(gapclosers.Sender.ServerPosition.ExtendPlayerVector(-280));
                            }
                        }
                        else
                        {
                            Core.DelayAction(delegate
                            {
                                if (Config.TumbleMenu.Backwards && gapclosers.Sender.ServerPosition.ExtendPlayerVector(-300).IsPositionSafe())
                                {
                                    Q.Cast(gapclosers.Sender.ServerPosition.ExtendPlayerVector(-280));
                                }
                            }, menudata.Delay);
                        }
                    }
                }
            }
        }
    }
}