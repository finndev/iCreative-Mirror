using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Settings = Simple_Vayne.Config.Modes.Harass;

namespace Simple_Vayne.Modes
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Harass : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        public override void Execute()
        {
            Orbwalker.OnPostAttack += OrbwalkerOnOnPostAttack;
        }

        private void OrbwalkerOnOnPostAttack(AttackableUnit target, EventArgs args)
        {
            var unit = target as AIHeroClient;

            if (unit == null)
            {
                Orbwalker.OnPostAttack -= OrbwalkerOnOnPostAttack;
                return;
            }

            if (Settings.UseQ && Q.IsReady() && unit.GetSilverStacks() >= 0)
            {
                var sidePolygon = Helpers.GetSidePolygons();
                var positions = new List<Vector2>();

                for (var i = 0; i < 2; i++)
                {
                    positions.Add(sidePolygon[i].Points.Where(index => index.ToVector3().ExtendPlayerVector().IsInRange(unit, Player.Instance.GetAutoAttackRange())).OrderByDescending(
                    index => index.Distance(unit.ServerPosition)).FirstOrDefault());
                }
                Q.Cast(positions.OrderByDescending(index => index.Distance(unit)).FirstOrDefault().ToVector3().ExtendPlayerVector(200));
            }

            if (Settings.UseE && E.IsReady() && unit.GetSilverStacks() == 1 && unit.IsECastableOnEnemy())
            {
                SpellManager.E.Cast(unit);
            }
            Orbwalker.OnPostAttack -= OrbwalkerOnOnPostAttack;
        }
    }
}