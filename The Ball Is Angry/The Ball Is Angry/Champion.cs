using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;

namespace The_Ball_Is_Angry
{
    public static class Champion
    {
        public static string Author = "iCreative";
        public static string AddonName = "The Ball Is Angry";
        
        private static void Main()
        {
            Loading.OnLoadingComplete += delegate
            {
                if (Util.MyHero.Hero != EloBuddy.Champion.Orianna) return;
                Chat.Print(AddonName + " made by " + Author + " loaded, have fun!.");
                Util.Initialize();
                MyTargetSelector.Initialize();
                SpellManager.Initialize();
                MenuManager.Initialize();
                DrawManager.Initialize();
                ModeManager.Initialize();
                Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
                Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
                Dash.OnDash += Dash_OnDash;
            };
        }
        

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsValidTarget() && sender.IsEnemy)
            {

            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender.IsValidTarget() && sender.IsEnemy)
            {
            }
        }

        private static void Dash_OnDash(Obj_AI_Base sender, Dash.DashEventArgs e)
        {
            if (sender.IsValidTarget() && sender.IsEnemy)
            {
            }
        }

        public class BestResult
        {
            public List<Obj_AI_Base> List;
            public Obj_AI_Base Target;
        }
    }
}