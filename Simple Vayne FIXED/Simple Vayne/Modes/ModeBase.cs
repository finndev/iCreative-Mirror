using EloBuddy.SDK;

namespace Simple_Vayne.Modes
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ModeBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected Spell.Skillshot Q
        {
            get { return SpellManager.Q; }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Spell.Targeted E
        {
            get { return SpellManager.E; }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Spell.Active R
        {
            get { return SpellManager.R; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract bool ShouldBeExecuted();

        /// <summary>
        /// 
        /// </summary>
        public abstract void Execute();
    }
}