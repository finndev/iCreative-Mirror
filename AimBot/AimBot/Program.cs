using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Spells;

namespace AimBot
{
    internal static class Program
    {
        // ReSharper disable once InconsistentNaming
        internal static Menu Menu;
        internal static readonly Dictionary<SpellSlot, ComboBox> SlotsComboBox = new Dictionary<SpellSlot, ComboBox>();
        internal static readonly Dictionary<SpellSlot, Slider> SlotsSlider = new Dictionary<SpellSlot, Slider>();
        internal static List<SpellInfo> _spells;
        internal static AIHeroClient MyHero
        {
            get { return Player.Instance; }
        }
        internal static bool Enabled
        {
            get { return Menu["Enable"].Cast<KeyBind>().CurrentValue; }
        }
        static void Main()
        {
            Loading.OnLoadingComplete += delegate
            {
                Load();
            };
        }
        private static void Load()
        {
            _spells = SpellDatabase.GetSpellInfoList(MyHero.BaseSkinName);
            Menu = MainMenu.AddMenu("AimBot", "AimBot 6.4.0 " + MyHero.BaseSkinName);
            Menu.Add("Enable", new KeyBind("Enable / Disable", true, KeyBind.BindTypes.PressToggle, 'K'));
            var drawings = Menu.Add("Draw", new CheckBox("Draw Text"));
            var slots = new HashSet<SpellSlot>();
            foreach (var info in _spells)
            {
                slots.Add(info.Slot);
            }
            foreach (var slot in slots)
            {
                Menu.AddGroupLabel(slot + " Settings");
                SlotsComboBox[slot] = Menu.Add(slot + "ComboBox", new ComboBox("Use", new List<string> { "Never", "In Combo", "Always" }, 2));
                SlotsSlider[slot] = Menu.Add(slot + "HitChancePercent", new Slider("HitChancePercent", 60));
            }
            Game.OnTick += delegate
            {
                OnTick();
            };
            Drawing.OnDraw += delegate
            {
                if (drawings.CurrentValue)
                {
                    Drawing.DrawText(MyHero.Position.WorldToScreen(), Color.White, "AIMbot " + (Enabled ? "ON" : "OFF"), 10);
                }
            };
        }

        private static int _lastChargeTime;
        private static bool IsCharging
        {
            get { return MyHero.Spellbook.IsCharging; }
        }

        public static float HitChancePercent(SpellSlot slot)
        {
            return SlotsSlider[slot].CurrentValue + (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ? 5f : 0f);
        }
        private static void OnTick()
        {
            if (MyHero.IsDead || !Enabled || (!IsCharging && !Orbwalker.CanMove) || Chat.IsOpen || (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) && Orbwalker.ShouldWait))
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

        private const float ViktorRealRange = 525f;
        private static void Cast(SpellSlot slot)
        {
            var spellInstance = MyHero.Spellbook.GetSpell(slot);
            if (spellInstance != null && spellInstance.IsReady)
            {
                var first = _spells.FirstOrDefault(spell => spell.Slot == slot && (string.IsNullOrEmpty(spell.SpellName) || string.Equals(spellInstance.Name, spell.SpellName, StringComparison.CurrentCultureIgnoreCase)));
                if (first != null)
                {
                    var range = first.Range;
                    if (first.Chargeable)
                    {
                        if (IsCharging)
                        {
                            var percentageGrowth = Math.Min(1 / 1000f * (Core.GameTickCount - _lastChargeTime - first.CastRangeGrowthStartTime) / first.CastRangeGrowthDuration, 1);
                            range = (first.CastRangeGrowthMax - first.CastRangeGrowthMin) * percentageGrowth + first.CastRangeGrowthMin;
                        }
                    }
                    var isViktorEType = (MyHero.BaseSkinName == "Viktor" && slot == SpellSlot.E) || (MyHero.BaseSkinName == "Rumble" && slot == SpellSlot.R);
                    if (isViktorEType)
                    {
                        range += ViktorRealRange;
                    }
                    var target = TargetSelector.GetTarget(range + first.Radius * 2, MyHero.TotalAttackDamage > MyHero.TotalMagicalDamage ? DamageType.Physical : DamageType.Magical);
                    if (target != null)
                    {
                        var sourcePosition = MyHero.ServerPosition;
                        if (isViktorEType)
                        {
                            sourcePosition = target.IsInRange(MyHero, ViktorRealRange) ? target.ServerPosition : (MyHero.Position + (target.ServerPosition - MyHero.ServerPosition).Normalized() * ViktorRealRange);
                        }
                        var radius = first.Radius;
                        SkillShotType? skillShotType = null;
                        switch (first.Type)
                        {
                            case SpellType.Circle:
                                skillShotType = SkillShotType.Circular;
                                break;
                            case SpellType.Line:
                                skillShotType = SkillShotType.Linear;
                                break;
                            case SpellType.Cone:
                                skillShotType = SkillShotType.Cone;
                                radius = radius * 2;
                                break;
                            case SpellType.Arc:
                                skillShotType = SkillShotType.Circular;
                                break;
                            case SpellType.MissileLine:
                                skillShotType = SkillShotType.Linear;
                                break;
                            case SpellType.MissileAoe:
                                skillShotType = SkillShotType.Circular;
                                break;
                            case SpellType.Self:
                                skillShotType = SkillShotType.Circular;
                                radius = Prediction.Manager.PredictionSelected == "ICPrediction" ? 0f : radius;
                                break;
                            case SpellType.Ring:
                                break;
                        }
                        if (skillShotType.HasValue)
                        {
                            var collidesWithWall = first.Collisions.Contains(CollisionType.YasuoWall);
                            var predInput = new Prediction.Manager.PredictionInput
                            {
                                Target = target, Range = range, Delay = first.Delay + first.MissileFixedTravelTime, Speed = first.MissileSpeed, Radius = radius, From = sourcePosition
                            };
                            foreach (var col in first.Collisions)
                            {
                                predInput.CollisionTypes.Add(col);
                            }
                            var predResult = Prediction.Position.GetPrediction(predInput);
                            if (predResult.HitChancePercent >= HitChancePercent(slot))
                            {
                                if (first.Chargeable)
                                {
                                    if (IsCharging)
                                    {
                                        Player.Instance.Spellbook.UpdateChargeableSpell(slot, predResult.CastPosition, true);
                                    }
                                    else
                                    {
                                        Player.Instance.Spellbook.CastSpell(slot, Game.CursorPos);
                                    }
                                }
                                else if (!IsCharging)
                                {
                                    if (isViktorEType)
                                    {
                                        var endPosition = sourcePosition + (predResult.CastPosition - sourcePosition).Normalized() * (range - ViktorRealRange);
                                        Player.Instance.Spellbook.CastSpell(slot, endPosition, sourcePosition);
                                    }
                                    else if (!collidesWithWall || !YasuoWallManager.WillHitYasuoWall(predInput.From, predResult.CastPosition))
                                    {
                                        Player.Instance.Spellbook.CastSpell(slot, predResult.CastPosition);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
