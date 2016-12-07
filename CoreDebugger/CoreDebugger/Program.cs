using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;

namespace CoreDebugger
{
    internal static class CoreDebugger
    {
        private static Menu _myMenu;
        private static Menu _subMenu;
        private static readonly Dictionary<int, int> Counters = new Dictionary<int, int>();
        private static readonly Dictionary<int, List<string>> Animations = new Dictionary<int, List<string>>();

        private static bool EntityManager
        {
            get { return _myMenu["EntityManager"].Cast<CheckBox>().CurrentValue; }
        }
        private static bool DamageStats
        {
            get { return _myMenu["DamageStats"].Cast<CheckBox>().CurrentValue; }
        }
        private static bool IsValidTarget
        {
            get { return _myMenu["IsValidTarget"].Cast<CheckBox>().CurrentValue; }
        }
        private static bool BuffInstance
        {
            get { return _myMenu["BuffInstance"].Cast<CheckBox>().CurrentValue; }
        }
        private static bool StreamingMode
        {
            get { return _myMenu["StreamingMode"].Cast<CheckBox>().CurrentValue; }
        }

        private static bool HealthPrediction
        {
            get { return _myMenu["HealthPrediction"].Cast<CheckBox>().CurrentValue; }
        }

        private static bool CheckPrediction
        {
            get { return _myMenu["Prediction"].Cast<CheckBox>().CurrentValue; }
        }

        private static bool CheckSpellbook
        {
            get { return _myMenu["Spellbook"].Cast<CheckBox>().CurrentValue; }
        }

        private static bool CheckMissileClient
        {
            get { return _myMenu["MissileClient"].Cast<CheckBox>().CurrentValue; }
        }

        private static bool CheckOrbwalker
        {
            get { return _myMenu["Orbwalker"].Cast<CheckBox>().CurrentValue; }
        }

        private static bool CheckItems
        {
            get { return _myMenu["Items"].Cast<CheckBox>().CurrentValue; }
        }

        private static bool EnableConsole
        {
            get { return _subMenu["Console"].Cast<CheckBox>().CurrentValue; }
        }

        private static bool OnPlayAnimation
        {
            get { return _myMenu["OnPlayAnimation"].Cast<CheckBox>().CurrentValue; }
        }

        private static bool OnProcessSpell
        {
            get { return _myMenu["OnProcessSpell"].Cast<CheckBox>().CurrentValue; }
        }

        private static void Main()
        {
            Loading.OnLoadingComplete += delegate { Initialize(); };
        }

        public static bool IsVisibleTarget(this AttackableUnit target, bool checkForDead = true)
        {
            if (target == null || !target.IsValid || !target.IsVisible || !target.VisibleOnScreen)
            {
                return false;
            }
            if (checkForDead)
            {
                if (target.IsDead)
                {
                    return false;
                }
            }
            /*
            var baseObject = target as Obj_AI_Base;
            if (baseObject != null && !baseObject.IsHPBarRendered)
            {
                return false;
            }*/
            return true;
        }

