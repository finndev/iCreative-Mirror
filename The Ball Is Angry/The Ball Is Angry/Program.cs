using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace The_Ball_Is_Angry
{
    internal static class Program
    {
        private static readonly string Author = "iCreative";
        private static readonly string AddonName = "The ball is angry";
        private static readonly float RefreshTime = 0.4f;
        private static readonly Dictionary<int, DamageInfo> PredictedDamage = new Dictionary<int, DamageInfo>();
        private static Menu Menu;
        private static readonly Dictionary<string, Menu> SubMenu = new Dictionary<string, Menu>();
        private static Spell.Skillshot Q, W, E, R;
        private static Spell.Targeted Ignite;
        private static readonly List<MissileClient> missiles = new List<MissileClient>();
        private static GameObject E_Target;
        private static float Q_LastRequest;
        private static float E_LastRequest;
        private static GameObject BallObject;
        private static float LastGapclose;

        private static AIHeroClient myHero
        {
            get { return Player.Instance; }
        }

        private static Vector3 mousePos
        {
            get { return Game.CursorPos; }
        }

        private static Vector3 Ball
        {
            get
            {
                if (BallObject != null && BallObject.IsValid)
                {
                    return BallObject.Position;
                }
                return myHero.Position;
            }
        }

        private static float Overkill
        {
            get { return (100f + GetSlider(SubMenu["Misc"], "Overkill")) / 100f; }
        }

        private static bool IsCombo
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo); }
        }

        private static bool IsHarass
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass); }
        }

        private static bool IsClear
        {
            get { return IsLaneClear || IsJungleClear; }
        }

        private static bool IsLaneClear
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear); }
        }

        private static bool IsJungleClear
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear); }
        }

        private static bool IsLastHit
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit); }
        }

        private static bool IsFlee
        {
            get { return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee); }
        }

        private static bool IsNone
        {
            get { return !IsFlee && !IsLastHit && !IsClear && !IsHarass && !IsCombo; }
        }
        
        private static void OnLoad(EventArgs args)
        {
            Chat.Print(AddonName + " made by " + Author + " loaded, have fun!.");
            Q = new Spell.Skillshot(SpellSlot.Q, 815, SkillShotType.Circular, 0, 1200, 130)
            {
                AllowedCollisionCount = int.MaxValue
            };
            W = new Spell.Skillshot(SpellSlot.W, 255, SkillShotType.Linear, 250, int.MaxValue, 50)
            {
                AllowedCollisionCount = int.MaxValue
            };
            E = new Spell.Skillshot(SpellSlot.E, 1095, SkillShotType.Circular, 0, 1800, 85)
            {
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Skillshot(SpellSlot.R, 400, SkillShotType.Linear, 500, int.MaxValue, 50)
            {
                AllowedCollisionCount = int.MaxValue
            };
            var slot = myHero.GetSpellSlotFromName("summonerdot");
            if (slot != SpellSlot.Unknown)
            {
                Ignite = new Spell.Targeted(slot, 600);
            }
            Menu = MainMenu.AddMenu(AddonName, AddonName + " by " + Author + " v1.40");
            Menu.AddLabel(AddonName + " made by " + Author);

            SubMenu["Prediction"] = Menu.AddSubMenu("Prediction", "Prediction 2.1");
            SubMenu["Prediction"].AddGroupLabel("Q Settings");
            SubMenu["Prediction"].Add("QCombo", new Slider("Combo HitChancePercent", 65, 0, 100));
            SubMenu["Prediction"].Add("QHarass", new Slider("Harass HitChancePercent", 75, 0, 100));
            SubMenu["Prediction"].AddGroupLabel("W Settings");
            SubMenu["Prediction"].Add("WCombo", new Slider("Combo HitChancePercent", 70, 0, 100));
            SubMenu["Prediction"].Add("WHarass", new Slider("Harass HitChancePercent", 75, 0, 100));
            SubMenu["Prediction"].AddGroupLabel("E Settings");
            SubMenu["Prediction"].Add("ECombo", new Slider("Combo HitChancePercent", 45, 0, 100));
            SubMenu["Prediction"].Add("EHarass", new Slider("Harass HitChancePercent", 60, 0, 100));
            SubMenu["Prediction"].AddGroupLabel("R Settings");
            SubMenu["Prediction"].Add("RCombo", new Slider("Combo HitChancePercent", 65, 0, 100));
            SubMenu["Prediction"].Add("RHarass", new Slider("Harass HitChancePercent", 75, 0, 100));

            SubMenu["Combo"] = Menu.AddSubMenu("Combo", "Combo");
            SubMenu["Combo"].Add("TF", new Slider("Use TeamFight Logic if enemies near >=", 3, 1, 5));
            SubMenu["Combo"].AddGroupLabel("Common Logic");
            SubMenu["Combo"].Add("Q", new CheckBox("Use Q On Target", true));
            SubMenu["Combo"].Add("W", new CheckBox("Use W On Target", true));
            SubMenu["Combo"].Add("Shield", new CheckBox("Use Shield On Enemy Missiles", true));
            SubMenu["Combo"].Add("E", new Slider("Use E If Hit", 1, 1, 5));
            SubMenu["Combo"].Add("E2", new Slider("Use E If HealthPercent <=", 50, 0, 100));
            SubMenu["Combo"].AddGroupLabel("1 vs 1 Logic");
            SubMenu["Combo"].Add("R", new CheckBox("Use R On Target If Killable", true));
            SubMenu["Combo"].AddGroupLabel("TeamFight Logic");
            SubMenu["Combo"].Add("Q2", new Slider("Use Q If Hit", 2, 1, 5));
            SubMenu["Combo"].Add("W2", new Slider("Use W If Hit", 2, 1, 5));
            SubMenu["Combo"].Add("R2", new Slider("Use R if Hit", 3, 1, 5));

            SubMenu["Harass"] = Menu.AddSubMenu("Harass", "Harass");
            SubMenu["Harass"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["Harass"].Add("W", new CheckBox("Use W", true));
            SubMenu["Harass"].Add("Shield", new CheckBox("Use Shield On Enemy Missiles", true));
            SubMenu["Harass"].Add("E", new Slider("Use E If Hit", 1, 1, 5));
            SubMenu["Harass"].Add("E2", new Slider("Use E If HealthPercent <=", 40, 0, 100));
            SubMenu["Harass"].Add("Mana", new Slider("Min. Mana Percent:", 20, 0, 100));

            SubMenu["LaneClear"] = Menu.AddSubMenu("LaneClear", "LaneClear");
            SubMenu["LaneClear"].AddGroupLabel("LaneClear Minions");
            SubMenu["LaneClear"].Add("Q", new Slider("Use Q If Hit", 4, 0, 10));
            SubMenu["LaneClear"].Add("W", new Slider("Use W If Hit", 3, 0, 10));
            SubMenu["LaneClear"].Add("E", new Slider("Use E If Hit", 6, 0, 10));
            SubMenu["LaneClear"].AddGroupLabel("Unkillable minions");
            SubMenu["LaneClear"].Add("Q2", new CheckBox("Use Q", true));
            SubMenu["LaneClear"].Add("Mana", new Slider("Min. Mana Percent:", 50, 0, 100));

            SubMenu["LastHit"] = Menu.AddSubMenu("LastHit", "LastHit");
            SubMenu["LastHit"].AddGroupLabel("Unkillable minions");
            SubMenu["LastHit"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["LastHit"].Add("Mana", new Slider("Min. Mana Percent:", 50, 0, 100));

            SubMenu["JungleClear"] = Menu.AddSubMenu("JungleClear", "JungleClear");
            SubMenu["JungleClear"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["JungleClear"].Add("W", new CheckBox("Use W", true));
            SubMenu["JungleClear"].Add("E", new CheckBox("Use E", true));
            SubMenu["JungleClear"].Add("Mana", new Slider("Min. Mana Percent:", 20, 0, 100));

            SubMenu["KillSteal"] = Menu.AddSubMenu("KillSteal", "KillSteal");
            SubMenu["KillSteal"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["KillSteal"].Add("W", new CheckBox("Use W", true));
            SubMenu["KillSteal"].Add("E", new CheckBox("Use E", true));
            SubMenu["KillSteal"].Add("R", new CheckBox("Use R", false));
            SubMenu["KillSteal"].Add("Ignite", new CheckBox("Use Ignite", true));

            SubMenu["Flee"] = Menu.AddSubMenu("Flee", "Flee");
            SubMenu["Flee"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["Flee"].Add("W", new CheckBox("Use W", true));
            SubMenu["Flee"].Add("E", new CheckBox("Use E", true));

            SubMenu["Draw"] = Menu.AddSubMenu("Drawing", "Drawing");
            SubMenu["Draw"].Add("Ball", new CheckBox("Draw ball position", true));
            SubMenu["Draw"].Add("Q", new CheckBox("Draw Q Range", true));
            SubMenu["Draw"].Add("W", new CheckBox("Draw W Range", true));
            SubMenu["Draw"].Add("R", new CheckBox("Draw R Range", true));

            SubMenu["Misc"] = Menu.AddSubMenu("Misc", "Misc");
            SubMenu["Misc"].Add("Overkill", new Slider("Overkill % for damage prediction", 10, 0, 100));
            SubMenu["Misc"].Add("BlockR", new CheckBox("Block R if will not hit", true));
            SubMenu["Misc"].Add("R", new CheckBox("Use R to Interrupt Channeling", true));
            SubMenu["Misc"].Add("E", new CheckBox("Use E to Initiate", true));
            SubMenu["Misc"].Add("Shield", new CheckBox("Use Shield On Enemy Missiles", false));
            SubMenu["Misc"].Add("W2", new Slider("Use W if Hit", 3, 1, 5));
            SubMenu["Misc"].Add("R2", new Slider("Use R if Hit", 4, 1, 5));
            SubMenu["Misc"].AddGroupLabel("Don't use R in:");
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                SubMenu["Misc"].Add("BlackList." + enemy.ChampionName, new CheckBox(enemy.ChampionName, false));
            }

            Game.OnTick += OnTick;
            GameObject.OnCreate += OnCreate;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;
            Interrupter.OnInterruptableSpell += OnInterruptableSpell;
            Gapcloser.OnGapcloser += OnGapcloser;
            Spellbook.OnCastSpell += OnCastSpell;
            Obj_AI_Base.OnBasicAttack += Obj_AI_Base_OnBasicAttack;
            BallObject =
                ObjectManager.Get<GameObject>()
                    .FirstOrDefault(
                        obj => obj.Name != null && !obj.IsDead && obj.IsValid && obj.Name.ToLower().Contains("doomball"));
            GameObject.OnCreate += MissileClient_OnCreate;
            GameObject.OnDelete += MissileClient_OnDelete;
        }


        private static void Obj_AI_Base_OnBasicAttack(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.Team != myHero.Team && args.Target.IsMe &&
                (GetCheckBox(SubMenu["Misc"], "Shield") || (IsCombo && GetCheckBox(SubMenu["Combo"], "Shield")) ||
                 (IsHarass && GetCheckBox(SubMenu["Harass"], "Shield"))))
            {
                if (sender is AIHeroClient)
                {
                    var hero = sender as AIHeroClient;
                    if (hero.IsMelee)
                    {
                        CastE(myHero);
                    }
                }
                else if (sender is Obj_AI_Turret)
                {
                    CastE(myHero);
                }
            }
        }

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == R.Slot && SubMenu["Misc"]["BlockR"].Cast<CheckBox>().CurrentValue &&
                (HitR() == 0 || BallObject is MissileClient))
            {
                args.Process = false;
            }
            if (IsHarass && !Orbwalker.CanMove && Orbwalker.LastTarget.Type != myHero.Type)
            {
                args.Process = false;
            }
        }

        private static void OnTick(EventArgs args)
        {
            if (myHero.IsDead)
            {
                return;
            }
            Q.SourcePosition = Ball;
            Q.RangeCheckSource = myHero.Position;
            E.SourcePosition = Ball;
            E.RangeCheckSource = myHero.Position;
            W.SourcePosition = Ball;
            W.RangeCheckSource = Ball;
            R.SourcePosition = Ball;
            R.RangeCheckSource = Ball;
            if (GetCheckBox(SubMenu["Misc"], "Shield"))
            {
                CheckMissiles();
            }
            if (R.IsReady() && SubMenu["Misc"]["R2"].Cast<Slider>().CurrentValue <= HitR())
            {
                myHero.Spellbook.CastSpell(R.Slot);
            }
            if (W.IsReady() &&
                SubMenu["Misc"]["W2"].Cast<Slider>().CurrentValue <=
                HitW(EntityManager.Heroes.Enemies.ToList<Obj_AI_Base>()))
            {
                myHero.Spellbook.CastSpell(W.Slot);
            }
            KillSteal();
            if (IsCombo)
            {
                Combo();
            }
            else if (IsHarass)
            {
                Harass();
            }
            else if (IsClear)
            {
                if (IsJungleClear)
                {
                    JungleClear();
                }
                if (IsLaneClear)
                {
                    LaneClear();
                }
            }
            else if (IsLastHit)
            {
                LastHit();
            }
            if (IsFlee)
            {
                Flee();
            }
        }

        private static void KillSteal()
        {
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                if (enemy.IsValidTarget(E.Range) && enemy.HealthPercent <= 40)
                {
                    var damageI = GetBestCombo(enemy);
                    if (damageI.Damage >= enemy.TotalShieldHealth())
                    {
                        if (SubMenu["KillSteal"]["Q"].Cast<CheckBox>().CurrentValue &&
                            (Damage(enemy, Q.Slot) >= enemy.TotalShieldHealth() || damageI.Q))
                        {
                            CastQ(enemy);
                        }
                        if (SubMenu["KillSteal"]["W"].Cast<CheckBox>().CurrentValue &&
                            (Damage(enemy, W.Slot) >= enemy.TotalShieldHealth() || damageI.W))
                        {
                            CastW(enemy);
                        }
                        if (SubMenu["KillSteal"]["E"].Cast<CheckBox>().CurrentValue &&
                            (Damage(enemy, E.Slot) >= enemy.TotalShieldHealth() || damageI.E))
                        {
                            CastE(enemy);
                        }
                        if (SubMenu["KillSteal"]["R"].Cast<CheckBox>().CurrentValue &&
                            (Damage(enemy, R.Slot) >= enemy.TotalShieldHealth() || damageI.R))
                        {
                            CastR(enemy);
                        }
                        if ((SubMenu["KillSteal"]["E"].Cast<CheckBox>().CurrentValue ||
                             SubMenu["KillSteal"]["Q"].Cast<CheckBox>().CurrentValue) &&
                            ((Damage(enemy, Q.Slot) >= enemy.TotalShieldHealth() || damageI.Q) ||
                             (Damage(enemy, W.Slot) >= enemy.TotalShieldHealth() || damageI.W) ||
                             (Damage(enemy, R.Slot) >= enemy.TotalShieldHealth() || damageI.R)))
                        {
                            ThrowBall(enemy);
                        }
                    }
                    if (Ignite != null && SubMenu["KillSteal"]["Ignite"].Cast<CheckBox>().CurrentValue &&
                        Ignite.IsReady() &&
                        myHero.GetSummonerSpellDamage(enemy, DamageLibrary.SummonerSpells.Ignite) >= enemy.TotalShieldHealth())
                    {
                        Ignite.Cast(enemy);
                    }
                }
            }
        }

        private static void Combo()
        {
            if (E.IsReady() && GetCheckBox(SubMenu["Combo"], "Shield"))
            {
                CheckMissiles();
            }
            var target = TargetSelector.GetTarget(Q.Range + Q.Width, DamageType.Magical);
            if (target.IsValidTarget())
            {
                var damageI = GetBestCombo(target);
                if (myHero.CountEnemiesInRange(E.Range) >= SubMenu["Combo"]["TF"].Cast<Slider>().CurrentValue)
                {
                    if (Q.IsReady() && SubMenu["Combo"]["Q2"].Cast<Slider>().CurrentValue > 0)
                    {
                        var list =
                            EntityManager.Heroes.Enemies.Where<Obj_AI_Base>(o => o.IsValidTarget(Q.Range + Q.Width))
                                .ToList();
                        if (list.Count >= SubMenu["Combo"]["Q2"].Cast<Slider>().CurrentValue)
                        {
                            var info = BestHitQ(list);
                            if (info.Item1 != Vector3.Zero &&
                                info.Item2 >= SubMenu["Combo"]["Q2"].Cast<Slider>().CurrentValue)
                            {
                                Q.Cast(info.Item1);
                            }
                        }
                    }
                    if (W.IsReady() && SubMenu["Combo"]["W2"].Cast<Slider>().CurrentValue > 0)
                    {
                        if (HitW(EntityManager.Heroes.Enemies.ToList<Obj_AI_Base>()) >=
                            SubMenu["Combo"]["W2"].Cast<Slider>().CurrentValue)
                        {
                            myHero.Spellbook.ChastSpell(W.Slot);
                        }
                    }
                    if (R.IsReady() && SubMenu["Combo"]["R2"].Cast<Slider>().CurrentValue > 0 &&
                        HitR() >= SubMenu["Combo"]["R2"].Cast<Slider>().CurrentValue)
                    {
                        myHero.Spellbook.CastSpell(R.Slot);
                    }
                    CastQR();
                    CastER();
                }
                else
                {
                    if (SubMenu["Combo"]["R"].Cast<CheckBox>().CurrentValue && damageI.R &&
                        damageI.Damage >= target.TotalShieldHealth())
                    {
                        CastR(target);
                    }
                }
                if (SubMenu["Combo"]["Q"].Cast<CheckBox>().CurrentValue)
                {
                    //if (Game.Time - LastGapclose < 0.2f) { return; }
                    CastQ(target);
                }
                if (W.IsReady() && SubMenu["Combo"]["W"].Cast<CheckBox>().CurrentValue)
                {
                    CastW(target);
                }
                if (E.IsReady() && SubMenu["Combo"]["E"].Cast<Slider>().CurrentValue > 0)
                {
                    var list = EntityManager.Heroes.Enemies.Where<Obj_AI_Base>(o => o.IsValidTarget(E.Range)).ToList();
                    if (list.Count >= SubMenu["Combo"]["E"].Cast<Slider>().CurrentValue)
                    {
                        var info = BestHitE(list);
                        if (info.Item1 != null && info.Item2 > 0)
                        {
                            var bestAlly = info.Item1;
                            if (info.Item2 > SubMenu["Combo"]["E"].Cast<Slider>().CurrentValue && bestAlly.IsValid)
                            {
                                CastE(bestAlly);
                            }
                        }
                    }
                }
                if (E.IsReady() && SubMenu["Combo"]["E2"].Cast<Slider>().CurrentValue > myHero.HealthPercent &&
                    myHero.HealthPercent < target.HealthPercent)
                {
                    foreach (var enemy in EntityManager.Heroes.Enemies.Where(o => o.IsValidTarget(E.Range)))
                    {
                        if (enemy.IsInAutoAttackRange(myHero))
                        {
                            CastE(myHero);
                        }
                    }
                }
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range + Q.Width, DamageType.Magical);
            if (myHero.ManaPercent >= SubMenu["Harass"]["Mana"].Cast<Slider>().CurrentValue)
            {
                if (target.IsValidTarget())
                {
                    var damageI = GetBestCombo(target);
                    if (SubMenu["Harass"]["Q"].Cast<CheckBox>().CurrentValue)
                    {
                        CastQ(target);
                    }
                    if (W.IsReady() && SubMenu["Harass"]["W"].Cast<CheckBox>().CurrentValue)
                    {
                        CastW(target);
                    }
                    if (E.IsReady() && SubMenu["Harass"]["E"].Cast<Slider>().CurrentValue > 0)
                    {
                        var list =
                            EntityManager.Heroes.Enemies.Where<Obj_AI_Base>(o => o.IsValidTarget(E.Range)).ToList();
                        if (list.Count >= SubMenu["Harass"]["E"].Cast<Slider>().CurrentValue)
                        {
                            var info = BestHitE(list);
                            if (info.Item1 != null && info.Item2 > 0)
                            {
                                var bestAlly = info.Item1;
                                if (info.Item2 > SubMenu["Harass"]["E"].Cast<Slider>().CurrentValue && bestAlly.IsValid)
                                {
                                    CastE(bestAlly);
                                }
                            }
                        }
                    }
                    if (E.IsReady() && SubMenu["Harass"]["E2"].Cast<Slider>().CurrentValue > myHero.HealthPercent &&
                        myHero.HealthPercent < target.HealthPercent)
                    {
                        foreach (var enemy in EntityManager.Heroes.Enemies.Where(o => o.IsValidTarget(E.Range)))
                        {
                            if (enemy.IsInAutoAttackRange(myHero))
                            {
                                CastE(myHero);
                            }
                        }
                    }
                }
                if (E.IsReady() && GetCheckBox(SubMenu["Harass"], "Shield"))
                {
                    CheckMissiles();
                }
            }
        }

        private static void LaneClear()
        {
            if (myHero.ManaPercent >= SubMenu["LaneClear"]["Mana"].Cast<Slider>().CurrentValue)
            {
                if (E.IsReady() && SubMenu["LaneClear"]["E"].Cast<Slider>().CurrentValue > 0)
                {
                    var minions = Orbwalker.LaneclearMinions.Concat(Orbwalker.UnLasthittableMinions).Where(m => myHero.IsInRange(m, E.Range)).ToList<Obj_AI_Base>();
                    if (minions.Count >= SubMenu["LaneClear"]["E"].Cast<Slider>().CurrentValue)
                    {
                        var info = BestHitE(minions);
                        if (info.Item1 != null)
                        {
                            var bestAlly = info.Item1;
                            var bestHit = info.Item2;
                            if (SubMenu["LaneClear"]["E"].Cast<Slider>().CurrentValue > 0 &&
                                bestHit >= SubMenu["LaneClear"]["E"].Cast<Slider>().CurrentValue && bestAlly.IsValid)
                            {
                                CastE(bestAlly);
                            }
                        }
                    }
                }
                if (Q.IsReady() && SubMenu["LaneClear"]["Q"].Cast<Slider>().CurrentValue > 0)
                {
                    var minions = Orbwalker.LaneclearMinions.Concat(Orbwalker.UnLasthittableMinions).Where(m => myHero.IsInRange(m, Q.Range + Q.Width)).ToList<Obj_AI_Base>();
                    if (minions.Count >= SubMenu["LaneClear"]["Q"].Cast<Slider>().CurrentValue)
                    {
                        var info2 = BestHitQ(minions);
                        if (info2.Item1 != Vector3.Zero)
                        {
                            var bestPos = info2.Item1;
                            var bestHit = info2.Item2;
                            if (SubMenu["LaneClear"]["Q"].Cast<Slider>().CurrentValue > 0 &&
                                bestHit >= SubMenu["LaneClear"]["Q"].Cast<Slider>().CurrentValue)
                            {
                                Q.Cast(bestPos);
                            }
                        }
                    }
                }
                if (W.IsReady() && SubMenu["LaneClear"]["W"].Cast<Slider>().CurrentValue > 0 &&
                    HitW(Orbwalker.LaneclearMinions.Concat(Orbwalker.UnLasthittableMinions).Where(m => Ball.IsInRange(m, m.BoundingRadius + W.Width)).ToList<Obj_AI_Base>()) >=
                    SubMenu["LaneClear"]["Q"].Cast<Slider>().CurrentValue)
                {
                    myHero.Spellbook.CastSpell(W.Slot);
                }
                if (SubMenu["LaneClear"]["Q2"].Cast<CheckBox>().CurrentValue)
                {
                    LastHitSpell(Q);
                }
            }
        }

        private static int GetLastHitTime(this Spell.Skillshot spell, Obj_AI_Base minion)
        {
            return (int)(1000 * (spell.SourcePosition ?? Player.Instance.ServerPosition).Distance(minion) / spell.Speed) + spell.CastDelay;
        }

        private static void LastHit(this Spell.Skillshot spell)
        {
            foreach (var tuple in Prediction.Health.GetPrediction(Orbwalker.LaneclearMinions.Concat(Orbwalker.UnLasthittableMinions).ToList<Obj_AI_Base>().ToDictionary(minion => minion, spell.GetLastHitTime)).Where(tuple => tuple.Value >= 0 && Damage(tuple.Key, spell.Slot) >= tuple.Value))
            {
                spell.Cast(tuple.Key);
            }
        }

        private static void LastHit()
        {
            if (myHero.ManaPercent >= SubMenu["LastHit"]["Mana"].Cast<Slider>().CurrentValue)
            {
                if (SubMenu["LastHit"]["Q"].Cast<CheckBox>().CurrentValue)
                {
                    LastHitSpell(Q);
                }
            }
        }

        private static void LastHitSpell(Spell.Skillshot s)
        {
            if (s.IsReady())
            {
                foreach (var minion in from minion in Orbwalker.UnLasthittableMinions let predHealth = Prediction.Health.GetPrediction(minion, (int)(1000f * (s.SourcePosition ?? myHero.Position).Distance(minion) / s.Speed) + s.CastDelay) where predHealth >= 0 where Damage(minion, s.Slot) >= predHealth select minion)
                {
                    CastQ(minion);
                }
                /*
                var enemyminions =
                    EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, myHero.Position,
                        s.Range + s.Width, true).Where(o => o.Health <= 2.0f * Damage(o, s.Slot));
                if (enemyminions.Any())
                {
                    foreach (var minion in enemyminions)
                    {
                        var canCalculate = false;
                        if (minion.IsValidTarget())
                        {
                            if (!Orbwalker.CanAutoAttack)
                            {
                                if (Orbwalker.CanMove && Orbwalker.LastTarget != null &&
                                    Orbwalker.LastTarget.NetworkId != minion.NetworkId)
                                {
                                    canCalculate = true;
                                }
                            }
                            else
                            {
                                if (!myHero.IsInAutoAttackRange(minion))
                                {
                                    canCalculate = true;
                                }
                                else
                                {
                                    var speed = myHero.BasicAttack.MissileSpeed;
                                    var time =
                                        (int)
                                            (1000 * myHero.Distance(minion) / speed + myHero.AttackCastDelay * 1000);
                                    var predHealth = Prediction.Health.GetPrediction(minion, time);
                                    if (predHealth <= -20)
                                    {
                                        canCalculate = true;
                                    }
                                }
                            }
                        }
                        if (canCalculate)
                        {
                            var dmg = Damage(minion, s.Slot);
                            var time = (int)(1000 * s.SourcePosition.Value.Distance(minion) / s.Speed + s.CastDelay);
                            var predHealth = Prediction.Health.GetPrediction(minion, time);
                            if (time > 0 && Math.Abs(predHealth - minion.Health) < float.Epsilon)
                            {
                                return;
                            }
                            if (dmg > predHealth && predHealth > 0)
                            {
                            }
                        }
                    }
                }*/
            }
        }

        private static void JungleClear()
        {
            if (myHero.ManaPercent >= SubMenu["JungleClear"]["Mana"].Cast<Slider>().CurrentValue)
            {
                var jungleminions =
                    EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Monster,
                        EntityManager.UnitTeam.Enemy, myHero.Position, E.Range, true);
                if (jungleminions.Any())
                {
                    foreach (var minion in jungleminions.Where(minion => myHero.ManaPercent >= SubMenu["JungleClear"]["Mana"].Cast<Slider>().CurrentValue))
                    {
                        if (SubMenu["JungleClear"]["E"].Cast<CheckBox>().CurrentValue)
                        {
                            CastE(minion);
                        }
                        if (SubMenu["JungleClear"]["Q"].Cast<CheckBox>().CurrentValue)
                        {
                            CastQ(minion);
                        }
                        if (SubMenu["JungleClear"]["W"].Cast<CheckBox>().CurrentValue)
                        {
                            CastW(minion);
                        }
                    }
                }
            }
        }

        private static void Flee()
        {
            if (SubMenu["Flee"]["Q"].Cast<CheckBox>().CurrentValue)
            {
                if (Q.IsReady() && myHero.Distance(Ball, true) > W.RangeSquared && !E.IsReady() && BallObject != null &&
                    !BallObject.Name.ToLower().Contains("missile"))
                {
                    myHero.Spellbook.CastSpell(Q.Slot, myHero.ServerPosition);
                }
            }
            if (SubMenu["Flee"]["W"].Cast<CheckBox>().CurrentValue)
            {
                if (W.IsReady() && myHero.Distance(Ball, true) < W.RangeSquared)
                {
                    myHero.Spellbook.CastSpell(W.Slot);
                }
            }
            if (SubMenu["Flee"]["E"].Cast<CheckBox>().CurrentValue)
            {
                if (E.IsReady() && myHero.Distance(Ball, true) > W.RangeSquared)
                {
                    CastE(myHero);
                }
            }
        }

        private static void CheckMissiles()
        {
            if (E.IsReady() && missiles.Count > 0)
            {
                foreach (var m in missiles.Where(a => a.IsValidMissile()))
                {
                    var hero = myHero;
                    var canCast = false;
                    if (m.Target != null)
                    {
                        canCast = m.Target.IsMe;
                    }
                    if (m.EndPosition != null && m.SData.LineWidth > 0f)
                    {
                        const float multiplier = 1.15f;
                        var width = (m.SData.LineWidth + hero.BoundingRadius) * multiplier;
                        var widthSqrt = width * width;
                        var startpos = m.StartPosition != null ? m.StartPosition : m.SpellCaster.Position;
                        var extendedendpos = m.EndPosition + (m.EndPosition - startpos).Normalized() * width;
                        var info = hero.Position.To2D().ProjectOn(startpos.To2D(), extendedendpos.To2D());
                        canCast = info.IsOnSegment &&
                                  info.SegmentPoint.Distance(hero.Position.To2D(), true) <= widthSqrt;
                    }
                    if (canCast)
                    {
                        CastE(hero);
                    }
                }
            }
        }

        private static void CastQ(Obj_AI_Base target, int minhits = 1)
        {
            if (Q.IsReady() && target.IsValidTarget(Q.Range + Q.Width))
            {
                if (E.IsReady() &&
                    myHero.Mana >=
                    myHero.Spellbook.GetSpell(Q.Slot).SData.Mana + myHero.Spellbook.GetSpell(E.Slot).SData.Mana &&
                    target.Type == myHero.Type && Ball.Distance(target, true) > Math.Pow(Q.Range * 1.2f, 2) &&
                    myHero.Distance(target, true) < Ball.Distance(target, true))
                {
                    var pred = Q.GetPrediction(target);
                    if (pred.HitChancePercent <= 5)
                    {
                        Q.SourcePosition = myHero.Position;
                        if (Q.GetPrediction(target).HitChancePercent >= HitChancePercent(Q.Slot))
                        {
                            CastE(myHero);
                        }
                        Q.SourcePosition = Ball;
                    }
                }
                var list = new List<Obj_AI_Base>();
                if (target.Type == myHero.Type)
                {
                    list =
                        EntityManager.Heroes.Enemies.Where(o => o.IsValidTarget(Q.Range + Q.Width)).ToList<Obj_AI_Base>();
                }
                else
                {
                    var enemyminions =
                        EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Minion,
                            EntityManager.UnitTeam.Enemy, myHero.Position, Q.Range + Q.Width).ToList<Obj_AI_Base>();
                    var jungleminions =
                        EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Monster,
                            EntityManager.UnitTeam.Enemy, myHero.Position, Q.Range + Q.Width).ToList<Obj_AI_Base>();
                    if (enemyminions.Count > 0)
                    {
                        list = enemyminions;
                    }
                    else if (jungleminions.Count > 0)
                    {
                        list = jungleminions.ToList();
                    }
                }
                if (list.Count < minhits)
                {
                    return;
                }
                var t = BestHitQ(list, target);
                if (t.Item1 != Vector3.Zero && t.Item2 >= minhits)
                {
                    Q.Cast(t.Item1);
                }
                /**
                var pred = Q.GetPrediction(target);
                if (pred.HitChancePercent >= 70)
                {
                    Q.Cast(pred.CastPosition);
                }**/
            }
        }

        private static void CastW(Obj_AI_Base target)
        {
            if (W.IsReady())
            {
                var pred = W.GetPrediction(target);
                if (pred.HitChancePercent >= HitChancePercent(W.Slot) &&
                    Ball.Distance(pred.CastPosition, true) < W.RangeSquared)
                {
                    myHero.Spellbook.CastSpell(W.Slot);
                }
            }
        }

        private static void CastE(Obj_AI_Base target)
        {
            if (E.IsReady() && target != null && target.IsValid && myHero.Distance(target, true) < E.RangeSquared)
            {
                if (target.Team == myHero.Team)
                {
                    myHero.Spellbook.CastSpell(E.Slot, target);
                }
                else
                {
                    var list = new List<Obj_AI_Base>();
                    if (target.Type == myHero.Type)
                    {
                        list = EntityManager.Heroes.Enemies.ToList<Obj_AI_Base>();
                    }
                    else
                    {
                        var enemyminions =
                            EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Minion,
                                EntityManager.UnitTeam.Enemy, myHero.Position, Q.Range + Q.Width).ToList<Obj_AI_Base>();
                        var jungleminions =
                            EntityManager.MinionsAndMonsters.Get(EntityManager.MinionsAndMonsters.EntityType.Monster,
                                EntityManager.UnitTeam.Enemy, myHero.Position, Q.Range + Q.Width).ToList<Obj_AI_Base>();
                        if (enemyminions.Count > 0)
                        {
                            list = enemyminions;
                        }
                        else if (jungleminions.Count > 0)
                        {
                            list = jungleminions;
                        }
                    }
                    if (list.Count > 0)
                    {
                        var info = BestHitE(list);
                        if (info.Item1 != null)
                        {
                            var bestAlly = info.Item1;
                            var bestHit = info.Item2;
                            if (bestHit > 0 && bestAlly.IsValid)
                            {
                                CastE(bestAlly);
                            }
                        }
                    }
                }
            }
        }

        private static void ThrowBall(Obj_AI_Base target)
        {
            Obj_AI_Base eAlly = null;
            var predictedPos = Vector3.Zero;
            if (E.IsReady() && target.IsValidTarget() && Ball.Distance(target, true) > R.RangeSquared)
            {
                var pred = E.GetPrediction(target);
                foreach (
                    var ally in
                        EntityManager.Heroes.Allies.Where(
                            o => o.IsValid && myHero.Distance(o, true) < E.RangeSquared && Ball.Distance(o, true) > 0))
                {
                    var pred2 = E.GetPrediction(ally);
                    if (pred.CastPosition.Distance(pred2.CastPosition, true) <= R.RangeSquared * 1.5f * 1.5f)
                    {
                        if (eAlly == null)
                        {
                            eAlly = ally;
                            predictedPos = pred2.CastPosition;
                        }
                        else if (pred.CastPosition.Distance(predictedPos, true) >
                                 pred.CastPosition.Distance(pred2.CastPosition, true))
                        {
                            eAlly = ally;
                            predictedPos = pred2.CastPosition;
                        }
                    }
                }
            }
            if (eAlly != null)
            {
                CastE(eAlly);
            }
            else
            {
                CastQ(target);
            }
        }

        private static int HitW(List<Obj_AI_Base> list)
        {
            var count = 0;
            if (W.IsReady() && list.Count > 0)
            {
                count += list.Select(target => W.GetPrediction(target)).Count(pred => pred.HitChancePercent >= HitChancePercent(W.Slot));
            }
            return count;
        }

        private static int HitR()
        {
            var count = 0;
            if (R.IsReady())
            {
                count += EntityManager.Heroes.Enemies.Where(h=> h.IsValidTarget()).Select(target => R.GetPrediction(target)).Count(pred => pred.HitChancePercent >= HitChancePercent(R.Slot));
            }
            return count;
        }

        private static void CastQR()
        {
            if (Q.IsReady() && R.IsReady())
            {
                var qWidth = Q.Width;
                var qDelay = Q.CastDelay;
                Q.CastDelay = R.CastDelay;
                Q.Width = (int)R.Range;
                var positions = (from enemy in EntityManager.Heroes.Enemies.Where(o => o.IsValidTarget(Q.Range + R.Range)) select Q.GetPrediction(enemy) into pred where pred.HitChancePercent >= HitChancePercent(R.Slot) select pred.CastPosition.To2D()).ToList();
                if (positions.Count > 0)
                {
                    var bestPos = Vector2.Zero;
                    var bestCount = 0;
                    foreach (var vec in positions)
                    {
                        var count = positions.Count(v => vec.Distance(v, true) < Math.Pow(R.Width * 1.4f, 2));
                        if (bestPos == Vector2.Zero)
                        {
                            bestPos = vec;
                            bestCount = count;
                        }
                        else if (bestCount < count)
                        {
                            bestPos = vec;
                            bestCount = count;
                        }
                    }
                    if (bestCount >= SubMenu["Combo"]["R2"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(bestPos.To3D());
                    }
                }
                Q.Width = qWidth;
                Q.CastDelay = qDelay;
            }
        }

        private static void CastER()
        {
            if (E.IsReady() && R.IsReady())
            {
                var bestCount = -1;
                Obj_AI_Base bestAlly = null;
                foreach (
                    var ally in
                        EntityManager.Heroes.AllHeroes.Where(
                            o => o.IsValid && o.Team == myHero.Team && myHero.Distance(o, true) < E.RangeSquared))
                {
                    var count = ally.CountEnemiesInRange(R.Range * 1.5f);
                    if (bestCount == -1)
                    {
                        bestCount = count;
                        bestAlly = ally;
                    }
                    else if (bestCount < count)
                    {
                        bestCount = count;
                        bestAlly = ally;
                    }
                }

                if (bestCount >= SubMenu["Combo"]["R2"].Cast<Slider>().CurrentValue)
                {
                    CastE(bestAlly);
                }
            }
        }

        private static Tuple<int, Dictionary<int, bool>> CountHitQ(Vector3 startPos, Vector3 endPos,
            List<Obj_AI_Base> list, Obj_AI_Base target)
        {
            var counted = new Dictionary<int, bool>();
            counted[target.NetworkId] = true;
            if (Q.IsReady() && list.Count > 0)
            {
                foreach (
                    var obj in list.Where(o => o.IsValidTarget(Q.Range + Q.Width) && target.NetworkId != o.NetworkId))
                {
                    var extendedendpos = endPos + (endPos - startPos).Normalized() * Q.Width;
                    var info = obj.ServerPosition.To2D().ProjectOn(startPos.To2D(), extendedendpos.To2D());
                    if (info.IsOnSegment &&
                        obj.ServerPosition.To2D().Distance(info.SegmentPoint, true) <=
                        Math.Pow(Q.Width * 1.5f + obj.BoundingRadius / 3, 2))
                    {
                        var hitchancepercent = obj.Type == myHero.Type ? HitChancePercent(Q.Slot) : 30;
                        var pred = Q.GetPrediction(obj);
                        if (pred.HitChancePercent >= hitchancepercent)
                        {
                            info = pred.CastPosition.To2D().ProjectOn(startPos.To2D(), extendedendpos.To2D());
                            if (info.IsOnSegment &&
                                pred.CastPosition.To2D().Distance(info.SegmentPoint, true) <=
                                Math.Pow(Q.Width + obj.BoundingRadius / 3, 2))
                            {
                                counted[obj.NetworkId] = true;
                            }
                        }
                    }
                }
            }
            return new Tuple<int, Dictionary<int, bool>>(counted.Count, counted);
        }

        private static Tuple<Vector3, int> BestHitQ(List<Obj_AI_Base> list, AttackableUnit target = null)
        {
            if (Game.Time < Q_LastRequest)
            {
                return new Tuple<Vector3, int>(Vector3.Zero, 0);
            }
            Q_LastRequest = Game.Time + (float)Math.Pow(list.Count, 3) / 1000;
            var bestPos = Vector3.Zero;
            var bestHit = -1;
            var checktarget = target != null && target.IsValidTarget();
            if (Q.IsReady() && list.Count > 0)
            {
                foreach (var obj in list.Where(o => o.IsValidTarget(Q.Range + Q.Width)))
                {
                    var pred = Q.GetPrediction(obj);
                    var hitchancepercent = obj.Type == myHero.Type ? HitChancePercent(Q.Slot) : 30;
                    if (pred.HitChancePercent >= hitchancepercent)
                    {
                        var t = CountHitQ(Ball, pred.CastPosition, list, obj);
                        var hit = t.Item1;
                        var counted = t.Item2;
                        var b = true;
                        if (checktarget)
                        {
                            b = counted.ContainsKey(target.NetworkId);
                        }
                        if (hit <= bestHit || !b) continue;
                        bestHit = hit;
                        bestPos = pred.CastPosition;
                        if (bestHit == list.Count)
                        {
                            break;
                        }
                    }
                }
            }
            return new Tuple<Vector3, int>(bestPos, bestHit);
        }

        private static Tuple<int, Dictionary<int, bool>> CountHitE(Vector3 startPos, Vector3 endPos,
            IReadOnlyCollection<Obj_AI_Base> list)
        {
            var count = 0;
            var counted = new Dictionary<int, bool>();
            if (E.IsReady() && list.Count > 0)
            {
                foreach (var obj in list.Where(o => o.IsValidTarget(E.Range)))
                {
                    var info = obj.ServerPosition.To2D().ProjectOn(startPos.To2D(), endPos.To2D());
                    if (info.IsOnSegment &&
                        obj.ServerPosition.To2D().Distance(info.SegmentPoint, true) <=
                        Math.Pow(E.Width * 1.5f + obj.BoundingRadius / 3, 2))
                    {
                        var pred = E.GetPrediction(obj);
                        var hitchancepercent = obj.Type == myHero.Type ? HitChancePercent(E.Slot) : 30;
                        if (pred.HitChancePercent >= hitchancepercent &&
                            pred.CastPosition.Distance(myHero, true) <= E.RangeSquared)
                        {
                            info = pred.CastPosition.To2D().ProjectOn(startPos.To2D(), endPos.To2D());
                            if (info.IsOnSegment &&
                                pred.CastPosition.To2D().Distance(info.SegmentPoint, true) <=
                                Math.Pow(E.Width + obj.BoundingRadius / 3, 2))
                            {
                                count++;
                                counted[obj.NetworkId] = true;
                            }
                        }
                    }
                }
            }
            return new Tuple<int, Dictionary<int, bool>>(count, counted);
        }

        private static Tuple<Obj_AI_Base, int> BestHitE(List<Obj_AI_Base> list, Obj_AI_Base target = null)
        {
            if (Game.Time < E_LastRequest)
            {
                return new Tuple<Obj_AI_Base, int>(null, 0);
            }
            E_LastRequest = Game.Time + (float)Math.Pow(list.Count, 3) / 1000;
            Obj_AI_Base bestAlly = null;
            var bestHit = 0;
            var checktarget = target != null && target.IsValidTarget();
            if (E.IsReady() && list.Count > 0)
            {
                foreach (
                    AIHeroClient ally in
                        EntityManager.Heroes.AllHeroes.Where(
                            o => o.IsValid && o.Team == myHero.Team && myHero.Distance(o, true) < E.RangeSquared))
                {
                    if (Ball.Distance(ally, true) > 0)
                    {
                        var pred = E.GetPrediction(ally);
                        var info = CountHitE(Ball, pred.CastPosition, list);
                        var hit = info.Item1;
                        var counted = info.Item2;
                        var b = true;
                        if (checktarget)
                        {
                            b = counted.ContainsKey(target.NetworkId);
                        }
                        if (hit > bestHit && b)
                        {
                            bestHit = hit;
                            bestAlly = ally;
                            if (bestHit == list.Count)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return new Tuple<Obj_AI_Base, int>(bestAlly, bestHit);
        }

        private static void CastR(AIHeroClient target)
        {
            if (R.IsReady() && !SubMenu["Misc"]["BlackList." + target.ChampionName].Cast<CheckBox>().CurrentValue)
            {
                var pred = R.GetPrediction(target);
                if (pred.HitChancePercent >= HitChancePercent(R.Slot) &&
                    Ball.Distance(pred.CastPosition, true) < R.RangeSquared)
                {
                    myHero.Spellbook.CastSpell(R.Slot);
                }
            }
        }


        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.Team == myHero.Team)
            {
                var target = TargetSelector.GetTarget(E.Range, DamageType.Magical, myHero.Position);
                if (SubMenu["Misc"]["E"].Cast<CheckBox>().CurrentValue && target.IsValidTarget() && e.End.Distance(target, true) <= Ball.Distance(target, true))
                {
                    CastE(sender);
                    LastGapclose = Game.Time;
                }
            }
        }

        private static void OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            var hero = sender as AIHeroClient;
            if (sender.Team != myHero.Team && hero != null)
            {
                if (SubMenu["Misc"]["R"].Cast<CheckBox>().CurrentValue)
                {
                    if (Ball.Distance(sender, true) > R.RangeSquared)
                    {
                        ThrowBall(sender);
                    }
                    else
                    {
                        CastR(hero);
                    }
                }
            }
        }

        private static void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Animation.ToLower().Equals("prop"))
                {
                    BallObject = sender;
                }
            }
        }

        private static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.E)
                {
                    E_Target = args.Target;
                }
            }
            if (sender.Team != myHero.Team && args.Target != null && args.Target.IsMe &&
                (GetCheckBox(SubMenu["Misc"], "Shield") || (IsCombo && GetCheckBox(SubMenu["Combo"], "Shield")) ||
                 (IsHarass && GetCheckBox(SubMenu["Harass"], "Shield"))))
            {
                if (sender is AIHeroClient)
                {
                    CastE(myHero);
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (myHero.IsDead)
            {
                return;
            }
            if (SubMenu["Draw"]["Ball"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(new ColorBGRA(0, 0, 255, 100), 120, Ball);
            }
            if (SubMenu["Draw"]["Q"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                Circle.Draw(new ColorBGRA(255, 255, 255, 100), Q.Range, myHero.Position);
            }
            if (SubMenu["Draw"]["W"].Cast<CheckBox>().CurrentValue && W.IsReady())
            {
                Circle.Draw(new ColorBGRA(255, 255, 255, 100), W.Range, Ball);
            }
            if (SubMenu["Draw"]["R"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                Circle.Draw(new ColorBGRA(255, 255, 255, 100), R.Range, Ball);
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender != null && sender.Name != null)
            {
                if (sender is Obj_GeneralParticleEmitter &&
                    sender.Name.ToLower().Contains(myHero.ChampionName.ToLower()))
                {
                    if (sender.Name.ToLower().Contains("yomu") && sender.Name.ToLower().Contains("green"))
                    {
                        BallObject = sender;
                    }
                }
            }
        }

        private static void MissileClient_OnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (missile != null)
            {
                if (missile.SpellCaster.IsMe &&
                    (missile.SData.Name.ToLower().Contains("orianaizuna") ||
                     missile.SData.Name.ToLower().Contains("orianaredact")))
                {
                    BallObject = sender;
                }
                if (GetCheckBox(SubMenu["Combo"], "Shield") || GetCheckBox(SubMenu["Harass"], "Shield") ||
                    GetCheckBox(SubMenu["Misc"], "Shield"))
                {
                    var spellCaster = missile.SpellCaster;
                    if (!spellCaster.Name.ToLower().Contains("minion") && myHero.Team != missile.SpellCaster.Team &&
                        myHero.Distance(sender, true) < E.RangeSquared * 1.5f)
                    //(missile.SpellCaster.Type == GameObjectType.AIHeroClient && missile.SpellCaster.Type == GameObjectType.obj_AI_Turret) &&
                    {
                        missiles.Add(missile);
                        //Core.DelayAction(delegate { missiles.Remove(missile); }, 1000 * (int)(1.25f * Extensions.Distance(missile.Position, missile.EndPosition) / missile.SData.MissileSpeed));
                    }
                }
            }
        }

        private static void MissileClient_OnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (missile != null)
            {
                if (missile.SpellCaster.IsMe && missile.SData.Name.ToLower().Contains("orianaredact"))
                {
                    BallObject = E_Target;
                }
                if (GetCheckBox(SubMenu["Combo"], "Shield") || GetCheckBox(SubMenu["Harass"], "Shield") ||
                    GetCheckBox(SubMenu["Misc"], "Shield"))
                {
                    var spellCaster = missile.SpellCaster;
                    if (!spellCaster.Name.ToLower().Contains("minion") && myHero.Team != missile.SpellCaster.Team)
                    //(missile.SpellCaster.Type == GameObjectType.AIHeroClient && missile.SpellCaster.Type == GameObjectType.obj_AI_Turret) &&
                    {
                        if (missiles.Count > 0)
                        {
                            missiles.RemoveAll(m => m.NetworkId == missile.NetworkId);
                        }
                    }
                }
            }
        }

        private static float HitChancePercent(SpellSlot s)
        {
            string slot;
            switch (s)
            {
                case SpellSlot.Q:
                    slot = "Q";
                    break;
                case SpellSlot.W:
                    slot = "W";
                    break;
                case SpellSlot.E:
                    slot = "E";
                    break;
                case SpellSlot.R:
                    slot = "R";
                    break;
                default:
                    slot = "Q";
                    break;
            }
            if (IsHarass)
            {
                return SubMenu["Prediction"][slot + "Harass"].Cast<Slider>().CurrentValue;
            }
            return SubMenu["Prediction"][slot + "Combo"].Cast<Slider>().CurrentValue;
        }

        private static float Damage(Obj_AI_Base target, SpellSlot slot)
        {
            if (target.IsValidTarget())
            {
                switch (slot)
                {
                    case SpellSlot.Q:
                        return myHero.CalculateDamageOnUnit(target, DamageType.Magical, 30f * Q.Level + 30f + 0.5f * myHero.TotalMagicalDamage);
                    case SpellSlot.W:
                        return myHero.CalculateDamageOnUnit(target, DamageType.Magical, 45f * W.Level + 25f + 0.7f * myHero.TotalMagicalDamage);
                    case SpellSlot.E:
                        return myHero.CalculateDamageOnUnit(target, DamageType.Magical, 30f * E.Level + 30f + 0.3f * myHero.TotalMagicalDamage);
                    case SpellSlot.R:
                        return myHero.CalculateDamageOnUnit(target, DamageType.Magical, 75f * R.Level + 75f + 0.7f * myHero.TotalMagicalDamage);
                }
            }
            return myHero.GetSpellDamage(target, slot);
        }

        private static DamageInfo GetComboDamage(Obj_AI_Base target, bool q, bool w, bool e, bool r)
        {
            var comboDamage = 0f;
            var manaWasted = 0f;
            if (target.IsValidTarget())
            {
                if (q)
                {
                    comboDamage += Damage(target, Q.Slot);
                    manaWasted += myHero.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;
                }
                if (w)
                {
                    comboDamage += Damage(target, W.Slot);
                    manaWasted += myHero.Spellbook.GetSpell(SpellSlot.W).SData.Mana;
                }
                if (e)
                {
                    comboDamage += Damage(target, E.Slot);
                    manaWasted += myHero.Spellbook.GetSpell(SpellSlot.E).SData.Mana;
                }
                if (r)
                {
                    comboDamage += Damage(target, R.Slot);
                    manaWasted += myHero.Spellbook.GetSpell(SpellSlot.R).SData.Mana;
                }
                if (Ignite != null && Ignite.IsReady())
                {
                    comboDamage += myHero.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite);
                }
                comboDamage += myHero.GetAutoAttackDamage(target, true);
            }
            comboDamage = comboDamage * Overkill;
            return new DamageInfo(comboDamage, manaWasted);
        }

        private static DamageInfo GetBestCombo(Obj_AI_Base target)
        {
            var q = Q.IsReady() ? new[] { false, true } : new[] { false };
            var w = W.IsReady() ? new[] { false, true } : new[] { false };
            var e = E.IsReady() ? new[] { false, true } : new[] { false };
            var r = R.IsReady() ? new[] { false, true } : new[] { false };
            if (target.IsValidTarget())
            {
                DamageInfo damageI2;
                if (PredictedDamage.ContainsKey(target.NetworkId))
                {
                    var damageI = PredictedDamage[target.NetworkId];
                    if (Game.Time - damageI.Time <= RefreshTime)
                    {
                        return damageI;
                    }
                    bool[] best =
                    {
                        false, false, false, false
                    };
                    var bestdmg = 0f;
                    var bestmana = 0f;
                    foreach (var r1 in r)
                    {
                        foreach (var q1 in q)
                        {
                            foreach (var w1 in w)
                            {
                                foreach (var e1 in e)
                                {
                                    damageI2 = GetComboDamage(target, q1, w1, e1, r1);
                                    var d = damageI2.Damage;
                                    var m = damageI2.Mana;
                                    if (myHero.Mana >= m)
                                    {
                                        if (bestdmg >= target.TotalShieldHealth())
                                        {
                                            if (d >= target.TotalShieldHealth() &&
                                                (d < bestdmg || m < bestmana || (best[3] && !damageI2.R)))
                                            {
                                                bestdmg = d;
                                                bestmana = m;
                                                best = new[] { q1, w1, e1, r1 };
                                            }
                                        }
                                        else
                                        {
                                            if (d >= bestdmg)
                                            {
                                                bestdmg = d;
                                                bestmana = m;
                                                best = new[] { q1, w1, e1, r1 };
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    PredictedDamage[target.NetworkId] = new DamageInfo(best[0], best[1], best[2], best[3], bestdmg,
                        bestmana, Game.Time);
                    return PredictedDamage[target.NetworkId];
                }
                damageI2 = GetComboDamage(target, Q.IsReady(), W.IsReady(), E.IsReady(), R.IsReady());
                PredictedDamage[target.NetworkId] = new DamageInfo(false, false, false, false, damageI2.Damage,
                    damageI2.Mana, Game.Time - Game.Ping * 2);
                return GetBestCombo(target);
            }
            return new DamageInfo(false, false, false, false, 0, 0, 0);
        }

        private static int GetSlider(Menu m, string s)
        {
            return m[s].Cast<Slider>().CurrentValue;
        }

        private static bool GetCheckBox(Menu m, string s)
        {
            return m[s].Cast<CheckBox>().CurrentValue;
        }

        private static bool GetKeyBind(Menu m, string s)
        {
            return m[s].Cast<KeyBind>().CurrentValue;
        }
    }
}