using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace MaddeningJinx
{
    public static class ModeManager
    {
        public static void Initialize()
        {
            Game.OnTick += Game_OnUpdate;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
        }
        internal static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (!IsNone)
            {
                if (Orbwalker.ForcedTarget != null)
                {
                    args.Process = Orbwalker.ForcedTarget.IdEquals(args.Target) && Champion.HasFishBonesActive;
                }
            }
        }

        public static bool CanUseQ = true;
        public static bool CanUseW = true;
        public static bool CanUseE = true;
        private static void Game_OnUpdate(EventArgs args)
        {
            if (Util.MyHero.IsDead) { return; }
            KillSteal.Execute();
            if (Orbwalker.ForcedTarget != null && !Orbwalker.ForcedTarget.IsInFishBonesRange())
            {
                Orbwalker.ForcedTarget = null;
            }
            CanUseQ = Util.MyHero.Mana >= SpellSlot.W.Mana() + (SpellSlot.E.IsLearned() ? SpellSlot.E.Mana() : 0f) + (SpellSlot.R.IsLearned() ? SpellSlot.R.Mana() : 0f);
            CanUseW = Util.MyHero.Mana >= (SpellSlot.W.Mana() + (SpellSlot.R.IsLearned() ? SpellSlot.R.Mana() : 0f) + 20);
            CanUseE = Util.MyHero.Mana >= SpellSlot.E.Mana() + (SpellSlot.R.IsLearned() ? SpellSlot.R.Mana() : 0f);
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
                return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && EntityManager.MinionsAndMonsters.Monsters.Any(m => Util.MyHero.IsInAutoAttackRange(m));
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
