using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using SharpDX;

using Prediction = EloBuddy.SDK.Prediction.Position;

namespace Simple_Vayne
{
    /// <summary>
    /// Usefull functions
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Vayne's passive range
        /// </summary>
        public static int PassiveRange = 2000;

        /// <summary>
        /// Returns Q's additional percentage damage scaling
        /// </summary>
        public static readonly float[] QAdditionalDamage = {0, 0.3f, 0.35f, 0.40f, 0.45f, 0.5f};

        /// <summary>
        /// Returns W base damage
        /// </summary>
        public static readonly int[] WBaseDamage = {0, 40, 60, 80, 100, 120};

        /// <summary>
        /// Returns how much target's perc health of W can deal
        /// </summary>
        public static readonly float[] WHealthPercentage = {0, 0.06f, 0.075f, 0.09f, 0.105f, 0.12f};

        /// <summary>
        /// Returns E base damage
        /// </summary>
        public static readonly int[] EBaseDamage = {0, 45, 80, 115, 150, 185};

        /// <summary>
        /// Returns R duration
        /// </summary>
        public static readonly int[] RDuration = {0, 8, 10, 12};

        /// <summary>
        /// Last message tick
        /// </summary>
        public static float LastMessageTick;

        /// <summary>
        /// Last Message String
        /// </summary>
        public static string LastMessageString;

        /// <summary>
        /// Female champions list
        /// </summary>
        public static List<string> FemaleChampions = new List<string>
        {
            "Ahri", "Akali", "Anivia", "Annie",
            "Ashe", "Caitlyn", "Cassiopeia", "Diana",
            "Elise", "Evelynn", "Fiora", "Illaoi",
            "Irelia", "Janna", "Jinx", "Kalista",
            "Karma", "Katarina", "Kayle", "Kindred",
            "Leblanc", "Leona", "Lissandra", "Lulu",
            "Lux", "MissFortune", "Morgana", "Nami",
            "Nidalee", "Orianna", "Poppy", "Quinn",
            "RekSai", "Riven", "Sejuani", "Shyvana",
            "Sivir", "Sona", "Soraka", "Syndra",
            "Tristana", "Vayne",  "Vi", "Zyra"
        };

        /// <summary>
        /// Gets the Silver stacks on unit
        /// </summary>
        /// <param name="unit">Target</param>
        /// <returns>Silver stacks</returns>
        public static int GetSilverStacks(this Obj_AI_Base unit)
        {
            var firstorDefault = unit?.Buffs.FirstOrDefault(x => x.Name == "VayneSilveredDebuff");
            return firstorDefault?.Count ?? 0;
        }

        /// <summary>
        /// Gets the Silver stacks on unit
        /// </summary>
        /// <param name="unit">Target</param>
        /// <returns>Silver stacks</returns>
        public static int GetSilverStacks(this AIHeroClient unit)
        {
            var firstorDefault = unit?.Buffs.FirstOrDefault(x => x.Name == "VayneSilveredDebuff");
            return firstorDefault?.Count ?? 0;
        }

        /// <summary>
        /// Modifies a Vecto2 to Vector3
        /// </summary>
        /// <param name="from">Input Vector</param>
        /// <returns>Vector3</returns>
        public static Vector3 ToVector3(this Vector2 from)
        {
            return new Vector3(from.X, from.Y, NavMesh.GetHeightForPosition(from.X, from.Y));
        }

        /// <summary>
        /// Checks if enemy is killable from E and 3 Silver stacks
        /// </summary>
        /// <param name="unit"></param>
        /// <returns>True if enemy will die</returns>
        public static bool IsKillableFromSilverStacks(this AIHeroClient unit)
        {
            if (!unit.IsECastableOnEnemy())
                return false;

            var edmg = Player.Instance.CalculateDamageOnUnit(unit, DamageType.Physical,
                EBaseDamage[SpellManager.E.Level] + Player.Instance.FlatPhysicalDamageMod/2);

            var damage = unit.CalculateWDamage() + edmg;

            return unit.Health <= damage;
        }

