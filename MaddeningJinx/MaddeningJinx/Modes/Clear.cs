namespace MaddeningJinx
{
    public static class Clear
    {
        public static bool IsActive
        {
            get { return ModeManager.IsClear; }
        }

        public static void Execute()
        {
            if (ModeManager.IsLaneClear)
            {
                LaneClear.Execute();
            }
            if (ModeManager.IsJungleClear)
            {
                JungleClear.Execute();
            }
        }
    }
}
