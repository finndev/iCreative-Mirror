using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;


namespace Syndra
{
    public static class BallManager
    {
        public static List<Ball> Balls = new List<Ball>();
        public static void Init(EventArgs args)
        {
            Game.OnTick += Game_OnTick;
            GameObject.OnCreate += GameObject_OnCreate;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
        }

        private static void Game_OnTick(EventArgs args)
        {
            Balls.RemoveAll(m => !m.ObjectIsValid);
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsBall())
            {
                Balls.Add(new Ball(sender));
            }
        }
        private static void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!sender.IsBall()) return;
            foreach (var b in Balls.Where(m => m.ObjectIsValid && m.Object.NetworkId == sender.NetworkId))
            {
                b.LastAnimation = args.Animation;
            }
            if (args.Animation.Equals("Death"))
            {
                Balls.RemoveAll(m => m.Object.NetworkId == sender.NetworkId);
            }
        }
        public static bool IsBall(this GameObject obj)
        {
            return obj != null && obj.Name != null && obj is Obj_AI_Minion && obj.IsAlly && obj.Name.Equals("Seed");
        }
        public static bool IsBall(this Obj_AI_Base obj)
        {
            return obj != null && obj.Name != null && obj is Obj_AI_Minion && obj.IsAlly && obj.Name.Equals("Seed");
        }

        public static Vector3 E_StartPosition(this Vector3 position)
        {
            if (Util.MyHero.Distance(position) > SpellManager.E.Range / 3f)
            {
                return position + (Util.MyHero.Position - position).Normalized() * SpellManager.E.Range / 3f;
            }
            return Util.MyHero.Position + (position - Util.MyHero.Position).Normalized() * 10f;
        }
        public static Vector3 E_StartPosition2(this Vector3 position)
        {
            return Util.MyHero.Position + (position - Util.MyHero.Position).Normalized() * (SpellManager.E.Range + SpellManager.E_ExtraWidth);
        }
        public static Vector3 E_EndPosition(this Vector3 position)
        {
            return Util.MyHero.Position + (position.E_StartPosition() - Util.MyHero.Position).Normalized() * ((Util.MyHero.Distance(position.E_StartPosition(), true) >= Math.Pow(200, 2)) ? SpellManager.QE.Range : 1000);
        }
    }
}
