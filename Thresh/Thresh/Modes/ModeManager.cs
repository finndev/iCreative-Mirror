using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace Thresh
{
    public static class ModeManager
    {
        public static void Init(EventArgs args)
        {
            Game.OnTick += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Util.MyHero.IsDead)
            {
                return;
            }
            if (MenuManager.MiscMenu.GetSliderValue("W") > 0 && SpellSlot.W.IsReady())
            {
                var ally =
                    EntityManager.Heroes.Allies.Where(
                        h =>
                            h.IsValidTarget(1300) && !h.IsMe && 
                            h.CountEnemiesInside(450) >= MenuManager.MiscMenu.GetSliderValue("W"))
                        .OrderByDescending(h => h.CountEnemiesInside(450) * h.GetPriority() / h.HealthPercent)
                        .FirstOrDefault();
                if (ally != null && ally.CountEnemiesInside(450) > Util.MyHero.CountEnemiesInside(450))
                {
                    SpellManager.CastW(ally);
                }
            }
            if ((MenuManager.MiscMenu.GetCheckBoxValue("Turret.Q") && SpellSlot.Q.IsReady()) ||
                (MenuManager.MiscMenu.GetCheckBoxValue("Turret.E") && SpellSlot.E.IsReady()))
            {
                var t = EntityManager.Turrets.Allies.FirstOrDefault(t2 => !t2.IsDead && Util.MyHero.IsInRange(t2, 800));
                if (t != null)
                {
                    foreach (var h in EntityManager.Heroes.Enemies.Where(h => h.IsValidTarget() && h.IsInRange(t, 1100)))
                    {
                        if (MenuManager.MiscMenu.GetCheckBoxValue("Turret.E") && SpellSlot.E.IsReady())
                        {
                            SpellManager.Pull(h);
                        }
                        if (MenuManager.MiscMenu.GetCheckBoxValue("Turret.Q") && SpellSlot.Q.IsReady())
                        {
                            SpellManager.CastQ1(h);
                        }
                    }
                }
            }
            KillSteal.Execute();
            if (IsCombo)
            {
                Combo.Execute();
            }
            else if (IsHarass)
            {
                Harass.Execute();
            }
            else if (IsClear)
            {
                Clear.Execute();
            }
            else if (IsLastHit)
            {
                LastHit.Execute();
            }
            if (IsFlee)
            {
                Flee.Execute();
            }
        }

        public static bool IsCombo
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo); }
        }

        public static bool IsHarass
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass); }
        }

        public static bool IsClear
        {
            get { return IsLaneClear || IsJungleClear; }
        }

        public static bool IsLaneClear
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear); }
        }

        public static bool IsJungleClear
        {
            get
            {
                return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) &&
                       EntityManager.MinionsAndMonsters.GetJungleMonsters(Util.MyHero.Position,
                           Util.MyHero.GetAutoAttackRange() + 200, false).Any();
            }
        }

        public static bool IsLastHit
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit); }
        }

        public static bool IsFlee
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee); }
        }

        public static bool IsNone
        {
            get { return !IsLastHit && !IsClear && !IsHarass && !IsCombo; }
        }
    }
}
