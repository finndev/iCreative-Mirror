using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using SharpDX;

namespace LeeSin
{
    public static class Champion
    {
        //Falta agregar laneclear, lasthit.
        public static string Author = "iCreative";
        public static string AddonName = "Master the enemy";
        public static string PassiveName = "blindmonkpassive_cosmetic";
        public static int PassiveStack
        {
            get
            {
                return Util.MyHero.HasBuff2(PassiveName) ? Util.MyHero.GetBuff2(PassiveName).Count : 0;
            }
        }
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Util.MyHero.Hero != EloBuddy.Champion.LeeSin) { return; }
            SpellManager.Init();
            MenuManager.Init();
            ModeManager.Init();
            WardManager.Init();
            _Q.Init();
            _R.Init();
            Insec.Init();
            AutoSmite.Init();
            DrawManager.Init();
            TargetSelector.Init(SpellManager.Q2.Range + 200, DamageType.Physical);
            Chat.Print(AddonName + " made by " + Author + " loaded, have fun!.");
            LoadCallbacks();
        }
        private static void LoadCallbacks()
        {
            Game.OnTick += Game_OnTick;

            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;

        }
        public static void GapCloseWithWard(Obj_AI_Base target)
        {
            if (SpellManager.CanCastW1)
            {
                var obj = GetBestObjectNearTo(target.Position);
                if (obj != null && Util.MyHero.Distance(target, true) > obj.Distance(target, true))
                {
                    SpellManager.CastW1(obj);
                }
                else if (WardManager.CanCastWard)
                {
                    var pred = SpellManager.W1.GetPrediction(target);
                    if (pred.HitChance >= HitChance.AveragePoint)
                    {
                        WardManager.CastWardTo(pred.CastPosition);
                    }
                }
            }
        }
        public static void GapCloseWithoutWard(Obj_AI_Base target)
        {
            if (SpellManager.CanCastW1)
            {
                var obj = GetBestObjectNearTo(target.Position);
                if (obj != null && Util.MyHero.Distance(target, true) > obj.Distance(target, true))
                {
                    SpellManager.CastW1(obj);
                }
            }
        }
        public static Obj_AI_Base GetBestObjectFarFrom(Vector3 position)
        {
            var minion = AllyMinionManager.GetFurthestTo(position);
            var ally = AllyHeroManager.GetFurthestTo(position);
            var ward = WardManager.GetFurthestTo(position);
            var miniondistance = minion != null ? position.Distance(minion, true) : 0;
            var allydistance = ally != null ? position.Distance(ally, true) : 0;
            var warddistance = ward != null ? position.Distance(ward, true) : 0;
            var best = Math.Max(miniondistance, Math.Max(allydistance, warddistance));
            if (best > 0f)
            {
                if (Math.Abs(best - allydistance) < float.Epsilon)
                {
                    return ally;
                }
                if (Math.Abs(best - miniondistance) < float.Epsilon)
                {
                    return minion;
                }
                if (Math.Abs(best - warddistance) < float.Epsilon)
                {
                    return ward;
                }
            }
            return null;
        }
        public static Obj_AI_Base GetBestObjectNearTo(Vector3 position)
        {
            var minion = AllyMinionManager.GetNearestTo(position);
            var ally = AllyHeroManager.GetNearestTo(position);
            var ward = WardManager.GetNearestTo(position);
            var miniondistance = minion != null ? position.Distance(minion, true) : 999999999f;
            var allydistance = ally != null ? position.Distance(ally, true) : 999999999f;
            var warddistance = ward != null ? position.Distance(ward, true) : 999999999f;
            var best = Math.Min(miniondistance, Math.Min(allydistance, warddistance));
            if (best <= Math.Pow(250f, 2))
            {
                if (Math.Abs(best - allydistance) < float.Epsilon)
                {
                    return ally;
                }
                if (Math.Abs(best - miniondistance) < float.Epsilon)
                {
                    return minion;
                }
                if (Math.Abs(best - warddistance) < float.Epsilon)
                {
                    return ward;
                }
            }
            return null;
        }

        public static void ForceQ2(Obj_AI_Base target = null)
        {
            if (SpellSlot.Q.IsReady() && !SpellSlot.Q.IsFirstSpell())
            {
                if (target == null)
                {
                    target = TargetSelector.Target;
                }
                if (_Q.IsValidTarget && target.IsValidTarget())
                {
                    if (target.Distance(_Q.Target, true) < Util.MyHero.Distance(_Q.Target, true))
                    {
                        SpellManager.CastQ2(_Q.Target);
                    }
                }
            }
            /*
            if (SpellSlot.Q.IsReady() && !SpellSlot.Q.IsFirstSpell())
            {
                Core.DelayAction(ForceQ2, 100);
            }*/
        }

        private static void Game_OnTick(EventArgs args)
        {
            TargetSelector.Range = 1000f;
            if (SpellSlot.Q.IsReady() && SpellSlot.Q.IsFirstSpell())
            {
                TargetSelector.Range = 1300f;
            }
            if (SpellSlot.Q.IsReady() && !SpellSlot.Q.IsFirstSpell() && _Q.Target != null)
            {
                TargetSelector.Range = 1500f;
            }
            if (!Insec.IsActive)
            {
                var t = _R.BestHitR(MenuManager.MiscMenu.GetSliderValue("R.Hit"));
                if (MenuManager.MiscMenu.GetSliderValue("R.Hit") <= t.Item1)
                {
                    SpellManager.CastR(t.Item2);
                }
            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender.IsValidTarget(TargetSelector.Range) && sender.IsEnemy && sender is AIHeroClient)
            {
                if (MenuManager.MiscMenu.GetCheckBoxValue("Interrupter"))
                {
                    if (SpellSlot.R.IsReady())
                    {
                        SpellManager.CastQ(sender);
                        SpellManager.CastR(sender);
                        GapCloseWithWard(sender);
                    }
                }
            }
        }


    }
}
