using System.Linq;
using System;
using System.Collections.Generic;
using static TownOfSushi.TownOfSushi;
using UnityEngine;
using TownOfSushi.Utilities;

namespace TownOfSushi
{
    public class RoleInfo 
    {
        public Color Color;
        public string Name;
        public string IntroDescription;
        public string ShortDescription;
        public string RoleDescription;
        public RoleId RoleId;
        public Factions FactionId;
        /*public bool isImpostor => Color == Palette.ImpostorRed && !(RoleId == RoleId.Spy);
        public static Dictionary<RoleId, RoleInfo> RoleInfoById = new();*/
        public RoleInfo(string name, Color Color, string IntroDescription, string ShortDescription, RoleId RoleId, Factions FactionId, string RoleDescription)
        {
            this.Color = Color;
            this.Name = name;
            this.IntroDescription = IntroDescription;
            this.ShortDescription = ShortDescription;
            this.RoleId = RoleId;
            this.FactionId = FactionId;
            this.RoleDescription = RoleDescription;
            //RoleInfoById.TryAdd(RoleId, this);
        }

        #region Neutrals
        public readonly static RoleInfo jester = new("Jester", Jester.Color, "Get voted out", "Get voted out", RoleId.Jester, Factions.Neutral, "As a Jester, your job is to get voted out by any means to win, if you don't get voted out, you lose.");
        public readonly static RoleInfo thief = new("Thief", Thief.Color, "Steal a killer's role by killing them", "Steal a killer's role", RoleId.Thief, Factions.Neutral, "As the Thief you have to kill an evil person to win. You may be able to kill the Sheriff. When you kill someone, you gain their role and their win condition.");
        public readonly static RoleInfo arsonist = new("Arsonist", Arsonist.Color, "Let them burn", "Let them burn", RoleId.Arsonist, Factions.Neutral, "The Arsonist needs to spread gasoline on every single player to win.");
        public readonly static RoleInfo vulture = new("Vulture", Vulture.Color, "Eat corpses to win", "Eat dead bodies", RoleId.Vulture, Factions.Neutral, $"The goal of the Vulture is to eat {Vulture.vultureNumberToWin - Vulture.eatenBodies} dead bodies to win.");
        public readonly static RoleInfo lawyer = new("Lawyer", Lawyer.Color, "Defend your client", "Defend your client", RoleId.Lawyer, Factions.Neutral, "The Lawyer's duty is to prevent their client from getting ejected, if they get voted out, you suicide, if they are killed, you become a Pursuer.");
        public readonly static RoleInfo amnesiac = new("Amnesiac", Amnesiac.Color, "Gain an identity from a corpse", "Find a dead body to remember a role", RoleId.Amnesiac, Factions.Neutral, "The Amnesiac has to find a dead body in order to gain a role as they are roleless at the begining. Once you find a body you can remember who you were and you will get the corpse's role. You may have arrows pointing to bodies depending on settings.");
        public readonly static RoleInfo prosecutor = new("Prosecutor", Lawyer.Color, "Vote out your target", "Vote out your target", RoleId.Prosecutor, Factions.Neutral, "The Prosecutor is the opposite of a Lawyer, they are also given a target, but instead of protecting them, you have to make them look guilty. If they are voted out you win. If they die you become a Pursuer.");
        public readonly static RoleInfo pursuer = new("Pursuer", Pursuer.Color, "Blank the Impostors", "Blank the Impostors", RoleId.Pursuer, Factions.Neutral, "As the Pursuer you were either a Lawyer, Romantic or Prosecutor which target die. All you have to do is stay alive to win, you may also blank players to prevent them from killing.");
        public readonly static RoleInfo romantic = new("Romantic", Romantic.Color, "Create a lover to win with you", "Create, protect and assist your lover", RoleId.Romantic, Factions.Neutral, "As the Romantic, you must pick a player to love, working together to ensure both of your survival. If your target dies, you will become a Pursuer.");

        #endregion

