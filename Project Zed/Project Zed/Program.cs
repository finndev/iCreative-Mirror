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

namespace Project_Zed
{
    static class Program
    {
        private static readonly string Author = "iCreative";
        private static readonly string AddonName = "Project Zed";
        private static readonly float RefreshTime = 0.4f;
        private static readonly Dictionary<int, DamageInfo> PredictedDamage = new Dictionary<int, DamageInfo>();
        private static Menu menu;
        private static readonly Dictionary<string, Menu> SubMenu = new Dictionary<string, Menu>();
        private static Spell.Skillshot Q, W, E;
        private static Spell.Targeted Ignite, R;
        private static _Spell _W, _R;
        private static Obj_AI_Minion wFound, rFound;
        private static GameObject IsDeadObject;
        private static readonly Dictionary<int, bool> PassiveUsed = new Dictionary<int, bool>();
        private static readonly List<Obj_AI_Minion> Shadows = new List<Obj_AI_Minion>();
        private const float WRange = 700f;

        public static AIHeroClient MyHero
        {
            get { return Player.Instance; }
        }

        private static Vector3 MousePos
        {
            get { return Game.CursorPos; }
        }

        private static bool IsWaitingShadow
        {
            get
            {
                return Game.Time - _W.LastCastTime < 1.5f && WShadow == null;
            }
        }

        private static bool IsW1
        {
            get { return MyHero.Spellbook.GetSpell(W.Slot).SData.Name.ToLower() != "zedw2"; }
        }

        private static bool IsR1
        {
            get { return MyHero.Spellbook.GetSpell(R.Slot).SData.Name.ToLower() != "zedr2"; }
        }

        private static int TsRange
        {
            get
            {
                if (WShadow != null && RShadow != null)
                {
                    return (int)(Q.Range + Math.Max(MyHero.Distance(RShadow), MyHero.Distance(WShadow)));
                }
                if (IsW1 && W.IsReady() && RShadow != null)
                {
                    return (int)(Q.Range + Math.Max(MyHero.Distance(RShadow), WRange));
                }
                if (WShadow != null)
                {
                    return (int)(Q.Range + MyHero.Distance(WShadow));
                }
                if (IsW1 && W.IsReady())
                {
                    return (int)(Q.Range + WRange);
                }
                return (int)Q.Range;
            }
        }

        private static AIHeroClient TsTarget
        {
            get
            {
                if (!IsR1)
                {
                    foreach (
                        var enemy in
                            EntityManager.Heroes.Enemies.Where(
                                enemy => enemy.IsValidTarget(TsRange) && TargetHaveR(enemy)))
                    {
                        return enemy;
                    }
                }
                var t = TargetSelector.GetTarget(TsRange, DamageType.Physical);
                if (IsDead(t))
                {
                    AIHeroClient t2 = null;
                    foreach (
                        var enemy in
                            EntityManager.Heroes.Enemies.Where(
                                o => o.NetworkId != t.NetworkId && o.IsValidTarget(TsRange)))
                    {
                        if (t2 == null)
                        {
                            t2 = enemy;
                        }
                        else if (TargetSelector.GetPriority(enemy) > TargetSelector.GetPriority(t2))
                        {
                            t2 = enemy;
                        }
                    }
                    if (t2 != null && t2.IsValidTarget(TsRange))
                    {
                        return t2;
                    }
                }
                return t;
            }
        }

        private static Obj_AI_Minion RShadow
        {
            get
            {
                if (IsR1 && R.IsReady() || _R.End == Vector3.Zero)
                {
                    return null;
                }
                if (rFound != null && !rFound.IsDead)
                {
                    return rFound;
                }
                rFound =
                    Shadows.FirstOrDefault(obj => !obj.IsDead && obj.Team == MyHero.Team && _R.End.Distance(obj) < 60);
                return rFound;
            }
        }

        private static Obj_AI_Minion WShadow
        {
            get
            {
                if (IsW1 && W.IsReady() || _W.End == Vector3.Zero)
                {
                    return null;
                }
                if (wFound != null && !wFound.IsDead)
                {
                    return wFound;
                }
                if (RShadow != null)
                {
                    wFound =
                        Shadows.FirstOrDefault(
                            obj =>
                                !obj.IsDead && obj.Team == MyHero.Team && _W.End.Distance(obj) < 100 &&
                                RShadow.Distance(obj) > 0);
                    return wFound;
                }
                wFound =
                    Shadows.Where(obj => !obj.IsDead && obj.Team == MyHero.Team && _W.End.Distance(obj) < WRange)
                        .OrderBy(o => _W.End.Distance(o))
                        .FirstOrDefault();
                return wFound;
            }
        }

        private static float Overkill
        {
            get { return (100f + GetSlider(SubMenu["Misc"], "Overkill")) / 100f; }
        }

        private static bool IsCombo2
        {
            get { return GetKeyBind(SubMenu["Combo"], "Combo2"); }
        }

        private static bool IsHarass2
        {
            get { return GetKeyBind(SubMenu["Harass"], "Harass2"); }
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

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoad;
        }

        public static void AddStringList(this Menu m, string uniqueId, string displayName, string[] values, int defaultValue)
        {
            var mode = m.Add(uniqueId, new Slider(displayName, defaultValue, 0, values.Length - 1));
            mode.DisplayName = displayName + ": " + values[mode.CurrentValue];
            mode.OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
            {
                sender.DisplayName = displayName + ": " + values[args.NewValue];
            };
        }

