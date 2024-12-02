namespace TownOfSushi.Roles.Impostors.Power.WitchRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Witch)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = GetRole<Witch>(PlayerControl.LocalPlayer);
            if (role.SpellTimer() != 0f) return false;
            
            if (__instance == role.SpellButton)
            {
                var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                if (interact.AbilityUsed)
                {
                    role.SpelledPlayers.Add(role.ClosestPlayer.PlayerId);
                    Rpc(CustomRPC.Spell, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
                }
                if (interact.FullCooldownReset)
                {
                    role.LastSpelled = DateTime.UtcNow;
                    return false;
                }
                else if (interact.GaReset)
                {
                    role.LastSpelled = DateTime.UtcNow;
                    role.LastSpelled = role.LastSpelled.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.SpellCd);
                    return false;
                }
                else if (interact.ZeroSecReset) return false;
            }
            return true;
        }
    }
}