using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using SharpDX;

namespace The_Ball_Is_Angry
{
    public static class KillSteal
    {
        public static readonly List<Obj_AI_Base> RKillableBases = new List<Obj_AI_Base>();
        public static readonly List<Vector3> RHittableBases = new List<Vector3>();
        public static readonly Dictionary<int, float> RDamageOnEnemies = new Dictionary<int, float>();
        public static bool UsingQ;
        public static Menu Menu
        {
            get
            {
                return MenuManager.GetSubMenu("KillSteal");
            }
        }

        public static void Execute()
        {
        }
    }
}
