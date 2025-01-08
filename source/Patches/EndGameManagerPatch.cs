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

        public static IEnumerator CheckForEndGame()
        {
            yield return new WaitForSeconds(1f);

            foreach (var role in AllRoles)
            {
                if (NeutralEvilWin())
                {
                    yield break;
                }

                var ImpostorsAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Impostors) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var NeutralKillerAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleAlignment.NeutralKilling) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var AliveKillers = PlayerControl.AllPlayerControls.ToArray().Where(x => x.IsKillingRole() && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var PassiveAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors) && !x.Is(RoleAlignment.NeutralKilling) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var CrewKillerAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.IsCrewKiller() && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var GlitchAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Glitch) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var HitmanAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Hitman) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var AgentAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Agent) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var ArsoAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Arsonist) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var PlaguebearerAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Plaguebearer) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var JuggernautAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Juggernaut) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var PestilenceAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Pestilence) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var WerewolfAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Werewolf) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var VampiresAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Vampire) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();
                var AliveSerialKiller = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.SerialKiller) && !x.Data.IsDead && !x.Data.Disconnected && !AddHauntPatch.AssassinatedPlayers.Contains(x)).ToList();

                if (ImpostorsAlive.Count  >= PassiveAlive.Count && 
                    NeutralKillerAlive.Count == 0 && 
                    CrewKillerAlive.Count == 0)
                    {
                        ImpostorsWin = true;
                        Rpc(CustomRPC.ImpostorWin);
                        EndGame(GameOverReason.ImpostorByVote);
                        yield break;
                    }
                
                if (HitmanAlive.Count >= PassiveAlive.Count - HitmanAlive.Count && 
                    PlaguebearerAlive.Count == 0 &&
                    GlitchAlive.Count == 0 &&
                    PestilenceAlive.Count == 0 &&
                    VampiresAlive.Count == 0 &&
                    ArsoAlive.Count == 0 &&
                    JuggernautAlive.Count == 0 &&
                    ImpostorsAlive.Count == 0 &&
                    AliveSerialKiller.Count == 0 &&
                    WerewolfAlive.Count == 0 &&
                    CrewKillerAlive.Count == 0)
                    {
                        HitmanWin = true;
                        Rpc(CustomRPC.HitmanWin);
                        EndGame();
                        yield break;
                    }

                
                if (AgentAlive.Count >= PassiveAlive.Count - AgentAlive.Count && 
                    PlaguebearerAlive.Count == 0 &&
                    GlitchAlive.Count == 0 &&
                    PestilenceAlive.Count == 0 &&
                    VampiresAlive.Count == 0 &&
                    ArsoAlive.Count == 0 &&
                    JuggernautAlive.Count == 0 &&
                    ImpostorsAlive.Count == 0 &&
                    AliveSerialKiller.Count == 0 &&
                    WerewolfAlive.Count == 0 &&
                    CrewKillerAlive.Count == 0)
                    {
                        AgentWin = true;
                        Rpc(CustomRPC.AgentWin);
                        EndGame();
                        yield break;
                    }

                if (GlitchAlive.Count >= PassiveAlive.Count - GlitchAlive.Count && 
                    PlaguebearerAlive.Count == 0 &&
                    HitmanAlive.Count == 0 &&
                    AgentAlive.Count == 0 &&
                    PestilenceAlive.Count == 0 &&
                    VampiresAlive.Count == 0 &&
                    ArsoAlive.Count == 0 &&
                    JuggernautAlive.Count == 0 &&
                    ImpostorsAlive.Count == 0 &&
                    AliveSerialKiller.Count == 0 &&
                    WerewolfAlive.Count == 0 &&
                    CrewKillerAlive.Count == 0)
                    {
                        GlitchWin = true;
                        Rpc(CustomRPC.GlitchWin);
                        EndGame();
                        yield break;
                    }

                
                if (ArsoAlive.Count >= PassiveAlive.Count - ArsoAlive.Count && 
                    PlaguebearerAlive.Count == 0 &&
                    HitmanAlive.Count == 0 &&
                    AgentAlive.Count == 0 &&
                    PestilenceAlive.Count == 0 &&
                    VampiresAlive.Count == 0 &&
                    GlitchAlive.Count == 0 &&
                    JuggernautAlive.Count == 0 &&
                    ImpostorsAlive.Count == 0 &&
                    AliveSerialKiller.Count == 0 &&
                    WerewolfAlive.Count == 0 &&
                    CrewKillerAlive.Count == 0)
                    {
                        ArsonistWin = true;
                        Rpc(CustomRPC.ArsonistWin);
                        EndGame();
                        yield break;
                    }

                if (VampiresAlive.Count >= PassiveAlive.Count - VampiresAlive.Count && 
                    PlaguebearerAlive.Count == 0 &&
                    HitmanAlive.Count == 0 &&
                    AgentAlive.Count == 0 &&
                    PestilenceAlive.Count == 0 &&
                    ArsoAlive.Count == 0 &&
                    GlitchAlive.Count == 0 &&
                    JuggernautAlive.Count == 0 &&
                    ImpostorsAlive.Count == 0 &&
                    AliveSerialKiller.Count == 0 &&
                    WerewolfAlive.Count == 0 &&
                    CrewKillerAlive.Count == 0)
                    {
                        VampireWins = true;
                        Rpc(CustomRPC.TeamVampiresWin);
                        EndGame();
                        yield break;
                    }

                
                if (PlaguebearerAlive.Count >= PassiveAlive.Count - PlaguebearerAlive.Count && 
                    GlitchAlive.Count == 0 &&
                    HitmanAlive.Count == 0 &&
                    AgentAlive.Count == 0 &&
                    VampiresAlive.Count == 0 &&
                    ArsoAlive.Count == 0 &&
                    JuggernautAlive.Count == 0 &&
                    ImpostorsAlive.Count == 0 &&
                    AliveSerialKiller.Count == 0 &&
                    WerewolfAlive.Count == 0 &&
                    CrewKillerAlive.Count == 0)
                    {
                        PlaguebearerWin = true;
                        Rpc(CustomRPC.PlaguebearerWin);
                        EndGame();
                        yield break;
                    }
                
                if (PestilenceAlive.Count >= PassiveAlive.Count - PestilenceAlive.Count && 
                    HitmanAlive.Count == 0 &&
                    AgentAlive.Count == 0 &&
                    VampiresAlive.Count == 0 &&
                    ArsoAlive.Count == 0 &&
                    JuggernautAlive.Count == 0 &&
                    ImpostorsAlive.Count == 0 &&
                    AliveSerialKiller.Count == 0 &&
                    WerewolfAlive.Count == 0 &&
                    CrewKillerAlive.Count == 0)
                    {
                        PestilenceWin = true;
                        Rpc(CustomRPC.PestilenceWin);
                        EndGame();
                        yield break;
                    }

                
                if (JuggernautAlive.Count >= PassiveAlive.Count - JuggernautAlive.Count && 
                    PlaguebearerAlive.Count == 0 &&
                    HitmanAlive.Count == 0 &&
                    AgentAlive.Count == 0 &&
                    PestilenceAlive.Count == 0 &&
                    VampiresAlive.Count == 0 &&
                    GlitchAlive.Count == 0 &&
                    ArsoAlive.Count == 0 &&
                    ImpostorsAlive.Count == 0 &&
                    AliveSerialKiller.Count == 0 &&
                    WerewolfAlive.Count == 0 &&
                    CrewKillerAlive.Count == 0)
                    {
                        JuggernautWin = true;
                        Rpc(CustomRPC.JuggernautWin);
                        EndGame();
                        yield break;
                    }
                
                if (AliveSerialKiller.Count >= PassiveAlive.Count - AliveSerialKiller.Count && 
                    PlaguebearerAlive.Count == 0 &&
                    HitmanAlive.Count == 0 &&
                    AgentAlive.Count == 0 &&
                    PestilenceAlive.Count == 0 &&
                    VampiresAlive.Count == 0 &&
                    GlitchAlive.Count == 0 &&
                    ArsoAlive.Count == 0 &&
                    ImpostorsAlive.Count == 0 &&
                    JuggernautAlive.Count == 0 &&
                    WerewolfAlive.Count == 0 &&
                    CrewKillerAlive.Count == 0)
                    {
                        SerialKillerWin = true;
                        Rpc(CustomRPC.SerialKillerWin);
                        EndGame();
                        yield break; 
                    }

                
                if (WerewolfAlive.Count >= PassiveAlive.Count - WerewolfAlive.Count && 
                    PlaguebearerAlive.Count == 0 &&
                    HitmanAlive.Count == 0 &&
                    AgentAlive.Count == 0 &&
                    PestilenceAlive.Count == 0 &&
                    VampiresAlive.Count == 0 &&
                    GlitchAlive.Count == 0 &&
                    ArsoAlive.Count == 0 &&
                    ImpostorsAlive.Count == 0 &&
                    AliveSerialKiller.Count == 0 &&
                    JuggernautAlive.Count == 0 &&
                    CrewKillerAlive.Count == 0)
                    {
                        WerewolfWin = true;
                        Rpc(CustomRPC.WerewolfWin);
                        EndGame();
                        yield break;
                    }

                if (AliveKillers.Count == 0)
                {
                    CrewmatesWin = true;
                    Rpc(CustomRPC.CrewmateWin);
                    EndGame(GameOverReason.HumansByVote);
                    yield break;
                }
            }
        }
    }
}
