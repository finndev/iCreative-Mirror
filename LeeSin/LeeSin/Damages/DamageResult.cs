using EloBuddy;
using EloBuddy.SDK;

namespace LeeSin
{
    public class DamageResult
    {
        public bool Q = false;
        public bool W = false;
        public bool E = false;
        public bool R = false;
        public float Damage = 0f;
        public float Mana = 0f;
        public float Time = 0f;
        public Obj_AI_Base Target = null;

        public DamageResult(Obj_AI_Base target, float dmg, float m, bool q, bool w, bool e, bool r, float t)
        {
            this.Target = target;
            this.Q = q;
            this.W = w;
            this.E = e;
            this.R = r;
            this.Damage = dmg;
            this.Mana = m;
            this.Time = t;
        }
        public DamageResult(Obj_AI_Base target, float dmg, float m)
        {
            this.Target = target;
            this.Damage = dmg;
            this.Mana = m;
        }
        private bool GetBoolFromSlot(SpellSlot slot)
        {
            switch (slot)
            {
                case SpellSlot.Q:
                    return Q;
                case SpellSlot.W:
                    return W;
                case SpellSlot.E:
                    return E;
                case SpellSlot.R:
                    return R;
            }
            return false;
        }
        public bool IsKillable
        {
            get
            {
                return Target.IsValidTarget() && Target.TotalShieldHealth() <= Damage;
            }
        }
        public bool CanKillWith(SpellSlot slot)
        {
            return Target.IsValidTarget() && (slot.GetSpellDamage(Target) >= Target.TotalShieldHealth() || GetBoolFromSlot(slot));
        }
    }
}
