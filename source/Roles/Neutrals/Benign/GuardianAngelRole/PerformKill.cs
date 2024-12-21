namespace TownOfSushi.Roles.Neutral.Benign.GuardianAngelRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class Protect
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = GetRole<GuardianAngel>(PlayerControl.LocalPlayer);
            if (!role.ButtonUsable) return false;
            var protectButton = DestroyableSingleton<HudManager>.Instance.KillButton;
            if (__instance == protectButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.ProtectTimer() != 0) return false;
                var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;
                role.TimeRemaining = CustomGameOptions.ProtectDuration;
                role.MaxUses--;
                role.Protect();
                Rpc(CustomRPC.GAProtect, PlayerControl.LocalPlayer.PlayerId);
                return false;
            }
            return true;
        }
    }
}