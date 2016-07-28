using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Utils;
using SharpDX;
using Simple_Vayne.Utility;
using Color = System.Drawing.Color;

namespace Simple_Vayne
{
    /// <summary>
    /// Program class
    /// </summary>  
    public static class Program
    {
        /// <summary>
        /// Champion name constant
        /// </summary>
        private const string ChampName = "Vayne";

        public static AIHeroClient CurrentTarget;

        public static readonly List<ProcessSpellCastCache> CachedAntiGapclosers = new List<ProcessSpellCastCache>();
        public static readonly List<ProcessSpellCastCache> CachedInterruptibleSpells = new List<ProcessSpellCastCache>();
        public static readonly Dictionary<Champion, DangerousSpells.DangerousSpell> CachedDangerousSpells = new Dictionary<Champion, DangerousSpells.DangerousSpell>();
        public static readonly List<DangerousSpells.DangerousSpell> DangerousSpellsActive = new List<DangerousSpells.DangerousSpell>();

        private static Vector3 FlagPos;
        private static int FlagCreateTick;

        public static Text[] InfoText { get; set; }

        public static int DangerLevel;

        public static Spell.Skillshot Flash;

        /// <summary>
        /// Main event
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != ChampName)
            {
                return;
            }

            Config.Initialize();
            SpellManager.Initialize();
            ModeManager.Initialize();

            Drawing.OnDraw += OnDraw;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Game.OnTick += Game_OnTick;

            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Obj_AI_Base.OnBuffGain += ObjAiBaseOnOnBuffGain;
            Obj_AI_Base.OnBuffLose += Obj_AI_Base_OnBuffLose;
            Drawing.OnEndScene += Drawing_OnEndScene;
            PermaShow.Initalize();
            //Evade.Initializer();

            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                if (enemy.Hero == Champion.Rengar)
                {
                    GameObject.OnCreate += Obj_AI_Base_OnCreate;
                }

