using TownOfSushi.Roles.Crewmates.Investigative.TrapperMod;
using TownOfSushi.Roles.Crewmates.Investigative.InvestigatorMod;

namespace TownOfSushi.Roles.Crewmates.Support.ImitatorRole
{

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo meetingTarget)
        {
            if (__instance == null)
            {
                return;
            }
            if (StartImitate.ImitatingPlayer != null)
            {
                PlayerControl lastExaminedPlayer = null;
                List<RoleEnum> trappedPlayers = null;
                PlayerControl confessingPlayer = null;

                if (PlayerControl.LocalPlayer == StartImitate.ImitatingPlayer)
                {
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) Footprint.DestroyAll(GetRole<Investigator>(PlayerControl.LocalPlayer));

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer))
                    {
                        var engineerRole = GetRole<Engineer>(PlayerControl.LocalPlayer);
                        Object.Destroy(engineerRole.UsesText);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
                    {
                        var trackerRole = GetRole<Tracker>(PlayerControl.LocalPlayer);
                        trackerRole.TrackerArrows.Values.DestroyAll();
                        trackerRole.TrackerArrows.Clear();
                        Object.Destroy(trackerRole.UsesText);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                    {
                        var mysticRole = GetRole<Mystic>(PlayerControl.LocalPlayer);
                        mysticRole.BodyArrows.Values.DestroyAll();
                        mysticRole.BodyArrows.Clear();
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                    {
                        var transporterRole = GetRole<Transporter>(PlayerControl.LocalPlayer);
                        Object.Destroy(transporterRole.UsesText);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                    {
                        var Mystic = GetRole<Mystic>(PlayerControl.LocalPlayer);
                        lastExaminedPlayer = Mystic.LastExaminedPlayer;
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
                    {
                        var veteranRole = GetRole<Veteran>(PlayerControl.LocalPlayer);
                        Object.Destroy(veteranRole.UsesText);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
                    {
                        var trapperRole = GetRole<Trapper>(PlayerControl.LocalPlayer);
                        Object.Destroy(trapperRole.UsesText);
                        trapperRole.traps.ClearTraps();
                        trappedPlayers = trapperRole.trappedPlayers;
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Hunter))
                    {
                        var hunterRole = Role.GetRole<Hunter>(PlayerControl.LocalPlayer);
                        Object.Destroy(hunterRole.UsesText);
                        hunterRole.ClosestPlayer = null;
                        hunterRole.ClosestStalkPlayer = null;
                        hunterRole.StalkButton.SetTarget(null);
                        hunterRole.StalkButton.gameObject.SetActive(false);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Oracle))
                    {
                        var oracleRole = GetRole<Oracle>(PlayerControl.LocalPlayer);
                        oracleRole.ClosestPlayer = null;
                        confessingPlayer = oracleRole.Confessor;
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator))
                    {
                        var detecRole = GetRole<Investigator>(PlayerControl.LocalPlayer);
                        detecRole.ClosestPlayer = null;
                        detecRole.ExamineButton.gameObject.SetActive(false);
                    }

                    if (!PlayerControl.LocalPlayer.Is(RoleEnum.Investigator) && !PlayerControl.LocalPlayer.Is(RoleEnum.Mystic)
                        ) DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
                }

                if (StartImitate.ImitatingPlayer.Is(RoleEnum.Medium))
                {
                    var medRole = GetRole<Medium>(StartImitate.ImitatingPlayer);
                    medRole.MediatedPlayers.Values.DestroyAll();
                    medRole.MediatedPlayers.Clear();
                }

                var role = GetPlayerRole(StartImitate.ImitatingPlayer);
                var killsList = (role.Kills, role.CorrectKills, role.IncorrectShots, role.CorrectShot, role.CorrectVigilanteShot, role.CorrectAssassinKills, role.IncorrectAssassinKills);
                RoleDictionary.Remove(StartImitate.ImitatingPlayer.PlayerId);
                var imitator = new Imitator(StartImitate.ImitatingPlayer);
                imitator.trappedPlayers = trappedPlayers;
                imitator.confessingPlayer = confessingPlayer;
                var newRole = GetPlayerRole(StartImitate.ImitatingPlayer);
                newRole.RemoveFromRoleHistory(newRole.RoleType);
                newRole.Kills = killsList.Kills;
                newRole.CorrectKills = killsList.CorrectKills;
                newRole.IncorrectShots = killsList.IncorrectShots;
                newRole.CorrectShot = killsList.CorrectShot;
                newRole.CorrectVigilanteShot = killsList.CorrectVigilanteShot;
                newRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
                newRole.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                GetRole<Imitator>(StartImitate.ImitatingPlayer).ImitatePlayer = null;
                StartImitate.ImitatingPlayer = null;
            }
            return;
        }
    }
}