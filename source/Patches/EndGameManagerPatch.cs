using System.Collections;

namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public class EndGameManagerPatch
    {
        public static void Postfix()
        {
            if (!AmongUsClient.Instance.AmHost || GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek || AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            Coroutines.Start(CheckForEndGame());
        }

        public static void CheckVampireWin()
        {
            var Player = GetRole<Vampire>(PlayerControl.LocalPlayer).Player;

            if (Player.Data.IsDead || Player.Data.Disconnected) return;

            var CrewKillerAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && x.IsCrewKiller() && !x.Data.IsDead && !x.Data.Disconnected).ToList();

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(RoleAlignment.NeutralKilling))) == 1 && CrewKillerAlive.Count <= 0)
            {
                VampireWins = true;
                Rpc(CustomRPC.TeamVampiresWin);
                EndGame();
                return;
            }
            else if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 4 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(RoleAlignment.NeutralKilling)) && !x.Is(RoleEnum.Vampire)) == 0 && CrewKillerAlive.Count <= 0)
            {
                var vampsAlives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(RoleEnum.Vampire)).ToList();
                if (vampsAlives.Count == 1) return;
                VampireWins = true;
                Rpc(CustomRPC.TeamVampiresWin);
                EndGame();
                return;
            }
            else
            {
                var VampsAlives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(RoleEnum.Vampire)).ToList();
                if (VampsAlives.Count == 1 || VampsAlives.Count == 2) return;
                var alives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                var KillersAlive = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(RoleEnum.Vampire) && (x.Is(Faction.Impostors) || x.Is(RoleAlignment.NeutralKilling))).ToList();
                if (KillersAlive.Count > 0) return;
                if (CrewKillerAlive.Count > 0) return;
                if (alives.Count <= 6)
                {
                    VampireWins = true;
                    Rpc(CustomRPC.TeamVampiresWin);
                    EndGame();
                    return;
                }
                return;
            }
        }

        public static IEnumerator CheckForEndGame()
        {
            yield return new WaitForSeconds(1f);

            foreach (var role in AllRoles)
            {
                if (NeutralEvilWin())
                {
                    yield break;
                }

                var ImpostorsAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => (x.Is(Faction.Impostors)) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var NeutralKillerAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleAlignment.NeutralKilling) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var AliveKillers = PlayerControl.AllPlayerControls.ToArray().Where(x => x.IsKillingRole() && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var PassiveAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors) && !x.Is(RoleAlignment.NeutralKilling) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var CrewKillerAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.IsCrewKiller() && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                
                if (NeutralKillerAlive.Count <= 0 && PassiveAlive.Count <= ImpostorsAlive.Count && CrewKillerAlive.Count <= 0)
                {
                    ImpostorsWin = true;
                    Rpc(CustomRPC.ImpostorWin);
                    EndGame(GameOverReason.ImpostorByVote);
                    yield break;
                }

                else if (NeutralKillerAlive.Count <= 1 && PassiveAlive.Count <= NeutralKillerAlive.Count && ImpostorsAlive.Count <= 0 && CrewKillerAlive.Count <= 0)
                {
                    var roleAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Hitman) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                    if (PassiveAlive.Count <= roleAlive.Count && roleAlive.Count > 0)
                    {
                        HitmanWin = true;
                        Rpc(CustomRPC.HitmanWin);
                        EndGame();
                        yield break;
                    }
                }

                else if (NeutralKillerAlive.Count <= 1 && PassiveAlive.Count <= NeutralKillerAlive.Count && ImpostorsAlive.Count <= 0 && CrewKillerAlive.Count <= 0)
                {
                    var roleAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Agent) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                    if (PassiveAlive.Count <= roleAlive.Count && roleAlive.Count > 0)
                    {
                        AgentWin = true;
                        Rpc(CustomRPC.AgentWin);
                        EndGame();
                        yield break;
                    }
                }

                else if (NeutralKillerAlive.Count <= 1 && PassiveAlive.Count <= NeutralKillerAlive.Count && ImpostorsAlive.Count <= 0 && CrewKillerAlive.Count <= 0)
                {
                    var roleAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Glitch) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                    if (PassiveAlive.Count <= roleAlive.Count && roleAlive.Count > 0)
                    {
                        GlitchWin = true;
                        Rpc(CustomRPC.GlitchWin);
                        EndGame();
                        yield break;
                    }
                }

                else if (NeutralKillerAlive.Count <= 1 && PassiveAlive.Count <= NeutralKillerAlive.Count && ImpostorsAlive.Count <= 0 && CrewKillerAlive.Count <= 0)
                {
                    var roleAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Arsonist) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                    if (PassiveAlive.Count <= roleAlive.Count && roleAlive.Count > 0)
                    {
                        ArsonistWin = true;
                        Rpc(CustomRPC.ArsonistWin);
                        EndGame();
                        yield break;
                    }
                }

                else if (NeutralKillerAlive.Count <= 1 && PassiveAlive.Count <= NeutralKillerAlive.Count && ImpostorsAlive.Count <= 0 && CrewKillerAlive.Count <= 0)
                {
                    var roleAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Plaguebearer) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                    if (PassiveAlive.Count <= roleAlive.Count && roleAlive.Count > 0)
                    {
                        PlaguebearerWin = true;
                        Rpc(CustomRPC.PlaguebearerWin);
                        EndGame();
                        yield break;
                    }
                }

                else if (NeutralKillerAlive.Count <= 1 && PassiveAlive.Count <= NeutralKillerAlive.Count && ImpostorsAlive.Count <= 0 && CrewKillerAlive.Count <= 0)
                {
                    var roleAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Pestilence) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                    if (PassiveAlive.Count <= roleAlive.Count && roleAlive.Count > 0)
                    {
                        PestilenceWin = true;
                        Rpc(CustomRPC.PestilenceWin);
                        EndGame();
                        yield break;
                    }
                }

                else if (NeutralKillerAlive.Count <= 1 && PassiveAlive.Count <= NeutralKillerAlive.Count && ImpostorsAlive.Count <= 0 && CrewKillerAlive.Count <= 0)
                {
                    var roleAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Juggernaut) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                    if (PassiveAlive.Count <= roleAlive.Count && roleAlive.Count > 0)
                    {
                        JuggernautWin = true;
                        Rpc(CustomRPC.JuggernautWin);
                        EndGame();
                        yield break;
                    }
                }

                else if (NeutralKillerAlive.Count <= 1 && PassiveAlive.Count <= NeutralKillerAlive.Count && ImpostorsAlive.Count <= 0 && CrewKillerAlive.Count <= 0)
                {
                    var roleAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Plaguebearer) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                    if (PassiveAlive.Count <= roleAlive.Count && roleAlive.Count > 0)
                    {
                        PlaguebearerWin = true;
                        Rpc(CustomRPC.PlaguebearerWin);
                        EndGame();
                        yield break;
                    }
                }

                else if (NeutralKillerAlive.Count <= 1 && PassiveAlive.Count <= NeutralKillerAlive.Count && ImpostorsAlive.Count <= 0 && CrewKillerAlive.Count <= 0)
                {
                    var roleAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.SerialKiller) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                    if (PassiveAlive.Count <= roleAlive.Count && roleAlive.Count > 0)
                    {
                        SerialKillerWin = true;
                        Rpc(CustomRPC.SerialKillerWin);
                        EndGame();
                        yield break;
                    }
                }

                else if (NeutralKillerAlive.Count <= 1 && PassiveAlive.Count <= NeutralKillerAlive.Count && ImpostorsAlive.Count <= 0 && CrewKillerAlive.Count <= 0)
                {
                    var roleAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Werewolf) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                    if (PassiveAlive.Count <= roleAlive.Count && roleAlive.Count > 0)
                    {
                        WerewolfWin = true;
                        Rpc(CustomRPC.WerewolfWin);
                        EndGame();
                        yield break;
                    }
                }

                else if (AliveKillers.Count <= 0)
                {
                    CrewmatesWin = true;
                    Rpc(CustomRPC.CrewmateWin);
                    EndGame();
                    yield break;
                }

                else 
                {
                    CheckVampireWin();
                }
            }
        }
    }
}
