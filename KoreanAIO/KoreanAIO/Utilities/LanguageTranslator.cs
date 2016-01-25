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
        Portuguese,
        Polish
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
                {"Enabled", "Enabled"},
                {"Disabled", "Disabled"},
                {"Available", "Available"},
                /* Keys */
                {"Combo.WithoutR", "Combo without R"},
                {"Harass.QWE", "Harass QWE"},
                /* Toggles */
                {"LastHit.Toggle", "LastHit Toggle"},
                {"Harass.Toggle", "Harass Toggle"},
                /* Submenus */
                {"Keys", "Keys"},
                {"Prediction", "Prediction"},
                {"Combo", "Combo"},
                {"Harass", "Harass"},
                {"Clear", "Clear"},
                {"LaneClear", "LaneClear"},
                {"LastHit", "LastHit"},
                {"JungleClear", "JungleClear"},
                {"KillSteal", "KillSteal"},
                {"Automatic", "Automatic"},
                {"Evader", "Evader"},
                {"Drawings", "Drawings"},
                {"Flee", "Flee"},
                {"Misc", "Misc"},
                /* LastHit */
                {"Never", "Never"},
                {"Smartly", "Smartly"},
                {"Always", "Always"},
                /* Checkbox, sliders and others */
                {"DisableUnderEnemyTurret", "Disable under enemy turret"},
                {"IfKillable", "If killable"},
                {"IfNeeded", "If needed"},
                {"UseIgnite", "Use Ignite"},
                {"UseIgnite.Killable", "Use Ignite if target is killable"},
                {"UseSmite", "Use Smite"},
                {"UseQ", "Use Q"},
                {"UseQ.Hit", "Use Q if hit is greater than {0}"},
                {"UseQ.Gapcloser", "Use Q on hero gapclosing / dashing"},
                {"UseQ.Interrupter", "Use Q on channeling spells"},
                {"UseW", "Use W"},
                {"UseW.Hit", "Use W if hit is greater than {0}"},
                {"UseW.Gapcloser", "Use W on hero gapclosing / dashing"},
                {"UseW.Interrupter", "Use W on channeling spells"},
                {"UseE", "Use E"},
                {"UseE.Hit", "Use E if hit is greater than {0}"},
                {"UseE.Gapcloser", "Use E on hero gapclosing / dashing"},
                {"UseE.Interrupter", "Use E on channeling spells"},
                {"UseR", "Use R"},
                {"UseR.Killable", "Use R if target is killable"},
                {"UseR.Hit", "Use R if hit is greater than {0}"},
                {"UseR.Gapcloser", "Use R on hero gapclosing / dashing"},
                {"UseR.Interrupter", "Use R on channeling spells"},
                {"R.BlackList", "Don't use R on:"},
                {"Items", "Use offensive items"},
                {"Zhonyas", "Use Zhonyas if my % of health is less than {0}"},
                /* Zed */
                {"R.Prevent", "Don't use spells before R"},
                {"R.Combo.Mode", "R Combo Mode"},
                {"UseQ.Collision", "Check collision when casting Q (more damage)"},
                {"UseW1", "Use W1"},
                {"UseW2", "Use W2"},
                {"UseR1", "Use R1"},
                {"UseR2", "Use R2"},
                {"SwapDead", "Use W2/R2 if target will die"},
                {"SwapGapclose", "Use W2/R2 to get close to target"},
                {"SwapKillable", "Use W2 if target is killable"},
                {"SwapHP", "Use W2/R2 if my % of health is less than {0}"},
                {"MinimumManaPercent", "Minimum Mana Percent"},
                {"Line", "Line"},
                {"Triangle", "Triangle"},
                {"MousePos", "MousePos"},
                {"IsDead", "Is Dead"},
                {"Passive", "Passive"},
                {"Draw.WShadow", "Draw W shadow circle"},
                {"Draw.RShadow", "Draw R shadow circle"},
                {"Draw.TargetIsDead", "Draw text if target will die"},
                {"Draw.PassiveIsReady", "Draw text when passive is available"},
                /* Cassiopeia */
                {"Poisoned", "If Poisoned"},
                {"AssistedUltimate", "Assisted Ultimate"},
                /* Diana */
                {"UseQR", "Use QR on minion to gapclose"},
                {"R.2nd", "Use always second r"},
                /* Orianna */
                {"TeamFight.Count", "Use TeamFight logic if enemies near is greater than {0}"},
                {"Common.Logic", "Common logic"},
                {"1vs1.Logic", "1 vs 1 logic"},
                {"TeamFight.Logic", "TeamFight logic"},
                {"UseE.HealthPercent", "Use E if my % of health is less than {0}"},
                {"UseE.Spells", "Use E on enemy spells"},
                {"Draw.Ball", "Draw ball position"},
                {"R.Block", "Block R if will not hit"},
                /* Drawings */
                {"Draw.Disable", "Disable all drawings"},
                {"Draw.DamageIndicator", "Draw damage indicator"},
                {"Draw.Target", "Draw circle on target"},
                {"Draw.Q", "Draw Q range"},
                {"Draw.W", "Draw W range"},
                {"Draw.E", "Draw E range"},
                {"Draw.R", "Draw R range"},
                { "Draw.Toggles", "Draw toggles status"},
                /* Prediction*/
                {"Q.HitChancePercent", "Q: HitChancePercent"},
                {"W.HitChancePercent", "W: HitChancePercent"},
                {"E.HitChancePercent", "E: HitChancePercent"},
                {"QE.HitChancePercent", "QE: HitChancePercent"},
                {"R.HitChancePercent", "R: HitChancePercent"},
            };
            Translations[Language.Spanish] = new Dictionary<string, string>
            {
                {"Language", "Idioma"},
                {"Spanish", "Español"},
                {"Enabled", "Activado"},
                {"Disabled", "Desactivado"},
                {"Available", "Disponible"},
                /* Keys */
                {"Combo.WithoutR", "Combate sin R"},
                {"Harass.QWE", "Acoso QWE"},
                /* Toggles */
                {"LastHit.Toggle", "Interruptor último golpe"},
                {"Harass.Toggle", "Interruptor acoso"},
                /* Submenus */
                {"Keys", "Teclas"},
                {"Prediction", "Predicción"},
                {"Combo", "Combate"},
                {"Harass", "Acoso"},
                {"Clear", "Farmear"},
                {"LaneClear", "Limpiar la linea"},
                {"LastHit", "Último golpe"},
                {"JungleClear", "Limpiar la jungla"},
                {"KillSteal", "KillSteal"},
                {"Automatic", "Automático"},
                {"Evader", "Evadir"},
                {"Drawings", "Dibujos"},
                {"Flee", "Escape"},
                {"Misc", "Misceláneo"},
                /* LastHit */
                {"Never", "Nunca"},
                {"Smartly", "Inteligentemente"},
                {"Always", "Siempre"},
                /* Checkbox, sliders and others */
                {"DisableUnderEnemyTurret", "Desactivar bajo torre enemiga"},
                {"IfKillable", "Si es matable"},
                {"IfNeeded", "Si se requiere"},
                {"UseIgnite", "Usar Ignición"},
                {"UseIgnite.Killable", "Usar Ignición si el objetivo es matable"},
                {"UseSmite", "Usar Castigo"},
                {"UseQ", "Usar Q"},
                {"UseQ.Hit", "Usar Q si golpea a más de {0}"},
                {"UseQ.Gapcloser", "Usar Q en campeones que estén corriendo"},
                {"UseQ.Interrupter", "Usar Q en campeones canalizando"},
                {"UseW", "Usar W"},
                {"UseW.Hit", "Usar W si golpea a más de {0}"},
                {"UseW.Gapcloser", "Usar W en campeones que estén corriendo"},
                {"UseW.Interrupter", "Usar W en campeones canalizando"},
                {"UseE", "Usar E"},
                {"UseE.Hit", "Usar E si golpea a más de {0}"},
                {"UseE.Gapcloser", "Usar E en campeones que estén corriendo"},
                {"UseE.Interrupter", "Usar E en campeones canalizando"},
                {"UseR", "Usar R"},
                {"UseR.Killable", "Usar R si el objetivo es matable"},
                {"UseR.Hit", "Usar R si golpea a más de {0}"},
                {"UseR.Gapcloser", "Usar R en campeones que estén corriendo"},
                {"UseR.Interrupter", "Usar R en campeones canalizando"},
                {"R.BlackList", "No usar la R contra:"},
                {"Items", "Use items ofensivos"},
                {"Zhonyas", "Usar Zhonyas si mi % de vida es menor a {0}"},
                /* Zed */
                {"R.Prevent", "No usar habilidades antes de la R"},
                {"R.Combo.Mode", "Modo de combate con la R"},
                {"UseQ.Collision", "Chequear colisión al castear la Q (más daño)"},
                {"UseW1", "Usar W1"},
                {"UseW2", "Usar W2"},
                {"UseR1", "Usar R1"},
                {"UseR2", "Usar R2"},
                {"SwapDead", "Usar W2/R2 si el objetivo va a morir"},
                {"SwapGapclose", "Usar W2/R2 para acercarse al objetivo"},
                {"SwapKillable", "Usar W2 si el objetivo es matable"},
                {"SwapHP", "Usar W2/R2 si mi % de vida es menor a {0}"},
                {"MinimumManaPercent", "Mínimo porcentaje de mana"},
                {"Line", "Línea"},
                {"Triangle", "Triángulo"},
                {"MousePos", "Posición del mouse"},
                {"IsDead", "Está muerto"},
                {"Passive", "Pasiva"},
                {"Draw.WShadow", "Dibujar un circulo sobre la sombra de la W"},
                {"Draw.RShadow", "Dibujar un circulo sobre la sombra de la R"},
                {"Draw.TargetIsDead", "Dibujar texto is el objetivo va a morir"},
                {"Draw.PassiveIsReady", "Dibujar texto si la pasiva está disponible"},
                /* Cassiopeia */
                {"Poisoned", "Si está envenenado"},
                {"AssistedUltimate", "R asistida"},
                /* Diana */
                {"UseQR", "Usar QR en minions para acercarse"},
                {"R.2nd", "Usar siempre la segunda R"},
                /* Orianna */
                {"TeamFight.Count", "Usar lógica de TeamFight si los enemigos son más de {0}"},
                {"Common.Logic", "Lógica comun"},
                {"1vs1.Logic", "Lógica 1 versus 1"},
                {"TeamFight.Logic", "Lógica de TeamFight"},
                {"UseE.HealthPercent", "Usar E si mi % de vida es menor a {0}"},
                {"UseE.Spells", "Usar E contra habilidades del enemigo"},
                {"Draw.Ball", "Dibujar un circulo sobre la bola"},
                {"R.Block", "Bloquear la R si no va a golpear"},
                /* Drawings */
                {"Draw.Disable", "Desactivar todos los dibujos"},
                {"Draw.DamageIndicator", "Dibujar indicador de daño"},
                {"Draw.Target", "Dibujar circulo sobre el objetivo"},
                {"Draw.Q", "Dibujar el rango de la Q"},
                {"Draw.W", "Dibujar el rango de la W"},
                {"Draw.E", "Dibujar el rango de la E"},
                {"Draw.R", "Dibujar el rango de la R"},
                { "Draw.Toggles", "Dibujar el estado de los interruptores"},
                /* Prediction*/
                {"Q.HitChancePercent", "Q: Porcentaje de probabilidad de golpe"},
                {"W.HitChancePercent", "W: Porcentaje de probabilidad de golpe"},
                {"E.HitChancePercent", "E: Porcentaje de probabilidad de golpe"},
                {"QE.HitChancePercent", "QE: Porcentaje de probabilidad de golpe"},
                {"R.HitChancePercent", "R: Porcentaje de probabilidad de golpe"},
            };
            Translations[Language.German] = new Dictionary<string, string>
            {
                {"Language", "Sprache"},
                {"German", "Deutsch"},
                {"Enabled", "Enabled"},
                {"Disabled", "Disabled"},
                {"Available", "Available"},
                /* Keys */
                {"Combo.WithoutR", "Combo without R"},
                {"Harass.QWE", "Harass QWE"},
                /* Toggles */
                {"LastHit.Toggle", "LastHit Toggle"},
                {"Harass.Toggle", "Harass Toggle"},
                /* Submenus */
                {"Keys", "Keys"},
                {"Prediction", "Prediction"},
                {"Combo", "Combo"},
                {"Harass", "Harass"},
                {"Clear", "Clear"},
                {"LaneClear", "LaneClear"},
                {"LastHit", "LastHit"},
                {"JungleClear", "JungleClear"},
                {"KillSteal", "KillSteal"},
                {"Automatic", "Automatic"},
                {"Evader", "Evader"},
                {"Drawings", "Drawings"},
                {"Flee", "Flee"},
                {"Misc", "Misc"},
                /* LastHit */
                {"Never", "Never"},
                {"Smartly", "Smartly"},
                {"Always", "Always"},
                /* Checkbox, sliders and others */
                {"DisableUnderEnemyTurret", "Disable under enemy turret"},
                {"IfKillable", "If killable"},
                {"IfNeeded", "If needed"},
                {"UseIgnite", "Use Ignite"},
                {"UseIgnite.Killable", "Use Ignite if target is killable"},
                {"UseSmite", "Use Smite"},
                {"UseQ", "Use Q"},
                {"UseQ.Hit", "Use Q if hit is greater than {0}"},
                {"UseQ.Gapcloser", "Use Q on hero gapclosing / dashing"},
                {"UseQ.Interrupter", "Use Q on channeling spells"},
                {"UseW", "Use W"},
                {"UseW.Hit", "Use W if hit is greater than {0}"},
                {"UseW.Gapcloser", "Use W on hero gapclosing / dashing"},
                {"UseW.Interrupter", "Use W on channeling spells"},
                {"UseE", "Use E"},
                {"UseE.Hit", "Use E if hit is greater than {0}"},
                {"UseE.Gapcloser", "Use E on hero gapclosing / dashing"},
                {"UseE.Interrupter", "Use E on channeling spells"},
                {"UseR", "Use R"},
                {"UseR.Killable", "Use R if target is killable"},
                {"UseR.Hit", "Use R if hit is greater than {0}"},
                {"UseR.Gapcloser", "Use R on hero gapclosing / dashing"},
                {"UseR.Interrupter", "Use R on channeling spells"},
                {"R.BlackList", "Don't use R on:"},
                {"Items", "Use offensive items"},
                {"Zhonyas", "Use Zhonyas if my % of health is less than {0}"},
                /* Zed */
                {"R.Prevent", "Don't use spells before R"},
                {"R.Combo.Mode", "R Combo Mode"},
                {"UseQ.Collision", "Check collision when casting Q (more damage)"},
                {"UseW1", "Use W1"},
                {"UseW2", "Use W2"},
                {"UseR1", "Use R1"},
                {"UseR2", "Use R2"},
                {"SwapDead", "Use W2/R2 if target will die"},
                {"SwapGapclose", "Use W2/R2 to get close to target"},
                {"SwapKillable", "Use W2 if target is killable"},
                {"SwapHP", "Use W2/R2 if my % of health is less than {0}"},
                {"MinimumManaPercent", "Minimum Mana Percent"},
                {"Line", "Line"},
                {"Triangle", "Triangle"},
                {"MousePos", "MousePos"},
                {"IsDead", "Is Dead"},
                {"Passive", "Passive"},
                {"Draw.WShadow", "Draw W shadow circle"},
                {"Draw.RShadow", "Draw R shadow circle"},
                {"Draw.TargetIsDead", "Draw text if target will die"},
                {"Draw.PassiveIsReady", "Draw text when passive is available"},
                /* Cassiopeia */
                {"Poisoned", "If Poisoned"},
                {"AssistedUltimate", "Assisted Ultimate"},
                /* Diana */
                {"UseQR", "Use QR on minion to gapclose"},
                {"R.2nd", "Use always second r"},
                /* Orianna */
                {"TeamFight.Count", "Use TeamFight logic if enemies near is greater than {0}"},
                {"Common.Logic", "Common logic"},
                {"1vs1.Logic", "1 vs 1 logic"},
                {"TeamFight.Logic", "TeamFight logic"},
                {"UseE.HealthPercent", "Use E if my % of health is less than {0}"},
                {"UseE.Spells", "Use E on enemy spells"},
                {"Draw.Ball", "Draw ball position"},
                {"R.Block", "Block R if will not hit"},
                /* Drawings */
                {"Draw.Disable", "Disable all drawings"},
                {"Draw.DamageIndicator", "Draw damage indicator"},
                {"Draw.Target", "Draw circle on target"},
                {"Draw.Q", "Draw Q range"},
                {"Draw.W", "Draw W range"},
                {"Draw.E", "Draw E range"},
                {"Draw.R", "Draw R range"},
                { "Draw.Toggles", "Draw toggles status"},
                /* Prediction*/
                {"Q.HitChancePercent", "Q: HitChancePercent"},
                {"W.HitChancePercent", "W: HitChancePercent"},
                {"E.HitChancePercent", "E: HitChancePercent"},
                {"QE.HitChancePercent", "QE: HitChancePercent"},
                {"R.HitChancePercent", "R: HitChancePercent"},
            };
            Translations[Language.Polish] = new Dictionary<string, string>
            {
                {"Language", "Język"},
                {"Polish", "Polski"},
                {"Enabled", "Włączony"},
                {"Disabled", "Wyłączony"},
                /* Keys */
                {"Combo.WithoutR", "Combo bez R"},
                {"Harass.QWE", "Harass QWE"},
                /* Toggles */
                {"LastHit.Toggle", "LastHit Włączony"},
                {"Harass.Toggle", "Harass Włączony"},
                /* Submenus */
                {"Keys", "Przyciski"},
                {"Prediction", "Predykcja"},
                {"Combo", "Combo"},
                {"Harass", "Harass"},
                {"Clear", "Clear"},
                {"LaneClear", "LaneClear"},
                {"LastHit", "LastHit"},
                {"JungleClear", "JungleClear"},
                {"KillSteal", "KillSteal"},
                {"Automatic", "Automatyczny"},
                {"Evader", "Evader"},
                {"Drawings", "Rysowania"},
                {"Flee", "Ucieczka"},
                {"Misc", "Inne"},
                /* LastHit */
                {"Never", "Nigdy"},
                {"Smartly", "Ograniczony"},
                {"Always", "Zawsze"},
                /* Checkbox, sliders and others */
                {"DisableUnderEnemyTurret", "Wyłączone gdy przeciwnik jest pod wieżą"},
                {"IfKillable", "Jeśli jest do zabicia"},
                {"IfNeeded", "Jeśli potrzebny"},
                {"UseIgnite", "Użyj podpalenia"},
                {"UseIgnite.Killable", "Użyj podpalenia gdy przeciwnik jest do zabicia"},
                {"UseSmite", "Użyj porażenia"},
                {"UseQ", "Użyj Q"},
                {"UseQ.Hit", "Użyj Q gdy może trafić więcej celów niż {0}"},
                {"UseQ.Gapcloser", "Użyj Q na postać gapclosing / dashing"},
                {"UseQ.Interrupter", "Użyj Q na kanałowe umiejętności"},
                {"UseW", "Użyj W"},
                {"UseW.Hit", "Użyj W gdy może trafić więcej celów niż {0}"},
                {"UseW.Gapcloser", "Użyj W na postać gapclosing / dashing"},
                {"UseW.Interrupter", "Użyj W na kanałowe umiejętności"},
                {"UseE", "Użyj E"},
                {"UseE.Hit", "Użyj E gdy może trafić więcej celów niż {0}"},
                {"UseE.Gapcloser", "Użyj E na postacie gapclosing / dashing"},
                {"UseE.Interrupter", "Użyj E na kanałowe umiejętności"},
                {"UseR", "Użyj R"},
                {"UseR.Killable", "Użyj R jeśli cel zginie"},
                {"UseR.Hit", "Użyj R gdy może trafić więcej celów {0}"},
                {"UseR.Gapcloser", "Użyj R na postacie gapclosing / dashing"},
                {"UseR.Interrupter", "Użyj R na kanałowe umiejętności"},
                {"R.BlackList", "Nie używaj R na:"},
                {"Items", "Użyj ofensywnych przedmiotów"},
                {"Zhonyas", "Użyj Zhonyas gdy % życia jest niższe od {0}"},
                /* Zed */
                {"R.Prevent", "Nie używaj umiejętności przed użyciem R"},
                {"R.Combo.Mode", "R Tryb Combo"},
                {"UseQ.Collision", "Sprawdź kolizję przed użyciem Q (więcej obrażeń)"},
                {"UseW1", "Użyj W1"},
                {"UseW2", "Użyj W2"},
                {"UseR1", "Użyj R1"},
                {"UseR2", "Użyj R2"},
                {"SwapDead", "Użyj W2/R2 jeśli cel zginie"},
                {"SwapGapclose", "Użyj W2/R2 aby zbliżyć się do celu"},
                {"SwapKillable", "Użyj W2 jeśli cel jest martwy"},
                {"SwapHP", "Użyj W2/R2 jeśli % mojego zdrowia jest niższy od {0}"},
                {"MinimumManaPercent", "Minimalny Procent Many"},
                {"Line", "Liniowy"},
                {"Triangle", "Trójkątny"},
                {"MousePos", "PozycjaMyszy"},
                {"Draw.WShadow", "Pokaż W cień okrąg"},
                {"Draw.RShadow", "Pokaż R cień okrąg"},
                {"Draw.TargetIsDead", "Pokaż tekst gdy cel jest martwy"},
                {"Draw.PassiveIsReady", "Pokaż tekst gdy pasywna umiejętność jest gotowa"},
                /* Cassiopeia */
                {"Poisoned", "Jeśli zatruty"},
                {"AssistedUltimate", "Asystent Ultimate"},
                /* Diana */
                {"UseQR", "Użyj QR na miniony aby się zbliżyć"},
                {"R.2nd", "Użyj zawsze pierwsze R"},
                /* Orianna */
                {"TeamFight.Count", "Użyj logiki WalkiDrużynowej gdy w pobliżu jest więcej przeciwników od {0}"},
                {"Common.Logic", "Common logic"},
                {"1vs1.Logic", "Logika 1vs1"},
                {"TeamFight.Logic", "Logika WalkiDrużynowej"},
                {"UseE.HealthPercent", "Użyj E gdy moje hp % zdrowia jest mniejsze od {0}"},
                {"UseE.Spells", "Użyj E na umiejętności przeciwnika"},
                {"Draw.Ball", "Pokaż pozycje kuli"},
                {"R.Block", "Blokuj R gdy nie trafi żadnego celu"},
                /* Drawings */
                {"Draw.Disable", "Wyłącz wszystkie rysowania"},
                {"Draw.DamageIndicator", "Pokaż wskaźnik obrażeń"},
                {"Draw.Target", "Pokaż krąg na celu"},
                {"Draw.Q", "Pokaż Q zasięg"},
                {"Draw.W", "Pokaż W zasięg"},
                {"Draw.E", "Pokaż E zasięg"},
                {"Draw.R", "Pokaż R zasięg"},
                { "Draw.Toggles", "Draw toggles status"},
                /* Prediction*/
                {"Q.HitChancePercent", "Q: SzansaNaTrafienie"},
                {"W.HitChancePercent", "W: SzansaNaTrafienie"},
                {"E.HitChancePercent", "E: SzansaNaTrafienie"},
                {"QE.HitChancePercent", "QE: SzansaNaTrafienie"},
                {"R.HitChancePercent", "R: SzansaNaTrafienie"},
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

        public static string GetTranslationFromId(this string name)
        {
            if (Translations.ContainsKey(CurrentLanguage) && Translations[CurrentLanguage].ContainsKey(name))
            {
                return Translations[CurrentLanguage][name];
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