        #region Impostors
        public readonly static RoleInfo godfather = new("Godfather", Godfather.Color, "Kill all Crewmates", "Kill all Crewmates", RoleId.Godfather, Factions.Impostor, "As the Godfather, you are a plain impostor that has other 2 helpers, the Janitor and the Mafioso.");
        public readonly static RoleInfo mafioso = new("Mafioso", Mafioso.Color, "Work with the <color=#FF1919FF>Mafia</color> to kill the Crewmates", "Kill all Crewmates", RoleId.Mafioso, Factions.Impostor, "The Mafioso is an Impostor who cannot kill until the Godfather is dead.");
        public readonly static RoleInfo janitor = new("Janitor", Janitor.Color, "Work with the <color=#FF1919FF>Mafia</color> by hiding dead bodies", "Hide dead bodies", RoleId.Janitor, Factions.Impostor, "The Janitor is an Impostor who cannot kill, but they can hide dead bodies instead.");
        public readonly static RoleInfo undertaker = new("Undertaker", Ninja.Color, "Drag dead bodies and hide them around the map", "Drag dead bodies", RoleId.Undertaker, Factions.Impostor, "The Undertaker is an Impostor who can drag dead bodies around the map. The Undertaker may vent during the drag, depending on settings.");
        public readonly static RoleInfo morphling = new("Morphling", Morphling.Color, "Change your look to not get caught", "Change your look", RoleId.Morphling, Factions.Impostor, $"The Morphling can morph into the form of their fellow Crewmates, morphing changes the Morphling's look to make them not look sus. The morphling can only morph into a crewmate once every {Morphling.Cooldown} seconds and lasts for {Morphling.Duration} seconds.");
        public readonly static RoleInfo camouflager = new("Camouflager", Camouflager.Color, "Camouflage and kill the Crewmates", "Hide among others", RoleId.Camouflager, Factions.Impostor, $"The Camouflager can turn everyone gray making everyone unkown and nobody knows who is who for {Camouflager.Duration}s every {Camouflager.Cooldown}s.");
        public readonly static RoleInfo vampire = new("Vampire", Vampire.Color, "Kill the Crewmates with your bites", "Bite your enemies", RoleId.Vampire, Factions.Impostor,  $"The Vampire can bite a player every {Vampire.Cooldown} seconds, after {Vampire.delay} seconds the player die. Players with protection can't be killed by the Vampire. If the Vampire is alive in the last 4, they will directly kill instead of bitting.");
        public readonly static RoleInfo eraser = new("Eraser", Eraser.Color, "Kill the Crewmates and erase their roles", "Erase the roles of your enemies", RoleId.Eraser, Factions.Impostor, "The Eraser can delete player's role for the rest of the game, making them become regular crewmate. They may be able to erase Neutral killers depending on settings.");
        public readonly static RoleInfo trickster = new("Trickster", Trickster.Color, "Use your jack-in-the-boxes to surprise others", "Surprise your enemies", RoleId.Trickster, Factions.Impostor, "The trickster can place boxes around the map which works like a vent, only the Trickster may use them. They can also manually sabotage lights, at any time, with any sabotage on but lights.");
        public readonly static RoleInfo cleaner = new("Cleaner", Cleaner.Color, "Kill everyone and leave no traces", "Clean up dead bodies", RoleId.Cleaner, Factions.Impostor, "The Cleaner is an Impostor that can clean up bodies. Both their Kill and Clean ability have a shared Cooldown, meaning they have to choose which one they want to use.");
        public readonly static RoleInfo warlock = new("Warlock", Warlock.Color, "Curse other players and kill everyone", "Curse and kill everyone", RoleId.Warlock, Factions.Impostor, "The Warlock is an Impostor, that can curse another player (the cursed player doesn't get notified). If the cursed person stands next to another player, the Warlock is able to kill that player (no matter how far away they are).");
        public readonly static RoleInfo bountyHunter = new("Bounty Hunter", BountyHunter.Color, "Hunt your bounty down", "Hunt your bounty down", RoleId.BountyHunter, Factions.Impostor,  "As the Bounty Hunter, you are given a target, which your task is to eliminate them, killing your target gives you a short Cooldown, else will give you a long penalty Cooldown.");
        public readonly static RoleInfo impostor = new("Impostor", Palette.ImpostorRed, Helpers.ColorString(Palette.ImpostorRed, "Sabotage and kill everyone"), "Sabotage and kill everyone", RoleId.Impostor, Factions.Impostor,  "Just a regular Impostor");
        public readonly static RoleInfo witch = new("Witch", Witch.Color, "Cast a spell upon your foes", "Cast a spell upon your foes", RoleId.Witch, Factions.Impostor, "The Witch is an Impostor who has the ability to cast a spell on other players. During the next meeting, the spellbound player will be highlighted and they'll die right after the meeting. There are multiple options listed down below with which you can configure to fit your taste. Similar to the Vampire, shields and blanks will be checked twice (at the end of casting the spell on the player and at the end of the meeting, when the spell will be activated).");
        public readonly static RoleInfo ninja = new("Ninja", Ninja.Color, "Surprise and assassinate your foes", "Surprise and assassinate your foes", RoleId.Ninja, Factions.Impostor, "The Ninja is an Impostor who has the ability to kill another player all over the map. You can mark a player with your ability and by using the ability again, you jump to the position of the marked player and kill it.");
        public readonly static RoleInfo yoyo = new("Yo-Yo", Yoyo.Color, "Blink to a marked location and Back", "Blink to a location", RoleId.Yoyo, Factions.Impostor, "The Yo-Yo is an Impostor who has the ability mark a position and later blink (teleport) to this position. After the initial blink, the Yo-Yo has a fixed amount of time (option) to do whatever they want, before automatically blinking back to the starting point of the first blink. Each blink leaves behind a silhouette with configurable transparency. The silhouette is very hard to see.The Yo-Yo may also have access to a mobile admin table, depending on the settings.");

        #endregion

