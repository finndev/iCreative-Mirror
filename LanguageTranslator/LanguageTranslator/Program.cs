using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using EloBuddy;
using EloBuddy.Sandbox;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Newtonsoft.Json;
using Version = System.Version;

namespace LanguageTranslator
{
    internal static class Program
    {
        private const string VersionUrl = "https://raw.githubusercontent.com/jachicao/EloBuddy/master/LanguageTranslator/LanguageTranslator/Properties/AssemblyInfo.cs";
        private const string JsonUrl = "https://raw.githubusercontent.com/jachicao/EloBuddy/master/LanguageTranslator/LanguageTranslator/Translations.json";
        private const string VersionRegex = @"\[assembly\: AssemblyVersion\(""(\d+\.\d+\.\d+\.\d+)""\)\]";
        private static string _jsonPath;
        private static string _programDirectory;

        private static Menu _menu;
        private static bool _loaded;
        private static bool _ready;
        private static Dictionary<string, Dictionary<Language, Dictionary<int, string>>> Translations = new Dictionary<string, Dictionary<Language, Dictionary<int, string>>>();
        private static readonly Dictionary<string, Language> CulturesToLanguage = new Dictionary<string, Language>
        {
            { "en-US", Language.English },
            { "en-GB", Language.English },
            { "es-ES", Language.Spanish },
            { "fr-FR", Language.French },
            { "de-DE", Language.German },
            { "it-IT", Language.Italian },
            { "pt-BR", Language.Portuguese },
            { "pt-PT", Language.Portuguese },
            { "pl-PL", Language.Polish },
            { "tr-TR", Language.Turkish },
            { "zh-CHS", Language.Chinese },
            { "zh-CHT", Language.ChineseTraditional },
            { "ko-KR", Language.Korean },
            { "ro-RO", Language.Romanian },
            { "vi-VN", Language.Vietnamese },
        };
        private static bool _jsonPathExists;
        private static Language CurrentCulture
        {
            get { return CulturesToLanguage.ContainsKey(CultureInfo.InstalledUICulture.ToString()) ? CulturesToLanguage[CultureInfo.InstalledUICulture.ToString()] : Language.English; }
        }

        private static void Main()
        {
            Loading.OnLoadingComplete += delegate
            {
                var time = Game.Time;
                Game.OnTick += delegate
                {
                    if (Game.Time - time >= 2 && time > 0)
                    {
                        time = 0;
                        _programDirectory = Path.Combine(SandboxConfig.DataDirectory, "LanguageTranslator");
                        if (!Directory.Exists(_programDirectory))
                        {
                            Directory.CreateDirectory(_programDirectory);
                        }
                        _jsonPath = Path.Combine(_programDirectory, "Translations.json");
                        _jsonPathExists = File.Exists(_jsonPath);
                        if (!_jsonPathExists)
                        {
                            File.Create(_jsonPath).Close();
                            DownloadNewJson();
                        }
                        else
                        {
                            var jsonConvert = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<Language, Dictionary<int, string>>>>(File.ReadAllText(_jsonPath));
                            if (jsonConvert != null)
                            {
                                Translations = jsonConvert;
                            }
                            DownloadNewJson();
                            //var webClient = new WebClient { Encoding = Encoding.UTF8 };
                            //webClient.DownloadStringCompleted += VersionCompleted;
                            //webClient.DownloadStringAsync(new Uri(VersionUrl, UriKind.Absolute));
                        }
                    }
                    if (_ready)
                    {
                        OnLoad();
                    }
                };
            };
        }

        private static void VersionCompleted(object sender, DownloadStringCompletedEventArgs args)
        {
            if (args.Cancelled || args.Error != null)
            {
                Console.WriteLine("Failed to download internet version.");
                _ready = true;
                return;
            }
            var match = Regex.Match(args.Result, VersionRegex);
            var internetVersion = Version.Parse(match.Groups[1].Value);
            var localVersion = Assembly.GetExecutingAssembly().GetName().Version;
            if (internetVersion > localVersion)
            {
                DownloadNewJson();
            }
            else
            {
                _ready = true;
            }
        }

        private static void DownloadNewJson()
        {
            var webClient = new WebClient { Encoding = Encoding.UTF8 };
            webClient.DownloadStringCompleted += JsonDownloaded;
            webClient.DownloadStringAsync(new Uri(JsonUrl, UriKind.Absolute));
        }

        private static void JsonDownloaded(object sender, DownloadStringCompletedEventArgs args)
        {
            if (args.Cancelled || args.Error != null)
            {
                Console.WriteLine("Failed to download json file.");
                if (_jsonPathExists)
                {
                    _ready = true;
                }
                return;
            }
            File.WriteAllText(_jsonPath, args.Result);
            var jsonConvert = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<Language, Dictionary<int, string>>>>(args.Result);
            if (jsonConvert != null)
            {
                Translations = jsonConvert;
            }
            _ready = true;
        }

        private static void OnLoad()
        {
            if (!_loaded)
            {
                _ready = false;
                _loaded = true;
                _menu = MainMenu.AddMenu("LanguageTranslator", "LanguageTranslator");
                var languagesAvailable = Enum.GetValues(typeof (Language)).Cast<Language>().ToArray().Select(i => i.ToString());
                var currentLanguage = (int) CurrentCulture;
                var comboBox = _menu.Add("Language", new ComboBox("Language:", languagesAvailable, currentLanguage));
                comboBox.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args) { Translate((Language) args.OldValue, (Language) args.NewValue); };
                var saveCheckBox = _menu.Add("Save", new CheckBox("Save Current Addons Names", false));
                saveCheckBox.OnValueChange += delegate(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs args)
                {
                    if (sender.CurrentValue)
                    {
                        Save();
                        sender.CurrentValue = false;
                    }
                };
                Translate(Language.English, (Language) comboBox.CurrentValue);
            }
        }

