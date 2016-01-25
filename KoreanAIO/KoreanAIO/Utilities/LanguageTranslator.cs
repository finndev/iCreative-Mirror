using System.Collections.Generic;
using EloBuddy.SDK.Menu.Values;
using KoreanAIO.Managers;

namespace KoreanAIO.Utilities
{
    public enum Language
    {
        English,
        Spanish,
        Korean,
        Chinese,
        French,
        German,
        Italian,
        Portuguese
    }

    public static class LanguageTranslator
    {
        private static readonly Dictionary<Language, Dictionary<string, string>> Translations =
            new Dictionary<Language, Dictionary<string, string>>();

        static LanguageTranslator()
        {
            Translations[Language.English] = new Dictionary<string, string>
            {
                {"Language", "Language"},
                {"English", "English" /* Spanish = Espanol (for ex)*/},
                {"Keys", "Keys"},
                {"Prediction", "Prediction"},
                {"Combo", "Combo"},
                {"Harass", "Harass"},
                {"LaneClear", "Laneclear"},
                {"LastHit", "LastHit"},
                {"JungleClear", "JungleClear"},
                {"KillSteal", "KillSteal"},
                {"Automatic", "Automatic"},
                {"Evader", "Evader"},
                {"Drawings", "Drawings"},
                {"Flee", "Flee"},
                {"Misc", "Misc"},
                {"Never", "Never"},
                {"Smartly", "Smartly"},
                {"Always", "Always"},
                {"UseQ", "Use Q"},
                {"Q.Hit", "Use Q if hit is greater than {0}"},
                {"UseW", "Use W"},
                {"W.Hit", "Use W if hit is greater than {0}"},
                {"UseE", "Use E"},
                {"E.Hit", "Use E if hit is greater than {0}"},
                {"UseR", "Use R"},
                {"R.Hit", "Use R if hit is greater than {0}"},
                {"R.Prevent", "Don't use spells before R"},
                {"R.BlackList", "Don't use R on:"},
                {"Items", "Use Items"},
                {"SwapDead", "Use W2/R2 if target will die"},
                {"SwapGapclose", "Use W2/R2 to get close to target"},
                {"SwapHP", "Use W2/R2 if the % of Health is less than {0}"},
                {"MinimumManaPercent", "Minimum Mana Percent"}
            };
            Translations[Language.Spanish] = new Dictionary<string, string>
            {
                {"Language", "Idioma"},
                {"Spanish", "Espanol"},
                {"Keys", "Teclas"},
                {"Prediction", "Prediccion"},
                {"Combo", "Combo"},
                {"Harass", "Harass"},
                {"LaneClear", "LaneClear"},
                {"LastHit", "LastHit"},
                {"JungleClear", "JungleClear"},
                {"KillSteal", "KillSteal"},
                {"Automatic", "Automatico"},
                {"Evader", "Evadir"},
                {"Drawings", "Dibujos"},
                {"Flee", "Escapar"},
                {"Misc", "Miscelaneo"},
                {"Never", "Nunca"},
                {"Smartly", "Inteligentemente"},
                {"Always", "Siempre"},
                {"UseQ", "Usar Q"},
                {"Q.Hit", "Usar Q si golpea a mas de {0}"},
                {"UseW", "Usar W"},
                {"W.Hit", "Usar W si golpea a mas de {0}"},
                {"UseE", "Usar E"},
                {"E.Hit", "Usar E si golpea a mas de {0}"},
                {"UseR", "Usar R"},
                {"R.Hit", "Usar R si golpea a mas de {0}"},
                {"R.Prevent", "No usar habilidades antes de la R"},
                {"R.BlackList", "No usar la R en:"},
                {"Items", "Usar los items"},
                {"SwapDead", "Usar W2/R2 si el target va a morir"},
                {"SwapGapclose", "Usar W2/R2 para acercarse al target"},
                {"SwapHP", "Usar W2/R2 si el % de vida es menor a {0}"},
                {"MinimumManaPercent", "Minimo de porcentaje de mana"}
            };
        }

        public static Language CurrentLanguage
        {
            get { return (Language) MenuManager.Menu["Language"].Cast<Slider>().CurrentValue; }
        }

        public static void Initialize()
        {
            MenuManager.Translate(Language.English, (Language) MenuManager.Menu["Language"].Cast<Slider>().CurrentValue);
        }

        public static string GetTranslationFromId(Language language, string name)
        {
            if (Translations.ContainsKey(language) && Translations[language].ContainsKey(name))
            {
                return Translations[language][name];
            }
            if (Translations.ContainsKey(Language.English))
            {
                if (Translations[Language.English].ContainsKey(name))
                {
                    return Translations[Language.English][name];
                }
            }
            return name;
        }

        public static string GetTranslationFromDisplayName(Language from, Language to, string displayName)
        {
            if (from != to)
            {
                if (Translations.ContainsKey(from))
                {
                    foreach (var pair in Translations[from])
                    {
                        if (pair.Value == displayName)
                        {
                            if (Translations.ContainsKey(to))
                            {
                                if (Translations[to].ContainsKey(pair.Key))
                                {
                                    return Translations[to][pair.Key];
                                }
                            }
                        }
                    }
                }
                if (Translations.ContainsKey(Language.English))
                {
                    foreach (var pair in Translations[Language.English])
                    {
                        if (pair.Value == displayName)
                        {
                            if (Translations.ContainsKey(to))
                            {
                                if (Translations[to].ContainsKey(pair.Key))
                                {
                                    return Translations[to][pair.Key];
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}