        #region Crewmates
        public readonly static RoleInfo crewmate = new("Crewmate", Color.white, "Find the Impostors", "Find the Impostors", RoleId.Crewmate, Factions.Crewmate, "Just a regular Crewmate.");
        public readonly static RoleInfo lighter = new("Lighter", Lighter.Color, "Your light never goes out", "Your light never goes out", RoleId.Lighter, Factions.Crewmate, "As the lighter, you have the ability to temporarly enhance your vision.");
        public readonly static RoleInfo detective = new("Detective", Detective.Color, "Find the <color=#FF1919FF>Impostors</color> by examining footprints", "Examine footprints", RoleId.Detective, Factions.Crewmate,  "The Detective can see footprints that other players leave behind. The Detective's other feature shows when they report a corpse: they receive clues about the killer's identity. The type of information they get is based on the time it took them to find the corpse.");
        public readonly static RoleInfo hacker = new("Hacker", Hacker.Color, "Hack systems to find the <color=#FF1919FF>Impostors</color>", "Hack to find the Impostors", RoleId.Hacker, Factions.Crewmate, "If the Hacker activates the Hacker mode, the Hacker gets more information than others from the admin table and vitals for a set Duration. Otherwise they see the same information as everyone else. The Hacker can see the colors (or Color types) of the players on the table. They can also see how long dead players have been dead for. The Hacker can access his mobile gadgets (vitals & admin table), with a maximum of charges (uses) and a configurable amount of tasks needed to recharge.");
        public readonly static RoleInfo tracker = new("Tracker", Tracker.Color, "Track the <color=#FF1919FF>Impostors</color> down", "Track the Impostors down", RoleId.Tracker, Factions.Crewmate, $"The Tracker is able to track the movements of other players. The Arrow's Color will be the tracked players Color. The arrow will update the position of the player every {Tracker.updateIntervall} seconds. The Arrows will reset depending on settings after each meeting. They can track dead bodies depending on settings as well.");
        public readonly static RoleInfo snitch = new("Snitch", Snitch.Color, "Finish your tasks to find the <color=#FF1919FF>Impostors</color>", "Finish your tasks", RoleId.Snitch, Factions.Crewmate, "The Snitch has to finish tasks to be able to see the killers or evil players in game.");
        public readonly static RoleInfo crusader = new("Crusader", Crusader.Color, "Fortify a Crewmate to Eliminate the <color=#FF1919FF>Impostors</color>", "Fortify a Crewmate", RoleId.Crusader, Factions.Crewmate, "The Crusader can fortify a player in order to protect them from being touched. If somebody tries to kill the fortified player the killer will die. If a non killing role interacts with them, nothing will happen. The Crusader can Fortify one player per round.");
        public readonly static RoleInfo spy = new("Spy", Spy.Color, "Confuse the <color=#FF1919FF>Impostors</color>", "Confuse the Impostors", RoleId.Spy, Factions.Crewmate, "The Spy appears as another Impostor when there's more than 2 Impostors, they may vent or be able to die by the Sheriff, your job is to confuse the impostors into killing themselves.");
        public readonly static RoleInfo vigilante = new("Vigilante", Vigilante.Color, "Seal vents and place cameras", "Seal vents and place cameras", RoleId.Vigilante, Factions.Crewmate, "The Vigilante is a Crewmate that has a certain number of screws that they can use for either sealing vents or for placing new cameras. bPlacing a new camera and sealing vents takes a configurable amount of screws. The total number of screws that a Vigilante has can also be configured. The new camera will be visible after the next meeting and accessible by everyone. The vents will be sealed after the next meeting, players can't enter or exit sealed vents, but they can still move to them underground.");
        public readonly static RoleInfo mayor = new("Mayor", Mayor.Color, "Your vote counts twice", "Your vote counts twice", RoleId.Mayor, Factions.Crewmate, "The Mayor leads the Crewmates by having a vote that counts twice. The Mayor can always use their meeting, even if the maximum number of meetings was reached. The Mayor has a portable Meeting Button, depending on the options. The Mayor can see the vote colors after completing a configurable amount of tasks, depending on the options. The Mayor has the option to vote with only one vote instead of two (via a button in the meeting screen), depending on the settings.");
        public readonly static RoleInfo portalmaker = new("Portalmaker", Portalmaker.Color, "You can create portals", "You can create portals", RoleId.Portalmaker, Factions.Crewmate, "The Portalmaker is a Crewmate that can place two portals on the map. These two portals are connected to each other. Those portals will be visible after the next meeting and can be used by everyone. Additionally to that, the Portalmaker gets information about who used the portals and when in the chat during each meeting, depending on the options. The Portalmaker can teleport themself to their placed portals from anywhere if the setting is enabled.");
        public readonly static RoleInfo veteran = new("Veteran", Veteran.Color, "Alert to murder whoever touches you", "Alert to kill the <color=#FF1919FF>Impostors</color>", RoleId.Veteran, Factions.Crewmate, $"The Veteran is able to alert, Alerting makes the Veteran Unkillable and will kill anyone who interacts with them. At the start of the game the Veteran can alert a maximum of " + Veteran.Charges + " times.");
        public readonly static RoleInfo engineer = new("Engineer",  Engineer.Color, "Maintain important systems on the ship", "Repair the ship", RoleId.Engineer, Factions.Crewmate, $"The Engineer is able to vent around the map and fix sabotages. The Engineer can fix a maximum of " + Engineer.remainingFixes + " sabotages.");
        public readonly static RoleInfo sheriff = new("Sheriff", Sheriff.Color, "Shoot the <color=#FF1919FF>Impostors</color>", "Shoot the Impostors", RoleId.Sheriff, Factions.Crewmate, "The Sheriff is able to kill players during rounds, if the player they kill is an impostor, or Neutral Killer, the Sheriff will survive. If the player they kill is a crewmate, the Sheriff will die.");
        public readonly static RoleInfo medium = new("Medium", Medium.Color, "Question the souls of the dead to gain information", "Question the souls", RoleId.Medium, Factions.Crewmate, "The medium is a crewmate who can ask the souls of dead players for information. Like the Mystic, the medium will see the souls of the players who have died (after the next meeting) and can question them. They then gets random information about the soul or the killer in the chat. The souls only stay for one round, i.e. until the next meeting. Depending on the options, the souls can only be questioned once and then disappear.");
        public readonly static RoleInfo trapper = new("Trapper", Trapper.Color, "Place traps to find the Impostors", "Place traps", RoleId.Trapper, Factions.Crewmate, "The Tracker can select one player to track. Depending on the options the Tracker can track a different person after each meeting or the Tracker tracks the same person for the whole game. An arrow points to the last tracked position of the player. The arrow updates its position every few seconds (configurable). By an option, the arrow can be replaced or combined with the Proximity Tracker from Hide N Seek. Depending on the options, the Tracker has another ability: They can track all corpses on the map for a set amount of time. They will keep tracking corpses, even if they were cleaned or eaten by the Vulture.");
        public readonly static RoleInfo timeMaster = new("Time Master", TimeMaster.Color, "Save yourself with your time shield", "Use your time shield", RoleId.TimeMaster, Factions.Crewmate, "The Time Master has a time shield which they can activate. The time shield remains active for a configurable amount of time. If a player tries to kill the Time Master while the time shield is active, the kill won't happen and the time will rewind for a set amount of time. The kill Cooldown of the killer won't be reset, so the Time Master has to make sure that the game won't result in the same situation. The Time Master won't be affected by the rewind.");
        public readonly static RoleInfo medic = new("Medic", Medic.Color, "Protect someone with your shield", "Protect other players", RoleId.Medic, Factions.Crewmate, "The Medic can shield (highlighted by an outline around the player) one player per game, which makes the player unkillable. The shield is also shown in the meeting as brackets around the shielded player's name. The shielded player can still be voted out and might also be an Impostor. If set in the options, the shielded player and/or the Medic will get a red flash on their screen if someone (Impostor, Sheriff, ...) tried to murder them. If the Medic dies, the shield disappears with them. The Sheriff will not die if they try to kill a shielded Crewmate and won't perform a kill if they try to kill a shielded Impostor. Depending on the options, guesses from the Guesser will be blocked by the shield and the shielded player/medic might be notified. The Medic's other feature shows when they report a corpse: they will see how long ago the player died.");
        public readonly static RoleInfo swapper = new("Swapper", Swapper.Color, "Swap votes to exile the <color=#FF1919FF>Impostors</color>", "Swap votes", RoleId.Swapper, Factions.Crewmate, "During meetings the Swapper can exchange votes that two people get (i.e. all votes that player A got will be given to player B and vice versa). Because of the Swapper's strength in meetings, they might not start emergency meetings and can't fix lights and comms. The Swapper now has initial swap charges and can recharge those charges after completing a configurable amount of tasks.");
        public readonly static RoleInfo oracle = new("Oracle", Oracle.Color, "Make the <color=#FF1919FF>Impostors</color> confess their sins", "Get another player to confess on your passing", RoleId.Oracle, Factions.Crewmate, $"The Oracle can compel another player to confess their secrets upon death. The oracle will get information about 3 players being possibly evil each meeting. The Oracle can only make a player confess once per meeting. When the Oracle dies, the player they made confess will reveal their faction with a probability of {Oracle.Accuracy}% of being right.");
        public readonly static RoleInfo mystic = new("Mystic", Mystic.Color, "You will see players die", "You will see players die", RoleId.Mystic, Factions.Crewmate, "The Mystic gets a list of the possible roles that the examined player can be in meetings. The Mystic has more abilities (one can activate one of them or both in the options). The Mystic sees the souls of players that died a round earlier, the souls slowly fade away. The Mystic gets a blue flash on their screen, if a player dies somewhere on the map.");