                if (DangerousSpells.DangerousSpellsDictionary.ContainsKey(enemy.Hero))
                {
                    var spell = DangerousSpells.DangerousSpellsDictionary.FirstOrDefault(x => x.Key == enemy.Hero);

                    CachedDangerousSpells.Add(enemy.Hero, new DangerousSpells.DangerousSpell
                    {
                        DangerLevel = spell.Value.DangerLevel,
                        IfEnemiesNear = spell.Value.IfEnemiesNear,
                        SpellSlot = spell.Value.SpellSlot
                    });
                }
            }
            
            InfoText  = new Text[3];
            InfoText[0] = new Text("", new Font("calibri", Config.Drawings.UltDurationFontSize, FontStyle.Regular));
            InfoText[1] = new Text("", new Font("calibri", Config.Drawings.HPBarFontSize, FontStyle.Regular));
            InfoText[2] = new Text("", new Font("calibri", Config.Drawings.DangerousSpellsFontSize, FontStyle.Regular));

            if (Config.Misc.SkinHack)
            {
                Player.Instance.SetSkin(Player.Instance.BaseSkinName, Config.Misc.SkinId);
            }

            var summonerFlash = Player.Spells.FirstOrDefault(s => s.Name.ToLower().Contains("summonerflash"));
            if (summonerFlash != null)
            {
                Flash = new Spell.Skillshot(summonerFlash.Slot, 425, SkillShotType.Circular);
            }
            Helpers.PrintInfoMessage("Addon loaded !");
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (Config.Drawings.DangerousSpells)
            {
                var count = 0;

                InfoText[2].X = Config.Drawings.DangerousSpellsX;
                InfoText[2].Y = Config.Drawings.DangerousSpellsY;
                InfoText[2].TextValue = "Dangerous spells found : \n";
                InfoText[2].Color = Color.Red;

                foreach (var enemy in DangerousSpellsActive.OrderBy(x => x.DangerLevel))
                {
                    InfoText[2].TextValue += enemy.Champion + " | " + enemy.SpellSlot + "\n";
                    count++;
                }

                if (count == 0)
                {
                    InfoText[2].Color = Color.Green;
                    InfoText[2].TextValue += "NONE !\n";
                }

                InfoText[2].TextValue += "Danger Level : "+DangerLevel;
                InfoText[2].Draw();
            }

            if (Config.Drawings.UltDuration && SpellManager.R.IsLearned)
            {
                var rbuff = Player.Instance.GetBuff("VayneInquisition");

                if (rbuff != null)
                {
                    var percentage = 100 * Math.Max(0, rbuff.EndTime - Game.Time) / Helpers.RDuration[SpellManager.R.Level];

                    var g = Math.Max(0, 255f / 100f * percentage);
                    var r = Math.Max(0, 255 - g);

                    var color = Color.FromArgb((int)r, (int)g, 0);

                    InfoText[0].Color = color;
                    InfoText[0].X = (int)Drawing.WorldToScreen(Player.Instance.Position).X;
                    InfoText[0].Y = (int)Drawing.WorldToScreen(Player.Instance.Position).Y;
                    InfoText[0].TextValue = "\n\nR expiry time : " + Math.Max(0, rbuff.EndTime - Game.Time).ToString("F1") + "s";
                    InfoText[0].Draw();
                }
            }

            if (Config.CondemnMenu.EAfterNextAuto)
            {
                InfoText[0].Color = Color.DeepPink;
                InfoText[0].X = (int)Drawing.WorldToScreen(Player.Instance.Position).X - 80;
                InfoText[0].Y = (int)Drawing.WorldToScreen(Player.Instance.Position).Y + 60;
                InfoText[0].TextValue = "\nE after next auto active !";
                InfoText[0].Draw();
            }

            if (!Config.Drawings.HpBar || !Player.Instance.Spellbook.GetSpell(SpellSlot.W).IsLearned)
                return;

            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        a => !a.IsDead && a.GetSilverStacks() >= 1 && a.IsHPBarRendered && a.IsValidTarget(2000)))
            {
                var stacks = enemy.GetSilverStacks();

                if (stacks > 0)
                {
                    var damage = enemy.CalculateWDamage();
                    var calc = damage / enemy.Health * 100;

                    var r = Math.Max(0, 255f / 100f * calc);
                    var g = Math.Max(0, 255f - r);

                    var color = Color.FromArgb((int)Math.Min(r, 255), (int)Math.Min(g, 255), 0);

                    InfoText[1].Color = color;
                    InfoText[1].X = (int)(enemy.HPBarPosition.X + 140);
                    InfoText[1].Y = (int)enemy.HPBarPosition.Y;
                    InfoText[1].TextValue = "W Damage: " + damage;
                    InfoText[1].Draw();

                    for (var i = 0; i < 3; i++)
                    {
                        Drawing.DrawLine(enemy.HPBarPosition.X + 30 + i * 20, enemy.HPBarPosition.Y - 30,
                            enemy.HPBarPosition.X + 30 + i * 20 + 20,
                            enemy.HPBarPosition.Y - 30, 10, stacks <= i ? Color.DarkGray : Color.MediumVioletRed);
                    }
                }
            }
        }

        private static void ObjAiBaseOnOnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!Config.Misc.NoAAWhileStealth || !sender.IsMe || args.Buff.DisplayName != "VayneInquisitionStealth" || !(Player.Instance.Position.CountEnemiesInRange(1500f) >= Config.Misc.NoAAEnemies))
                return;

            Orbwalker.DisableAttacking = true;

            Core.DelayAction(() => Orbwalker.DisableAttacking = false, Config.Misc.NoAADelay);
        }

        private static void Obj_AI_Base_OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (!Config.Misc.NoAAWhileStealth || !sender.IsMe || args.Buff.DisplayName != "VayneInquisitionStealth")
                return;

            Orbwalker.DisableAttacking = false;
        }

        private static void Obj_AI_Base_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name != "Rengar_LeapSound.troy")
                return;

            var gapcloserMenuInfo = Config.AntiGapcloserMenuValues.FirstOrDefault(x=> x.Champion == "Rengar");

            if (gapcloserMenuInfo == null || !gapcloserMenuInfo.Enabled)
                return;

            foreach (var rengar in EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(1000) && x.ChampionName == "Rengar").Where(rengar => rengar.Distance(Player.Instance.Position) < 1000))
            {
                CachedAntiGapclosers.Add(new ProcessSpellCastCache
                {
                    Sender = rengar,
                    NetworkId = rengar.NetworkId,
                    DangerLevel = gapcloserMenuInfo.DangerLevel,
                    Tick = (int) Game.Time*1000
                });
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
                return;
            

            var enemy = sender as AIHeroClient;

            if (enemy == null)
                return;
            
            if (Config.GapcloserMenu.Enabled && Config.GapcloserMenu.foundgapclosers != 0)
            {
                var menudata = Config.AntiGapcloserMenuValues.FirstOrDefault(x => x.Champion == enemy.ChampionName);

                if (menudata == null)
                    return;

                if (enemy.Hero == Champion.Nidalee || enemy.Hero == Champion.Tristana || enemy.Hero == Champion.JarvanIV)
                {
                    if (enemy.Hero == Champion.JarvanIV && menudata.Enabled &&
                        args.SData.Name.ToLower() == "jarvanivdemacianstandard" &&
                        args.End.Distance(Player.Instance.Position) < 1000)
                    {
                        FlagPos.X = args.End.X;
                        FlagPos.Y = args.End.Y;
                        FlagPos.Z = NavMesh.GetHeightForPosition(args.End.X, args.End.Y);
                        FlagCreateTick = (int) Game.Time*1000;
                    }

                    if (enemy.Hero == Champion.Nidalee && menudata.Enabled && args.SData.Name.ToLower() == "pounce" &&
                        args.End.Distance(Player.Instance.Position) < 350)
                    {
                        CachedAntiGapclosers.Add(new ProcessSpellCastCache
                        {
                            Sender = enemy,
                            NetworkId = enemy.NetworkId,
                            DangerLevel = menudata.DangerLevel,
                            Tick = (int) Game.Time*1000
                        });
                    }
                    /*if (enemy.Hero == Champion.Tristana && menudata.Enabled &&
                        args.SData.Name.ToLower() == "tristanaw" &&
                        args.End.Distance(Player.Instance.Position) < 500)
                    {
                        CachedAntiGapclosers.Add(new ProcessSpellCastCache
                        {
                            Sender = enemy,
                            NetworkId = enemy.NetworkId,
                            DangerLevel = menudata.DangerLevel,
                            Tick = (int) Game.Time*1000
                        });
                    }*/
                    if (enemy.Hero == Champion.JarvanIV && menudata.Enabled &&
                        args.SData.Name.ToLower() == "jarvanivdragonstrike" &&
                        args.End.Distance(Player.Instance.Position) < 1000)
                    {
                        var flagpolygon = new Geometry.Polygon.Circle(FlagPos, 150);
                        var playerpolygon = new Geometry.Polygon.Circle(Player.Instance.Position, 150);

                        for (var i = 900; i > 0; i -= 100)
                        {
                            if (flagpolygon.IsInside(enemy.Position.Extend(args.End, i)) && playerpolygon.IsInside(enemy.ServerPosition.Extend(args.End, i)))
                            {
                                CachedAntiGapclosers.Add(new ProcessSpellCastCache
                                {
                                    Sender = enemy,
                                    NetworkId = enemy.NetworkId,
                                    DangerLevel = menudata.DangerLevel,
                                    Tick = (int) Game.Time*1000
                                });
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (menudata.Enabled && args.Slot == menudata.SpellSlot &&
                        args.End.Distance(Player.Instance.Position) < 350)
                    {
                        CachedAntiGapclosers.Add(new ProcessSpellCastCache
                        {
                            Sender = enemy,
                            NetworkId = enemy.NetworkId,
                            DangerLevel = menudata.DangerLevel,
                            Tick = (int) Game.Time*1000
                        });
                    }
                }
            }
            if (Config.InterrupterMenu.Enabled && Config.InterrupterMenu.foundinterruptiblespells != 0)
            {
                var menudata = Config.InterrupterMenuValues.FirstOrDefault(info => info.Champion == enemy.Hero);
                
                if (menudata == null)
                    return;

                if (menudata.Enabled && args.Slot == menudata.SpellSlot)
                {
                    CachedInterruptibleSpells.Add(new ProcessSpellCastCache
                    {
                        Sender = enemy,
                        NetworkId = enemy.NetworkId,
                        DangerLevel = menudata.DangerLevel,
                        Tick = (int) Game.Time*1000
                    });
                }
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (FlagCreateTick != 0 && FlagCreateTick + 8500 < Game.Time*1000)
            {
                FlagCreateTick = 0;
                FlagPos = Vector3.Zero;
            }
            
            CurrentTarget = TargetSelector.GetTarget(SpellManager.E.Range, DamageType.Physical);

            CachedAntiGapclosers.RemoveAll(x => Game.Time * 1000 > x.Tick + 850);
            CachedInterruptibleSpells.RemoveAll(x => Game.Time * 1000 > x.Tick + 8000);

            if (Config.InterrupterMenu.Enabled)
            {
                var processSpellCastCache = CachedInterruptibleSpells.FirstOrDefault();

                if (processSpellCastCache != null)
                {
                    var enemy = processSpellCastCache.Sender;

                    if (!enemy.Spellbook.IsCastingSpell && !enemy.Spellbook.IsCharging && !enemy.Spellbook.IsChanneling)
                        CachedInterruptibleSpells.Remove(processSpellCastCache);
                }
            }
                foreach (
                    var x in
                        EntityManager.Heroes.Enemies.Where(
                            x =>(x.IsDead || DangerousSpellsActive.Any(index => Game.Time*1000 > index.Tick + 5000 || !x.IsReady(index.SpellSlot) || !(Player.Instance.CountEnemiesInRange(2000) <= index.IfEnemiesNear))) &&
                                DangerousSpellsActive.Exists(key => key.Champion == x.Hero)))
                {
                    foreach (var enemy in DangerousSpellsActive.Where(index => index.Champion == x.Hero))
                    {
                        switch (enemy.DangerLevel)
                        {
                            case DangerousSpells.SpellDangerLevel.Extreme:
                            {
                                DangerLevel -= 75;
                                break;
                            }
                            case DangerousSpells.SpellDangerLevel.High:
                            {
                                DangerLevel -= 50;
                                break;
                            }
                            case DangerousSpells.SpellDangerLevel.Medium:
                            {
                                DangerLevel -= 25;
                                break;
                            }
                            case DangerousSpells.SpellDangerLevel.Low:
                            {
                                DangerLevel -= 12;
                                break;
                            }
                            default:
                            {
                                break;
                            }
                        }
                    }
                    DangerousSpellsActive.RemoveAll(index => index.Champion == x.Hero);
                }

            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        client =>
                            client.IsValidTarget(2000) && CachedDangerousSpells.ContainsKey(client.Hero) &&
                            !DangerousSpellsActive.Exists(x => x.Champion == client.Hero)))
            {
                var dangerousspell = DangerousSpells.DangerousSpellsDictionary.FirstOrDefault(x => x.Key == enemy.Hero);

                if (enemy.IsReady(dangerousspell.Value.SpellSlot) &&
                    Player.Instance.CountEnemiesInRange(2000) <= dangerousspell.Value.IfEnemiesNear)
                {
                    switch (dangerousspell.Value.DangerLevel)
                    {
                        case DangerousSpells.SpellDangerLevel.Extreme:
                        {
                            DangerLevel += 75;
                            break;
                        }
                        case DangerousSpells.SpellDangerLevel.High:
                        {
                            DangerLevel += 50;
                            break;
                        }
                        case DangerousSpells.SpellDangerLevel.Medium:
                        {
                            DangerLevel += 25;
                            break;
                        }
                        case DangerousSpells.SpellDangerLevel.Low:
                        {
                            DangerLevel += 12;
                            break;
                        }
                        default:
                        {
                            break;
                        }
                    }
                    DangerousSpellsActive.Add(new DangerousSpells.DangerousSpell(enemy.Hero,
                        dangerousspell.Value.SpellSlot, dangerousspell.Value.IfEnemiesNear,
                        dangerousspell.Value.DangerLevel, (int) Game.Time*1000));
                }
            }
        }

        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            var unit = target as AIHeroClient;

            if (unit == null)
                return;

            if (Config.CondemnMenu.Execute && Config.CondemnMenu.ExecuteMode == 0 &&  Player.Instance.Position.CountEnemiesInRange(1200) <= Config.CondemnMenu.ExecuteEnemiesNearby &&
                unit.GetSilverStacks() == 1)
            {
                Core.DelayAction(() =>
                {
                    if (unit.IsKillableFromSilverStacks())
                    {
                        Helpers.PrintInfoMessage("Casting condemn on <font color=\"#EB54A2\"><b>" +
                                                 CurrentTarget.BaseSkinName + "</b></font> to execute him.");
                        SpellManager.E.Cast(unit);
                    }
                }, 40 + Game.Ping / 2);
            }
            if (Config.CondemnMenu.EAfterNextAuto && CurrentTarget != null && SpellManager.E.IsReady() &&
                CurrentTarget.IsValidTarget(SpellManager.E.Range))
            {
                SpellManager.E.Cast(CurrentTarget);
                Config.CondemnMenu.EAfterNextAuto = false;
                Helpers.PrintInfoMessage("Casting condemn on <font color=\"#EB54A2\"><b>" +
                                         CurrentTarget.BaseSkinName + "</b></font> as next auto.");

            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (CurrentTarget != null && Config.Drawings.CurrentTarget)
            {
                Circle.Draw(SharpDX.Color.DeepPink, CurrentTarget.BoundingRadius, 2, CurrentTarget.Position);
            }

            /*var unit =
                EntityManager.Heroes.Enemies.Where(index => index.IsValidTarget(1200))
                    .OrderBy(by => by.Distance(Player.Instance)).FirstOrDefault();

            if (unit == null)
            {
                return;
            }
            var enemies = Player.Instance.CountEnemiesInRange(1200);
            var polygons = Helpers.SegmentedAutoattackPolygons();
            var positions = new List<IEnumerable<Vector2>>();

            for (var i = 0; i < 4; i++)
            {
                positions.Add(polygons[i].Points.Where(e => e.ToVector3().ExtendPlayerVector().IsPositionSafe() && e.ToVector3().ExtendPlayerVector().Distance(unit, true) > (enemies <= 2 ? 150 * 150 : 300 * 300)));
            }
            foreach (var points in positions)
            {
                foreach (var point in points)
                {
                    Circle.Draw(SharpDX.Color.Green, 25, 2, point.ToVector3().ExtendPlayerVector());
                }
            }*/
        }
    }
}