using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;


namespace Thresh
{
    public static class Harass
    {
        public static Menu Menu
        {
            get { return MenuManager.GetSubMenu("Harass"); }
        }

        public static bool IsActive
        {
            get { return ModeManager.IsHarass; }
        }

        public static void Execute()
        {
            var target = TargetSelector.Target;
            if (target == null) return;
            if (Menu.GetSliderValue("E1") > 0 && SpellSlot.E.IsReady())
            {
                switch (Menu.GetSliderValue("E1"))
                {
                    case 1:
                        PerformPull();
                        break;
                    case 2:
                        PerformPush();
                        break;
                    case 3:
                        if (EntityManager.Heroes.Allies.HealthPercent(TargetSelector.Range) >= EntityManager.Heroes.Enemies.HealthPercent(TargetSelector.Range))
                        {
                            PerformPull();
                        }
                        else
                        {
                            PerformPush();
                        }
                        break;
                }
            }
            if (SpellSlot.W.IsReady())
            {
                if (Menu.GetSliderValue("W1") > 0)
                {
                    switch (Menu.GetSliderValue("W1"))
                    {
                        case 1:
                            if (SpellManager.QTarget.IsValidTarget() && SpellManager.QTarget.NetworkId == target.NetworkId)
                            {
                                SpellManager.CastW(Champion.Best_Offensive_W_Ally(target));
                            }
                            break;
                        case 2:
                            SpellManager.CastW(Champion.Best_Offensive_W_Ally(target));
                            break;
                    }
                }
                if (Menu.GetSliderValue("W2") > 0)
                {
                    var ally =
                        EntityManager.Heroes.Allies.Where(h => h.IsValidTarget(TargetSelector.Range) && !h.IsMe && h.HealthPercent <= Menu.GetSliderValue("W2"))
                            .OrderByDescending(h => h.GetPriority() / h.HealthPercent)
                            .FirstOrDefault();
                    if (ally != null)
                    {
                        SpellManager.CastW(ally);
                    }
                }
            }
            if (SpellSlot.Q.IsReady())
            {

                if (Menu.GetSliderValue("Q1") > 0)
                {
                    switch (Menu.GetSliderValue("Q1"))
                    {
                        case 1:
                            SpellManager.CastQ1(target);
                            break;
                        case 2:
                            foreach (var h in EntityManager.Heroes.Enemies.Where(h => h.IsValidTarget()))
                            {
                                SpellManager.CastQ1(h);
                            }
                            break;
                    }
                }
                if (Menu.GetSliderValue("Q2") > 0 && SpellManager.QTarget.IsValidTarget())
                {
                    switch (Menu.GetSliderValue("Q2"))
                    {
                        case 1:
                            if (SpellManager.QTarget.NetworkId == target.NetworkId)
                            {
                                SpellManager.CastQ2(target);
                            }
                            break;
                        case 2:
                            if (SpellManager.QTarget.IsInRange(target, 350))
                            {
                                SpellManager.CastQ2(target);
                            }
                            break;
                    }
                }
            }
        }

        public static void PerformPull()
        {
            switch (Menu.GetSliderValue("E2"))
            {
                case 1:
                    SpellManager.Pull(TargetSelector.Target);
                    break;
                case 2:
                    foreach (var h in EntityManager.Heroes.Enemies.Where(h => h.IsValidTarget()))
                    {
                        SpellManager.Pull(h);
                    }
                    break;
            }
        }
        public static void PerformPush()
        {
            switch (Menu.GetSliderValue("E2"))
            {
                case 1:
                    SpellManager.Push(TargetSelector.Target);
                    break;
                case 2:
                    foreach (var h in EntityManager.Heroes.Enemies.Where(h => h.IsValidTarget()))
                    {
                        SpellManager.Push(h);
                    }
                    break;
            }
        }
    }
}
