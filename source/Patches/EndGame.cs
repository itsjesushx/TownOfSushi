using System.Collections;
using System.Text;
using TMPro;

namespace TownOfSushi.Patches
{
    static class AdditionalTempData 
    {
        public static List<PlayerRoleInfo> GameSummaryText = new List<PlayerRoleInfo>();
        public static void Clear() 
        {
            GameSummaryText.Clear();
        }
        internal class PlayerRoleInfo
        {
            public string PlayerName { get; set; }
            public string GameSummaryText { get; set; }
        }
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class OnGameEndPatch 
    {
        public static void Postfix()
        {
            // Reset zoomed out ghosts
            ToggleZoom(reset: true);
            AdditionalTempData.GameSummaryText.Clear();
            
            foreach (var playerControl in PlayerControl.AllPlayerControls)
            {
                var SummaryText = "";

                var info = playerControl.AllPlayerInfo();
                var role = info[0] as Role;
                var modifier = info[1] as Modifier;
                var ability = info[2] as Ability;
                var player = GetPlayerRole(playerControl);

                if (role != null)
                {
                    SummaryText += $"{role.ColorString}{role.Name}</color>";
                }

                if (modifier?.ModifierType != null)
                    SummaryText += $" ({modifier?.ColorString}{modifier?.Name}</color>)";

                if (ability?.AbilityType != null)
                    SummaryText += $" [{ability?.ColorString}{ability?.Name}</color>]";
                    
                if (playerControl.IsShielded())
                {
                    SummaryText += $" | {ColorString(ColorManager.Medic, $"[<b>+</b>]")}";
                }
                if (playerControl.IsFortified())
                {
                    SummaryText += $" | {ColorString(ColorManager.Crusader, $"[<b>+</b>]")}";
                }
                if (playerControl.IsBeloved())
                {
                    SummaryText += $" | {ColorString(ColorManager.Romantic, $"[♥]")}";
                }
                if (playerControl.IsGATarget())
                {
                    SummaryText += $" | {ColorString(ColorManager.GuardianAngel, $"[★]")}";
                }
                if (playerControl.IsExeTarget())
                {
                    SummaryText += $" | {ColorString(ColorManager.Executioner, $"[⦿]")}";
                }
                if (playerControl.IsSpelled())
                {
                    SummaryText += $" | {ColorString(ColorManager.ImpostorRed, $"[†]")}";
                }
                if (playerControl.Is(RoleEnum.Vulture) && !VultureWin)
                {
                    SummaryText += $" | {ColorString(ColorManager.Vulture, $" {GetRole<Vulture>(playerControl).BodiesRemainingToWin()}")}";
                }
                if (playerControl.HasTasks())
                {
                    if ((player.TotalTasks - player.TasksLeft)/player.TotalTasks == 1) SummaryText += $" | Tasks: " + ColorString(Color.green, $"{player.TotalTasks - player.TasksLeft}/{player.TotalTasks}");
                    else SummaryText += $" | Tasks: <color=#FAD934FF>{player.TotalTasks - player.TasksLeft}/{player.TotalTasks}</color>";
                }
                if (player.Kills > 0 && !playerControl.Is(Faction.Crewmates))
                {
                    SummaryText  += $" | {ColorString(Color.red, $"Kills: {player.Kills}")}";
                }
                if (player.CorrectShot > 0)
                {
                    SummaryText  += $" | {ColorString(Color.green, $"Correct Shots: {player.CorrectShot}")}";
                }
                if (player.IncorrectShots > 0)
                {
                    SummaryText  += $" | {ColorString(Color.red, $"Incorrect Shots: {player.IncorrectShots}")}";
                }
                if (player.CorrectAssassinKills > 0)
                {
                    SummaryText  += $" | {ColorString(Color.green, $"Guesses: {player.CorrectAssassinKills}")}";
                }
                if (player.CorrectVigilanteShot > 0)
                {
                    SummaryText  += $" | {ColorString(ColorManager.Vigilante, $"Correct Shots: {player.CorrectVigilanteShot}")}";
                }
                if (player.CorrectDeputyShot > 0)
                {
                    SummaryText  += $" | {ColorString(ColorManager.Deputy, $"Correct Shots: {player.CorrectDeputyShot}")}";
                }
                if (player.Misfired)
                {
                    SummaryText += $" | {ColorString(Color.red, $"Misfired")}";
                }
                if (player.CorrectKills > 0)
                {
                    SummaryText += $" | {ColorString(Color.green, $"Correct Kills: {player.CorrectKills}")}";
                }
                if (playerControl.Data.IsDead) 
                {
                    SummaryText += $" | {playerControl.GetDeadInfo()}";
                }

                var info2 = new AdditionalTempData.PlayerRoleInfo()
                {
                    PlayerName = playerControl.Data.PlayerName,
                    GameSummaryText = SummaryText
                };
                AdditionalTempData.GameSummaryText.Add(info2);
                }
            }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public class EndGameManagerSetUpPatch 
    {
        public static void Postfix(EndGameManager __instance) 
        {
            var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
            GameObject roleSummary = Object.Instantiate(__instance.WinText.gameObject);
            roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -214f); 
            roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);
            var roleSummaryText = new StringBuilder();
            var winnersText = new StringBuilder();
            var winnersCache = new StringBuilder();
            var losersText = new StringBuilder();
            var winnerCount = 0;
            var loserCount = 0;

            roleSummaryText.AppendLine("<size=125%><u><b>Game Stats</b></u>:</size>");
            roleSummaryText.AppendLine();
            winnersText.AppendLine("<size=105%><color=#00FF00FF><b>★ - Winners - ★</b></color></size>");
            losersText.AppendLine("<size=105%><color=#FF0000FF><b>◆ - Losers - ◆</b></color></size>");

            foreach (var data in AdditionalTempData.GameSummaryText)
            {
                var dataString = $"<size=70%>{data.PlayerName} - {data.GameSummaryText}</size>";

                if (data.PlayerName.IsWinner())
                {
                    winnersText.AppendLine(dataString);
                    winnerCount++;
                }
                else
                {
                    losersText.AppendLine(dataString);
                    loserCount++;
                }
            }

            if (winnerCount == 0)
            {
                winnersText.AppendLine("<size=95%>No One Won</size>");
                winnersCache.AppendLine("No One Won");
            }

            if (loserCount == 0)
            {
                losersText.AppendLine("<size=95%>No One Lost</size>");
            }

            roleSummaryText.Append(winnersText);
            roleSummaryText.AppendLine();
            roleSummaryText.Append(losersText);

            var roleSummaryTextMesh = roleSummary.GetComponent<TMP_Text>();
            roleSummaryTextMesh.alignment = TextAlignmentOptions.TopLeft;
            roleSummaryTextMesh.color = Color.white;
            roleSummaryTextMesh.fontSizeMin = 1f;
            roleSummaryTextMesh.fontSizeMax = 1f;
            roleSummaryTextMesh.fontSize = 1f;
            roleSummaryTextMesh.text = $"{roleSummaryText}";
            roleSummaryTextMesh.GetComponent<RectTransform>().anchoredPosition = new(position.x + 3.5f, position.y - 0.1f);
            AdditionalTempData.Clear();
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public class EndGameManagerSetWinnerPatch
    {
        public static void Postfix()
        {
            if (!AmongUsClient.Instance.AmHost || IsHideNSeek() || AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
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
                var ImpostorsAlive = AllPlayers().Where(x => x.Is(Faction.Impostors) && !x.Data.IsDead && !x.Data.Disconnected && !AssassinExileControllerPatch.AssassinatedPlayers.Contains(x)).ToList();
                var AliveKillers = AllPlayers().Where(x => x.IsKillingRole() && !x.Data.IsDead && !x.Data.Disconnected && !AssassinExileControllerPatch.AssassinatedPlayers.Contains(x)).ToList();
                var PassiveAlive = AllPlayers().Where(x => !x.Data.IsDead && !x.Data.Disconnected && !AssassinExileControllerPatch.AssassinatedPlayers.Contains(x)).ToList();
                var CrewKillerAlive = AllPlayers().Where(x => x.IsCrewKiller() && !x.Data.IsDead && !x.Data.Disconnected && !AssassinExileControllerPatch.AssassinatedPlayers.Contains(x)).ToList();
                var GlitchAlive = AllPlayers().Where(x => x.Is(RoleEnum.Glitch) && !x.Data.IsDead && !x.Data.Disconnected && !AssassinExileControllerPatch.AssassinatedPlayers.Contains(x)).ToList();
                var HitmanAlive = AllPlayers().Where(x => x.Is(RoleEnum.Hitman) && !x.Data.IsDead && !x.Data.Disconnected && !AssassinExileControllerPatch.AssassinatedPlayers.Contains(x)).ToList();
                var AgentAlive = AllPlayers().Where(x => x.Is(RoleEnum.Agent) && !x.Data.IsDead && !x.Data.Disconnected && !AssassinExileControllerPatch.AssassinatedPlayers.Contains(x)).ToList();
                var ArsoAlive = AllPlayers().Where(x => x.Is(RoleEnum.Arsonist) && !x.Data.IsDead && !x.Data.Disconnected && !AssassinExileControllerPatch.AssassinatedPlayers.Contains(x)).ToList();
                var PlaguebearerAlive = AllPlayers().Where(x => x.Is(RoleEnum.Plaguebearer) && !x.Data.IsDead && !x.Data.Disconnected && !AssassinExileControllerPatch.AssassinatedPlayers.Contains(x)).ToList();
                var JuggernautAlive = AllPlayers().Where(x => x.Is(RoleEnum.Juggernaut) && !x.Data.IsDead && !x.Data.Disconnected && !AssassinExileControllerPatch.AssassinatedPlayers.Contains(x)).ToList();
                var PestilenceAlive = AllPlayers().Where(x => x.Is(RoleEnum.Pestilence) && !x.Data.IsDead && !x.Data.Disconnected && !AssassinExileControllerPatch.AssassinatedPlayers.Contains(x)).ToList();
                var WerewolfAlive = AllPlayers().Where(x => x.Is(RoleEnum.Werewolf) && !x.Data.IsDead && !x.Data.Disconnected && !AssassinExileControllerPatch.AssassinatedPlayers.Contains(x)).ToList();
                var VampiresAlive = AllPlayers().Where(x => x.Is(RoleEnum.Vampire) && !x.Data.IsDead && !x.Data.Disconnected && !AssassinExileControllerPatch.AssassinatedPlayers.Contains(x)).ToList();
                var AliveSerialKiller = AllPlayers().Where(x => x.Is(RoleEnum.SerialKiller) && !x.Data.IsDead && !x.Data.Disconnected && !AssassinExileControllerPatch.AssassinatedPlayers.Contains(x)).ToList();

                if (ImpostorsAlive.Count >= PassiveAlive.Count - ImpostorsAlive.Count &&
                    PlaguebearerAlive.Count == 0 &&
                    GlitchAlive.Count == 0 &&
                    PestilenceAlive.Count == 0 &&
                    VampiresAlive.Count == 0 &&
                    ArsoAlive.Count == 0 &&
                    JuggernautAlive.Count == 0 &&
                    HitmanAlive.Count == 0 &&
                    AliveSerialKiller.Count == 0 &&
                    WerewolfAlive.Count == 0 &&
                    CrewKillerAlive.Count == 0)
                    {
                        ImpostorsWin = true;
                        StartRPC(CustomRPC.ImpostorWin);
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
                        StartRPC(CustomRPC.HitmanWin);
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
                        StartRPC(CustomRPC.AgentWin);
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
                        StartRPC(CustomRPC.GlitchWin);
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
                        StartRPC(CustomRPC.ArsonistWin);
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
                        StartRPC(CustomRPC.VampireWin);
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
                        StartRPC(CustomRPC.PlaguebearerWin);
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
                        StartRPC(CustomRPC.PestilenceWin);
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
                        StartRPC(CustomRPC.JuggernautWin);
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
                        StartRPC(CustomRPC.SerialKillerWin);
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
                        StartRPC(CustomRPC.WerewolfWin);
                        EndGame();
                        yield break;
                    }

                if (AliveKillers.Count == 0)
                {
                    CrewmatesWin = true;
                    StartRPC(CustomRPC.CrewmateWin);
                    EndGame(GameOverReason.HumansByVote);
                    yield break;
                }
            }
        }
    }
}