        #endregion

        #region Neutral Killers
        public readonly static RoleInfo jackal = new("Jackal", Jackal.Color, "Kill all Crewmates and <color=#FF1919FF>Impostors</color> to win", "Kill everyone", RoleId.Jackal, Factions.NeutralKiller, "The Jackal is part of an extra team, that tries to eliminate all the other players. The Jackal has no tasks and can kill Impostors, Crewmates and Neutrals. The Jackal (if allowed by the options) can select another player to be their Sidekick. Creating a Sidekick removes all tasks of the Sidekick and adds them to the team Jackal. The Sidekick loses their current role (except if they're a Lover, then they play in two teams). The Create Sidekick Action may only be used once per Jackal or once per game (depending on the options). The Jackal can also promote Impostors to be their Sidekick, but depending on the options the Impostor will either really turn into the Sidekick and leave the team Impostors or they will just look like the Sidekick to the Jackal and remain as they were. Also if a Spy or Impostor gets sidekicked, they still will appear red to the Impostors.");
        public readonly static RoleInfo plaguebearer = new("Plaguebearer", Plaguebearer.Color, "Infect all players to become Pestilence", "Infect to become Pestilence", RoleId.Pestilence, Factions.NeutralKiller, "The Plaguebearer is a Neutral role with its own win condition, as well as an ability to transform into another role. The Plaguebearer has one ability, which allows them to infect other players. Once all players are infected, the Plaguebearer becomes Pestilence.");
        public readonly static RoleInfo pestilence = new("Pestilence", Pestilence.Color, "", "Kill with your unstoppable abilities", RoleId.Pestilence, Factions.NeutralKiller, "The Pestilence is a unkillable force which can only be killed by being voted out or them guessing wrong. The Pestilence needs to be the last killer alive to win the game.");
        public readonly static RoleInfo juggernaut = new("Juggernaut", Juggernaut.Color, "Kill all your <color=#FF1919FF>Enemies</color> to win", "Each kill makes you more dangerous", RoleId.Juggernaut, Factions.NeutralKiller, "The Juggernaut is a Neutral role with its own win condition. The Juggernaut's special ability is that their kill Cooldown reduces with each kill. This means in theory the Juggernaut can have a 0 second kill Cooldown!. The Juggernaut needs to be the last killer alive to win the game.");
        public readonly static RoleInfo sidekick = new("Sidekick", Sidekick.Color, "Help your Jackal to kill everyone", "Help your Jackal to kill everyone", RoleId.Sidekick, Factions.NeutralKiller, "Gets assigned to a player during the game by the Create Sidekick Action of the Jackal and joins the Jackal in their quest to eliminate all other players. Upon the death of the Jackal (depending on the options), they might get promoted to Jackal themself and potentially even assign a Sidekick of their own.");
        public readonly static RoleInfo agent = new("Agent", Agent.Color, "Finish your duties to start the dirty work", "Finish your tasks", RoleId.Agent, Factions.NeutralKiller, "The Agent is a Neutral killer role with its own win condition. They need to finish tasks in order to gain new abilities. Depending on settings they may be able to vent so they finish tasks faster.");
        public readonly static RoleInfo hitman = new("Hitman", Hitman.Color, "Kill your enemies to win", "Kill your enemies", RoleId.Hitman, Factions.NeutralKiller, "The Hitman is a Neutral role with its own win condition. The Hitman's aim is to kill win alone. The Hitman is able to kill players, morph into them like a Morphling or a Glitch for a set amount of time. They can also drag dead bodies just like an Undertaker. They may be able to vent depending on settings.");
        public readonly static RoleInfo serialKiller = new("Serial Killer", SerialKiller.Color, "Stab to make everyone die", "Murder everyone when stabbing", RoleId.SerialKiller, Factions.NeutralKiller, "The Serial Killer is a Neutral role with its own win condition. The Serial Killer has an invisible kill button, but they can't use it unless they are stabbing. Once the Serial Killer rampages they gain Impostor vision and the ability to kill. However, unlike most killers their kill Cooldown is really short. The Serial Killer needs to be the last killer alive to win the game.");
        public readonly static RoleInfo glitch = new("Glitch", Glitch.Color, "Hack, Kill and Mimic your <color=#FF1919FF>enemies</color>", "Hack, Kill and Mimic your <color=#FF1919FF>enemies</color>", RoleId.Glitch, Factions.NeutralKiller, "Glitch is a Neutral role with its own win condition. Glitch's aim is to kill everyone and be the last person standing. Glitch can Hack players, resulting in them being unable to report bodies and do tasks. Hacking prevents the hacked player from doing anything but walk around the map. Glitch can Mimic someone, which results in them looking exactly like the other person.");
        public readonly static RoleInfo vromantic = new("Vengeful Romantic", Romantic.Color, "", "Avenge your lover", RoleId.VengefulRomantic, Factions.NeutralKiller, "As the Vengeful Romantic you were once a Romantic with a lover, but they died somehow. Now you are mad for revenge and will murder everyone in order to avenge your lover, if you win your dead lover also does.");
        public readonly static RoleInfo werewolf = new("Werewolf", Werewolf.Color, "Maul and eliminate your <color=#FF1919FF>enemies</color>", "Maul to eliminate your <color=#FF1919FF>enemies</color>", RoleId.Werewolf, Factions.NeutralKiller, "The Werewolf can kill all players within a certain radius.");