        /// <summary>
        /// Calculates damage on enemy
        /// </summary>
        /// <param name="unit">Enemy</param>
        /// <returns>The damage</returns>
        public static float CalculateWDamage(this Obj_AI_Base unit)
        {
            var wLevel = Player.Instance.Spellbook.GetSpell(SpellSlot.W).Level;

            return Math.Max(WBaseDamage[wLevel], unit.MaxHealth*WHealthPercentage[wLevel]);
        }

        /// <summary>
        /// Calculates damage on enemy
        /// </summary>
        /// <param name="unit">Enemy</param>
        /// <returns>The damage</returns>
        public static float CalculateWDamage(this AIHeroClient unit)
        {
            var wLevel = Player.Instance.Spellbook.GetSpell(SpellSlot.W).Level;

            return Math.Max(WBaseDamage[wLevel], unit.MaxHealth * WHealthPercentage[wLevel]);
        }

        /// <summary>
        /// Gets if enemy is vurnerable to condemn
        /// </summary>
        /// <param name="unit">Target</param>
        /// <returns>Returns true if E can be used on an enemy</returns>
        public static bool IsECastableOnEnemy(this AIHeroClient unit)
        {
            return SpellManager.E.IsReady() && Config.CondemnMenu.UseEOn(unit.ChampionName.ToLower()) && !unit.IsDead && !unit.IsZombie &&
                   unit.IsValidTarget(SpellManager.E.Range) &&
                   !unit.HasBuffOfType(BuffType.Invulnerability) && !unit.HasBuffOfType(BuffType.SpellImmunity) &&
                   !unit.HasBuffOfType(BuffType.SpellShield);
        }

