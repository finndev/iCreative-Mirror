using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Settings = Simple_Vayne.Config.Modes.Jungleclear;
namespace Simple_Vayne.Modes
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class JungleClear : ModeBase
    {
        private static readonly string[] JungleMonsterNames =
        {
            "SRU_Gromp", "SRU_Blue", "SRU_Red", "SRU_Razorbeak", "SRU_Krug", "SRU_Murkwolf", "Sru_Crab", "SRU_RiftHerald", "SRU_Dragon", "SRU_Baron"
        };

        private static readonly string[] JungleCondemnTargets =
        {
            "SRU_Gromp", "SRU_Blue", "SRU_Red", "SRU_Razorbeak", "SRU_Krug", "SRU_Murkwolf", "Sru_Crab"
        };

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool ShouldBeExecuted()
        {
            if (EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition, Player.Instance.GetAutoAttackRange()).Any() && 
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Execute()
        {
            if (Settings.UseQ && Q.IsReady() && Player.Instance.ManaPercent >= Settings.UseQMana)
            {
                Orbwalker.OnPostAttack += OrbwalkerOnOnPostAttack;
            }
            if (Settings.UseE && E.IsReady() && Player.Instance.ManaPercent >= Settings.UseEMana)
            {
                Orbwalker.OnPostAttack += OrbwalkerOnOnPostAttack;
            }
        }

        private void OrbwalkerOnOnPostAttack(AttackableUnit target, EventArgs args)
        {
            var unit = target as Obj_AI_Base;

            if (unit == null || !JungleMonsterNames.Contains(unit.BaseSkinName) || !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                Orbwalker.OnPostAttack -= OrbwalkerOnOnPostAttack;
                return;
            }

            if (Q.IsReady())
            {
                var polygons = Helpers.SegmentedAutoattackPolygons();

                for (var i = 0; i < 4; i++)
                {
                    var pos = polygons[i].Points.Where(index => index.ToVector3().ExtendPlayerVector(200).IsWall())
                        .OrderBy(index => index.Distance(Player.Instance.Position))
                        .FirstOrDefault().ToVector3().ExtendPlayerVector(200);

                    if (pos == Vector3.Zero)
                        continue;

                    Q.Cast(pos);
                }
            }

            if (!E.IsReady())
            {
                Orbwalker.OnPostAttack -= OrbwalkerOnOnPostAttack;
                return;
            }

            var monster = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.ServerPosition,
                E.Range).FirstOrDefault(index => JungleCondemnTargets.Contains(index.BaseSkinName));

            if (monster == null)
            {
                Orbwalker.OnPostAttack -= OrbwalkerOnOnPostAttack;
                return;
            }

            const int range = 430;

            for (var i = 100; i < range; i += 100)
            {
                var vec = monster.ServerPosition.Extend(Player.Instance.ServerPosition, -Math.Min(i, range));
                if (vec.IsWall())
                {
                    E.Cast(monster);
                }
            }
            Orbwalker.OnPostAttack -= OrbwalkerOnOnPostAttack;
        }
    }
}
