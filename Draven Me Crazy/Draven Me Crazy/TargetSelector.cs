using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;



namespace Draven_Me_Crazy
{
    public static class TargetSelector
    {
        public static DamageType damageType;
        public static AIHeroClient ForcedTarget;
        public static float Range;
        public static void Init(float range, DamageType d)
        {
            Range = range;
            Game.OnWndProc += Game_OnWndProc;
            damageType = d;
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg == (uint)WindowMessages.LeftButtonDown)
            {
                var target = EloBuddy.SDK.TargetSelector.GetTarget(200f, damageType, Util.MousePos);
                if (target.IsValidTarget())
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
                return EloBuddy.SDK.TargetSelector.GetTarget(Range, damageType, Util.MyHero.Position);
            }
        }


    }
}