        private static void Translate(Language from, Language to)
        {
            foreach (var pair in MainMenu.MenuInstances)
            {
                foreach (var menu in pair.Value)
                {
                    var addonId = menu.Parent == null ? menu.DisplayName : menu.Parent.DisplayName;
                    menu.DisplayName = GetTranslationFromDisplayName(addonId, from, to, menu.DisplayName);
                    foreach (var pair2 in menu.LinkedValues)
                    {
                        pair2.Value.DisplayName = GetTranslationFromDisplayName(addonId, from, to, pair2.Value.DisplayName);
                        var comboBox = pair2.Value as ComboBox;
                        if (comboBox != null)
                        {
                            foreach (var val in comboBox.Overlay.Children)
                            {
                                val.TextValue = GetTranslationFromDisplayName(addonId, from, to, val.TextValue);
                            }
                        }
                    }
                    foreach (var subMenu in menu.SubMenus)
                    {
                        subMenu.DisplayName = GetTranslationFromDisplayName(addonId, from, to, subMenu.DisplayName);
                    }
                    foreach (var subMenu in menu.SubMenus)
                    {
                        foreach (var pair2 in subMenu.LinkedValues)
                        {
                            pair2.Value.DisplayName = GetTranslationFromDisplayName(addonId, from, to, pair2.Value.DisplayName);
                            var comboBox = pair2.Value as ComboBox;
                            if (comboBox != null)
                            {
                                foreach (var val in comboBox.Overlay.Children)
                                {
                                    val.TextValue = GetTranslationFromDisplayName(addonId, from, to, val.TextValue);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string GetTranslationFromDisplayName(string addonId, Language from, Language to, string displayName)
        {
            if (Translations.ContainsKey(addonId))
            {
                var dictionary = Translations[addonId];
                to = dictionary.ContainsKey(to) ? to : Language.English;
                if (dictionary.ContainsKey(to))
                {
                    if (dictionary.ContainsKey(from))
                    {
                        foreach (var pair in dictionary[from])
                        {
                            if (pair.Value == displayName)
                            {
                                if (dictionary[to].ContainsKey(pair.Key))
                                {
                                    return dictionary[to][pair.Key];
                                }
                            }
                        }
                    }
                    if (dictionary.ContainsKey(Language.English))
                    {
                        foreach (var pair in dictionary[Language.English])
                        {
                            if (pair.Value == displayName)
                            {
                                if (dictionary[to].ContainsKey(pair.Key))
                                {
                                    return dictionary[to][pair.Key];
                                }
                            }
                        }
                    }
                }
            }
            return displayName;
        }

        private static void Save()
        {
            Translate((Language)_menu["Language"].Cast<ComboBox>().CurrentValue, Language.English);
            foreach (var pair in MainMenu.MenuInstances)
            {
                foreach (var menu in pair.Value)
                {
                    var counter = 0;
                    var addonId = menu.Parent == null ? menu.DisplayName : menu.Parent.DisplayName;
                    if (!Translations.ContainsKey(addonId))
                    {
                        Translations.Add(addonId, new Dictionary<Language, Dictionary<int, string>>());
                    }
                    if (Translations[addonId].Count == 0)
                    {
                        Translations[addonId].Add(Language.English, new Dictionary<int, string>());
                    }
                    else
                    {
                        counter = Translations[addonId][Language.English].Count;
                    }
                    var dictionary = Translations[addonId][Language.English];
                    var value = menu.DisplayName;
                    if (dictionary.All(i => i.Value != value))
                    {
                        dictionary.Add(counter, value);
                        counter++;
                    }
                    foreach (var pair2 in menu.LinkedValues)
                    {
                        var value2 = pair2.Value.DisplayName;
                        if (dictionary.All(i => i.Value != value2))
                        {
                            dictionary.Add(counter, value2);
                            counter++;
                        }
                        var comboBox = pair2.Value as ComboBox;
                        if (comboBox != null)
                        {
                            foreach (var val in comboBox.Overlay.Children)
                            {
                                var value3 = val.TextValue;
                                if (dictionary.All(i => i.Value != value3))
                                {
                                    dictionary.Add(counter, value3);
                                    counter++;
                                }
                            }
                        }
                    }
                    foreach (var subMenu in menu.SubMenus)
                    {
                        var value2 = subMenu.DisplayName;
                        if (dictionary.All(i => i.Value != value2))
                        {
                            dictionary.Add(counter, value2);
                            counter++;
                        }
                    }
                    foreach (var subMenu in menu.SubMenus)
                    {
                        foreach (var pair2 in subMenu.LinkedValues)
                        {
                            var value2 = pair2.Value.DisplayName;
                            if (dictionary.All(i => i.Value != value2))
                            {
                                dictionary.Add(counter, value2);
                                counter++;
                            }
                            var comboBox = pair2.Value as ComboBox;
                            if (comboBox != null)
                            {
                                foreach (var val in comboBox.Overlay.Children)
                                {
                                    var value3 = val.TextValue;
                                    if (dictionary.All(i => i.Value != value3))
                                    {
                                        dictionary.Add(counter, value3);
                                        counter++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            var converted = JsonConvert.SerializeObject(Translations, Formatting.Indented, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            });
            File.WriteAllText(_jsonPath, converted);
        }

        private enum Language
        {
            English,
            Spanish,
            French,
            German,
            Italian,
            Portuguese,
            Polish,
            Turkish,
            Chinese,
            ChineseTraditional,
            Korean,
            Romanian,
            Vietnamese
        }
    }
}
