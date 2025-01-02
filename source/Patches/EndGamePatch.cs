using System.Text;
using TMPro;
using TownOfSushi.Utilities;

namespace TownOfSushi.Patches
{
    static class AdditionalTempData 
    {
        public static WinCondition WinCondition = WinCondition.Default;
        public static List<WinCondition> AdditionalWinConditions = new List<WinCondition>();
        public static List<PlayerRoleInfo> PlayerRoles = new List<PlayerRoleInfo>();

        public static void Clear() 
        {
            PlayerRoles.Clear();
            AdditionalWinConditions.Clear();
            WinCondition = WinCondition.Default;
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
        private static GameOverReason gameOverReason;
        public static void Prefix(AmongUsClient __instance, [HarmonyArgument(0)]ref EndGameResult endGameResult) 
        {
            gameOverReason = endGameResult.GameOverReason;
            if ((int)endGameResult.GameOverReason >= 10) endGameResult.GameOverReason = GameOverReason.ImpostorByKill;
            // Reset zoomed out ghosts
            ToggleZoom(reset: true);
        }
        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)]ref EndGameResult endGameResult) {
            if (CameraEffect.singleton) CameraEffect.singleton.materials.Clear();
            AdditionalTempData.Clear();
            var GameSummaryText = "";
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
                    else if (role.Value == RoleEnum.Snitch) { GameSummaryText += "<color=#" + Colors.Snitch.ToHtmlStringRGBA() + ">Snitch</color> > "; }
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
                if (playerControl.HasTasks())
                {
                    if ((player.TotalTasks - player.TasksLeft)/player.TotalTasks == 1) GameSummaryText += $" | Tasks: " + ColorString(Color.green, $"{player.TotalTasks - player.TasksLeft}/{player.TotalTasks}");
                    else GameSummaryText += $" | Tasks: {player.TotalTasks - player.TasksLeft}/{player.TotalTasks}";
                }
                if (playerControl.IsShielded())
                {
                    GameSummaryText += ColorString(Colors.Medic, $"[<b>+</b>] ");
                }
                if (playerControl.IsBeloved() || playerControl.IsRomantic())
                {
                    GameSummaryText += ColorString(Colors.Romantic, $"[♥] ");
                }
                if (playerControl.IsGATarget())
                {
                    GameSummaryText += ColorString(Colors.GuardianAngel, $"[★] ");
                }
                if (playerControl.IsExeTarget())
                {
                    GameSummaryText += ColorString(Colors.Executioner, $"[⦿] ");
                }
                if (playerControl.IsSpelled())
                {
                    GameSummaryText += ColorString(Colors.Impostor, $"[†] ");
                }
                if (playerControl.Is(RoleEnum.Vulture))
                {
                    GameSummaryText += ColorString(Colors.Vulture, $" ({GetRole<Vulture>(playerControl).BodiesRemainingToWin()} left)");
                }
                if (player.Kills > 0 && !playerControl.Is(Faction.Crewmates))
                {
                    GameSummaryText += ColorString(Colors.Impostor, $"Kills: {player.Kills}");
                }
                if (player.CorrectShot > 0)
                {
                    GameSummaryText += ColorString(Color.green, $"Correct Shots: {player.CorrectShot}");
                }
                if (player.IncorrectShots > 0)
                {
                    GameSummaryText += ColorString(Colors.Impostor, $"Incorrect Shots: {player.IncorrectShots}");
                }
                if (player.CorrectAssassinKills > 0)
                {
                    GameSummaryText += ColorString(Color.green, $"Guesses: {player.CorrectAssassinKills}");
                }
                if (player.IncorrectAssassinKills > 0)
                {
                    GameSummaryText += ColorString(Colors.Impostor, "Misguessed");
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
                AdditionalTempData.PlayerRoles.Add(new AdditionalTempData.PlayerRoleInfo() { PlayerName = playerControl.Data.PlayerName, GameSummaryText = GameSummaryText });
            }

            // Remove Neutrals from winners (if they win, they'll be readded)
            List<PlayerControl> notWinners = new List<PlayerControl>();
            foreach (var role in GetRoles(RoleEnum.Amnesiac))
            {
                var amne = (Amnesiac)role;
                notWinners.Add(amne.Player);
            }
            foreach (var role in GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;
                notWinners.Add(ga.Player);
            }
            foreach (var role in GetRoles(RoleEnum.Romantic))
            {
                var romantic = (Romantic)role;
                notWinners.Add(romantic.Player);
            }
            foreach (var role in GetRoles(RoleEnum.Doomsayer))
            {
                var doom = (Doomsayer)role;
                notWinners.Add(doom.Player);
            }
            foreach (var role in GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner)role;
                notWinners.Add(exe.Player);
            }
            foreach (var role in GetRoles(RoleEnum.Jester))
            {
                var jest = (Jester)role;
                notWinners.Add(jest.Player);
            }
            foreach (var role in GetRoles(RoleEnum.Vulture))
            {
                var vult = (Vulture)role;
                notWinners.Add(vult.Player);
            }
            foreach (var role in GetRoles(RoleEnum.Arsonist))
            {
                var arso = (Arsonist)role;
                notWinners.Add(arso.Player);
            }
            foreach (var role in GetRoles(RoleEnum.Juggernaut))
            {
                var jugg = (Juggernaut)role;
                notWinners.Add(jugg.Player);
            }
            foreach (var role in GetRoles(RoleEnum.Pestilence))
            {
                var pest = (Pestilence)role;
                notWinners.Add(pest.Player);
            }
            foreach (var role in GetRoles(RoleEnum.Plaguebearer))
            {
                var pb = (Plaguebearer)role;
                notWinners.Add(pb.Player);
            }
            foreach (var role in GetRoles(RoleEnum.Glitch))
            {
                var glitch = (Glitch)role;
                notWinners.Add(glitch.Player);
            }
            foreach (var role in GetRoles(RoleEnum.SerialKiller))
            {
                var glitch = (SerialKiller)role;
                notWinners.Add(glitch.Player);
            }
            foreach (var role in GetRoles(RoleEnum.Agent))
            {
                var glitch = (Agent)role;
                notWinners.Add(glitch.Player);
            }
            foreach (var role in GetRoles(RoleEnum.Hitman))
            {
                var glitch = (Hitman)role;
                notWinners.Add(glitch.Player);
            }
            foreach (var role in GetRoles(RoleEnum.Vampire))
            {
                var vamp = (Vampire)role;
                notWinners.Add(vamp.Player);
            }
            foreach (var role in GetRoles(RoleEnum.Werewolf))
            {
                var ww = (Werewolf)role;
                notWinners.Add(ww.Player);
            }

            List<CachedPlayerData> winnersToRemove = new List<CachedPlayerData>();
            foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator()) {
                if (notWinners.Any(x => x.Data.PlayerName == winner.PlayerName)) winnersToRemove.Add(winner);
            }
            foreach (var winner in winnersToRemove) EndGameResult.CachedWinners.Remove(winner);
            bool DoomsayerWin = false;
            foreach (var role in GetRoles(RoleEnum.Doomsayer))
            {
                var Doomsayer = (Doomsayer)role;
                DoomsayerWin = Doomsayer.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.DoomsayerWin;
            }
            bool ExecutionerWin = false;
            foreach (var role in GetRoles(RoleEnum.Executioner))
            {
                var Executioner = (Executioner)role;
                ExecutionerWin = Executioner.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.ExecutionerWin;
            }
            bool JesterWin = false;
            foreach (var role in GetRoles(RoleEnum.Jester))
            {
                var Jester = (Jester)role;
                JesterWin = Jester.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.JesterWin;
            }
            bool AgentWin = false;
            foreach (var role in GetRoles(RoleEnum.Agent))
            {
                var agent = (Agent)role;
                AgentWin = agent.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.AgentWin;
            }
            bool HitmanWin = false;
            foreach (var role in GetRoles(RoleEnum.Hitman))
            {
                var Hitman = (Hitman)role;
                HitmanWin = Hitman.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.HitmanWin;
            }
            bool SerialKillerWin = false;
            foreach (var role in GetRoles(RoleEnum.SerialKiller))
            {
                var SerialKiller = (SerialKiller)role;
                SerialKillerWin = SerialKiller.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.SerialKillerWin;
            }
            
            bool ImpWin = gameOverReason == GameOverReason.ImpostorByKill || gameOverReason == GameOverReason.ImpostorBySabotage  || gameOverReason == GameOverReason.ImpostorByVote;
            bool CrewWin = gameOverReason == GameOverReason.HumansByTask  || gameOverReason == GameOverReason.HumansByVote;

            bool VultureWin = false;
            foreach (var role in GetRoles(RoleEnum.Vulture))
            {
                var Vulture = (Vulture)role;
                VultureWin = Vulture.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.VultureWin;
            }
            bool ArsonistWin = false;
            foreach (var role in GetRoles(RoleEnum.Arsonist))
            {
                var Arsonist = (Arsonist)role;
                ArsonistWin = Arsonist.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.ArsonistWin;
            }
            bool JuggernautWin = false;
            foreach (var role in GetRoles(RoleEnum.Juggernaut))
            {
                var Juggernaut = (Juggernaut)role;
                JuggernautWin = Juggernaut.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.JuggernautWin;
            }
            bool PestilenceWin = false;
            foreach (var role in GetRoles(RoleEnum.Pestilence))
            {
                var Pestilence = (Pestilence)role;
                PestilenceWin = Pestilence.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.PestilenceWin;
            }
            bool PlaguebearerWin = false;
            foreach (var role in GetRoles(RoleEnum.Plaguebearer))
            {
                var Plaguebearer = (Plaguebearer)role;
                PlaguebearerWin = Plaguebearer.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.PlaguebearerWin;
            }
            bool GlitchWin = false;
            foreach (var role in GetRoles(RoleEnum.Glitch))
            {
                var Glitch = (Glitch)role;
                GlitchWin = Glitch.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.GlitchWin;
            }
            bool VampireWin = false;
            foreach (var role in GetRoles(RoleEnum.Vampire))
            {
                var Vampire = (Vampire)role;
                VampireWin = Vampire.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.TeamVampiresWin;
            }
            bool WerewolfWin = false;
            foreach (var role in GetRoles(RoleEnum.Werewolf))
            {
                var Werewolf = (Werewolf)role;
                WerewolfWin = Werewolf.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.WerewolfWin;
            }
            // Doomsayer Win
            if (DoomsayerWin)
            {
                foreach (var role in GetRoles(RoleEnum.Doomsayer))
                {
                    var Doomsayer = (Doomsayer)role;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    CachedPlayerData wpd = new CachedPlayerData(Doomsayer.Player.Data);
                    EndGameResult.CachedWinners.Add(wpd);
                    AdditionalTempData.WinCondition = WinCondition.DoomsayerWin;
                }
            }
            // Agent Win
            else if (AgentWin)
            {
                foreach (var role in GetRoles(RoleEnum.Agent))
                {
                    var Agent = (Agent)role;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    CachedPlayerData wpd = new CachedPlayerData(Agent.Player.Data);
                    EndGameResult.CachedWinners.Add(wpd);
                    AdditionalTempData.WinCondition = WinCondition.AgentWin;
                }
            }

            // Hitman Win
            else if (HitmanWin)
            {
                foreach (var role in GetRoles(RoleEnum.Hitman))
                {
                    var Hitman = (Hitman)role;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    CachedPlayerData wpd = new CachedPlayerData(Hitman.Player.Data);
                    EndGameResult.CachedWinners.Add(wpd);
                    AdditionalTempData.WinCondition = WinCondition.HitmanWin;
                }
            }

            // Serial Killer Win
            else if (SerialKillerWin)
            {
                foreach (var role in GetRoles(RoleEnum.SerialKiller))
                {
                    var SerialKiller = (SerialKiller)role;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    CachedPlayerData wpd = new CachedPlayerData(SerialKiller.Player.Data);
                    EndGameResult.CachedWinners.Add(wpd);
                    AdditionalTempData.WinCondition = WinCondition.SerialKillerWin;
                }
            }

            // Crew/Imp Win
            else if (ImpWin)
            {
                AdditionalTempData.WinCondition = WinCondition.ImpostorWin;  
            }

            else if (CrewWin)
            {
                AdditionalTempData.WinCondition = WinCondition.CrewmateWin;  
            }

            // Executioner Win
            else if (ExecutionerWin)
            {
                foreach (var role in GetRoles(RoleEnum.Executioner))
                {
                    var Executioner = (Executioner)role;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    CachedPlayerData wpd = new CachedPlayerData(Executioner.Player.Data);
                    EndGameResult.CachedWinners.Add(wpd);
                    AdditionalTempData.WinCondition = WinCondition.ExecutionerWin;
                }
            }
            // Jester Win
            else if (JesterWin)
            {
                foreach (var role in GetRoles(RoleEnum.Jester))
                {
                    var Jester = (Jester)role;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    CachedPlayerData wpd = new CachedPlayerData(Jester.Player.Data);
                    EndGameResult.CachedWinners.Add(wpd);
                    AdditionalTempData.WinCondition = WinCondition.JesterWin;
                }
            }
            // Vulture Win
            else if (VultureWin)
            {
                foreach (var role in GetRoles(RoleEnum.Vulture))
                {
                    var Vulture = (Vulture)role;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    CachedPlayerData wpd = new CachedPlayerData(Vulture.Player.Data);
                    EndGameResult.CachedWinners.Add(wpd);
                    AdditionalTempData.WinCondition = WinCondition.VultureWin;
                }
            }
            // Arsonist Win
            else if (ArsonistWin)
            {
                foreach (var role in GetRoles(RoleEnum.Arsonist))
                {
                    var Arsonist = (Arsonist)role;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    CachedPlayerData wpd = new CachedPlayerData(Arsonist.Player.Data);
                    EndGameResult.CachedWinners.Add(wpd);
                    wpd.IsImpostor = false;
                    AdditionalTempData.WinCondition = WinCondition.ArsonistWin;
                }
            }
            // Juggernaut Win
            else if (JuggernautWin)
            {
                foreach (var role in GetRoles(RoleEnum.Juggernaut))
                {
                    var Juggernaut = (Juggernaut)role;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    CachedPlayerData wpd = new CachedPlayerData(Juggernaut.Player.Data);
                    EndGameResult.CachedWinners.Add(wpd);
                    wpd.IsImpostor = false;
                    AdditionalTempData.WinCondition = WinCondition.JuggernautWin;
                }
            }
            // Pestilence Win
            else if (PestilenceWin)
            {
                foreach (var role in GetRoles(RoleEnum.Pestilence))
                {
                    var Pestilence = (Pestilence)role;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    CachedPlayerData wpd = new CachedPlayerData(Pestilence.Player.Data);
                    EndGameResult.CachedWinners.Add(wpd);
                    wpd.IsImpostor = false;
                    AdditionalTempData.WinCondition = WinCondition.PestilenceWin;
                }
            }
            // Plaguebearer Win
            else if (PlaguebearerWin)
            {
                foreach (var role in GetRoles(RoleEnum.Plaguebearer))
                {
                    var Plaguebearer = (Plaguebearer)role;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    CachedPlayerData wpd = new CachedPlayerData(Plaguebearer.Player.Data);
                    EndGameResult.CachedWinners.Add(wpd);
                    wpd.IsImpostor = false;
                    AdditionalTempData.WinCondition = WinCondition.PlaguebearerWin;
                }
            }
            // Glitch Win
            else if (GlitchWin)
            {
                foreach (var role in GetRoles(RoleEnum.Glitch))
                {
                    var Glitch = (Glitch)role;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    CachedPlayerData wpd = new CachedPlayerData(Glitch.Player.Data);
                    EndGameResult.CachedWinners.Add(wpd);
                    wpd.IsImpostor = false;
                    AdditionalTempData.WinCondition = WinCondition.GlitchWin;
                }
            }
            // Vampire Win
            else if (VampireWin)
            {
                foreach (var role in GetRoles(RoleEnum.Vampire))
                {
                    var Vampire = (Vampire)role;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    CachedPlayerData wpd = new CachedPlayerData(Vampire.Player.Data);
                    EndGameResult.CachedWinners.Add(wpd);
                    wpd.IsImpostor = false;
                    AdditionalTempData.WinCondition = WinCondition.TeamVampiresWin;
                }
            }
            // Werewolf Win
            else if (WerewolfWin)
            {
                foreach (var role in GetRoles(RoleEnum.Werewolf))
                {
                    var Werewolf = (Werewolf)role;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    CachedPlayerData wpd = new CachedPlayerData(Werewolf.Player.Data);
                    EndGameResult.CachedWinners.Add(wpd);
                    wpd.IsImpostor = false;
                    AdditionalTempData.WinCondition = WinCondition.WerewolfWin;
                }
            }

            // Possible Additional winner: Guardian Angel
            foreach (var role in GetRoles(RoleEnum.GuardianAngel))
            {
                CachedPlayerData winningClient = null;
                var ga = (GuardianAngel)role;
                if (ga.Player != null && !ga.Player.Data.Disconnected && ga.target != null)
                {
                    foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator()) 
                    {
                        if (winner.PlayerName == ga.target.Data.PlayerName)
                            winningClient = winner;
                    }
                    if (winningClient != null) { // The GA wins if the client is winning
                        if (!EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == ga.Player.Data.PlayerName))
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(ga.Player.Data));
                        AdditionalTempData.AdditionalWinConditions.Add(WinCondition.AdditionalGuardianAngelWin); // The GA wins together with the client
                    }
                }
            }
            
            // Possible Additional winner: Romantic
            foreach (var role in GetRoles(RoleEnum.Romantic))
            {
                CachedPlayerData winningClient = null;
                var romantic = (Romantic)role;
                if (romantic.Player != null && !romantic.Player.Data.Disconnected && romantic.Beloved != null)
                {
                    foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator()) 
                    {
                        if (winner.PlayerName == romantic.Beloved.Data.PlayerName)
                            winningClient = winner;
                    }
                    if (winningClient != null) { // The Romantic wins if the client is winning
                        if (!EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == romantic.Player.Data.PlayerName))
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(romantic.Player.Data));
                        AdditionalTempData.AdditionalWinConditions.Add(WinCondition.AdditionalRomanticWin); // The Romantic wins together with the client
                    }
                }
            }
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
                Object.Destroy(pb.gameObject);
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
            }

            // Additional code
            GameObject bonusText = Object.Instantiate(__instance.WinText.gameObject);
            bonusText.transform.position = new Vector3(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.5f, __instance.WinText.transform.position.z);
            bonusText.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            TMPro.TMP_Text textRenderer = bonusText.GetComponent<TMPro.TMP_Text>();
            textRenderer.text = "";

            if (AdditionalTempData.WinCondition == WinCondition.JesterWin)
            {
                textRenderer.text = "Jester Wins!";
                textRenderer.color = Colors.Jester;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Jester);
            }
            if (AdditionalTempData.WinCondition == WinCondition.ExecutionerWin)
            {
                textRenderer.text = "Executioner Wins!";
                textRenderer.color = Colors.Executioner;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Executioner);
            }
            if (AdditionalTempData.WinCondition == WinCondition.DoomsayerWin)
            {
                textRenderer.text = "Doomsayer Wins!";
                textRenderer.color = Colors.Doomsayer;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Doomsayer);
            }
            if (AdditionalTempData.WinCondition == WinCondition.VultureWin)
            {
                textRenderer.text = "Vulture Wins!";
                textRenderer.color = Colors.Vulture;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Vulture);
            }
            if (AdditionalTempData.WinCondition == WinCondition.AgentWin)
            {
                textRenderer.text = "Agent Wins!";
                textRenderer.color = Colors.Agent;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Agent);
            }
            if (AdditionalTempData.WinCondition == WinCondition.HitmanWin)
            {
                textRenderer.text = "Hitman Wins!";
                textRenderer.color = Colors.Hitman;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Hitman);
            }
            if (AdditionalTempData.WinCondition == WinCondition.SerialKillerWin)
            {
                textRenderer.text = "Serial Killer Wins!";
                textRenderer.color = Colors.SerialKiller;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.SerialKiller);
            }
            if (AdditionalTempData.WinCondition == WinCondition.CrewmateWin)
            {
                textRenderer.text = "Crewmates Win!";
                textRenderer.color = Colors.Crewmate;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Crewmate);
            }
            if (AdditionalTempData.WinCondition == WinCondition.ImpostorWin)
            {
                textRenderer.text = "Impostors Win!";
                textRenderer.color = Colors.Impostor;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Impostor);
            }
            if (AdditionalTempData.WinCondition == WinCondition.TeamVampiresWin)
            {
                textRenderer.text = "Vampires Win!";
                textRenderer.color = Colors.Vampire;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Vampire);
            }
            if (AdditionalTempData.WinCondition == WinCondition.GlitchWin)
            {
                textRenderer.text = "Glitch Wins!";
                textRenderer.color = Colors.Glitch;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Glitch);
            }
            if (AdditionalTempData.WinCondition == WinCondition.JuggernautWin)
            {
                textRenderer.text = "Juggernaut Wins!";
                textRenderer.color = Colors.Juggernaut;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Juggernaut);
            }
            if (AdditionalTempData.WinCondition == WinCondition.ArsonistWin)
            {
                textRenderer.text = "Arsonist Wins!";
                textRenderer.color = Colors.Arsonist;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Arsonist);
            }
            if (AdditionalTempData.WinCondition == WinCondition.PlaguebearerWin)
            {
                textRenderer.text = "Plaguebearer Wins!";
                textRenderer.color = Colors.Plaguebearer;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Plaguebearer);
            }
            if (AdditionalTempData.WinCondition == WinCondition.PestilenceWin)
            {
                textRenderer.text = "Pestilence Wins!";
                textRenderer.color = Colors.Pestilence;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Pestilence);
            }
            if (AdditionalTempData.WinCondition == WinCondition.WerewolfWin)
            {
                textRenderer.text = "Werewolf Wins!";
                textRenderer.color = Colors.Werewolf;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Werewolf);
            }

            foreach (WinCondition cond in AdditionalTempData.AdditionalWinConditions) 
            {
                if (cond == WinCondition.AdditionalRomanticWin) {
                    textRenderer.text += $"\n{ColorString(Colors.Romantic, "The Romantic wins with the beloved!")}";
                } else if (cond == WinCondition.AdditionalGuardianAngelWin) {
                    textRenderer.text += $"\n{ColorString(Colors.GuardianAngel, "The Guardian Angel wins with the target!")}";
                }
            }

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

            roleSummaryText.AppendLine("<size=125%><u><b>Game Summary</b></u>:</size>");
            roleSummaryText.AppendLine();
            winnersText.AppendLine("<size=105%><color=#00FF00FF><b>◈ - Winners - ◈</b></color></size>");
            losersText.AppendLine("<size=105%><color=#FF0000FF><b>◆ - Losers - ◆</b></color></size>");

            foreach (var data in AdditionalTempData.PlayerRoles)
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

    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))] 
    class CheckEndCriteriaPatch 
    {
        public static bool Prefix(ShipStatus __instance) 
        {
            if (!GameData.Instance) return false;
            if (DestroyableSingleton<TutorialManager>.InstanceExists) // InstanceExists | Don't check Custom Criteria when in Tutorial
                return true;
            var statistics = new PlayerStatistics(__instance);
            if (CheckAndEndGameForJesterWin(__instance)) return false;
            if (CheckAndEndGameForExecutionerWin(__instance)) return false;
            if (CheckAndEndGameForDoomsayerWin(__instance)) return false;
            if (CheckAndEndGameForVultureWin(__instance)) return false;
            if (CheckAndEndGameForSabotageWin(__instance)) return false;
            if (CheckAndEndGameForSabotageWin(__instance)) return false;
            if (CheckAndEndGameForTaskWin(__instance)) return false;
            if (CheckAndEndGameForTeamVampiresWin(__instance, statistics)) return false;
            if (CheckAndEndGameForGlitchWin(__instance, statistics)) return false;
            if (CheckAndEndGameForJuggernautWin(__instance, statistics)) return false;
            if (CheckAndEndGameForAgentWin(__instance, statistics)) return false;
            if (CheckAndEndGameForHitmanWin(__instance, statistics)) return false;
            if (CheckAndEndGameForSerialKillerWin(__instance, statistics)) return false;
            if (CheckAndEndGameForArsonistWin(__instance, statistics)) return false;
            if (CheckAndEndGameForPlaguebearerWin(__instance, statistics)) return false;
            if (CheckAndEndGameForPestilenceWin(__instance, statistics)) return false;
            if (CheckAndEndGameForWerewolfWin(__instance, statistics)) return false;
            if (CheckAndEndGameForImpostorWin(__instance, statistics)) return false;
            if (CheckAndEndGameForCrewmateWin(__instance, statistics)) return false;
            return false;
        }

        private static bool CheckAndEndGameForJesterWin(ShipStatus __instance) 
        {
            foreach (var role in GetRoles(RoleEnum.Jester))
            {
                var jester = (Jester)role;
                if (jester.VotedOut)
                {
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.JesterWin, false);
                    return true;
                }
            }
            return false;
        }
        private static bool CheckAndEndGameForExecutionerWin(ShipStatus __instance) {
            foreach (var role in GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner)role;
                if (exe.TargetVotedOut)
                {
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ExecutionerWin, false);
                    return true;
                }
            }
            return false;
        }
        private static bool CheckAndEndGameForDoomsayerWin(ShipStatus __instance) {
            foreach (var role in GetRoles(RoleEnum.Doomsayer))
            {
                var doom = (Doomsayer)role;
                if (doom.WonByGuessing)
                {
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.DoomsayerWin, false);
                    return true;
                }
            }
            return false;
        }
        private static bool CheckAndEndGameForVultureWin(ShipStatus __instance) {
            foreach (var role in GetRoles(RoleEnum.Vulture))
            {
                var vulture = (Vulture)role;
                if (vulture.WonByEating)
                {
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.VultureWin, false);
                    return true;
                }
            }
            return false;
        }
        private static bool CheckAndEndGameForSabotageWin(ShipStatus __instance) 
        {
            if (ShipStatus.Instance.Systems == null) return false;
            var systemType = ShipStatus.Instance.Systems.ContainsKey(SystemTypes.LifeSupp) ? ShipStatus.Instance.Systems[SystemTypes.LifeSupp] : null;
            if (systemType != null) {
                LifeSuppSystemType lifeSuppSystemType = systemType.TryCast<LifeSuppSystemType>();
                if (lifeSuppSystemType != null && lifeSuppSystemType.Countdown < 0f) {
                    EndGameForOxygenSabotage();
                    lifeSuppSystemType.Countdown = 10000f;
                    return true;
                }
            }
            var systemType2 = ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor) ? ShipStatus.Instance.Systems[SystemTypes.Reactor] : null;
            if (systemType2 == null) {
                systemType2 = ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Laboratory) ? ShipStatus.Instance.Systems[SystemTypes.Laboratory] : null;
            }
            if (systemType2 != null) {
                ICriticalSabotage criticalSystem = systemType2.TryCast<ICriticalSabotage>();
                if (criticalSystem != null && criticalSystem.Countdown < 0f) {
                    EndGameForReactorSabotage();
                    criticalSystem.ClearSabotage();
                    return true;
                }
            }
            return false;
        }
        private static bool CheckAndEndGameForTaskWin(ShipStatus __instance) 
        {
            if (GameData.Instance.TotalTasks > 0 && GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks) {
                GameManager.Instance.RpcEndGame(GameOverReason.HumansByTask, false);
                return true;
            }
            return false;
        }
        private static bool CheckAndEndGameForTeamVampiresWin(ShipStatus __instance, PlayerStatistics statistics) {
            if (statistics.VampiresAlive >= statistics.TotalAlive - statistics.VampiresAlive 
            && statistics.SerialKillerAlive == 0
            && statistics.TeamImpostorsAlive == 0 
            && statistics.HitmanAlive == 0 
            && statistics.AgentAlive == 0 
            && statistics.WerewolfAlive == 0 
            && statistics.GlitchAlive == 0 
            && statistics.CrewKillingAlive == 0 
            && statistics.JuggernautAlive == 0 
            && statistics.ArsonistAlive == 0 
            && statistics.PlaguebearerAlive == 0 
            && statistics.PestilenceAlive == 0) 
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.TeamVampiresWin, false);
                return true;
            }
            return false;
        }

        private static bool CheckAndEndGameForGlitchWin(ShipStatus __instance, PlayerStatistics statistics) {
            if (statistics.GlitchAlive >= statistics.TotalAlive - statistics.GlitchAlive 
            && statistics.SerialKillerAlive == 0
            && statistics.HitmanAlive == 0 
            && statistics.TeamImpostorsAlive == 0 
            && statistics.AgentAlive == 0
            && statistics.VampiresAlive == 0 
            && statistics.WerewolfAlive == 0 
            && statistics.CrewKillingAlive == 0 
            && statistics.JuggernautAlive == 0 
            && statistics.ArsonistAlive == 0 
            && statistics.PlaguebearerAlive == 0 
            && statistics.PestilenceAlive == 0) 
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.GlitchWin, false);
                return true;
            }
            return false;
        }
        private static bool CheckAndEndGameForSerialKillerWin(ShipStatus __instance, PlayerStatistics statistics) {
            if (statistics.SerialKillerAlive >= statistics.TotalAlive - statistics.SerialKillerAlive 
            && statistics.HitmanAlive == 0 
            && statistics.GlitchAlive == 0 
            && statistics.TeamImpostorsAlive == 0 
            && statistics.AgentAlive == 0 
            && statistics.VampiresAlive == 0 
            && statistics.WerewolfAlive == 0 
            && statistics.CrewKillingAlive == 0 
            && statistics.JuggernautAlive == 0 
            && statistics.ArsonistAlive == 0 
            && statistics.PlaguebearerAlive == 0
            && statistics.PestilenceAlive == 0) {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.SerialKillerWin, false);
                return true;
            }
            return false;
        }
        private static bool CheckAndEndGameForHitmanWin(ShipStatus __instance, PlayerStatistics statistics) {
            if (statistics.HitmanAlive >= statistics.TotalAlive - statistics.HitmanAlive 
            && statistics.SerialKillerAlive == 0 
            && statistics.GlitchAlive == 0 
            && statistics.TeamImpostorsAlive == 0 
            && statistics.AgentAlive == 0 
            && statistics.VampiresAlive == 0 
            && statistics.WerewolfAlive == 0 
            && statistics.CrewKillingAlive == 0 
            && statistics.JuggernautAlive == 0 
            && statistics.ArsonistAlive == 0 
            && statistics.PlaguebearerAlive == 0 
            && statistics.PestilenceAlive == 0 ) 
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.HitmanWin, false);
                return true;
            }
            return false;
        }
        private static bool CheckAndEndGameForAgentWin(ShipStatus __instance, PlayerStatistics statistics) {
            if (statistics.AgentAlive >= statistics.TotalAlive - statistics.AgentAlive 
            && statistics.SerialKillerAlive == 0 
            && statistics.CrewKillingAlive == 0 
            && statistics.TeamImpostorsAlive == 0 
            && statistics.VampiresAlive == 0 
            && statistics.WerewolfAlive == 0 
            && statistics.GlitchAlive == 0 
            && statistics.JuggernautAlive == 0 
            && statistics.ArsonistAlive == 0 
            && statistics.PlaguebearerAlive == 0 
            && statistics.PestilenceAlive == 0) 
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.AgentWin, false);
                return true;
            }
            return false;
        }
        private static bool CheckAndEndGameForJuggernautWin(ShipStatus __instance, PlayerStatistics statistics) {
            if (statistics.JuggernautAlive >= statistics.TotalAlive - statistics.JuggernautAlive 
            && statistics.SerialKillerAlive == 0 
            && statistics.CrewKillingAlive == 0 
            && statistics.TeamImpostorsAlive == 0 
            && statistics.HitmanAlive == 0 
            && statistics.AgentAlive == 0 
            && statistics.VampiresAlive == 0 
            && statistics.GlitchAlive == 0 
            && statistics.WerewolfAlive == 0 
            && statistics.ArsonistAlive == 0 
            && statistics.PlaguebearerAlive == 0 
            && statistics.PestilenceAlive == 0) 
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.JuggernautWin, false);
                return true;
            }
            return false;
        }
        private static bool CheckAndEndGameForArsonistWin(ShipStatus __instance, PlayerStatistics statistics) {
            if (statistics.ArsonistAlive >= statistics.TotalAlive - statistics.ArsonistAlive 
            && statistics.TeamImpostorsAlive == 0 
            && statistics.CrewKillingAlive == 0 
            && statistics.HitmanAlive == 0 
            && statistics.AgentAlive == 0 
            && statistics.VampiresAlive == 0 
            && statistics.GlitchAlive == 0 
            && statistics.JuggernautAlive == 0 
            && statistics.WerewolfAlive == 0 
            && statistics.PlaguebearerAlive == 0 
            && statistics.PestilenceAlive == 0 
            && statistics.SerialKillerAlive == 0) 
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ArsonistWin, false);
                return true;
            }
            return false;
        }
        private static bool CheckAndEndGameForPlaguebearerWin(ShipStatus __instance, PlayerStatistics statistics) {
            if (statistics.PlaguebearerAlive >= statistics.TotalAlive - statistics.PlaguebearerAlive 
            && statistics.TeamImpostorsAlive == 0 
            && statistics.CrewKillingAlive == 0 
            && statistics.HitmanAlive == 0 
            && statistics.AgentAlive == 0 
            && statistics.VampiresAlive == 0 
            && statistics.GlitchAlive == 0 
            && statistics.JuggernautAlive == 0 
            && statistics.ArsonistAlive == 0 
            && statistics.WerewolfAlive == 0 
            && statistics.PestilenceAlive == 0 
            && statistics.SerialKillerAlive == 0) 
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.PlaguebearerWin, false);
                return true;
            }
            return false;
        }
        private static bool CheckAndEndGameForPestilenceWin(ShipStatus __instance, PlayerStatistics statistics) {
            if (statistics.PestilenceAlive >= statistics.TotalAlive - statistics.PestilenceAlive 
            && statistics.TeamImpostorsAlive == 0 
            && statistics.CrewKillingAlive == 0 
            && statistics.HitmanAlive == 0 
            && statistics.AgentAlive == 0 
            && statistics.VampiresAlive == 0 
            && statistics.GlitchAlive == 0 
            && statistics.JuggernautAlive == 0 
            && statistics.ArsonistAlive == 0 
            && statistics.PlaguebearerAlive == 0 
            && statistics.WerewolfAlive == 0 
            && statistics.SerialKillerAlive == 0) 
            {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.PestilenceWin, false);
                return true;
            }
            return false;
        }
        private static bool CheckAndEndGameForWerewolfWin(ShipStatus __instance, PlayerStatistics statistics) {
            if (statistics.WerewolfAlive >= statistics.TotalAlive - statistics.WerewolfAlive 
            && statistics.CrewKillingAlive == 0 
            && statistics.TeamImpostorsAlive == 0 
            && statistics.HitmanAlive == 0 
            && statistics.AgentAlive == 0 
            && statistics.VampiresAlive == 0
            && statistics.GlitchAlive == 0 
            && statistics.JuggernautAlive == 0 
            && statistics.ArsonistAlive == 0 
            && statistics.PlaguebearerAlive == 0 
            && statistics.PestilenceAlive == 0 
            && statistics.SerialKillerAlive == 0) {
                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.WerewolfWin, false);
                return true;
            }
            return false;
        }
        private static bool CheckAndEndGameForImpostorWin(ShipStatus __instance, PlayerStatistics statistics) 
        {
            if (statistics.TeamImpostorsAlive >= statistics.TotalAlive - statistics.TeamImpostorsAlive 
            && statistics.CrewKillingAlive == 0 
            && statistics.VampiresAlive == 0 
            && statistics.HitmanAlive == 0 
            && statistics.AgentAlive == 0 
            && statistics.GlitchAlive == 0 
            && statistics.JuggernautAlive == 0 
            && statistics.ArsonistAlive == 0 
            && statistics.PlaguebearerAlive == 0 
            && statistics.PestilenceAlive == 0 
            && statistics.WerewolfAlive == 0 
            && statistics.SerialKillerAlive == 0) 
            {
                GameOverReason endReason;
                switch (GameData.LastDeathReason) {
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
            && statistics.VampiresAlive == 0 
            && statistics.GlitchAlive == 0 
            && statistics.JuggernautAlive == 0 
            && statistics.HitmanAlive == 0 
            && statistics.AgentAlive == 0  
            && statistics.ArsonistAlive == 0 
            && statistics.PlaguebearerAlive == 0 
            && statistics.PestilenceAlive == 0 
            && statistics.WerewolfAlive == 0 
            && statistics.SerialKillerAlive == 0) 
            {
                GameManager.Instance.RpcEndGame(GameOverReason.HumansByVote, false);
                return true;
            }
            return false;
        }

        private static void EndGameForOxygenSabotage()
        {
            GameManager.Instance.RpcEndGame(GameOverReason.ImpostorBySabotage, false);
        }

        private static void EndGameForReactorSabotage()
        {
            GameManager.Instance.RpcEndGame(GameOverReason.ImpostorBySabotage, false);
        }
    }

    internal class PlayerStatistics {
        public int TeamImpostorsAlive {get;set;}
        public int VampiresAlive {get;set;}
        public int GlitchAlive {get;set;}
        public int JuggernautAlive {get;set;}
        public int ArsonistAlive {get;set;}
        public int PlaguebearerAlive {get;set;}
        public int PestilenceAlive {get;set;}
        public int CrewKillingAlive {get;set;}
        public int SerialKillerAlive {get;set;}
        public int HitmanAlive {get;set;}
        public int AgentAlive {get;set;}
        public int WerewolfAlive {get;set;}
        public int TotalAlive {get;set;}

        public PlayerStatistics(ShipStatus __instance) {
            GetPlayerCounts();
        }

        private void GetPlayerCounts() {
            int TeamImpostorsAlive = 0;
            int VampiresAlive = 0;
            int GlitchAlive = 0;
            int JuggernautAlive = 0;
            int ArsonistAlive = 0;
            int PlaguebearerAlive = 0;
            int CrewKillingAlive = 0;
            int SerialKillerAlive = 0;
            int HitmanAlive = 0;
            int AgentAlive = 0;
            int PestilenceAlive = 0;
            int WerewolfAlive = 0;
            int TotalAlive = 0;

            foreach (var playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator()) {
                if (!playerInfo.Disconnected)
                {
                    if (!playerInfo.IsDead)
                    {
                        TotalAlive++;

                        if (playerInfo.Role.IsImpostor) {
                            TeamImpostorsAlive++;
                        }
                        foreach (var role in GetRoles(RoleEnum.Vampire))
                        {
                            var Vampire = (Vampire)role;
                            if (Vampire.Player != null && Vampire.Player.PlayerId == playerInfo.PlayerId) {
                                VampiresAlive++;
                            }
                        }
                        foreach (var role in GetRoles(RoleEnum.Vigilante))
                        {
                            var Vigilante = (Vigilante)role;
                            if (Vigilante.Player != null && Vigilante.Player.PlayerId == playerInfo.PlayerId) {
                                CrewKillingAlive++;
                            }
                        }
                        foreach (var role in GetRoles(RoleEnum.Veteran))
                        {
                            var Veteran = (Veteran)role;
                            if (Veteran.Player != null && Veteran.Player.PlayerId == playerInfo.PlayerId && Veteran.UsesLeft > 0) {
                                CrewKillingAlive++;
                            }
                        }
                        foreach (var role in GetRoles(RoleEnum.Swapper))
                        {
                            var Swapper = (Swapper)role;
                            if (Swapper.Player != null && Swapper.Player.PlayerId == playerInfo.PlayerId) {
                                CrewKillingAlive++;
                            }
                        }
                        foreach (var role in GetRoles(RoleEnum.Hunter))
                        {
                            var Hunter = (Hunter)role;
                            if (Hunter.Player != null && Hunter.Player.PlayerId == playerInfo.PlayerId && Hunter.MaxUses > 0 || (Hunter.StalkedPlayer != null && !Hunter.StalkedPlayer.Data.IsDead && !Hunter.StalkedPlayer.Data.Disconnected && Hunter.StalkedPlayer.Is(RoleAlignment.NeutralKilling)) ||
                                Hunter.CaughtPlayers.Count(player => !player.Data.IsDead && !player.Data.Disconnected && player.Is(RoleAlignment.NeutralKilling)) > 0)
                            {
                                CrewKillingAlive++;
                            }
                        }
                        foreach (var role in GetRoles(RoleEnum.Glitch))
                        {
                            var Glitch = (Glitch)role;
                            if (Glitch.Player != null && Glitch.Player.PlayerId == playerInfo.PlayerId) {
                                GlitchAlive++;
                            }
                        }
                        foreach (var role in GetRoles(RoleEnum.Agent))
                        {
                            var Agent = (Agent)role;
                            if (Agent.Player != null && Agent.Player.PlayerId == playerInfo.PlayerId) {
                                AgentAlive++;
                            }
                        }
                        foreach (var role in GetRoles(RoleEnum.Hitman))
                        {
                            var Hitman = (Hitman)role;
                            if (Hitman.Player != null && Hitman.Player.PlayerId == playerInfo.PlayerId) {
                                HitmanAlive++;
                            }
                        }
                        foreach (var role in GetRoles(RoleEnum.SerialKiller))
                        {
                            var SerialKiller = (SerialKiller)role;
                            if (SerialKiller.Player != null && SerialKiller.Player.PlayerId == playerInfo.PlayerId) {
                                SerialKillerAlive++;
                            }
                        }
                        foreach (var role in GetRoles(RoleEnum.Juggernaut))
                        {
                            var Juggernaut = (Juggernaut)role;
                            if (Juggernaut.Player != null && Juggernaut.Player.PlayerId == playerInfo.PlayerId) {
                                JuggernautAlive++;
                            }
                        }
                        foreach (var role in GetRoles(RoleEnum.Arsonist))
                        {
                            var Arsonist = (Arsonist)role;
                            if (Arsonist.Player != null && Arsonist.Player.PlayerId == playerInfo.PlayerId) {
                                ArsonistAlive++;
                            }
                        }
                        foreach (var role in GetRoles(RoleEnum.Plaguebearer))
                        {
                            var Plaguebearer = (Plaguebearer)role;
                            if (Plaguebearer.Player != null && Plaguebearer.Player.PlayerId == playerInfo.PlayerId) {
                                PlaguebearerAlive++;
                            }
                        }
                        foreach (var role in GetRoles(RoleEnum.Pestilence))
                        {
                            var Pestilence = (Pestilence)role;
                            if (Pestilence.Player != null && Pestilence.Player.PlayerId == playerInfo.PlayerId) {
                                PestilenceAlive++;
                            }
                        }
                        foreach (var role in GetRoles(RoleEnum.Werewolf))
                        {
                            var Werewolf = (Werewolf)role;
                            if (Werewolf.Player != null && Werewolf.Player.PlayerId == playerInfo.PlayerId) {
                                WerewolfAlive++;
                            }
                        }
                    }
                }
            }


            this.TeamImpostorsAlive = TeamImpostorsAlive;
            this.VampiresAlive = VampiresAlive;
            this.GlitchAlive = GlitchAlive;
            this.JuggernautAlive = JuggernautAlive;
            this.ArsonistAlive = ArsonistAlive;
            this.CrewKillingAlive = CrewKillingAlive;
            this.PlaguebearerAlive = PlaguebearerAlive;
            this.SerialKillerAlive = SerialKillerAlive;
            this.AgentAlive = AgentAlive;
            this.HitmanAlive = HitmanAlive;
            this.PestilenceAlive = PestilenceAlive;
            this.WerewolfAlive = WerewolfAlive;
            this.TotalAlive = TotalAlive;
        }
    }
}