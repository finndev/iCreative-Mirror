using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace LeeSin
{
    public static class WardManager
    {
        public static float WardRange = 600f;

        private static readonly Item[] ItemWards =
        {
            new Item(ItemId.Ruby_Sightstone, WardRange),
            new Item(ItemId.Sightstone, WardRange),
            new Item(ItemId.Eye_of_the_Watchers, WardRange), new Item(ItemId.Eye_of_the_Oasis, WardRange), new Item(ItemId.Eye_of_the_Equinox, WardRange),
            new Item(ItemId.Trackers_Knife, WardRange), new Item(ItemId.Trackers_Knife_Enchantment_Cinderhulk, WardRange), new Item(ItemId.Trackers_Knife_Enchantment_Devourer, WardRange), new Item(ItemId.Trackers_Knife_Enchantment_Runic_Echoes, WardRange), new Item(ItemId.Trackers_Knife_Enchantment_Sated_Devourer, WardRange), new Item(ItemId.Trackers_Knife_Enchantment_Warrior, WardRange),
            new Item(ItemId.Warding_Totem_Trinket, WardRange),
            new Item(ItemId.Greater_Stealth_Totem_Trinket, WardRange), new Item(ItemId.Explorers_Ward, WardRange),
            new Item(ItemId.Greater_Vision_Totem_Trinket, WardRange), new Item(ItemId.Vision_Ward, WardRange),
        };

        private static List<Obj_AI_Minion> _wardsAvailable;
        private static Vector3 _lastWardJumpVector = Vector3.Zero;
        private static float _lastWardJumpTime;
        public static float LastWardCreated;

        public static bool IsTryingToJump
        {
            get { return _lastWardJumpVector != Vector3.Zero && Game.Time - _lastWardJumpTime < 1.25f; }
        }

        public static bool CanWardJump
        {
            get { return CanCastWard && SpellManager.CanCastW1; }
        }

        public static bool CanCastWard
        {
            get { return Game.Time - _lastWardJumpTime > 1.25f && IsReady; }
        }

        public static bool IsReady
        {
            get { return ItemWards.Any(i => i.IsReady()); }
        }

        private static Item GetItem
        {
            get { return ItemWards.FirstOrDefault(i => i.IsReady()); }
        }

        public static void Init()
        {
            Game.OnUpdate += Game_OnTick;

            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

            GameObject.OnCreate += Obj_Ward_OnCreate;
            GameObject.OnDelete += Obj_Ward_OnDelete;
            _wardsAvailable = new List<Obj_AI_Minion>();
            Core.DelayAction(delegate {
                _wardsAvailable =
                    ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsValid && !m.IsDead && m.IsWard()).ToList();
            }, 500);
            //Shop.OnBuyItem += Shop_OnBuyItem;
        }
        
        private static void Shop_OnBuyItem(AIHeroClient sender, ShopActionEventArgs args)
        {
            Chat.Print(args.Id);
        }

        public static void CastWardTo(Vector3 position)
        {
            if (CanWardJump)
            {
                var endPos = Util.MyHero.Position +
                             (position - Util.MyHero.Position).Normalized() *
                             Math.Min(WardRange, Util.MyHero.Distance(position));
                var ward = GetItem;
                if (ward != null)
                {
                    ward.Cast(endPos);
                    LastWardCreated = Game.Time;
                    _lastWardJumpVector = endPos;
                    _lastWardJumpTime = Game.Time;
                }
            }
        }

        public static void JumpToVector(Vector3 position)
        {
            if (SpellManager.CanCastW1)
            {
                var ward = GetNearestTo(position);
                if (ward != null && position.To2D().Distance(ward.Position.To2D(), true) < Math.Pow(250f, 2))
                {
                    SpellManager.CastW1(ward);
                }
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (IsTryingToJump)
            {
                JumpToVector(_lastWardJumpVector);
            }
            _wardsAvailable.RemoveAll(w => !w.IsValid || w.IsDead);
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name.Equals(SpellSlot.W.GetSpellDataInst().SData.Name) &&
                    args.SData.Name.ToLower().Contains("one"))
                {
                    _lastWardJumpVector = Vector3.Zero;
                }
            }
        }

        private static bool IsWard(this GameObject sender)
        {
            return sender is Obj_AI_Minion && sender.Team == Util.MyHero.Team && sender.Name != null &&
                   (sender.Name.ToLower().Contains("sightward") || sender.Name.ToLower().Contains("visionward"));
        }

        private static void Obj_Ward_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsWard())
            {
                var ward = sender as Obj_AI_Minion;
                _wardsAvailable.Add(ward);
                LastWardCreated = Game.Time;
                if (IsTryingToJump)
                {
                    if (ward != null &&
                        _lastWardJumpVector.To2D().Distance(ward.Position.To2D(), true) < Math.Pow(80, 2))
                    {
                        SpellManager.CastW1(ward);
                    }
                }
            }
        }

        private static void Obj_Ward_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.IsWard())
            {
                var ward = sender as Obj_AI_Minion;
                _wardsAvailable.RemoveAll(m => ward != null && m.NetworkId == ward.NetworkId);
            }
        }

        public static Obj_AI_Minion GetNearestTo(Vector3 position)
        {
            return
                _wardsAvailable.Where(
                    m =>
                        m.IsValid && !m.IsDead &&
                        Util.MyHero.Distance(m, true) <= Math.Pow(SpellManager.W1Range + SpellManager.WExtraRange, 2))
                    .OrderBy(m => m.Distance(position, true))
                    .FirstOrDefault();
        }

        public static Obj_AI_Minion GetFurthestTo(Vector3 position)
        {
            return
                _wardsAvailable.Where(
                    m =>
                        m.IsValid && !m.IsDead &&
                        Util.MyHero.Distance(m, true) <= Math.Pow(SpellManager.W1Range + SpellManager.WExtraRange, 2))
                    .OrderBy(m => m.Distance(position, true))
                    .LastOrDefault();
        }
    }
}