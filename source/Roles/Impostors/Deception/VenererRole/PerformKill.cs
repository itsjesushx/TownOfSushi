namespace TownOfSushi.Roles.Impostors.Deception.VenererRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Venerer);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (MushroomSabotageActive()) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = GetRole<Venerer>(PlayerControl.LocalPlayer);
            if (__instance == role.AbilityButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.AbilityTimer() != 0 || role.Kills < 1) return false;
                var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;
                Rpc(CustomRPC.Camouflage, PlayerControl.LocalPlayer.PlayerId, role.Kills);
                role.TimeRemaining = CustomGameOptions.AbilityDuration;
                role.KillsAtStartAbility = role.Kills;
                role.Ability();
                return false;
            }

            return true;
        }
    }
}