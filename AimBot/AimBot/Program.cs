using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Spells;

namespace AimBot
{
    internal class Program
    {
        private static void Main()
        {
            Loading.OnLoadingComplete += delegate
            {
                Initialize();
            };
        }

        internal static Menu _menu;
        internal static AIHeroClient MyHero
        {
            get { return Player.Instance; }
        }
        internal static bool Enabled
        {
            get { return _menu["Enable"].Cast<KeyBind>().CurrentValue; }
        }
        internal static readonly Dictionary<SpellSlot, ComboBox> SlotsComboBox = new Dictionary<SpellSlot, ComboBox>();
        internal static readonly Dictionary<SpellSlot, Slider> SlotsSlider = new Dictionary<SpellSlot, Slider>();
        internal static List<SpellInfo> _spells;
        private static void Initialize()
        {
            _spells = SpellDatabase.GetSpellInfoList(MyHero.BaseSkinName);
            if (_spells.Count > 0)
            {
                YasuoWallManager.Initialize();
                _menu = MainMenu.AddMenu("AimBot", "AimBot 6.4.0 " + MyHero.BaseSkinName);
                _menu.Add("Enable", new KeyBind("Enable / Disable", true, KeyBind.BindTypes.PressToggle, 'K'));
                var drawings = _menu.Add("Draw", new CheckBox("Draw Text"));
                var slots = new HashSet<SpellSlot>();
                foreach (var info in _spells)
                {
                    slots.Add(info.Slot);
                }
                foreach (var slot in slots)
                {
                    _menu.AddGroupLabel(slot + " Settings");
                    SlotsComboBox[slot] = _menu.Add(slot + "ComboBox", new ComboBox("Use", new List<string> { "Never", "In Combo", "Always" }, 2));
                    SlotsSlider[slot] = _menu.Add(slot + "HitChancePercent", new Slider("HitChancePercent", 60));
                }
                Game.OnTick += Game_OnTick;
                Drawing.OnDraw += delegate
                {
                    if (drawings.CurrentValue)
                    {
                        Drawing.DrawText(MyHero.Position.WorldToScreen(), Color.White, "AIMbot " + (Enabled ? "ON" : "OFF"), 10);
                    }
                };
            }
        }

        private static int _lastChargeTime;
        private static void Game_OnTick(EventArgs args)
        {
            if (MyHero.IsDead || !Enabled || (!IsCharging && !Orbwalker.CanMove))
            {
                return;
            }
            if (_lastChargeTime == 0 && IsCharging)
            {
                _lastChargeTime = Core.GameTickCount;
            }
            else if (_lastChargeTime > 0 && !IsCharging)
            {
                _lastChargeTime = 0;
            }
            foreach (var slot in SlotsComboBox.Where(i => Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ? i.Value.CurrentValue >= 1 : i.Value.CurrentValue == 2).Select(i => i.Key))
            {
                Cast(slot);
            }
        }

        private static bool IsCharging
        {
            get { return MyHero.Spellbook.IsCharging; }
        }

        private static void Cast(SpellSlot slot)
        {
            var first = _spells.FirstOrDefault(spell => spell.Slot == slot && (string.IsNullOrEmpty(spell.SpellName) || string.Equals(MyHero.Spellbook.GetSpell(slot).Name, spell.SpellName, StringComparison.CurrentCultureIgnoreCase)));
            if (first != null)
            {
                var allowedCollisionCount = int.MaxValue;
                if (first.Collisions.Contains(CollisionType.AiHeroClient))
                {
                    allowedCollisionCount = 0;
                }
                else if (first.Collisions.Contains(CollisionType.ObjAiMinion))
                {
                    allowedCollisionCount = -1;
                }
                var collidesWithWall = first.Collisions.Contains(CollisionType.YasuoWall);
                SpellBase spell = null;
                switch (first.Type)
                {
                    case EloBuddy.SDK.Spells.SpellType.Self:
                        spell = new SpellBase(slot, SpellType.Self, first.Range) { Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, AllowedCollisionCount = allowedCollisionCount, CollidesWithYasuoWall = collidesWithWall };
                        break;
                    case EloBuddy.SDK.Spells.SpellType.Circle:
                        spell = new SpellBase(slot, SpellType.Circular, first.Range) { Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, AllowedCollisionCount = allowedCollisionCount, CollidesWithYasuoWall = collidesWithWall, Width = first.Radius };
                        break;
                    case EloBuddy.SDK.Spells.SpellType.Line:
                        spell = new SpellBase(slot, SpellType.Linear, first.Range) { Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, AllowedCollisionCount = allowedCollisionCount, CollidesWithYasuoWall = collidesWithWall, Width = first.Radius };
                        break;
                    case EloBuddy.SDK.Spells.SpellType.Cone:
                        spell = new SpellBase(slot, SpellType.Cone, first.Range) { Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, AllowedCollisionCount = allowedCollisionCount, CollidesWithYasuoWall = collidesWithWall, Width = first.Radius };
                        break;
                    case EloBuddy.SDK.Spells.SpellType.Ring:
                        break;
                    case EloBuddy.SDK.Spells.SpellType.MissileLine:
                        spell = new SpellBase(slot, SpellType.Linear, first.Range) { Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, AllowedCollisionCount = allowedCollisionCount, CollidesWithYasuoWall = collidesWithWall, Width = first.Radius };
                        break;
                    case EloBuddy.SDK.Spells.SpellType.MissileAoe:
                        spell = new SpellBase(slot, SpellType.Circular, first.Range) { Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, AllowedCollisionCount = allowedCollisionCount, CollidesWithYasuoWall = collidesWithWall, Width = first.Radius };
                        break;
                }
                if (spell != null)
                {
                    if (first.Chargeable)
                    {
                        if (IsCharging)
                        {
                            var percentageGrowth = Math.Min(1 / 1000f * (Core.GameTickCount - _lastChargeTime - first.CastRangeGrowthStartTime) / first.CastRangeGrowthDuration, 1);
                            spell.Range = (first.CastRangeGrowthMax - first.CastRangeGrowthMin) * percentageGrowth + first.CastRangeGrowthMin;
                            spell.ReleaseCast();
                        }
                        else
                        {
                            spell.StartCast();
                        }
                    }
                    else if (!IsCharging)
                    {
                        spell.Cast();
                    }
                }
            }
        }
    }
}
