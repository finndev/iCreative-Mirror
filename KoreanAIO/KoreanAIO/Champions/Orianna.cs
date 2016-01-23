using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using KoreanAIO.Managers;
using KoreanAIO.Model;
using KoreanAIO.Utilities;
using SharpDX;

namespace KoreanAIO.Champions
{
    public class Orianna : ChampionBase
    {
        private GameObject _ballObject;
        private bool _canShield;
        private int _hitR;
        private int _hitW;
        private bool _ballIsMissile;
        public static string BallName = "Orianna_Base_Q_yomu_ring_green.troy";
        private const int QAoeWidth = 145;

        public Orianna()
        {
            try
            {
                AIO.Initializers.Add(delegate
                {
                    _ballObject =
                        ObjectManager.Get<Obj_GeneralParticleEmitter>()
                            .FirstOrDefault(
                                o => o.IsValid && !o.IsDead && o.Name.Equals(BallName));
                });
                Q = new SpellBase(SpellSlot.Q, SpellType.Circular, 815)
                {
                    Speed = 1200,
                    Width = 80,
                    CollidesWithYasuoWall = false
                };
                Q.SetSourceFunction(() => Ball);
                W = new SpellBase(SpellSlot.W, SpellType.Self, 255);
                W.SetSourceFunction(() => Ball);
                W.SetRangeCheckSourceFunction(() => Ball);
                E = new SpellBase(SpellSlot.E, SpellType.Circular, 1095)
                {
                    Speed = 1800,
                    Width = 85,
                    MinHitChancePercent = 45,
                    CollidesWithYasuoWall = false
                };
                E.SetSourceFunction(() => Ball);
                R = new SpellBase(SpellSlot.R, SpellType.Self, 400)
                {
                    CastDelay = 500
                };
                R.SetSourceFunction(() => Ball);
                R.SetRangeCheckSourceFunction(() => Ball);
                Spellbook.OnCastSpell += delegate (Spellbook sender, SpellbookCastSpellEventArgs args)
                {
                    if (sender.Owner.IsMe)
                    {
                        if (args.Slot == SpellSlot.R && MiscMenu.CheckBox("R.Block"))
                        {
                            args.Process = _hitR != 0 && !_ballIsMissile;
                        }
                    }
                };
                GameObject eTarget = null;
                Obj_AI_Base.OnProcessSpellCast += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
                {
                    if (sender.IsMe)
                    {
                        if (args.Slot == SpellSlot.E)
                        {
                            eTarget = args.Target;
                        }
                    }
                };
                GameObject.OnCreate += delegate (GameObject sender, EventArgs args)
                {
                    if (sender.Name.Equals(BallName))
                    {
                        _ballObject = sender;
                    }
                    else
                    {
                        var missile = sender as MissileClient;
                        if (missile != null && missile.SpellCaster != null && missile.SpellCaster.IsMe)
                        {
                            if (missile.SData.Name.Equals("orianaizuna") || missile.SData.Name.Equals("orianaredact"))
                            {
                                _ballObject = missile;
                            }
                        }
                    }
                };
                GameObject.OnDelete += delegate (GameObject sender, EventArgs args)
                {
                    var missile = sender as MissileClient;
                    if (missile != null && missile.SpellCaster != null && missile.SpellCaster.IsMe)
                    {
                        if (missile.SData.Name.Equals("orianaredact"))
                        {
                            _ballObject = eTarget;
                        }
                    }
                };
                Obj_AI_Base.OnPlayAnimation += delegate (Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
                {
                    if (sender.IsMe && args.Animation.Equals("Prop"))
                    {
                        _ballObject = sender;
                    }
                };
                Gapcloser.OnGapcloser += delegate (AIHeroClient sender, Gapcloser.GapcloserEventArgs args)
                {
                    if (sender.IsAlly)
                    {
                        if (Target != null && AutomaticMenu.CheckBox("Gapcloser") &&
                            Ball.GetDistanceSqr(Target) > args.End.Distance(Target, true) && args.End.Distance(Target, true) < args.Sender.GetDistanceSqr(Target))
                        {
                            CastE(sender);
                        }
                    }
                };
                Dash.OnDash += delegate (Obj_AI_Base sender, Dash.DashEventArgs args)
                {
                    if (sender.IsAlly)
                    {
                        if (Target != null && AutomaticMenu.CheckBox("Gapcloser") &&
                            Ball.GetDistanceSqr(Target) > args.EndPos.Distance(Target, true) && args.EndPos.Distance(Target, true) < sender.GetDistanceSqr(Target))
                        {
                            CastE(sender);
                        }
                    }
                };
                Interrupter.OnInterruptableSpell +=
                    delegate (Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs args)
                    {
                        if (sender.IsEnemy)
                        {
                            if (AutomaticMenu.CheckBox("Interrupter"))
                            {
                                if (!Ball.InRange(args.Sender, R.Range))
                                {
                                    ThrowBall(args.Sender);
                                }
                                else
                                {
                                    CastR(args.Sender);
                                }
                            }
                        }
                    };
                Obj_AI_Base.OnBasicAttack += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
                {
                    if (sender.IsEnemy && _canShield && args.Target != null && args.Target.IsMe)
                    {
                        if (sender.Type == GameObjectType.AIHeroClient)
                        {
                            if (sender.IsMelee)
                            {
                                CastE(MyHero);
                            }
                        }
                        else if (sender.Type == GameObjectType.obj_AI_Turret)
                        {
                            CastE(MyHero);
                        }
                    }
                };
                Obj_AI_Base.OnProcessSpellCast += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
                {
                    if (sender.IsEnemy && _canShield && args.Target != null && args.Target.IsMe)
                    {
                        if (sender.Type == GameObjectType.AIHeroClient)
                        {
                            CastE(MyHero);
                        }
                    }
                };

                Q.AddConfigurableHitChancePercent();
                R.AddConfigurableHitChancePercent();

                MenuManager.AddSubMenu("Combo");
                {
                    ComboMenu.Add("TeamFight", new Slider("Use TeamFight logic if enemies near >= {0}", 3, 1, 5));
                    ComboMenu.AddGroupLabel("Common logic");
                    ComboMenu.Add("Q", new CheckBox("Use Q on target"));
                    ComboMenu.Add("W", new CheckBox("Use W on target"));
                    ComboMenu.Add("E.Shield", new CheckBox("Use E on enemy spells"));
                    ComboMenu.Add("E.HealthPercent", new Slider("Use E if HealthPercent <= {0}", 40));
                    ComboMenu.AddGroupLabel("1 vs 1 logic");
                    ComboMenu.Add("R.Killable", new CheckBox("Use R on target if killable"));
                    ComboMenu.AddGroupLabel("TeamFight logic");
                    ComboMenu.Add("Q.Hit", new Slider("Use Q if hit >= {0}", 2, 1, 5));
                    ComboMenu.Add("W.Hit", new Slider("Use W if hit >= {0}", 2, 1, 5));
                    ComboMenu.Add("R.Hit", new Slider("Use R if hit >= {0}", 3, 1, 5));
                }

                MenuManager.AddSubMenu("Harass");
                {
                    HarassMenu.Add("Q", new CheckBox("Use Q"));
                    HarassMenu.Add("W", new CheckBox("Use W"));
                    HarassMenu.Add("E.Shield", new CheckBox("Use E on enemy spells"));
                    HarassMenu.Add("E.HealthPercent", new Slider("Use E if HealthPercent <= {0}", 40));
                    HarassMenu.Add("ManaPercent", new Slider("Min. ManaPercent", 25));
                }

                MenuManager.AddSubMenu("Clear");
                {
                    ClearMenu.AddGroupLabel("LaneClear");
                    {
                        ClearMenu.Add("LaneClear.Q", new Slider("Use Q if hit >= {0}", 4, 0, 10));
                        ClearMenu.Add("LaneClear.W", new Slider("Use W if hit >= {0}", 3, 0, 10));
                        ClearMenu.Add("LaneClear.E", new Slider("Use E if hit >= {0}", 6, 0, 10));
                        ClearMenu.Add("LaneClear.ManaPercent", new Slider("Min. ManaPercent", 50));
                    }
                    ClearMenu.AddGroupLabel("LastHit");
                    {
                        ClearMenu.AddStringList("LastHit.Q", "Use Q", new[] { "None", "Smart", "Always" }, 1);
                        ClearMenu.Add("LastHit.ManaPercent", new Slider("Min. ManaPercent", 50));
                    }
                    ClearMenu.AddGroupLabel("JungleClear");
                    {
                        ClearMenu.Add("JungleClear.Q", new CheckBox("Use Q"));
                        ClearMenu.Add("JungleClear.W", new CheckBox("Use W"));
                        ClearMenu.Add("JungleClear.E", new CheckBox("Use E"));
                        ClearMenu.Add("JungleClear.ManaPercent", new Slider("Min. ManaPercent", 20));
                    }
                }

                MenuManager.AddKillStealMenu();
                {
                    KillStealMenu.Add("Q", new CheckBox("Use Q"));
                    KillStealMenu.Add("W", new CheckBox("Use W"));
                    KillStealMenu.Add("E", new CheckBox("Use E"));
                    KillStealMenu.Add("R", new CheckBox("Use R", false));
                }

                MenuManager.AddSubMenu("Automatic");
                {
                    AutomaticMenu.Add("Gapcloser", new CheckBox("Use E on ally gapclosing"));
                    AutomaticMenu.Add("Interrupter", new CheckBox("Use R to interrupt enemy spell"));
                    AutomaticMenu.Add("E.Shield", new CheckBox("Use E on enemy spells"));
                    AutomaticMenu.Add("W.Hit", new Slider("Use W if hit >= {0}", 2, 1, 5));
                    AutomaticMenu.Add("R.Hit", new Slider("Use R if hit >= {0}", 3, 1, 5));
                }

                MenuManager.AddDrawingsMenu();
                {
                    var c = DrawingsMenu.Add("Ball", new CheckBox("Draw ball position"));
                    CircleManager.Circles.Add(new Circle(c, new ColorBGRA(0, 0, 255, 100), () => 120, () => true,
                        () => Ball)
                    { Width = 3 });
                    Q.AddDrawings();
                    W.AddDrawings();
                    E.AddDrawings(false);
                    R.AddDrawings();
                }
                MenuManager.AddSubMenu("Misc");
                {
                    MiscMenu.Add("R.Block", new CheckBox("Block R if will not hit"));
                    if (EntityManager.Heroes.Enemies.Count > 0)
                    {
                        var enemiesAdded = new HashSet<string>();
                        MiscMenu.AddGroupLabel("Don't use R in:");
                        foreach (var enemy in EntityManager.Heroes.Enemies)
                        {
                            if (!enemiesAdded.Contains(enemy.ChampionName))
                            {
                                MiscMenu.Add("BlackList." + enemy.ChampionName, new CheckBox(enemy.ChampionName, false));
                                enemiesAdded.Add(enemy.ChampionName);
                            }
                        }
                    }
                }
                
            }
            catch (Exception e)
            {
                AIO.WriteInConsole(e.ToString());
            }
        }

        private GameObject Ball
        {
            get
            {
                if (_ballObject != null && _ballObject.IsValid && !_ballObject.IsDead)
                {
                    return _ballObject;
                }
                return MyHero;
            }
        }

        protected override void PermaActive()
        {
            _hitR = R.IsReady ? R.ObjectsInRange(R.EnemyHeroes).Count : 0;
            _hitW = W.IsReady ? W.ObjectsInRange(W.EnemyHeroes).Count : 0;
            Range = Q.Range + Q.Width;
            _canShield = AutomaticMenu.CheckBox("E.Shield") || (ModeManager.Combo && ComboMenu.CheckBox("E.Shield")) ||
                         (ModeManager.Harass && HarassMenu.CheckBox("E.Shield"));
            _ballIsMissile = _ballObject != null && _ballObject.IsValid && _ballObject.Type == GameObjectType.MissileClient;
            Target = TargetSelector.GetTarget(UnitManager.ValidEnemyHeroesInRange, DamageType.Magical);
            if (_hitR >= AutomaticMenu.Slider("R.Hit"))
            {
                R.Cast();
            }
            if (_hitW >= AutomaticMenu.Slider("W.Hit"))
            {
                W.Cast();
            }
            if (E.IsReady && AutomaticMenu.CheckBox("E.Shield"))
            {
                if (MissileManager.MissileWillHitMyHero)
                {
                    CastE(MyHero);
                }
            }
            base.PermaActive();
        }

        protected override void KillSteal()
        {
            foreach (var enemy in UnitManager.ValidEnemyHeroesInRange.Where(h => h.HealthPercent <= 40f))
            {
                var result = GetBestCombo(enemy);
                if (KillStealMenu.CheckBox("Q") && (result.Q || Q.IsKillable(enemy)))
                {
                    CastQ(enemy);
                }
                if (KillStealMenu.CheckBox("W") && (result.W || W.IsKillable(enemy)))
                {
                    CastW(enemy);
                }
                if (KillStealMenu.CheckBox("E") && (result.E || E.IsKillable(enemy)))
                {
                    CastE(enemy);
                }
                if (KillStealMenu.CheckBox("R") && (result.R || R.IsKillable(enemy)))
                {
                    CastR(enemy);
                }
                if ((KillStealMenu.CheckBox("Q") || KillStealMenu.CheckBox("E")) &&
                    (
                        (result.Q || Q.IsKillable(enemy))
                        ||
                        (result.W || W.IsKillable(enemy))
                        ||
                        (result.R || R.IsKillable(enemy))
                        )
                    )
                {
                    ThrowBall(enemy);
                }
            }
            base.KillSteal();
        }

        protected override void Combo()
        {
            if (Target != null)
            {
                var bestCombo = GetBestCombo(Target);
                if (MyHero.CountEnemiesInRange(E.Range) >= ComboMenu.Slider("TeamFight"))
                {
                    if (ComboMenu.Slider("Q.Hit") > 0)
                    {
                        var list = Q.EnemyHeroes;
                        if (list.Count >= ComboMenu.Slider("Q.Hit"))
                        {
                            var result = GetBestHitQ(list);
                            if (result.Hits >= ComboMenu.Slider("Q.Hit"))
                            {
                                Q.Cast(result.Position);
                            }
                        }
                    }
                    if (W.IsReady && ComboMenu.Slider("W.Hit") > 0)
                    {
                        var list = W.EnemyHeroes;
                        if (list.Count >= ComboMenu.Slider("W.Hit"))
                        {
                            if (W.ObjectsInRange(list).Count >= ComboMenu.Slider("W.Hit"))
                            {
                                W.Cast();
                            }
                        }
                    }
                    if (R.IsReady && ComboMenu.Slider("R.Hit") > 0)
                    {
                        var list = R.EnemyHeroes;
                        if (list.Count >= ComboMenu.Slider("R.Hit"))
                        {
                            if (R.ObjectsInRange(list).Count >= ComboMenu.Slider("R.Hit"))
                            {
                                R.Cast();
                            }
                        }
                    }
                    CastQr();
                    CastEr();
                }
                else
                {
                    if (R.IsReady && ComboMenu.CheckBox("R.Killable") && bestCombo.R && R.IsKillable(Target))
                    {
                        CastR(Target);
                    }
                }
                if (ComboMenu.CheckBox("Q"))
                {
                    CastQ(Target);
                }
                if (ComboMenu.CheckBox("W"))
                {
                    CastW(Target);
                }
                if (ComboMenu.Slider("E.HealthPercent") >= MyHero.HealthPercent)
                {
                    var enemy = UnitManager.ValidEnemyHeroesInRange.FirstOrDefault(h => h.InAutoAttackRange(MyHero));
                    if (enemy != null)
                    {
                        CastE(MyHero);
                    }
                }
            }
            base.Combo();
        }

        protected override void Harass()
        {
            if (MyHero.ManaPercent >= HarassMenu.Slider("ManaPercent"))
            {
                if (E.IsReady && HarassMenu.CheckBox("E.Shield"))
                {
                    if (MissileManager.MissileWillHitMyHero)
                    {
                        CastE(MyHero);
                    }
                }
                if (Target != null)
                {
                    if (HarassMenu.CheckBox("Q"))
                    {
                        CastQ(Target);
                    }
                    if (HarassMenu.CheckBox("W"))
                    {
                        CastW(Target);
                    }
                    if (HarassMenu.Slider("E.HealthPercent") >= MyHero.HealthPercent)
                    {
                        var enemy =
                            UnitManager.ValidEnemyHeroesInRange.FirstOrDefault(h => h.InAutoAttackRange(MyHero));
                        if (enemy != null)
                        {
                            CastE(MyHero);
                        }
                    }
                }
            }
            base.Harass();
        }

        protected override void LaneClear()
        {
            if (MyHero.ManaPercent >= ClearMenu.Slider("LaneClear.ManaPercent"))
            {
                if (Q.IsReady && ClearMenu.Slider("LaneClear.Q") > 0)
                {
                    var minions = Q.LaneClearMinions;
                    if (minions.Count >= ClearMenu.Slider("LaneClear.Q"))
                    {
                        var result = GetBestHitQ(minions);
                        if (result.Hits >= ClearMenu.Slider("LaneClear.Q"))
                        {
                            Q.Cast(result.Position);
                        }
                    }
                }
                if (W.IsReady && ClearMenu.Slider("LaneClear.W") > 0)
                {
                    var minions = W.LaneClearMinions;
                    if (minions.Count() >= ClearMenu.Slider("LaneClear.W"))
                    {
                        W.Cast();
                    }
                }
                if (E.IsReady && ClearMenu.Slider("LaneClear.E") > 0)
                {
                    var minions = E.LaneClearMinions;
                    if (minions.Count >= ClearMenu.Slider("LaneClear.E"))
                    {
                        var result = GetBestHitE(minions);
                        if (result.Hits >= ClearMenu.Slider("LaneClear.E"))
                        {
                            CastE(result.Target);
                        }
                    }
                }
            }
            base.LaneClear();
        }

        protected override void LastHit()
        {
            if (MyHero.ManaPercent >= ClearMenu.Slider("LastHit.ManaPercent"))
            {
                var tuples = Q.LastHit((LastHitType)ClearMenu.Slider("LastHit.Q"), false);
                var minion = tuples.FirstOrDefault();
                if (minion != null)
                {
                    CastQ(minion);
                }
            }
            base.LastHit();
        }

        protected override void JungleClear()
        {
            if (MyHero.ManaPercent >= ClearMenu.Slider("JungleClear.ManaPercent"))
            {
                if (ClearMenu.CheckBox("JungleClear.Q"))
                {
                    CastQ(Q.JungleClear(false));
                }
                if (ClearMenu.CheckBox("JungleClear.W"))
                {
                    CastW(W.JungleClear(false));
                }
                if (ClearMenu.CheckBox("JungleClear.E"))
                {
                    var minion = E.JungleClear(false);
                    CastE(minion);
                }
            }
            base.JungleClear();
        }

        protected override void Flee()
        {
            if (Ball.InRange(MyHero, W.Range))
            {
                if (W.IsReady)
                {
                    W.Cast();
                }
            }
            else
            {
                if (E.IsReady)
                {
                    CastE(MyHero);
                }
                else if (Q.IsReady && !(_ballObject is MissileClient))
                {
                    Q.Cast(MyHero);
                }
            }
            base.Flee();
        }

        private void CastQ(Obj_AI_Base target, int minHits = 1)
        {
            if (Q.IsReady && target != null)
            {
                var pred = Q.GetPrediction(target);
                if (E.IsReady && MyHero.Mana >= Q.Mana + E.Mana && target.Type == GameObjectType.AIHeroClient &&
                    pred.HitChancePercent <= 5 &&
                    !Q.Source.IsInRange(pred.CastPosition, Q.Range * 1.2f))
                {
                    var pred2 = Q.GetPrediction(target, new CustomSettings { Source = MyHero });
                    if (pred2.HitChancePercent >= Q.HitChancePercent)
                    {
                        CastE(MyHero);
                    }
                }
                var list = new List<Obj_AI_Base>();
                var tempList = Q.EnemyHeroes;
                if (tempList.Contains(target))
                {
                    list = tempList;
                }
                else
                {
                    tempList = Q.EnemyMinions;
                    if (tempList.Contains(target))
                    {
                        list = tempList;
                    }
                    else
                    {
                        tempList = Q.Monsters;
                        if (tempList.Contains(target))
                        {
                            list = tempList;
                        }
                    }
                }
                if (list.Count >= minHits)
                {
                    var res = GetBestHitQ(list, target);
                    if (res.Hits >= minHits)
                    {
                        Q.Cast(res.Position);
                    }
                }
            }
        }

        private void CastW(Obj_AI_Base target)
        {
            if (W.IsReady && target != null && !_ballIsMissile)
            {
                W.Cast(target);
            }
        }

        private void CastE(Obj_AI_Base target)
        {
            if (E.IsReady && target != null)
            {
                if (target.Team == MyHero.Team)
                {
                    MyHero.Spellbook.CastSpell(SpellSlot.E, target);
                }
                else
                {
                    var list = new List<Obj_AI_Base>();
                    var tempList = E.EnemyHeroes;
                    if (tempList.Contains(target))
                    {
                        list = tempList;
                    }
                    else
                    {
                        tempList = E.EnemyMinions;
                        if (tempList.Contains(target))
                        {
                            list = tempList;
                        }
                        else
                        {
                            tempList = E.Monsters;
                            if (tempList.Contains(target))
                            {
                                list = tempList;
                            }
                        }
                    }
                    if (list.Count > 0)
                    {
                        var result = GetBestHitE(list);
                        if (result.Hits > 0)
                        {
                            MyHero.Spellbook.CastSpell(SpellSlot.E, result.Target);
                        }
                    }
                }
            }
        }

        public void CastR(AIHeroClient target)
        {
            if (R.IsReady && target != null && !MiscMenu.CheckBox("BlackList." + target.ChampionName) && !_ballIsMissile)
            {
                R.Cast(target);
            }
        }

        private void ThrowBall(AIHeroClient target)
        {
            var bestAllyNear =
                UnitManager.ValidAllyHeroesInRange.Where(h => h.InRange(target, R.Range * 1.5f) && Ball.GetDistanceSqr(target) > h.GetDistanceSqr(target))
                    .OrderBy(h => h.GetDistanceSqr(target))
                    .FirstOrDefault();
            if (E.IsReady && bestAllyNear != null)
            {
                CastE(bestAllyNear);
            }
            else
            {
                CastQ(target);
            }
        }

        private void CastQr()
        {
            if (Q.IsReady && R.IsReady)
            {
                Q.CachedPredictions.Clear();
                var qWidth = Q.Width;
                var qCastDelay = Q.CastDelay;
                Q.CastDelay = R.CastDelay + qCastDelay;
                Q.Width = R.Range;
                var list = (from enemy in UnitManager.ValidEnemyHeroesInRange
                            select Q.GetPrediction(enemy)
                    into pred
                            where pred.HitChancePercent >= R.HitChancePercent / 2f
                            select pred.CastPosition.To2D()).ToList();
                if (list.Count > 0)
                {
                    var bestCount = -1;
                    var bestPoint = new Vector2(0, 0);
                    foreach (var point in list)
                    {
                        var count = list.Count(v => point.Distance(v, true) <= (R.Range * 1.4f).Pow());
                        if (count > bestCount)
                        {
                            bestCount = count;
                            bestPoint = point;
                        }
                    }
                    if (bestCount >= ComboMenu.Slider("R.Hit"))
                    {
                        Q.Cast(bestPoint.To3DWorld());
                    }
                }
                Q.CachedPredictions.Clear();
                Q.CastDelay = qCastDelay;
                Q.Width = qWidth;
            }
        }

        private void CastEr()
        {
            if (E.IsReady && R.IsReady)
            {
                E.CachedPredictions.Clear();
                var eWidth = E.Width;
                var eCastDelay = E.CastDelay;
                E.CastDelay = R.CastDelay + eCastDelay;
                E.Width = R.Range;
                E.Type = SpellType.Circular;
                var enemyList = (from enemy in UnitManager.ValidEnemyHeroesInRange
                                 select E.GetPrediction(enemy)
                    into pred
                                 where pred.HitChancePercent >= R.HitChancePercent / 2f
                                 select pred.CastPosition).ToList();
                if (enemyList.Count > 0)
                {
                    var allyList =
                        UnitManager.ValidAllyHeroesInRange.Select(ally => E.GetPrediction(ally))
                            .Select(pred => pred.CastPosition);
                    var bestCount = -1;
                    var bestPoint = new Vector3(0, 0, 0);
                    foreach (var ally in allyList)
                    {
                        var count = enemyList.Count(v => ally.IsInRange(v, R.Range * 1.4f));
                        if (count > bestCount)
                        {
                            bestCount = count;
                            bestPoint = ally;
                        }
                    }
                    if (bestCount >= ComboMenu.Slider("R.Hit"))
                    {
                        var allyNear =
                            UnitManager.ValidAllyHeroesInRange.OrderBy(h => h.Distance(bestPoint, true))
                                .FirstOrDefault();
                        if (allyNear != null)
                        {
                            CastE(allyNear);
                        }
                    }
                }
                E.CachedPredictions.Clear();
                E.Type = SpellType.Self;
                E.CastDelay = eCastDelay;
                E.Width = eWidth;
            }
        }

        private BestPositionResult GetBestHitQ(IReadOnlyCollection<Obj_AI_Base> list, Obj_AI_Base target = null)
        {
            var bestResult = new BestPositionResult();
            if (Q.IsReady)
            {
                var checkTarget = target != null;
                foreach (var obj in list)
                {
                    var pred = Q.GetPrediction(obj, new CustomSettings {Width = QAoeWidth});
                    if (pred.HitChancePercent >= Q.HitChancePercent)
                    {
                        var res = Q.ObjectsInLine(list.Where(o => !o.IdEquals(obj)).ToList(), obj);
                        if (!checkTarget || (res.Contains(target) || obj.IdEquals(target)))
                        {
                            var count = res.Count + 1;
                            if (bestResult.Hits < count)
                            {
                                bestResult.Hits = count;
                                bestResult.Position = pred.CastPosition;
                                bestResult.Target = obj;
                                if (bestResult.Hits == list.Count)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return bestResult;
        }

        private BestPositionResult GetBestHitE(List<Obj_AI_Base> objAiBases)
        {
            var bestResult = new BestPositionResult();
            if (E.IsReady)
            {
                foreach (
                    var ally in
                        UnitManager.ValidAllyHeroes.Where(
                            h => E.InRange(h) && h.GetDistanceSqr(Ball) > 0))
                {
                    var pred = E.GetPrediction(ally);
                    if (pred.HitChancePercent >= E.HitChancePercent / 3)
                    {
                        var res = E.ObjectsInLine(objAiBases, ally);
                        var count = res.Count + 1;
                        if (bestResult.Hits < count)
                        {
                            bestResult.Hits = res.Count + 1;
                            bestResult.Position = pred.CastPosition;
                            bestResult.Target = ally;
                            if (bestResult.Hits == objAiBases.Count)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return bestResult;
        }

        public override float GetSpellDamage(SpellSlot slot, Obj_AI_Base target)
        {
            if (target != null)
            {
                var level = slot.GetSpellDataInst().Level;
                switch (slot)
                {
                    case SpellSlot.Q:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            30f * level + 30f + 0.5f * MyHero.FlatMagicDamageMod);
                    case SpellSlot.W:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            45f * level + 25f + 0.7f * MyHero.FlatMagicDamageMod);
                    case SpellSlot.E:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            30f * level + 30f + 0.3f * MyHero.FlatMagicDamageMod);
                    case SpellSlot.R:
                        return MyHero.CalculateDamageOnUnit(target, DamageType.Magical,
                            75f * level + 75f + 0.7f * MyHero.FlatMagicDamageMod);
                }
            }
            return base.GetSpellDamage(slot, target);
        }
    }
}