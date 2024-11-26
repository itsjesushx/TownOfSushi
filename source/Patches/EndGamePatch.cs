using System.Text;
using TownOfSushi.Utilities;

namespace TownOfSushi.Patches
{
    static class AdditionalTempData 
    {
        public static WinCondition winCondition = WinCondition.Default;
        public static List<WinCondition> additionalWinConditions = new List<WinCondition>();
        public static List<PlayerRoleInfo> playerRoles = new List<PlayerRoleInfo>();

        public static void clear() 
        {
            playerRoles.Clear();
            additionalWinConditions.Clear();
            winCondition = WinCondition.Default;
        }

        internal class PlayerRoleInfo 
        {
            public string PlayerName { get; set; }
            public string Role { get; set; }
            public bool ExeTarget { get; set; }
            public bool Shielded { get; set;}
            public bool GATarget { get; set; }
            public bool Loved { get; set; }
        }

        internal class Winners
        {
            public string PlayerName { get; set; }
            public RoleEnum Role { get; set; }
        }
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public class OnGameEndPatch {
        private static GameOverReason gameOverReason;
        public static void Prefix(AmongUsClient __instance, [HarmonyArgument(0)]ref EndGameResult endGameResult) {
            gameOverReason = endGameResult.GameOverReason;
            if ((int)endGameResult.GameOverReason >= 10) endGameResult.GameOverReason = GameOverReason.ImpostorByKill;
        }

        public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)]ref EndGameResult endGameResult) {
            if (CameraEffect.singleton) CameraEffect.singleton.materials.Clear();
            AdditionalTempData.clear();
            
            var playerRole = "";
            foreach (var playerControl in PlayerControl.AllPlayerControls) 
            {
                playerRole = "";

                //Extra information
                bool loved = GetRole<Romantic>(playerControl).Beloved || GetRole<Romantic>(playerControl).Player;
                bool exeTarget = playerControl.IsExeTarget();
                bool shielded = playerControl.IsShielded();
                bool gaTarget = playerControl.IsGATarget();

                //Roles
                foreach (var role in RoleHistory.Where(x => x.Key == playerControl.PlayerId))
                {
                    if (role.Value == RoleEnum.Crewmate) { playerRole += "<color=#" + Colors.Crewmate.ToHtmlStringRGBA() + ">Crewmate</color> > "; }
                    else if (role.Value == RoleEnum.Impostor) { playerRole += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Impostor</color> > "; }
                    else if (role.Value == RoleEnum.Engineer) { playerRole += "<color=#" + Colors.Engineer.ToHtmlStringRGBA() + ">Engineer</color> > "; }
                    else if (role.Value == RoleEnum.Investigator) { playerRole += "<color=#" + Colors.Investigator.ToHtmlStringRGBA() + ">Investigator</color> > "; }
                    else if (role.Value == RoleEnum.Mayor) { playerRole += "<color=#" + Colors.Mayor.ToHtmlStringRGBA() + ">Mayor</color> > "; }
                    else if (role.Value == RoleEnum.Medic) { playerRole += "<color=#" + Colors.Medic.ToHtmlStringRGBA() + ">Medic</color> > "; }
                    else if (role.Value == RoleEnum.Vigilante) { playerRole += "<color=#" + Colors.Vigilante.ToHtmlStringRGBA() + ">Vigilante</color> > "; }
                    else if (role.Value == RoleEnum.Seer) { playerRole += "<color=#" + Colors.Seer.ToHtmlStringRGBA() + ">Seer</color> > "; }
                    else if (role.Value == RoleEnum.Snitch) { playerRole += "<color=#" + Colors.Snitch.ToHtmlStringRGBA() + ">Snitch</color> > "; }
                    else if (role.Value == RoleEnum.Vigilante) { playerRole += "<color=#" + Colors.Vigilante.ToHtmlStringRGBA() + ">Vigilante</color> > "; }
                    else if (role.Value == RoleEnum.Arsonist) { playerRole += "<color=#" + Colors.Arsonist.ToHtmlStringRGBA() + ">Arsonist</color> > "; }
                    else if (role.Value == RoleEnum.Executioner) { playerRole += "<color=#" + Colors.Executioner.ToHtmlStringRGBA() + ">Executioner</color> > "; }
                    else if (role.Value == RoleEnum.Glitch) { playerRole += "<color=#" + Colors.Glitch.ToHtmlStringRGBA() + ">Glitch</color> > "; }
                    else if (role.Value == RoleEnum.Jester) { playerRole += "<color=#" + Colors.Jester.ToHtmlStringRGBA() + ">Jester</color> > "; }
                    else if (role.Value == RoleEnum.Phantom) { playerRole += "<color=#" + Colors.Phantom.ToHtmlStringRGBA() + ">Phantom</color> > "; }
                    else if (role.Value == RoleEnum.Grenadier) { playerRole += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Grenadier</color> > "; }
                    else if (role.Value == RoleEnum.Janitor) { playerRole += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Janitor</color> > "; }
                    else if (role.Value == RoleEnum.Miner) { playerRole += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Miner</color> > "; }
                    else if (role.Value == RoleEnum.Morphling) { playerRole += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Morphling</color> > "; }
                    else if (role.Value == RoleEnum.Swooper) { playerRole += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Swooper</color> > "; }
                    else if (role.Value == RoleEnum.Undertaker) { playerRole += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Undertaker</color> > "; }
                    else if (role.Value == RoleEnum.Haunter) { playerRole += "<color=#" + Colors.Haunter.ToHtmlStringRGBA() + ">Haunter</color> > "; }
                    else if (role.Value == RoleEnum.Veteran) { playerRole += "<color=#" + Colors.Veteran.ToHtmlStringRGBA() + ">Veteran</color> > "; }
                    else if (role.Value == RoleEnum.Amnesiac) { playerRole += "<color=#" + Colors.Amnesiac.ToHtmlStringRGBA() + ">Amnesiac</color> > "; }
                    else if (role.Value == RoleEnum.Vulture) { playerRole += "<color=#" + Colors.Vulture.ToHtmlStringRGBA() + ">Vulture</color> > "; }
                    else if (role.Value == RoleEnum.Juggernaut) { playerRole += "<color=#" + Colors.Juggernaut.ToHtmlStringRGBA() + ">Juggernaut</color> > "; }
                    else if (role.Value == RoleEnum.Tracker) { playerRole += "<color=#" + Colors.Tracker.ToHtmlStringRGBA() + ">Tracker</color> > "; }
                    else if (role.Value == RoleEnum.Transporter) { playerRole += "<color=#" + Colors.Transporter.ToHtmlStringRGBA() + ">Transporter</color> > "; }
                    else if (role.Value == RoleEnum.Medium) { playerRole += "<color=#" + Colors.Medium.ToHtmlStringRGBA() + ">Medium</color> > "; }
                    else if (role.Value == RoleEnum.Trapper) { playerRole += "<color=#" + Colors.Trapper.ToHtmlStringRGBA() + ">Trapper</color> > "; }
                    else if (role.Value == RoleEnum.Romantic) { playerRole += "<color=#" + Colors.Romantic.ToHtmlStringRGBA() + ">Romantic</color> > "; }
                    else if (role.Value == RoleEnum.GuardianAngel) { playerRole += "<color=#" + Colors.GuardianAngel.ToHtmlStringRGBA() + ">Guardian Angel</color> > "; }
                    else if (role.Value == RoleEnum.Mystic) { playerRole += "<color=#" + Colors.Mystic.ToHtmlStringRGBA() + ">Mystic</color> > "; }
                    else if (role.Value == RoleEnum.Blackmailer) { playerRole += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Blackmailer</color> > "; }
                    else if (role.Value == RoleEnum.Plaguebearer) { playerRole += "<color=#" + Colors.Plaguebearer.ToHtmlStringRGBA() + ">Plaguebearer</color> > "; }
                    else if (role.Value == RoleEnum.Pestilence) { playerRole += "<color=#" + Colors.Pestilence.ToHtmlStringRGBA() + ">Pestilence</color> > "; }
                    else if (role.Value == RoleEnum.Agent) { playerRole += "<color=#" + Colors.Agent.ToHtmlStringRGBA() + ">Agent</color> > "; }
                    else if (role.Value == RoleEnum.Hitman) { playerRole += "<color=#" + Colors.Hitman.ToHtmlStringRGBA() + ">Hitman</color> > "; }
                    else if (role.Value == RoleEnum.Werewolf) { playerRole += "<color=#" + Colors.Werewolf.ToHtmlStringRGBA() + ">Werewolf</color> > "; }
                    else if (role.Value == RoleEnum.Escapist) { playerRole += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Escapist</color> > "; }
                    else if (role.Value == RoleEnum.Chameleon) { playerRole += "<color=#" + Colors.Chameleon.ToHtmlStringRGBA() + ">Chameleon</color> > "; }
                    else if (role.Value == RoleEnum.Imitator) { playerRole += "<color=#" + Colors.Imitator.ToHtmlStringRGBA() + ">Imitator</color> > "; }
                    else if (role.Value == RoleEnum.Bomber) { playerRole += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Bomber</color> > "; }
                    else if (role.Value == RoleEnum.Doomsayer) { playerRole += "<color=#" + Colors.Doomsayer.ToHtmlStringRGBA() + ">Doomsayer</color> > "; }
                    else if (role.Value == RoleEnum.Witch) { playerRole += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Witch</color> > "; }
                    else if (role.Value == RoleEnum.Vampire) { playerRole += "<color=#" + Colors.Vampire.ToHtmlStringRGBA() + ">Vampire</color> > "; }
                    else if (role.Value == RoleEnum.Prosecutor) { playerRole += "<color=#" + Colors.Prosecutor.ToHtmlStringRGBA() + ">Prosecutor</color> > "; }
                    else if (role.Value == RoleEnum.Jailor) { playerRole += "<color=#" + Colors.Jailor.ToHtmlStringRGBA() + ">Jailor</color> > "; }
                    else if (role.Value == RoleEnum.Warlock) { playerRole += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Warlock</color> > "; }
                    else if (role.Value == RoleEnum.Oracle) { playerRole += "<color=#" + Colors.Oracle.ToHtmlStringRGBA() + ">Oracle</color> > "; }
                    else if (role.Value == RoleEnum.Venerer) { playerRole += "<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Venerer</color> > "; }
                    else if (role.Value == RoleEnum.SerialKiller) { playerRole += "<color=#" + Colors.SerialKiller.ToHtmlStringRGBA() + ">Serial Killer</color> > "; }
                }
                
                if (!string.IsNullOrEmpty(playerRole)) playerRole = playerRole.Remove(playerRole.Length - 3);
                
                //Modifiers
                if (playerControl.Is(ModifierEnum.Giant)) { playerRole += " (<color=#" + Colors.Giant.ToHtmlStringRGBA() + ">Giant</color>)"; }
                else if (playerControl.Is(ModifierEnum.Aftermath)) { playerRole += " (<color=#" + Colors.Aftermath.ToHtmlStringRGBA() + ">Aftermath</color>)"; }
                else if (playerControl.Is(ModifierEnum.Bait)) { playerRole += " (<color=#" + Colors.Bait.ToHtmlStringRGBA() + ">Bait</color>)";}
                else if (playerControl.Is(ModifierEnum.Diseased)) { playerRole += " (<color=#" + Colors.Diseased.ToHtmlStringRGBA() + ">Diseased</color>)";}
                else if (playerControl.Is(ModifierEnum.Disperser)) { playerRole += " (<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Disperser</color>)";}
                else if (playerControl.Is(ModifierEnum.Ghoul)) { playerRole += " (<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Ghoul</color>)";}
                else if (playerControl.Is(ModifierEnum.DoubleShot)) { playerRole += " (<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Double Shot</color>)";}
                else if (playerControl.Is(ModifierEnum.Underdog)) { playerRole += " (<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Underdog</color>)";}
                else if (playerControl.Is(ModifierEnum.Frosty)) { playerRole += " (<color=#" + Colors.Frosty.ToHtmlStringRGBA() + ">Frosty</color>)";}

                //Abilities
                if (playerControl.Is(AbilityEnum.Drunk)) { playerRole += " [<color=#" + Colors.Drunk.ToHtmlStringRGBA() + ">Drunk</color>]";}
                else if (playerControl.Is(AbilityEnum.Chameleon)) { playerRole += " [<color=#" + Colors.Chameleon.ToHtmlStringRGBA() + ">Chameleon</color>]";}
                else if (playerControl.Is(AbilityEnum.Assassin)) { playerRole += " [<color=#" + Colors.Impostor.ToHtmlStringRGBA() + ">Assassin</color>]";}
                else if (playerControl.Is(AbilityEnum.Flash)) { playerRole += " [<color=#" + Colors.Flash.ToHtmlStringRGBA() + ">Flash</color>]";}
                else if (playerControl.Is(AbilityEnum.Multitasker)) { playerRole += " (<color=#" + Colors.Multitasker.ToHtmlStringRGBA() + ">Multitasker</color>)";}
                else if (playerControl.Is(AbilityEnum.ButtonBarry)) { playerRole += " [<color=#" + Colors.ButtonBarry.ToHtmlStringRGBA() + ">Button Barry</color>]";}
                else if (playerControl.Is(AbilityEnum.Tiebreaker)) { playerRole += " [<color=#" + Colors.Tiebreaker.ToHtmlStringRGBA() + ">Tiebreaker</color>]"; }
                else if (playerControl.Is(AbilityEnum.Spy)) { playerRole += " [<color=#" + Colors.Spy.ToHtmlStringRGBA() + ">Spy</color>]";}
                else if (playerControl.Is(AbilityEnum.Torch)) { playerRole += " [<color=#" + Colors.Torch.ToHtmlStringRGBA() + ">Torch</color>]";}
                else if (playerControl.Is(AbilityEnum.Sleuth)) { playerRole += " [<color=#" + Colors.Sleuth.ToHtmlStringRGBA() + ">Sleuth</color>]";}
                else if (playerControl.Is(AbilityEnum.Radar)) { playerRole += " [<color=#" + Colors.Radar.ToHtmlStringRGBA() + ">Radar</color>]";}

                var player = GetPlayerRole(playerControl);
                //Stats
                if (playerControl.HasTasks())
                {
                    if ((player.TotalTasks - player.TasksLeft)/player.TotalTasks == 1) playerRole += $" | Tasks: <color=#" + Color.green.ToHtmlStringRGBA() + $">{player.TotalTasks - player.TasksLeft}/{player.TotalTasks}</color>";
                    else playerRole += $" | Tasks: {player.TotalTasks - player.TasksLeft}/{player.TotalTasks}";
                }
                if (player.Kills > 0 && !playerControl.Is(Faction.Crewmates))
                {
                    playerRole += " |<color=#" + Colors.Impostor.ToHtmlStringRGBA() + $"> Kills: {player.Kills}</color>";
                }
                if (player.CorrectShot > 0)
                {
                    playerRole += " |<color=#" + Color.green.ToHtmlStringRGBA() + $"> Correct Shots: {player.CorrectShot}</color>";
                }

                if (playerControl.Is(RoleEnum.Vulture))
                {
                    playerRole += ColorString(Colors.Vulture, $" ({GetRole<Vulture>(playerControl).BodiesRemainingToWin()} left)</color>");
                }
                if (player.IncorrectShots > 0)
                {
                    playerRole += " |<color=#" + Colors.Impostor.ToHtmlStringRGBA() + $"> Incorrect Shots: {player.IncorrectShots}</color>";
                }
                if (player.CorrectAssassinKills > 0)
                {
                    playerRole += " |<color=#" + Color.green.ToHtmlStringRGBA() + $"> Guesses: {player.CorrectAssassinKills}</color>";
                }
                if (player.IncorrectAssassinKills > 0)
                {
                    playerRole += " |<color=#" + Colors.Impostor.ToHtmlStringRGBA() + $"> Misguessed</color>";
                }
                if (player.CorrectVigilanteShot > 0)
                {
                    playerRole += ColorString(Colors.Vigilante, $" | Correct Shots: {player.CorrectVigilanteShot}</color>");
                }
                if (player.CorrectKills > 0)
                {
                    playerRole += ColorString(Color.green, $" | Correct Kills: {player.CorrectKills}</color>");
                }
                playerRole += " | " + playerControl.DeathReason();
                AdditionalTempData.playerRoles.Add(new AdditionalTempData.PlayerRoleInfo() { PlayerName = playerControl.Data.PlayerName,
                    ExeTarget = exeTarget, Shielded = shielded,
                    GATarget = gaTarget, Loved = loved, Role = playerRole });
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
            foreach (var role in GetRoles(RoleEnum.Phantom))
            {
                var phan = (Phantom)role;
                notWinners.Add(phan.Player);
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
            bool PhantomWin = false;
            foreach (var role in GetRoles(RoleEnum.Phantom))
            {
                var Phantom = (Phantom)role;
                PhantomWin = Phantom.Player != null && gameOverReason == (GameOverReason)CustomGameOverReason.PhantomWin;
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
                    AdditionalTempData.winCondition = WinCondition.DoomsayerWin;
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
                    AdditionalTempData.winCondition = WinCondition.AgentWin;
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
                    AdditionalTempData.winCondition = WinCondition.HitmanWin;
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
                    AdditionalTempData.winCondition = WinCondition.SerialKillerWin;
                }
            }

            // Crew/Imp Win
            else if (ImpWin)
            {
                AdditionalTempData.winCondition = WinCondition.ImpostorWin;  
            }

            else if (CrewWin)
            {
                AdditionalTempData.winCondition = WinCondition.CrewmateWin;  
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
                    AdditionalTempData.winCondition = WinCondition.ExecutionerWin;
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
                    AdditionalTempData.winCondition = WinCondition.JesterWin;
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
                    AdditionalTempData.winCondition = WinCondition.VultureWin;
                }
            }
            // Phantom Win
            else if (PhantomWin)
            {
                foreach (var role in GetRoles(RoleEnum.Phantom))
                {
                    var Phantom = (Phantom)role;
                    EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                    CachedPlayerData wpd = new CachedPlayerData(Phantom.Player.Data);
                    EndGameResult.CachedWinners.Add(wpd);
                    AdditionalTempData.winCondition = WinCondition.PhantomWin;
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
                    AdditionalTempData.winCondition = WinCondition.ArsonistWin;
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
                    AdditionalTempData.winCondition = WinCondition.JuggernautWin;
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
                    AdditionalTempData.winCondition = WinCondition.PestilenceWin;
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
                    AdditionalTempData.winCondition = WinCondition.PlaguebearerWin;
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
                    AdditionalTempData.winCondition = WinCondition.GlitchWin;
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
                    AdditionalTempData.winCondition = WinCondition.TeamVampiresWin;
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
                    AdditionalTempData.winCondition = WinCondition.WerewolfWin;
                }
            }

            // Possible Additional winner: Guardian Angel
            foreach (var role in GetRoles(RoleEnum.GuardianAngel))
            {
                CachedPlayerData winningClient = null;
                var ga = (GuardianAngel)role;
                if (ga.Player != null && !ga.Player.Data.Disconnected && ga.target != null)
                {
                    foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator()) {
                        if (winner.PlayerName == ga.target.Data.PlayerName)
                            winningClient = winner;
                    }
                    if (winningClient != null) { // The GA wins if the client is winning
                        if (!EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == ga.Player.Data.PlayerName))
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(ga.Player.Data));
                        AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalGuardianAngelWin); // The GA wins together with the client
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
                    foreach (CachedPlayerData winner in EndGameResult.CachedWinners.GetFastEnumerator()) {
                        if (winner.PlayerName == romantic.Beloved.Data.PlayerName)
                            winningClient = winner;
                    }
                    if (winningClient != null) { // The Romantic wins if the client is winning
                        if (!EndGameResult.CachedWinners.ToArray().Any(x => x.PlayerName == romantic.Player.Data.PlayerName))
                            EndGameResult.CachedWinners.Add(new CachedPlayerData(romantic.Player.Data));
                        AdditionalTempData.additionalWinConditions.Add(WinCondition.AdditionalRomanticWin); // The Romantic wins together with the client
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    public class EndGameManagerSetUpPatch {
        public static void Postfix(EndGameManager __instance) {
            // Delete and readd PoolablePlayers always showing the name and role of the player
            foreach (PoolablePlayer pb in __instance.transform.GetComponentsInChildren<PoolablePlayer>()) {
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

            for (int i = 0; i < list.Count; i++) {
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
                if (CachedPlayerData2.IsDead) {
                    poolablePlayer.SetBodyAsGhost();
                    poolablePlayer.SetDeadFlipX(i % 2 == 0);
                } else {
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
            if (AdditionalTempData.winCondition == WinCondition.JesterWin)
            {
                textRenderer.text = "Jester Wins!";
                textRenderer.color = Colors.Jester;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Jester);
            }
            if (AdditionalTempData.winCondition == WinCondition.ExecutionerWin)
            {
                textRenderer.text = "Executioner Wins!";
                textRenderer.color = Colors.Executioner;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Executioner);
            }
            if (AdditionalTempData.winCondition == WinCondition.DoomsayerWin)
            {
                textRenderer.text = "Doomsayer Wins!";
                textRenderer.color = Colors.Doomsayer;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Doomsayer);
            }
            if (AdditionalTempData.winCondition == WinCondition.VultureWin)
            {
                textRenderer.text = "Vulture Wins!";
                textRenderer.color = Colors.Vulture;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Vulture);
            }
            if (AdditionalTempData.winCondition == WinCondition.AgentWin)
            {
                textRenderer.text = "Agent Wins!";
                textRenderer.color = Colors.Agent;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Agent);
            }
            if (AdditionalTempData.winCondition == WinCondition.HitmanWin)
            {
                textRenderer.text = "Hitman Wins!";
                textRenderer.color = Colors.Hitman;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Hitman);
            }
            if (AdditionalTempData.winCondition == WinCondition.SerialKillerWin)
            {
                textRenderer.text = "Serial Killer Wins!";
                textRenderer.color = Colors.SerialKiller;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.SerialKiller);
            }
            if (AdditionalTempData.winCondition == WinCondition.PhantomWin)
            {
                textRenderer.text = "Phantom Wins!";
                textRenderer.color = Colors.Phantom;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Phantom);
            }
            if (AdditionalTempData.winCondition == WinCondition.CrewmateWin)
            {
                textRenderer.text = "Crewmates Win!";
                textRenderer.color = Colors.Crewmate;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Crewmate);
            }
            if (AdditionalTempData.winCondition == WinCondition.ImpostorWin)
            {
                textRenderer.text = "Impostors Win!";
                textRenderer.color = Colors.Impostor;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Impostor);
            }
            if (AdditionalTempData.winCondition == WinCondition.TeamVampiresWin)
            {
                textRenderer.text = "Vampires Win!";
                textRenderer.color = Colors.Vampire;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Vampire);
            }
            if (AdditionalTempData.winCondition == WinCondition.GlitchWin)
            {
                textRenderer.text = "Glitch Wins!";
                textRenderer.color = Colors.Glitch;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Glitch);
            }
            if (AdditionalTempData.winCondition == WinCondition.JuggernautWin)
            {
                textRenderer.text = "Juggernaut Wins!";
                textRenderer.color = Colors.Juggernaut;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Juggernaut);
            }
            if (AdditionalTempData.winCondition == WinCondition.ArsonistWin)
            {
                textRenderer.text = "Arsonist Wins!";
                textRenderer.color = Colors.Arsonist;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Arsonist);
            }
            if (AdditionalTempData.winCondition == WinCondition.PlaguebearerWin)
            {
                textRenderer.text = "Plaguebearer Wins!";
                textRenderer.color = Colors.Plaguebearer;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Plaguebearer);
            }
            if (AdditionalTempData.winCondition == WinCondition.PestilenceWin)
            {
                textRenderer.text = "Pestilence Wins!";
                textRenderer.color = Colors.Pestilence;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Pestilence);
            }
            if (AdditionalTempData.winCondition == WinCondition.WerewolfWin)
            {
                textRenderer.text = "Werewolf Wins!";
                textRenderer.color = Colors.Werewolf;
                __instance.BackgroundBar.material.SetColor("_Color", Colors.Werewolf);
            }

            foreach (WinCondition cond in AdditionalTempData.additionalWinConditions) {
                if (cond == WinCondition.AdditionalRomanticWin) {
                    textRenderer.text += $"\n{ColorString(Colors.Romantic, "The Romantic wins with the beloved!")}";
                } else if (cond == WinCondition.AdditionalGuardianAngelWin) {
                    textRenderer.text += $"\n{ColorString(Colors.GuardianAngel, "The Guardian Angel wins with the target!")}";
                }
            }

            var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
            GameObject roleSummary = Object.Instantiate(__instance.WinText.gameObject);
            roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -214f); 
            roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

            var roleSummaryText = new StringBuilder();
            var winnersText = new StringBuilder();
            var losersText = new StringBuilder();
            var winners = EndGameResult.CachedWinners;
            var winnerCount = 0;
            var loserCount = 0;

            roleSummaryText.AppendLine("<size=80%><u><b>End Game Summary</b></u>: </size>");
            roleSummaryText.AppendLine(" ");
            winnersText.AppendLine("<color=#00FF04><size=60%><b>★ Winners ★</b></size> </color>-");
            losersText.AppendLine("<color=#FF0000><size=60%><b>Losers</b></size> </color>-");
            
            foreach(var data in AdditionalTempData.playerRoles) 
            {
                var role = string.Join(" ", data.Role);
                string exeTarget = data.ExeTarget ? ColorString(Colors.Executioner, " [⦿]") : "";
                string gaTarget = data.GATarget ?  ColorString(Colors.GuardianAngel, " [★]") : "";
                string beloved = data.Loved ? ColorString(Colors.Romantic, " [♥]") : "";
                string shielded = data.Shielded ? ColorString(Colors.Medic, " [<b>+</b>]") : "";

                var roleInfos = $">{data.PlayerName}{exeTarget}{gaTarget}{beloved}{shielded} - {role}";
                if (data.PlayerName.IsWinner())
                {
                    winnersText.AppendLine($"<size=40%>{roleInfos}</size>");
                    winnerCount += 1;
                }
                else
                {
                    losersText.AppendLine($"<size=40%>{roleInfos}</size>");
                    loserCount += 1;
                }
            }

            if (winnerCount == 0)
                winnersText.AppendLine("<size=60%>No One Won</size>");

            if (loserCount == 0)
                winnersText.AppendLine("<size=60%>No One Lost</size>");

            //I resized it all because it was way too big for the End Game Screen and it felt like it was a lot

            winnersText.AppendLine(" ");   

            TMPro.TMP_Text roleSummaryTextMesh = roleSummary.GetComponent<TMPro.TMP_Text>();
            roleSummaryTextMesh.alignment = TMPro.TextAlignmentOptions.TopLeft;
            roleSummaryTextMesh.color = Color.white;
            roleSummaryTextMesh.fontSizeMin = 1.5f;
            roleSummaryTextMesh.fontSizeMax = 1.5f;
            roleSummaryTextMesh.fontSize = 1.5f;
                
            var roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
            roleSummaryTextMeshRectTransform.anchoredPosition = new Vector2(position.x + 3.5f, position.y - 0.1f);
            roleSummaryTextMesh.text = roleSummaryText.ToString() + winnersText.ToString() + losersText.ToString();
            AdditionalTempData.clear();
        }
    }

    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))] 
    class CheckEndCriteriaPatch {
        public static bool Prefix(ShipStatus __instance) {
            if (!GameData.Instance) return false;
            if (DestroyableSingleton<TutorialManager>.InstanceExists) // InstanceExists | Don't check Custom Criteria when in Tutorial
                return true;
            var statistics = new PlayerStatistics(__instance);
            if (CheckAndEndGameForJesterWin(__instance)) return false;
            if (CheckAndEndGameForExecutionerWin(__instance)) return false;
            if (CheckAndEndGameForDoomsayerWin(__instance)) return false;
            if (CheckAndEndGameForVultureWin(__instance)) return false;
            if (CheckAndEndGameForPhantomWin(__instance)) return false;
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
        private static bool CheckAndEndGameForPhantomWin(ShipStatus __instance) {
            foreach (var role in GetRoles(RoleEnum.Phantom))
            {
                var phantom = (Phantom)role;
                if (phantom.CompletedTasks && !phantom.Caught)
                {
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.PhantomWin, false);
                    return true;
                }
            }
            return false;
        }
        private static bool CheckAndEndGameForSabotageWin(ShipStatus __instance) {
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
        private static bool CheckAndEndGameForTaskWin(ShipStatus __instance) {
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
                        foreach (var role in GetRoles(RoleEnum.Mayor))
                        {
                            var Mayor = (Mayor)role;
                            if (Mayor.Player != null && Mayor.Player.PlayerId == playerInfo.PlayerId) {
                                CrewKillingAlive++;
                            }
                        }
                        foreach (var role in GetRoles(RoleEnum.Prosecutor))
                        {
                            var Prosecutor = (Prosecutor)role;
                            if (Prosecutor.Player != null && Prosecutor.Player.PlayerId == playerInfo.PlayerId && !Prosecutor.HasProsecuted) {
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