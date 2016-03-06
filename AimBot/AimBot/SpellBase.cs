using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace AimBot
{
    public enum SpellType
    {
        Self,
        Targeted,
        Linear,
        Circular,
        Cone,
        Unknown
    }
    public class SpellBase
    {
        private float _speed;

        private float _width;

        public int AllowedCollisionCount = int.MaxValue;
        public bool Aoe;

        public float Delay;
        public bool CollidesWithYasuoWall = true;

        public int LastCastTime;
        public Vector3 LastEndPosition;
        public int LastCastSpellAttempt;
        public Vector3 LastStartPosition;
        public float MinHitChancePercent = 60f;

        public float Range;

        public SpellSlot Slot;
        public SpellType Type;

        public SpellBase(SpellSlot slot, SpellType? type, float range = float.MaxValue)
        {
            Slot = slot;
            Type = type ?? SpellType.Self;
            Range = range;
        }

        public SpellDataInst Instance
        {
            get { return Slot != SpellSlot.Unknown ? Player.Instance.Spellbook.GetSpell(Slot) : null; }
        }

        public float Speed
        {
            get { return _speed > 0 ? _speed : float.MaxValue; }
            set { _speed = value; }
        }

        public float Width
        {
            get { return _width > 0 ? _width : 1; }
            set { _width = value; }
        }

        public string Name
        {
            get { return Slot != SpellSlot.Unknown ? Instance.Name : ""; }
        }

        public bool IsReady
        {
            get { return Slot != SpellSlot.Unknown && Instance.IsReady; }
        }

        public float Mana
        {
            get { return Slot != SpellSlot.Unknown ? Instance.SData.Mana : 0; }
        }

        public float HitChancePercent
        {
            get
            {
                return Program.SlotsSlider[Slot].CurrentValue + (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ? 5f : 0f);
            }
        }

        public bool IsInRange(Obj_AI_Base target)
        {
            switch (Type)
            {
                case SpellType.Targeted:
                    return Player.Instance.IsInRange(target, Range);
                case SpellType.Circular:
                    return Player.Instance.IsInRange(target, Range + Width / 2 + target.BoundingRadius / 2f);
                case SpellType.Linear:
                    return Player.Instance.IsInRange(target, Range + Width);
            }
            //Self
            return Player.Instance.IsInRange(target, Range + target.BoundingRadius / 2f);
        }

        public bool PredictedPosInRange(Obj_AI_Base target, Vector3 position)
        {
            switch (Type)
            {
                case SpellType.Targeted:
                    return Player.Instance.IsInRange(position, Range);
                case SpellType.Circular:
                    return Player.Instance.IsInRange(position, Range + Width / 2f + target.BoundingRadius / 2f);
                case SpellType.Linear:
                    return Player.Instance.IsInRange(position, Range);
            }
            //Self
            return Player.Instance.IsInRange(position, Range + target.BoundingRadius / 2f);
        }
        public PredictionResult GetPrediction(Obj_AI_Base target, Vector3? startPos = null)
        {
            var startPosition = startPos ?? Program.MyHero.Position;
            PredictionResult result;
            switch (Type)
            {
                case SpellType.Circular:
                    result = Prediction.Position.PredictCircularMissile(target, Range, (int)Width, (int)(1000 * Delay), Speed,
                        startPosition);
                    break;
                case SpellType.Cone:
                    result = Prediction.Position.PredictConeSpell(target, Range, (int)Width, (int)(1000 * Delay), Speed,
                        startPosition);
                    break;
                case SpellType.Self:
                    result = Prediction.Position.PredictCircularMissile(target, Range, (int)Width, (int)(1000 * Delay), Speed,
                        startPosition);
                    break;
                default:
                    result = Prediction.Position.PredictLinearMissile(target, Range, (int)Width, (int)(1000 * Delay), Speed,
                        AllowedCollisionCount, startPosition);
                    break;
            }
            return result;
        }

        public bool WillHitYasuoWall(Vector3 position)
        {
            return Speed > 0 && CollidesWithYasuoWall && YasuoWallManager.WillHitYasuoWall(Player.Instance.Position, position);
        }

        public AIHeroClient Target
        {
            get { return TargetSelector.GetTarget(Range, DamageType.Physical, null, true); }
        }

        public void Cast()
        {
            if (Chat.IsOpen || !IsReady || (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && Orbwalker.ShouldWait))
            {
                return;
            }
            AIHeroClient target;
            if ((Program.MyHero.Hero == Champion.Viktor && Slot == SpellSlot.E) || (Program.MyHero.Hero == Champion.Rumble && Slot == SpellSlot.R))
            {
                const float realRange = 525f;
                Range += realRange;
                target = Target;
                if (target != null)
                {
                    var startPos = target.IsInRange(Program.MyHero, realRange) ? target.Position : (Program.MyHero.Position + (target.Position - Program.MyHero.Position).Normalized() * realRange);
                    var pred = GetPrediction(target, startPos);
                    var endPos = startPos + (pred.CastPosition - startPos).Normalized() * (Range - realRange);
                    if (pred.HitChancePercent >= HitChancePercent)
                    {
                        if (WillHitYasuoWall(pred.CastPosition) || !PredictedPosInRange(target, pred.CastPosition))
                        {
                            return;
                        }
                        if (Player.Instance.Spellbook.CastSpell(Slot, endPos, startPos))
                        {
                            LastCastSpellAttempt = Core.GameTickCount;
                        }
                    }
                }
                return;
            }
            target = Target;
            if (target == null)
            {
                return;
            }
            if (!IsInRange(target))
            {
                return;
            }
            if (Type == SpellType.Linear || Type == SpellType.Circular || Type == SpellType.Cone)
            {
                var pred = GetPrediction(target);
                if (pred.HitChancePercent >= HitChancePercent)
                {
                    if (WillHitYasuoWall(pred.CastPosition) || !PredictedPosInRange(target, pred.CastPosition))
                    {
                        return;
                    }
                    if (Player.Instance.Spellbook.CastSpell(Slot, pred.CastPosition))
                    {
                        LastCastSpellAttempt = Core.GameTickCount;
                    }
                }
            }
            else if (Type == SpellType.Targeted)
            {
                if (WillHitYasuoWall(target.ServerPosition))
                {
                    return;
                }
                if (Player.Instance.Spellbook.CastSpell(Slot, target))
                {
                    LastCastSpellAttempt = Core.GameTickCount;
                }
            }
            else if (Type == SpellType.Self)
            {
                var pred = GetPrediction(target);
                if (pred.HitChancePercent >= HitChancePercent)
                {
                    if (!PredictedPosInRange(target, pred.CastPosition))
                    {
                        return;
                    }
                    if (Player.Instance.Spellbook.CastSpell(Slot))
                    {
                        LastCastSpellAttempt = Core.GameTickCount;
                    }
                }
            }
        }

        public void StartCast()
        {
            var target = Target;
            if (target != null)
            {
                if (Chat.IsOpen || !IsReady || !IsInRange(target))
                {
                    return;
                }
                if (Player.Instance.Spellbook.CastSpell(Slot, Game.CursorPos))
                {
                    LastCastSpellAttempt = Core.GameTickCount;
                }
            }
        }
        public void ReleaseCast()
        {
            var target = Target;
            if (target != null)
            {
                if (Chat.IsOpen || !IsReady || !IsInRange(target))
                {
                    return;
                }
                if (Type == SpellType.Linear || Type == SpellType.Circular || Type == SpellType.Cone)
                {
                    var pred = GetPrediction(target);
                    if (pred.HitChancePercent >= HitChancePercent)
                    {
                        if (WillHitYasuoWall(pred.CastPosition) || !PredictedPosInRange(target, pred.CastPosition))
                        {
                            return;
                        }
                        if (Player.Instance.Spellbook.UpdateChargeableSpell(Slot, pred.CastPosition, true))
                        {
                            LastCastSpellAttempt = Core.GameTickCount;
                        }
                    }
                }
            }
        }
        public void Cast(Vector3 position)
        {
            if (Chat.IsOpen || !IsReady)
            {
                return;
            }
            if (Player.Instance.Spellbook.CastSpell(Slot, position))
            {
                LastCastSpellAttempt = Core.GameTickCount;
            }
        }
        
    }
}