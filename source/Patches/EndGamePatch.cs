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
        internal class Winners
        {
            public string PlayerName { get; set; }
            public RoleEnum Role { get; set; }
        }
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class OnGameEndPatch 
    {
        public static GameOverReason GameOverReason;
        public static void Prefix(AmongUsClient __instance, [HarmonyArgument(0)]ref EndGameResult endGameResult) 
        {
            GameOverReason = endGameResult.GameOverReason;
            if ((int)endGameResult.GameOverReason >= 10) endGameResult.GameOverReason = GameOverReason.ImpostorByKill;
            // Reset zoomed out ghosts
            ToggleZoom(reset: true);
        }

        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] EndGameResult endGameResult)
        {
            if (CameraEffect.singleton) CameraEffect.singleton.materials.Clear();
            AdditionalTempData.Clear();
            var GameSummaryText = "";
            foreach (var playerControl in PlayerControl.AllPlayerControls) 
            {
                GameSummaryText = "";
                foreach (var role in RoleHistory.Where(x => x.Key == playerControl.PlayerId))
                {
                    // Crewmate Roles

                    if (role.Value == RoleEnum.Crewmate) { GameSummaryText += "<color=#" + Colors.Crewmate.ToHtmlStringRGBA() + ">Crewmate</color> > "; }
                    else if (role.Value == RoleEnum.Mystic) { GameSummaryText += "<color=#" + Colors.Mystic.ToHtmlStringRGBA() + ">Mystic</color> > "; }
                    else if (role.Value == RoleEnum.Engineer) { GameSummaryText += "<color=#" + Colors.Engineer.ToHtmlStringRGBA() + ">Engineer</color> > "; }
                    else if (role.Value == RoleEnum.Investigator) { GameSummaryText += "<color=#" + Colors.Investigator.ToHtmlStringRGBA() + ">Investigator</color> > "; }
                    else if (role.Value == RoleEnum.Medic) { GameSummaryText += "<color=#" + Colors.Medic.ToHtmlStringRGBA() + ">Medic</color> > "; }
                    else if (role.Value == RoleEnum.Vigilante) { GameSummaryText += "<color=#" + Colors.Vigilante.ToHtmlStringRGBA() + ">Vigilante</color> > "; }
                    else if (role.Value == RoleEnum.Guardian) { GameSummaryText += "<color=#" + Colors.Guardian.ToHtmlStringRGBA() + ">Guardian</color> > "; }
                    else if (role.Value == RoleEnum.Seer) { GameSummaryText += "<color=#" + Colors.Seer.ToHtmlStringRGBA() + ">Seer</color> > "; }
                    else if (role.Value == RoleEnum.Jailor) { GameSummaryText += "<color=#" + Colors.Jailor.ToHtmlStringRGBA() + ">Jailor</color> > "; }
                    else if (role.Value == RoleEnum.Oracle) { GameSummaryText += "<color=#" + Colors.Oracle.ToHtmlStringRGBA() + ">Oracle</color> > "; }
                    else if (role.Value == RoleEnum.Swapper) { GameSummaryText += "<color=#" + Colors.Swapper.ToHtmlStringRGBA() + ">Swapper</color> > "; }
                    else if (role.Value == RoleEnum.Imitator) { GameSummaryText += "<color=#" + Colors.Imitator.ToHtmlStringRGBA() + ">Imitator</color> > "; }
                    else if (role.Value == RoleEnum.Vigilante) { GameSummaryText += "<color=#" + Colors.Vigilante.ToHtmlStringRGBA() + ">Vigilante</color> > "; }
                    else if (role.Value == RoleEnum.Tracker) { GameSummaryText += "<color=#" + Colors.Tracker.ToHtmlStringRGBA() + ">Tracker</color> > "; }
                    else if (role.Value == RoleEnum.Medium) { GameSummaryText += "<color=#" + Colors.Medium.ToHtmlStringRGBA() + ">Medium</color> > "; }
                    else if (role.Value == RoleEnum.Trapper) { GameSummaryText += "<color=#" + Colors.Trapper.ToHtmlStringRGBA() + ">Trapper</color> > "; }
                    else if (role.Value == RoleEnum.Veteran) { GameSummaryText += "<color=#" + Colors.Veteran.ToHtmlStringRGBA() + ">Veteran</color> > "; }
                    else if (role.Value == RoleEnum.Hunter) { GameSummaryText += "<color=#" + Colors.Hunter.ToHtmlStringRGBA() + ">Hunter</color> > "; }

                    // Impostor Roles

                    else if (role.Value == RoleEnum.Impostor) { GameSummaryText += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Impostor</color> > "; }
                    else if (role.Value == RoleEnum.Grenadier) { GameSummaryText += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Grenadier</color> > "; }
                    else if (role.Value == RoleEnum.Janitor) { GameSummaryText += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Janitor</color> > "; }
                    else if (role.Value == RoleEnum.Miner) { GameSummaryText += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Miner</color> > "; }
                    else if (role.Value == RoleEnum.Bomber) { GameSummaryText += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Bomber</color> > "; }
                    else if (role.Value == RoleEnum.Witch) { GameSummaryText += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Witch</color> > "; }
                    else if (role.Value == RoleEnum.Morphling) { GameSummaryText += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Morphling</color> > "; }
                    else if (role.Value == RoleEnum.Swooper) { GameSummaryText += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Swooper</color> > "; }
                    else if (role.Value == RoleEnum.Undertaker) { GameSummaryText += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Undertaker</color> > "; }
                    else if (role.Value == RoleEnum.Blackmailer) { GameSummaryText += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Blackmailer</color> > "; }
                    else if (role.Value == RoleEnum.Venerer) { GameSummaryText += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Venerer</color> > "; }
                    else if (role.Value == RoleEnum.Warlock) { GameSummaryText += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Warlock</color> > "; }
                    else if (role.Value == RoleEnum.Escapist) { GameSummaryText += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Escapist</color> > "; }

                    // Neutral Benign Roles

                    else if (role.Value == RoleEnum.Amnesiac) { GameSummaryText += "<color=#" + Colors.Amnesiac.ToHtmlStringRGBA() + ">Amnesiac</color> > "; }
                    else if (role.Value == RoleEnum.Romantic) { GameSummaryText += "<color=#" + Colors.Romantic.ToHtmlStringRGBA() + ">Romantic</color> > "; }
                    else if (role.Value == RoleEnum.GuardianAngel) { GameSummaryText += "<color=#" + Colors.GuardianAngel.ToHtmlStringRGBA() + ">Guardian Angel</color> > "; }

                    // Neutral Evil Roles

                    else if (role.Value == RoleEnum.Vulture) { GameSummaryText += "<color=#" + Colors.Vulture.ToHtmlStringRGBA() + ">Vulture</color> > "; }
                    else if (role.Value == RoleEnum.Doomsayer) { GameSummaryText += "<color=#" + Colors.Doomsayer.ToHtmlStringRGBA() + ">Doomsayer</color> > "; }
                    else if (role.Value == RoleEnum.Framer) { GameSummaryText += "<color=#" + Colors.Framer.ToHtmlStringRGBA() + ">Framer</color> > "; }
                    else if (role.Value == RoleEnum.Jester) { GameSummaryText += "<color=#" + Colors.Jester.ToHtmlStringRGBA() + ">Jester</color> > "; }
                    else if (role.Value == RoleEnum.Executioner) { GameSummaryText += "<color=#" + Colors.Executioner.ToHtmlStringRGBA() + ">Executioner</color> > "; }

                    // Neutral Killing Roles

                    else if (role.Value == RoleEnum.Juggernaut) { GameSummaryText += "<color=#" + Colors.Juggernaut.ToHtmlStringRGBA() + ">Juggernaut</color> > "; }
                    else if (role.Value == RoleEnum.Arsonist) { GameSummaryText += "<color=#" + Colors.Arsonist.ToHtmlStringRGBA() + ">Arsonist</color> > "; }
                    else if (role.Value == RoleEnum.Plaguebearer) { GameSummaryText += "<color=#" + Colors.Plaguebearer.ToHtmlStringRGBA() + ">Plaguebearer</color> > "; }
                    else if (role.Value == RoleEnum.Pestilence) { GameSummaryText += "<color=#" + Colors.Pestilence.ToHtmlStringRGBA() + ">Pestilence</color> > "; }
                    else if (role.Value == RoleEnum.SerialKiller) { GameSummaryText += "<color=#" + Colors.SerialKiller.ToHtmlStringRGBA() + ">Serial Killer</color> > "; }
                    else if (role.Value == RoleEnum.Vampire) { GameSummaryText += "<color=#" + Colors.Vampire.ToHtmlStringRGBA() + ">Vampire</color> > "; }
                    else if (role.Value == RoleEnum.Agent) { GameSummaryText += "<color=#" + Colors.Agent.ToHtmlStringRGBA() + ">Agent</color> > "; }
                    else if (role.Value == RoleEnum.Hitman) { GameSummaryText += "<color=#" + Colors.Hitman.ToHtmlStringRGBA() + ">Hitman</color> > "; }
                    else if (role.Value == RoleEnum.Werewolf) { GameSummaryText += "<color=#" + Colors.Werewolf.ToHtmlStringRGBA() + ">Werewolf</color> > "; }
                    else if (role.Value == RoleEnum.Glitch) { GameSummaryText += "<color=#" + Colors.Glitch.ToHtmlStringRGBA() + ">Glitch</color> > "; }
                }
                
                if (!string.IsNullOrEmpty(GameSummaryText)) GameSummaryText = GameSummaryText.Remove(GameSummaryText.Length - 3);
                
                // Modifiers
                if (playerControl.Is(ModifierEnum.Giant)) { GameSummaryText += " (<color=#" + Colors.Giant.ToHtmlStringRGBA() + ">Giant</color>)"; }
                else if (playerControl.Is(ModifierEnum.Aftermath)) { GameSummaryText += " (<color=#" + Colors.Aftermath.ToHtmlStringRGBA() + ">Aftermath</color>)"; }
                else if (playerControl.Is(ModifierEnum.Mini)) { GameSummaryText += " (<color=#" + Colors.Mini.ToHtmlStringRGBA() + ">Mini</color>)"; }
                else if (playerControl.Is(ModifierEnum.Bait)) { GameSummaryText += " (<color=#" + Colors.Bait.ToHtmlStringRGBA() + ">Bait</color>)";}
                else if (playerControl.Is(ModifierEnum.Diseased)) { GameSummaryText += " (<color=#" + Colors.Diseased.ToHtmlStringRGBA() + ">Diseased</color>)";}
                else if (playerControl.Is(ModifierEnum.Disperser)) { GameSummaryText += " (<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Disperser</color>)";}
                else if (playerControl.Is(ModifierEnum.DoubleShot)) { GameSummaryText += " (<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Double Shot</color>)";}
                else if (playerControl.Is(ModifierEnum.Underdog)) { GameSummaryText += " (<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Underdog</color>)";}
                else if (playerControl.Is(ModifierEnum.Frosty)) { GameSummaryText += " (<color=#" + Colors.Frosty.ToHtmlStringRGBA() + ">Frosty</color>)";}

                // Abilities

                if (playerControl.Is(AbilityEnum.Drunk)) { GameSummaryText += " [<color=#" + Colors.Drunk.ToHtmlStringRGBA() + ">Drunk</color>] ";}
                else if (playerControl.Is(AbilityEnum.Chameleon)) { GameSummaryText += " [<color=#" + Colors.Chameleon.ToHtmlStringRGBA() + ">Chameleon</color>] ";}
                else if (playerControl.Is(AbilityEnum.Assassin)) { GameSummaryText += " [<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Assassin</color>] ";}
                else if (playerControl.Is(AbilityEnum.Flash)) { GameSummaryText += " [<color=#" + Colors.Flash.ToHtmlStringRGBA() + ">Flash</color>] ";}
                else if (playerControl.Is(AbilityEnum.Multitasker)) { GameSummaryText += " [<color=#" + Colors.Multitasker.ToHtmlStringRGBA() + ">Multitasker</color>] ";}
                else if (playerControl.Is(AbilityEnum.ButtonBarry)) { GameSummaryText += " [<color=#" + Colors.ButtonBarry.ToHtmlStringRGBA() + ">Button Barry</color>] ";}
                else if (playerControl.Is(AbilityEnum.Tiebreaker)) { GameSummaryText += " [<color=#" + Colors.Tiebreaker.ToHtmlStringRGBA() + ">Tiebreaker</color>] "; }
                else if (playerControl.Is(AbilityEnum.Spy)) { GameSummaryText += " [<color=#" + Colors.Spy.ToHtmlStringRGBA() + ">Spy</color>] ";}
                else if (playerControl.Is(AbilityEnum.Torch)) { GameSummaryText += " [<color=#" + Colors.Torch.ToHtmlStringRGBA() + ">Torch</color>] ";}
                else if (playerControl.Is(AbilityEnum.Sleuth)) { GameSummaryText += " [<color=#" + Colors.Sleuth.ToHtmlStringRGBA() + ">Sleuth</color>] ";}
                else if (playerControl.Is(AbilityEnum.Radar)) { GameSummaryText += " [<color=#" + Colors.Radar.ToHtmlStringRGBA() + ">Radar</color>] ";}
                
                
                
                // Stats

                var player = GetPlayerRole(playerControl);
                if (playerControl.IsShielded())
                {
                    GameSummaryText += ColorString(Colors.Medic, $" | [<b>+</b>] ");
                }
                if (playerControl.IsBeloved() || playerControl.IsRomantic())
                {
                    GameSummaryText += ColorString(Colors.Romantic, $" | [♥] ");
                }
                if (playerControl.IsGATarget())
                {
                    GameSummaryText += ColorString(Colors.GuardianAngel, $" | [★] ");
                }
                if (playerControl.IsExeTarget())
                {
                    GameSummaryText += ColorString(Colors.Executioner, $" | [⦿] ");
                }
                if (playerControl.IsFramerTarget())
                {
                    GameSummaryText += ColorString(Colors.Framer, $" | [F] ");
                }
                if (playerControl.IsSpelled())
                {
                    GameSummaryText += ColorString(Colors.Impostor, $" | [†] ");
                }
                if (playerControl.Is(RoleEnum.Vulture) && !VultureWin)
                {
                    GameSummaryText += ColorString(Colors.Vulture, $" | ({GetRole<Vulture>(playerControl).BodiesRemainingToWin()} to eat left)");
                }
                if (playerControl.HasTasks())
                {
                    if ((player.TotalTasks - player.TasksLeft)/player.TotalTasks == 1) GameSummaryText += $" | Tasks: " + ColorString(Color.green, $"{player.TotalTasks - player.TasksLeft}/{player.TotalTasks}");
                    else GameSummaryText += $" | Tasks: {player.TotalTasks - player.TasksLeft}/{player.TotalTasks}";
                }
                if (player.Kills > 0 && !playerControl.Is(Faction.Crewmates))
                {
                    GameSummaryText += ColorString(Colors.Impostor, $" | Kills: {player.Kills}");
                }
                if (player.CorrectShot > 0)
                {
                    GameSummaryText += ColorString(Color.green, $" | Correct Shots: {player.CorrectShot}");
                }
                if (player.IncorrectShots > 0)
                {
                    GameSummaryText += ColorString(Colors.Impostor, $" | Incorrect Shots: {player.IncorrectShots}");
                }
                if (player.CorrectAssassinKills > 0)
                {
                    GameSummaryText += ColorString(Color.green, $" | Guesses: {player.CorrectAssassinKills}");
                }
                if (player.IncorrectAssassinKills > 0)
                {
                    GameSummaryText += ColorString(Colors.Impostor, " | Misguessed");
                }
                if (player.CorrectVigilanteShot > 0)
                {
                    GameSummaryText += ColorString(Colors.Vigilante, $" | Correct Shots: {player.CorrectVigilanteShot}");
                }
                if (player.Misfired)
                {
                    GameSummaryText += ColorString(Color.red, $" | Misfired");
                }
                if (player.CorrectKills > 0)
                {
                    GameSummaryText += ColorString(Color.green, $" | Correct Kills: {player.CorrectKills}");
                }
                GameSummaryText += " | " + playerControl.DeathReason();
                AdditionalTempData.GameSummaryText.Add(new AdditionalTempData.PlayerRoleInfo() { PlayerName = playerControl.Data.PlayerName, GameSummaryText = GameSummaryText });
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

            roleSummaryText.AppendLine("<size=125%><u><b>Game Summary</b></u>:</size>");
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
                        StartRPC(CustomRPC.TeamVampiresWin);
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