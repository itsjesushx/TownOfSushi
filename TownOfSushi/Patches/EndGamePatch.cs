using HarmonyLib;
using static TownOfSushi.TownOfSushi;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Text;
using TownOfSushi.Utilities;

namespace TownOfSushi.Patches 
{

    static class AdditionalTempData 
    {
        // Should be implemented using a proper GameOverReason in the future
        public static WinCondition winCondition = WinCondition.Default;
        public static List<WinCondition> additionalWinConditions = new List<WinCondition>();
        public static List<PlayerRoleInfo> playerRoles = new List<PlayerRoleInfo>();
        public static float timer = 0;

        public static void Clear() 
        {
            playerRoles.Clear();
            additionalWinConditions.Clear();
            winCondition = WinCondition.Default;
            timer = 0;
        }

        internal class PlayerRoleInfo 
        {
            public string PlayerName { get; set; }
            public List<RoleInfo> Roles {get;set;}
            public string RoleNames { get; set; }
            public int TasksCompleted  {get;set;}
            public int TasksTotal  {get;set;}
            public bool IsGuesser {get; set;}
            public int? Kills {get; set;}
            public bool IsAlive { get; set; }
        }
    }


    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public static class OnGameEndPatch 
    {
        public static GameOverReason gameOverReason = GameOverReason.HumansByTask;
        public static void Prefix(AmongUsClient __instance, [HarmonyArgument(0)]ref EndGameResult endGameResult) 
        {
            gameOverReason = endGameResult.GameOverReason;
            if ((int)endGameResult.GameOverReason >= 10) endGameResult.GameOverReason = GameOverReason.ImpostorByKill;

            // Reset zoomed out ghosts
            Helpers.ToggleZoom(reset: true);
        }

        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)]ref EndGameResult endGameResult) 
        {
            AdditionalTempData.Clear();

            foreach(var playerControl in PlayerControl.AllPlayerControls) 
            {
                var roles = RoleInfo.GetRoleInfoForPlayer(playerControl);
                var (tasksCompleted, tasksTotal) = TasksHandler.TaskInfo(playerControl.Data);
                bool isGuesser = HandleGuesser.IsGuesser(playerControl.PlayerId);
                int? killCount = GameHistory.deadPlayers.FindAll(x => x.killerIfExisting != null && x.killerIfExisting.PlayerId == playerControl.PlayerId).Count;
                if (killCount == 0 && !(new List<RoleInfo>() { RoleInfo.sheriff, RoleInfo.jackal, RoleInfo.sidekick, RoleInfo.thief }.Contains(RoleInfo.GetRoleInfoForPlayer(playerControl, false).FirstOrDefault()) || playerControl.Data.Role.IsImpostor)) 
                {
                    killCount = null;
                }
                string roleString = RoleInfo.GetRolesString(playerControl, true, true, false);
                AdditionalTempData.playerRoles.Add(new AdditionalTempData.PlayerRoleInfo() { PlayerName = playerControl.Data.PlayerName, Roles = roles, RoleNames = roleString, TasksTotal = tasksTotal, TasksCompleted = tasksCompleted, IsGuesser = isGuesser, Kills = killCount, IsAlive = !playerControl.Data.IsDead });
            }

            // Remove Jester, Arsonist, Vulture, Jackal, former Jackals and Sidekick from winners (if they win, they'll be readded)
            List<PlayerControl> notWinners = new List<PlayerControl>();
            if (Jester.jester != null) notWinners.Add(Jester.jester);
            if (Sidekick.sidekick != null) notWinners.Add(Sidekick.sidekick);
            if (Jackal.jackal != null) notWinners.Add(Jackal.jackal);
            if (Glitch.Player != null) notWinners.Add(Glitch.Player);
            if (Werewolf.Player != null) notWinners.Add(Werewolf.Player);
            if (SerialKiller.Player != null) notWinners.Add(SerialKiller.Player);
            if (Arsonist.arsonist != null) notWinners.Add(Arsonist.arsonist);
            if (Vulture.vulture != null) notWinners.Add(Vulture.vulture);
            if (Lawyer.lawyer != null) notWinners.Add(Lawyer.lawyer);
            if (Pursuer.pursuer != null) notWinners.Add(Pursuer.pursuer);
            if (Thief.thief != null) notWinners.Add(Thief.thief);

            notWinners.AddRange(Jackal.formerJackals);

            List<CachedPlayerData> winnersToRemove = new List<CachedPlayerData>();
            foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator()) 
            {
                if (notWinners.Any(x => x.Data.PlayerName == winner.PlayerName)) winnersToRemove.Add(winner);
            }
            foreach (var winner in winnersToRemove) EndGameResult.CachedWinners.Remove(winner);

            bool jesterWin = Jester.jester != null && gameOverReason == (GameOverReason)CustomGameOverReason.JesterWin;
            bool arsonistWin = Arsonist.arsonist != null && gameOverReason == (GameOverReason)CustomGameOverReason.ArsonistWin;
            bool glitchWin = Glitch.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.GlitchWin;
            bool wwWin = Werewolf.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.WerewolfWin;
            bool miniLose = Mini.mini != null && gameOverReason == (GameOverReason)CustomGameOverReason.MiniLose;
            bool SKWin = SerialKiller.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.SerialKillerWin;
            bool loversWin = Lovers.ExistingAndAlive() && (gameOverReason == (GameOverReason)CustomGameOverReason.LoversWin || (GameManager.Instance.DidHumansWin(gameOverReason) && !Lovers.ExistingWithKiller())); // Either they win if they are among the last 3 players, or they win if they are both Crewmates and both alive and the Crew wins (Team Imp/Jackal Lovers can only win solo wins)
            bool teamJackalWin = gameOverReason == (GameOverReason)CustomGameOverReason.TeamJackalWin && ((Jackal.jackal != null && !Jackal.jackal.Data.IsDead) || (Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead));
            bool vultureWin = Vulture.vulture != null && gameOverReason == (GameOverReason)CustomGameOverReason.VultureWin;
            bool prosecutorWin = Lawyer.lawyer != null && gameOverReason == (GameOverReason)CustomGameOverReason.ProsecutorWin;

            // Mini lose
            if (miniLose) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Mini.mini.Data);
                wpd.IsYou = false; // If "no one is the Mini", it will display the Mini, but also show defeat to everyone
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.MiniLose;
            }

            // Jester win
            else if (jesterWin) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Jester.jester.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.JesterWin;
            }

            // Glitch win
            else if (glitchWin) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Glitch.Player.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.GlitchWin;
            }

            // Werewolf win
            else if (wwWin) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Werewolf.Player.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.WerewolfWin;
            }

            // Serial Killer win
            else if (SKWin) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(SerialKiller.Player.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.SerialKillerWin;
            }

            // Arsonist win
            else if (arsonistWin) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Arsonist.arsonist.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.ArsonistWin;
            }

            // Vulture win
            else if (vultureWin) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Vulture.vulture.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.VultureWin;
            }

            // Jester win
            else if (prosecutorWin) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Lawyer.lawyer.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.ProsecutorWin;
            }

            // Lovers win conditions
            else if (loversWin) 
            {
                // Double win for lovers, crewmates also win
                if (!Lovers.ExistingWithKiller()) 
                {
                    AdditionalTempData.winCondition = WinCondition.LoversTeamWin;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    foreach (PlayerControl p in PlayerControl.AllPlayerControls) 
                    {
                        if (p == null) continue;
                        if (p == Lovers.lover1 || p == Lovers.lover2)
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(p.Data));
                        else if (p == Pursuer.pursuer && !Pursuer.pursuer.Data.IsDead)
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(p.Data));
                        else if (p != Jester.jester && p != SerialKiller.Player && p != Jackal.jackal && p != Glitch.Player && p != Werewolf.Player && p != Sidekick.sidekick && p != Arsonist.arsonist && p != Vulture.vulture && !Jackal.formerJackals.Contains(p) && !p.Data.Role.IsImpostor)
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(p.Data));
                    }
                }
                // Lovers solo win
                else 
                {
                    AdditionalTempData.winCondition = WinCondition.LoversSoloWin;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(Lovers.lover1.Data));
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(Lovers.lover2.Data));
                }
            }

            // Jackal win condition (should be implemented using a proper GameOverReason in the future)
            else if (teamJackalWin) 
            {
                // Jackal wins if nobody except jackal is alive
                AdditionalTempData.winCondition = WinCondition.JackalWin;
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Jackal.jackal.Data);
                wpd.IsImpostor = false;
                EndGameResult.CachedWinners.Add(wpd);
                // If there is a sidekick. The sidekick also wins
                if (Sidekick.sidekick != null) 
                {
                    CachedPlayerData wpdSidekick = new CachedPlayerData(Sidekick.sidekick.Data);
                    wpdSidekick.IsImpostor = false;
                    EndGameResult.CachedWinners.Add(wpdSidekick);
                }
                foreach (var player in Jackal.formerJackals) 
                {
                    CachedPlayerData wpdFormerJackal = new CachedPlayerData(player.Data);
                    wpdFormerJackal.IsImpostor = false;
                    EndGameResult.CachedWinners.Add(wpdFormerJackal);
                }
            }

            // Possible Additional winner: Lawyer
            if (Lawyer.lawyer != null && Lawyer.target != null && (!Lawyer.target.Data.IsDead || Lawyer.target == Jester.jester) && !Pursuer.notAckedExiled && !Lawyer.isProsecutor) {
                CachedPlayerData winningClient = null;
                foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator()) 
                {
                    if (winner.PlayerName == Lawyer.target.Data.PlayerName)
                        winningClient = winner;
                }
                if (winningClient != null) { // The Lawyer wins if the client is winning (and alive, but if he wasn't the Lawyer shouldn't exist anymore)
                    if (!EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == Lawyer.lawyer.Data.PlayerName))
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(Lawyer.lawyer.Data));
                    AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalLawyerBonusWin); // The Lawyer wins together with the client
                } 
            }

            // Possible Additional winner: Pursuer
            if (Pursuer.pursuer != null && !Pursuer.pursuer.Data.IsDead && !Pursuer.notAckedExiled && !EndGameResult.CachedWinners.ToArray().Any(x => x.IsImpostor)) {
                if (!EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == Pursuer.pursuer.Data.PlayerName))
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(Pursuer.pursuer.Data));
                AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalAlivePursuerWin);
            }
            RPCProcedure.ResetVariables();
            EventUtility.GameEndsUpdate();
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public class EndGameManagerSetUpPatch 
    {
        public static void Postfix(EndGameManager __instance) 
        {
            // Delete and readd PoolablePlayers always showing the name and role of the player
            foreach (PoolablePlayer pb in __instance.transform.GetComponentsInChildren<PoolablePlayer>()) 
            {
                UnityEngine.Object.Destroy(pb.gameObject);
            }
            int num = Mathf.CeilToInt(7.5f);
            List<CachedPlayerData> list = EndGameResult.CachedWinners.ToArray().ToList().OrderBy(delegate(CachedPlayerData b)
            {
                if (!b.IsYou)
                {
                    return 0;
                }
                return -1;
            }).ToList<CachedPlayerData>();
            for (int i = 0; i < list.Count; i++) 
            {
                CachedPlayerData CachedPlayerData2 = list[i];
                int num2 = (i % 2 == 0) ? -1 : 1;
                int num3 = (i + 1) / 2;
                float num4 = (float)num3 / (float)num;
                float num5 = Mathf.Lerp(1f, 0.75f, num4);
                float num6 = (float)((i == 0) ? -8 : -1);
                PoolablePlayer poolablePlayer = UnityEngine.Object.Instantiate<PoolablePlayer>(__instance.PlayerPrefab, __instance.transform);
                poolablePlayer.transform.localPosition = new Vector3(1f * (float)num2 * (float)num3 * num5, FloatRange.SpreadToEdges(-1.125f, 0f, num3, num), num6 + (float)num3 * 0.01f) * 0.9f;
                float num7 = Mathf.Lerp(1f, 0.65f, num4) * 0.9f;
                Vector3 vector = new Vector3(num7, num7, 1f);
                poolablePlayer.transform.localScale = vector;
                if (CachedPlayerData2.IsDead) 
                {
                    poolablePlayer.SetBodyAsGhost();
                    poolablePlayer.SetDeadFlipX(i % 2 == 0);
                } 
                else 
                {
                    poolablePlayer.SetFlipX(i % 2 == 0);
                }
                poolablePlayer.UpdateFromPlayerOutfit(CachedPlayerData2.Outfit, PlayerMaterial.MaskType.None, CachedPlayerData2.IsDead, true);

                poolablePlayer.cosmetics.nameText.color = Color.white;
                poolablePlayer.cosmetics.nameText.transform.localScale = new Vector3(1f / vector.x, 1f / vector.y, 1f / vector.z);
                poolablePlayer.cosmetics.nameText.transform.localPosition = new Vector3(poolablePlayer.cosmetics.nameText.transform.localPosition.x, poolablePlayer.cosmetics.nameText.transform.localPosition.y, -15f);
                poolablePlayer.cosmetics.nameText.text = CachedPlayerData2.PlayerName;

                foreach(var data in AdditionalTempData.playerRoles) 
                {
                    if (data.PlayerName != CachedPlayerData2.PlayerName) continue;
                    var roles = 
                    poolablePlayer.cosmetics.nameText.text += $"\n{string.Join("\n", data.Roles.Select(x => Helpers.ColorString(x.color, x.name)))}";
                }
            }

            // Additional code
            GameObject bonusText = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
            bonusText.transform.position = new Vector3(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.5f, __instance.WinText.transform.position.z);
            bonusText.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            TMPro.TMP_Text textRenderer = bonusText.GetComponent<TMPro.TMP_Text>();
            textRenderer.text = "";

            if (AdditionalTempData.winCondition == WinCondition.JesterWin) 
            {
                textRenderer.text = "Jester Wins";
                textRenderer.color = Jester.color;
                __instance.BackgroundBar.material.SetColor("_Color", Jester.color);
            } 
            else if (AdditionalTempData.winCondition == WinCondition.ArsonistWin) 
            {
                textRenderer.text = "Arsonist Wins";
                textRenderer.color = Arsonist.color;
                __instance.BackgroundBar.material.SetColor("_Color", Arsonist.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.VultureWin) 
            {
                textRenderer.text = "Vulture Wins";
                textRenderer.color = Vulture.color;
                __instance.BackgroundBar.material.SetColor("_Color", Vulture.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.ProsecutorWin) 
            {
                textRenderer.text = "Prosecutor Wins";
                textRenderer.color = Lawyer.color;
                __instance.BackgroundBar.material.SetColor("_Color", Lawyer.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.LoversTeamWin) 
            {
                textRenderer.text = "Lovers And Crewmates Win";
                textRenderer.color = Lovers.color;
                __instance.BackgroundBar.material.SetColor("_Color", Lovers.color);
            } 
            else if (AdditionalTempData.winCondition == WinCondition.LoversSoloWin) 
            {
                textRenderer.text = "Lovers Win";
                textRenderer.color = Lovers.color;
                __instance.BackgroundBar.material.SetColor("_Color", Lovers.color);
            } 
            else if (AdditionalTempData.winCondition == WinCondition.JackalWin) 
            {
                textRenderer.text = "Jackals Win";
                textRenderer.color = Jackal.color;
                __instance.BackgroundBar.material.SetColor("_Color", Jackal.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.GlitchWin) 
            {
                textRenderer.text = "The Glitch Wins";
                textRenderer.color = Glitch.color;
                __instance.BackgroundBar.material.SetColor("_Color", Glitch.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.SerialKillerWin) 
            {
                textRenderer.text = "Serial Kille Wins";
                textRenderer.color = SerialKiller.color;
                __instance.BackgroundBar.material.SetColor("_Color", SerialKiller.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.WerewolfWin) 
            {
                textRenderer.text = "Werewolf Wins";
                textRenderer.color = Werewolf.color;
                __instance.BackgroundBar.material.SetColor("_Color", Werewolf.color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.MiniLose) 
            {
                textRenderer.text = "Mini died";
                textRenderer.color = Mini.color;
                __instance.BackgroundBar.material.SetColor("_Color", Mini.color);
            } 
            else if (AdditionalTempData.winCondition == WinCondition.Default) 
            {
                switch (OnGameEndPatch.gameOverReason) 
                {
                    case GameOverReason.ImpostorDisconnect:
                        textRenderer.text = "Last Crewmate Disconnected";
                        textRenderer.color = Palette.ImpostorRed;
                        __instance.BackgroundBar.material.SetColor("_Color", Palette.ImpostorRed);
                        break;
                    case GameOverReason.ImpostorByKill:
                        textRenderer.text = "Impostors Win";
                        textRenderer.color = Palette.ImpostorRed;
                        __instance.BackgroundBar.material.SetColor("_Color", Palette.ImpostorRed);
                        break;
                    case GameOverReason.ImpostorBySabotage:
                        textRenderer.text = "Impostors Win";
                        textRenderer.color = Palette.ImpostorRed;
                        __instance.BackgroundBar.material.SetColor("_Color", Palette.ImpostorRed);
                        break;
                    case GameOverReason.ImpostorByVote:
                        textRenderer.text = "Impostors Win";
                        textRenderer.color = Palette.ImpostorRed;
                        __instance.BackgroundBar.material.SetColor("_Color", Palette.ImpostorRed);
                        break;
                    case GameOverReason.HumansByTask:
                        textRenderer.text = "Crewmates Win";
                        textRenderer.color = Palette.CrewmateBlue;
                        __instance.BackgroundBar.material.SetColor("_Color", Palette.CrewmateBlue);
                        break;
                    case GameOverReason.HumansDisconnect:
                        textRenderer.text = "Impostors Disconnected";
                        textRenderer.color = Palette.CrewmateBlue;
                        __instance.BackgroundBar.material.SetColor("_Color", Palette.CrewmateBlue);
                        break;
                    case GameOverReason.HumansByVote:
                        textRenderer.text = "Crewmates Win";
                        textRenderer.color = Palette.CrewmateBlue;
                        __instance.BackgroundBar.material.SetColor("_Color", Palette.CrewmateBlue);
                        break;
                }
            }

            foreach (WinCondition cond in AdditionalTempData.additionalWinConditions) 
            {
                if (cond == WinCondition.AdditionalLawyerBonusWin) 
                {
                    textRenderer.text += $"\n{Helpers.ColorString(Lawyer.color, "The Lawyer wins with the client")}";
                } else if (cond == WinCondition.AdditionalAlivePursuerWin) 
                {
                    textRenderer.text += $"\n{Helpers.ColorString(Pursuer.color, "The Pursuer survived")}";
                }
            }

            if (MapOptions.showRoleSummary) 
            {
                var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
                GameObject roleSummary = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
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

                foreach(var data in AdditionalTempData.playerRoles) 
                {
                    string roles = data.RoleNames;
                    var taskInfo = data.TasksTotal > 0 ? $" - <color=#FAD934FF>({data.TasksCompleted}/{data.TasksTotal})</color>" : "";
                    if (data.Kills != null) taskInfo += $" - <color=#FF0000FF>(Kills: {data.Kills})</color>";
                    var dataString = $"<size=70%>{Helpers.ColorString(data.IsAlive ? Color.white : new Color(.7f,.7f,.7f), data.PlayerName)} - {roles}{taskInfo}</size>"; 

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

                var roleSummaryTextMesh = roleSummary.GetComponent<TMPro.TMP_Text>();
                roleSummaryTextMesh.alignment = TMPro.TextAlignmentOptions.TopLeft;
                roleSummaryTextMesh.color = Color.white;
                roleSummaryTextMesh.fontSizeMin = 1f;
                roleSummaryTextMesh.fontSizeMax = 1f;
                roleSummaryTextMesh.fontSize = 1f;
                roleSummaryTextMesh.text = $"{roleSummaryText}";
                roleSummaryTextMesh.GetComponent<RectTransform>().anchoredPosition = new(position.x + 3.5f, position.y - 0.1f);
            }
            AdditionalTempData.Clear();
        }
    }

    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))] 
    class CheckEndCriteriaPatch 
    {
        public static bool Prefix(ShipStatus __instance) 
        {
            if (!GameData.Instance) return false;
            if (DestroyableSingleton<TutorialManager>.InstanceExists) // InstanceExists | Don't check Custom Criteria when in Tutorial
                return true;
            var statistics = new PlayerStatistics(__instance);
            if (CheckAndEndGameForMiniLose(__instance)) return false;
            if (CheckAndEndGameForJesterWin(__instance)) return false;
            if (CheckAndEndGameForArsonistWin(__instance)) return false;
            if (CheckAndEndGameForVultureWin(__instance)) return false;
            if (CheckAndEndGameForSabotageWin(__instance)) return false;
            if (CheckAndEndGameForTaskWin(__instance)) return false;
            if (CheckAndEndGameForProsecutorWin(__instance)) return false;
            if (CheckAndEndGameForLoverWin(__instance, statistics)) return false;
            if (CheckAndEndGameForJackalWin(__instance, statistics)) return false;
            if (CheckAndEndGameForGlitchWin(__instance, statistics)) return false;
            if (CheckAndEndGameForWerewolfWin(__instance, statistics)) return false;
            if (CheckAndEndGameForSerialKillerWin(__instance, statistics)) return false;
            if (CheckAndEndGameForImpostorWin(__instance, statistics)) return false;
            if (CheckAndEndGameForCrewmateWin(__instance, statistics)) return false;
            return false;
        }

        private static bool CheckAndEndGameForMiniLose(ShipStatus __instance) 
        {
            if (Mini.triggerMiniLose) 
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.MiniLose, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForJesterWin(ShipStatus __instance) 
        {
            if (Jester.triggerJesterWin) 
            {

                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.JesterWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForArsonistWin(ShipStatus __instance) 
        {
            if (Arsonist.triggerArsonistWin) 
            {

                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ArsonistWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForVultureWin(ShipStatus __instance) 
        {
            if (Vulture.triggerVultureWin) 
            {

                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.VultureWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForSabotageWin(ShipStatus __instance) 
        {
            if (MapUtilities.Systems == null) return false;
            var systemType = MapUtilities.Systems.ContainsKey(SystemTypes.LifeSupp) ? MapUtilities.Systems[SystemTypes.LifeSupp] : null;
            if (systemType != null) 
            {
                LifeSuppSystemType lifeSuppSystemType = systemType.TryCast<LifeSuppSystemType>();
                if (lifeSuppSystemType != null && lifeSuppSystemType.Countdown < 0f) 
                {
                    EndGameForSabotage(__instance);
                    lifeSuppSystemType.Countdown = 10000f;
                    return true;
                }
            }
            var systemType2 = MapUtilities.Systems.ContainsKey(SystemTypes.Reactor) ? MapUtilities.Systems[SystemTypes.Reactor] : null;
            if (systemType2 == null) 
            {
                systemType2 = MapUtilities.Systems.ContainsKey(SystemTypes.Laboratory) ? MapUtilities.Systems[SystemTypes.Laboratory] : null;
            }
            if (systemType2 != null) {
                ICriticalSabotage criticalSystem = systemType2.TryCast<ICriticalSabotage>();
                if (criticalSystem != null && criticalSystem.Countdown < 0f) 
                {
                    EndGameForSabotage(__instance);
                    criticalSystem.ClearSabotage();
                    return true;
                }
            }
            return false;
        }

        private static bool CheckAndEndGameForTaskWin(ShipStatus __instance) 
        {
            if (GameData.Instance.TotalTasks > 0 && GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks) 
            {

                GameManager.Instance.RpcEndGame(GameOverReason.HumansByTask, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForProsecutorWin(ShipStatus __instance) 
        {
            if (Lawyer.triggerProsecutorWin) 
            {

                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ProsecutorWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForLoverWin(ShipStatus __instance, PlayerStatistics statistics) 
        {
            if (statistics.TeamLoversAlive == 2 && statistics.TotalAlive <= 3) {

                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.LoversWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForJackalWin(ShipStatus __instance, PlayerStatistics statistics) 
        {
            if (statistics.TeamJackalAlive >= statistics.TotalAlive - statistics.TeamJackalAlive
            && statistics.TeamImpostorsAlive == 0
            && statistics.GlitchAlive == 0
            && statistics.WerewolfAlive == 0
            && statistics.SerialKillerAlive == 0
            && statistics.CrewPowerAlive == 0
            && !(statistics.TeamJackalHasAliveLover 
            && statistics.TeamLoversAlive == 2)) 
            {

                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.TeamJackalWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForWerewolfWin(ShipStatus __instance, PlayerStatistics statistics) 
        {
            if (statistics.WerewolfAlive >= statistics.TotalAlive - statistics.WerewolfAlive
            && statistics.TeamImpostorsAlive == 0
            && statistics.GlitchAlive == 0
            && statistics.TeamJackalAlive == 0
            && statistics.SerialKillerAlive == 0
            && statistics.CrewPowerAlive == 0
            && !(statistics.WerewolfAliveHasLover 
            && statistics.TeamLoversAlive == 2)) 
            {

                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.WerewolfWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForSerialKillerWin(ShipStatus __instance, PlayerStatistics statistics) 
        {
            if (statistics.SerialKillerAlive >= statistics.TotalAlive - statistics.SerialKillerAlive
            && statistics.TeamImpostorsAlive == 0
            && statistics.CrewPowerAlive == 0
            && statistics.TeamJackalAlive == 0
            && statistics.WerewolfAlive == 0
            && statistics.GlitchAlive == 0
            && !(statistics.SerialKillerHasLover
            && statistics.TeamLoversAlive == 2)) 
            {

                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.SerialKillerWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForGlitchWin(ShipStatus __instance, PlayerStatistics statistics) 
        {
            if (statistics.GlitchAlive >= statistics.TotalAlive - statistics.GlitchAlive
            && statistics.TeamImpostorsAlive == 0
            && statistics.CrewPowerAlive == 0
            && statistics.TeamJackalAlive == 0
            && statistics.WerewolfAlive == 0
            && statistics.SerialKillerAlive == 0
            && !(statistics.GlitchHasLoverAlive
            && statistics.TeamLoversAlive == 2)) 
            {

                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.GlitchWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForImpostorWin(ShipStatus __instance, PlayerStatistics statistics) 
        {
            if (statistics.TeamImpostorsAlive >= statistics.TotalAlive - statistics.TeamImpostorsAlive 
            && statistics.TeamJackalAlive == 0
            && statistics.GlitchAlive == 0
            && statistics.WerewolfAlive == 0
            && statistics.SerialKillerAlive == 0
            && statistics.CrewPowerAlive == 0
            && !(statistics.TeamImpostorHasAliveLover 
            && statistics.TeamLoversAlive == 2)) 
            {

                GameOverReason endReason;
                switch (GameData.LastDeathReason) 
                {
                    case DeathReason.Exile:
                        endReason = GameOverReason.ImpostorByVote;
                        break;
                    case DeathReason.Kill:
                        endReason = GameOverReason.ImpostorByKill;
                        break;
                    default:
                        endReason = GameOverReason.ImpostorByVote;
                        break;
                }
                GameManager.Instance.RpcEndGame(endReason, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForCrewmateWin(ShipStatus __instance, PlayerStatistics statistics) 
        {
            if (statistics.TeamImpostorsAlive == 0
            && statistics.TeamJackalAlive == 0
            && statistics.SerialKillerAlive == 0
            && statistics.WerewolfAlive == 0
            && statistics.GlitchAlive == 0) 
            {

                GameManager.Instance.RpcEndGame(GameOverReason.HumansByVote, false);
                return true;
            }
            return false;
        }

        private static void EndGameForSabotage(ShipStatus __instance) 
        {
            //__instance.enabled = false;
            GameManager.Instance.RpcEndGame(GameOverReason.ImpostorBySabotage, false);
            return;
        }

    }

    internal class PlayerStatistics 
    {
        public int TeamImpostorsAlive {get;set;}
        public int TeamJackalAlive {get;set;}
        public int GlitchAlive {get;set;}
        public int WerewolfAlive {get;set;}
        public int SerialKillerAlive {get;set;}
        public int CrewPowerAlive {get;set;}
        public int TeamLoversAlive {get;set;}
        public int TotalAlive {get;set;}
        public bool TeamImpostorHasAliveLover {get;set;}
        public bool TeamJackalHasAliveLover {get;set;}
        public bool WerewolfAliveHasLover {get;set;}
        public bool GlitchHasLoverAlive {get;set;}
        public bool SerialKillerHasLover {get;set;}

        public PlayerStatistics(ShipStatus __instance) 
        {
            GetPlayerCounts();
        }

        private static bool IsLover(NetworkedPlayerInfo p) 
        {
            return (Lovers.lover1 != null && Lovers.lover1.PlayerId == p.PlayerId) || (Lovers.lover2 != null && Lovers.lover2.PlayerId == p.PlayerId);
        }

        private void GetPlayerCounts() 
        {
            int numJackalAlive = 0;
            int numGlitchAlive = 0;
            int numImpostorsAlive = 0;
            int numWerewolfAlive = 0;
            int numSerialKillerAlive = 0;
            int numCrewPowerAlive = 0;
            int numLoversAlive = 0;
            int numTotalAlive = 0;
            bool impLover = false;
            bool glitchLover = false;
            bool SKLover = false;
            bool jackalLover = false;
            bool WerewolfLover = false;

            foreach (var playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
            {
                if (!playerInfo.Disconnected)
                {
                    if (!playerInfo.IsDead)
                    {
                        numTotalAlive++;

                        bool lover = IsLover(playerInfo);
                        if (lover) numLoversAlive++;

                        if (playerInfo.Role.IsImpostor) 
                        {
                            numImpostorsAlive++;
                            if (lover) impLover = true;
                        }
                        if (Jackal.jackal != null && Jackal.jackal.PlayerId == playerInfo.PlayerId) 
                        {
                            numJackalAlive++;
                            if (lover) jackalLover = true;
                        }
                        if (Sheriff.sheriff != null && Sheriff.sheriff.PlayerId == playerInfo.PlayerId) 
                        {
                            numCrewPowerAlive++;
                        }
                        if (Mayor.mayor != null && Mayor.mayor.PlayerId == playerInfo.PlayerId) 
                        {
                            numCrewPowerAlive++;
                        }
                        if (Veteran.Player != null && Veteran.Charges > 0 && Veteran.Player.PlayerId == playerInfo.PlayerId) 
                        {
                            numCrewPowerAlive++;
                        }
                        if (Swapper.swapper != null && Swapper.charges > 0 && Swapper.swapper.PlayerId == playerInfo.PlayerId) 
                        {
                            numCrewPowerAlive++;
                        }
                        if (Sidekick.sidekick != null && Sidekick.sidekick.PlayerId == playerInfo.PlayerId) 
                        {
                            numJackalAlive++;
                            if (lover) jackalLover = true;
                        }
                        if (SerialKiller.Player != null && SerialKiller.Player.PlayerId == playerInfo.PlayerId) 
                        {
                            numSerialKillerAlive++;
                            if (lover) SKLover = true;
                        }
                        if (Glitch.Player != null && Glitch.Player.PlayerId == playerInfo.PlayerId) 
                        {
                            numGlitchAlive++;
                            if (lover) glitchLover = true;
                        }
                        if (Werewolf.Player != null && Werewolf.Player.PlayerId == playerInfo.PlayerId) 
                        {
                            numWerewolfAlive++;
                            if (lover) WerewolfLover = true;
                        }
                    }
                }
            }

            TeamJackalAlive = numJackalAlive;
            GlitchAlive = numGlitchAlive;
            SerialKillerAlive = numSerialKillerAlive;
            WerewolfAlive = numWerewolfAlive;
            CrewPowerAlive = numCrewPowerAlive;
            TeamImpostorsAlive = numImpostorsAlive;
            TeamLoversAlive = numLoversAlive;
            TotalAlive = numTotalAlive;
            TeamImpostorHasAliveLover = impLover;
            TeamJackalHasAliveLover = jackalLover;
            GlitchHasLoverAlive = glitchLover;
            SerialKillerHasLover = SKLover;
            WerewolfAliveHasLover = WerewolfLover;
        }
    }
}
