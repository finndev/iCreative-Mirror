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
        private static Menu MyMenu;
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
            get { return MyMenu["EntityManager"].Cast<CheckBox>().CurrentValue; }
            set { MyMenu["EntityManager"].Cast<CheckBox>().CurrentValue = value; }
        }
        private static bool MyDamageStats
        {
            get { return MyMenu["MyDamageStats"].Cast<CheckBox>().CurrentValue; }
            set { MyMenu["MyDamageStats"].Cast<CheckBox>().CurrentValue = value; }
        }
        private static bool IsValidTarget
        {
            get { return MyMenu["IsValidTarget"].Cast<CheckBox>().CurrentValue; }
            set { MyMenu["IsValidTarget"].Cast<CheckBox>().CurrentValue = value; }
        }
        private static bool BuffInstance
        {
            get { return MyMenu["BuffInstance"].Cast<CheckBox>().CurrentValue; }
            set { MyMenu["BuffInstance"].Cast<CheckBox>().CurrentValue = value; }
        }
        private static bool TargetDamageStats
        {
            get { return MyMenu["TargetDamageStats"].Cast<CheckBox>().CurrentValue; }
            set { MyMenu["TargetDamageStats"].Cast<CheckBox>().CurrentValue = value; }
        }
        private static bool StreamingMode
        {
            get { return MyMenu["StreamingMode"].Cast<CheckBox>().CurrentValue; }
            set { MyMenu["StreamingMode"].Cast<CheckBox>().CurrentValue = value; }
        }

        private static void Initialize()
        {
            MyMenu = MainMenu.AddMenu("CoreDebugger", "CoreDebugger");
            MyMenu.AddGroupLabel("General");
            MyMenu.Add("MyDamageStats", new CheckBox("My damage stats", false));
            MyMenu.Add("EntityManager", new CheckBox("EntityManager properties", false));
            MyMenu.Add("IsValidTarget", new CheckBox("IsValidTarget properties", false));
            MyMenu.Add("BuffInstance", new CheckBox("BuffInstance properties", false));
            MyMenu.Add("TargetDamageStats", new CheckBox("Target damage stats", false));
            MyMenu.Add("StreamingMode", new CheckBox("Streaming Mode", false));
            MyMenu.AddGroupLabel("AutoAttack");
            MyMenu.Add("autoAttack", new CheckBox("Debug autoattack", false));
            MyMenu.Add("autoAttackDamage", new CheckBox("Print autoattack damage"));
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
                            if (MyMenu["autoAttack"].Cast<CheckBox>().CurrentValue)
                            {
                                Chat.Print("- AutoAttack -");
                                if (MyMenu["autoAttackDamage"].Cast<CheckBox>().CurrentValue)
                                {
                                    Chat.Print("Real Damage: " + args.Damage + ", SDK Damage: " + Player.Instance.GetAutoAttackDamage(baseTarget, true));
                                }
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
                    var minions = ObjectManager.Get<Obj_AI_Minion>().Where(i => i.IsValidTarget() && i.VisibleOnScreen);
                    foreach (var minion in minions)
                    {
                        DrawText(minion, "BaseSkinName: " + minion.BaseSkinName);
                        DrawText(minion, "Team: " + minion.Team);
                        DrawText(minion, "IsEnemy: " + minion.IsEnemy);
                        DrawText(minion, "TotalShieldHealth: " + minion.TotalShieldHealth());
                        DrawText(minion, "MaxHealth: " + minion.MaxHealth);
                        DrawText(minion, "IsMinion: " + minion.IsMinion);
                        DrawText(minion, "IsMonster: " + minion.IsMonster);
                    }
                }
                if (MyDamageStats)
                {
                    DrawText(Player.Instance, "TotalAttackDamage: " + Player.Instance.TotalAttackDamage + ", PercentArmorPenetrationMod: " + Player.Instance.PercentArmorPenetrationMod + ", FlatArmorPenetrationMod: " + Player.Instance.FlatArmorPenetrationMod + ", PercentBonusArmorPenetrationMod: " + Player.Instance.PercentBonusArmorPenetrationMod);
                    DrawText(Player.Instance, "TotalMagicalDamage: " + Player.Instance.TotalMagicalDamage + ", PercentMagicPenetrationMod: " + Player.Instance.PercentMagicPenetrationMod + ", FlatMagicPenetrationMod: " + Player.Instance.FlatMagicPenetrationMod);
                }
                if (TargetDamageStats)
                {
                    var targets = ObjectManager.Get<Obj_AI_Base>().Where(i => i.IsValid && i.VisibleOnScreen);
                    foreach (var target in targets)
                    {
                        var str = "Armor: " + target.Armor + ", BaseArmor: " + target.CharData.Armor + ", SpellBlock: " + target.SpellBlock;
                        if (target is Obj_AI_Minion)
                        {
                            str += ", PercentDamageToBarracksMinionMod: " + target.PercentDamageToBarracksMinionMod + ", FlatDamageReductionFromBarracksMinionMod: " +
                                   target.FlatDamageReductionFromBarracksMinionMod;
                        }
                        DrawText(target, str);
                    }
                }
                if (BuffInstance)
                {
                    var targets = ObjectManager.Get<Obj_AI_Base>().Where(i => i.IsValidTarget() && i.VisibleOnScreen);
                    foreach (var target in targets)
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

        private static void DrawText(Obj_AI_Base minion, string text)
        {
            if (!Counters.ContainsKey(minion.NetworkId))
            {
                Counters.Add(minion.NetworkId, 0);
            }
            else
            {
                Counters[minion.NetworkId]++;
            }
            var minionBarPosition = new Vector2(minion.HPBarXOffset, 50 + minion.HPBarYOffset + Counters[minion.NetworkId] * 18) + minion.Position.WorldToScreen();
            Drawing.DrawText(minionBarPosition, Color.AliceBlue, text, 10);
        }
    }
}