        #endregion

        #region Modifiers
        public readonly static RoleInfo bloody = new("Bloody", Color.yellow, "Your killer leaves a bloody trail", "Your killer leaves a bloody trail", RoleId.Bloody, Factions.Modifier, "");
        public readonly static RoleInfo antiTeleport = new("Anti Teleport", Color.yellow, "You will not get teleported", "You will not get teleported", RoleId.AntiTeleport, Factions.Modifier, "");
        public readonly static RoleInfo tiebreaker = new("Tiebreaker", Color.yellow, "Your vote breaks the tie", "Break the tie", RoleId.Tiebreaker, Factions.Modifier, "");
        public readonly static RoleInfo bait = new("Bait", Color.yellow, "Bait your enemies", "Bait your enemies", RoleId.Bait, Factions.Modifier, "");
        public readonly static RoleInfo sunglasses = new("Sunglasses", Color.yellow, "You got the sunglasses", "Your vision is reduced", RoleId.Sunglasses, Factions.Modifier, "");
        public readonly static RoleInfo sleuth = new("Sleuth", Color.yellow, "Learn from your reports", "Get to know the role of who you report", RoleId.Sleuth, Factions.Modifier, "");
        public readonly static RoleInfo lover = new("Lover", Lovers.Color, $"You are in love", "Stay alive until the end with your lover", RoleId.Lover, Factions.Modifier, "");
        public readonly static RoleInfo mini = new("Mini", Color.yellow, "No one will harm you until you grow up", "No one will harm you", RoleId.Mini, Factions.Modifier, "");
        public readonly static RoleInfo vip = new("VIP", Color.yellow, "You are the VIP", "Everyone is notified when you die", RoleId.Vip, Factions.Modifier, "");
        public readonly static RoleInfo invert = new("Invert", Color.yellow, "Your movement is inverted", "Your movement is inverted", RoleId.Invert, Factions.Modifier, "");
        public readonly static RoleInfo chameleon = new("Chameleon", Color.yellow, "You're hard to see when not moving", "You're hard to see when not moving", RoleId.Chameleon, Factions.Modifier, "");
        public readonly static RoleInfo armored = new("Armored", Color.yellow, "You are protected from one murder attempt", "You are protected from one murder attempt", RoleId.Armored, Factions.Modifier, "");
        public readonly static RoleInfo disperser = new("Disperser", Color.yellow, "Disperse the Crew to random vents", "Disperse players to random vents", RoleId.Disperser, Factions.Modifier, "");
        public readonly static RoleInfo shifter = new("Shifter", Color.yellow, "Shift your role", "Shift your role", RoleId.Shifter, Factions.Modifier, "");