        private static void OnLoad(EventArgs args)
        {
            if (MyHero.Hero != Champion.Zed)
            {
                return;
            }
            Chat.Print(AddonName + " made by " + Author + " loaded, have fun!.");
            Q = new Spell.Skillshot(SpellSlot.Q, 925, SkillShotType.Linear, 250, 1700, 50)
            {
                AllowedCollisionCount = int.MaxValue
            };
            W = new Spell.Skillshot(SpellSlot.W, 1700, SkillShotType.Linear, 0, 1750, 50)
            {
                AllowedCollisionCount = int.MaxValue
            };
            E = new Spell.Skillshot(SpellSlot.E, 280, SkillShotType.Circular, 0, int.MaxValue, 100)
            {
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Targeted(SpellSlot.R, 625);
            var slot = MyHero.GetSpellSlotFromName("summonerdot");
            if (slot != SpellSlot.Unknown)
            {
                Ignite = new Spell.Targeted(slot, 600);
            }
            _W = new _Spell();
            _R = new _Spell();
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                PassiveUsed.Add(enemy.NetworkId, false);
            }

            menu = MainMenu.AddMenu(AddonName, AddonName + " by " + Author + "v1.10");
            menu.AddLabel(AddonName + " made by " + Author);

            SubMenu["Prediction"] = menu.AddSubMenu("Prediction", "Prediction");
            SubMenu["Prediction"].AddGroupLabel("Q Settings");
            SubMenu["Prediction"].Add("QCombo", new Slider("Combo HitChancePercent", 70, 0, 100));
            SubMenu["Prediction"].Add("QHarass", new Slider("Harass HitChancePercent", 75, 0, 100));

            SubMenu["Combo"] = menu.AddSubMenu("Combo", "Combo");
            Orbwalker.RegisterKeyBind(SubMenu["Combo"].Add("Combo2", new KeyBind("Combo without R", false, KeyBind.BindTypes.HoldActive, 'A')), Orbwalker.ActiveModes.Combo);
            SubMenu["Combo"].AddStringList("Mode", "R Mode", new[] { "Line", "Triangle", "MousePos" }, 0);
            SubMenu["Combo"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["Combo"].Add("W", new CheckBox("Use W", true));
            SubMenu["Combo"].Add("E", new CheckBox("Use E", true));
            SubMenu["Combo"].Add("R", new CheckBox("Use R", true));
            SubMenu["Combo"].Add("Items", new CheckBox("Use Items", true));
            SubMenu["Combo"].Add("SwapDead", new CheckBox("Use W2/R2 if target will die", true));
            SubMenu["Combo"].Add("SwapHP", new Slider("Use W2/R2 if my HealthPercent is less than", 10, 0, 100));
            SubMenu["Combo"].Add("SwapGapclose", new CheckBox("Use W2/R2 to get close to target", true));
            SubMenu["Combo"].Add("Prevent",
                new KeyBind("Don't use spells before R", true, KeyBind.BindTypes.PressToggle, 'L'));
            SubMenu["Combo"].AddGroupLabel("Don't use R on");
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                SubMenu["Combo"].Add(enemy.ChampionName, new CheckBox(enemy.ChampionName, false));
            }

            SubMenu["Harass"] = menu.AddSubMenu("Harass", "Harass");
            Orbwalker.RegisterKeyBind(SubMenu["Harass"].Add("Harass2", new KeyBind("Harass 2 Key", false, KeyBind.BindTypes.HoldActive, 'S')), Orbwalker.ActiveModes.Harass);
            SubMenu["Harass"].Add("Collision", new CheckBox("Check collision with Q", false));
            SubMenu["Harass"].Add("SwapGapclose", new CheckBox("Use W2 if target is killable", true));
            SubMenu["Harass"].AddGroupLabel("Harass 1");
            SubMenu["Harass"].Add("Q", new CheckBox("Use Q on Harass 1", true));
            SubMenu["Harass"].Add("W", new CheckBox("Use W on Harass 1", false));
            SubMenu["Harass"].Add("E", new CheckBox("Use E on Harass 1", true));
            SubMenu["Harass"].Add("Mana", new Slider("Min. Energy Percent:", 20, 0, 100));
            SubMenu["Harass"].AddGroupLabel("Harass 2");
            SubMenu["Harass"].Add("Q2", new CheckBox("Use Q on Harass 2", true));
            SubMenu["Harass"].Add("W2", new CheckBox("Use W on Harass 2", true));
            SubMenu["Harass"].Add("E2", new CheckBox("Use E on Harass 2", true));
            SubMenu["Harass"].Add("Mana2", new Slider("Min. Energy Percent:", 20, 0, 100));

            SubMenu["LaneClear"] = menu.AddSubMenu("LaneClear", "LaneClear");
            SubMenu["LaneClear"].Add("E", new Slider("Use E if Hit >= ", 3, 0, 10));
            SubMenu["LaneClear"].AddGroupLabel("Unkillable minions");
            SubMenu["LaneClear"].Add("Q2", new CheckBox("Use Q", true));
            SubMenu["LaneClear"].Add("Mana", new Slider("Min. Energy Percent:", 50, 0, 100));

            SubMenu["LastHit"] = menu.AddSubMenu("LastHit", "LastHit");
            SubMenu["LastHit"].AddGroupLabel("Unkillable minions");
            SubMenu["LastHit"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["LastHit"].Add("Mana", new Slider("Min. Energy Percent:", 50, 0, 100));

            SubMenu["JungleClear"] = menu.AddSubMenu("JungleClear", "JungleClear");
            SubMenu["JungleClear"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["JungleClear"].Add("W", new CheckBox("Use W", true));
            SubMenu["JungleClear"].Add("E", new CheckBox("Use E", true));
            SubMenu["JungleClear"].Add("Mana", new Slider("Min. Energy Percent:", 20, 0, 100));

            SubMenu["KillSteal"] = menu.AddSubMenu("KillSteal", "KillSteal");
            SubMenu["KillSteal"].Add("Q", new CheckBox("Use Q", true));
            SubMenu["KillSteal"].Add("W", new CheckBox("Use W", true));
            SubMenu["KillSteal"].Add("E", new CheckBox("Use E", true));
            SubMenu["KillSteal"].Add("Ignite", new CheckBox("Use Ignite", true));

            SubMenu["Flee"] = menu.AddSubMenu("Flee", "Flee");
            SubMenu["Flee"].Add("W", new CheckBox("Use W", true));
            SubMenu["Flee"].Add("E", new CheckBox("Use E", true));

            SubMenu["Draw"] = menu.AddSubMenu("Drawing", "Drawing");
            SubMenu["Draw"].Add("W", new CheckBox("Draw W Shadow", true));
            SubMenu["Draw"].Add("R", new CheckBox("Draw R Shadow", true));
            SubMenu["Draw"].Add("IsDead", new CheckBox("Draw text if target will die", true));
            SubMenu["Draw"].Add("Passive", new CheckBox("Draw text when passive is ready", true));

            SubMenu["Misc"] = menu.AddSubMenu("Misc", "Misc");
            SubMenu["Misc"].Add("Overkill", new Slider("Overkill % for damage prediction", 10, 0, 100));
            SubMenu["Misc"].Add("AutoE", new CheckBox("Use Auto E", false));
            SubMenu["Misc"].Add("SwapDead", new CheckBox("Use Auto W2/R2 if target will die", false));
            SubMenu["Misc"].AddSeparator();
            SubMenu["Misc"].Add("EvadeR1", new CheckBox("Use R1 to Evade", true));
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                SubMenu["Misc"].AddGroupLabel(enemy.ChampionName);
                SubMenu["Misc"].Add(enemy.ChampionName + "Q", new CheckBox("Q", false));
                SubMenu["Misc"].Add(enemy.ChampionName + "W", new CheckBox("W", false));
                SubMenu["Misc"].Add(enemy.ChampionName + "E", new CheckBox("E", false));
                SubMenu["Misc"].Add(enemy.ChampionName + "R", new CheckBox("R", false));
            }
            /*
            if (Orbwalker.Menu["Combo"].Cast<KeyBind>().Keys.Item2 == KeyBind.UnboundKey)
            {
                //Orbwalker.Menu["Combo"].Cast<KeyBind>().Keys = new Tuple<uint, uint>(Orbwalker.Menu["Combo"].Cast<KeyBind>().Keys.Item1, (uint)'A');
            }
            if (Orbwalker.Menu["Harass"].Cast<KeyBind>().Keys.Item2 == KeyBind.UnboundKey)
            {
                Orbwalker.Menu["Harass"].Cast<KeyBind>().Keys = new Tuple<uint, uint>(Orbwalker.Menu["Harass"].Cast<KeyBind>().Keys.Item2, (uint)'S');
            }
            */
            Game.OnTick += OnTick;
            GameObject.OnCreate += OnCreateObj;
            GameObject.OnDelete += OnDeleteObj;
            Game.OnWndProc += OnWndProc;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;
            Obj_AI_Base.OnPlayAnimation += Obj_AI_Base_OnPlayAnimation;
        }

        private static void Obj_AI_Base_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            if (!args.Animation.ToLower().Contains("death")) return;
            if (!Shadows.Any()) return;
            Shadows.RemoveAll(o => o.NetworkId == sender.NetworkId);
        }