        private static void Initialize()
        {
            _myMenu = MainMenu.AddMenu("CoreDebugger", "CoreDebugger");
            _subMenu = _myMenu.AddSubMenu("Console Debugger");
            _subMenu.Add("Console", new CheckBox("Enable Console", false));
            _myMenu.AddGroupLabel("General");
            _myMenu.Add("DamageStats", new CheckBox("Damage stats", false)).OnValueChange += OnOnValueChange;
            _myMenu.Add("EntityManager", new CheckBox("EntityManager properties", false)).OnValueChange += OnOnValueChange;
            _myMenu.Add("HealthPrediction", new CheckBox("HealthPrediction properties", false)).OnValueChange += OnOnValueChange;
            _myMenu.Add("IsValidTarget", new CheckBox("IsValidTarget properties", false)).OnValueChange += OnOnValueChange;
            _myMenu.Add("BuffInstance", new CheckBox("BuffInstance properties", false)).OnValueChange += OnOnValueChange;
            _myMenu.Add("Prediction", new CheckBox("Prediction", false)).OnValueChange += OnOnValueChange;
            _myMenu.Add("StreamingMode", new CheckBox("Streaming Mode", false)).OnValueChange += OnOnValueChange;
            _myMenu.Add("Spellbook", new CheckBox("Spellbook", false)).OnValueChange += OnOnValueChange;
            _myMenu.Add("MissileClient", new CheckBox("MissileClient", false)).OnValueChange += OnOnValueChange;
            _myMenu.Add("Orbwalker", new CheckBox("Orbwalker", false)).OnValueChange += OnOnValueChange;
            _myMenu.Add("Items", new CheckBox("Items", false)).OnValueChange += OnOnValueChange;
            _myMenu.Add("OnPlayAnimation", new CheckBox("OnPlayAnimation", false)).OnValueChange += OnOnValueChange;
            _myMenu.Add("OnProcessSpell", new CheckBox("OnProcessSpell", false)).OnValueChange += OnOnValueChange;
            _myMenu["StreamingMode"].Cast<CheckBox>().CurrentValue = false;
            _myMenu.AddGroupLabel("AutoAttack");
            _myMenu.Add("autoAttackDamage", new CheckBox("Print autoattack damage")).OnValueChange += OnOnValueChange;
            foreach (var value in _myMenu.LinkedValues.Values.Select(i => i as CheckBox).Where(i => i != null))
            {
                value.CurrentValue = false;
            }
            var autoAttacking = false;
            AttackableUnit.OnDamage += delegate(AttackableUnit sender, AttackableUnitDamageEventArgs args)
            {
                if (args.Source.IsMe)
                {
                    var baseTarget = args.Target as Obj_AI_Base;
                    if (baseTarget != null)
                    {
                        if (autoAttacking)
                        {
                            if (_myMenu["autoAttackDamage"].Cast<CheckBox>().CurrentValue)
                            {
                                Chat.Print("Real Damage: " + args.Damage + ", SDK Damage: " + Player.Instance.GetAutoAttackDamage(baseTarget, true));
                            }
                            autoAttacking = false;
                        }
                    }
                }
            };
            Obj_AI_Base.OnPlayAnimation += delegate(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
            {
                if (!Animations.ContainsKey(sender.NetworkId))
                {
                    Animations.Add(sender.NetworkId, new List<string>());
                }
                Animations[sender.NetworkId].Add(args.Animation + " - " + args.AnimationHash.ToString("x8") + " - " + Game.Time);
            };
            GameObject.OnDelete += delegate(GameObject sender, EventArgs args)
            {
                Animations.Remove(sender.NetworkId);
            };
            Obj_AI_Base.OnBasicAttack += delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (sender.IsMe)
                {
                    autoAttacking = true;
                }
            };
            Player.OnPostIssueOrder += delegate(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
            {
                if (sender.IsMe)
                {
                    if (StreamingMode)
                    {
                        Hud.ShowClick(args.Target != null ? ClickType.Attack : ClickType.Move, args.TargetPosition);
                    }
                }
            };
            Obj_AI_Base.OnProcessSpellCast += delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (OnProcessSpell)
                {
                    if (sender.IsMe)
                    {
                        Chat.Print("\n OnProcessSpell");
                        PrintOnProcessSpell(sender, args);
                    }
                }
            };
            Obj_AI_Base.OnBasicAttack += delegate(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (OnProcessSpell)
                {
                    if (sender.IsMe)
                    {
                        Chat.Print("\n OnBasicAttack");
                        PrintOnProcessSpell(sender, args);
                    }
                }
            };
            Drawing.OnEndScene += delegate
            {
                Counters.Clear();
                if (BuffInstance)
                {
                    foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(i => i.IsVisibleTarget()))
                    {
                        foreach (var buff in target.Buffs)
                        {
                            var sum = "";
                            sum += GetValue("IsActive", () => buff.IsActive);
                            sum += GetValue("IsValid", () => buff.IsValid);
                            sum += GetValue("HasBuff", () => target.HasBuff(buff.DisplayName));
                            sum += GetValue("Type", () => buff.Type.ToString());
                            sum += GetValue("Name", () => buff.Name);
                            sum += GetValue("DisplayName", () => buff.DisplayName);
                            sum += GetValue("Count", () => buff.Count);
                            sum += GetValue("GetBuffCount", () => target.GetBuffCount(buff.Name));
                            sum += GetValue("SourceName", () => buff.SourceName);
                            sum += GetValue("Caster", () => buff.Caster.Name);
                            sum += GetValue("CasterBaseSkinName", () => buff.Caster is Obj_AI_Base ? ((Obj_AI_Base)buff.Caster).BaseSkinName : "");
                            sum += GetValue("RemainingTime", () => buff.EndTime - Game.Time);
                            DrawText(target, sum);
                        }
                    }
                }
                if (CheckItems)
                {
                    foreach (var target in ObjectManager.Get<AIHeroClient>().Where(i => i.IsVisibleTarget()))
                    {
                        foreach (var item in target.InventoryItems)
                        {
                            var sum = "";
                            sum += GetValue("Id", () => item.Id.ToString());
                            sum += GetValue("Name", () => item.Name);
                            sum += GetValue("DisplayName", () => item.DisplayName);
                            DrawText(target, sum);
                        }
                    }
                }
                if (DamageStats)
                {
                    foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(i => i.IsVisibleTarget()))
                    {
                        var heroTarget = target as AIHeroClient;
                        DrawText(target, "Source");
                        DrawText(target, "TotalAttackDamage: " + target.TotalAttackDamage + ", TotalMagicalDamage: " + target.TotalMagicalDamage);
                        if (heroTarget != null)
                        {
                            DrawText(heroTarget, "Crit: " + heroTarget.Crit + ", FlatCritChanceMod: " + heroTarget.FlatCritChanceMod);
                            DrawText(heroTarget,
                                "PercentArmorPenetrationMod: " + heroTarget.PercentArmorPenetrationMod + ", FlatArmorPenetrationMod: " +
                                heroTarget.FlatArmorPenetrationMod + ", PercentBonusArmorPenetrationMod: " + heroTarget.PercentBonusArmorPenetrationMod);
                            DrawText(heroTarget,
                                "PercentMagicPenetrationMod: " + heroTarget.PercentMagicPenetrationMod + ", FlatMagicPenetrationMod: " +
                                heroTarget.FlatMagicPenetrationMod);
                        }
                        DrawText(target, "Target");
                        DrawText(target, "Armor: " + target.Armor + ", SpellBlock: " + target.SpellBlock + ", BaseArmor: " + target.CharData.Armor);
                        if (heroTarget != null)
                        {
                            DrawText(heroTarget, "FlatPhysicalReduction: " + heroTarget.FlatPhysicalReduction + ", PercentPhysicalReduction: " + heroTarget.PercentPhysicalReduction);
                            DrawText(heroTarget, "FlatMagicReduction: " + heroTarget.FlatMagicReduction + ", PercentMagicReduction: " + heroTarget.PercentMagicReduction);
                        }
                    }
                }
                if (CheckOrbwalker)
                {
                    DrawText(Player.Instance, GetValue("AttackCastDelay", () => Player.Instance.AttackCastDelay));
                    DrawText(Player.Instance, GetValue("LastAutoAttack", () => Orbwalker.LastAutoAttack));
                    DrawText(Player.Instance, GetValue("CanAttack", () => Player.Instance.CanAttack));
                    DrawText(Player.Instance, GetValue("CanMove", () => Player.Instance.CanMove));
                    DrawText(Player.Instance, GetValue("IsChanneling", () => Player.Instance.Spellbook.IsChanneling));
                    DrawText(Player.Instance, GetValue("IsAutoAttacking", () => Player.Instance.Spellbook.IsAutoAttacking));
                    DrawText(Player.Instance, GetValue("CanAttack", () => Player.Instance.CanAttack));
                    DrawText(Player.Instance, GetValue("IsChanneling", () => Player.Instance.Spellbook.IsChanneling));
                    DrawText(Player.Instance, GetValue("CastEndTime", () => Player.Instance.Spellbook.CastEndTime));
                    DrawText(Player.Instance, GetValue("AnimationTimeLeft", () => Math.Max(0, Player.Instance.Spellbook.CastEndTime - Game.Time)));
                    DrawText(Player.Instance, GetValue("GetAutoAttackRange", () => Player.Instance.GetAutoAttackRange()));
                    DrawText(Player.Instance, GetValue("CanAutoAttack", () => Orbwalker.CanAutoAttack));
                    DrawText(Player.Instance, GetValue("CanMove", () => Orbwalker.CanMove));
                }
                if (OnPlayAnimation)
                {
                    var targets = ObjectManager.Get<Obj_AI_Base>().Where(i => i.IsVisibleTarget(false));
                    foreach (var target in targets)
                    {
                        if (Animations.ContainsKey(target.NetworkId))
                        {
                            var counter = 0;
                            foreach (var animation in Enumerable.Reverse(Animations[target.NetworkId]))
                            {
                                if (counter > 15)
                                {
                                    break;
                                }
                                DrawText(target, GetValue("", () => animation));
                                counter++;
                            }
                        }
                    }
                }
                if (IsValidTarget)
                {
                    var targets = ObjectManager.Get<Obj_AI_Base>().Where(i => i.IsValid && i.VisibleOnScreen);
                    foreach (var target in targets)
                    {
                        DrawText(target, GetValue("IsValidTarget", () => target.IsValidTarget()));
                        DrawText(target, GetValue("IsDead", () => target.IsDead));
                        DrawText(target, GetValue("IsVisible", () => target.IsVisible));
                        DrawText(target, GetValue("IsTargetable", () => target.IsTargetable));
                        DrawText(target, GetValue("IsInvulnerable", () => target.IsInvulnerable));
                        DrawText(target, GetValue("IsHPBarRendered", () => target.IsHPBarRendered));
                    }
                }
                if (EntityManager)
                {
                    var targets = ObjectManager.Get<Obj_AI_Base>().Where(i => i.IsVisibleTarget());
                    foreach (var target in targets)
                    {
                        DrawText(target, GetValue("NetworkId", () => target.NetworkId));
                        DrawText(target, GetValue("Type", () => target.Type.ToString()));
                        DrawText(target, GetValue("Name", () => target.Name));
                        DrawText(target, GetValue("BaseSkinName", () => target.BaseSkinName));
                        DrawText(target, GetValue("Team", () => target.Team.ToString()));
                        DrawText(target, GetValue("IsEnemy", () => target.IsEnemy));
                        DrawText(target, GetValue("TotalShieldHealth", () => target.TotalShieldHealth()));
                        DrawText(target, GetValue("HPRegenRate", () => target.HPRegenRate));
                        DrawText(target, GetValue("MaxHealth", () => target.MaxHealth));
                        if (target is Obj_AI_Minion)
                        {
                            DrawText(target, GetValue("IsMinion", () => target.IsMinion));
                            DrawText(target, GetValue("IsMonster", () => target.IsMonster));
                        }
                    }
                }
                if (HealthPrediction)
                {
                    var targets = ObjectManager.Get<Obj_AI_Base>().Where(i => i.IsVisibleTarget() && i.IsAlly && (i is Obj_AI_Minion || i is Obj_AI_Turret || i.IsMe));
                    foreach (var target in targets)
                    {
                        DrawText(target, GetValue("IsRanged", () => target.IsRanged));
                        DrawText(target, GetValue("Health", () => target.Health));
                        DrawText(target, GetValue("TotalAttackDamage", () => target.TotalAttackDamage));
                        DrawText(target, GetValue("AttackCastDelay", () => target.AttackCastDelay));
                        DrawText(target, GetValue("AttackDelay", () => target.AttackDelay));
                        DrawText(target, GetValue("MissileSpeed", () => target.BasicAttack.MissileSpeed));
                        if (target is Obj_AI_Minion)
                        {
                            DrawText(target, GetValue("PercentDamageToBarracksMinionMod", () => target.PercentDamageToBarracksMinionMod));
                            DrawText(target, GetValue("FlatDamageReductionFromBarracksMinionMod", () => target.FlatDamageReductionFromBarracksMinionMod));
                        }
                    }
                    DrawText(Player.Instance, "Ping: " + Game.Ping);
                }
                if (CheckPrediction)
                {
                    var targets = ObjectManager.Get<Obj_AI_Base>().Where(i => i.IsVisibleTarget());
                    foreach (var target in targets)
                    {
                        DrawText(target, GetValue("IsMoving", () => target.IsMoving));
                        DrawText(target, GetValue("PathLength", () => target.Path.Length));
                        DrawText(target, GetValue("BoundingRadius", () => target.BoundingRadius));
                        DrawText(target, GetValue("MovementBlockedDebuffDuration", () => Math.Max(0f, target.GetMovementBlockedDebuffDuration())));
                        DrawText(target, GetValue("CastEndTimeLeft", () => Math.Max(0f, target.Spellbook.CastEndTime - Game.Time)));
                        DrawText(target, GetValue("CastTimeLeft", () => Math.Max(0f, target.Spellbook.CastTime - Game.Time)));
                        DrawText(target, GetValue("IsChanneling", () => target.Spellbook.IsChanneling));
                    }
                }
                if (CheckSpellbook)
                {
                    var targets = ObjectManager.Get<Obj_AI_Base>().Where(i => i.IsVisibleTarget());
                    foreach (var target in targets)
                    {
                        foreach (var spell in target.Spellbook.Spells)
                        {
                            if (spell != null && !spell.Name.Contains("Unknown") && !spell.Name.Contains("BaseSpell"))
                            {
                                DrawText(target,
                                    "Name: " + spell.Name + ", Slot: " + spell.Slot + ", State: " + spell.State + ", ToggleState: " + spell.ToggleState + ", Level: " + spell.Level + ", Cooldown: " +
                                    spell.Cooldown + ", CooldownExpires: " + Math.Max(0f, spell.CooldownExpires - Game.Time) + ", Ammo: " + spell.Ammo + ", CastRange: " + spell.SData.CastRange +
                                    ", CastRangeDisplayOverride: " + spell.SData.CastRangeDisplayOverride);
                            }
                        }
                    }
                }
                if (CheckMissileClient)
                {
                    var missiles = ObjectManager.Get<MissileClient>().Where(i => i.IsValid && !i.IsDead);
                    foreach (var missile in missiles)
                    {
                        DrawText(missile, GetValue("Slot", () => missile.Slot.ToString()));
                        DrawText(missile, GetValue("SpellCaster", delegate
                        {
                            var caster = missile.SpellCaster;
                            return caster != null ? caster.BaseSkinName : "";
                        }));
                        DrawText(missile, GetValue("Target", delegate
                        {
                            var target = missile.Target as Obj_AI_Base;
                            return target != null ? target.BaseSkinName : "";
                        }));
                        var heroCaster = missile.SpellCaster as AIHeroClient;
                        if (heroCaster != null)
                        {
                            EloBuddy.SDK.Rendering.Circle.Draw(SharpDX.Color.Blue, 65f, 1f, missile.StartPosition);
                            EloBuddy.SDK.Rendering.Circle.Draw(SharpDX.Color.Blue, 65f, 1f, missile.EndPosition);
                        }
                        DrawText(missile, GetValue("Name", () => missile.SData.Name));
                        DrawText(missile, GetValue("StartPosition", () => missile.StartPosition));
                        DrawText(missile, GetValue("EndPosition", () => missile.EndPosition));
                        DrawText(missile, GetValue("MissileFixedTravelTime", () => missile.SData.MissileFixedTravelTime));
                        DrawText(missile, GetValue("MissileSpeed", () => missile.SData.MissileSpeed));
                        DrawText(missile, GetValue("LineWidth", () => missile.SData.LineWidth));
                    }
                }
            };
        }

        private static void PrintOnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.SData != null)
            {
                try
                {
                    var str =   "Sender: " + sender.BaseSkinName
                                + ", Name: " + args.SData.Name
                                + ", Slot: " + args.Slot
                                + ", LineWidth: " + args.SData.LineWidth
                                + (args.SData.CastRadius > 0 ? ", CastRadius: " + args.SData.CastRadius : "")
                                + (args.SData.CastRadiusSecondary > 0 ? ", CastRadiusSecondary: " + args.SData.CastRadiusSecondary : "")
                                + ", CastConeAngle: " + args.SData.CastConeAngle
                                + (args.SData.CastRange > 0 ? ", CastRange: " + args.SData.CastRange : "")
                                + (args.SData.CastRangeDisplayOverride > 0 ? ", CastRangeDisplayOverride: " + args.SData.CastRangeDisplayOverride : "")
                                + ", MissileSpeed: " + args.SData.MissileSpeed
                                + ", CastConeDistance: " + args.SData.CastConeDistance
                                + ", CastTime: " + args.SData.CastTime
                                + (args.Target != null ? ", Target: " + args.Target.Name : "");
                    Chat.Print(str);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        private static void OnOnValueChange(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
        {
            if (args.NewValue)
            {
                foreach (var value in _myMenu.LinkedValues.Values.Select(i => i as CheckBox).Where(i => i != null))
                {
                    if (sender.SerializationId != value.SerializationId)
                    {
                        value.CurrentValue = false;
                    }
                }
            }
        }

        private static string GetValue(string name, Func<string> parameterFunc)
        {
            if (EnableConsole)
            {
                Console.WriteLine("Checking " + name);
            }
            var parameter = parameterFunc();
            return name + ": " + parameter + ", ";
        }
        private static string GetValue(string name, Func<bool> parameterFunc)
        {
            if (EnableConsole)
            {
                Console.WriteLine("Checking " + name);
            }
            var parameter = parameterFunc();
            return name + ": " + parameter + ", ";
        }
        private static string GetValue(string name, Func<int> parameterFunc)
        {
            if (EnableConsole)
            {
                Console.WriteLine("Checking " + name);
            }
            var parameter = parameterFunc();
            return name + ": " + parameter + ", ";
        }

        private static string GetValue(string name, Func<float> parameterFunc)
        {
            if (EnableConsole)
            {
                Console.WriteLine("Checking " + name);
            }
            var parameter = parameterFunc();
            return name + ": " + parameter + ", ";
        }


        private static string GetValue(string name, Func<Vector3> parameterFunc)
        {
            if (EnableConsole)
            {
                Console.WriteLine("Checking " + name);
            }
            var parameter = parameterFunc();
            return name + ": " + parameter + ", ";
        }

        private static void DrawText(GameObject target, string value)
        {
            if (!Counters.ContainsKey(target.NetworkId))
            {
                Counters.Add(target.NetworkId, 0);
            }
            else
            {
                Counters[target.NetworkId]++;
            }
            var targetPosition = new Vector2(0, 30 + Counters[target.NetworkId] * 18) + target.Position.WorldToScreen();
            Drawing.DrawText(targetPosition, Color.AliceBlue, value, 10);
        }
    }
}
