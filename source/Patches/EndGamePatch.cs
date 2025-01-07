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
            // Theres a better way of doing this e.g. switch statement or dictionary. But this works for now.
            foreach (var playerControl in PlayerControl.AllPlayerControls) 
            {
                GameSummaryText = "";
                foreach (var role in RoleHistory.Where(x => x.Key == playerControl.PlayerId))
                {
                    #region Crewmate Roles

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
                    else if (role.Value == RoleEnum.Transporter) { GameSummaryText += "<color=#" + Colors.Transporter.ToHtmlStringRGBA() + ">Transporter</color> > "; }
                    else if (role.Value == RoleEnum.Medium) { GameSummaryText += "<color=#" + Colors.Medium.ToHtmlStringRGBA() + ">Medium</color> > "; }
                    else if (role.Value == RoleEnum.Trapper) { GameSummaryText += "<color=#" + Colors.Trapper.ToHtmlStringRGBA() + ">Trapper</color> > "; }
                    else if (role.Value == RoleEnum.Veteran) { GameSummaryText += "<color=#" + Colors.Veteran.ToHtmlStringRGBA() + ">Veteran</color> > "; }
                    else if (role.Value == RoleEnum.Hunter) { GameSummaryText += "<color=#" + Colors.Hunter.ToHtmlStringRGBA() + ">Hunter</color> > "; }

                    #endregion

                    #region Impostor Roles

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

                    #endregion

                    #region Neutral Benign Roles

                    else if (role.Value == RoleEnum.Amnesiac) { GameSummaryText += "<color=#" + Colors.Amnesiac.ToHtmlStringRGBA() + ">Amnesiac</color> > "; }
                    else if (role.Value == RoleEnum.Romantic) { GameSummaryText += "<color=#" + Colors.Romantic.ToHtmlStringRGBA() + ">Romantic</color> > "; }
                    else if (role.Value == RoleEnum.GuardianAngel) { GameSummaryText += "<color=#" + Colors.GuardianAngel.ToHtmlStringRGBA() + ">Guardian Angel</color> > "; }

                    #endregion

                    #region Neutral Evil Roles

                    else if (role.Value == RoleEnum.Vulture) { GameSummaryText += "<color=#" + Colors.Vulture.ToHtmlStringRGBA() + ">Vulture</color> > "; }
                    else if (role.Value == RoleEnum.Doomsayer) { GameSummaryText += "<color=#" + Colors.Doomsayer.ToHtmlStringRGBA() + ">Doomsayer</color> > "; }
                    else if (role.Value == RoleEnum.Jester) { GameSummaryText += "<color=#" + Colors.Jester.ToHtmlStringRGBA() + ">Jester</color> > "; }
                    else if (role.Value == RoleEnum.Executioner) { GameSummaryText += "<color=#" + Colors.Executioner.ToHtmlStringRGBA() + ">Executioner</color> > "; }

                    #endregion

                    #region Neutral Killing Roles

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
                    
                    #endregion
                }
                
                if (!string.IsNullOrEmpty(GameSummaryText)) GameSummaryText = GameSummaryText.Remove(GameSummaryText.Length - 3);
                
                #region Modifiers
                if (playerControl.Is(ModifierEnum.Giant)) { GameSummaryText += " (<color=#" + Colors.Giant.ToHtmlStringRGBA() + ">Giant</color>)"; }
                else if (playerControl.Is(ModifierEnum.Aftermath)) { GameSummaryText += " (<color=#" + Colors.Aftermath.ToHtmlStringRGBA() + ">Aftermath</color>)"; }
                else if (playerControl.Is(ModifierEnum.Mini)) { GameSummaryText += " (<color=#" + Colors.Mini.ToHtmlStringRGBA() + ">Mini</color>)"; }
                else if (playerControl.Is(ModifierEnum.Bait)) { GameSummaryText += " (<color=#" + Colors.Bait.ToHtmlStringRGBA() + ">Bait</color>)";}
                else if (playerControl.Is(ModifierEnum.Diseased)) { GameSummaryText += " (<color=#" + Colors.Diseased.ToHtmlStringRGBA() + ">Diseased</color>)";}
                else if (playerControl.Is(ModifierEnum.Disperser)) { GameSummaryText += " (<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Disperser</color>)";}
                else if (playerControl.Is(ModifierEnum.DoubleShot)) { GameSummaryText += " (<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Double Shot</color>)";}
                else if (playerControl.Is(ModifierEnum.Underdog)) { GameSummaryText += " (<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Underdog</color>)";}
                else if (playerControl.Is(ModifierEnum.Frosty)) { GameSummaryText += " (<color=#" + Colors.Frosty.ToHtmlStringRGBA() + ">Frosty</color>)";}

                #endregion

                #region Abilities
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
                
                #endregion
                
                #region Stats

                var player = GetPlayerRole(playerControl);
                //Stats
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
                if (playerControl.IsSpelled())
                {
                    GameSummaryText += ColorString(Colors.Impostor, $" | [†] ");
                }
                if (playerControl.Is(RoleEnum.Vulture))
                {
                    GameSummaryText += ColorString(Colors.Vulture, $" | ({GetRole<Vulture>(playerControl).BodiesRemainingToWin()} bodies to eat left)");
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
                #endregion
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
}