        /// <summary>
        /// Returns true if all 3 condemn vectors have wall collision flags
        /// </summary>
        /// <param name="unit">Target</param>
        /// <param name="range">Push distance</param>
        /// <returns>Returns true if vectors have wall collision flags</returns>
        public static bool CanICastE(AIHeroClient unit, float range)
        {
            if (unit == null || !unit.IsECastableOnEnemy() || unit.IsDashing())
                return false;

            var accuracy = Config.CondemnMenu.EMethod;
            var position = Prediction.PredictUnitPosition(unit, 200);

            if (!unit.CanMove)
            {
                for (var i = 25; i < range + 50; i += 50)
                {
                    var vec = unit.ServerPosition.Extend(Player.Instance.ServerPosition, -Math.Min(i, range));
                    if (vec.IsWall())
                    {
                        return true;
                    }
                }
            }
            else
            {
                switch (accuracy)
                {
                    case 0:
                    {
                        for (var i = Config.CondemnMenu.PushDistance; i >= 100; i -= 100)
                        {
                            var vec = position.Extend(Player.Instance.ServerPosition, -i);
                            var left = new Vector2[5];
                            var right = new Vector2[5];
                            var var = 18*i/100;

                            for (var x = 0; x < 5; x++)
                            {
                                left[x] =
                                    position.Extend(
                                        vec + (position - vec).Normalized().Rotated((float) ToRadian(Math.Max(0, var)))*
                                        Math.Abs(i < 200 ? 50 : 45*x), i);
                                right[x] =
                                    position.Extend(
                                        vec +
                                        (position - vec).Normalized().Rotated((float) ToRadian(-Math.Max(0, var)))*
                                        Math.Abs(i < 200 ? 50 : 45*x), i);
                                }
                            if (left[0].IsWall() && right[0].IsWall() && left[1].IsWall() && right[1].IsWall() &&
                                left[2].IsWall() && right[2].IsWall() && left[3].IsWall() && right[3].IsWall() &&
                                left[4].IsWall() && right[4].IsWall() && vec.IsWall())
                            {
                                return true;
                            }
                        }
                        break;
                    }
                    case 1:
                    {
                        for (var i = 25; i < range + 50; i += 50)
                        {
                            var vec = position.Extend(Player.Instance.ServerPosition, -Math.Min(i, range));
                            if (vec.IsWall())
                            {
                                return true;
                            }
                        }
                        break;
                    }
                    default:
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        /// <param name="degree">Degrees</param>
        /// <returns>Radians</returns>
        public static double ToRadian(int degree)
        {
            return Math.PI/180*degree;
        }

        /// <summary>
        /// Checks if the given vector is under enemy tower
        /// </summary>
        /// <param name="vector">Vector Position</param>
        /// <returns><c>True</c> if vector is under enemy  tower.</returns>
        public static bool IsVectorUnderEnemyTower(this Vector3 vector)
        {
            return EntityManager.Turrets.Enemies.Any(x => x.IsValidTarget(900, true, vector));
        }

        /// <summary>
        /// Checks if the spell is ready
        /// </summary>
        /// <param name="unit">Unit</param>
        /// <param name="spellSlot">SpellSlot</param>
        /// <returns><c>True</c> if spell is ready</returns>
        public static bool IsReady(this AIHeroClient unit, SpellSlot spellSlot)
        {
            return unit.Spellbook.GetSpell(spellSlot).IsLearned &&
                   unit.Spellbook.GetSpell(spellSlot).State == SpellState.Surpressed ||
                   unit.Spellbook.GetSpell(spellSlot).State == SpellState.Disabled;
        }

        /// <summary>
        /// Extends Player.ServerPosition vector
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <param name="distance">Distance</param>
        /// <returns></returns>
        public static Vector3 ExtendPlayerVector(this Vector3 vector, int distance = 300)
        {
            return vector == Vector3.Zero
                ? Vector3.Zero
                : Player.Instance.ServerPosition.Extend(vector, distance).ToVector3();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="unit"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static bool IsInsideUnitsRange(this Vector3 vector, AIHeroClient unit, float range)
        {
            var polygon = new Geometry.Polygon.Circle(unit.ServerPosition, range);
            return polygon.IsInside(vector);
        }

        /// <summary>
        /// Checks if the vector is inside enemy AutoAttack range
        /// </summary>
        /// <param name="vector">Vector3 position</param>
        /// <param name="range">Range of enemy AutoAttack range</param>
        /// <returns></returns>
        public static bool IsInsideEnemyRange(this Vector3 vector, float range)
        {
            return
                EntityManager.Heroes.Enemies.Where(index => index.IsValidTarget(1500))
                    .Select(enemy => new Geometry.Polygon.Circle(enemy.ServerPosition, range))
                    .Select(polygon => polygon.IsInside(vector))
                    .FirstOrDefault();
        }

        /// <summary>
        /// Checks if the vector is inside enemy AutoAttack range
        /// </summary>
        /// <param name="vector">Vector3 position</param>
        /// <param name="range">Range of enemy AutoAttack range</param>
        /// <param name="countEnemies">Set to to check how much enemies is inside</param>
        /// <returns></returns>
        public static int IsInsideEnemyRange(this Vector3 vector, float range, bool countEnemies)
        {
            return
                EntityManager.Heroes.Enemies.Where(index => index.IsValidTarget(1500))
                    .Select(enemy => new Geometry.Polygon.Circle(enemy.ServerPosition, range))
                    .Select(polygon => polygon.IsInside(vector))
                    .Count();
        }

        /// <summary>
        /// Counts melee champions in the given range
        /// </summary>
        /// <param name="range">Range</param>
        /// <returns>Number of enemies</returns>
        public static int CountMeelesInRange(float range)
        {
            return EntityManager.Heroes.Enemies.Count(index => index.IsValidTarget(range) && index.IsMelee);
        }

        /// <summary>
        /// Gets if the position is safe or not
        /// </summary>
        /// <param name="vec">Vector3 position</param>
        /// <returns><c>True</c> if position is safe</returns>
        public static bool IsPositionSafe(this Vector3 vec)
        {
            var player = Player.Instance;
            var enemy = EntityManager.Heroes.Enemies.Where(index => index.IsValidTarget(1500));
            var allies = player.ServerPosition.CountAlliesInRange(1500);
            var enemies = player.ServerPosition.CountEnemiesInRange(1500);
            var meeles = CountMeelesInRange(1500);
            var ignoreallchecks = Config.TumbleMenu.IgnoreAllChecks;
            var dangerouspells = Config.TumbleMenu.DangerousSpells;
            var dangerlevel = Program.DangerLevel;
            var vector = vec.ExtendPlayerVector();

            if (ignoreallchecks || enemies == 0)
                return true;
            
            if (vector.IsVectorUnderEnemyTower())
                return false;

            if (dangerouspells)
            {
                if (dangerlevel == 0)
                {
                    var playerhp = Player.Instance.HealthPercent;

                    if (enemies == 1 && meeles == 0)
                    {
                        return true;
                    }

                    if (enemies == 1 && meeles == 1)
                    {
                        var closest = enemy.OrderBy(x => x.Distance(vector)).FirstOrDefault();
                        var positionAfter = Prediction.PredictUnitPosition(closest, 250);

                        if (closest != null && (vector.Distance(positionAfter, true) > 250 * 250 && closest.HealthPercent > playerhp))
                        {
                            return true;
                        }
                        if (closest != null && playerhp > closest.HealthPercent)
                        {
                            return true;
                        }
                    }
                    else if (enemies > allies)
                    {
                        return (from enemyTarget in EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(1200))
                                let positionAfter = Prediction.PredictUnitPosition(enemyTarget, 250)
                                select
                                    new Geometry.Polygon.Circle(positionAfter,
                                        enemyTarget.IsMelee
                                            ? enemyTarget.GetAutoAttackRange() + 350
                                            : enemyTarget.GetAutoAttackRange() + 150)).Any(
                                            polygonCircle => polygonCircle.IsOutside(vector.To2D()));
                    }

                    else if (enemies <= allies)
                    {
                        return (from enemyTarget in EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(1200))
                                let positionAfter = Prediction.PredictUnitPosition(enemyTarget, 250)
                                select
                                    new Geometry.Polygon.Circle(positionAfter,
                                        enemyTarget.IsMelee
                                            ? enemyTarget.GetAutoAttackRange() + 150
                                            : enemyTarget.GetAutoAttackRange())).Any(
                                            polygonCircle => polygonCircle.IsOutside(vector.To2D()));
                    }
                }
                else if (dangerlevel > 0 && dangerlevel < 150)
                {
                    var playerhp = Player.Instance.HealthPercent;

                    if (enemies == 1 && meeles == 0)
                    {
                        return true;
                    }

                    if (enemies == 1 && meeles == 1)
                    {
                        var closest = enemy.OrderBy(x => x.Distance(vector)).FirstOrDefault();
                        var positionAfter = Prediction.PredictUnitPosition(closest, 250);

                        if (closest != null && (vector.Distance(positionAfter, true) > 300 * 300 && closest.HealthPercent > playerhp))
                        {
                            return true;
                        }
                        if (closest != null && playerhp > closest.HealthPercent)
                        {
                            return true;
                        }
                    }
                    else if (enemies > allies)
                    {
                        return (from enemyTarget in EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(2000))
                                let positionAfter = Prediction.PredictUnitPosition(enemyTarget, 500)
                                select
                                    new Geometry.Polygon.Circle(positionAfter,
                                        enemyTarget.IsMelee
                                            ? enemyTarget.GetAutoAttackRange() + 600
                                            : enemyTarget.GetAutoAttackRange() + 150)).Any(
                                            polygonCircle => polygonCircle.IsOutside(vector.To2D()));
                    }

                    else if (enemies <= allies)
                    {
                        return (from enemyTarget in EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(2000))
                                let positionAfter = Prediction.PredictUnitPosition(enemyTarget, 500)
                                select
                                    new Geometry.Polygon.Circle(positionAfter,
                                        enemyTarget.IsMelee
                                            ? enemyTarget.GetAutoAttackRange() + 450
                                            : enemyTarget.GetAutoAttackRange() + 150)).Any(
                                            polygonCircle => polygonCircle.IsOutside(vector.To2D()));
                    }
                }
                else if (dangerlevel > 150 && dangerlevel < 200)
                {
                    if (vector.IsInsideEnemyRange(800, true) == 0)
                        return true;

                    if (GetTumbleEndPos(vector).Distance(Player.Instance) < 280)
                        return false;

                    var playerhp = Player.Instance.HealthPercent;

                    if (enemies == 1 && meeles == 0)
                    {
                        return true;
                    }

                    if (enemies == 1 && meeles == 1)
                    {
                        var closest = enemy.OrderBy(x => x.Distance(vector)).FirstOrDefault();
                        var positionAfter = Prediction.PredictUnitPosition(closest, 250);

                        if (closest != null && (vector.Distance(positionAfter, true) > 400 * 400 && closest.HealthPercent > playerhp))
                        {
                            return true;
                        }
                        if (closest != null && playerhp > closest.HealthPercent)
                        {
                            return true;
                        }
                    }
                    else if (enemies > allies)
                    {
                        return (from enemyTarget in EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(2000))
                                let positionAfter = Prediction.PredictUnitPosition(enemyTarget, 500)
                                select
                                    new Geometry.Polygon.Circle(positionAfter,
                                        enemyTarget.IsMelee
                                            ? enemyTarget.GetAutoAttackRange() + 600
                                            : enemyTarget.GetAutoAttackRange() + 300)).Any(
                                            polygonCircle => polygonCircle.IsOutside(vector.To2D()));
                    }

                    else if (enemies <= allies)
                    {
                        return (from enemyTarget in EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(2000))
                                let positionAfter = Prediction.PredictUnitPosition(enemyTarget, 500)
                                select
                                    new Geometry.Polygon.Circle(positionAfter,
                                        enemyTarget.IsMelee
                                            ? enemyTarget.GetAutoAttackRange() + 450
                                            : enemyTarget.GetAutoAttackRange() + 300)).Any(
                                            polygonCircle => polygonCircle.IsOutside(vector.To2D()));
                    }
                }
            }
            else
            {
                var playerhp = Player.Instance.HealthPercent;

                if (enemies == 1 && meeles == 0)
                {
                    return true;
                }

                if (enemies == 1 && meeles == 1)
                {
                    var closest = enemy.OrderBy(x => x.Distance(vector)).FirstOrDefault();
                    var positionAfter = Prediction.PredictUnitPosition(closest, 250);

                    if (closest != null && (vector.Distance(positionAfter, true) > 250*250 && closest.HealthPercent > playerhp))
                    {
                        return true;
                    }
                    if (closest != null && playerhp > closest.HealthPercent)
                    {
                        return true;
                    }
                }
                else if (enemies > allies)
                {
                    return (from enemyTarget in EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(1200))
                        let positionAfter = Prediction.PredictUnitPosition(enemyTarget, 250)
                        select
                            new Geometry.Polygon.Circle(positionAfter,
                                enemyTarget.IsMelee
                                    ? enemyTarget.GetAutoAttackRange() + 350
                                    : enemyTarget.GetAutoAttackRange() + 150)).Any(
                                        polygonCircle => polygonCircle.IsOutside(vector.To2D()));
                }

                else if (enemies <= allies)
                {
                    return (from enemyTarget in EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(1200))
                            let positionAfter = Prediction.PredictUnitPosition(enemyTarget, 250)
                            select
                                new Geometry.Polygon.Circle(positionAfter,
                                    enemyTarget.IsMelee
                                        ? enemyTarget.GetAutoAttackRange() + 150
                                        : enemyTarget.GetAutoAttackRange())).Any(
                                        polygonCircle => polygonCircle.IsOutside(vector.To2D()));
                }
            }
            return false;
        }

        /// <summary>
        /// Prints info message
        /// </summary>
        /// <param name="message">The message string</param>
        /// <param name="possibleFlood">Set to true if there is a flood possibility</param>
        public static void PrintInfoMessage(string message, bool possibleFlood = true)
        {
            if (FemaleChampions.Any(key => message.Contains(key)))
            {
                message = Regex.Replace(message, "him", "her", RegexOptions.IgnoreCase);
                message = Regex.Replace(message, "his", "hers", RegexOptions.IgnoreCase);
            }

            if (possibleFlood && LastMessageTick + 500 > Game.Time * 1000 && LastMessageString == message)
                return;

            LastMessageTick = Game.Time * 1000;
            LastMessageString = message;
            
            Chat.Print("<font color=\"#0075B0\">[<b>SIMPLE VAYNE</b>]</font> " + message + "</font>");
        }


        /// <summary>
        /// Gets the tumble end position
        /// </summary>
        /// <param name="startpos">Vector3 start position</param>
        /// <returns></returns>
        public static Vector3 GetTumbleEndPos(this Vector3 startpos)
        {
            if (startpos == Vector3.Zero)
                return Vector3.Zero;

            var vector = startpos.Extend(Player.Instance.Position, 0f);

            return !vector.IsWall() ? vector.ToVector3() : CutVectorNearWall(vector);
        }

        /// <summary>
        /// Cuts vector near wall
        /// </summary>
        /// <param name="from">vector2 position</param>
        /// <returns></returns>
        public static Vector3 CutVectorNearWall(Vector2 from)
        {
            var distance = Player.Instance.Position.Distance(from);

            var x = from.Shorten(Player.Instance.Position.To2D(), distance);

            var output = new Vector2();

            for (var i = 0; i < 1000; i++)
            {
                var vec = Player.Instance.Position.Extend(x, i);

                if (!vec.IsWall())
                    continue;

                output = vec;

                break;
            }
            return output.ToVector3();
        }

        /// <summary>
        /// Converts list to polygon
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Geometry.Polygon ToPolygon(this IEnumerable<Vector2> list)
        {
            var polygon = new Geometry.Polygon();

            foreach (var x in list)
            {
                polygon.Add(x);
            }

            return polygon;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Geometry.Polygon[] SegmentedAutoattackPolygons()
        {
            var aaPolygon = new Geometry.Polygon.Circle(Player.Instance.Position,
                Player.Instance.GetAutoAttackRange(), 24);

            var points = new Vector2[24];
            var i = 0;

            foreach (var point in aaPolygon.Points)
            {
                points[i] = point;
                i++;
            }

            var polygons = new Geometry.Polygon[4];

            var dictionary = new Dictionary<int, List<Vector2>>
            {
                {
                    0, // 1
                    new List<Vector2>
                    {
                        aaPolygon.Center,
                        points[3],
                        points[4],
                        points[5],
                        points[6],
                        points[7],
                        points[8],
                        points[9]
                    }
                },
                {
                    1, // 2
                    new List<Vector2>
                    {
                        aaPolygon.Center,
                        points[9],
                        points[10],
                        points[11],
                        points[12],
                        points[13],
                        points[14],
                        points[15]
                    }
                },
                {
                    2,// Reverse 1
                    new List<Vector2>
                    {
                        aaPolygon.Center,
                        points[15],
                        points[16],
                        points[17],
                        points[18],
                        points[19],
                        points[20],
                        points[21]
                    }
                },
                {
                    3, // Reverse 2
                    new List<Vector2>
                    {
                        aaPolygon.Center,
                        points[21],
                        points[22],
                        points[23],
                        points[0],
                        points[1],
                        points[2],
                        points[3]
                    }
                }
            };

            for (var x = 0; x < 4; x++)
            {
                polygons[x] = new Geometry.Polygon();
                polygons[x].Add(dictionary[x].ToPolygon());
            }
            return polygons;
        }

        /// <summary>
        /// Gets the Forth and rear autoattack polygons
        /// </summary>
        /// <returns>Index 0 = Forth, index 1 = rear</returns>
        public static Geometry.Polygon[] ReturnForthandRearPolygons()
        {
            var polygons = SegmentedAutoattackPolygons();
            var index = 0;
            var forthAndRearPolygons = new Geometry.Polygon[2];

            for (var i = 0; i < 4; i++)
            {
                var objAiTurret = EntityManager.Turrets.Enemies.OrderBy(a => a.Distance(Player.Instance.ServerPosition)).FirstOrDefault(x => x.Distance(Player.Instance.Position) < 2500);
                var enemy = EntityManager.Heroes.Enemies.OrderBy(a => a.Distance(Player.Instance.ServerPosition)).FirstOrDefault(x => x.Distance(Player.Instance.Position) < 1200);

                Vector3 direction;
                if (enemy != null)
                {
                    direction = enemy.ServerPosition;
                }
                else if (objAiTurret != null)
                {
                    direction = objAiTurret.ServerPosition;
                }
                else
                {
                    direction = Player.Instance.Path.Last();
                }

                if (polygons[i].IsInside(direction.ExtendPlayerVector()))
                {
                    index = i;
                }
            }
            var reverseid = index < 2 ? index + 2 : index - 2;

            forthAndRearPolygons[0] = polygons[index];
            forthAndRearPolygons[1] = polygons[reverseid];

            return forthAndRearPolygons;
        }

        /// <summary>
        /// Gets the Forth and rear autoattack polygon ids
        /// </summary>
        /// <returns>Index 0 = Forth, index 1 = rear</returns>
        public static int[] ReturnForthandRearPolygons(bool returnId) 
        {
            if (!returnId)
                return default(int[]);

            var polygons = SegmentedAutoattackPolygons();
            var index = new int[2];

            for (var i = 0; i < 4; i++)
            {
                var objAiTurret = EntityManager.Turrets.Enemies.OrderBy(a => a.Distance(Player.Instance.ServerPosition)).FirstOrDefault(x => x.Distance(Player.Instance.Position) < 2500);
                var enemy = EntityManager.Heroes.Enemies.OrderBy(a => a.Distance(Player.Instance.ServerPosition)).FirstOrDefault(x => x.Distance(Player.Instance.Position) < 1200);

                Vector3 direction;
                if (enemy != null)
                {
                    direction = enemy.ServerPosition;
                }
                else if (objAiTurret != null)
                {
                    direction = objAiTurret.ServerPosition;
                }
                else
                {
                    direction = Player.Instance.Path.Last();
                }

                if (polygons[i].IsInside(direction.ExtendPlayerVector()))
                {
                    index[0] = i;
                }
            }
            var reverseid = index[0] < 2 ? index[0] + 2 : index[0] - 2;
            index[1] = reverseid;

            return index;
        }

        /// <summary>
        /// Gets the side autoattack polygons
        /// </summary>
        /// <returns></returns>
        public static Geometry.Polygon[] GetSidePolygons()
        {
            var id = ReturnForthandRearPolygons(true)[0];
            var reverseid = id < 2 ? id + 2 : id - 2;
            
            var allpolygons = SegmentedAutoattackPolygons();
            var polygons = new Geometry.Polygon[2];

            if (reverseid == 0 || reverseid == 2 && id == 0 || id == 2)
            {
                polygons[0] = allpolygons[1];
                polygons[1] = allpolygons[3];
            }
            else
            {
                polygons[0] = allpolygons[0];
                polygons[1] = allpolygons[2];
            }

            return polygons;
        }

        /// <summary>
        /// Gets the side autoattack polygon ids
        /// </summary>
        /// <returns></returns>
        public static int[] GetSidePolygons(bool returnId)
        {
            if (!returnId)
                return default(int[]);

            var index = new int[2];
            var id = ReturnForthandRearPolygons(true)[0];
            var reverseid = id < 2 ? id + 2 : id - 2;
            
            if (reverseid == 0 || reverseid == 2 && id == 0 || id == 2)
            {
                index[0] = 1;
                index[1] = 3;
            }
            else
            {
                index[0] = 0;
                index[1] = 2;
            }

            return index;
        }
    }

    /// <summary>
    /// Class for easier List ussage
    /// </summary>
    public class ProcessSpellCastCache
    {
        /// <summary>
        /// Game*1000
        /// </summary>
        public int Tick;

        /// <summary>
        /// Sender
        /// </summary>
        public AIHeroClient Sender;

        /// <summary>
        /// Sender's NetworkID
        /// </summary>
        public int NetworkId;

        /// <summary>
        /// Danger Level
        /// </summary>
        public DangerLevel DangerLevel;
    }
}