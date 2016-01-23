using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using KoreanAIO.Model;
using SharpDX;

namespace KoreanAIO.Utilities
{
    public class BestPositionResult
    {
        public Vector3 Position;
        public int Hits;
        public Obj_AI_Base Target;
        public List<Obj_AI_Base> ObjectsHit = new List<Obj_AI_Base>();

        public BestPositionResult()
        {
            Hits = -1;
        }
    }
    
    public class BestDamageResult
    {
        public float Damage;
        public float Mana;
        public Obj_AI_Base Target;

        public bool IsKillable
        {
            get { return Target != null && Damage >= Target.TotalShieldHealth(); }
        }
        public List<SpellBase> List = new List<SpellBase>();

        public bool Q
        {
            get { return List.Contains(AIO.CurrentChampion.Q); }
        }
        public bool W
        {
            get { return List.Contains(AIO.CurrentChampion.W); }
        }
        public bool E
        {
            get { return List.Contains(AIO.CurrentChampion.E); }
        }
        public bool R
        {
            get { return List.Contains(AIO.CurrentChampion.R); }
        }
        
        public bool Contains(SpellBase spell)
        {
            return List.Contains(spell);
        }
        
    }
}