        private static void OnWndProc(WndEventArgs args)
        {
        }

        private static bool IsWall(Vector3 v)
        {
            var v2 = v.To2D();
            return NavMesh.GetCollisionFlags(v2.X, v2.Y).HasFlag(CollisionFlags.Wall);
        }

        private static void OnTick(EventArgs args)
        {
            if (MyHero.IsDead)
            {
                return;
            }
            KillSteal();
            Swap();
            if (IsHarass && SubMenu["Harass"]["Collision"].Cast<CheckBox>().CurrentValue)
            {
                Q.AllowedCollisionCount = 0;
                W.AllowedCollisionCount = 0;
            }
            else
            {
                Q.AllowedCollisionCount = int.MaxValue;
                W.AllowedCollisionCount = int.MaxValue;
            }
            if (IsCombo)
            {
                Combo();
            }
            else if (IsHarass)
            {
                if (IsHarass2)
                {
                    Harass2();
                }
                else
                {
                    Harass();
                }
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
            if (SubMenu["Misc"]["AutoE"].Cast<CheckBox>().CurrentValue)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    if (enemy.IsValidTarget(TsRange))
                    {
                        CastE(enemy);
                    }
                }
            }
        }

        private static void KillSteal()
        {
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                if (enemy.IsValidTarget(TsRange) && enemy.HealthPercent <= 40f && !IsDead(enemy))
                {
                    var damageI = GetBestCombo(enemy);
                    if (damageI.Damage >= enemy.TotalShieldHealth())
                    {
                        if (SubMenu["KillSteal"]["Q"].Cast<CheckBox>().CurrentValue &&
                            (Damage(enemy, Q.Slot) >= enemy.TotalShieldHealth() || damageI.Q))
                        {
                            CastQ(enemy);
                        }
                        if (SubMenu["KillSteal"]["W"].Cast<CheckBox>().CurrentValue && enemy.HealthPercent < 25f &&
                            (Damage(enemy, W.Slot) >= enemy.TotalShieldHealth() || damageI.W))
                        {
                            CastW(enemy);
                        }
                        if (SubMenu["KillSteal"]["E"].Cast<CheckBox>().CurrentValue &&
                            (Damage(enemy, E.Slot) >= enemy.TotalShieldHealth() || damageI.E))
                        {
                            CastE(enemy);
                        }
                    }
                    if (Ignite != null && SubMenu["KillSteal"]["Ignite"].Cast<CheckBox>().CurrentValue)
                    {
                        if (Ignite.IsReady() &&
                            MyHero.GetSummonerSpellDamage(enemy, DamageLibrary.SummonerSpells.Ignite) >= enemy.TotalShieldHealth())
                        {
                            Ignite.Cast(enemy);
                        }
                    }
                }
            }
        }

        private static void Combo()
        {
            var target = TsTarget;
            if (target.IsValidTarget())
            {
                if (SubMenu["Combo"]["Items"].Cast<CheckBox>().CurrentValue)
                {
                    ItemManager.UseOffensiveItems(target);
                }
                if (SubMenu["Combo"]["R"].Cast<CheckBox>().CurrentValue &&
                    !SubMenu["Combo"][target.ChampionName].Cast<CheckBox>().CurrentValue && !IsCombo2)
                {
                    CastR(target);
                }
                if (SubMenu["Combo"]["Prevent"].Cast<KeyBind>().CurrentValue && R.IsReady() && IsR1 &&
                    !SubMenu["Combo"][target.ChampionName].Cast<CheckBox>().CurrentValue && !IsCombo2)
                {
                    return;
                }
                if (SubMenu["Combo"]["W"].Cast<CheckBox>().CurrentValue && NeedsW(target))
                {
                    CastW(target);
                }
                if (SubMenu["Combo"]["E"].Cast<CheckBox>().CurrentValue)
                {
                    CastE(target);
                }
                if (SubMenu["Combo"]["Q"].Cast<CheckBox>().CurrentValue)
                {
                    CastQ(target);
                }
            }
        }

        private static void Harass()
        {
            var target = TsTarget;
            if (target.IsValidTarget() && MyHero.ManaPercent >= SubMenu["Harass"]["Mana"].Cast<Slider>().CurrentValue)
            {
                if (SubMenu["Harass"]["W"].Cast<CheckBox>().CurrentValue)
                {
                    CastW(target);
                }
                if (SubMenu["Harass"]["E"].Cast<CheckBox>().CurrentValue)
                {
                    CastE(target);
                }
                if (SubMenu["Harass"]["Q"].Cast<CheckBox>().CurrentValue)
                {
                    CastQ(target);
                }
            }
        }

        private static void Harass2()
        {
            var target = TsTarget;
            if (target.IsValidTarget() && MyHero.ManaPercent >= SubMenu["Harass"]["Mana2"].Cast<Slider>().CurrentValue)
            {
                if (SubMenu["Harass"]["W2"].Cast<CheckBox>().CurrentValue)
                {
                    CastW(target);
                }
                if (SubMenu["Harass"]["E2"].Cast<CheckBox>().CurrentValue)
                {
                    CastE(target);
                }
                if (SubMenu["Harass"]["Q2"].Cast<CheckBox>().CurrentValue)
                {
                    CastQ(target);
                }
            }
        }

        private static void Swap()
        {
            var target = TsTarget;
            if (target.IsValidTarget() && !IsDead(target))
            {
                var damageI = GetBestCombo(target);
                if (IsDeadObject != null && SubMenu["Misc"]["SwapDead"].Cast<CheckBox>().CurrentValue)
                {
                    var heroCount = MyHero.CountEnemiesInRange(400);
                    var wCount = (WShadow != null && W.IsReady()) ? WShadow.CountEnemiesInRange(400) : 1000;
                    var rCount = (RShadow != null && R.IsReady()) ? RShadow.CountEnemiesInRange(400) : 1000;
                    var min = Math.Min(rCount, wCount);
                    if (heroCount > min)
                    {
                        if (min == wCount)
                        {
                            MyHero.Spellbook.CastSpell(W.Slot);
                        }
                        else if (min == rCount)
                        {
                            MyHero.Spellbook.CastSpell(R.Slot);
                        }
                    }
                }
                if (IsCombo)
                {
                    if (SubMenu["Combo"]["SwapGapclose"].Cast<CheckBox>().CurrentValue &&
                        MyHero.Distance(target) > E.Range * 1.3f)
                    {
                        var heroDistance = MyHero.Distance(target);
                        var wShadowDistance = (WShadow != null && W.IsReady()) ? target.Distance(WShadow) : 999999f;
                        var rShadowDistance = (RShadow != null && R.IsReady()) ? target.Distance(RShadow) : 999999f;
                        var min = Math.Min(Math.Min(wShadowDistance, rShadowDistance), heroDistance);
                        if (min <= 500 && min < heroDistance)
                        {
                            if (Math.Abs(min - wShadowDistance) < float.Epsilon)
                            {
                                MyHero.Spellbook.CastSpell(W.Slot);
                            }
                            else if (Math.Abs(min - rShadowDistance) < float.Epsilon)
                            {
                                MyHero.Spellbook.CastSpell(R.Slot);
                            }
                        }
                    }
                    if (SubMenu["Combo"]["SwapHP"].Cast<Slider>().CurrentValue >= MyHero.HealthPercent)
                    {
                        if (damageI.Damage <= target.TotalShieldHealth() || MyHero.HealthPercent < target.HealthPercent)
                        {
                            var heroCount = MyHero.CountEnemiesInRange(400);
                            var wCount = (WShadow != null && W.IsReady()) ? WShadow.CountEnemiesInRange(400) : 1000;
                            var rCount = (RShadow != null && R.IsReady()) ? RShadow.CountEnemiesInRange(400) : 1000;
                            var min = Math.Min(rCount, wCount);
                            if (heroCount > min)
                            {
                                if (min == wCount)
                                {
                                    MyHero.Spellbook.CastSpell(W.Slot);
                                }
                                else if (min == rCount)
                                {
                                    MyHero.Spellbook.CastSpell(R.Slot);
                                }
                            }
                        }
                    }
                    if (IsDeadObject != null && SubMenu["Combo"]["SwapDead"].Cast<CheckBox>().CurrentValue)
                    {
                        var heroCount = MyHero.CountEnemiesInRange(400);
                        var wCount = (WShadow != null && W.IsReady()) ? WShadow.CountEnemiesInRange(400) : 1000;
                        var rCount = (RShadow != null && R.IsReady()) ? RShadow.CountEnemiesInRange(400) : 1000;
                        var min = Math.Min(rCount, wCount);
                        if (heroCount > min)
                        {
                            if (min == wCount)
                            {
                                MyHero.Spellbook.CastSpell(W.Slot);
                            }
                            else if (min == rCount)
                            {
                                MyHero.Spellbook.CastSpell(R.Slot);
                            }
                        }
                    }
                }
                else if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    if (SubMenu["Harass"]["SwapGapclose"].Cast<CheckBox>().CurrentValue && W.IsReady() && !IsW1 &&
                        WShadow != null && target.HealthPercent <= 50 && Passive(target, target.Health) > 0f &&
                        damageI.Damage / Overkill >= target.TotalShieldHealth() && MyHero.Distance(target) > WShadow.Distance(target) &&
                        WShadow.Distance(target) < E.Range)
                    {
                        MyHero.Spellbook.CastSpell(W.Slot);
                    }
                }
            }
        }

        private static void Flee()
        {
            if (SubMenu["Flee"]["W"].Cast<CheckBox>().CurrentValue && W.IsReady())
            {
                if (IsW1)
                    W.Cast(MousePos);
                else
                    MyHero.Spellbook.CastSpell(W.Slot);
            }
            if (SubMenu["Flee"]["E"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                foreach (
                    var enemy in
                        EntityManager.Heroes.Enemies.Where(
                            enemy => enemy.IsValidTarget(TsRange) && !enemy.IsValidTarget(E.Range)))
                {
                    CastE(enemy);
                }
            }
        }

        private static void LaneClear()
        {
            var m = SubMenu["LaneClear"];
            if (MyHero.ManaPercent >= GetSlider(m, "Mana"))
            {
                if (GetCheckBox(m, "Q2"))
                {
                    LastHitSpell(Q);
                }
                if (GetSlider(m, "E") > 0 && E.IsReady())
                {
                    if (GetSlider(m, "E") <=
                        EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, MyHero.Position,
                            E.Range).Count())
                    {
                        MyHero.Spellbook.CastSpell(E.Slot);
                    }
                }
            }
        }

        private static void LastHit()
        {
            var m = SubMenu["LastHit"];
            if (MyHero.ManaPercent >= GetSlider(m, "Mana"))
            {
                if (GetCheckBox(m, "Q"))
                {
                    LastHitSpell(Q);
                }
            }
        }

        private static void JungleClear()
        {
            if (MyHero.ManaPercent >= SubMenu["JungleClear"]["Mana"].Cast<Slider>().CurrentValue)
            {
                var jungleminions = EntityManager.MinionsAndMonsters.GetJungleMonsters(MyHero.Position, 1000f);
                if (jungleminions.Any())
                {
                    foreach (
                        var minion in
                            jungleminions.Cast<Obj_AI_Base>()
                                .Where(
                                    minion =>
                                        minion.IsValidTarget() &&
                                        MyHero.ManaPercent >= SubMenu["JungleClear"]["Mana"].Cast<Slider>().CurrentValue)
                        )
                    {
                        if (SubMenu["JungleClear"]["W"].Cast<CheckBox>().CurrentValue)
                        {
                            CastW(minion);
                        }
                        if (SubMenu["JungleClear"]["Q"].Cast<CheckBox>().CurrentValue)
                        {
                            CastQ(minion);
                        }
                        if (SubMenu["JungleClear"]["E"].Cast<CheckBox>().CurrentValue)
                        {
                            CastE(minion);
                        }
                    }
                }
            }
        }

        private static void CastQ(Obj_AI_Base target)
        {
            if (Q.IsReady() && target.IsValidTarget() && !IsWaitingShadow)
            {
                var heroDistance = MyHero.Distance(target);
                var wShadowDistance = WShadow != null ? MyHero.Distance(WShadow) : 999999f;
                var rShadowDistance = RShadow != null ? MyHero.Distance(RShadow) : 999999f;
                var min = Math.Min(Math.Min(rShadowDistance, wShadowDistance), heroDistance);
                if (Math.Abs(min - heroDistance) < float.Epsilon)
                {
                    Q.SourcePosition = MyHero.Position;
                }
                else if (Math.Abs(min - rShadowDistance) < float.Epsilon)
                {
                    Q.SourcePosition = RShadow.Position;
                }
                else
                {
                    Q.SourcePosition = WShadow.Position;
                }
                Q.RangeCheckSource = Q.SourcePosition;
                var pred = Q.GetPrediction(target);
                var hitchance = HitChancePercent(Q.Slot);
                if (pred.HitChancePercent >= hitchance)
                {
                    Q.Cast(pred.CastPosition);
                }
            }
        }

        private static void CastW(Obj_AI_Base target)
        {
            if (W.IsReady() && IsW1 && target.IsValidTarget())
            {
                _W.LastCastTime = Game.Time;
                var r = W.GetPrediction(target);
                if (r.HitChancePercent >= 50 && Game.Time - _W.LastSentTime > 0.25f)
                {
                    _W.LastSentTime = Game.Time;
                    Vector3 wPos = MyHero.Position + (r.CastPosition - MyHero.Position).Normalized() * WRange;
                    if (RShadow != null)
                    {
                        if (GetSlider(SubMenu["Combo"], "Mode") == 0)
                        {
                            wPos = MyHero.Position + (r.CastPosition - RShadow.Position).Normalized() * WRange;
                        }
                        else if (GetSlider(SubMenu["Combo"], "Mode") == 1)
                        {
                            wPos = MyHero.Position + (r.CastPosition - RShadow.Position).Normalized().To2D().Perpendicular().To3D();
                        }
                        else if (GetSlider(SubMenu["Combo"], "Mode") == 2)
                        {
                            wPos = MousePos;
                        }
                    }
                    else
                    {
                        wPos = MyHero.Position + (r.CastPosition - MyHero.Position).Normalized() * WRange;
                    }
                    /**
                    if (IsWall(wPos))
                    {
                        for (float i = Extensions.Distance(MyHero, wPos); i > 0; i = i - 10)
                        {
                            var notwall = MyHero.Position + (wPos - MyHero.Position).Normalized() * i;
                            if (!IsWall(notwall))
                            {
                                wPos = notwall;
                                break;
                            }
                        }
                    }**/
                    W.Cast(wPos);
                }
            }
        }

        private static void CastE(Obj_AI_Base target)
        {
            if (E.IsReady() && target.IsValidTarget() && !IsWaitingShadow)
            {
                var pred = E.GetPrediction(target);
                var heroDistance = MyHero.Distance(pred.CastPosition);
                var wShadowDistance = WShadow != null ? pred.CastPosition.Distance(WShadow) : 999999f;
                var rShadowDistance = RShadow != null ? pred.CastPosition.Distance(RShadow) : 999999f;
                var min = Math.Min(Math.Min(rShadowDistance, wShadowDistance), heroDistance);
                if (min <= E.Range)
                {
                    MyHero.Spellbook.CastSpell(E.Slot);
                }
            }
        }

        private static void CastR(AIHeroClient target)
        {
            if (R.IsReady() && target.IsValidTarget() && IsR1)
            {
                R.Cast(target);
            }
        }

        private static void LastHitSpell(Spell.Skillshot s)
        {
            if (s.IsReady())
            {
                foreach (var minion in from minion in Orbwalker.UnLasthittableMinions let predHealth = Prediction.Health.GetPrediction(minion, (int)(1000f * (s.SourcePosition ?? MyHero.Position).Distance(minion) / s.Speed) + s.CastDelay) where predHealth >= 0 where Damage(minion, s.Slot) >= predHealth select minion)
                {
                    CastQ(minion);
                }
                /*
                var enemyminions =
                    EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, MyHero.Position,
                        s.Range + s.Width, true).Where(o => o.IsValidTarget() && o.Health <= 2.0f * Damage(o, s.Slot));
                if (enemyminions.Any())
                {
                    foreach (var minion in enemyminions)
                    {
                        var CanCalculate = false;
                        if (!Orbwalker.CanAutoAttack)
                        {
                            if (Orbwalker.CanMove && Orbwalker.LastTarget != null &&
                                Orbwalker.LastTarget.NetworkId != minion.NetworkId)
                            {
                                CanCalculate = true;
                            }
                        }
                        else
                        {
                            if (MyHero.GetAutoAttackRange(minion) <= MyHero.Distance(minion))
                            {
                                CanCalculate = true;
                            }
                            else
                            {
                                var speed = MyHero.BasicAttack.MissileSpeed;
                                var time =
                                    (int)
                                        (1000 * MyHero.Distance(minion) / speed + MyHero.AttackCastDelay * 1000);
                                var predHealth = Prediction.Health.GetPrediction(minion, time);
                                if (predHealth <= 0)
                                {
                                    CanCalculate = true;
                                }
                            }
                        }
                        if (CanCalculate)
                        {
                            var dmg = Damage(minion, s.Slot);
                            var source = s.SourcePosition ?? MyHero.Position;
                            var time = (int)(1000 * source.Distance(minion) / s.Speed + s.CastDelay);
                            var predHealth = Prediction.Health.GetPrediction(minion, time);
                            if (time > 0 && Math.Abs(predHealth - minion.Health) < float.Epsilon)
                            {
                                return;
                            }
                            if (dmg > predHealth && predHealth > 0)
                            {
                                CastQ(minion);
                            }
                        }
                    }
                }*/
            }
        }

        private static void OnCreateObj(GameObject sender, EventArgs args)
        {
            if (sender is Obj_AI_Minion)
            {
                if (sender.Team == MyHero.Team && sender.Name.ToLower().Equals("shadow"))
                {
                    var s = sender as Obj_AI_Minion;
                    Shadows.Add(s);
                }
            }
            if (sender is Obj_GeneralParticleEmitter && sender.Name.ToLower().Contains(MyHero.ChampionName.ToLower()))
            {
                if (sender.Name.ToLower().Contains("base_r") && sender.Name.ToLower().Contains("buf_tell") &&
                    TsTarget != null && TsTarget.Distance(sender) < 200)
                {
                    IsDeadObject = sender;
                }
                if (sender.Name.ToLower().Contains("passive") && sender.Name.ToLower().Contains("proc") &&
                    sender.Name.ToLower().Contains("target"))
                {
                    if (Orbwalker.LastTarget != null)
                    {
                        if (Orbwalker.LastTarget.Distance(sender) < 100 &&
                            PassiveUsed.ContainsKey(Orbwalker.LastTarget.NetworkId))
                        {
                            var target = Orbwalker.LastTarget;
                            PassiveUsed[Orbwalker.LastTarget.NetworkId] = true;
                            Core.DelayAction(delegate { PassiveUsed[target.NetworkId] = false; }, 10 * 1000);
                        }
                    }
                }
            }
        }

        private static void OnDeleteObj(GameObject sender, EventArgs args)
        {
            if (sender is Obj_AI_Minion)
            {
                if (sender.Team == MyHero.Team && sender.Name.ToLower().Equals("shadow"))
                {
                    Shadows.RemoveAll(m => m.NetworkId == sender.NetworkId);
                }
            }
            if (sender is Obj_GeneralParticleEmitter && sender.Name.ToLower().Contains(MyHero.ChampionName.ToLower()) &&
                sender.Name.ToLower().Contains("base_r") && sender.Name.ToLower().Contains("buf_tell"))
            {
                IsDeadObject = null;
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (MyHero.IsDead)
            {
                return;
            }
            if (WShadow != null && SubMenu["Draw"]["W"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Color.Blue, 100, WShadow.Position);
            }
            if (RShadow != null && SubMenu["Draw"]["R"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Color.Orange, 100, RShadow.Position);
            }
            if (IsDeadObject != null && SubMenu["Draw"]["IsDead"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawText(Drawing.WorldToScreen(IsDeadObject.Position), System.Drawing.Color.Red, "TARGET DEAD",
                    200);
            }
        }


        private static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.W)
                {
                    if (args.SData.Name.ToLower() == "zedw2")
                    {
                        _W.End = args.End;
                    }
                    else
                    {
                        var pos = args.End;
                        if (IsWall(pos))
                        {
                            for (var i = MyHero.Distance(args.End); i > 0; i = i - 10)
                            {
                                var notwall = MyHero.Position + (pos - MyHero.Position).Normalized() * i;
                                if (IsWall(notwall)) continue;
                                pos = notwall;
                                break;
                            }
                        }
                        _W.End = pos;
                        _W.LastCastTime = Game.Time;
                        Core.DelayAction(ResetW, (int)(MyHero.Distance(_W.End) / W.Speed + 6) * 1000);
                    }
                }
                else if (args.Slot == SpellSlot.R)
                {
                    if (args.SData.Name.ToLower() == "zedr2")
                    {
                        _R.End = args.End;
                    }
                    else
                    {
                        _R.End = MyHero.Position;
                        _R.LastCastTime = Game.Time;
                        Core.DelayAction(ResetR, 8 * 1000);
                    }
                }
            }
            if (sender.Type == MyHero.Type && sender.Team != MyHero.Team &&
                SubMenu["Misc"]["EvadeR1"].Cast<CheckBox>().CurrentValue)
            {
                if (MyHero.Distance(sender) < 1000)
                {
                    var unit = (AIHeroClient)sender;
                    if (SubMenu["Misc"]["EvadeR1"].Cast<CheckBox>().CurrentValue)
                    {
                        if (SpellIsActive(unit, args.SData.Name))
                        {
                            var target =
                                EntityManager.Heroes.Enemies.Where(o => o.IsValidTarget(R.Range))
                                    .OrderByDescending(o => TargetSelector.GetPriority(o))
                                    .First();
                            if (target.IsValidTarget())
                            {
                                CastR(target);
                            }
                        }
                    }
                }
            }
        }

        private static bool SpellIsActive(AIHeroClient unit, string name)
        {
            var slot = "Q";
            if (name.Equals(unit.Spellbook.GetSpell(SpellSlot.Q).SData.Name))
            {
                slot = "Q";
            }
            else if (name.Equals(unit.Spellbook.GetSpell(SpellSlot.W).SData.Name))
            {
                slot = "W";
            }
            else if (name.Equals(unit.Spellbook.GetSpell(SpellSlot.E).SData.Name))
            {
                slot = "E";
            }
            else if (name.Equals(unit.Spellbook.GetSpell(SpellSlot.R).SData.Name))
            {
                slot = "R";
            }
            return SubMenu["Misc"][unit.ChampionName + slot].Cast<CheckBox>().CurrentValue;
        }

        private static void ResetR()
        {
            _R.End = Vector3.Zero;
            rFound = null;
            IsDeadObject = null;
        }

        private static void ResetW()
        {
            _W.End = Vector3.Zero;
            wFound = null;
        }

        private static bool NeedsW(Obj_AI_Base target)
        {
            if (target.IsValidTarget())
            {
                var damageI = GetBestCombo(target);
                if (MyHero.Distance(target) < WRange &&
                    (MyHero.Mana <
                     MyHero.Spellbook.GetSpell(SpellSlot.W).SData.Mana +
                     MyHero.Spellbook.GetSpell(SpellSlot.Q).SData.Mana ||
                     MyHero.Mana <
                     MyHero.Spellbook.GetSpell(SpellSlot.W).SData.Mana +
                     MyHero.Spellbook.GetSpell(SpellSlot.E).SData.Mana))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool TargetHaveR(Obj_AI_Base target)
        {
            return target.HasBuff("zedrtargetmark");
        }

        private static bool IsDead(Obj_AI_Base target)
        {
            if (!IsR1 && target.IsValidTarget() && TargetHaveR(target))
            {
                if (IsDeadObject != null)
                {
                    return IsDeadObject.Distance(target, true) < 200.Pow();
                }
            }
            return false;
        }

        private static float Passive(Obj_AI_Base target, float health)
        {
            var damage = 0f;
            if (100 * health / target.MaxHealth <= 50)
            {
                if (PassiveUsed.ContainsKey(target.NetworkId))
                {
                    if (PassiveUsed[target.NetworkId])
                    {
                        return 0f;
                    }
                }
                return MyHero.CalculateDamageOnUnit(target, DamageType.Physical, (4 + 2 * R.Level) / target.MaxHealth);
            }
            return damage;
        }

        private static float Damage(Obj_AI_Base target, SpellSlot slot)
        {
            if (target.IsValidTarget())
            {
                if (slot == SpellSlot.Q)
                {
                    return MyHero.CalculateDamageOnUnit(target, DamageType.Physical,
                        40f * Q.Level + 35 + 1f * MyHero.TotalAttackDamage);
                }
                if (slot == SpellSlot.W)
                {
                    return 0;
                }
                if (slot == SpellSlot.E)
                {
                    return MyHero.CalculateDamageOnUnit(target, DamageType.Physical,
                        30f * E.Level + 30 + 0.8f * MyHero.TotalAttackDamage);
                }
                if (slot == SpellSlot.R)
                {
                    return MyHero.CalculateDamageOnUnit(target, DamageType.Physical, 1f * MyHero.TotalAttackDamage);
                }
            }
            return MyHero.GetSpellDamage(target, slot);
        }

        private static DamageInfo GetComboDamage(Obj_AI_Base target, bool q, bool w, bool e, bool r)
        {
            var comboDamage = 0f;
            var manaWasted = 0f;
            if (target.IsValidTarget())
            {
                if (w && IsW1 && W.IsReady())
                {
                    manaWasted += MyHero.Spellbook.GetSpell(SpellSlot.W).SData.Mana;
                }
                if (w && e)
                {
                    comboDamage += Damage(target, E.Slot);
                }
                if (w && q)
                {
                    comboDamage += 0.5f * Damage(target, Q.Slot);
                }
                if (r && q)
                {
                    comboDamage += 0.5f * Damage(target, Q.Slot);
                }

                if (q)
                {
                    comboDamage += Damage(target, Q.Slot);
                    manaWasted += MyHero.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;
                }
                if (e)
                {
                    comboDamage += Damage(target, E.Slot);
                    manaWasted += MyHero.Spellbook.GetSpell(SpellSlot.E).SData.Mana;
                }
                if (Ignite != null && Ignite.IsReady())
                {
                    comboDamage += MyHero.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite);
                }
                if (r)
                {
                    comboDamage += Damage(target, R.Slot);
                    comboDamage += comboDamage * (20f + R.Level * 10) / 100f;
                    manaWasted += MyHero.Spellbook.GetSpell(SpellSlot.R).SData.Mana;
                }
                if (TargetHaveR(target))
                {
                    comboDamage += comboDamage * (20f + R.Level * 10) / 100f;
                }
                comboDamage += MyHero.GetAutoAttackDamage(target, true);
            }
            comboDamage += Passive(target, target.TotalShieldHealth() - comboDamage);
            comboDamage = comboDamage * Overkill;
            return new DamageInfo(comboDamage, manaWasted);
        }

        private static DamageInfo GetBestCombo(Obj_AI_Base target)
        {
            var q = Q.IsReady() ? new[] { false, true } : new[] { false };
            var w = ((W.IsReady() && IsW1) || WShadow != null) ? new[] { false, true } : new[] { false };
            var e = E.IsReady() ? new[] { false, true } : new[] { false };
            var r = (R.IsReady() && IsR1) ? new[] { false, true } : new[] { false };
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
                        Q.IsReady(),
                        W.IsReady(),
                        E.IsReady(),
                        R.IsReady()
                    };
                    var bestdmg = 0f;
                    var bestmana = 0f;
                    foreach (var q1 in q)
                    {
                        foreach (var w1 in w)
                        {
                            foreach (var e1 in e)
                            {
                                foreach (var r1 in r)
                                {
                                    damageI2 = GetComboDamage(target, q1, w1, e1, r1);
                                    var d = damageI2.Damage;
                                    var m = damageI2.Mana;
                                    if (MyHero.Mana >= m)
                                    {
                                        if (bestdmg >= target.TotalShieldHealth())
                                        {
                                            if (d >= target.TotalShieldHealth() && (d < bestdmg || m < bestmana))
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

        private static float HitChancePercent(SpellSlot s)
        {
            var slot = s.ToString().Trim();
            return IsHarass
                ? SubMenu["Prediction"][slot + "Harass"].Cast<Slider>().CurrentValue
                : SubMenu["Prediction"][slot + "Combo"].Cast<Slider>().CurrentValue;
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

    public class DamageInfo
    {
        public float Damage;
        public bool E;
        public float Mana;
        public bool Q;
        public bool R;
        public float Time;
        public bool W;

        public DamageInfo(bool Q, bool W, bool E, bool R, float Damage, float Mana, float Time)
        {
            this.Q = Q;
            this.W = W;
            this.E = E;
            this.R = R;
            this.Damage = Damage;
            this.Mana = Mana;
            this.Time = Time;
        }

        public DamageInfo(float Damage, float Mana)
        {
            this.Damage = Damage;
            this.Mana = Mana;
        }
    }

    public class _Spell
    {
        public Vector3 End = Vector3.Zero;
        public float LastCastTime;
        public float LastSentTime;
    }
}