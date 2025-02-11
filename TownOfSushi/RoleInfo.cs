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
        public Color color;
        public string name;
        public string introDescription;
        public string shortDescription;
        public RoleId roleId;
        public Factions FactionId;

        public RoleInfo(string name, Color color, string introDescription, string shortDescription, RoleId roleId, Factions FactionId) 
        {
            this.color = color;
            this.name = name;
            this.introDescription = introDescription;
            this.shortDescription = shortDescription;
            this.roleId = roleId;
            this.FactionId = FactionId;
        }

        #region Neutrals
        public readonly static RoleInfo jester = new("Jester", Jester.color, "Get voted out", "Get voted out", RoleId.Jester, Factions.Neutral);
        public readonly static RoleInfo thief = new("Thief", Thief.color, "Steal a killers role by killing them", "Steal a killers role", RoleId.Thief, Factions.Neutral);
        public readonly static RoleInfo arsonist = new("Arsonist", Arsonist.color, "Let them burn", "Let them burn", RoleId.Arsonist, Factions.Neutral);
        public readonly static RoleInfo vulture = new("Vulture", Vulture.color, "Eat corpses to win", "Eat dead bodies", RoleId.Vulture, Factions.Neutral);
        public readonly static RoleInfo lawyer = new("Lawyer", Lawyer.color, "Defend your client", "Defend your client", RoleId.Lawyer, Factions.Neutral);
        public readonly static RoleInfo prosecutor = new("Prosecutor", Lawyer.color, "Vote out your target", "Vote out your target", RoleId.Prosecutor, Factions.Neutral);
        public readonly static RoleInfo pursuer = new("Pursuer", Pursuer.color, "Blank the Impostors", "Blank the Impostors", RoleId.Pursuer, Factions.Neutral);

        #endregion

        #region Impostors
        public readonly static RoleInfo godfather = new("Godfather", Godfather.color, "Kill all Crewmates", "Kill all Crewmates", RoleId.Godfather, Factions.Impostor);
        public readonly static RoleInfo mafioso = new("Mafioso", Mafioso.color, "Work with the <color=#FF1919FF>Mafia</color> to kill the Crewmates", "Kill all Crewmates", RoleId.Mafioso, Factions.Impostor);
        public readonly static RoleInfo janitor = new("Janitor", Janitor.color, "Work with the <color=#FF1919FF>Mafia</color> by hiding dead bodies", "Hide dead bodies", RoleId.Janitor, Factions.Impostor);
        public readonly static RoleInfo morphling = new("Morphling", Morphling.color, "Change your look to not get caught", "Change your look", RoleId.Morphling, Factions.Impostor);
        public readonly static RoleInfo camouflager = new("Camouflager", Camouflager.color, "Camouflage and kill the Crewmates", "Hide among others", RoleId.Camouflager, Factions.Impostor);
        public readonly static RoleInfo vampire = new("Vampire", Vampire.color, "Kill the Crewmates with your bites", "Bite your enemies", RoleId.Vampire, Factions.Impostor);
        public readonly static RoleInfo eraser = new("Eraser", Eraser.color, "Kill the Crewmates and erase their roles", "Erase the roles of your enemies", RoleId.Eraser, Factions.Impostor);
        public readonly static RoleInfo trickster = new("Trickster", Trickster.color, "Use your jack-in-the-boxes to surprise others", "Surprise your enemies", RoleId.Trickster, Factions.Impostor);
        public readonly static RoleInfo cleaner = new("Cleaner", Cleaner.color, "Kill everyone and leave no traces", "Clean up dead bodies", RoleId.Cleaner, Factions.Impostor);
        public readonly static RoleInfo warlock = new("Warlock", Warlock.color, "Curse other players and kill everyone", "Curse and kill everyone", RoleId.Warlock, Factions.Impostor);
        public readonly static RoleInfo bountyHunter = new("Bounty Hunter", BountyHunter.color, "Hunt your bounty down", "Hunt your bounty down", RoleId.BountyHunter, Factions.Impostor);
        public readonly static RoleInfo impostor = new("Impostor", Palette.ImpostorRed, Helpers.ColorString(Palette.ImpostorRed, "Sabotage and kill everyone"), "Sabotage and kill everyone", RoleId.Impostor, Factions.Impostor);
        public readonly static RoleInfo witch = new("Witch", Witch.color, "Cast a spell upon your foes", "Cast a spell upon your foes", RoleId.Witch, Factions.Impostor);
        public readonly static RoleInfo ninja = new("Ninja", Ninja.color, "Surprise and assassinate your foes", "Surprise and assassinate your foes", RoleId.Ninja, Factions.Impostor);
        public readonly static RoleInfo bomber = new("Bomber", Bomber.color, "Bomb all Crewmates", "Bomb all Crewmates", RoleId.Bomber, Factions.Impostor);
        public readonly static RoleInfo yoyo = new("Yo-Yo", Yoyo.color, "Blink to a marked location and Back", "Blink to a location", RoleId.Yoyo, Factions.Impostor);

        #endregion

        #region Crewmates
        public readonly static RoleInfo crewmate = new("Crewmate", Color.white, "Find the Impostors", "Find the Impostors", RoleId.Crewmate, Factions.Crewmate);
        public readonly static RoleInfo lighter = new("Lighter", Lighter.color, "Your light never goes out", "Your light never goes out", RoleId.Lighter, Factions.Crewmate);
        public readonly static RoleInfo detective = new("Detective", Detective.color, "Find the <color=#FF1919FF>Impostors</color> by examining footprints", "Examine footprints", RoleId.Detective, Factions.Crewmate);
        public readonly static RoleInfo hacker = new("Hacker", Hacker.color, "Hack systems to find the <color=#FF1919FF>Impostors</color>", "Hack to find the Impostors", RoleId.Hacker, Factions.Crewmate);
        public readonly static RoleInfo tracker = new("Tracker", Tracker.color, "Track the <color=#FF1919FF>Impostors</color> down", "Track the Impostors down", RoleId.Tracker, Factions.Crewmate);
        public readonly static RoleInfo snitch = new("Snitch", Snitch.color, "Finish your tasks to find the <color=#FF1919FF>Impostors</color>", "Finish your tasks", RoleId.Snitch, Factions.Crewmate);
        public readonly static RoleInfo spy = new("Spy", Spy.color, "Confuse the <color=#FF1919FF>Impostors</color>", "Confuse the Impostors", RoleId.Spy, Factions.Crewmate);
        public readonly static RoleInfo securityGuard = new("Security Guard", SecurityGuard.color, "Seal vents and place cameras", "Seal vents and place cameras", RoleId.SecurityGuard, Factions.Crewmate);
        public readonly static RoleInfo mayor = new("Mayor", Mayor.color, "Your vote counts twice", "Your vote counts twice", RoleId.Mayor, Factions.Crewmate);
        public readonly static RoleInfo portalmaker = new("Portalmaker", Portalmaker.color, "You can create portals", "You can create portals", RoleId.Portalmaker, Factions.Crewmate);
        public readonly static RoleInfo engineer = new("Engineer",  Engineer.color, "Maintain important systems on the ship", "Repair the ship", RoleId.Engineer, Factions.Crewmate);
        public readonly static RoleInfo sheriff = new("Sheriff", Sheriff.color, "Shoot the <color=#FF1919FF>Impostors</color>", "Shoot the Impostors", RoleId.Sheriff, Factions.Crewmate);
        public readonly static RoleInfo medium = new("Medium", Medium.color, "Question the souls of the dead to gain information", "Question the souls", RoleId.Medium, Factions.Crewmate);
        public readonly static RoleInfo trapper = new("Trapper", Trapper.color, "Place traps to find the Impostors", "Place traps", RoleId.Trapper, Factions.Crewmate);
        public readonly static RoleInfo timeMaster = new("Time Master", TimeMaster.color, "Save yourself with your time shield", "Use your time shield", RoleId.TimeMaster, Factions.Crewmate);
        public readonly static RoleInfo medic = new("Medic", Medic.color, "Protect someone with your shield", "Protect other players", RoleId.Medic, Factions.Crewmate);
        public readonly static RoleInfo swapper = new("Swapper", Swapper.color, "Swap votes to exile the <color=#FF1919FF>Impostors</color>", "Swap votes", RoleId.Swapper, Factions.Crewmate);
        public readonly static RoleInfo seer = new("Seer", Seer.color, "You will see players die", "You will see players die", RoleId.Seer, Factions.Crewmate);

        #endregion

        #region Neutral Killers
        public readonly static RoleInfo jackal = new("Jackal", Jackal.color, "Kill all Crewmates and <color=#FF1919FF>Impostors</color> to win", "Kill everyone", RoleId.Jackal, Factions.NeutralKiller);
        public readonly static RoleInfo sidekick = new("Sidekick", Sidekick.color, "Help your Jackal to kill everyone", "Help your Jackal to kill everyone", RoleId.Sidekick, Factions.NeutralKiller);
        public readonly static RoleInfo glitch = new("Glitch", Glitch.color, "Hack, Kill and Mimic your <color=#FF1919FF>enemies</color>", "Hack, Kill and Mimic your <color=#FF1919FF>enemies</color>", RoleId.Glitch, Factions.NeutralKiller);

        #endregion

        #region Modifiers
        public readonly static RoleInfo bloody = new("Bloody", Color.yellow, "Your killer leaves a bloody trail", "Your killer leaves a bloody trail", RoleId.Bloody, Factions.Modifier);
        public readonly static RoleInfo antiTeleport = new("Anti tp", Color.yellow, "You will not get teleported", "You will not get teleported", RoleId.AntiTeleport, Factions.Modifier);
        public readonly static RoleInfo tiebreaker = new("Tiebreaker", Color.yellow, "Your vote breaks the tie", "Break the tie", RoleId.Tiebreaker, Factions.Modifier);
        public readonly static RoleInfo bait = new("Bait", Color.yellow, "Bait your enemies", "Bait your enemies", RoleId.Bait, Factions.Modifier);
        public readonly static RoleInfo sunglasses = new("Sunglasses", Color.yellow, "You got the sunglasses", "Your vision is reduced", RoleId.Sunglasses, Factions.Modifier);
        public readonly static RoleInfo lover = new("Lover", Lovers.color, $"You are in love", $"You are in love", RoleId.Lover, Factions.Modifier);
        public readonly static RoleInfo mini = new("Mini", Color.yellow, "No one will harm you until you grow up", "No one will harm you", RoleId.Mini, Factions.Modifier);
        public readonly static RoleInfo vip = new("VIP", Color.yellow, "You are the VIP", "Everyone is notified when you die", RoleId.Vip, Factions.Modifier);
        public readonly static RoleInfo invert = new("Invert", Color.yellow, "Your movement is inverted", "Your movement is inverted", RoleId.Invert, Factions.Modifier);
        public readonly static RoleInfo chameleon = new("Chameleon", Color.yellow, "You're hard to see when not moving", "You're hard to see when not moving", RoleId.Chameleon, Factions.Modifier);
        public readonly static RoleInfo armored = new("Armored", Color.yellow, "You are protected from one murder attempt", "You are protected from one murder attempt", RoleId.Armored, Factions.Modifier);
        public readonly static RoleInfo shifter = new("Shifter", Color.yellow, "Shift your role", "Shift your role", RoleId.Shifter, Factions.Modifier);

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
            warlock,
            bountyHunter,
            witch,
            ninja,
            bomber,
            yoyo,
            lover,
            jester,
            arsonist,
            jackal,
            sidekick,
            vulture,
            pursuer,
            lawyer,
            thief,
            prosecutor,
            crewmate,
            mayor,
            portalmaker,
            engineer,
            sheriff,
            glitch,
            lighter,
            detective,
            timeMaster,
            medic,
            swapper,
            seer,
            hacker,
            tracker,
            snitch,
            spy,
            securityGuard,
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
            if (showModifier) {
                // after dead modifier
                if (!CustomOptionHolder.modifiersAreHidden.GetBool() || PlayerControl.LocalPlayer.Data.IsDead || AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Ended)
                {
                    if (Bait.bait.Any(x => x.PlayerId == p.PlayerId)) infos.Add(bait);
                    if (Bloody.bloody.Any(x => x.PlayerId == p.PlayerId)) infos.Add(bloody);
                    if (Vip.vip.Any(x => x.PlayerId == p.PlayerId)) infos.Add(vip);
                }
                if (p == Lovers.lover1 || p == Lovers.lover2) infos.Add(lover);
                if (p == Tiebreaker.tiebreaker) infos.Add(tiebreaker);
                if (AntiTeleport.antiTeleport.Any(x => x.PlayerId == p.PlayerId)) infos.Add(antiTeleport);
                if (Sunglasses.sunglasses.Any(x => x.PlayerId == p.PlayerId)) infos.Add(sunglasses);
                if (p == Mini.mini) infos.Add(mini);
                if (Invert.invert.Any(x => x.PlayerId == p.PlayerId)) infos.Add(invert);
                if (Chameleon.chameleon.Any(x => x.PlayerId == p.PlayerId)) infos.Add(chameleon);
                if (p == Armored.armored) infos.Add(armored);
                if (p == Shifter.shifter) infos.Add(shifter);
            }

            int count = infos.Count;  // Save count after modifiers are added so that the role count can be checked

            // Special roles
            if (p == Jester.jester) infos.Add(jester);
            if (p == Mayor.mayor) infos.Add(mayor);
            if (p == Portalmaker.portalmaker) infos.Add(portalmaker);
            if (p == Engineer.engineer) infos.Add(engineer);
            if (p == Sheriff.sheriff) infos.Add(sheriff);
            if (p == Glitch.Player) infos.Add(glitch);
            if (p == Lighter.lighter) infos.Add(lighter);
            if (p == Godfather.godfather) infos.Add(godfather);
            if (p == Mafioso.mafioso) infos.Add(mafioso);
            if (p == Janitor.janitor) infos.Add(janitor);
            if (p == Morphling.morphling) infos.Add(morphling);
            if (p == Camouflager.camouflager) infos.Add(camouflager);
            if (p == Vampire.vampire) infos.Add(vampire);
            if (p == Eraser.eraser) infos.Add(eraser);
            if (p == Trickster.trickster) infos.Add(trickster);
            if (p == Cleaner.cleaner) infos.Add(cleaner);
            if (p == Warlock.warlock) infos.Add(warlock);
            if (p == Witch.witch) infos.Add(witch);
            if (p == Ninja.ninja) infos.Add(ninja);
            if (p == Bomber.bomber) infos.Add(bomber);
            if (p == Yoyo.yoyo) infos.Add(yoyo);
            if (p == Detective.detective) infos.Add(detective);
            if (p == TimeMaster.timeMaster) infos.Add(timeMaster);
            if (p == Medic.medic) infos.Add(medic);
            if (p == Swapper.swapper) infos.Add(swapper);
            if (p == Seer.seer) infos.Add(seer);
            if (p == Hacker.hacker) infos.Add(hacker);
            if (p == Tracker.tracker) infos.Add(tracker);
            if (p == Snitch.snitch) infos.Add(snitch);
            if (p == Jackal.jackal || (Jackal.formerJackals != null && Jackal.formerJackals.Any(x => x.PlayerId == p.PlayerId))) infos.Add(jackal);
            if (p == Sidekick.sidekick) infos.Add(sidekick);
            if (p == Spy.spy) infos.Add(spy);
            if (p == SecurityGuard.securityGuard) infos.Add(securityGuard);
            if (p == Arsonist.arsonist) infos.Add(arsonist);
            if (p == BountyHunter.bountyHunter) infos.Add(bountyHunter);
            if (p == Vulture.vulture) infos.Add(vulture);
            if (p == Medium.medium) infos.Add(medium);
            if (p == Lawyer.lawyer && !Lawyer.isProsecutor) infos.Add(lawyer);
            if (p == Lawyer.lawyer && Lawyer.isProsecutor) infos.Add(prosecutor);
            if (p == Trapper.trapper) infos.Add(trapper);
            if (p == Pursuer.pursuer) infos.Add(pursuer);
            if (p == Thief.thief) infos.Add(thief);

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
            roleName = String.Join(" ", GetRoleInfoForPlayer(p, showModifier).Select(x => useColors ? Helpers.ColorString(x.color, x.name) : x.name).ToArray());
            if (Lawyer.target != null && p.PlayerId == Lawyer.target.PlayerId && PlayerControl.LocalPlayer != Lawyer.target) 
                roleName += (useColors ? Helpers.ColorString(Pursuer.color, " §") : " §");
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
                if (p == Shifter.shifter && (PlayerControl.LocalPlayer == Shifter.shifter || Helpers.shouldShowGhostInfo()) && Shifter.futureShift != null)
                    roleName += Helpers.ColorString(Color.yellow, " ← " + Shifter.futureShift.Data.PlayerName);
                if (p == Vulture.vulture && (PlayerControl.LocalPlayer == Vulture.vulture || Helpers.shouldShowGhostInfo()))
                    roleName = roleName + Helpers.ColorString(Vulture.color, $" ({Vulture.vultureNumberToWin - Vulture.eatenBodies} left)");
                if (Helpers.shouldShowGhostInfo()) 
                {
                    if (Eraser.futureErased.Contains(p))
                        roleName = Helpers.ColorString(Color.gray, "(erased) ") + roleName;
                    if (Vampire.vampire != null && !Vampire.vampire.Data.IsDead && Vampire.bitten == p && !p.Data.IsDead)
                        roleName = Helpers.ColorString(Vampire.color, $"(bitten {(int)HudManagerStartPatch.vampireKillButton.Timer + 1}) ") + roleName;
                    if (Glitch.HackedPlayers.Contains(p.PlayerId))
                        roleName = Helpers.ColorString(Color.gray, "(hacked) ") + roleName;
                    if (Glitch.HackedKnows.ContainsKey(p.PlayerId))  // Active cuff
                        roleName = Helpers.ColorString(Glitch.color, "(hacked) ") + roleName;
                    if (p == Warlock.curseVictim)
                        roleName = Helpers.ColorString(Warlock.color, "(cursed) ") + roleName;
                    if (p == Ninja.ninjaMarked)
                        roleName = Helpers.ColorString(Ninja.color, "(marked) ") + roleName;
                    if (Pursuer.blankedList.Contains(p) && !p.Data.IsDead)
                        roleName = Helpers.ColorString(Pursuer.color, "(blanked) ") + roleName;
                    if (Witch.futureSpelled.Contains(p) && !MeetingHud.Instance) // This is already displayed in meetings!
                        roleName = Helpers.ColorString(Witch.color, "☆ ") + roleName;
                    if (BountyHunter.bounty == p)
                        roleName = Helpers.ColorString(BountyHunter.color, "(bounty) ") + roleName;
                    if (Arsonist.dousedPlayers.Contains(p))
                        roleName = Helpers.ColorString(Arsonist.color, "♨ ") + roleName;
                    if (p == Arsonist.arsonist)
                        roleName = roleName + Helpers.ColorString(Arsonist.color, $" ({PlayerControl.AllPlayerControls.ToArray().Count(x => { return x != Arsonist.arsonist && !x.Data.IsDead && !x.Data.Disconnected && !Arsonist.dousedPlayers.Any(y => y.PlayerId == x.PlayerId); })} left)");
                    if (p == Jackal.fakeSidekick)
                        roleName = Helpers.ColorString(Sidekick.color, $" (fake SK)") + roleName;

                    // Death Reason on Ghosts
                    if (p.Data.IsDead) {
                        string deathReasonString = "";
                        var deadPlayer = GameHistory.deadPlayers.FirstOrDefault(x => x.player.PlayerId == p.PlayerId);

                        Color killerColor = new();
                        if (deadPlayer != null && deadPlayer.killerIfExisting != null) {
                            killerColor = GetRoleInfoForPlayer(deadPlayer.killerIfExisting, false).FirstOrDefault().color;
                        }

                        if (deadPlayer != null) 
                        {
                            switch (deadPlayer.deathReason) 
                            {
                                case DeadPlayer.CustomDeathReason.Disconnect:
                                    deathReasonString = " - disconnected";
                                    break;
                                case DeadPlayer.CustomDeathReason.Exile:
                                    deathReasonString = " - voted out";
                                    break;
                                case DeadPlayer.CustomDeathReason.Kill:
                                    deathReasonString = $" - killed by {Helpers.ColorString(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Guess:
                                    if (deadPlayer.killerIfExisting.Data.PlayerName == p.Data.PlayerName)
                                        deathReasonString = $" - failed guess";
                                    else
                                        deathReasonString = $" - guessed by {Helpers.ColorString(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Shift:
                                    deathReasonString = $" - {Helpers.ColorString(Color.yellow, "shifted")} {Helpers.ColorString(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.WitchExile:
                                    deathReasonString = $" - {Helpers.ColorString(Witch.color, "witched")} by {Helpers.ColorString(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.LoverSuicide:
                                    deathReasonString = $" - {Helpers.ColorString(Lovers.color, "lover died")}";
                                    break;
                                case DeadPlayer.CustomDeathReason.LawyerSuicide:
                                    deathReasonString = $" - {Helpers.ColorString(Lawyer.color, "bad Lawyer")}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Bomb:
                                    deathReasonString = $" - bombed by {Helpers.ColorString(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Arson:
                                    deathReasonString = $" - burnt by {Helpers.ColorString(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
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
