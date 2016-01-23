using System;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;



namespace Syndra
{
    public class Ball
    {
        public Obj_AI_Minion Object;
        public string LastAnimation = "Idle1";
        public Ball(GameObject obj)
        {
            Object = obj as Obj_AI_Minion;
        }
        public Vector3 Position
        {
            get { return ObjectIsValid ? Object.Position : Vector3.Zero; }
        }

        public bool ObjectIsValid
        {
            get { return Object != null && Object.IsValid && !Object.IsDead; }
        }

        public bool IsIdle
        {
            get { return ObjectIsValid && LastAnimation.ToLower().Contains("idle") && Object.IsTargetable; }
        }

        public bool IsWObject
        {
            get { return ObjectIsValid && !Object.IsTargetable; }
        }

        public bool E_WillHit(Obj_AI_Base target)
        {
            if (!EIsOnRange || !target.IsValidTarget(SpellManager.QE.Range)) return false;
            var startPosition = Util.MyHero.Position.To2D() +
                                (Position - Util.MyHero.Position).To2D().Normalized() *
                                Math.Min(Util.MyHero.Distance(Position), SpellManager.E.Range / 2f);
            var info = target.ServerPosition.To2D().ProjectOn(startPosition, EEndPosition.To2D());
            return info.IsOnSegment &&
                   target.ServerPosition.To2D().Distance(info.SegmentPoint, true) <=
                   Math.Pow(1 * (SpellManager.QE.Width + target.BoundingRadius), 2);
        }
        public bool EIsOnTime
        {
            get
            {
                return ObjectIsValid &&
                       Game.Time - SpellManager.E_LastCastTime <=
                       SpellManager.E.CastDelay/1000f + 1.5f*Util.MyHero.Distance(Position)/SpellManager.E.Speed;
            }
        }

        public bool EIsOnRange
        {
            get
            {
                return ObjectIsValid &&
                       Util.MyHero.Distance(Position, true) <=
                       Math.Pow(SpellManager.E.Range + SpellManager.E_ExtraWidth, 2);
            }
        }

        public Vector3 EEndPosition
        {
            get { return Position.E_EndPosition(); }
        }
    }
}
