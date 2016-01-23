using System;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;

namespace Thresh
{
    public static class Champion
    {
        public static string Author = "iCreative";
        public static string AddonName = "iMadLife";
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Util.MyHero.Hero != EloBuddy.Champion.Thresh) { return; }
            Chat.Print(AddonName + " made by " + Author + " loaded, have fun!.");
            TargetSelector.Init(1150f, DamageType.Physical);
            SpellManager.Init(args);
            MenuManager.Init(args);
            DrawManager.Init(args);
            ModeManager.Init(args);
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
        }

        public static AIHeroClient Best_Offensive_W_Ally(AIHeroClient target)
        {
            return target != null ? EntityManager.Heroes.Allies.Where(h=> h.IsValidTarget(TargetSelector.Range) && !h.IsMe && !h.IsInAutoAttackRange(target) && target.Distance(Util.MyHero, true) < target.Distance(h, true)).OrderByDescending(h=> h.GetPriority() / h.HealthPercent).FirstOrDefault() : null;
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsValidTarget()) return;
            if (MenuManager.MiscMenu.GetCheckBoxValue("Gapcloser.E"))
            {
                if (TargetSelector.Ally.Distance(e.End, true) < TargetSelector.Ally.Distance(e.Start, true))
                {
                    SpellManager.Push(sender);
                }
                else
                {
                    SpellManager.Pull(sender);
                }
            }
            if (MenuManager.MiscMenu.GetCheckBoxValue("Gapcloser.Q"))
            {
                if (TargetSelector.Ally.Distance(e.End, true) > TargetSelector.Ally.Distance(e.Start, true))
                {
                    SpellManager.CastQ(sender);
                }
            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsValidTarget()) return;
            if (MenuManager.MiscMenu.GetCheckBoxValue("Interrupter"))
            {
                SpellManager.Pull(sender);
                SpellManager.CastQ(sender);
            }
        }
    }
}
