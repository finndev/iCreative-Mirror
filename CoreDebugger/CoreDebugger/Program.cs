using System;
using System.Collections.Generic;
using System.Globalization;
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
        private static void Main()
        {
            Loading.OnLoadingComplete += delegate
            {
                Initialize();
            };
        }
        private static readonly Dictionary<int, int> Counters = new Dictionary<int, int>();

        private static bool EntityManager
        {
            get { return _myMenu["EntityManager"].Cast<CheckBox>().CurrentValue; }
        }
        private static bool MyDamageStats
        {
            get { return _myMenu["MyDamageStats"].Cast<CheckBox>().CurrentValue; }
        }
        private static bool IsValidTarget
        {
            get { return _myMenu["IsValidTarget"].Cast<CheckBox>().CurrentValue; }
        }
        private static bool BuffInstance
        {
            get { return _myMenu["BuffInstance"].Cast<CheckBox>().CurrentValue; }
        }
        private static bool TargetDamageStats
        {
            get { return _myMenu["TargetDamageStats"].Cast<CheckBox>().CurrentValue; }
        }
        private static bool StreamingMode
        {
            get { return _myMenu["StreamingMode"].Cast<CheckBox>().CurrentValue; }
        }

        private static bool HealthPrediction
        {
            get { return _myMenu["HealthPrediction"].Cast<CheckBox>().CurrentValue; }
        }

        private static void Initialize()
        {
            _myMenu = MainMenu.AddMenu("CoreDebugger", "CoreDebugger");
            _myMenu.AddGroupLabel("General");
            _myMenu.Add("MyDamageStats", new CheckBox("My damage stats", false)).OnValueChange += OnOnValueChange;
            _myMenu.Add("TargetDamageStats", new CheckBox("Target damage stats", false)).OnValueChange += OnOnValueChange;
            _myMenu.Add("EntityManager", new CheckBox("EntityManager properties", false)).OnValueChange += OnOnValueChange;
            _myMenu.Add("HealthPrediction", new CheckBox("HealthPrediction properties", false)).OnValueChange += OnOnValueChange;
            _myMenu.Add("IsValidTarget", new CheckBox("IsValidTarget properties", false)).OnValueChange += OnOnValueChange;
            _myMenu.Add("BuffInstance", new CheckBox("BuffInstance properties", false)).OnValueChange += OnOnValueChange;
            _myMenu.Add("StreamingMode", new CheckBox("Streaming Mode", false)).OnValueChange += OnOnValueChange;
            _myMenu["StreamingMode"].Cast<CheckBox>().CurrentValue = false;
            _myMenu.AddGroupLabel("AutoAttack");
            _myMenu.Add("autoAttackDamage", new CheckBox("Print autoattack damage")).OnValueChange += OnOnValueChange;
            var autoAttacking = false;
            AttackableUnit.OnDamage += delegate (AttackableUnit sender, AttackableUnitDamageEventArgs args)
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
            Obj_AI_Base.OnBasicAttack += delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
            {
                if (sender.IsMe)
                {
                    autoAttacking = true;
                }
            };
            Player.OnPostIssueOrder += delegate (Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
            {
                if (sender.IsMe)
                {
                    if (StreamingMode)
                    {
                        Hud.ShowClick(args.Target != null ? ClickType.Attack : ClickType.Move, args.TargetPosition);
                    }
                }
            };
            Drawing.OnEndScene += delegate
            {
                Counters.Clear();
                if (MyDamageStats)
                {
                    DrawText(Player.Instance, "TotalAttackDamage: " + Player.Instance.TotalAttackDamage + ", PercentArmorPenetrationMod: " + Player.Instance.PercentArmorPenetrationMod + ", FlatArmorPenetrationMod: " + Player.Instance.FlatArmorPenetrationMod + ", PercentBonusArmorPenetrationMod: " + Player.Instance.PercentBonusArmorPenetrationMod);
                    DrawText(Player.Instance, "TotalMagicalDamage: " + Player.Instance.TotalMagicalDamage + ", PercentMagicPenetrationMod: " + Player.Instance.PercentMagicPenetrationMod + ", FlatMagicPenetrationMod: " + Player.Instance.FlatMagicPenetrationMod);
                    DrawText(Player.Instance, "Crit: " + Player.Instance.Crit + ", FlatCritChanceMod: " + Player.Instance.FlatCritChanceMod);
                }
                if (IsValidTarget)
                {
                    var targets = ObjectManager.Get<Obj_AI_Base>().Where(i => i.IsValid && i.VisibleOnScreen);
                    foreach (var target in targets)
                    {
                        DrawText(target, "IsValidTarget: " + target.IsValidTarget());
                        DrawText(target, "IsDead: " + target.IsDead);
                        DrawText(target, "IsVisible: " + target.IsVisible);
                        DrawText(target, "IsTargetable: " + target.IsTargetable);
                        DrawText(target, "IsInvulnerable: " + target.IsInvulnerable);
                        DrawText(target, "IsHPBarRendered: " + target.IsHPBarRendered);
                    }
                }
                if (EntityManager)
                {
                    var targets = ObjectManager.Get<Obj_AI_Base>().Where(i => i.IsValidTarget() && i.VisibleOnScreen);
                    foreach (var target in targets)
                    {
                        DrawText(target, "Type: " + target.Type);
                        DrawText(target, "BaseSkinName: " + target.BaseSkinName);
                        DrawText(target, "Team: " + target.Team);
                        DrawText(target, "IsEnemy: " + target.IsEnemy);
                        DrawText(target, "TotalShieldHealth: " + target.TotalShieldHealth());
                        DrawText(target, "MaxHealth: " + target.MaxHealth);
                        if (target is Obj_AI_Minion)
                        {

                            DrawText(target, "IsMinion: " + target.IsMinion);
                            DrawText(target, "IsMonster: " + target.IsMonster);
                        }
                    }
                }
                if (HealthPrediction)
                {
                    var targets = ObjectManager.Get<Obj_AI_Base>().Where(i => i.IsValidTarget() && i.IsAlly && i.VisibleOnScreen && (i is Obj_AI_Minion || i is Obj_AI_Turret));
                    foreach (var target in targets)
                    {
                        DrawText(target, "IsRanged: " + target.IsRanged);
                        DrawText(target, "Health: " + target.Health);
                        DrawText(target, "TotalAttackDamage: " + target.TotalAttackDamage);
                        DrawText(target, "AttackCastDelay: " + target.AttackCastDelay);
                        DrawText(target, "AttackDelay: " + target.AttackDelay);
                        DrawText(target, "MissileSpeed: " + target.BasicAttack.MissileSpeed);
                        if (target is Obj_AI_Minion)
                        {
                            DrawText(target, "PercentDamageToBarracksMinionMod: " + target.PercentDamageToBarracksMinionMod);
                            DrawText(target, "FlatDamageReductionFromBarracksMinionMod: " + target.FlatDamageReductionFromBarracksMinionMod);
                        }
                    }
                    DrawText(Player.Instance, "Ping: " + Game.Ping);
                }
                if (TargetDamageStats)
                {
                    foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(i => i.IsValid && i.VisibleOnScreen))
                    {
                        DrawText(target, "Armor: " + target.Armor + ", SpellBlock: " + target.SpellBlock + ", BaseArmor: " + target.CharData.Armor);
                    }
                }
                if (BuffInstance)
                {
                    foreach (var target in ObjectManager.Get<Obj_AI_Base>().Where(i => i.IsValidTarget() && i.VisibleOnScreen))
                    {
                        foreach (var buff in target.Buffs)
                        {
                            var endTime = Math.Max(0, buff.EndTime - Game.Time);
                            DrawText(target,
                                "Name: " + buff.Name + ", DisplayName: " + buff.DisplayName + ", Count: " + buff.Count + ", SourceName: " + buff.SourceName + ", RemainingTime: " +
                                (endTime > 1000 ? "Infinite" : Convert.ToString(endTime, CultureInfo.InvariantCulture)));
                        }
                    }
                }
            };
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

        private static void DrawText(Obj_AI_Base target, string text)
        {
            if (!Counters.ContainsKey(target.NetworkId))
            {
                Counters.Add(target.NetworkId, 0);
            }
            else
            {
                Counters[target.NetworkId]++;
            }
            var minionBarPosition = new Vector2(target.HPBarXOffset, 50 + target.HPBarYOffset + Counters[target.NetworkId] * 18) + target.Position.WorldToScreen();
            Drawing.DrawText(minionBarPosition, Color.AliceBlue, text, 10);
        }
    }
}
