using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace MaddeningJinx
{
    public class OnDamageEvent
    {
        public int Time;
        public float Damage;

        internal OnDamageEvent(int time, float damage)
        {
            Time = time;
            Damage = damage;
        }
    }

    public static class JungleStealer
    {

        public static readonly Dictionary<int, List<OnDamageEvent>> DamagesOnTime = new Dictionary<int, List<OnDamageEvent>>();
        public static void Initialize()
        {
            Game.OnTick += Game_OnTick;
            AttackableUnit.OnDamage += AttackableUnit_OnDamage;
            GameObject.OnDelete += GameObject_OnDelete;
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var minion = sender as Obj_AI_Minion;
            if (minion != null && minion.IsMonster && minion.IsEnemy && minion.MaxHealth >= 3500)
            {
                if (DamagesOnTime.ContainsKey(minion.NetworkId))
                {
                    DamagesOnTime.Remove(minion.NetworkId);
                }
            }
        }

        private static void AttackableUnit_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (args.Source.IsEnemy)
            {
                var minion = args.Target as Obj_AI_Minion;
                if (minion != null && minion.IsMonster && minion.MaxHealth >= 3500)
                {
                    if (!DamagesOnTime.ContainsKey(minion.NetworkId)) DamagesOnTime[minion.NetworkId] = new List<OnDamageEvent>();
                    DamagesOnTime[minion.NetworkId].Add(new OnDamageEvent(Core.GameTickCount, args.Damage));
                }
                var sourceMinion = args.Source as Obj_AI_Minion;
                if (sourceMinion != null && sourceMinion.IsMonster && sourceMinion.MaxHealth >= 3500)
                {
                    if (!DamagesOnTime.ContainsKey(sourceMinion.NetworkId)) DamagesOnTime[sourceMinion.NetworkId] = new List<OnDamageEvent>();
                    DamagesOnTime[sourceMinion.NetworkId].Add(new OnDamageEvent(Core.GameTickCount, 0));
                }
            }

        }

        private static float GetUltimateDamage(this Obj_AI_Base monster, float health)
        {
            var percentMod = Math.Min((int)(Util.MyHero.Distance(monster) / 100f) * 6f + 10f, 100f) / 100f;
            var level = Util.MyHero.Spellbook.GetSpell(SpellSlot.R).Level;
            var rawDamage = 0.8f * percentMod * (200f + 50f * level + Util.MyHero.TotalAttackDamage + Math.Min((0.25f + 0.05f * level) * (monster.MaxHealth - health), 300f));
            return Util.MyHero.CalculateDamageOnUnit(monster, DamageType.Physical, rawDamage);
        }

        private static float GetPredictedDamage(this Obj_AI_Base monster, int time)
        {
            if (!DamagesOnTime.ContainsKey(monster.NetworkId))
            {
                return 0f;
            }
            return DamagesOnTime[monster.NetworkId].Where(onDamage => onDamage.Time > Core.GameTickCount - time && onDamage.Time <= Core.GameTickCount).Sum(onDamage => onDamage.Damage);
        }

        private static float GetPredictedHealth(this Obj_AI_Base monster, int time)
        {
            return monster.TotalShieldHealth() + monster.HPRegenRate * 2 - monster.GetPredictedDamage(time);
        }

        private static bool IsGettingAttacked(this Obj_AI_Minion monster)
        {
            return DamagesOnTime.ContainsKey(monster.NetworkId) && DamagesOnTime[monster.NetworkId].LastOrDefault(o => Core.GameTickCount - o.Time < 12000) != null;
        }

        public static IEnumerable<Obj_AI_Base> BigMonsters
        {
            get
            {
                return EntityManager.MinionsAndMonsters.Monsters.Where(m => m.MaxHealth >= 3500 && m.IsGettingAttacked());
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            DrawManager.JungleStealText.TextValue = "";
            if (SpellSlot.R.IsReady() && Util.MyHero.Mana >= SpellSlot.R.Mana())
            {
                foreach (var monster in BigMonsters)
                {
                    var time = (int)(1000 * monster.GetUltimateTravelTime());
                    var health = monster.GetPredictedHealth(time);
                    var damage = monster.GetUltimateDamage(health);
                    var heroNear = MyTargetSelector.ValidEnemies.FirstOrDefault(h => h.IsInRange(monster, 225f + monster.BoundingRadius));
                    DrawManager.JungleStealText.TextValue = monster.Name + " is getting attacked. Damage left: " + (int)(100 * Math.Max(health - damage, 0) / monster.MaxHealth) + "%.";
                    if (health <= damage && heroNear != null)
                    {
                        KillSteal.RKillableBases.Add(monster);
                        DrawManager.JungleStealText.TextValue = "";
                        if (MenuManager.GetSubMenu("Automatic").CheckBox("R.JungleSteal") || MenuManager.TapKeyPressed)
                        {
                            KillSteal.RHittableBases.Add(heroNear.ServerPosition);
                        }
                    }
                }
            }
        }
    }
}
