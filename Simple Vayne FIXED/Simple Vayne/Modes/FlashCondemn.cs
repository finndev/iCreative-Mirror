using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using SharpDX;
using Settings = Simple_Vayne.Config.CondemnMenu;

namespace Simple_Vayne.Modes
{
    public sealed class FlashCondemn : ModeBase
    {
        private float LastTick;

        public override bool ShouldBeExecuted()
        {
            return Settings.FlashCondemn && Program.Flash.IsLearned;
        }

        public override void Execute()
        {
            Orbwalker.MoveTo(Game.CursorPos);

            if (LastTick + 25 > Game.Time*1000)
                return; 
            
            var player = Player.Instance;
            var t = Program.CurrentTarget;

            if (t == null || IsCondemnable(player.ServerPosition.To2D(), t, 450) || !t.IsECastableOnEnemy() || !Program.Flash.IsReady() || t.IsDashing() || !t.IsValidTarget(E.Range))
                return;

            LastTick = Game.Time * 1000;

            var polygons = Helpers.SegmentedAutoattackPolygons();

            foreach (var vector in polygons.SelectMany(point => point.Points).OrderByDescending(x => x.Distance(t)))
            {
                for (var i = -425; i < 425; i += 75)
                {
                    var x = vector.ToVector3().ExtendPlayerVector(i);
                    if (IsCondemnable(x.To2D(), t, 430))
                    {
                        E.Cast(t);
                        Core.DelayAction(() => Program.Flash.Cast(x), 200 + Game.Ping / 2);
                    }
                }
            }
        }

        private static bool IsCondemnable(Vector2 from, Obj_AI_Base unit, int range)
        {
            if(from.ToVector3().GetTumbleEndPos().Distance(unit) < 300 || !from.IsInRange(unit, 425))
                return false;
            
            var position = Prediction.Position.GetPrediction(unit, new Prediction.Position.PredictionData(Prediction.Position.PredictionData.PredictionType.Linear, Settings.PushDistance, 80, 0, 550, 1800)).UnitPosition.To2D();

            for (var i = range; i >= 100; i -= 100)
            {
                var vec = position.Extend(from, -i);

                const int var = 18 * 4 / 100;

                var left = position.Extend(
                    vec + (position - vec).Normalized().Rotated((float) Helpers.ToRadian(Math.Max(0, var)))*
                    Math.Abs(i < 200 ? 50 : 60*4), i);

                var right = position.Extend(
                    vec + (position - vec).Normalized().Rotated((float) Helpers.ToRadian(-Math.Max(0, var)))*
                    Math.Abs(i < 200 ? 50 : 60*4), i);

                if (left.IsWall() && right.IsWall() && vec.IsWall())
                {
                    return true;
                }
            }
            return false;
        }
    }
}