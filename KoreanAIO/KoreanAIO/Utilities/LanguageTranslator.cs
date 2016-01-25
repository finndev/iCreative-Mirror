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
                {"Harass.WEQ", "Harass WEQ"},
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
                {"MinimumManaPercent", "Minimum Mana Percent"},
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
                {"Harass.WEQ", "Acoso WEQ"},
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
                {"MinimumManaPercent", "Mínimo porcentaje de mana"},
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
            };
            Translations[Language.Polish] = new Dictionary<string, string>
            {
                {"Language", "Język"},
                {"Polish", "Polski"},
                {"Enabled", "Włączony"},
                {"Disabled", "Wyłączony"},
                /* Keys */
                {"Combo.WithoutR", "Combo bez R"},
                {"Harass.WEQ", "Harass WEQ"},
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
                {"MinimumManaPercent", "Minimalny Procent Many"},
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
            Translations[Language.Chinese] = new Dictionary<string, string>
            {
                {"Language", "语言"},
                {"Chinese", "中文"},
                {"Enabled", "开启"},
                {"Disabled", "关闭"},
                {"Available", "可用的"},
                /* Keys */
                {"Combo.WithoutR", "不用R连招"},
                {"Harass.WEQ", "骚扰 2"},
                /* Toggles */
                {"LastHit.Toggle", "自动尾兵"},
                {"Harass.Toggle", "自动骚扰"},
                /* Submenus */
                {"Keys", "热键"},
                {"Prediction", "预判"},
                {"Combo", "连招"},
                {"Harass", "骚扰"},
                {"Clear", "清线"},
                {"LaneClear", "清线"},
                {"LastHit", "尾兵"},
                {"JungleClear", "清野"},
                {"KillSteal", "抢人头"},
                {"Automatic", "自动"},
                {"Evader", "躲避"},
                {"Drawings", "线圈"},
                {"Flee", "Flee"},
                {"Misc", "杂项"},
                /* LastHit */
                {"Never", "从不"},
                {"Smartly", "中毒的"},
                {"Always", "一直"},
                /* Checkbox, sliders and others */
                {"MinimumManaPercent", "最低能量使用"},
                {"DisableUnderEnemyTurret", "Disable under enemy turret"},
                {"IfKillable", "If killable"},
                {"IfNeeded", "If needed"},
                {"UseIgnite", "使用点燃抢头"},
                {"UseIgnite.Killable", "使用点燃如果可击杀"},
                {"UseSmite", "使用惩戒抢头"},
                {"UseQ", "使用 Q"},
                {"UseQ.Hit", "使用Q如果命中敌人数量 >= {0}"},
                {"UseQ.Gapcloser", "敌人切入时使用Q"},
                {"UseQ.Interrupter", "使用Q打断敌人技能"},
                {"UseW", "使用 W"},
                {"UseW.Hit", "使用W如果命中敌人数量 >= {0}"},
                {"UseW.Gapcloser", "敌人切入时使用W"},
                {"UseW.Interrupter", "使用W打断敌人技能"},
                {"UseE", "使用 E"},
                {"UseE.Hit", "使用E如果命中敌人数量 >= {0}"},
                {"UseE.Gapcloser", "敌人切入时使用E"},
                {"UseE.Interrupter", "使用E打断敌人技能"},
                {"UseR", "使用 R"},
                {"UseR.Killable", "当敌人可击杀使用R"},
                {"UseR.Hit", "使用R如果命中敌人数量 >= {0}"},
                {"UseR.Gapcloser", "敌人切入时使用R"},
                {"UseR.Interrupter", "使用R打断敌人技能"},
                {"R.BlackList", "对敌人不使用R"},
                {"Items", "使用物品"},
                {"Zhonyas", "Use Zhonyas if my % of health is less than {0}"},
                /* Zed */
                {"R.Prevent", "R之前不使用技能"},
                {"R.Combo.Mode", "R 模式"},
                {"UseQ.Collision", "为Q检查施法路径 （更多伤害）"},
                {"UseW1", "使用 W1"},
                {"UseW2", "使用 W2"},
                {"UseR1", "使用 R1"},
                {"UseR2", "使用 R2"},
                {"SwapDead", "使用 W2/R2 如果可击杀敌人"},
                {"SwapGapclose", "使用 W2/R2 接近目标"},
                {"SwapKillable", "使用W2如果可击杀"},
                {"SwapHP", "使用 W2/R2 如果生命 >= {0}"},
                {"Line", "线性"},
                {"Triangle", "三角"},
                {"MousePos", "鼠标位置"},
                {"IsDead", "接受死亡吧"},
                {"Passive", "被动"},
                {"Draw.WShadow", "显示W线圈范围"},
                {"Draw.RShadow", "显示R线圈范围"},
                {"Draw.TargetIsDead", "显示目标可击杀提示"},
                {"Draw.PassiveIsReady", "显示主动技能冷却提示"},
                /* Cassiopeia */
                {"Poisoned", "中毒的"},
                {"AssistedUltimate", "大招辅助"},
                /* Diana */
                {"UseQR", "在小兵上使用QR进行间距"},
                {"R.2nd", "连招使用R2"},
                /* Orianna */
                {"TeamFight.Count", "使用团战逻辑当敌人数量 >= {0}"},
                {"Common.Logic", "普通逻辑"},
                {"1vs1.Logic", "1 vs 1 逻辑"},
                {"TeamFight.Logic", "团战逻辑"},
                {"UseE.HealthPercent", "使用E当生命低于 <= {0}"},
                {"UseE.Spells", "使用E抵挡敌方技能"},
                {"Draw.Ball", "显示球的位置"},
                {"R.Block", "屏蔽R如果会空大"},
                /* Drawings */
                {"Draw.Disable", "关闭线圈"},
                {"Draw.DamageIndicator", "敌方HP显示技能总伤害"},
                {"Draw.Target", "目标身上显示圈"},
                {"Draw.Q", "显示 Q 范围"},
                {"Draw.W", "显示 W 范围"},
                {"Draw.E", "显示 E 范围"},
                {"Draw.R", "显示 R 范围"},
                { "Draw.Toggles", "自动技能面板显示"},
                /* Prediction*/
                {"Q.HitChancePercent", "Q: 命中率百分比"},
                {"W.HitChancePercent", "W: 命中率百分比"},
                {"E.HitChancePercent", "E: 命中率百分比"},
                {"QE.HitChancePercent", "QE: 命中率百分比"},
                {"R.HitChancePercent", "R: 命中率百分比"},
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
