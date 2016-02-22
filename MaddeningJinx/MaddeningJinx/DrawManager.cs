using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;

namespace MaddeningJinx
{
    public static class DrawManager
    {
        public static Text JungleStealText;
        public static Text TapKeyText;
        private static readonly Vector2 BaseScreenPoint = new Vector2(100, 50);
        private const float TextScreenSize = 30F;
        private static readonly ColorBGRA WhiteColor = new ColorBGRA(255, 255, 255, 100);
        private static readonly Dictionary<int, Tuple<Text, Text>> DictionaryTexts = new Dictionary<int, Tuple<Text, Text>>();

        public static Menu Menu
        {
            get { return MenuManager.GetSubMenu("Drawings"); }
        }

        public static void Initialize()
        {
            JungleStealText = new Text("", new Font("Arial", TextScreenSize, FontStyle.Bold))
            {
                Color = Color.YellowGreen,
                TextValue = ""
            };
            TapKeyText = new Text("", new Font("Arial", TextScreenSize, FontStyle.Bold))
            {
                Color = Color.White,
                TextValue = ""
            };
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (Util.MyHero.IsDead)
            {
                return;
            }
            if (Menu.CheckBox("Disable"))
            {
                return;
            }
            if (Menu.CheckBox("DamageIndicator"))
            {
                DamageIndicator.Draw();
            }   
            if (Menu.CheckBox("R.Killable"))
            {
                var count = 0;
                foreach (var killable in KillSteal.RKillableBases)
                {
                    if (!DictionaryTexts.ContainsKey(killable.NetworkId))
                    {
                        var positionText = new Text("", new Font("Arial", 23F, FontStyle.Bold))
                        {
                            Color = Color.Red,
                            TextValue = "R Killable",
                        };
                        var killableHero = killable as AIHeroClient;
                        var screenText = new Text("", new Font("Arial", 30F, FontStyle.Bold))
                        {
                            Color = Color.Red,
                            TextValue = (killableHero != null ? killableHero.ChampionName : killable.BaseSkinName) + " is R Killable",
                        };
                        DictionaryTexts[killable.NetworkId] = new Tuple<Text, Text>(positionText, screenText);
                    }
                    if (killable.VisibleOnScreen)
                    {
                        DictionaryTexts[killable.NetworkId].Item1.Position = killable.Position.WorldToScreen();
                        DictionaryTexts[killable.NetworkId].Item1.Draw();
                    }
                    DictionaryTexts[killable.NetworkId].Item2.Position = BaseScreenPoint + new Vector2(0, 50 * count);
                    DictionaryTexts[killable.NetworkId].Item2.Draw();
                    count++;
                }
                if (!string.IsNullOrEmpty(JungleStealText.TextValue))
                {
                    JungleStealText.Position = BaseScreenPoint + new Vector2(0, 50 * count);
                    JungleStealText.Draw();
                    count++;
                }
                if (KillSteal.RKillableBases.Any() && !MenuManager.TapKeyPressed)
                {
                    TapKeyText.TextValue = "Press \'" + MenuManager.TapKey.KeyStrings.Item1 + "\' Tap Key";
                    TapKeyText.Position = BaseScreenPoint + new Vector2(0, 50 * count);
                    TapKeyText.Draw();
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Util.MyHero.IsDead)
            {
                return;
            }
            if (Menu.CheckBox("Disable"))
            {
                return;
            }
            var target = MyTargetSelector.Target;
            if (Menu.CheckBox("Target") && target != null)
            {
                Circle.Draw(SharpDX.Color.Red, 120f, 5, target);
            }
            if (Orbwalker.DrawRange && SpellSlot.Q.IsReady())
            {
                Circle.Draw(WhiteColor,
                    Champion.HasFishBonesActive ? Champion.GetPowPowRange() : Champion.GetFishBonesRange(), Util.MyHero);
            }
            if (Menu.CheckBox("W") && SpellSlot.W.IsReady())
            {
                Circle.Draw(WhiteColor, SpellManager.W.Range, Util.MyHero);
            }
        }
    }
}