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

    public class Axe
    {
        public GameObject Reticle = null;
        public MissileClient Missile = null;
        public float StartTime;
        public bool MoveSent = false;
        public static float LimitTime = 1.20f;
        public static float Radius = 100f;
        public static float Offset = 110f;
        public float Gravity
        {
            get
            {
                if (this.Missile != null)
                    return this.Missile.SData.MissileGravity;
                return 26;
            }
        }
        public float Speed
        {
            get
            {
                if (this.Missile != null)
                    return this.Missile.SData.MissileSpeed;
                return 700;
            }
        }
        public float TimeLeft
        {
            get
            {
                if (MissileIsValid || ReticleIsValid)
                    return LimitTime - (Game.Time - this.StartTime);//2 * Extensions.Distance(this.Reticle.Position, this.Missile.Position) / this.Speed; //
                return float.MaxValue;
            }
        }
        public bool SourceInRadius
        {
            get
            {
                if (MissileIsValid || ReticleIsValid)
                    return Extensions.Distance(AxesManager.CatchSource, EndPosition, true) <= Math.Pow(AxesManager.CatchRadius, 2);
                return false;
            }
        }
        public float OffsetTimeNeededToCatchReticle
        {
            get
            {
                return 0f;
                //return Offset / Util.myHero.MoveSpeed;
            }
        }
        public float TimeNeededToCatchReticle
        {
            get
            {
                return Extensions.Distance(StartPosition, EndPosition) / Util.MyHero.MoveSpeed;
            }
        }
        public float TimeToCatchReticle
        {
            get
            {
                var TimeLefts = 0f;
                foreach (Axe a1 in AxesManager.Axes.Where(m => m.InTime && m.TimeLeft < TimeLeft))
                {
                    TimeLefts += a1.TimeLeft;
                }
                return TimeLeft - (TimeLefts + TimeNeededToCatchReticle);
            }
        }
        private float TimeToMakeAutoAttack
        {
            get
            {
                return Util.MyHero.AttackCastDelay;
            }
        }
        public bool CanAttack
        {
            get
            {
                return TimeToCatchReticle - TimeToMakeAutoAttack > 0;
            }
        }
        public bool CanOrbwalkWithUserDelay
        {
            get
            {
                return TimeToCatchReticle - (1 - AxesManager.CatchDelay) * LimitTime - OffsetTimeNeededToCatchReticle > 0;
            }
        }
        public bool CanOrbwalk
        {
            get
            {
                return TimeToCatchReticle - OffsetTimeNeededToCatchReticle > 0;
            }
        }
        public bool CanMove
        {
            get
            {
                return TimeToCatchReticle > 0;
            }
        }
        public float DistanceFromHero
        {
            get
            {
                return Util.MyHero.Distance(AxeCatchPositionFromHero);
            }
        }
        public Vector3 AxeCatchPositionFromHero
        {
            get
            {
                if (HeroInReticle)
                {
                    return Util.MyHero.Position;
                }
                return EndPosition + (Util.MyHero.Position - EndPosition).Normalized() * Radius;
            }
        }
        public bool HeroInReticle
        {
            get
            {
                if (MissileIsValid || ReticleIsValid)
                    return Extensions.Distance(Util.MyHero.Position, EndPosition, true) < Math.Pow(Radius, 2);
                return false;
            }
        }
        public bool InTime
        {
            get
            {
                return Game.Time - this.StartTime <= LimitTime + 0.2f && (ReticleIsValid || MissileIsValid);
            }
        }
        public bool InTurret
        {
            get
            {
                if (MissileIsValid || ReticleIsValid)
                {
                    var turret = EntityManager.Turrets.Enemies.Where(m => m.Health > 0).OrderBy(m => Extensions.Distance(Util.MyHero, m, true)).FirstOrDefault();
                    if (turret != null)
                    {
                        return Util.MyHero.BoundingRadius + 750f >= Extensions.Distance(turret.Position, EndPosition) && AxesManager.Menu.GetCheckBoxValue("Turret");
                    }
                }
                return false;
            }
        }
        public Vector3 StartPosition
        {
            get
            {
                if (!this.IsFirst() && this.AxeBefore() != null)
                {
                    return this.AxeBefore().EndPosition;
                }
                return Util.MyHero.Position;
            }
        }
        public Vector3 EndPosition
        {
            get
            {
                if (ReticleIsValid)
                {
                    return this.Reticle.Position;
                }
                else if (MissileIsValid)
                {
                    return this.Missile.EndPosition;
                }
                return Vector3.Zero;
            }
        }
        public bool ReticleIsValid
        {
            get
            {
                return Reticle != null && Reticle.IsValid;
            }
        }
        public bool MissileIsValid
        {
            get
            {
                return Missile != null && Missile.IsValid;
            }
        }
        public Axe(MissileClient missile)
        {
            this.Reticle = null;
            this.Missile = missile;
            this.StartTime = Game.Time;
        }
        public void AddReticle(GameObject reticle)
        {
            this.Reticle = reticle;
            this.StartTime = Game.Time;
        }

    }
}