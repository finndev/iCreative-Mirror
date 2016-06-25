using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;


namespace Draven_Me_Crazy
{
    public static class SpellManager
    {
        public static Spell.Skillshot E, R = null;
        public static Spell.Active Q, W = null;
        public static Spell.Targeted Ignite, Smite = null;
        public static Spell.Skillshot Flash = null;
        public static void Init(EventArgs args)
        {
            Q = new Spell.Active(SpellSlot.Q, 1075);
            W = new Spell.Active(SpellSlot.W, 950);
            E = new Spell.Skillshot(SpellSlot.E, 1050, SkillShotType.Linear, 250, 1600, 130);
            E.AllowedCollisionCount = int.MaxValue;
            R = new Spell.Skillshot(SpellSlot.R, 20000, SkillShotType.Linear, 500, 2000, 155);
            R.AllowedCollisionCount = int.MaxValue;
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
            Game.OnTick += Game_OnTick;

        }

        private static void Game_OnTick(EventArgs args)
        {
            R = new Spell.Skillshot(R.Slot, (uint)MenuManager.MiscMenu.GetSliderValue("RRange"), R.Type, R.CastDelay, R.Speed, R.Width);
            R.AllowedCollisionCount = int.MaxValue;
            E = new Spell.Skillshot(E.Slot, (uint)MenuManager.MiscMenu.GetSliderValue("ERange"), E.Type, E.CastDelay, E.Speed, E.Width);
            E.AllowedCollisionCount = int.MaxValue;
        }

        public static SpellSlot SpellSlotFromName(this AIHeroClient hero, string name)
        {
            foreach (SpellDataInst s in hero.Spellbook.Spells)
            {
                if (s.Name.ToLower().Contains(name.ToLower()))
                {
                    return s.Slot;
                }
            }
            return SpellSlot.Unknown;
        }
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
            }
        }
        public static void CastQ(Obj_AI_Base target, int Count = 1)
        {
            if (SpellSlot.Q.IsReady() && target.IsValidTarget(Q.Range) && Orbwalker.CanAutoAttack)
            {
                if (AxesManager.AxesCount < Count)
                {
                    Util.MyHero.Spellbook.CastSpell(SpellSlot.Q);
                }
            }
        }

        public static void CastW(Obj_AI_Base target)
        {
            if (SpellSlot.W.IsReady() && target.IsValidTarget(W.Range) && !Util.MyHero.HasBuff("dravenfurybuff"))
            {
                var pred = E.GetPrediction(target);
                var damageI = target.GetBestCombo();
                if (pred.HitChancePercent >= 20f && ((damageI.IsKillable && Util.MyHero.Distance(target, true) < Util.MyHero.Distance(pred.CastPosition, true)) || Util.MyHero.Distance(target) >= 500f))
                {
                    Util.MyHero.Spellbook.CastSpell(SpellSlot.W);
                }
            }
        }

        public static void CastE(Obj_AI_Base target)
        {
            if (SpellSlot.E.IsReady() && target.IsValidTarget() && target.IsEnemy)
            {
                var pred = E.GetPrediction(target);
                if (pred.HitChance >= HitChance.High)
                {
                    E.Cast(pred.CastPosition);
                }
            }
        }
        public static void CastR(Obj_AI_Base target)
        {
            if (SpellSlot.R.IsReady() && target.IsValidTarget())
            {
                var pred = R.GetPrediction(target);
                if (pred.HitChancePercent >= 60f)
                {
                    R.Cast(pred.CastPosition);
                }
            }
        }
        public static float HitChancePercent(this SpellSlot s)
        {
            string slot = s.ToString().Trim();
            if (Harass.IsActive)
            {
                return MenuManager.PredictionMenu.GetSliderValue(slot + "Harass");
            }
            return MenuManager.PredictionMenu.GetSliderValue(slot + "Combo");
        }
        public static bool IsReady(this SpellSlot slot)
        {
            return slot.GetSpellDataInst().IsReady;
        }
        public static SpellDataInst GetSpellDataInst(this SpellSlot slot)
        {
            return Util.MyHero.Spellbook.GetSpell(slot);
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
        public static bool IsInSmiteRange(this Obj_AI_Base target)
        {
            return target.IsValidTarget(Smite.Range + Util.MyHero.BoundingRadius + target.BoundingRadius);
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
