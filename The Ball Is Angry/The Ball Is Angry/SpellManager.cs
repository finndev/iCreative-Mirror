using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace The_Ball_Is_Angry
{
    public static class SpellManager
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        public static Spell.Targeted Ignite, Smite;
        public static Spell.Skillshot Flash;

        public static void Initialize()
        {
            W = new Spell.Skillshot(SpellSlot.W, 1500, SkillShotType.Linear, 600, 3300, 60)
            {
                AllowedCollisionCount = 0
            };
            E = new Spell.Skillshot(SpellSlot.E, 900, SkillShotType.Circular, 1200, int.MaxValue, 100)
            {
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Skillshot(SpellSlot.R, 20000, SkillShotType.Linear, 600, 1700, 140)
            {
                AllowedCollisionCount = int.MaxValue
            };
            Game.OnTick += Game_OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe)
            {
            }
        }

        internal static void CastQ()
        {
            if (Q.IsReady())
            {

            }
        }

        internal static void CastW(Obj_AI_Base target)
        {
            if (W.IsReady())
            {
            }
        }

        internal static void CastE(Obj_AI_Base target)
        {
            if (E.IsReady())
            {

            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
        }
        
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                switch (args.Slot)
                {
                    case SpellSlot.Q:
                    case SpellSlot.W:
                        if (args.SData.Name.Equals("JinxW"))
                        {
                        }
                        break;
                    case SpellSlot.E:
                    case SpellSlot.R:
                        break;
                }
            }
        }
    }
}