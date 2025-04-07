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
                if (killCount == 0 && !(new List<RoleInfo>() { RoleInfo.sheriff, RoleInfo.veteran, RoleInfo.thief }.Contains(RoleInfo.GetRoleInfoForPlayer(playerControl, false).FirstOrDefault()) || playerControl.IsNeutralKiller() || playerControl.Data.Role.IsImpostor)) 
                {
                    killCount = null;
                }
                string roleString = RoleInfo.GetRolesString(playerControl, true, true, false);
                AdditionalTempData.playerRoles.Add(new AdditionalTempData.PlayerRoleInfo() { PlayerName = playerControl.Data.PlayerName, Roles = roles, RoleNames = roleString, TasksTotal = tasksTotal, TasksCompleted = tasksCompleted, IsGuesser = isGuesser, Kills = killCount, IsAlive = !playerControl.Data.IsDead });
            }

            // Remove Jester, Arsonist, Vulture, Jackal, former Jackals and Sidekick from winners (if they win, they'll be readded)
            List<PlayerControl> notWinners = new List<PlayerControl>();
            if (Jester.Player != null) notWinners.Add(Jester.Player);
            if (Sidekick.Player != null) notWinners.Add(Sidekick.Player);
            if (Jackal.Player != null) notWinners.Add(Jackal.Player);
            if (Glitch.Player != null) notWinners.Add(Glitch.Player);
            if (Werewolf.Player != null) notWinners.Add(Werewolf.Player);
            if (SerialKiller.Player != null) notWinners.Add(SerialKiller.Player);
            if (Arsonist.Player != null) notWinners.Add(Arsonist.Player);
            if (Vulture.Player != null) notWinners.Add(Vulture.Player);
            if (Lawyer.Player != null) notWinners.Add(Lawyer.Player);
            if (Pursuer.Player != null) notWinners.Add(Pursuer.Player);
            if (Romantic.Player != null) notWinners.Add(Romantic.Player);
            if (Juggernaut.Player != null) notWinners.Add(Juggernaut.Player);
            if (VengefulRomantic.Player != null) notWinners.Add(VengefulRomantic.Player);
            if (Thief.Player != null) notWinners.Add(Thief.Player);
            if (Plaguebearer.Player != null) notWinners.Add(Plaguebearer.Player);
            if (Pestilence.Player != null) notWinners.Add(Pestilence.Player);
            if (Agent.Player != null) notWinners.Add(Agent.Player);
            if (Hitman.Player != null) notWinners.Add(Hitman.Player);

            notWinners.AddRange(Jackal.formerJackals);

            List<CachedPlayerData> winnersToRemove = new List<CachedPlayerData>();
            foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator()) 
            {
                if (notWinners.Any(x => x.Data.PlayerName == winner.PlayerName)) winnersToRemove.Add(winner);
            }
            foreach (var winner in winnersToRemove) EndGameResult.CachedWinners.Remove(winner);

            bool jesterWin = Jester.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.JesterWin;
            bool arsonistWin = Arsonist.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.ArsonistWin;
            bool glitchWin = Glitch.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.GlitchWin;
            bool wwWin = Werewolf.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.WerewolfWin;
            bool miniLose = Mini.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.MiniLose;
            bool SKWin = SerialKiller.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.SerialKillerWin;
            bool RomanticWin = VengefulRomantic.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.VRomanticWin;
            bool JuggernautWin = Juggernaut.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.JuggernautWin;
            bool loversWin = Lovers.ExistingAndAlive() && (gameOverReason == (GameOverReason)CustomGameOverReason.LoversWin || (GameManager.Instance.DidHumansWin(gameOverReason) && !Lovers.ExistingWithKiller())); // Either they win if they are among the last 3 players, or they win if they are both Crewmates and both alive and the Crew wins (Team Imp/Jackal Lovers can only win solo wins)
            bool teamJackalWin = gameOverReason == (GameOverReason)CustomGameOverReason.TeamJackalWin && ((Jackal.Player != null && !Jackal.Player.Data.IsDead) || (Sidekick.Player != null && !Sidekick.Player.Data.IsDead));
            bool vultureWin = Vulture.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.VultureWin;
            bool prosecutorWin = Lawyer.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.ProsecutorWin;
            bool PlaguebearerWin = Plaguebearer.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.PlaguebearerWin;
            bool PestilenceWin = Pestilence.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.PestilenceWin;
            bool HitmanWin = Hitman.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.HitmanWin;
            bool AgentWin = Agent.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.AgentWin;

            // Mini lose
            if (miniLose) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Mini.Player.Data);
                wpd.IsYou = false; // If "no one is the Mini", it will display the Mini, but also show defeat to everyone
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.MiniLose;
            }

            // Hitman Win
            else if (HitmanWin) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Hitman.Player.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.HitmanWin;
            }

            // Agent Win
            else if (AgentWin) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Agent.Player.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.AgentWin;
            }

            // Jester win
            else if (jesterWin) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Jester.Player.Data);
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

            // Vengeful Romantic win
            else if (RomanticWin) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(VengefulRomantic.Player.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.VRomanticWin;
            }

            // Plaguebearer Win
            else if (PlaguebearerWin) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Plaguebearer.Player.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.PlaguebearerWin;
            }

            // Pestilence Win
            else if (PestilenceWin) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Pestilence.Player.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.PestilenceWin;
            }

            // Werewolf win
            else if (wwWin) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Werewolf.Player.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.WerewolfWin;
            }

            // Jugg win
            else if (JuggernautWin) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Juggernaut.Player.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.JuggernautWin;
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
                CachedPlayerData wpd = new CachedPlayerData(Arsonist.Player.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.ArsonistWin;
            }

            // Vulture win
            else if (vultureWin) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Vulture.Player.Data);
                EndGameResult.CachedWinners.Add(wpd);
                AdditionalTempData.winCondition = WinCondition.VultureWin;
            }

            // Jester win
            else if (prosecutorWin) 
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Lawyer.Player.Data);
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
                        if (p == Lovers.Lover1 || p == Lovers.Lover2)
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(p.Data));
                        else if (p == Pursuer.Player && !Pursuer.Player.Data.IsDead)
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(p.Data));
                        else if (p.IsCrew())
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(p.Data));
                    }
                }
                // Lovers solo win
                else 
                {
                    AdditionalTempData.winCondition = WinCondition.LoversSoloWin;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(Lovers.Lover1.Data));
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(Lovers.Lover2.Data));
                }
            }

            // Jackal win condition (should be implemented using a proper GameOverReason in the future)
            else if (teamJackalWin) 
            {
                // Jackal wins if nobody except jackal is alive
                AdditionalTempData.winCondition = WinCondition.JackalWin;
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                CachedPlayerData wpd = new CachedPlayerData(Jackal.Player.Data);
                wpd.IsImpostor = false;
                EndGameResult.CachedWinners.Add(wpd);
                // If there is a sidekick. The sidekick also wins
                if (Sidekick.Player != null) 
                {
                    CachedPlayerData wpdSidekick = new CachedPlayerData(Sidekick.Player.Data);
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
            if (Lawyer.Player != null && Lawyer.target != null && (!Lawyer.target.Data.IsDead || Lawyer.target == Jester.Player) && !Pursuer.notAckedExiled && !Lawyer.isProsecutor) {
                CachedPlayerData winningClient = null;
                foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator()) 
                {
                    if (winner.PlayerName == Lawyer.target.Data.PlayerName)
                        winningClient = winner;
                }
                if (winningClient != null) 
                { // The Lawyer wins if the client is winning (and alive, but if he wasn't the Lawyer shouldn't exist anymore)
                    if (!EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == Lawyer.Player.Data.PlayerName))
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(Lawyer.Player.Data));
                    AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalLawyerBonusWin); // The Lawyer wins together with the client
                } 
            }

            // Possible Additional winner: Romantic
            if (Romantic.Player != null && Romantic.beloved != null && (!Romantic.beloved.Data.IsDead || Romantic.beloved == Jester.Player) && !VengefulRomantic.notAckedExiled) 
            {
                CachedPlayerData winningClient = null;
                foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator()) 
                {
                    if (winner.PlayerName == Romantic.beloved.Data.PlayerName)
                        winningClient = winner;
                }
                if (winningClient != null) 
                { // The Romantic wins if the beloved is winning (and alive, but if he wasn't the Romantic shouldn't exist anymore)
                    if (!EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == Romantic.Player.Data.PlayerName))
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(Romantic.Player.Data));
                    AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalRomanticBonusWin); // The Romantic wins together with the beloved
                } 
            }

            // Possible Additional winner: Beloved
            if (VengefulRomantic.Player != null && VengefulRomantic.Lover != null && !VengefulRomantic.notAckedExiled) 
            {
                CachedPlayerData winningClient = null;
                foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator()) 
                {
                    if (winner.PlayerName == VengefulRomantic.Lover.Data.PlayerName)
                        winningClient = winner;
                }
                if (winningClient != null) 
                { // The Romantic wins if the beloved is winning (and alive, but if he wasn't the Romantic shouldn't exist anymore)
                    if (!EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == VengefulRomantic.Player.Data.PlayerName))
                        EndGameResult.CachedWinners.Add(new CachedPlayerData(VengefulRomantic.Player.Data));
                    AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalBelovedBonusWin); // The beloved wins together with the romantic
                } 
            }

            // Possible Additional winner: Pursuer
            if (Pursuer.Player != null && !Pursuer.Player.Data.IsDead && !Pursuer.notAckedExiled && !EndGameResult.CachedWinners.ToArray().Any(x => x.IsImpostor)) 
            {
                if (!EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == Pursuer.Player.Data.PlayerName))
                    EndGameResult.CachedWinners.Add(new CachedPlayerData(Pursuer.Player.Data));
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
                    poolablePlayer.cosmetics.nameText.text += $"\n{string.Join("\n", data.Roles.Select(x => Helpers.ColorString(x.Color, x.Name)))}";
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
                textRenderer.text = "Jester Wins!";
                textRenderer.color = Jester.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Jester.Color);
            } 
            else if (AdditionalTempData.winCondition == WinCondition.ArsonistWin) 
            {
                textRenderer.text = "Arsonist Wins!";
                textRenderer.color = Arsonist.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Arsonist.Color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.PlaguebearerWin) 
            {
                textRenderer.text = "Plaguebearer Wins!";
                textRenderer.color = Plaguebearer.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Plaguebearer.Color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.PestilenceWin)
            {
                textRenderer.text = "Pestilence Wins!";
                textRenderer.color = Pestilence.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Pestilence.Color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.VultureWin) 
            {
                textRenderer.text = "Vulture Wins!";
                textRenderer.color = Vulture.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Vulture.Color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.AgentWin) 
            {
                textRenderer.text = "Agent Wins!";
                textRenderer.color = Agent.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Agent.Color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.HitmanWin) 
            {
                textRenderer.text = "Hitman Wins!";
                textRenderer.color = Hitman.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Hitman.Color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.ProsecutorWin) 
            {
                textRenderer.text = "Prosecutor Wins!";
                textRenderer.color = Lawyer.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Lawyer.Color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.LoversTeamWin) 
            {
                textRenderer.text = "Lovers And Crewmates Win!";
                textRenderer.color = Lovers.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Lovers.Color);
            } 
            else if (AdditionalTempData.winCondition == WinCondition.JuggernautWin) 
            {
                textRenderer.text = "Juggernaut Wins!";
                textRenderer.color = Juggernaut.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Juggernaut.Color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.LoversSoloWin) 
            {
                textRenderer.text = "Love Couple Wins!";
                textRenderer.color = Lovers.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Lovers.Color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.VRomanticWin) 
            {
                textRenderer.text = "Vengeful Romantic Wins!";
                textRenderer.color = Romantic.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Romantic.Color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.JackalWin) 
            {
                textRenderer.text = "The Team Jackal Win!";
                textRenderer.color = Jackal.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Jackal.Color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.GlitchWin) 
            {
                textRenderer.text = "The Glitch Wins!";
                textRenderer.color = Glitch.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Glitch.Color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.SerialKillerWin) 
            {
                textRenderer.text = "Serial Kille Wins!";
                textRenderer.color = SerialKiller.Color;
                __instance.BackgroundBar.material.SetColor("_Color", SerialKiller.Color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.WerewolfWin) 
            {
                textRenderer.text = "Werewolf Wins!";
                textRenderer.color = Werewolf.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Werewolf.Color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.MiniLose) 
            {
                textRenderer.text = "Mini died. Everyone lost!";
                textRenderer.color = Mini.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Mini.Color);
            }
            else if (AdditionalTempData.winCondition == WinCondition.Default) 
            {
                switch (OnGameEndPatch.gameOverReason) 
                {
                    case GameOverReason.ImpostorDisconnect:
                        textRenderer.text = "Crewmates Disconnected. Impostors Win!";
                        textRenderer.color = Palette.ImpostorRed;
                        __instance.BackgroundBar.material.SetColor("_Color", Palette.ImpostorRed);
                        break;
                    case GameOverReason.ImpostorByKill:
                        textRenderer.text = "Impostors Win!";
                        textRenderer.color = Palette.ImpostorRed;
                        __instance.BackgroundBar.material.SetColor("_Color", Palette.ImpostorRed);
                        break;
                    case GameOverReason.ImpostorBySabotage:
                        textRenderer.text = "Impostors Win!";
                        textRenderer.color = Palette.ImpostorRed;
                        __instance.BackgroundBar.material.SetColor("_Color", Palette.ImpostorRed);
                        break;
                    case GameOverReason.ImpostorByVote:
                        textRenderer.text = "Impostors Win!";
                        textRenderer.color = Palette.ImpostorRed;
                        __instance.BackgroundBar.material.SetColor("_Color", Palette.ImpostorRed);
                        break;
                    case GameOverReason.HumansByTask:
                        textRenderer.text = "Crewmates Win!";
                        textRenderer.color = Palette.CrewmateBlue;
                        __instance.BackgroundBar.material.SetColor("_Color", Palette.CrewmateBlue);
                        break;
                    case GameOverReason.HumansDisconnect:
                        textRenderer.text = "Impostors Disconnected. Crewmates Win!";
                        textRenderer.color = Palette.CrewmateBlue;
                        __instance.BackgroundBar.material.SetColor("_Color", Palette.CrewmateBlue);
                        break;
                    case GameOverReason.HumansByVote:
                        textRenderer.text = "Crewmates Win!";
                        textRenderer.color = Palette.CrewmateBlue;
                        __instance.BackgroundBar.material.SetColor("_Color", Palette.CrewmateBlue);
                        break;
                }
            }

            foreach (WinCondition cond in AdditionalTempData.additionalWinConditions) 
            {
                if (cond == WinCondition.AdditionalLawyerBonusWin) 
                {
                    textRenderer.text += $"\n{Helpers.ColorString(Lawyer.Color, "The Lawyer wins with the client.")}";
                }
                else if (cond == WinCondition.AdditionalRomanticBonusWin) 
                {
                    textRenderer.text += $"\n{Helpers.ColorString(Romantic.Color, "The Romantic wins with their lover.")}";
                }
                else if (cond == WinCondition.AdditionalBelovedBonusWin) 
                {
                    textRenderer.text += $"\n{Helpers.ColorString(Romantic.Color, "The Beloved wins with their lover.")}";
                }
                else if (cond == WinCondition.AdditionalAlivePursuerWin) 
                {
                    textRenderer.text += $"\n{Helpers.ColorString(Pursuer.Color, "The Pursuer is alive. They also win!")}";
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
                winnersText.AppendLine("<size=105%><color=#00FF00FF><b>★ - Winners List - ★</b></color></size>");
                losersText.AppendLine("<size=105%><color=#FF0000FF><b>◆ - Losers List - ◆</b></color></size>");

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
                Helpers.PreviousEndGameSummary = $"<size=110%>{roleSummaryText.ToString()}</size>";
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
            if (DestroyableSingleton<TutorialManager>.InstanceExists)
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
            if (CheckAndEndGameForPestilenceWin(__instance, statistics)) return false;
            if (CheckAndEndGameForPlaguebearerWin(__instance, statistics)) return false;
            if (CheckAndEndGameForWerewolfWin(__instance, statistics)) return false;
            if (CheckAndEndGameForRomanticWin(__instance, statistics)) return false;
            if (CheckAndEndGameForSerialKillerWin(__instance, statistics)) return false;
            if (CheckAndEndGameForJuggernautWin(__instance, statistics)) return false;
            if (CheckAndEndGameForImpostorWin(__instance, statistics)) return false;
            if (CheckAndEndGameForCrewmateWin(__instance, statistics)) return false;
            if (CheckAndEndGameForHitmanWin(__instance, statistics)) return false;
            if (CheckAndEndGameForAgentWin(__instance, statistics)) return false;
            return false;
        }

        private static bool CheckAndEndGameForMiniLose(ShipStatus __instance) 
        {
            if (Mini.IsMiniLose) 
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.MiniLose, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForJesterWin(ShipStatus __instance) 
        {
            if (Jester.IsJesterWin) 
            {

                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.JesterWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForArsonistWin(ShipStatus __instance) 
        {
            if (Arsonist.IsArsonistWin) 
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ArsonistWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForVultureWin(ShipStatus __instance) 
        {
            if (Vulture.IsVultureWin) 
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
            if (systemType2 != null) 
            {
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
            if (Lawyer.IsProsecutorWin) 
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ProsecutorWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForLoverWin(ShipStatus __instance, PlayerStatistics statistics) 
        {
            if (statistics.TeamLoversAlive == 2 && statistics.TotalAlive <= 3) 
            {
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
            && statistics.VengefulRomanticAlive == 0
            && statistics.HitmanAlive == 0
            && statistics.AgentAlive == 0
            && statistics.PestilenceAlive == 0
            && statistics.PlaguebearerAlive == 0
            && statistics.CrewPowerAlive == 0
            && statistics.JuggernautAlive == 0
            && !(statistics.TeamJackalHasAliveLover 
            && statistics.TeamLoversAlive == 2)) 
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.TeamJackalWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForPestilenceWin(ShipStatus __instance, PlayerStatistics statistics) 
        {
            if (statistics.PestilenceAlive >= statistics.TotalAlive - statistics.PestilenceAlive
            && statistics.TeamImpostorsAlive == 0
            && statistics.GlitchAlive == 0
            && statistics.WerewolfAlive == 0
            && statistics.TeamJackalAlive == 0
            && statistics.SerialKillerAlive == 0
            && statistics.HitmanAlive == 0
            && statistics.AgentAlive == 0
            && statistics.VengefulRomanticAlive == 0
            && statistics.CrewPowerAlive == 0
            && statistics.JuggernautAlive == 0
            && !(statistics.PestilenceAliveHasLover 
            && statistics.TeamLoversAlive == 2)) 
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.PestilenceWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForPlaguebearerWin(ShipStatus __instance, PlayerStatistics statistics)
        {
            if (statistics.PlaguebearerAlive >= statistics.TotalAlive - statistics.PlaguebearerAlive
            && statistics.TeamImpostorsAlive == 0
            && statistics.GlitchAlive == 0
            && statistics.WerewolfAlive == 0
            && statistics.TeamJackalAlive == 0
            && statistics.SerialKillerAlive == 0
            && statistics.HitmanAlive == 0
            && statistics.AgentAlive == 0
            && statistics.VengefulRomanticAlive == 0
            && statistics.CrewPowerAlive == 0
            && statistics.JuggernautAlive == 0
            && !(statistics.PlaguebearerAliveHasLover 
            && statistics.TeamLoversAlive == 2)) 
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.PlaguebearerWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForJuggernautWin(ShipStatus __instance, PlayerStatistics statistics) 
        {
            if (statistics.JuggernautAlive >= statistics.TotalAlive - statistics.JuggernautAlive
            && statistics.TeamImpostorsAlive == 0
            && statistics.GlitchAlive == 0
            && statistics.HitmanAlive == 0
            && statistics.AgentAlive == 0
            && statistics.WerewolfAlive == 0
            && statistics.SerialKillerAlive == 0
            && statistics.VengefulRomanticAlive == 0
            && statistics.PestilenceAlive == 0
            && statistics.PlaguebearerAlive == 0
            && statistics.CrewPowerAlive == 0
            && statistics.TeamJackalAlive == 0
            && !(statistics.JuggernautHasAliveLover 
            && statistics.TeamLoversAlive == 2)) 
            {

                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.JuggernautWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForRomanticWin(ShipStatus __instance, PlayerStatistics statistics) 
        {
            if (statistics.VengefulRomanticAlive >= statistics.TotalAlive - statistics.VengefulRomanticAlive
            && statistics.TeamImpostorsAlive == 0
            && statistics.GlitchAlive == 0
            && statistics.WerewolfAlive == 0
            && statistics.PestilenceAlive == 0
            && statistics.PlaguebearerAlive == 0
            && statistics.SerialKillerAlive == 0
            && statistics.HitmanAlive == 0
            && statistics.AgentAlive == 0
            && statistics.TeamJackalAlive == 0
            && statistics.JuggernautAlive == 0
            && statistics.CrewPowerAlive == 0
            && !(statistics.VengefulRomanticHasAliveLover 
            && statistics.TeamLoversAlive == 2)) 
            {

                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.VRomanticWin, false);
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
            && statistics.PestilenceAlive == 0
            && statistics.HitmanAlive == 0
            && statistics.AgentAlive == 0
            && statistics.PlaguebearerAlive == 0
            && statistics.JuggernautAlive == 0
            && statistics.VengefulRomanticAlive == 0
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
            && statistics.PestilenceAlive == 0
            && statistics.PlaguebearerAlive == 0
            && statistics.HitmanAlive == 0
            && statistics.AgentAlive == 0
            && statistics.JuggernautAlive == 0
            && statistics.VengefulRomanticAlive == 0
            && !(statistics.SerialKillerHasLover
            && statistics.TeamLoversAlive == 2)) 
            {

                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.SerialKillerWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForHitmanWin(ShipStatus __instance, PlayerStatistics statistics) 
        {
            if (statistics.HitmanAlive >= statistics.TotalAlive - statistics.HitmanAlive
            && statistics.TeamImpostorsAlive == 0
            && statistics.CrewPowerAlive == 0
            && statistics.TeamJackalAlive == 0
            && statistics.WerewolfAlive == 0
            && statistics.GlitchAlive == 0
            && statistics.PestilenceAlive == 0
            && statistics.PlaguebearerAlive == 0
            && statistics.SerialKillerAlive == 0
            && statistics.JuggernautAlive == 0
            && !(statistics.HitmanHasAliveLover 
            && statistics.TeamLoversAlive == 2)) 
            {

                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.HitmanWin, false);
                return true;
            }
            return false;
        }
        private static bool CheckAndEndGameForAgentWin(ShipStatus __instance, PlayerStatistics statistics) 
        {
            if (statistics.AgentAlive >= statistics.TotalAlive - statistics.AgentAlive
            && statistics.TeamImpostorsAlive == 0
            && statistics.CrewPowerAlive == 0
            && statistics.TeamJackalAlive == 0
            && statistics.WerewolfAlive == 0
            && statistics.GlitchAlive == 0
            && statistics.PestilenceAlive == 0
            && statistics.PlaguebearerAlive == 0
            && statistics.SerialKillerAlive == 0
            && statistics.JuggernautAlive == 0
            && !(statistics.AgentHasAliveLover 
            && statistics.TeamLoversAlive == 2)) 
            {

                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.AgentWin, false);
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
            && statistics.HitmanAlive == 0
            && statistics.AgentAlive == 0
            && statistics.PestilenceAlive == 0
            && statistics.PlaguebearerAlive == 0
            && statistics.JuggernautAlive == 0
            && statistics.VengefulRomanticAlive == 0
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
            && statistics.HitmanAlive == 0
            && statistics.AgentAlive == 0
            && statistics.PestilenceAlive == 0
            && statistics.PlaguebearerAlive == 0
            && statistics.CrewPowerAlive == 0
            && statistics.VengefulRomanticAlive == 0
            && statistics.JuggernautAlive == 0
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
            && statistics.VengefulRomanticAlive == 0
            && statistics.PestilenceAlive == 0
            && statistics.PlaguebearerAlive == 0
            && statistics.HitmanAlive == 0
            && statistics.AgentAlive == 0
            && statistics.JuggernautAlive == 0
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
            GameManager.Instance.RpcEndGame(GameOverReason.ImpostorBySabotage, false);
            return;
        }

    }

    internal class PlayerStatistics 
    {
        public int TeamImpostorsAlive {get;set;}
        public int TeamJackalAlive {get;set;}
        public int GlitchAlive {get;set;}
        public int HitmanAlive {get;set;}
        public int AgentAlive {get;set;}
        public int PlaguebearerAlive {get;set;}
        public int PestilenceAlive {get;set;}
        public int WerewolfAlive {get;set;}
        public int VengefulRomanticAlive {get;set;}
        public int SerialKillerAlive {get;set;}
        public int JuggernautAlive {get;set;}
        public int CrewPowerAlive {get;set;}
        public int TeamLoversAlive {get;set;}
        public int TotalAlive {get;set;}
        public bool TeamImpostorHasAliveLover {get;set;}
        public bool TeamJackalHasAliveLover {get;set;}
        public bool VengefulRomanticHasAliveLover {get;set;}
        public bool HitmanHasAliveLover {get;set;}
        public bool AgentHasAliveLover {get;set;}
        public bool WerewolfAliveHasLover {get;set;}
        public bool PestilenceAliveHasLover {get;set;}
        public bool PlaguebearerAliveHasLover {get;set;}
        public bool GlitchHasLoverAlive {get;set;}
        public bool JuggernautHasAliveLover {get;set;}
        public bool SerialKillerHasLover {get;set;}

        public PlayerStatistics(ShipStatus __instance) 
        {
            GetPlayerCounts();
        }

        private static bool IsLover(NetworkedPlayerInfo p) 
        {
            return (Lovers.Lover1 != null && Lovers.Lover1.PlayerId == p.PlayerId) || (Lovers.Lover2 != null && Lovers.Lover2.PlayerId == p.PlayerId);
        }

        private void GetPlayerCounts() 
        {
            int numJackalAlive = 0;
            int numGlitchAlive = 0;
            int numImpostorsAlive = 0;
            int numberPestiAlive = 0;
            int numberPlagueAlive = 0;
            int numJuggAlive = 0;
            int numVRomanticsAlive = 0;
            int numWerewolfAlive = 0;
            int numSerialKillerAlive = 0;
            int numHitmanAlive = 0;
            int numAgentAlive = 0;
            int numCrewPowerAlive = 0;
            int numLoversAlive = 0;
            int numTotalAlive = 0;
            bool impLover = false;
            bool RomanticLover = false;
            bool glitchLover = false;
            bool PbLover = false;
            bool PestilenceLover = false;
            bool SKLover = false;
            bool jackalLover = false;
            bool JuggLover = false;
            bool WerewolfLover = false;
            bool hitmanLover = false;
            bool agentLover = false;

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
                        if (Jackal.Player != null && Jackal.Player.PlayerId == playerInfo.PlayerId) 
                        {
                            numJackalAlive++;
                            if (lover) jackalLover = true;
                        }
                        if (Hitman.Player != null && Hitman.Player.PlayerId == playerInfo.PlayerId) 
                        {
                            numHitmanAlive++;
                            if (lover) hitmanLover = true;
                        }
                        if (Agent.Player != null && Agent.Player.PlayerId == playerInfo.PlayerId) 
                        {
                            numAgentAlive++;
                            if (lover) agentLover = true;
                        }
                        if (VengefulRomantic.Player != null && VengefulRomantic.Player.PlayerId == playerInfo.PlayerId) 
                        {
                            numVRomanticsAlive++;
                            if (lover) RomanticLover = true;
                        }
                        if (Juggernaut.Player != null && Juggernaut.Player.PlayerId == playerInfo.PlayerId) 
                        {
                            numJuggAlive++;
                            if (lover) JuggLover = true;
                        }
                        if (Pestilence.Player != null && Pestilence.Player.PlayerId == playerInfo.PlayerId) 
                        {
                            numberPestiAlive++;
                            if (lover) PestilenceLover = true;
                        }
                        if (Plaguebearer.Player != null && Plaguebearer.Player.PlayerId == playerInfo.PlayerId) 
                        {
                            numberPlagueAlive++;
                            if (lover) PbLover = true;
                        }
                        if (Sheriff.Player != null && Sheriff.Player.PlayerId == playerInfo.PlayerId) 
                        {
                            numCrewPowerAlive++;
                        }
                        if (Mayor.Player != null && Mayor.Player.PlayerId == playerInfo.PlayerId) 
                        {
                            numCrewPowerAlive++;
                        }
                        if (Veteran.Player != null && Veteran.Charges > 0 && Veteran.Player.PlayerId == playerInfo.PlayerId) 
                        {
                            numCrewPowerAlive++;
                        }
                        if (Swapper.Player != null && Swapper.charges > 0 && Swapper.Player.PlayerId == playerInfo.PlayerId) 
                        {
                            numCrewPowerAlive++;
                        }
                        if (Tiebreaker.Player != null && Tiebreaker.Player.PlayerId == playerInfo.PlayerId && Tiebreaker.Player.IsCrew()) 
                        {
                            numCrewPowerAlive++;
                        }
                        if (Sidekick.Player != null && Sidekick.Player.PlayerId == playerInfo.PlayerId) 
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
            JuggernautAlive = numJuggAlive;
            HitmanAlive = numHitmanAlive;
            AgentAlive = numAgentAlive;
            PlaguebearerAlive = numberPlagueAlive;
            PestilenceAlive = numberPestiAlive;
            GlitchAlive = numGlitchAlive;
            VengefulRomanticAlive = numVRomanticsAlive;
            SerialKillerAlive = numSerialKillerAlive;
            WerewolfAlive = numWerewolfAlive;
            CrewPowerAlive = numCrewPowerAlive;
            TeamImpostorsAlive = numImpostorsAlive;
            TeamLoversAlive = numLoversAlive;
            TotalAlive = numTotalAlive;
            TeamImpostorHasAliveLover = impLover;
            TeamJackalHasAliveLover = jackalLover;
            GlitchHasLoverAlive = glitchLover;
            PlaguebearerAliveHasLover = PbLover;
            PestilenceAliveHasLover = PestilenceLover;
            HitmanHasAliveLover = hitmanLover;
            AgentHasAliveLover = agentLover;
            VengefulRomanticHasAliveLover = RomanticLover;
            SerialKillerHasLover = SKLover;
            JuggernautHasAliveLover = JuggLover;
            WerewolfAliveHasLover = WerewolfLover;
        }
    }
}