        #endregion

        public static List<RoleInfo> allRoleInfos = new List<RoleInfo>() 
        {
            impostor,
            godfather,
            mafioso,
            janitor,
            morphling,
            camouflager,
            vampire,
            eraser,
            trickster,
            cleaner,
            romantic,
            warlock,
            bountyHunter,
            crusader,
            witch,
            ninja,
            werewolf,
            pestilence,
            plaguebearer,
            disperser,
            yoyo,
            lover,
            jester,
            arsonist,
            hitman,
            agent,
            serialKiller,
            oracle,
            jackal,
            sidekick,
            vulture,
            amnesiac,
            pursuer,
            sleuth,
            lawyer,
            juggernaut,
            thief,
            prosecutor,
            undertaker,
            crewmate,
            vromantic,
            mayor,
            portalmaker,
            engineer,
            sheriff,
            veteran,
            glitch,
            lighter,
            detective,
            timeMaster,
            medic,
            swapper,
            mystic,
            hacker,
            tracker,
            snitch,
            spy,
            vigilante,
            bait,
            medium,
            trapper,
            bloody,
            antiTeleport,
            tiebreaker,
            sunglasses,
            mini,
            vip,
            invert,
            chameleon,
            armored,
            shifter
        };

        public static List<RoleInfo> GetRoleInfoForPlayer(PlayerControl p, bool showModifier = true) 
        {
            List<RoleInfo> infos = new List<RoleInfo>();
            if (p == null) return infos;

            // Modifier
            if (showModifier) 
            {
                // after dead modifier
                if (!CustomOptionHolder.modifiersAreHidden.GetBool() || PlayerControl.LocalPlayer.Data.IsDead || AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Ended)
                {
                    if (Bait.Players.Any(x => x.PlayerId == p.PlayerId)) infos.Add(bait);
                    if (Bloody.Players.Any(x => x.PlayerId == p.PlayerId)) infos.Add(bloody);
                    if (Vip.Players.Any(x => x.PlayerId == p.PlayerId)) infos.Add(vip);
                }
                if (p == Lovers.Lover1 || p == Lovers.Lover2) infos.Add(lover);
                if (p == Tiebreaker.Player) infos.Add(tiebreaker);
                if (AntiTeleport.Players.Any(x => x.PlayerId == p.PlayerId)) infos.Add(antiTeleport);
                if (Sleuth.Players.Any(x => x.PlayerId == p.PlayerId)) infos.Add(sleuth);
                if (Sunglasses.Players.Any(x => x.PlayerId == p.PlayerId)) infos.Add(sunglasses);
                if (p == Disperser.Player) infos.Add(disperser);
                if (p == Mini.Player) infos.Add(mini);
                if (Invert.Players.Any(x => x.PlayerId == p.PlayerId)) infos.Add(invert);
                if (Chameleon.Players.Any(x => x.PlayerId == p.PlayerId)) infos.Add(chameleon);
                if (p == Armored.Player) infos.Add(armored);
                if (p == Shifter.Player) infos.Add(shifter);
            }

            int count = infos.Count;  // Save count after modifiers are added so that the role count can be checked

            // Special roles
            if (p == Jester.Player) infos.Add(jester);
            if (p == Pestilence.Player) infos.Add(pestilence);
            if (p == Plaguebearer.Player) infos.Add(plaguebearer);
            if (p == Mayor.Player) infos.Add(mayor);
            if (p == Portalmaker.Player) infos.Add(portalmaker);
            if (p == Engineer.Player) infos.Add(engineer);
            if (p == Sheriff.Player) infos.Add(sheriff);
            if (p == Romantic.Player) infos.Add(romantic);
            if (p == Juggernaut.Player) infos.Add(juggernaut);
            if (p == Crusader.Player) infos.Add(crusader);
            if (p == Undertaker.Player) infos.Add(undertaker);
            if (p == VengefulRomantic.Player) infos.Add(vromantic);
            if (p == Glitch.Player) infos.Add(glitch);
            if (p == SerialKiller.Player) infos.Add(serialKiller);
            if (p == Oracle.Player) infos.Add(oracle);
            if (p == Werewolf.Player) infos.Add(werewolf);
            if (p == Veteran.Player) infos.Add(veteran);
            if (p == Lighter.Player) infos.Add(lighter);
            if (p == Godfather.Player) infos.Add(godfather);
            if (p == Mafioso.Player) infos.Add(mafioso);
            if (p == Janitor.Player) infos.Add(janitor);
            if (p == Morphling.Player) infos.Add(morphling);
            if (p == Camouflager.Player) infos.Add(camouflager);
            if (p == Vampire.Player) infos.Add(vampire);
            if (p == Eraser.Player) infos.Add(eraser);
            if (p == Trickster.Player) infos.Add(trickster);
            if (p == Cleaner.Player) infos.Add(cleaner);
            if (p == Warlock.Player) infos.Add(warlock);
            if (p == Witch.Player) infos.Add(witch);
            if (p == Ninja.ninja) infos.Add(ninja);
            if (p == Yoyo.Player) infos.Add(yoyo);
            if (p == Amnesiac.Player) infos.Add(amnesiac);
            if (p == Detective.Player) infos.Add(detective);
            if (p == TimeMaster.Player) infos.Add(timeMaster);
            if (p == Medic.Player) infos.Add(medic);
            if (p == Hitman.Player) infos.Add(hitman);
            if (p == Agent.Player) infos.Add(agent);
            if (p == Swapper.Player) infos.Add(swapper);
            if (p == Mystic.Player) infos.Add(mystic);
            if (p == Hacker.Player) infos.Add(hacker);
            if (p == Tracker.Player) infos.Add(tracker);
            if (p == Snitch.Player) infos.Add(snitch);
            if (p == Jackal.Player || (Jackal.formerJackals != null && Jackal.formerJackals.Any(x => x.PlayerId == p.PlayerId))) infos.Add(jackal);
            if (p == Sidekick.Player) infos.Add(sidekick);
            if (p == Spy.Player) infos.Add(spy);
            if (p == Vigilante.Player) infos.Add(vigilante);
            if (p == Arsonist.Player) infos.Add(arsonist);
            if (p == BountyHunter.Player) infos.Add(bountyHunter);
            if (p == Vulture.Player) infos.Add(vulture);
            if (p == Medium.medium) infos.Add(medium);
            if (p == Lawyer.Player && !Lawyer.isProsecutor) infos.Add(lawyer);
            if (p == Lawyer.Player && Lawyer.isProsecutor) infos.Add(prosecutor);
            if (p == Trapper.Player) infos.Add(trapper);
            if (p == Pursuer.Player) infos.Add(pursuer);
            if (p == Thief.Player) infos.Add(thief);

            // Default roles (just impostor, just crewmate, or hunter / hunted for hide n seek, prop hunt prop ...
            if (infos.Count == count) 
            {
                if (p.Data.Role.IsImpostor)
                    infos.Add(impostor);
                else
                    infos.Add(crewmate);
            }

            return infos;
        }

