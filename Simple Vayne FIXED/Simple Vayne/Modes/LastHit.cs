using EloBuddy;
using EloBuddy.SDK;

namespace Simple_Vayne.Modes
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class LastHit : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);
        }

        public override void Execute()
        {
            if (Q.IsReady() && Player.Instance.ManaPercent >= 20)
            {
                //Orbwalker.OnUnkillableMinion += Orbwalker_OnUnkillableMinion;
            }
        }

        private void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (Game.CursorPos.GetTumbleEndPos().IsPositionSafe())
            {
                Q.Cast(Game.CursorPos.ExtendPlayerVector(250));
            }

            Orbwalker.OnUnkillableMinion -= Orbwalker_OnUnkillableMinion;
        }
    }
}