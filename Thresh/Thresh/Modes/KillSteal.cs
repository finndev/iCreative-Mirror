using System.Linq;
using System.Runtime.Remoting.Messaging;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Thresh
{
    public static class KillSteal
    {
        public static Menu Menu
        {
            get
            {
                return MenuManager.GetSubMenu("KillSteal");
            }
        }
        public static void Execute()
        {
            foreach (var h in EntityManager.Heroes.Enemies.Where(h => h.IsValidTarget(TargetSelector.Range)))
            {
                if (Menu.GetCheckBoxValue("Ignite") && SpellManager.IgniteIsReady &&
                    Util.MyHero.GetSummonerSpellDamage(h, DamageLibrary.SummonerSpells.Ignite) >= h.Health)
                {
                    SpellManager.Ignite.Cast(h);
                }
                if (Menu.GetCheckBoxValue("Q") && SpellSlot.Q.IsReady() &&
                    Util.MyHero.GetSpellDamage(h, SpellSlot.Q) >= h.Health)
                {
                    SpellManager.CastQ1(h);
                }
                if (Menu.GetCheckBoxValue("E") && SpellSlot.E.IsReady() &&
                    Util.MyHero.GetSpellDamage(h, SpellSlot.E) >= h.Health)
                {
                    SpellManager.Pull(h);
                }
            }
        }
    }
}
