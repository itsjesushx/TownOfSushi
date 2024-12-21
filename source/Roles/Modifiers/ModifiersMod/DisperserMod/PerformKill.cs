namespace TownOfSushi.Modifiers.DisperserMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Disperser)) return true;

            var role = Modifier.GetModifier<Disperser>(PlayerControl.LocalPlayer);
            if (__instance != role.DisperseButton) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (role.MaxUses <= 0) return false;
            //if (role.StartTimer() > 0) return false;
            if (!(role.DisperseTimer() == 0f)) return false;
            if (!__instance.enabled) return false;
            if (!role.ButtonUsable) return false;
            role.MaxUses--;
            role.LastDispersed = DateTime.UtcNow;
            role.Disperse();

            return false;
        }
    }
}