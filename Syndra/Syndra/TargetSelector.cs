using EloBuddy;
using EloBuddy.SDK;


namespace Syndra
{
    public static class TargetSelector
    {
        public static DamageType DamageType;
        public static AIHeroClient ForcedTarget;
        public static float Range;
        public static void Init(float range, DamageType d)
        {
            DamageType = d;
            Range = range;
            Game.OnWndProc += Game_OnWndProc;
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowMessages.LeftButtonDown)
            {
                var target = EloBuddy.SDK.TargetSelector.GetTarget(200f, DamageType, Util.MousePos);
                if (target != null)
                {
                    ForcedTarget = target;
                }
            }
        }
        public static AIHeroClient Target
        {
            get
            {
                if (ForcedTarget != null)
                {
                    if (ForcedTarget.IsValidTarget(Range))
                    {
                        return ForcedTarget;
                    }
                }
                return EloBuddy.SDK.TargetSelector.GetTarget(Range, DamageType, Util.MyHero.Position);
            }
        }


    }
}
