using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace LeeSin
{
    public static class SpellManager
    {
        public static Spell.Skillshot Q1, W1, RKick, E1, E2;
        public static Spell.Targeted R;
        public static Spell.Active Q2, W2;
        public static Spell.Targeted Ignite, Smite;
        public static Spell.Skillshot Flash;
        public static float W1Range = 700f;
        public static float WExtraRange = 120f;
        public static float SmiteCastDelay = 0f;
        public static float WLastCastTime, FlashLastCastTime;
        public static void Init()
        {
            Q1 = new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Linear, 250, 1800, 60)
            {
                AllowedCollisionCount = 0
            };
            Q2 = new Spell.Active(SpellSlot.Q, 1300);

            W1 = new Spell.Skillshot(SpellSlot.W, 1200, SkillShotType.Linear, 50, 1500, 100)
            {
                AllowedCollisionCount = int.MaxValue
            };

            W2 = new Spell.Active(SpellSlot.W, 700);

            E1 = new Spell.Skillshot(SpellSlot.E, 350, SkillShotType.Linear, 250, int.MaxValue, 100)
            {
                AllowedCollisionCount = int.MaxValue
            };
            E2 = new Spell.Skillshot(SpellSlot.E, 675, SkillShotType.Linear, 250, int.MaxValue, 100)
            {
                AllowedCollisionCount = int.MaxValue
            };

            R = new Spell.Targeted(SpellSlot.R, 375);
            RKick = new Spell.Skillshot(SpellSlot.R, 275 + 550, SkillShotType.Linear, 400, 600, 75)
            {
                AllowedCollisionCount = int.MaxValue
            };
            var slot = Util.MyHero.SpellSlotFromName("summonerdot");
            if (slot != SpellSlot.Unknown)
            {
                Ignite = new Spell.Targeted(slot, 600);
            }
            slot = Util.MyHero.SpellSlotFromName("smite");
            if (slot != SpellSlot.Unknown)
            {
                Smite = new Spell.Targeted(slot, 500);
            }
            slot = Util.MyHero.SpellSlotFromName("flash");
            if (slot != SpellSlot.Unknown)
            {
                Flash = new Spell.Skillshot(slot, 400, SkillShotType.Circular);
            }

            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

        }
        public static bool IsInSmiteRange(this Obj_AI_Base target)
        {
            return target.IsValidTarget(Smite.Range + Util.MyHero.BoundingRadius + target.BoundingRadius);
        }
        public static SpellSlot SpellSlotFromName(this AIHeroClient hero, string name)
        {
            foreach (var s in hero.Spellbook.Spells.Where(s => s.Name.ToLower().Contains(name.ToLower())))
            {
                return s.Slot;
            }
            return SpellSlot.Unknown;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.W && args.SData.Name.ToLower().Contains("one"))
                {
                    WLastCastTime = Game.Time;
                }
                else if (args.SData.Name.ToLower().Contains("flash"))
                {
                    FlashLastCastTime = Game.Time;
                }
            }
        }
        public static void CastQ(Obj_AI_Base target)
        {
            if (SpellSlot.Q.IsReady())
            {
                if (SpellSlot.Q.IsFirstSpell())
                {
                    CastQ1(target);
                }
                else
                {
                    CastQ2(target);
                }
            }
        }

        public static void CastQ1(Obj_AI_Base target)
        {
            if (SpellSlot.Q.IsReady() && SpellSlot.Q.IsFirstSpell() && target.IsValidTarget(Q1.Range))
            {
                if (target is AIHeroClient)
                {
                    _Q.CheckSmite(target);
                }
                else
                {
                    var pred = Q1.GetPrediction(target);
                    if (pred.HitChancePercent >= SpellSlot.Q.HitChancePercent())
                    {
                        Q1.Cast(pred.CastPosition);
                    }
                }
            }
        }
        public static void CastQ2(Obj_AI_Base target)
        {
            if (SpellSlot.Q.IsReady() && !SpellSlot.Q.IsFirstSpell() && target.IsValidTarget(Q2.Range))
            {
                if (target.HaveQ())
                {
                    Util.MyHero.Spellbook.CastSpell(SpellSlot.Q, true);
                }
                else if (_Q.EndTime - Game.Time < 0.5f)
                {
                    Champion.ForceQ2(target);
                }
            }
        }
        public static void CastW(Obj_AI_Base target)
        {
            if (SpellSlot.W.IsReady() && target.IsValidAlly())
            {
                if (SpellSlot.W.IsFirstSpell())
                {
                    CastW1(target);
                }
                else
                {
                    CastW2();
                }
            }
        }
        public static void CastW1(Obj_AI_Base target)
        {
            if (SpellSlot.W.IsReady() && SpellSlot.W.IsFirstSpell() && target.IsValidAlly())
            {
                Util.MyHero.Spellbook.CastSpell(W1.Slot, target, true);
            }
        }

        public static void CastW2()
        {
            if (SpellSlot.W.IsReady() && !SpellSlot.W.IsFirstSpell())
            {
                W2.Cast();
            }
        }

        public static void CastE(Obj_AI_Base target)
        {
            if (SpellSlot.E.IsReady())
            {
                if (SpellSlot.E.IsFirstSpell())
                {
                    CastE1(target);
                }
                else
                {
                    CastE2(target);
                }
            }
        }

        public static void CastE1(Obj_AI_Base target)
        {
            if (SpellSlot.E.IsReady() && SpellSlot.E.IsFirstSpell() && target.IsValidTarget(E1.Range))
            {
                var pred = E1.GetPrediction(target);
                if (pred.HitChance >= HitChance.Medium)
                {
                    Util.MyHero.Spellbook.CastSpell(E1.Slot, true);
                }
            }
        }
        public static void CastE2(Obj_AI_Base target)
        {
            if (SpellSlot.E.IsReady() && !SpellSlot.E.IsFirstSpell() && target.IsValidTarget(E2.Range))
            {
                var pred = E2.GetPrediction(target);
                if (pred.HitChance >= HitChance.Medium)
                {
                    Util.MyHero.Spellbook.CastSpell(E2.Slot, true);
                }
            }
        }
        public static void CastR(Obj_AI_Base target)
        {
            if (SpellSlot.R.IsReady() && target.IsValidTarget(R.Range))
            {
                Util.MyHero.Spellbook.CastSpell(R.Slot, target, true);
            }
        }
        public static float HitChancePercent(this SpellSlot s)
        {
            var slot = s.ToString().Trim();
            return Harass.IsActive ? MenuManager.PredictionMenu.GetSliderValue(slot + "Harass") : MenuManager.PredictionMenu.GetSliderValue(slot + "Combo");
        }
        public static bool IsReady(this SpellSlot slot)
        {
            return slot.GetSpellDataInst().IsReady;
        }
        public static bool IsFirstSpell(this SpellSlot slot)
        {
            return slot.GetSpellDataInst().SData.Name.ToLower().Contains("one");
        }
        public static SpellDataInst GetSpellDataInst(this SpellSlot slot)
        {
            return Util.MyHero.Spellbook.GetSpell(slot);
        }
        public static bool CanCastW1
        {
            get
            {
                return SpellSlot.W.IsReady() && SpellSlot.W.IsFirstSpell() && Util.MyHero.Mana >= SpellSlot.W.GetSpellDataInst().SData.Mana;
            }
        }
        public static bool CanCastQ1
        {
            get
            {
                return SpellSlot.Q.IsReady() && SpellSlot.Q.IsFirstSpell() && Util.MyHero.Mana >= SpellSlot.Q.GetSpellDataInst().SData.Mana;
            }
        }
        public static bool SmiteIsReady
        {
            get
            {
                return Smite != null && Smite.IsReady();
            }
        }
        public static bool CanUseSmiteOnHeroes
        {
            get
            {
                if (SmiteIsReady)
                {
                    var name = Smite.Slot.GetSpellDataInst().SData.Name.ToLower();
                    if (name.Contains("smiteduel") || name.Contains("smiteplayerganker"))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public static float SmiteDamage(this Obj_AI_Base target)
        {
            if (target.IsValidTarget() && SmiteIsReady)
            {
                if (target is AIHeroClient)
                {
                    if (CanUseSmiteOnHeroes)
                    {
                        return Util.MyHero.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Smite);
                    }
                }
                else
                {
                    var level = Util.MyHero.Level;
                    return (new[] { 20 * level + 370, 30 * level + 330, 40 * level + 240, 50 * level + 100 }).Max();
                }
            }
            return 0;
        }
        public static float ERange
        {
            get
            {
                return !SpellSlot.E.IsFirstSpell() ? E2.Range : E1.Range;
            }
        }
        public static float QRange
        {
            get
            {
                return !SpellSlot.Q.IsFirstSpell() ? Q2.Range : Q1.Range;
            }
        }
        public static float WRange
        {
            get
            {
                return !SpellSlot.W.IsFirstSpell() ? W2.Range : W1Range;
            }
        }
        public static bool IgniteIsReady
        {
            get
            {
                return Ignite != null && Ignite.IsReady();
            }
        }
        public static bool FlashIsReady
        {
            get
            {
                return Flash != null && Flash.IsReady();
            }
        }
    }
}
