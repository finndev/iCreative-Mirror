using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using KoreanAIO.Champions;

namespace KoreanAIO
{
    // ReSharper disable once InconsistentNaming
    public static class AIO
    {
        public static ChampionBase CurrentChampion;

        public static AIHeroClient MyHero
        {
            get { return Player.Instance; }
        }

        public static HashSet<Champion> SupportedChampions = new HashSet<Champion>
        {
            Champion.Diana,
            Champion.Cassiopeia,
            Champion.Orianna,
            Champion.Zed
        };

        public static bool Initialized = false;
        public static List<Action> Initializers = new List<Action>
        {
            Managers.SpellManager.Initialize,
            Managers.UnitManager.Initialize,
            Managers.DrawingsManager.Initialize,
            Managers.MissileManager.Initialize,
            Managers.YasuoWallManager.Initialize,
            Utilities.DamageIndicator.Initialize,
            Managers.ItemManager.Initialize,
            Utilities.FpsBooster.Initialize,
            Managers.CacheManager.Initialize
        };

        public static void WriteInConsole(string message)
        {
            Console.WriteLine("KoreanAIO: " + message);
        }

        public static void WriteInChat(string message)
        {
            Chat.Print("KoreanAIO: " + message);
        }

        private static void Main()
        {
            Loading.OnLoadingComplete += delegate
            {
                /*
                Obj_AI_Base.OnProcessSpellCast +=
                    delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
                    {
                        if (sender.IsMe)
                        {
                            Chat.Print("Process: " + args.SData.TargettingType + " " + args.SData.MissileEffectName + " " + args.SData.MissileBoneName);
                        }
                    };
                Obj_AI_Base.OnSpellCast +=
                    delegate (Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
                    {
                        if (sender.IsMe)
                        {
                            Chat.Print("SpellCast: " + args.SData.TargettingType );
                        }
                    };
                GameObject.OnCreate += delegate (GameObject sender, EventArgs args)
                {
                    var missile = sender as MissileClient;
                    if (missile != null)
                    {
                        var hero = missile.SpellCaster as AIHeroClient;
                        if (hero != null)
                        {
                            Chat.Print("Missile: " + missile.SData.TargettingType + missile.SData.MissileEffectName);
                        }
                    }
                };
                */
                Core.DelayAction(delegate
                {
                    LoadChampion(MyHero.Hero);
                }, 1000);
            };
        }

        public static void LoadChampion(Champion champion)
        {
            try
            {
                if (SupportedChampions.Contains(champion))
                {
                    CurrentChampion = (ChampionBase)Activator.CreateInstance(Assembly.GetExecutingAssembly().GetTypes().First(type => type.Name.Equals(champion.ToString().Replace(" ", ""))));
                    foreach (var action in Initializers)
                    {
                        action();
                    }
                    Initialized = true;
                }
            }
            catch (Exception e)
            {
                WriteInConsole(e.ToString());
            }
        }
    }
}
