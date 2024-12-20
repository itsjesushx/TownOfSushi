using TownOfSushi.Roles.Crewmates.Support.MedicRole;

namespace TownOfSushi.Roles.Crewmates.Killing.VigilanteRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public static class Kill
    {
        [HarmonyPriority(Priority.First)]
        private static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante);
            if (!flag) return true;
            var role = GetRole<Vigilante>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var flag2 = role.VigilanteKillTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled || role.ClosestPlayer == null) return false;
            var distBetweenPlayers = GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers < KillDistance();
            if (!flag3) return false;

            var flag4 = role.ClosestPlayer.Data.IsImpostor() || role.ClosestPlayer.Is(RoleAlignment.NeutralKilling)||
                        role.ClosestPlayer.Is(RoleAlignment.NeutralEvil) && CustomGameOptions.VigilanteKillsNeutralEvil ||
                        role.ClosestPlayer.Is(RoleAlignment.NeutralBenign) && CustomGameOptions.VigilanteKillsNeutralBenign;

            var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return false;

            if (role.ClosestPlayer.Is(RoleEnum.Pestilence))
            {
                RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                return false;
            }
            if (role.ClosestPlayer.IsInfected() || role.Player.IsInfected())
            {
                foreach (var pb in GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
            }
            if (role.ClosestPlayer.IsOnAlert())
            {
                if (role.ClosestPlayer.IsShielded())
                {
                    var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                    Rpc(CustomRPC.AttemptSound, medic, role.ClosestPlayer.PlayerId);

                    if (CustomGameOptions.ShieldBreaks) role.LastKilled = DateTime.UtcNow;

                    StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                    RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                }
                else if (role.Player.IsShielded())
                {
                    var medic = role.Player.GetMedic().Player.PlayerId;
                    Rpc(CustomRPC.AttemptSound, medic, role.Player.PlayerId);
                    if (CustomGameOptions.ShieldBreaks) role.LastKilled = DateTime.UtcNow;
                    StopKill.BreakShield(medic, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                    RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                    if (CustomGameOptions.VigilanteKillOther && !role.ClosestPlayer.IsProtected() && CustomGameOptions.KilledOnAlert)
                        RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                }
                else
                {
                    RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                    if (CustomGameOptions.KilledOnAlert && CustomGameOptions.VigilanteKillOther)
                    {
                        RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                    }
                }

                return false;
            }
            else if (role.ClosestPlayer == ShowRoundOneShield.FirstRoundShielded) return false;
            else if (role.ClosestPlayer.IsShielded())
            {
                var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                Rpc(CustomRPC.AttemptSound, medic, role.ClosestPlayer.PlayerId);

                if (CustomGameOptions.ShieldBreaks) role.LastKilled = DateTime.UtcNow;

                StopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                return false;
            }
            else if (role.ClosestPlayer.IsProtected())
            {
                if (!flag4)
                {
                    RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                }
                role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                return false;
            }

            if (!flag4)
            {
                if (CustomGameOptions.VigilanteKillOther)
                    RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                role.LastKilled = DateTime.UtcNow;
            }
            else
            {
                RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                role.LastKilled = DateTime.UtcNow;
            }
            return false;
        }
    }
}