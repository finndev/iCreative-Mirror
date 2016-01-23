using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;

namespace MaddeningJinx
{
    public static class Champion
    {
        public static string Author = "iCreative";
        public static string AddonName = "Maddening Jinx";

        public static int ShouldWaitTime;

        public static bool HasFishBonesActive
        {
            get { return Util.MyHero.Spellbook.GetSpell(SpellSlot.Q).ToggleState == 2; }
        }

        public static int PowPowBuffCount
        {
            get
            {
                foreach (var b in Util.MyHero.Buffs)
                {
                    if (b.Name.Equals("jinxqramp"))
                    {
                        return b.Count;
                    }
                }
                return -1;
            }
        }

        private static float FishBonesExtraRange
        {
            get { return Util.MyHero.Spellbook.GetSpell(SpellSlot.Q).Level * 25f + 50; }
        }

        public static float GetPowPowRange(AttackableUnit target = null)
        {
            return Player.Instance.AttackRange - (HasFishBonesActive ? FishBonesExtraRange : 0) + Util.MyHero.BoundingRadius + (target != null ? (target.BoundingRadius - 20) : 45);
        }

        public static float GetFishBonesRange(AttackableUnit target = null)
        {
            return Player.Instance.AttackRange + (HasFishBonesActive ? 0 : FishBonesExtraRange) + +Util.MyHero.BoundingRadius +
                   (target != null ? (target.BoundingRadius - 20) : 45);
        }

        private static void Main()
        {
            Loading.OnLoadingComplete += delegate
            {
                if (Util.MyHero.Hero != EloBuddy.Champion.Jinx) return;
                Chat.Print(AddonName + " made by " + Author + " loaded, have fun!.");
                Util.Initialize();
                MyTargetSelector.Initialize();
                SpellManager.Initialize();
                MenuManager.Initialize();
                DrawManager.Initialize();
                ModeManager.Initialize();
                ImmobileTracker.Initialize();
                JungleStealer.Initialize();
                Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
                Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
                Dash.OnDash += Dash_OnDash;
            };
        }

        public static BestResult GetBestFishBonesTarget(this IEnumerable<Obj_AI_Base> list)
        {
            var bestList = new List<Obj_AI_Base>();
            Obj_AI_Base bestTarget = null;
            var aiBases = list as Obj_AI_Base[] ?? list.ToArray();
            foreach (var target in aiBases)
            {
                var objAiBases = aiBases.Where(@base => target.Distance(@base, true) <= (SpellManager.QWidth + @base.BoundingRadius).Pow()).ToList();
                if (objAiBases.Count > bestList.Count)
                {
                    bestTarget = target;
                    bestList = objAiBases;
                    if (bestList.Count == aiBases.Length)
                    {
                        break;
                    }
                }
            }
            return new BestResult { List = bestList, Target = bestTarget };
        }

        public static void EnableFishBones(Obj_AI_Base target)
        {
            if (target != null && target.IsInFishBonesRange())
            {
                Orbwalker.ForcedTarget = target;
            }
            if (!HasFishBonesActive)
            {
                SpellManager.CastQ();
            }
        }

        public static void DisableFishBones()
        {
            Orbwalker.ForcedTarget = null;
            if (HasFishBonesActive)
            {
                SpellManager.CastQ();
            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsValidTarget() && sender.IsEnemy)
            {
                if (Util.MyHero.Distance(e.Start, true) < Util.MyHero.Distance(e.SenderMousePos, true))
                {
                    if (MenuManager.Menu.GetCheckBoxValue("W.Spells"))
                    {
                        SpellManager.CastW(sender);
                    }
                }
                else
                {
                    if (Util.MyHero.Distance(e.SenderMousePos, true) < (sender.GetAutoAttackRange(Util.MyHero) * 1.5f).Pow())
                    {
                        ShouldWaitTime = Core.GameTickCount;
                    }
                    if (MenuManager.Menu.GetCheckBoxValue("E.Antigapcloser") && MenuManager.AntiGapcloserMenu != null && MenuManager.AntiGapcloserHashSet.Contains(sender.ChampionName + e.Slot) && MenuManager.AntiGapcloserMenu.GetCheckBoxValue(sender.ChampionName + e.Slot))
                    {
                        SpellManager.E.Cast(e.End);
                    }
                }
            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender.IsValidTarget() && sender.IsEnemy)
            {
                if (MenuManager.Menu.GetCheckBoxValue("E.Spells") && ImmobileTracker.GetETime() <= e.EndTime - Game.Time)
                //TODO
                {
                    SpellManager.E.Cast(sender.Position);
                }
            }
        }

        private static void Dash_OnDash(Obj_AI_Base sender, Dash.DashEventArgs e)
        {
            if (sender.IsValidTarget() && sender.IsEnemy)
            {
                if (Util.MyHero.Distance(e.StartPos, true) < Util.MyHero.Distance(e.EndPos, true))
                {
                    if (MenuManager.Menu.GetCheckBoxValue("W.Spells"))
                    {
                        SpellManager.CastW(sender);
                    }
                }
                else
                {
                    if (MenuManager.Menu.GetCheckBoxValue("E.Antigapcloser"))
                    {
                        //SpellManager.CastE(sender);
                    }
                    if (Util.MyHero.Distance(e.EndPos, true) < (sender.GetAutoAttackRange(Util.MyHero) * 1.5f).Pow())
                    {
                        ShouldWaitTime = Core.GameTickCount;
                    }
                }
            }
        }

        public class BestResult
        {
            public List<Obj_AI_Base> List;
            public Obj_AI_Base Target;
        }
    }
}