        public static String GetRolesString(PlayerControl p, bool useColors, bool showModifier = true, bool suppressGhostInfo = false) 
        {
            string roleName;
            roleName = String.Join(" ", GetRoleInfoForPlayer(p, showModifier).Select(x => useColors ? Helpers.ColorString(x.Color, x.Name) : x.Name).ToArray());
            if (Lawyer.target != null && p.PlayerId == Lawyer.target.PlayerId && PlayerControl.LocalPlayer != Lawyer.target) 
                roleName += (useColors ? Helpers.ColorString(Pursuer.Color, " §") : " §");
            if (Romantic.beloved != null && p.PlayerId == Romantic.beloved.PlayerId && PlayerControl.LocalPlayer != Romantic.beloved) 
                roleName += useColors ? Helpers.ColorString(Romantic.Color, " ♥") : " ♥";
            if (VengefulRomantic.Lover != null && p.PlayerId == VengefulRomantic.Lover.PlayerId && PlayerControl.LocalPlayer != VengefulRomantic.Lover) 
                roleName += useColors ? Helpers.ColorString(Romantic.Color, " ♥") : " ♥";

            if (HandleGuesser.IsGuesser(p.PlayerId)) 
            {
                int remainingShots = HandleGuesser.RemainingShots(p.PlayerId);
                var (playerCompleted, playerTotal) = TasksHandler.TaskInfo(p.Data);
                if (p.IsCrew() && playerCompleted < HandleGuesser.tasksToUnlock || remainingShots == 0)
                    roleName += Helpers.ColorString(Color.gray, " (Guesser)");
                else
                    roleName += Helpers.ColorString(Color.white," (Guesser)");
            }

            if (!suppressGhostInfo && p != null) 
            {
                if (p == Shifter.Player && (PlayerControl.LocalPlayer == Shifter.Player || Helpers.ShouldShowGhostInfo()) && Shifter.futureShift != null)
                    roleName += Helpers.ColorString(Color.yellow, " ← " + Shifter.futureShift.Data.PlayerName);
                if (p == Vulture.Player && (PlayerControl.LocalPlayer == Vulture.Player || Helpers.ShouldShowGhostInfo()))
                    roleName = roleName + Helpers.ColorString(Vulture.Color, $" ({Vulture.vultureNumberToWin - Vulture.eatenBodies} left)");
                if (Helpers.ShouldShowGhostInfo()) 
                {
                    if (Eraser.futureErased.Contains(p))
                        roleName = Helpers.ColorString(Color.gray, "(Erased) ") + roleName;
                    if (Vampire.Player != null && !Vampire.Player.Data.IsDead && Vampire.bitten == p && !p.Data.IsDead)
                        roleName = Helpers.ColorString(Vampire.Color, $"(Bitten {(int)HudManagerStartPatch.vampireKillButton.Timer + 1}) ") + roleName;
                    if (Glitch.HackedPlayers.Contains(p.PlayerId))
                        roleName = Helpers.ColorString(Color.gray, "(Hacked) ") + roleName;
                    if (Glitch.HackedKnows.ContainsKey(p.PlayerId))  // Active cuff
                        roleName = Helpers.ColorString(Glitch.Color, "(Hacked) ") + roleName;
                    if (p == Warlock.curseVictim)
                        roleName = Helpers.ColorString(Warlock.Color, "(Cursed) ") + roleName;
                    if (p == Ninja.ninjaMarked)
                        roleName = Helpers.ColorString(Ninja.Color, "(Marked) ") + roleName;
                    if (p == Crusader.FortifiedPlayer)
                        roleName = Helpers.ColorString(Crusader.Color, "(Fortified) ") + roleName;
                    if (Pursuer.blankedList.Contains(p) && !p.Data.IsDead)
                        roleName = Helpers.ColorString(Pursuer.Color, "(Blanked) ") + roleName;
                    if (Witch.futureSpelled.Contains(p) && !MeetingHud.Instance) // This is already displayed in meetings!
                        roleName = Helpers.ColorString(Witch.Color, "☆ ") + roleName;
                    if (BountyHunter.bounty == p)
                        roleName = Helpers.ColorString(BountyHunter.Color, "(Bounty) ") + roleName;
                    if (Plaguebearer.InfectedPlayers.Contains(p.PlayerId))
                        roleName = Helpers.ColorString(Plaguebearer.Color, "⦿ ") + roleName;
                    if (Arsonist.dousedPlayers.Contains(p))
                        roleName = Helpers.ColorString(Arsonist.Color, "♨ ") + roleName;
                    if (p == Arsonist.Player)
                        roleName = roleName + Helpers.ColorString(Arsonist.Color, $" ({PlayerControl.AllPlayerControls.ToArray().Count(x => { return x != Arsonist.Player && !x.Data.IsDead && !x.Data.Disconnected && !Arsonist.dousedPlayers.Any(y => y.PlayerId == x.PlayerId); })} left)");
                    if (p == Jackal.fakeSidekick)
                        roleName = Helpers.ColorString(Sidekick.Color, $"(Fake Sidekick) ") + roleName;

                    // Death Reason on Ghosts
                    if (p.Data.IsDead) {
                        string deathReasonString = "";
                        var deadPlayer = GameHistory.deadPlayers.FirstOrDefault(x => x.player.PlayerId == p.PlayerId);

                        Color killerColor = new();
                        if (deadPlayer != null && deadPlayer.killerIfExisting != null) 
                        {
                            killerColor = GetRoleInfoForPlayer(deadPlayer.killerIfExisting, false).FirstOrDefault().Color;
                        }

                        if (deadPlayer != null) 
                        {
                            switch (deadPlayer.deathReason) 
                            {
                                case DeadPlayer.CustomDeathReason.Disconnect:
                                    deathReasonString = " - Disconnected";
                                    break;
                                case DeadPlayer.CustomDeathReason.Exile:
                                    deathReasonString = " - Voted out";
                                    break;
                                case DeadPlayer.CustomDeathReason.Kill:
                                    deathReasonString = $" - Killed by {Helpers.ColorString(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Guess:
                                    if (deadPlayer.killerIfExisting.Data.PlayerName == p.Data.PlayerName)
                                        deathReasonString = $" - Failed Guessing";
                                    else
                                        deathReasonString = $" - Guessed by {Helpers.ColorString(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Shift:
                                    deathReasonString = $" - {Helpers.ColorString(Color.yellow, "Shifted")} {Helpers.ColorString(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.WitchExile:
                                    deathReasonString = $" - {Helpers.ColorString(Witch.Color, "Witched")} by {Helpers.ColorString(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Maul:
                                    deathReasonString = $" - {Helpers.ColorString(Werewolf.Color, "Mauled")} by {Helpers.ColorString(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.LoverSuicide:
                                    deathReasonString = $" - {Helpers.ColorString(Lovers.Color, "Lover Suicide")}";
                                    break;
                                case DeadPlayer.CustomDeathReason.LawyerSuicide:
                                    deathReasonString = $" - {Helpers.ColorString(Lawyer.Color, "Client Voted Out")}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Arson:
                                    deathReasonString = $" - Burnt by {Helpers.ColorString(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                            }
                            roleName = roleName + deathReasonString;
                        }
                    }
                }
            }
            return roleName;
        }
    }
}
