using EloBuddy;
using EloBuddy.SDK;

namespace MaddeningJinx
{
    public class DamageResult
    {
        public float Damage;
        public bool E;
        public float Mana;
        public bool Q;
        public bool R;
        public Obj_AI_Base Target;
        public float Time;
        public bool W;

        public DamageResult(Obj_AI_Base target, float dmg, float m, bool q, bool w, bool e, bool r, float t)
        {
            Target = target;
            Q = q;
            W = w;
            E = e;
            R = r;
            Damage = dmg;
            Mana = m;
            Time = t;
        }

        public DamageResult(Obj_AI_Base target, float dmg, float m)
        {
            Target = target;
            Damage = dmg;
            Mana = m;
        }

        public bool IsKillable
        {
            get { return Target.TotalShieldHealth() <= Damage; }
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

        public bool CanKillWith(SpellSlot slot)
        {
            return (slot.GetSpellDamage(Target) >= Target.TotalShieldHealth() || GetBoolFromSlot(slot));
        }
    }
}