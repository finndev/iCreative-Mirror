using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace Simple_Vayne
{
    public static class SpellManager
    {
        public static Spell.Skillshot Q { get; private set; }
        public static Spell.Targeted E { get; private set; }
        public static Spell.Active R { get; private set; }

        static SpellManager()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 300, SkillShotType.Linear);
            E = new Spell.Targeted(SpellSlot.E, 650);
            R = new Spell.Active(SpellSlot.R);
        }

        public static void Initialize()
        {

        }
    }
}
