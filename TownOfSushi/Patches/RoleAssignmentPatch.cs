using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using AmongUs.GameOptions;
using TownOfSushi.Utilities;
using static TownOfSushi.TownOfSushi;

namespace TownOfSushi.Patches 
{
    [HarmonyPatch(typeof(RoleOptionsCollectionV08), nameof(RoleOptionsCollectionV08.GetNumPerGame))]
    class RoleOptionsDataGetNumPerGamePatch
    {
        public static void Postfix(ref int __result) 
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal) __result = 0; // Deactivate Vanilla Roles if the mod roles are active
        }
    }

    [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.GetAdjustedNumImpostors))]
    class GameOptionsDataGetAdjustedNumImpostorsPatch 
    {
        public static void Postfix(ref int __result) 
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal) 
            {  // Ignore Vanilla impostor limits in TOR Games.
                __result = Mathf.Clamp(GameOptionsManager.Instance.CurrentGameOptions.NumImpostors, 1, 3);
            } 
        }
    }

    [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Validate))]
    class GameOptionsDataValidatePatch 
    {
        public static void Postfix(GameOptionsData __instance) 
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode != GameModes.Normal) return;
            __instance.NumImpostors = GameOptionsManager.Instance.CurrentGameOptions.NumImpostors;
        }
    }

    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
    class RoleManagerSelectRolesPatch 
    {
        private static int crewValues;
        private static int impValues;
        private static List<Tuple<byte, byte>> playerRoleMap = new List<Tuple<byte, byte>>();
        public static void Postfix() 
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ResetVaribles, Hazel.SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.ResetVariables();
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return; // Don't assign Roles in Hide N Seek
            AssignRoles();
        }

        private static void AssignRoles() 
        {
            var data = getRoleAssignmentData();
            AssignSpecialRoles(data); // Assign special roles like mafia and lovers first as they assign a role to multiple players and the chances are independent of the ticket system
            AssignEnsuredRoles(data); // Assign roles that should always be in the game next
            AssignChanceRoles(data); // Assign roles that may or may not be in the game last
            AssignRoleTargets(data); // Assign targets for Lawyer & Prosecutor
            AssignGuessers();
            AssignModifiers();
        }

        public static RoleAssignmentData getRoleAssignmentData() 
        {
            // Get the players that we want to assign the roles to. Crewmate and Neutral roles are assigned to natural crewmates. Impostor roles to impostors.
            List<PlayerControl> crewmates = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
            crewmates.RemoveAll(x => x.Data.Role.IsImpostor);
            List<PlayerControl> impostors = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
            impostors.RemoveAll(x => !x.Data.Role.IsImpostor);

            var crewmateMin = CustomOptionHolder.crewmateRolesCountMin.GetSelection();
            var crewmateMax = CustomOptionHolder.crewmateRolesCountMax.GetSelection();
            var neutralMin = CustomOptionHolder.neutralRolesCountMin.GetSelection();
            var neutralMax = CustomOptionHolder.neutralRolesCountMax.GetSelection();
            var neutralKMin = CustomOptionHolder.neutralKillingRolesCountMin.GetSelection();
            var neutralKMax = CustomOptionHolder.neutralKillingRolesCountMax.GetSelection();
            var impostorMin = CustomOptionHolder.impostorRolesCountMin.GetSelection();
            var impostorMax = CustomOptionHolder.impostorRolesCountMax.GetSelection();

            // Make sure min is less or equal to max
            if (crewmateMin > crewmateMax) crewmateMin = crewmateMax;
            if (neutralMin > neutralMax) neutralMin = neutralMax;
            if (neutralKMin > neutralKMax) neutralKMin = neutralKMax;
            if (impostorMin > impostorMax) impostorMin = impostorMax;

            // Automatically force everyone to get a role by setting crew Min / Max according to Neutral Settings
            if (CustomOptionHolder.crewmateRolesFill.GetBool()) 
            {
                int tempCrewmateMax = crewmates.Count - neutralMin;
                int tempCrewmateMin = crewmates.Count - neutralMax;
                int tempCrewmateMaxK = crewmates.Count - neutralKMin;
                int tempCrewmateMinK = crewmates.Count - neutralKMax;

                crewmateMax = Math.Min(tempCrewmateMax, tempCrewmateMaxK);
                crewmateMin = Math.Min(tempCrewmateMin, tempCrewmateMinK);
            }
           
            // Get the maximum allowed count of each role type based on the minimum and maximum option
            int crewCountSettings = rnd.Next(crewmateMin, crewmateMax + 1);
            int neutralCountSettings = rnd.Next(neutralMin, neutralMax + 1);
            int neutralKCountSettings = rnd.Next(neutralKMin, neutralKMax + 1);
            int impCountSettings = rnd.Next(impostorMin, impostorMax + 1);
            // If fill crewmates is enabled, make sure crew + neutral >= crewmates s.t. everyone has a role!
            while (crewCountSettings + neutralCountSettings + neutralKCountSettings < crewmates.Count && CustomOptionHolder.crewmateRolesFill.GetBool())
                crewCountSettings++;

            // Potentially lower the actual maximum to the assignable players
            int maxCrewmateRoles = Mathf.Min(crewmates.Count, crewCountSettings);
            int maxNeutralRoles = Mathf.Min(crewmates.Count, neutralCountSettings);
            int maxNeutralKRoles = Mathf.Min(crewmates.Count, neutralKCountSettings);
            int maxImpostorRoles = Mathf.Min(impostors.Count, impCountSettings);

            // Fill in the lists with the roles that should be assigned to players. Note that the special roles (like Mafia or Lovers) are NOT included in these lists
            Dictionary<byte, int> impSettings = new Dictionary<byte, int>();
            Dictionary<byte, int> neutralSettings = new Dictionary<byte, int>();
            Dictionary<byte, int> neutralKSettings = new Dictionary<byte, int>();
            Dictionary<byte, int> crewSettings = new Dictionary<byte, int>();
            
            impSettings.Add((byte)RoleId.Morphling, CustomOptionHolder.morphlingSpawnRate.GetSelection());
            impSettings.Add((byte)RoleId.Camouflager, CustomOptionHolder.camouflagerSpawnRate.GetSelection());
            impSettings.Add((byte)RoleId.Vampire, CustomOptionHolder.vampireSpawnRate.GetSelection());
            impSettings.Add((byte)RoleId.Eraser, CustomOptionHolder.eraserSpawnRate.GetSelection());
            impSettings.Add((byte)RoleId.Trickster, CustomOptionHolder.tricksterSpawnRate.GetSelection());
            impSettings.Add((byte)RoleId.Warlock, CustomOptionHolder.warlockSpawnRate.GetSelection());
            impSettings.Add((byte)RoleId.BountyHunter, CustomOptionHolder.bountyHunterSpawnRate.GetSelection());
            impSettings.Add((byte)RoleId.Witch, CustomOptionHolder.witchSpawnRate.GetSelection());
            impSettings.Add((byte)RoleId.Ninja, CustomOptionHolder.ninjaSpawnRate.GetSelection());
            impSettings.Add((byte)RoleId.Bomber, CustomOptionHolder.bomberSpawnRate.GetSelection());
            impSettings.Add((byte)RoleId.Yoyo, CustomOptionHolder.yoyoSpawnRate.GetSelection());

            neutralKSettings.Add((byte)RoleId.Jackal, CustomOptionHolder.jackalSpawnRate.GetSelection());
            neutralKSettings.Add((byte)RoleId.Glitch, CustomOptionHolder.GlitchSpawnRate.GetSelection());

            neutralSettings.Add((byte)RoleId.Jester, CustomOptionHolder.jesterSpawnRate.GetSelection());
            neutralSettings.Add((byte)RoleId.Arsonist, CustomOptionHolder.arsonistSpawnRate.GetSelection());
            neutralSettings.Add((byte)RoleId.Vulture, CustomOptionHolder.vultureSpawnRate.GetSelection());
            neutralSettings.Add((byte)RoleId.Thief, CustomOptionHolder.thiefSpawnRate.GetSelection());

            if (rnd.Next(1, 101) <= CustomOptionHolder.lawyerIsProsecutorChance.GetSelection() * 10) // Lawyer or Prosecutor
                neutralSettings.Add((byte)RoleId.Prosecutor, CustomOptionHolder.lawyerSpawnRate.GetSelection());
            else
                neutralSettings.Add((byte)RoleId.Lawyer, CustomOptionHolder.lawyerSpawnRate.GetSelection());

            crewSettings.Add((byte)RoleId.Mayor, CustomOptionHolder.mayorSpawnRate.GetSelection());
            crewSettings.Add((byte)RoleId.Sheriff, CustomOptionHolder.sheriffSpawnRate.GetSelection());
            crewSettings.Add((byte)RoleId.Portalmaker, CustomOptionHolder.portalmakerSpawnRate.GetSelection());
            crewSettings.Add((byte)RoleId.Engineer, CustomOptionHolder.engineerSpawnRate.GetSelection());
            crewSettings.Add((byte)RoleId.Lighter, CustomOptionHolder.lighterSpawnRate.GetSelection());
            crewSettings.Add((byte)RoleId.Detective, CustomOptionHolder.detectiveSpawnRate.GetSelection());
            crewSettings.Add((byte)RoleId.TimeMaster, CustomOptionHolder.timeMasterSpawnRate.GetSelection());
            crewSettings.Add((byte)RoleId.Medic, CustomOptionHolder.medicSpawnRate.GetSelection());
            crewSettings.Add((byte)RoleId.Swapper,CustomOptionHolder.swapperSpawnRate.GetSelection());
            crewSettings.Add((byte)RoleId.Seer, CustomOptionHolder.seerSpawnRate.GetSelection());
            crewSettings.Add((byte)RoleId.Hacker, CustomOptionHolder.hackerSpawnRate.GetSelection());
            crewSettings.Add((byte)RoleId.Tracker, CustomOptionHolder.trackerSpawnRate.GetSelection());
            crewSettings.Add((byte)RoleId.Snitch, CustomOptionHolder.snitchSpawnRate.GetSelection());
            crewSettings.Add((byte)RoleId.Medium, CustomOptionHolder.mediumSpawnRate.GetSelection());
            crewSettings.Add((byte)RoleId.Trapper, CustomOptionHolder.trapperSpawnRate.GetSelection());
            if (impostors.Count > 1) 
            {
                // Only add Spy if more than 1 impostor as the spy role is otherwise useless
                crewSettings.Add((byte)RoleId.Spy, CustomOptionHolder.spySpawnRate.GetSelection());
                // Only add Cleaner if more than 1 impostor as the Cleaner role is otherwise useless
                impSettings.Add((byte)RoleId.Cleaner, CustomOptionHolder.cleanerSpawnRate.GetSelection());
            }
            crewSettings.Add((byte)RoleId.SecurityGuard, CustomOptionHolder.securityGuardSpawnRate.GetSelection());

            return new RoleAssignmentData 
            {
                crewmates = crewmates,
                impostors = impostors,
                crewSettings = crewSettings,
                neutralSettings = neutralSettings,
                neutralKSettings = neutralKSettings,
                impSettings = impSettings,
                maxCrewmateRoles = maxCrewmateRoles,
                maxNeutralRoles = maxNeutralRoles,
                maxNeutralKRoles = maxNeutralKRoles,
                maxImpostorRoles = maxImpostorRoles
            };
        }

        private static void AssignSpecialRoles(RoleAssignmentData data) 
        {
            // Assign Mafia
            if (data.impostors.Count >= 3 && data.maxImpostorRoles >= 3 && (rnd.Next(1, 101) <= CustomOptionHolder.mafiaSpawnRate.GetSelection() * 10)) {
                SetRoleToRandomPlayer((byte)RoleId.Godfather, data.impostors);
                SetRoleToRandomPlayer((byte)RoleId.Janitor, data.impostors);
                SetRoleToRandomPlayer((byte)RoleId.Mafioso, data.impostors);
                data.maxImpostorRoles -= 3;
            }
        }

        private static void AssignEnsuredRoles(RoleAssignmentData data) 
        {
            // Get all roles where the chance to occur is set to 100%
            List<byte> ensuredCrewmateRoles = data.crewSettings.Where(x => x.Value == 10).Select(x => x.Key).ToList();
            List<byte> ensuredNeutralRoles = data.neutralSettings.Where(x => x.Value == 10).Select(x => x.Key).ToList();
            List<byte> ensuredNeutralKRoles = data.neutralKSettings.Where(x => x.Value == 10).Select(x => x.Key).ToList();
            List<byte> ensuredImpostorRoles = data.impSettings.Where(x => x.Value == 10).Select(x => x.Key).ToList();

            // Assign roles until we run out of either players we can assign roles to or run out of roles we can assign to players
            while (
                (data.impostors.Count > 0 && data.maxImpostorRoles > 0 && ensuredImpostorRoles.Count > 0) || 
                (data.crewmates.Count > 0 && (
                    (data.maxCrewmateRoles > 0 && ensuredCrewmateRoles.Count > 0) || 
                    (data.maxNeutralRoles > 0 && ensuredNeutralRoles.Count > 0)|| 
                    (data.maxNeutralKRoles > 0 && ensuredNeutralKRoles.Count > 0)
                ))) {
                    
                Dictionary<RoleType, List<byte>> rolesToAssign = new Dictionary<RoleType, List<byte>>();
                if (data.crewmates.Count > 0 && data.maxCrewmateRoles > 0 && ensuredCrewmateRoles.Count > 0) rolesToAssign.Add(RoleType.Crewmate, ensuredCrewmateRoles);
                if (data.crewmates.Count > 0 && data.maxNeutralRoles > 0 && ensuredNeutralRoles.Count > 0) rolesToAssign.Add(RoleType.Neutral, ensuredNeutralRoles);
                if (data.crewmates.Count > 0 && data.maxNeutralKRoles > 0 && ensuredNeutralKRoles.Count > 0) rolesToAssign.Add(RoleType.NK, ensuredNeutralKRoles);
                if (data.impostors.Count > 0 && data.maxImpostorRoles > 0 && ensuredImpostorRoles.Count > 0) rolesToAssign.Add(RoleType.Impostor, ensuredImpostorRoles);
                
                // Randomly select a pool of roles to assign a role from next (Crewmate role, Neutral role or Impostor role) 
                // then select one of the roles from the selected pool to a player 
                // and remove the role (and any potentially blocked role pairings) from the pool(s)
                var roleType = rolesToAssign.Keys.ElementAt(rnd.Next(0, rolesToAssign.Keys.Count())); 
                var players = roleType == RoleType.Crewmate || roleType == RoleType.Neutral || roleType == RoleType.NK ? data.crewmates : data.impostors;
                var index = rnd.Next(0, rolesToAssign[roleType].Count);
                var roleId = rolesToAssign[roleType][index];
                SetRoleToRandomPlayer(rolesToAssign[roleType][index], players);
                rolesToAssign[roleType].RemoveAt(index);

                if (CustomOptionHolder.blockedRolePairings.ContainsKey(roleId)) {
                    foreach(var blockedRoleId in CustomOptionHolder.blockedRolePairings[roleId]) 
                    {
                        // Set chance for the blocked roles to 0 for chances less than 100%
                        if (data.impSettings.ContainsKey(blockedRoleId)) data.impSettings[blockedRoleId] = 0;
                        if (data.neutralSettings.ContainsKey(blockedRoleId)) data.neutralSettings[blockedRoleId] = 0;
                        if (data.neutralKSettings.ContainsKey(blockedRoleId)) data.neutralKSettings[blockedRoleId] = 0;
                        if (data.crewSettings.ContainsKey(blockedRoleId)) data.crewSettings[blockedRoleId] = 0;
                        // Remove blocked roles even if the chance was 100%
                        foreach(var ensuredRolesList in rolesToAssign.Values) {
                            ensuredRolesList.RemoveAll(x => x == blockedRoleId);
                        }
                    }
                }

                // Adjust the role limit
                switch (roleType) {
                    case RoleType.Crewmate: data.maxCrewmateRoles--; crewValues -= 10; break;
                    case RoleType.Neutral: data.maxNeutralRoles--; break;
                    case RoleType.NK: data.maxNeutralKRoles--; break;
                    case RoleType.Impostor: data.maxImpostorRoles--; impValues -= 10;  break;
                }
            }
        }
        private static void AssignChanceRoles(RoleAssignmentData data) 
        {
            // Get all roles where the chance to occur is set grater than 0% but not 100% and build a ticket pool based on their weight
            List<byte> crewmateTickets = data.crewSettings.Where(x => x.Value > 0 && x.Value < 10).Select(x => Enumerable.Repeat(x.Key, x.Value)).SelectMany(x => x).ToList();
            List<byte> neutralTickets = data.neutralSettings.Where(x => x.Value > 0 && x.Value < 10).Select(x => Enumerable.Repeat(x.Key, x.Value)).SelectMany(x => x).ToList();
            List<byte> neutralKTickets = data.neutralKSettings.Where(x => x.Value > 0 && x.Value < 10).Select(x => Enumerable.Repeat(x.Key, x.Value)).SelectMany(x => x).ToList();
            List<byte> impostorTickets = data.impSettings.Where(x => x.Value > 0 && x.Value < 10).Select(x => Enumerable.Repeat(x.Key, x.Value)).SelectMany(x => x).ToList();

            // Assign roles until we run out of either players we can assign roles to or run out of roles we can assign to players
            while (
                (data.impostors.Count > 0 && data.maxImpostorRoles > 0 && impostorTickets.Count > 0) || 
                (data.crewmates.Count > 0 && (
                    (data.maxCrewmateRoles > 0 && crewmateTickets.Count > 0) || 
                    (data.maxNeutralRoles > 0 && neutralTickets.Count > 0)|| 
                    (data.maxNeutralKRoles > 0 && neutralKTickets.Count > 0)
                ))) {
                
                Dictionary<RoleType, List<byte>> rolesToAssign = new Dictionary<RoleType, List<byte>>();
                if (data.crewmates.Count > 0 && data.maxCrewmateRoles > 0 && crewmateTickets.Count > 0) rolesToAssign.Add(RoleType.Crewmate, crewmateTickets);
                if (data.crewmates.Count > 0 && data.maxNeutralRoles > 0 && neutralTickets.Count > 0) rolesToAssign.Add(RoleType.Neutral, neutralTickets);
                if (data.crewmates.Count > 0 && data.maxNeutralKRoles > 0 && neutralKTickets.Count > 0) rolesToAssign.Add(RoleType.NK, neutralKTickets);
                if (data.impostors.Count > 0 && data.maxImpostorRoles > 0 && impostorTickets.Count > 0) rolesToAssign.Add(RoleType.Impostor, impostorTickets);
                
                // Randomly select a pool of role tickets to assign a role from next (Crewmate role, Neutral role or Impostor role) 
                // then select one of the roles from the selected pool to a player 
                // and remove all tickets of this role (and any potentially blocked role pairings) from the pool(s)
                var roleType = rolesToAssign.Keys.ElementAt(rnd.Next(0, rolesToAssign.Keys.Count()));
                var players = roleType == RoleType.Crewmate || roleType == RoleType.Neutral || roleType == RoleType.NK ? data.crewmates : data.impostors;
                var index = rnd.Next(0, rolesToAssign[roleType].Count);
                var roleId = rolesToAssign[roleType][index];
                SetRoleToRandomPlayer(roleId, players);
                rolesToAssign[roleType].RemoveAll(x => x == roleId);

                if (CustomOptionHolder.blockedRolePairings.ContainsKey(roleId)) 
                {
                    foreach(var blockedRoleId in CustomOptionHolder.blockedRolePairings[roleId]) 
                    {
                        // Remove tickets of blocked roles from all pools
                        crewmateTickets.RemoveAll(x => x == blockedRoleId);
                        neutralTickets.RemoveAll(x => x == blockedRoleId);
                        neutralKTickets.RemoveAll(x => x == blockedRoleId);
                        impostorTickets.RemoveAll(x => x == blockedRoleId);
                    }
                }

                // Adjust the role limit
                switch (roleType) {
                    case RoleType.Crewmate: data.maxCrewmateRoles--; break;
                    case RoleType.Neutral: data.maxNeutralRoles--;break;
                    case RoleType.NK: data.maxNeutralKRoles--;break;
                    case RoleType.Impostor: data.maxImpostorRoles--;break;
                }
            }
        }

        private static void AssignRoleTargets(RoleAssignmentData data) 
        {
            // Set Lawyer or Prosecutor Target
            if (Lawyer.lawyer != null) 
            {
                var possibleTargets = new List<PlayerControl>();
                if (!Lawyer.isProsecutor) 
                { // Lawyer
                    foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                        if (!p.Data.IsDead && !p.Data.Disconnected && p != Lovers.lover1 && p != Lovers.lover2 && (p.Data.Role.IsImpostor || p == Jackal.jackal || (Lawyer.targetCanBeJester && p == Jester.jester)))
                            possibleTargets.Add(p);
                    }
                } 
                else 
                { // Prosecutor
                    foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                        if (!p.Data.IsDead && !p.Data.Disconnected && p != Lovers.lover1 && p != Lovers.lover2 && p != Mini.mini && !p.Data.Role.IsImpostor && !Helpers.IsNeutral(p) && p != Swapper.swapper)
                            possibleTargets.Add(p);
                    }
                }
                
                if (possibleTargets.Count == 0) {
                    MessageWriter w = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(w);
                    RPCProcedure.LawyerPromotesToPursuer();
                } else {
                    var target = possibleTargets[TownOfSushi.rnd.Next(0, possibleTargets.Count)];
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerSetTarget, Hazel.SendOption.Reliable, -1);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.LawyerSetTarget(target.PlayerId);
                }
            }
        }

        private static void AssignModifiers() 
        {
            var modifierMin = CustomOptionHolder.modifiersCountMin.GetSelection();
            var modifierMax = CustomOptionHolder.modifiersCountMax.GetSelection();
            if (modifierMin > modifierMax) modifierMin = modifierMax;
            int modifierCountSettings = rnd.Next(modifierMin, modifierMax + 1);
            List<PlayerControl> players = PlayerControl.AllPlayerControls.ToArray().ToList();
            if (!CustomOptionHolder.GuesserHaveModifier.GetBool())
                players.RemoveAll(x => Guesser.IsGuesser(x.PlayerId));
            int modifierCount = Mathf.Min(players.Count, modifierCountSettings);

            if (modifierCount == 0) return;

            List<RoleId> allModifiers = new List<RoleId>();
            List<RoleId> ensuredModifiers = new List<RoleId>();
            List<RoleId> chanceModifiers = new List<RoleId>();
            allModifiers.AddRange(new List<RoleId> 
            {
                RoleId.Tiebreaker,
                RoleId.Mini,
                RoleId.Bait,
                RoleId.Bloody,
                RoleId.AntiTeleport,
                RoleId.Sunglasses,
                RoleId.Vip,
                RoleId.Invert,
                RoleId.Chameleon,
                RoleId.Armored,
                RoleId.Shifter
            });

            if (rnd.Next(1, 101) <= CustomOptionHolder.modifierLover.GetSelection() * 10) { // Assign lover
                bool isEvilLover = rnd.Next(1, 101) <= CustomOptionHolder.modifierLoverImpLoverRate.GetSelection() * 10;
                byte firstLoverId;
                List<PlayerControl> impPlayer = new List<PlayerControl>(players);
                List<PlayerControl> crewPlayer = new List<PlayerControl>(players);
                impPlayer.RemoveAll(x => !x.Data.Role.IsImpostor);
                crewPlayer.RemoveAll(x => x.Data.Role.IsImpostor || x == Lawyer.lawyer);

                if (isEvilLover) firstLoverId = SetModifierToRandomPlayer((byte)RoleId.Lover, impPlayer);
                else firstLoverId = SetModifierToRandomPlayer((byte)RoleId.Lover, crewPlayer);
                byte secondLoverId = SetModifierToRandomPlayer((byte)RoleId.Lover, crewPlayer, 1);

                players.RemoveAll(x => x.PlayerId == firstLoverId || x.PlayerId == secondLoverId);
                modifierCount--;
            }

            foreach (RoleId m in allModifiers) {
                if (GetSelectionForRoleId(m) == 10) ensuredModifiers.AddRange(Enumerable.Repeat(m, GetSelectionForRoleId(m, true) / 10));
                else chanceModifiers.AddRange(Enumerable.Repeat(m, GetSelectionForRoleId(m, true)));
            }

            AssignModifiersToPlayers(ensuredModifiers, players, modifierCount); // Assign ensured modifier

            modifierCount -= ensuredModifiers.Count;
            if (modifierCount <= 0) return;
            int chanceModifierCount = Mathf.Min(modifierCount, chanceModifiers.Count);
            List<RoleId> chanceModifierToAssign = new List<RoleId>();
            while (chanceModifierCount > 0 && chanceModifiers.Count > 0) {
                var index = rnd.Next(0, chanceModifiers.Count);
                RoleId modifierId = chanceModifiers[index];
                chanceModifierToAssign.Add(modifierId);

                int modifierSelection = GetSelectionForRoleId(modifierId);
                while (modifierSelection > 0) {
                    chanceModifiers.Remove(modifierId);
                    modifierSelection--;
                }
                chanceModifierCount--;
            }

            AssignModifiersToPlayers(chanceModifierToAssign, players, modifierCount); // Assign chance modifier
        }

        private static void AssignGuessers() 
        {
            List<PlayerControl> impPlayer = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
            List<PlayerControl> neutralPlayer = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
            List<PlayerControl> crewPlayer = PlayerControl.AllPlayerControls.ToArray().ToList().OrderBy(x => Guid.NewGuid()).ToList();
            impPlayer.RemoveAll(x => !x.Data.Role.IsImpostor);
            neutralPlayer.RemoveAll(x => !x.IsNeutralKiller());
            crewPlayer.RemoveAll(x => !x.IsCrew());
            AssignGuesserToPlayers(crewPlayer, Mathf.RoundToInt(CustomOptionHolder.GuesserCrewNumber.GetFloat()));
            AssignGuesserToPlayers(neutralPlayer, Mathf.RoundToInt(CustomOptionHolder.GuesserNeutralNumber.GetFloat()));
            AssignGuesserToPlayers(impPlayer, Mathf.RoundToInt(CustomOptionHolder.GuesserImpNumber.GetFloat()));
        }

        private static void AssignGuesserToPlayers(List<PlayerControl> playerList, int count) 
        {
            for (int i = 0; i < count && playerList.Count > 0; i++) 
            {
                var index = rnd.Next(0, playerList.Count);
                byte playerId = playerList[index].PlayerId;
                playerList.RemoveAt(index);

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGuessers, Hazel.SendOption.Reliable, -1);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.SetGuessers(playerId);
            }
        }

        private static byte SetRoleToRandomPlayer(byte roleId, List<PlayerControl> playerList, bool removePlayer = true) 
        {
            var index = rnd.Next(0, playerList.Count);
            byte playerId = playerList[index].PlayerId;
            if (removePlayer) playerList.RemoveAt(index);

            playerRoleMap.Add(new Tuple<byte, byte>(playerId, roleId));

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetRole, Hazel.SendOption.Reliable, -1);
            writer.Write(roleId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.SetRole(roleId, playerId);
            return playerId;
        }

        private static byte SetModifierToRandomPlayer(byte modifierId, List<PlayerControl> playerList, byte flag = 0) 
        {
            if (playerList.Count == 0) return Byte.MaxValue;
            var index = rnd.Next(0, playerList.Count);
            byte playerId = playerList[index].PlayerId;
            playerList.RemoveAt(index);

            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetModifier, Hazel.SendOption.Reliable, -1);
            writer.Write(modifierId);
            writer.Write(playerId);
            writer.Write(flag);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.SetModifier(modifierId, playerId, flag);
            return playerId;
        }

        private static void AssignModifiersToPlayers(List<RoleId> modifiers, List<PlayerControl> playerList, int modifierCount) 
        {
            modifiers = modifiers.OrderBy(x => rnd.Next()).ToList(); // randomize list

            while (modifierCount < modifiers.Count) 
            {
                var index = rnd.Next(0, modifiers.Count);
                modifiers.RemoveAt(index);
            }

            byte playerId;

            List<PlayerControl> crewPlayer = new List<PlayerControl>(playerList);
            crewPlayer.RemoveAll(x => !x.IsCrew());
            if (modifiers.Contains(RoleId.Shifter)) 
            {
                var crewPlayerShifter = new List<PlayerControl>(crewPlayer);
                crewPlayerShifter.RemoveAll(x => x == Spy.spy);
                playerId = SetModifierToRandomPlayer((byte)RoleId.Shifter, crewPlayerShifter);
                crewPlayer.RemoveAll(x => x.PlayerId == playerId);
                playerList.RemoveAll(x => x.PlayerId == playerId);
                modifiers.RemoveAll(x => x == RoleId.Shifter);
            }
            if (modifiers.Contains(RoleId.Sunglasses)) 
            {
                int sunglassesCount = 0;
                while (sunglassesCount < modifiers.FindAll(x => x == RoleId.Sunglasses).Count) 
                {
                    playerId = SetModifierToRandomPlayer((byte)RoleId.Sunglasses, crewPlayer);
                    crewPlayer.RemoveAll(x => x.PlayerId == playerId);
                    playerList.RemoveAll(x => x.PlayerId == playerId);
                    sunglassesCount++;
                }
                modifiers.RemoveAll(x => x == RoleId.Sunglasses);
            }

            foreach (RoleId modifier in modifiers) 
            {
                if (playerList.Count == 0) break;
                playerId = SetModifierToRandomPlayer((byte)modifier, playerList);
                playerList.RemoveAll(x => x.PlayerId == playerId);
            }
        }

        private static int GetSelectionForRoleId(RoleId roleId, bool multiplyQuantity = false) 
        {
            int selection = 0;
            switch (roleId) {
                case RoleId.Lover:
                    selection = CustomOptionHolder.modifierLover.GetSelection(); break;
                case RoleId.Tiebreaker:
                    selection = CustomOptionHolder.modifierTieBreaker.GetSelection(); break;
                case RoleId.Mini:
                    selection = CustomOptionHolder.modifierMini.GetSelection(); break;
                case RoleId.Bait:
                    selection = CustomOptionHolder.modifierBait.GetSelection();
                    if (multiplyQuantity) selection *= CustomOptionHolder.modifierBaitQuantity.getQuantity();
                    break;
                case RoleId.Bloody:
                    selection = CustomOptionHolder.modifierBloody.GetSelection();
                    if (multiplyQuantity) selection *= CustomOptionHolder.modifierBloodyQuantity.getQuantity();
                    break;
                case RoleId.AntiTeleport:
                    selection = CustomOptionHolder.modifierAntiTeleport.GetSelection();
                    if (multiplyQuantity) selection *= CustomOptionHolder.modifierAntiTeleportQuantity.getQuantity();
                    break;
                case RoleId.Sunglasses:
                    selection = CustomOptionHolder.modifierSunglasses.GetSelection();
                    if (multiplyQuantity) selection *= CustomOptionHolder.modifierSunglassesQuantity.getQuantity();
                    break;
                case RoleId.Vip:
                    selection = CustomOptionHolder.modifierVip.GetSelection();
                    if (multiplyQuantity) selection *= CustomOptionHolder.modifierVipQuantity.getQuantity();
                    break;
                case RoleId.Invert:
                    selection = CustomOptionHolder.modifierInvert.GetSelection();
                    if (multiplyQuantity) selection *= CustomOptionHolder.modifierInvertQuantity.getQuantity();
                    break;
                case RoleId.Chameleon:
                    selection = CustomOptionHolder.modifierChameleon.GetSelection();
                    if (multiplyQuantity) selection *= CustomOptionHolder.modifierChameleonQuantity.getQuantity();
                    break;
                case RoleId.Armored:
                    selection = CustomOptionHolder.modifierArmored.GetSelection();
                    break;
                case RoleId.Shifter:
                    selection = CustomOptionHolder.modifierShifter.GetSelection();
                    break;
            }
                 
            return selection;
        }

        private static void SetRolesAgain()
        {

            while (playerRoleMap.Any())
            {
                byte amount = (byte)Math.Min(playerRoleMap.Count, 20);
                var writer = AmongUsClient.Instance!.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WorkaroundSetRoles, SendOption.Reliable, -1);
                writer.Write(amount);
                for (int i = 0; i < amount; i++)
                {
                    var option = playerRoleMap[0];
                    playerRoleMap.RemoveAt(0);
                    writer.WritePacked((uint)option.Item1);
                    writer.WritePacked((uint)option.Item2);
                }
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }

        public class RoleAssignmentData 
        {
            public List<PlayerControl> crewmates {get;set;}
            public List<PlayerControl> impostors {get;set;}
            public Dictionary<byte, int> impSettings = new Dictionary<byte, int>();
            public Dictionary<byte, int> neutralSettings = new Dictionary<byte, int>();
            public Dictionary<byte, int> neutralKSettings = new Dictionary<byte, int>();
            public Dictionary<byte, int> crewSettings = new Dictionary<byte, int>();
            public int maxCrewmateRoles {get;set;}
            public int maxNeutralRoles {get;set;}
            public int maxNeutralKRoles {get;set;}
            public int maxImpostorRoles {get;set;}
        }
        
        private enum RoleType 
        {
            Crewmate = 0,
            Neutral = 1,
            Impostor = 2,
            NK = 3
        }

    }
}
