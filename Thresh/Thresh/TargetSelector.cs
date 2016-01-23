using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace Thresh
{
    public static class TargetSelector
    {
        public static DamageType DamageType;
        public static AIHeroClient ForcedTarget;
        public static AIHeroClient ForcedAlly;
        public static float Range;

        public static AIHeroClient Target
        {
            get
            {
                if (ForcedTarget == null)
                    return EloBuddy.SDK.TargetSelector.GetTarget(Range, DamageType, Util.MyHero.Position);
                return ForcedTarget.IsValidTarget(Range)
                    ? ForcedTarget
                    : EloBuddy.SDK.TargetSelector.GetTarget(Range, DamageType, Util.MyHero.Position);
            }
        }

        public static AIHeroClient Ally
        {
            get
            {
                if (ForcedAlly.IsValidTarget(Range)) return ForcedAlly;
                var ally =
                    EntityManager.Heroes.Allies.Where(h => h.IsValidTarget(Range) && !h.IsMe)
                        .OrderByDescending(h => h.GetPriority() / h.HealthPercent)
                        .FirstOrDefault();
                return ally ?? Util.MyHero;
            }
        }

        public static void Init(float range, DamageType d)
        {
            DamageType = d;
            Range = range;
            Game.OnWndProc += Game_OnWndProc;
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != (uint)WindowMessages.LeftButtonDown) return;
            var target = EloBuddy.SDK.TargetSelector.GetTarget(200f, DamageType, Util.MousePos);
            if (target != null)
            {
                ForcedTarget = target;
            }
            else
            {
                target =
                    EntityManager.Heroes.Allies.Where(h => h.IsValidTarget() && !h.IsMe && Util.MousePos.IsInRange(h, 200))
                        .OrderByDescending(h => h.GetPriority() / h.HealthPercent)
                        .FirstOrDefault();
                if (target != null)
                {
                    ForcedAlly = Ally;
                }
            }
        }
    }
}