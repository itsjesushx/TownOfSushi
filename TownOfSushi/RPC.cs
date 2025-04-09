using HarmonyLib;
using Hazel;
using static TownOfSushi.TownOfSushi;
using static TownOfSushi.HudManagerStartPatch;
using static TownOfSushi.GameHistory;
using static TownOfSushi.MapOptions;
using TownOfSushi.Objects;
using TownOfSushi.Patches;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using TownOfSushi.Utilities;
using AmongUs.Data;
using AmongUs.GameOptions;
using Assets.CoreScripts;
using Reactor.Networking.Extensions;
using Reactor.Utilities.Extensions;

namespace TownOfSushi
{
    public static class RPCProcedure 
    {
        // Main Controls
        public static void ResetVariables() 
        {
            Garlic.clearGarlics();
            JackInTheBox.ClearJackInTheBoxes();
            NinjaTrace.ClearTraces();
            Silhouette.ClearSilhouettes();
            Portal.ClearPortals();
            Bloodytrail.ResetSprites();
            Trap.ClearTraps();
            Helpers.ToggleZoom(reset : true);
            GameStartManagerPatch.GameStartManagerUpdatePatch.startingTimer = 0;
            SurveillanceMinigamePatch.nightVisionOverlays = null;
            EventUtility.ClearAndReload();
            MapBehaviourPatch.ClearAndReload();
            ClearAndReloadMapOptions();
            ClearAndReloadRoles();
            ClearGameHistory();
            SetCustomButtonCooldowns();
            ReloadPluginOptions();
        }

        public static void HandleShareOptions(byte numberOfOptions, MessageReader reader) 
        {
            try 
            {
                for (int i = 0; i < numberOfOptions; i++) {
                    uint optionId = reader.ReadPackedUInt32();
                    uint selection = reader.ReadPackedUInt32();
                    CustomOption option = CustomOption.options.First(option => option.id == (int)optionId);
                    option.UpdateSelection((int)selection, i == numberOfOptions - 1);
                }
            } 
            catch (Exception e) 
            {
                TownOfSushiPlugin.Logger.LogError("Error while deserializing options: " + e.Message);
            }
        }

        public static void ForceEnd() 
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.Role.IsImpostor)
                {
                    
                    GameData.Instance.GetPlayerById(player.PlayerId); // player.RemoveInfected(); (was removed in 2022.12.08, no idea if we ever need that part again, replaced by these 2 lines.) 
                    player.CoSetRole(RoleTypes.Crewmate, true);

                    player.MurderPlayer(player);
                    player.Data.IsDead = true;
                }
            }
        }

        public static void StopStart(byte playerId) 
        {
            if (AmongUsClient.Instance.AmHost && CustomOptionHolder.anyPlayerCanStopStart.GetBool()) 
            {
                GameStartManager.Instance.ResetStartState();
                PlayerControl.LocalPlayer.RpcSendChat($"{Helpers.PlayerById(playerId).Data.PlayerName} stopped the game start!");
            }
        }

        public static void WorkaroundSetRoles(byte numberOfRoles, MessageReader reader)
        {
                for (int i = 0; i < numberOfRoles; i++)
                {                   
                    byte playerId = (byte) reader.ReadPackedUInt32();
                    byte roleId = (byte) reader.ReadPackedUInt32();
                    try {
                        SetRole(roleId, playerId);
                    } catch (Exception e) {
                        TownOfSushiPlugin.Logger.LogError("Error while deserializing roles: " + e.Message);
                    }
            }
            
        }

        public static void SetRole(byte roleId, byte playerId) 
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls) 
            {
                if (player.PlayerId == playerId) 
                {
                    switch ((RoleId)roleId) 
                    {
                    case RoleId.Jester:
                        Jester.Player = player;
                        break;
                    case RoleId.Mayor:
                        Mayor.Player = player;
                        break;
                    case RoleId.Portalmaker:
                        Portalmaker.Player = player;
                        break;
                    case RoleId.Engineer:
                        Engineer.Player = player;
                        break;
                    case RoleId.Sheriff:
                        Sheriff.Player = player;
                        break;
                    case RoleId.VengefulRomantic:
                        VengefulRomantic.Player = player;
                        break;
                    case RoleId.Glitch:
                        Glitch.Player = player;
                        break;
                    case RoleId.Werewolf:
                        Werewolf.Player = player;
                        break;
                    case RoleId.Lighter:
                        Lighter.Player = player;
                        break;
                    case RoleId.Agent:
                        Agent.Player = player;
                        break;
                    case RoleId.Hitman:
                        Hitman.Player = player;
                        break;
                    case RoleId.Undertaker:
                        Undertaker.Player = player;
                        break;
                    case RoleId.Oracle:
                        Oracle.Player = player;
                        break;
                    case RoleId.Godfather:
                        Godfather.Player = player;
                        break;
                    case RoleId.Amnesiac:
                        Amnesiac.Player = player;
                        break;
                    case RoleId.Mafioso:
                        Mafioso.Player = player;
                        break;
                    case RoleId.Janitor:
                        Janitor.Player = player;
                        break;
                    case RoleId.Plaguebearer:
                        Plaguebearer.Player = player;
                        break;
                    case RoleId.Pestilence:
                        Pestilence.Player = player;
                        break;
                    case RoleId.Detective:
                        Detective.Player = player;
                        break;
                    case RoleId.TimeMaster:
                        TimeMaster.Player = player;
                        break;
                    case RoleId.Veteran:
                        Veteran.Player = player;
                        break;
                    case RoleId.Medic:
                        Medic.Player = player;
                        break;
                    case RoleId.Crusader:
                        Crusader.Player = player;
                        break;
                    case RoleId.Swapper:
                        Swapper.Player = player;
                        break;
                    case RoleId.Mystic:
                        Mystic.Player = player;
                        break;
                    case RoleId.Juggernaut:
                        Juggernaut.Player = player;
                        break;
                    case RoleId.Morphling:
                        Morphling.Player = player;
                        break;
                    case RoleId.Camouflager:
                        Camouflager.Player = player;
                        break;
                    case RoleId.SerialKiller:
                        SerialKiller.Player = player;
                        break;
                    case RoleId.Hacker:
                        Hacker.Player = player;
                        break;
                    case RoleId.Tracker:
                        Tracker.Player = player;
                        break;
                    case RoleId.Vampire:
                        Vampire.Player = player;
                        break;
                    case RoleId.Snitch:
                        Snitch.Player = player;
                        break;
                    case RoleId.Jackal:
                        Jackal.Player = player;
                        break;
                    case RoleId.Romantic:
                        Romantic.Player = player;
                        break;
                    case RoleId.Sidekick:
                        Sidekick.Player = player;
                        break;
                    case RoleId.Eraser:
                        Eraser.Player = player;
                        break;
                    case RoleId.Spy:
                        Spy.Player = player;
                        break;
                    case RoleId.Trickster:
                        Trickster.Player = player;
                        break;
                    case RoleId.Cleaner:
                        Cleaner.Player = player;
                        break;
                    case RoleId.Warlock:
                        Warlock.Player = player;
                        break;
                    case RoleId.Vigilante:
                        Vigilante.Player = player;
                        break;
                    case RoleId.Arsonist:
                        Arsonist.Player = player;
                        break;
                    case RoleId.BountyHunter:
                        BountyHunter.Player = player;
                        break;
                    case RoleId.Vulture:
                        Vulture.Player = player;
                        break;
                    case RoleId.Medium:
                        Medium.medium = player;
                        break;
                    case RoleId.Trapper:
                        Trapper.Player = player;
                        break;
                    case RoleId.Lawyer:
                        Lawyer.Player = player;
                        break;
                    case RoleId.Prosecutor:
                        Lawyer.Player = player;
                        Lawyer.isProsecutor = true;
                        break;
                    case RoleId.Pursuer:
                        Pursuer.Player = player;
                        break;
                    case RoleId.Witch:
                        Witch.Player = player;
                        break;
                    case RoleId.Ninja:
                        Ninja.ninja = player;
                        break;
                    case RoleId.Thief:
                        Thief.Player = player;
                        break;
                    case RoleId.Yoyo:
                        Yoyo.Player = player;
                        break;
                    }
                    if (AmongUsClient.Instance.AmHost && player.IsVenter() && !player.Data.Role.IsImpostor) 
                    {
                        player.RpcSetRole(RoleTypes.Engineer);
                        player.CoSetRole(RoleTypes.Engineer, true);
                    }
                }
            }
        }

        public static void SetModifier(byte modifierId, byte playerId, byte flag) 
        {
            PlayerControl player = Helpers.PlayerById(playerId); 
            switch ((RoleId)modifierId) 
            {
                case RoleId.Bait:
                    Bait.Players.Add(player);
                    break;
                case RoleId.Lover:
                    if (flag == 0) Lovers.Lover1 = player;
                    else Lovers.Lover2 = player;
                    break;
                case RoleId.Bloody:
                    global::TownOfSushi.Bloody.Players.Add(player);
                    break;
                case RoleId.AntiTeleport:
                    AntiTeleport.Players.Add(player);
                    break;
                case RoleId.Sleuth:
                    Sleuth.Players.Add(player);
                    break;
                case RoleId.Tiebreaker:
                    Tiebreaker.Player = player;
                    break;
                case RoleId.Disperser:
                    Disperser.Player = player;
                    break;
                case RoleId.Sunglasses:
                    Sunglasses.Players.Add(player);
                    break;
                case RoleId.Mini:
                    Mini.Player = player;
                    break;
                case RoleId.Vip:
                    Vip.Players.Add(player);
                    break;
                case RoleId.Invert:
                    Invert.Players.Add(player);
                    break;
                case RoleId.Chameleon:
                    Chameleon.Players.Add(player);
                    break;
                case RoleId.Armored:
                    Armored.Player = player;
                    break;
                case RoleId.Shifter:
                    Shifter.Player = player;
                    break;
            }
        }

        public static void VersionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId) 
        {
            System.Version ver;
            if (revision < 0) 
                ver = new System.Version(major, minor, build);
            else 
                ver = new System.Version(major, minor, build, revision);
            GameStartManagerPatch.playerVersions[clientId] = new GameStartManagerPatch.PlayerVersion(ver, guid);
        }

        public static void UseUncheckedVent(int ventId, byte playerId, byte isEnter) 
        {
            PlayerControl player = Helpers.PlayerById(playerId);
            if (player == null) return;
            // Fill dummy MessageReader and call MyPhysics.HandleRpc as the corountines cannot be accessed
            MessageReader reader = new MessageReader();
            byte[] bytes = BitConverter.GetBytes(ventId);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            reader.Buffer = bytes;
            reader.Length = bytes.Length;

            JackInTheBox.StartAnimation(ventId);
            player.MyPhysics.HandleRpc(isEnter != 0 ? (byte)19 : (byte)20, reader);
        }

        public static void UncheckedMurderPlayer(byte sourceId, byte targetId, byte showAnimation) 
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            PlayerControl source = Helpers.PlayerById(sourceId);
            PlayerControl target = Helpers.PlayerById(targetId);
            if (source != null && target != null) 
            {
                if (showAnimation == 0) KillAnimationCoPerformKillPatch.hideNextAnimation = true;
                source.MurderPlayer(target);
            }
        }

        public static void UncheckedCmdReportDeadBody(byte sourceId, byte targetId) 
        {
            PlayerControl source = Helpers.PlayerById(sourceId);
            var t = targetId == Byte.MaxValue ? null : Helpers.PlayerById(targetId).Data;
            if (source != null) source.ReportDeadBody(t);
        }

        public static void UncheckedExilePlayer(byte targetId) 
        {
            PlayerControl target = Helpers.PlayerById(targetId);
            if (target != null) target.Exiled();
        }

        public static void DynamicMapOption(byte mapId) 
        {
           GameOptionsManager.Instance.currentNormalGameOptions.MapId = mapId;
        }

        public static void SetGameStarting() 
        {
            GameStartManagerPatch.GameStartManagerUpdatePatch.startingTimer = 5f;
        }

        // Role functionality

        public static void EngineerFixLights() 
        {
            SwitchSystem switchSystem = MapUtilities.Systems[SystemTypes.Electrical].CastFast<SwitchSystem>();
            switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;
        }

        public static void EngineerFixSubmergedOxygen() 
        {
            SubmergedCompatibility.RepairOxygen();
        }

        public static void EngineerUsedRepair() 
        {
            Engineer.remainingFixes--;
            if (Helpers.ShouldShowGhostInfo()) 
            {
                Helpers.ShowFlash(Engineer.Color, 0.5f, "Engineer Fix");
            }
        }

        public static void CleanBody(byte playerId, byte cleaningPlayerId) 
        {
            if (Medium.futureDeadBodies != null) 
            {
                var deadBody = Medium.futureDeadBodies.Find(x => x.Item1.player.PlayerId == playerId).Item1;
                if (deadBody != null) deadBody.wasCleaned = true;
            }

            DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            for (int i = 0; i < array.Length; i++) 
            {
                if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == playerId) {
                    UnityEngine.Object.Destroy(array[i].gameObject);
                }     
            }
            if (Vulture.Player != null && cleaningPlayerId == Vulture.Player.PlayerId) 
            {
                Vulture.eatenBodies++;
                if (Vulture.eatenBodies == Vulture.vultureNumberToWin) 
                {
                    Vulture.IsVultureWin = true;
                }
            }
        }

        public static void TimeMasterRewindTime() 
        {
            TimeMaster.shieldActive = false; // Shield is no longer active when rewinding
            SoundEffectsManager.Stop("timemasterShield");  // Shield sound stopped when rewinding
            if(TimeMaster.Player != null && TimeMaster.Player == PlayerControl.LocalPlayer) {
                ResetTimeMasterButton();
            }
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(0f, 0.5f, 0.8f, 0.3f);
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(TimeMaster.rewindTime / 2, new Action<float>((p) => {
                if (p == 1f) FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = false;
            })));

            if (TimeMaster.Player == null || PlayerControl.LocalPlayer == TimeMaster.Player) return; // Time Master himself does not rewind

            TimeMaster.isRewinding = true;

            if (MapBehaviour.Instance)
                MapBehaviour.Instance.Close();
            if (Minigame.Instance)
                Minigame.Instance.ForceClose();
            PlayerControl.LocalPlayer.moveable = false;
        }

        public static void TimeMasterShield() 
        {
            TimeMaster.shieldActive = true;
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(TimeMaster.shieldDuration, new Action<float>((p) => {
                if (p == 1f) TimeMaster.shieldActive = false;
            })));
        }

        public static void VeteranAlert() 
        {
            Veteran.AlertActive = true;
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Veteran.Duration, new Action<float>((p) => 
            {
                if (p == 1f) Veteran.AlertActive = false;
            })));
        }

        public static void VeterenAlertKill(byte targetId)
        {
            if (PlayerControl.LocalPlayer == Veteran.Player)
            {
                PlayerControl player = Helpers.PlayerById(targetId);
                Helpers.CheckMurderAttemptAndKill(Veteran.Player, player);
            }
        }

        public static void MedicSetShielded(byte shieldedId) 
        {
            Medic.usedShield = true;
            Medic.shielded = Helpers.PlayerById(shieldedId);
            Medic.futureShielded = null;
        }
        public static void RomanticSetBeloved(byte belovedId) 
        {
            Romantic.HasLover = true;
            Romantic.beloved = Helpers.PlayerById(belovedId);
        }
        public static void WerewolfMaul() 
        {
           var nearbyPlayers = Helpers.GetClosestPlayers(Werewolf.Player.GetTruePosition(), Werewolf.Radius);

            foreach (var player in nearbyPlayers)
            {
                if (Werewolf.Player == player || player.Data.IsDead || player == Armored.Player && !Armored.isBrokenArmor || player == Medic.shielded || player == FirstPlayerKilled)
                    continue;
                    
                Helpers.CheckMurderAttemptAndKill(Werewolf.Player, player, showAnimation: false);

                MessageWriter ReasonWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                ReasonWriter.Write(PlayerControl.LocalPlayer.PlayerId);
                ReasonWriter.Write((byte)GhostInfoTypes.DeathReasonAndKiller);
                ReasonWriter.Write(player.PlayerId);
                ReasonWriter.Write((byte)DeadPlayer.CustomDeathReason.Maul);
                ReasonWriter.Write(Werewolf.Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(ReasonWriter);
                OverrideDeathReasonAndKiller(player, DeadPlayer.CustomDeathReason.Maul, killer: Werewolf.Player);
            }
        }

        public static void Disperse()
        {
            Dictionary<byte, Vector2> coordinates = GenerateDisperseCoordinates();

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Disperse, SendOption.Reliable, -1);
            writer.Write((byte)coordinates.Count);
            foreach ((byte key, Vector2 value) in coordinates)
            {
                writer.Write(key);
                writer.Write(value);
            }
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            StartTransportation(coordinates);
            Disperser.Charges--;
        }
        public static void StartTransportation(Dictionary<byte, Vector2> coordinates)
        {
            if (coordinates.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
            {
                Helpers.ShowFlash(Palette.ImpostorRed, 2.5f);
                if (Minigame.Instance)
                {
                    try
                    {
                        Minigame.Instance.Close();
                    }
                    catch
                    {

                    }
                }

                if (PlayerControl.LocalPlayer.inVent)
                {
                    PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                    PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
                }
            }


            foreach ((byte key, Vector2 value) in coordinates)
            {
                PlayerControl player = Helpers.PlayerById(key);
                player.transform.position = value;
                if (PlayerControl.LocalPlayer == player) PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(value);
            }

            if (PlayerControl.LocalPlayer.walkingToVent)
            {
                PlayerControl.LocalPlayer.inVent = false;
                Vent.currentVent = null;
                PlayerControl.LocalPlayer.moveable = true;
                PlayerControl.LocalPlayer.MyPhysics.StopAllCoroutines();
            }

            if (SubmergedCompatibility.IsSubmerged) SubmergedCompatibility.ChangeFloor(PlayerControl.LocalPlayer.transform.position.y > -7f);
        }

        private static Dictionary<byte, Vector2> GenerateDisperseCoordinates()
        {
            List<PlayerControl> targets = PlayerControl.AllPlayerControls.ToArray().Where(player => !player.Data.IsDead && !player.Data.Disconnected).ToList();

            HashSet<Vent> vents = UnityEngine.Object.FindObjectsOfType<Vent>().ToHashSet();

            Dictionary<byte, Vector2> coordinates = new Dictionary<byte, Vector2>(targets.Count);
            foreach (PlayerControl target in targets)
            {
                Vent vent = vents.Random();

                Vector3 destination = SendPlayerToVent(vent);
                coordinates.Add(target.PlayerId, destination);
            }
            return coordinates;
        }

        public static Vector3 SendPlayerToVent(Vent vent)
        {
            Vector2 size = vent.GetComponent<BoxCollider2D>().size;
            Vector3 destination = vent.transform.position;
            destination.y += 0.3636f;
            return destination;
        }

        public static void Fortify(byte fortifiedId)
        {
            Crusader.Fortified = true;
            Crusader.Charges--;
            Crusader.FortifiedPlayer = Helpers.PlayerById(fortifiedId);
        }

        public static void Confess(byte confessorId)
        {
            if (Oracle.Player == null || Oracle.Player.Data.IsDead) return;

            Oracle.Confessor = Helpers.PlayerById(confessorId);
            if (Oracle.Confessor == null) return;

            RoleInfo roleInfo = RoleInfo.GetRoleInfoForPlayer(Oracle.Confessor, false).FirstOrDefault();
            if (roleInfo == null) return;

            bool showsCorrectFaction = UnityEngine.Random.RandomRangeInt(1, 101) <= Oracle.Accuracy;
            Factions revealedFaction;

            if (showsCorrectFaction)
            {
                // Reveal the actual faction
                revealedFaction = roleInfo.FactionId;
            }
            else
            {
                // Get all possible factions
                List<Factions> possibleFactions = new List<Factions> { Factions.Crewmate, Factions.Impostor, Factions.NeutralKiller };

                // Remove the actual faction from the list so we never guess correctly
                possibleFactions.Remove(roleInfo.FactionId);

                // Choose a random incorrect faction
                revealedFaction = possibleFactions[UnityEngine.Random.RandomRangeInt(0, possibleFactions.Count)];
            }

            // Save the revealed faction
            Oracle.RevealedFaction = revealedFaction;

            var results = Oracle.GetInfo(Oracle.Confessor);
            FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(Oracle.Player, $"{results}");

            // Send RPC to notify clients
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Confess, SendOption.Reliable, -1);
            writer.Write(Oracle.Confessor.PlayerId);
            writer.Write((int)revealedFaction);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            // Ghost Info
            MessageWriter GhostInfoWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareGhostInfo, SendOption.Reliable);
            GhostInfoWriter.Write(Oracle.Confessor.PlayerId);
            GhostInfoWriter.Write((byte)GhostInfoTypes.OracleInfo);
            GhostInfoWriter.Write(results);
            AmongUsClient.Instance.FinishRpcImmediately(GhostInfoWriter);
        }


        public static void ShieldedMurderAttempt() 
        {
            if (Medic.shielded == null || Medic.Player == null) return;
            
            bool isShieldedAndShow = Medic.shielded == PlayerControl.LocalPlayer && Medic.showAttemptToShielded;
            isShieldedAndShow = isShieldedAndShow && (Medic.meetingAfterShielding || !Medic.showShieldAfterMeeting);  // Dont show attempt, if shield is not shown yet
            bool isMedicAndShow = Medic.Player == PlayerControl.LocalPlayer && Medic.showAttemptToMedic;

            if (isShieldedAndShow || isMedicAndShow || Helpers.ShouldShowGhostInfo()) Helpers.ShowFlash(Palette.ImpostorRed, Duration: 0.5f, "Failed Murder Attempt on Shielded Player");
        }

        public static void FortifiedMurderAttempt() 
        {
            if (Crusader.FortifiedPlayer == null || Crusader.Player == null) return;

            if (Crusader.FortifiedPlayer == PlayerControl.LocalPlayer || Crusader.Player == PlayerControl.LocalPlayer || Helpers.ShouldShowGhostInfo()) Helpers.ShowFlash(Palette.ImpostorRed, Duration: 0.5f, "Murder Attempt on Fortified Player");
        }

        public static void ShifterShift(byte targetId) 
        {
            PlayerControl oldShifter = Shifter.Player;
            PlayerControl player = Helpers.PlayerById(targetId);
            if (player == null || oldShifter == null) return;

            Shifter.futureShift = null;
            Shifter.ClearAndReload();

            // Suicide (exile) when impostor or impostor variants
            if ((!player.IsCrew()) && !oldShifter.Data.IsDead) 
            {
                oldShifter.Exiled();
                OverrideDeathReasonAndKiller(oldShifter, DeadPlayer.CustomDeathReason.Shift, player);
                if (oldShifter == Lawyer.target && AmongUsClient.Instance.AmHost && Lawyer.Player != null) 
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerChangeRole, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    LawyerChangeRole();
                }
                if (oldShifter == Romantic.beloved && AmongUsClient.Instance.AmHost && Romantic.Player != null) 
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.RomanticChangeRole, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RomanticChangeRole();
                }
                return;
            }
            
            Shifter.ShiftRole(oldShifter, player);

            // Set cooldowns to max for both players
            if (PlayerControl.LocalPlayer == oldShifter || PlayerControl.LocalPlayer == player)
                CustomButton.ResetAllCooldowns();
        }

        public static void SwapperSwap(byte playerId1, byte playerId2) 
        {
            if (MeetingHud.Instance) 
            {
                Swapper.playerId1 = playerId1;
                Swapper.playerId2 = playerId2;
            }
        }

        public static void MorphlingMorph(byte playerId) 
        {
            PlayerControl target = Helpers.PlayerById(playerId);
            if (Morphling.Player == null || target == null) return;

            Morphling.morphTimer = Morphling.Duration;
            Morphling.morphTarget = target;
            if (Camouflager.CamouflageTimer <= 0f)
                Morphling.Player.SetLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
        }

        public static void GlitchMimic(byte playerId) 
        {
            PlayerControl target = Helpers.PlayerById(playerId);
            if (Glitch.Player == null || target == null) return;

            Glitch.MimicTimer = Glitch.MimicDuration;
            Glitch.MimicTarget = target;
            if (Camouflager.CamouflageTimer <= 0f)
                Glitch.Player.SetLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
        }

        public static void HitmanMorph(byte playerId) 
        {
            PlayerControl target = Helpers.PlayerById(playerId);
            if (Hitman.Player == null || target == null) return;

            Hitman.MorphTimer = Hitman.MorphDuration;
            Hitman.MorphTarget = target;
            if (Camouflager.CamouflageTimer <= 0f)
                Hitman.Player.SetLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
        }

        public static void CamouflagerCamouflage() 
        {
            if (Camouflager.Player == null) return;

            Camouflager.CamouflageTimer = Camouflager.Duration;
            if (Helpers.MushroomSabotageActive()) return; // Dont overwrite the fungle "camo"
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                player.SetLook("", 6, "", "", "", "");
        }

        public static void VampireSetBitten(byte targetId, byte performReset) 
        {
            if (performReset != 0) {
                Vampire.bitten = null;
                return;
            }

            if (Vampire.Player == null) return;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls) 
            {
                if (player.PlayerId == targetId && !player.Data.IsDead) {
                        Vampire.bitten = player;
                }
            }
        }

        public static void PlaceGarlic(byte[] buff) 
        {
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0*sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1*sizeof(float));
            new Garlic(position);
        }

        public static void TrackerUsedTracker(byte targetId) 
        {
            Tracker.usedTracker = true;
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == targetId)
                    Tracker.tracked = player;
        }

        public static void GlitchUsedHacks(byte targetId)
        {
            Glitch.remainingHacks--;
            Glitch.HackedPlayers.Add(targetId);
        }

        public static void JackalCreatesSidekick(byte targetId) 
        {
            PlayerControl player = Helpers.PlayerById(targetId);
            if (player == null) return;
            if (Lawyer.target == player && Lawyer.isProsecutor && Lawyer.Player != null && !Lawyer.Player.Data.IsDead) Lawyer.isProsecutor = false;

            if (!Jackal.canCreateSidekickFromImpostor && player.Data.Role.IsImpostor) 
            {
                Jackal.fakeSidekick = player;
            } 
            else 
            {
                bool wasSpy = Spy.Player != null && player == Spy.Player;
                bool wasImpostor = player.Data.Role.IsImpostor;  // This can only be reached if impostors can be sidekicked.
                FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
                if (player == Lawyer.Player && Lawyer.target != null)
                {
                    Transform playerInfoTransform = Lawyer.target.cosmetics.nameText.transform.parent.FindChild("Info");
                    TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                    if (playerInfo != null) playerInfo.text = "";
                }
                ErasePlayerRoles(player.PlayerId, true);
                Sidekick.Player = player;
                if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) PlayerControl.LocalPlayer.moveable = true;
                if (wasSpy || wasImpostor) Sidekick.wasTeamRed = true;
                Sidekick.wasSpy = wasSpy;
                Sidekick.wasImpostor = wasImpostor;
                if (player == PlayerControl.LocalPlayer) SoundEffectsManager.Play("jackalSidekick");
                if (CustomOptionHolder.GuesserSidekickIsAlwaysGuesser.GetBool() && !HandleGuesser.IsGuesser(targetId))
                    SetGuessers(targetId);
            }
            Jackal.canCreateSidekick = false;
        }

        public static void SidekickPromotes() 
        {
            Jackal.removeCurrentJackal();
            Jackal.Player = Sidekick.Player;
            Jackal.canCreateSidekick = Jackal.jackalPromotedFromSidekickCanCreateSidekick;
            Jackal.wasTeamRed = Sidekick.wasTeamRed;
            Jackal.wasSpy = Sidekick.wasSpy;
            Jackal.wasImpostor = Sidekick.wasImpostor;
            Sidekick.ClearAndReload();
            return;
        }
        
        public static void ErasePlayerRoles(byte playerId, bool ignoreModifier = true) 
        {
            PlayerControl player = Helpers.PlayerById(playerId);
            if (player == null || !player.CanBeErased()) return;

            // Crewmate roles
            if (player == Mayor.Player) Mayor.ClearAndReload();
            if (player == Portalmaker.Player) Portalmaker.ClearAndReload();
            if (player == Engineer.Player) Engineer.ClearAndReload();
            if (player == Sheriff.Player) Sheriff.ClearAndReload();
            if (player == Oracle.Player) Oracle.ClearAndReload();
            if (player == Lighter.Player) Lighter.ClearAndReload();
            if (player == Detective.Player) Detective.ClearAndReload();
            if (player == TimeMaster.Player) TimeMaster.ClearAndReload();
            if (player == Veteran.Player) Veteran.ClearAndReload();
            if (player == Medic.Player) Medic.ClearAndReload();
            if (player == Shifter.Player) Shifter.ClearAndReload();
            if (player == Mystic.Player) Mystic.ClearAndReload();
            if (player == Hacker.Player) Hacker.ClearAndReload();
            if (player == Tracker.Player) Tracker.ClearAndReload();
            if (player == Snitch.Player) Snitch.ClearAndReload();
            if (player == Swapper.Player) Swapper.ClearAndReload();
            if (player == Spy.Player) Spy.ClearAndReload();
            if (player == Vigilante.Player) Vigilante.ClearAndReload();
            if (player == Medium.medium) Medium.ClearAndReload();
            if (player == Trapper.Player) Trapper.ClearAndReload();

            // Impostor roles
            if (player == Morphling.Player) Morphling.ClearAndReload();
            if (player == Camouflager.Player) Camouflager.ClearAndReload();
            if (player == Godfather.Player) Godfather.ClearAndReload();
            if (player == Mafioso.Player) Mafioso.ClearAndReload();
            if (player == Janitor.Player) Janitor.ClearAndReload();
            if (player == Vampire.Player) Vampire.ClearAndReload();
            if (player == Eraser.Player) Eraser.ClearAndReload();
            if (player == Trickster.Player) Trickster.ClearAndReload();
            if (player == Undertaker.Player) Undertaker.ClearAndReload();
            if (player == Cleaner.Player) Cleaner.ClearAndReload();
            if (player == Warlock.Player) Warlock.ClearAndReload();
            if (player == Witch.Player) Witch.ClearAndReload();
            if (player == Ninja.ninja) Ninja.ClearAndReload();
            if (player == Yoyo.Player) Yoyo.ClearAndReload();

            // Other roles
            if (player == Jester.Player) Jester.ClearAndReload();
            if (player == Glitch.Player) Glitch.ClearAndReload();
            if (player == Werewolf.Player) Werewolf.ClearAndReload();
            if (player == Plaguebearer.Player) Plaguebearer.ClearAndReload();
            if (player == Pestilence.Player) Pestilence.ClearAndReload();
            if (player == SerialKiller.Player) SerialKiller.ClearAndReload();
            if (player == Arsonist.Player) Arsonist.ClearAndReload();
            if (Guesser.IsGuesser(player.PlayerId)) Guesser.Clear(player.PlayerId);
            if (player == Jackal.Player) 
            { // Promote Sidekick and hence override the the Jackal or erase Jackal
                if (Sidekick.promotesToJackal && Sidekick.Player != null && !Sidekick.Player.Data.IsDead) 
                {
                    SidekickPromotes();
                }
                else 
                {
                    Jackal.ClearAndReload();
                }
            }
            if (player == Sidekick.Player) Sidekick.ClearAndReload();
            if (player == BountyHunter.Player) BountyHunter.ClearAndReload();
            if (player == Vulture.Player) Vulture.ClearAndReload();
            if (player == Lawyer.Player) Lawyer.ClearAndReload();
            if (player == Romantic.Player) Romantic.ClearAndReload();
            if (player == VengefulRomantic.Player) VengefulRomantic.ClearAndReload();
            if (player == Pursuer.Player) Pursuer.ClearAndReload();
            if (player == Thief.Player) Thief.ClearAndReload();

            // Modifier
            if (!ignoreModifier)
            {
                if (player == Lovers.Lover1 || player == Lovers.Lover2) Lovers.ClearAndReload(); // The whole Lover couple is being erased
                if (Bait.Players.Any(x => x.PlayerId == player.PlayerId)) Bait.Players.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (global::TownOfSushi.Bloody.Players.Any(x => x.PlayerId == player.PlayerId)) global::TownOfSushi.Bloody.Players.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (AntiTeleport.Players.Any(x => x.PlayerId == player.PlayerId)) AntiTeleport.Players.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (Sleuth.Players.Any(x => x.PlayerId == player.PlayerId)) Sleuth.Players.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (Sunglasses.Players.Any(x => x.PlayerId == player.PlayerId)) Sunglasses.Players.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (player == Tiebreaker.Player) Tiebreaker.ClearAndReload();
                if (player == Mini.Player) Mini.ClearAndReload();
                if (Vip.Players.Any(x => x.PlayerId == player.PlayerId)) Vip.Players.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (Invert.Players.Any(x => x.PlayerId == player.PlayerId)) Invert.Players.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (Chameleon.Players.Any(x => x.PlayerId == player.PlayerId)) Chameleon.Players.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (player == Armored.Player) Armored.ClearAndReload();
            }
        }

        public static void SetFutureErased(byte playerId) 
        {
            PlayerControl player = Helpers.PlayerById(playerId);
            if (Eraser.futureErased == null) 
                Eraser.futureErased = new List<PlayerControl>();
            if (player != null) {
                Eraser.futureErased.Add(player);
            }
        }

        public static void SetFutureShifted(byte playerId) 
        {
            Shifter.futureShift = Helpers.PlayerById(playerId);
        }

        public static void SetFutureShielded(byte playerId) 
        {
            Medic.futureShielded = Helpers.PlayerById(playerId);
            Medic.usedShield = true;
        }

        public static void SetFutureSpelled(byte playerId) 
        {
            PlayerControl player = Helpers.PlayerById(playerId);
            if (Witch.futureSpelled == null)
                Witch.futureSpelled = new List<PlayerControl>();
            if (player != null) {
                Witch.futureSpelled.Add(player);
            }
        }

        public static void PlaceNinjaTrace(byte[] buff) 
        {
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            new NinjaTrace(position, Ninja.traceTime);
            if (PlayerControl.LocalPlayer != Ninja.ninja)
                Ninja.ninjaMarked = null;
        }

        public static void SetInvisible(byte playerId, byte flag)
        {
            PlayerControl target = Helpers.PlayerById(playerId);
            if (target == null) return;
            if (flag == byte.MaxValue)
            {
                target.cosmetics.currentBodySprite.BodySprite.color = Color.white;
                target.cosmetics.colorBlindText.gameObject.SetActive(DataManager.Settings.Accessibility.ColorBlindMode);
                target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(1f);

                if (Camouflager.CamouflageTimer <= 0 && !Helpers.MushroomSabotageActive()) target.SetDefaultLook();
                Ninja.isInvisble = false;
                return;
            }

            target.SetLook("", 6, "", "", "", "");
            Color color = Color.clear;
            bool canSee = PlayerControl.LocalPlayer.Data.Role.IsImpostor || PlayerControl.LocalPlayer.Data.IsDead;
            if (canSee) color.a = 0.1f;
            target.cosmetics.currentBodySprite.BodySprite.color = color;
            target.cosmetics.colorBlindText.gameObject.SetActive(false);
            target.cosmetics.colorBlindText.color = target.cosmetics.colorBlindText.color.SetAlpha(canSee ? 0.1f : 0f);
            Ninja.invisibleTimer = Ninja.invisibleDuration;
            Ninja.isInvisble = true;
        }

        public static void PlacePortal(byte[] buff) 
        {
            Vector3 position = Vector2.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            new Portal(position);
        }

        public static void UsePortal(byte playerId, byte exit) 
        {
            Portal.StartTeleport(playerId, exit);
        }

        public static void PlaceJackInTheBox(byte[] buff) 
        {
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0*sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1*sizeof(float));
            new JackInTheBox(position);
        }

        public static void LightsOut() 
        {
            Trickster.lightsOutTimer = Trickster.lightsOutDuration;
            // If the local player is impostor indicate lights out
            if(Helpers.HasImpVision(GameData.Instance.GetPlayerById(PlayerControl.LocalPlayer.PlayerId))) {
                new CustomMessage("Lights are out", Trickster.lightsOutDuration);
            }
        }

        public static void PlaceCamera(byte[] buff) 
        {
            var referenceCamera = UnityEngine.Object.FindObjectOfType<SurvCamera>(); 
            if (referenceCamera == null) return; // Mira HQ

            Vigilante.remainingScrews -= Vigilante.camPrice;
            Vigilante.placedCameras++;

            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0*sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1*sizeof(float));

            var camera = UnityEngine.Object.Instantiate<SurvCamera>(referenceCamera);
            camera.transform.position = new Vector3(position.x, position.y, referenceCamera.transform.position.z - 1f);
            camera.CamName = $"Security Camera {Vigilante.placedCameras}";
            camera.Offset = new Vector3(0f, 0f, camera.Offset.z);
            if (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 2 || GameOptionsManager.Instance.currentNormalGameOptions.MapId == 4) camera.transform.localRotation = new Quaternion(0, 0, 1, 1); // Polus and Airship 

            if (SubmergedCompatibility.IsSubmerged) 
            {
                // remove 2d box collider of console, so that no barrier can be created. (irrelevant for now, but who knows... maybe we need it later)
                var fixConsole = camera.transform.FindChild("FixConsole");
                if (fixConsole != null) {
                    var boxCollider = fixConsole.GetComponent<BoxCollider2D>();
                    if (boxCollider != null) UnityEngine.Object.Destroy(boxCollider);
                }
            }


            if (PlayerControl.LocalPlayer == Vigilante.Player) 
            {
                camera.gameObject.SetActive(true);
                camera.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
            } else {
                camera.gameObject.SetActive(false);
            }
            MapOptions.CamsToAdd.Add(camera);
        }

        public static void SealVent(int ventId) 
        {
            Vent vent = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault((x) => x != null && x.Id == ventId);
            if (vent == null) return;

            Vigilante.remainingScrews -= Vigilante.ventPrice;
            if (PlayerControl.LocalPlayer == Vigilante.Player) {
                PowerTools.SpriteAnim animator = vent.GetComponent<PowerTools.SpriteAnim>(); 
                
                vent.EnterVentAnim = vent.ExitVentAnim = null;
                Sprite newSprite = animator == null ? Vigilante.GetStaticVentSealedSprite() : Vigilante.getAnimatedVentSealedSprite();
                SpriteRenderer rend = vent.myRend;
                if (Helpers.IsFungle()) {
                    newSprite = Vigilante.GetFungleVentSealedSprite();
                    rend = vent.transform.GetChild(3).GetComponent<SpriteRenderer>();
                    animator = vent.transform.GetChild(3).GetComponent<PowerTools.SpriteAnim>();
                }
                animator?.Stop();
                rend.sprite = newSprite;
                if (SubmergedCompatibility.IsSubmerged && vent.Id == 0) vent.myRend.sprite = Vigilante.GetSubmergedCentralUpperSealedSprite();
                if (SubmergedCompatibility.IsSubmerged && vent.Id == 14) vent.myRend.sprite = Vigilante.GetSubmergedCentralLowerSealedSprite();
                rend.color = new Color(1f, 1f, 1f, 0.5f);
                vent.name = "FutureSealedVent_" + vent.name;
            }

            MapOptions.VentsToSeal.Add(vent);
        }

        public static void ArsonistWin() 
        {
            Arsonist.IsArsonistWin = true;
            foreach (PlayerControl p in PlayerControl.AllPlayerControls) 
            {
                if (p != Arsonist.Player && !p.Data.IsDead) 
                {
                    p.Exiled();
                    OverrideDeathReasonAndKiller(p, DeadPlayer.CustomDeathReason.Arson, Arsonist.Player);
                }
            }
        }

        public static void LawyerSetTarget(byte playerId) 
        {
            Lawyer.target = Helpers.PlayerById(playerId);
        }

        public static void LawyerChangeRole() 
        {
            PlayerControl player = Lawyer.Player;
            PlayerControl client = Lawyer.target;
            Lawyer.ClearAndReload(false);

            Pursuer.Player = player;

            if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId && client != null) 
            {
                    Transform playerInfoTransform = client.cosmetics.nameText.transform.parent.FindChild("Info");
                    TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                    if (playerInfo != null) playerInfo.text = "";
            }
        }

        public static void PlaguebearerTurnPestilence() 
        {
            PlayerControl player = Plaguebearer.Player;
            Plaguebearer.ClearAndReload();

            Pestilence.Player = player;
            if (player == PlayerControl.LocalPlayer)
            {
                Helpers.ShowFlash(Pestilence.Color, 2.5f);
                SoundManager.Instance.PlaySound(ShipStatus.Instance.SabotageSound, false, 1f, null);
                Helpers.ShowTextToast("You just transformed into the Pestilence!", 2.5f);
            }
        }

        public static void AgentTurnIntoHitman() 
        {
            PlayerControl player = Agent.Player;
            Agent.ClearAndReload();

            Hitman.Player = player;
            if (player == PlayerControl.LocalPlayer)
            {
                Helpers.ShowFlash(Hitman.Color, 2.5f);
                SoundManager.Instance.PlaySound(ShipStatus.Instance.SabotageSound, false, 1f, null);
                Helpers.ShowTextToast("You just became the Hitman!", 2.5f);
            }
        }

        public static void DragBody(byte BodyId)
        {
            DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            for (int i = 0; i < array.Length; i++)
            {
                if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == BodyId)
                {
                    Undertaker.CurrentTarget = array[i];
                }
            }
        }

        public static void DropBody(byte bodyId)
        {
            if (Undertaker.Player == null || Undertaker.CurrentTarget == null) return;
            Undertaker.CurrentTarget = null;
            Undertaker.CurrentTarget.transform.position = new Vector3(Undertaker.Player.GetTruePosition().x, Undertaker.Player.GetTruePosition().y, Undertaker.Player.transform.position.z);
        }

        public static void HitmanDragBody(byte BodyId)
        {
            DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            for (int i = 0; i < array.Length; i++)
            {
                if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == BodyId)
                {
                    Hitman.BodyTarget = array[i];
                }
            }
        }

        public static void HitmanDropBody(byte bodyId)
        {
            if (Hitman.Player == null || Hitman.BodyTarget == null) return;
            Hitman.BodyTarget = null;
            Hitman.BodyTarget.transform.position = new Vector3(Hitman.Player.GetTruePosition().x, Hitman.Player.GetTruePosition().y, Hitman.Player.transform.position.z);
        }


        public static void RomanticChangeRole() 
        {
            PlayerControl player = Romantic.Player;
            PlayerControl target = Romantic.beloved;
            Romantic.ClearAndReload(false);

            VengefulRomantic.Player = player;
            VengefulRomantic.Lover = target;

            if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId && target != null) 
            {
                    Transform playerInfoTransform = target.cosmetics.nameText.transform.parent.FindChild("Info");
                    TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                    if (playerInfo != null) playerInfo.text = "";
            }
        }
        public static void GuesserShoot(byte killerId, byte dyingTargetId, byte guessedTargetId, byte guessedRoleId) 
        {
            PlayerControl dyingTarget = Helpers.PlayerById(dyingTargetId);
            if (dyingTarget == null ) return;
            if (Lawyer.target != null && dyingTarget == Lawyer.target) Lawyer.targetWasGuessed = true;  // Lawyer shouldn't be exiled with the client for guesses
            PlayerControl dyingLoverPartner = Lovers.bothDie ? dyingTarget.GetPartner() : null; // Lover check
            if (Lawyer.target != null && dyingLoverPartner == Lawyer.target) Lawyer.targetWasGuessed = true;  // Lawyer shouldn't be exiled with the client for guesses

            PlayerControl guesser = Helpers.PlayerById(killerId);
            dyingTarget.Exiled();
            GameHistory.OverrideDeathReasonAndKiller(dyingTarget, DeadPlayer.CustomDeathReason.Guess, guesser);
            byte partnerId = dyingLoverPartner != null ? dyingLoverPartner.PlayerId : dyingTargetId;

            HandleGuesser.RemainingShots(killerId, true);
            if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(dyingTarget.KillSfx, false, 0.8f);
            if (MeetingHud.Instance) 
            {
                foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates) 
                {
                    if (pva.TargetPlayerId == dyingTargetId || pva.TargetPlayerId == partnerId) 
                    {
                        pva.SetDead(pva.DidReport, true);
                        pva.Overlay.gameObject.SetActive(true);
                        MeetingHudPatch.SwapperCheckAndReturnSwap(MeetingHud.Instance, pva.TargetPlayerId);
                    }

                    //Give players back their vote if target is shot dead
                    if (pva.VotedFor != dyingTargetId && pva.VotedFor != partnerId) continue;
                    pva.UnsetVote();
                    var voteAreaPlayer = Helpers.PlayerById(pva.TargetPlayerId);
                    if (!voteAreaPlayer.AmOwner) continue;
                    MeetingHud.Instance.ClearVote();

                }
                if (AmongUsClient.Instance.AmHost) 
                    MeetingHud.Instance.CheckForEndVoting();
            }
            if (FastDestroyableSingleton<HudManager>.Instance != null && guesser != null)
                if (PlayerControl.LocalPlayer == dyingTarget) 
                {
                    FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(guesser.Data, dyingTarget.Data);
                    if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                } 
                else if (dyingLoverPartner != null && PlayerControl.LocalPlayer == dyingLoverPartner) 
                {
                    FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(dyingLoverPartner.Data, dyingLoverPartner.Data);
                    if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                }

            // remove shoot button from targets for all guessers and close their guesserUI
            if (Guesser.IsGuesser(PlayerControl.LocalPlayer.PlayerId) && PlayerControl.LocalPlayer != guesser && !PlayerControl.LocalPlayer.Data.IsDead && Guesser.RemainingShots(PlayerControl.LocalPlayer.PlayerId) > 0 && MeetingHud.Instance)
            {
                MeetingHud.Instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == dyingTarget.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                if (dyingLoverPartner != null)
                    MeetingHud.Instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == dyingLoverPartner.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });

                if (MeetingHudPatch.guesserUI != null && MeetingHudPatch.guesserUIExitButton != null) 
                {
                    if (MeetingHudPatch.guesserCurrentTarget == dyingTarget.PlayerId)
                        MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                    else if (dyingLoverPartner != null && MeetingHudPatch.guesserCurrentTarget == dyingLoverPartner.PlayerId)
                        MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                }
            }

            PlayerControl guessedTarget = Helpers.PlayerById(guessedTargetId);
            if (PlayerControl.LocalPlayer.Data.IsDead && guessedTarget != null && guesser != null) 
            {
                RoleInfo roleInfo = RoleInfo.allRoleInfos.FirstOrDefault(x => (byte)x.RoleId == guessedRoleId);
                string msg = $"{guesser.Data.PlayerName} guessed the role {roleInfo?.Name ?? ""} for {guessedTarget.Data.PlayerName}!";
                if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
                    FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(guesser, msg);
                if (msg.IndexOf("who", StringComparison.OrdinalIgnoreCase) >= 0)
                    FastDestroyableSingleton<UnityTelemetry>.Instance.SendWho();
            }
        }

        public static void SetBlanked(byte playerId, byte value) 
        {
            PlayerControl target = Helpers.PlayerById(playerId);
            if (target == null) return;
            Pursuer.blankedList.RemoveAll(x => x.PlayerId == playerId);
            if (value > 0) Pursuer.blankedList.Add(target);            
        }

        public static void Bloody(byte killerPlayerId, byte bloodyPlayerId) 
        {
            if (global::TownOfSushi.Bloody.active.ContainsKey(killerPlayerId)) return;
            global::TownOfSushi.Bloody.active.Add(killerPlayerId, global::TownOfSushi.Bloody.Duration);
            global::TownOfSushi.Bloody.bloodyKillerMap.Add(killerPlayerId, bloodyPlayerId);
        }

        public static void SetFirstKill(byte playerId) 
        {
            PlayerControl target = Helpers.PlayerById(playerId);
            if (target == null) return;
            MapOptions.FirstPlayerKilled = target;
        }

        public static void SetTiebreak() 
        {
            Tiebreaker.isTiebreak = true;
        }
        public static void AmnesiacRemember(byte targetId)
        {
            PlayerControl target = Helpers.PlayerById(targetId);
            PlayerControl AmnesiacPlayer = Amnesiac.Player;
            if (target == null || AmnesiacPlayer == null) return;
            List<RoleInfo> targetInfo = RoleInfo.GetRoleInfoForPlayer(target);
            RoleInfo roleInfo = targetInfo.Where(info => info.FactionId != Factions.Modifier).FirstOrDefault();
            switch (roleInfo.RoleId)
            {
                case RoleId.Crewmate:
                    Amnesiac.ClearAndReload();
                    break;
                case RoleId.Impostor:
                    Helpers.BecomeImpostor(Amnesiac.Player);
                    Amnesiac.ClearAndReload();
                    break;
                case RoleId.Jester:
                    Jester.ClearAndReload();
                    Jester.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    Amnesiac.Player = target;
                    break;

                case RoleId.Werewolf:
                    Werewolf.ClearAndReload();
                    Werewolf.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    Amnesiac.Player = target;
                    break;

                case RoleId.Prosecutor:
                    Lawyer.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    Amnesiac.Player = target;
                    break;

                case RoleId.Mayor:
                    Mayor.ClearAndReload();
                    Mayor.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Portalmaker:
                    Portalmaker.ClearAndReload();
                    Portalmaker.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;
                
                case RoleId.Crusader:
                    Crusader.ClearAndReload();
                    Crusader.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Engineer:
                    Engineer.ClearAndReload();
                    Engineer.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Sheriff:
                    Sheriff.ClearAndReload();
                    Sheriff.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Lighter:
                    Lighter.ClearAndReload();
                    Lighter.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Godfather:
                    Helpers.BecomeImpostor(Amnesiac.Player);
                    Godfather.ClearAndReload();
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Mafioso:
                    Helpers.BecomeImpostor(Amnesiac.Player);
                    Mafioso.ClearAndReload();
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Janitor:
                    Helpers.BecomeImpostor(Amnesiac.Player);
                    Janitor.ClearAndReload();
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Detective:
                    Detective.ClearAndReload();
                    Detective.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.TimeMaster:
                    TimeMaster.ClearAndReload();
                    TimeMaster.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Veteran:
                    Veteran.ClearAndReload();
                    Veteran.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Medic:
                    Medic.ClearAndReload();
                    Medic.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Shifter:
                    Shifter.ClearAndReload();
                    Shifter.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Swapper:
                    Swapper.ClearAndReload();
                    Swapper.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Mystic:
                    Mystic.ClearAndReload();
                    Mystic.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Morphling:
                    Helpers.BecomeImpostor(Amnesiac.Player);
                    Morphling.ClearAndReload();
                    Morphling.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Yoyo:
                    Helpers.BecomeImpostor(Amnesiac.Player);
                    Yoyo.ClearAndReload();
                    Yoyo.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Camouflager:
                    Helpers.BecomeImpostor(Amnesiac.Player);
                    Camouflager.ClearAndReload();
                    Camouflager.Player = AmnesiacPlayer;
                   Amnesiac.ClearAndReload();
                    break;

                case RoleId.Hacker:
                    Hacker.ClearAndReload();
                    Hacker.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Tracker:
                    Tracker.ClearAndReload();
                    Tracker.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Vampire:
                    Helpers.BecomeImpostor(Amnesiac.Player);
                    Vampire.ClearAndReload();
                    Vampire.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Snitch:
                    Snitch.ClearAndReload();
                    Snitch.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;
                
                case RoleId.Glitch:
                    Glitch.ClearAndReload();
                    Glitch.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;
                
                case RoleId.SerialKiller:
                    SerialKiller.ClearAndReload();
                    SerialKiller.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;
                
                case RoleId.Juggernaut:
                    Juggernaut.ClearAndReload();
                    Juggernaut.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;
                
                case RoleId.Oracle:
                    Oracle.ClearAndReload();
                    Oracle.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;
                
                case RoleId.Agent:
                    Agent.ClearAndReload();
                    Agent.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;
                
                 case RoleId.Hitman:
                    Hitman.ClearAndReload();
                    Hitman.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;
                
                case RoleId.Undertaker:
                    Helpers.BecomeImpostor(Amnesiac.Player);
                    Undertaker.ClearAndReload();
                    Undertaker.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;
                
                case RoleId.Romantic:
                    Romantic.ClearAndReload();
                    Romantic.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Jackal:
                    Jackal.Player = AmnesiacPlayer;
                    Jackal.formerJackals.Add(target);
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Sidekick:
                    Jackal.formerJackals.Add(target);
                    Sidekick.ClearAndReload();
                    Sidekick.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Eraser:
                    Helpers.BecomeImpostor(Amnesiac.Player);
                    Eraser.ClearAndReload();
                    Eraser.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Spy:
                    Spy.ClearAndReload();
                    Spy.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Trickster:
                    Helpers.BecomeImpostor(Amnesiac.Player);
                    Trickster.ClearAndReload();
                    Trickster.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Cleaner:
                    Helpers.BecomeImpostor(Amnesiac.Player);
                    Cleaner.ClearAndReload();
                    Cleaner.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Warlock:
                    Helpers.BecomeImpostor(Amnesiac.Player);
                    Warlock.ClearAndReload();
                    Warlock.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Vigilante:
                    Vigilante.ClearAndReload();
                    Vigilante.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;
                
                case RoleId.Plaguebearer:
                    Plaguebearer.ClearAndReload();
                    Plaguebearer.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    Amnesiac.Player = target;
                    break;
                
                case RoleId.Pestilence:
                    Pestilence.ClearAndReload();
                    Pestilence.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Arsonist:
                    Arsonist.ClearAndReload();
                    Arsonist.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    Amnesiac.Player = target;

                    if (PlayerControl.LocalPlayer == Arsonist.Player)
                    {
                        int playerCounter = 0;
                        Vector3 bottomLeft = new(-FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.x, FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.y, FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.z);
                        foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                        {
                            if (BeanIcons.ContainsKey(p.PlayerId) && p != Arsonist.Player)
                            {
                                //Arsonist.poolIcons.Add(p);
                                if (Arsonist.dousedPlayers.Contains(p))
                                {
                                    BeanIcons[p.PlayerId].SetSemiTransparent(false);
                                }
                                else
                                {
                                    BeanIcons[p.PlayerId].SetSemiTransparent(true);
                                }

                                BeanIcons[p.PlayerId].transform.localPosition = bottomLeft + new Vector3(-0.25f, -0.25f, 0) + (Vector3.right * playerCounter++ * 0.35f);
                                BeanIcons[p.PlayerId].transform.localScale = Vector3.one * 0.2f;
                                BeanIcons[p.PlayerId].gameObject.SetActive(true);
                            }
                        }
                    }
                    break;

                case RoleId.BountyHunter:
                    Helpers.BecomeImpostor(Amnesiac.Player);
                    BountyHunter.ClearAndReload();
                    BountyHunter.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();

                    BountyHunter.bountyUpdateTimer = 0f;
                    if (PlayerControl.LocalPlayer == BountyHunter.Player)
                    {
                        Vector3 bottomLeft = new Vector3(-FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.x, FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.y, FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition.z) + new Vector3(-0.25f, 1f, 0);
                        BountyHunter.cooldownText = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
                        BountyHunter.cooldownText.alignment = TMPro.TextAlignmentOptions.Center;
                        BountyHunter.cooldownText.transform.localPosition = bottomLeft + new Vector3(0f, -1f, -1f);
                        BountyHunter.cooldownText.gameObject.SetActive(true);

                        foreach (PlayerControl p in PlayerControl.AllPlayerControls)
                        {
                            if (BeanIcons.ContainsKey(p.PlayerId))
                            {
                                BeanIcons[p.PlayerId].SetSemiTransparent(false);
                                BeanIcons[p.PlayerId].transform.localPosition = bottomLeft + new Vector3(0f, -1f, 0);
                                BeanIcons[p.PlayerId].transform.localScale = Vector3.one * 0.4f;
                                BeanIcons[p.PlayerId].gameObject.SetActive(false);
                            }
                        }
                    }
                    break;

                case RoleId.Vulture:
                    Vulture.ClearAndReload();
                    Vulture.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    Amnesiac.Player = target;
                    break;

                case RoleId.Medium:
                    Medium.ClearAndReload();
                    Medium.medium = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Lawyer:
                    Lawyer.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    Amnesiac.Player = target;
                    break;

                case RoleId.Pursuer:
                    Pursuer.ClearAndReload();
                    Pursuer.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    Amnesiac.Player = target;
                    break;

                case RoleId.Witch:
                    Helpers.BecomeImpostor(Amnesiac.Player);
                    Witch.ClearAndReload();
                    Witch.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Thief:
                    Thief.ClearAndReload();
                    Thief.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    Amnesiac.Player = target;
                    break;

                case RoleId.Trapper:
                    Trapper.ClearAndReload();
                    Trapper.Player = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;

                case RoleId.Ninja:
                    Helpers.BecomeImpostor(Amnesiac.Player);
                    Ninja.ClearAndReload();
                    Ninja.ninja = AmnesiacPlayer;
                    Amnesiac.ClearAndReload();
                    break;
            }
        }

        public static void ThiefStealsRole(byte playerId) 
        {
            PlayerControl target = Helpers.PlayerById(playerId);
            PlayerControl thief = Thief.Player;
            if (target == null) return;
            if (target == Sheriff.Player) Sheriff.Player = thief;
            if (target == Glitch.Player) Glitch.Player = thief;
            if (target == SerialKiller.Player) SerialKiller.Player = thief;
            if (target == VengefulRomantic.Player) VengefulRomantic.Player = thief;
            if (target == Werewolf.Player) Werewolf.Player = thief;
            if (target == Plaguebearer.Player) Plaguebearer.Player = thief;
            if (target == Pestilence.Player) Pestilence.Player = thief;
            if (target == Jackal.Player)
            {
                Jackal.Player = thief;
                Jackal.formerJackals.Add(target);
            }
            if (target == Sidekick.Player) 
            {
                Sidekick.Player = thief;
                Jackal.formerJackals.Add(target);
                if (CustomOptionHolder.GuesserSidekickIsAlwaysGuesser.GetBool() && !HandleGuesser.IsGuesser(thief.PlayerId))
                    SetGuessers(thief.PlayerId);
            }
            if (target == Godfather.Player) Godfather.Player = thief;
            if (target == Mafioso.Player) Mafioso.Player = thief;
            if (target == Janitor.Player) Janitor.Player = thief;
            if (target == Morphling.Player) Morphling.Player = thief;
            if (target == Undertaker.Player) Undertaker.Player = thief;
            if (target == Camouflager.Player) Camouflager.Player = thief;
            if (target == Vampire.Player) Vampire.Player = thief;
            if (target == Eraser.Player) Eraser.Player = thief;
            if (target == Trickster.Player) Trickster.Player = thief;
            if (target == Cleaner.Player) Cleaner.Player = thief;
            if (target == Warlock.Player) Warlock.Player = thief;
            if (target == BountyHunter.Player) BountyHunter.Player = thief;
            if (target == Witch.Player) 
            {
                Witch.Player = thief;
                if (MeetingHud.Instance) 
                    if (Witch.witchVoteSavesTargets)  // In a meeting, if the thief guesses the witch, all targets are saved or no target is saved.
                        Witch.futureSpelled = new();
                else  // If thief kills witch during the round, remove the thief from the list of spelled people, keep the rest
                    Witch.futureSpelled.RemoveAll(x => x.PlayerId == thief.PlayerId);
            }
            if (target == Ninja.ninja) Ninja.ninja = thief;
            if (target == Yoyo.Player) {
                Yoyo.Player = thief;
                Yoyo.markedLocation = null;
            }
            if (target.Data.Role.IsImpostor) 
            {
                RoleManager.Instance.SetRole(Thief.Player, RoleTypes.Impostor);
                FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(Thief.Player.killTimer, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            }
            if (Lawyer.Player != null && target == Lawyer.target)
                Lawyer.target = thief;
            if (Thief.Player == PlayerControl.LocalPlayer) CustomButton.ResetAllCooldowns();
            Thief.ClearAndReload();
            Thief.formerThief = thief;  // After ClearAndReload, else it would get reset...
        }
        
        public static void SetTrap(byte[] buff) 
        {
            if (Trapper.Player == null) return;
            Trapper.charges -= 1;
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            new Trap(position);
        }

        public static void TriggerTrap(byte playerId, byte trapId) 
        {
            Trap.TriggerTrap(playerId, trapId);
        }

        public static void SetGuessers(byte playerId)
        {
            PlayerControl target = Helpers.PlayerById(playerId);
            if (target == null) return;
            new Guesser(target);
        }

        public static void ReceiveGhostInfo (byte senderId, MessageReader reader)
        {
            PlayerControl sender = Helpers.PlayerById(senderId);

            GhostInfoTypes infoType = (GhostInfoTypes)reader.ReadByte();
            switch (infoType) {
                case GhostInfoTypes.HackNoticed:
                    Glitch.SetHackedKnows(true, senderId);
                    break;
                case GhostInfoTypes.HackOver:
                    _ = Glitch.HackedKnows.Remove(senderId);
                    break;
                case GhostInfoTypes.ArsonistDouse:
                    Arsonist.dousedPlayers.Add(Helpers.PlayerById(reader.ReadByte()));
                    break;
                case GhostInfoTypes.PlaguebearerInfect:
                    Plaguebearer.InfectedPlayers.Add(Helpers.PlayerById(reader.ReadByte()).PlayerId);
                    break;
                case GhostInfoTypes.BountyTarget:
                    BountyHunter.bounty = Helpers.PlayerById(reader.ReadByte());
                    break;
                case GhostInfoTypes.NinjaMarked:
                    Ninja.ninjaMarked = Helpers.PlayerById(reader.ReadByte());
                    break;
                case GhostInfoTypes.WarlockTarget:
                    Warlock.curseVictim = Helpers.PlayerById(reader.ReadByte());
                    break;
                case GhostInfoTypes.MediumInfo:
                    string mediumInfo = reader.ReadString();
		             if (Helpers.ShouldShowGhostInfo())
                    	FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(sender, mediumInfo);
                    break;
                case GhostInfoTypes.MysticInfo:
                    string mysticInfo = reader.ReadString();
		             if (Helpers.ShouldShowGhostInfo())
                    	FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(sender, mysticInfo);
                    break;
                case GhostInfoTypes.OracleInfo:
                    string oracleInfo = reader.ReadString();
		             if (Helpers.ShouldShowGhostInfo())
                    	FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(sender, oracleInfo);
                    break;
                case GhostInfoTypes.DetectiveOrMedicInfo:
                    string detectiveInfo = reader.ReadString();
                    if (Helpers.ShouldShowGhostInfo())
		    	        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(sender, detectiveInfo);
                    break;
                case GhostInfoTypes.BlankUsed:
                    Pursuer.blankedList.Remove(sender);
                    break;
                case GhostInfoTypes.VampireTimer:
                    vampireKillButton.Timer = (float)reader.ReadByte();
                    break;
                case GhostInfoTypes.DeathReasonAndKiller:
                    OverrideDeathReasonAndKiller(Helpers.PlayerById(reader.ReadByte()), (DeadPlayer.CustomDeathReason)reader.ReadByte(), Helpers.PlayerById(reader.ReadByte()));
                    break;
            }
        }

        public static void ShareRoom(byte playerId, byte roomId) 
        {
            if (Snitch.playerRoomMap.ContainsKey(playerId)) Snitch.playerRoomMap[playerId] = roomId;
            else Snitch.playerRoomMap.Add(playerId, roomId);
        }

        public static void YoyoMarkLocation(byte[] buff) 
        {
            if (Yoyo.Player == null) return;
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            Yoyo.MarkLocation(position);
            new Silhouette(position, -1, false);
        }

        public static void YoyoBlink(bool isFirstJump, byte[] buff) 
        {
            if (Yoyo.Player == null || Yoyo.markedLocation == null) return;
            var markedPos = (Vector3)Yoyo.markedLocation;
            Yoyo.Player.NetTransform.SnapTo(markedPos);

            var markedSilhouette = Silhouette.silhouettes.FirstOrDefault(s => s.gameObject.transform.position.x == markedPos.x && s.gameObject.transform.position.y == markedPos.y);
            if (markedSilhouette != null)
                markedSilhouette.permanent = false;

            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            // Create Silhoutte At Start Position:
            if (isFirstJump) {
                Yoyo.MarkLocation(position);
                new Silhouette(position, Yoyo.blinkDuration, true);
            } else {
                new Silhouette(position, 5, true);
                Yoyo.markedLocation = null;
            }
            if (Chameleon.Players.Any(x => x.PlayerId == Yoyo.Player.PlayerId)) // Make the Yoyo visible if chameleon!
                Chameleon.lastMoved[Yoyo.Player.PlayerId] = Time.time;            
        }

        public static void BreakArmor() 
        {
            if (Armored.Player == null || Armored.isBrokenArmor) return;
            Armored.isBrokenArmor = true;
            if (PlayerControl.LocalPlayer.Data.IsDead) 
            {
                Armored.Player.ShowFailedMurder();
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    class RPCHandlerPatch
    {
        static void Postfix([HarmonyArgument(0)]byte callId, [HarmonyArgument(1)]MessageReader reader)
        {
            byte packetId = callId;
            switch (packetId) 
            {

                // Main Controls

                case (byte)CustomRPC.ResetVaribles:
                    RPCProcedure.ResetVariables();
                    break;
                case (byte)CustomRPC.ShareOptions:
                    RPCProcedure.HandleShareOptions(reader.ReadByte(), reader);
                    break;
                case (byte)CustomRPC.ForceEnd:
                    RPCProcedure.ForceEnd();
                    break; 
                case (byte)CustomRPC.WorkaroundSetRoles:
                    RPCProcedure.WorkaroundSetRoles(reader.ReadByte(), reader);
                    break;
                case (byte)CustomRPC.SetRole:
                    byte roleId = reader.ReadByte();
                    byte playerId = reader.ReadByte();
                    RPCProcedure.SetRole(roleId, playerId);
                    break;
               /* case (byte)CustomRPC.DraftModePickOrder:
                    Modules.RoleDraft.ReceivePickOrder(reader.ReadByte(), reader);
                    break;
                case (byte)CustomRPC.DraftModePick:
                    Modules.RoleDraft.ReceivePick(reader.ReadByte(), reader.ReadByte());
                    break;*/
                case (byte)CustomRPC.SetModifier:
                    byte modifierId = reader.ReadByte();
                    byte pId = reader.ReadByte();
                    byte flag = reader.ReadByte();
                    RPCProcedure.SetModifier(modifierId, pId, flag);
                    break;
                case (byte)CustomRPC.VersionHandshake:
                    byte major = reader.ReadByte();
                    byte minor = reader.ReadByte();
                    byte patch = reader.ReadByte();
                    float timer = reader.ReadSingle();
                    if (!AmongUsClient.Instance.AmHost && timer >= 0f) GameStartManagerPatch.timer = timer;
                    int versionOwnerId = reader.ReadPackedInt32();
                    byte revision = 0xFF;
                    Guid guid;
                    if (reader.Length - reader.Position >= 17) { // enough bytes left to read
                        revision = reader.ReadByte();
                        // GUID
                        byte[] gbytes = reader.ReadBytes(16);
                        guid = new Guid(gbytes);
                    } else {
                        guid = new Guid(new byte[16]);
                    }
                    RPCProcedure.VersionHandshake(major, minor, patch, revision == 0xFF ? -1 : revision, guid, versionOwnerId);
                    break;
                case (byte)CustomRPC.UseUncheckedVent:
                    int ventId = reader.ReadPackedInt32();
                    byte ventingPlayer = reader.ReadByte();
                    byte isEnter = reader.ReadByte();
                    RPCProcedure.UseUncheckedVent(ventId, ventingPlayer, isEnter);
                    break;
                case (byte)CustomRPC.UncheckedMurderPlayer:
                    byte source = reader.ReadByte();
                    byte target = reader.ReadByte();
                    byte showAnimation = reader.ReadByte();
                    RPCProcedure.UncheckedMurderPlayer(source, target, showAnimation);
                    break;
                case (byte)CustomRPC.UncheckedExilePlayer:
                    byte exileTarget = reader.ReadByte();
                    RPCProcedure.UncheckedExilePlayer(exileTarget);
                    break;
                case (byte)CustomRPC.UncheckedCmdReportDeadBody:
                    byte reportSource = reader.ReadByte();
                    byte reportTarget = reader.ReadByte();
                    RPCProcedure.UncheckedCmdReportDeadBody(reportSource, reportTarget);
                    break;
                case (byte)CustomRPC.DragBody:
                    RPCProcedure.DragBody(reader.ReadByte());
                    break;
                case (byte)CustomRPC.DropBody:
                    RPCProcedure.DropBody(reader.ReadByte());
                    break;
                case (byte)CustomRPC.DynamicMapOption:
                    byte mapId = reader.ReadByte();
                    RPCProcedure.DynamicMapOption(mapId);
                    break;
                case (byte)CustomRPC.SetGameStarting:
                    RPCProcedure.SetGameStarting();
                    break;

                // Role functionality

                case (byte)CustomRPC.EngineerFixLights:
                    RPCProcedure.EngineerFixLights();
                    break;
                case (byte)CustomRPC.EngineerFixSubmergedOxygen:
                    RPCProcedure.EngineerFixSubmergedOxygen();
                    break;
                case (byte)CustomRPC.EngineerUsedRepair:
                    RPCProcedure.EngineerUsedRepair();
                    break;
                case (byte)CustomRPC.CleanBody:
                    RPCProcedure.CleanBody(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.TimeMasterRewindTime:
                    RPCProcedure.TimeMasterRewindTime();
                    break;
                case (byte)CustomRPC.AmnesiacRemember:
                    RPCProcedure.AmnesiacRemember(reader.ReadByte());
                    break;
                case (byte)CustomRPC.TimeMasterShield:
                    RPCProcedure.TimeMasterShield();
                    break;
                case (byte)CustomRPC.VeteranAlert:
                    RPCProcedure.VeteranAlert();
                    break;
                case (byte)CustomRPC.VeterenAlertKill:
                    RPCProcedure.VeterenAlertKill(reader.ReadByte());
                    break;
                case (byte)CustomRPC.MedicSetShielded:
                    RPCProcedure.MedicSetShielded(reader.ReadByte());
                    break;
                case (byte)CustomRPC.Fortify:
                    RPCProcedure.Fortify(reader.ReadByte());
                    break;
                case (byte)CustomRPC.RomanticSetBeloved:
                    RPCProcedure.RomanticSetBeloved(reader.ReadByte());
                    break;
                case (byte)CustomRPC.ShieldedMurderAttempt:
                    RPCProcedure.ShieldedMurderAttempt();
                    break;
                case (byte)CustomRPC.FortifiedMurderAttempt:
                    RPCProcedure.FortifiedMurderAttempt();
                    break;
                case (byte)CustomRPC.ShifterShift:
                    RPCProcedure.ShifterShift(reader.ReadByte());
                    break;
                case (byte)CustomRPC.Confess:
                    byte confessorId = reader.ReadByte();
                    Oracle.Confessor = Helpers.PlayerById(confessorId);
                    if (Oracle.Confessor == null) break; // Ensure the confessor exists
                    // Read the revealed faction from the RPC
                    int factionId = reader.ReadInt32();
                    // Map the received integer to the correct Factions enum
                    if (Enum.IsDefined(typeof(Factions), factionId))
                    {
                        Oracle.RevealedFaction = (Factions)factionId;
                    }
                    else
                    {
                        TownOfSushiPlugin.Logger.LogError($"Invalid faction ID received: {factionId}");
                        Oracle.RevealedFaction = Factions.Other; // Default to Other in case of error
                    }
                    break;
                case (byte)CustomRPC.SwapperSwap:
                    byte playerId1 = reader.ReadByte();
                    byte playerId2 = reader.ReadByte();
                    RPCProcedure.SwapperSwap(playerId1, playerId2);
                    break;
                case (byte)CustomRPC.MayorSetVoteTwice:
                    Mayor.voteTwice = reader.ReadBoolean();
                    break;
                case (byte)CustomRPC.Disperse:
                    byte teleports = reader.ReadByte();
                    Dictionary<byte, Vector2> coordinates = new Dictionary<byte, Vector2>();
                    for (int i = 0; i < teleports; i++)
                    {
                        byte playerId11 = reader.ReadByte();
                        Vector2 location = reader.ReadVector2();
                        coordinates.Add(playerId11, location);
                    }
                    RPCProcedure.StartTransportation(coordinates);
                    break;
                case (byte)CustomRPC.MorphlingMorph:
                    RPCProcedure.MorphlingMorph(reader.ReadByte());
                    break;
                case (byte)CustomRPC.GlitchMimic:
                    RPCProcedure.GlitchMimic(reader.ReadByte());
                    break;
                case (byte)CustomRPC.HitmanMorph:
                    RPCProcedure.HitmanMorph(reader.ReadByte());
                    break;
                case (byte)CustomRPC.CamouflagerCamouflage:
                    RPCProcedure.CamouflagerCamouflage();
                    break;
                case (byte)CustomRPC.WerewolfMaul:
                    RPCProcedure.WerewolfMaul();
                    break;
                case (byte)CustomRPC.VampireSetBitten:
                    byte bittenId = reader.ReadByte();
                    byte reset = reader.ReadByte();
                    RPCProcedure.VampireSetBitten(bittenId, reset);
                    break;
                case (byte)CustomRPC.PlaceGarlic:
                    RPCProcedure.PlaceGarlic(reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.TrackerUsedTracker:
                    RPCProcedure.TrackerUsedTracker(reader.ReadByte());
                    break;               
                case (byte)CustomRPC.GlitchUsedHacks:
                    RPCProcedure.GlitchUsedHacks(reader.ReadByte());
                    break;
                case (byte)CustomRPC.JackalCreatesSidekick:
                    RPCProcedure.JackalCreatesSidekick(reader.ReadByte());
                    break;
                case (byte)CustomRPC.SidekickPromotes:
                    RPCProcedure.SidekickPromotes();
                    break;
                case (byte)CustomRPC.ErasePlayerRoles:
                    byte eraseTarget = reader.ReadByte();
                    RPCProcedure.ErasePlayerRoles(eraseTarget);
                    Eraser.alreadyErased.Add(eraseTarget);
                    break;
                case (byte)CustomRPC.SetFutureErased:
                    RPCProcedure.SetFutureErased(reader.ReadByte());
                    break;
                case (byte)CustomRPC.SetFutureShifted:
                    RPCProcedure.SetFutureShifted(reader.ReadByte());
                    break;
                case (byte)CustomRPC.SetFutureShielded:
                    RPCProcedure.SetFutureShielded(reader.ReadByte());
                    break;
                case (byte)CustomRPC.PlaceNinjaTrace:
                    RPCProcedure.PlaceNinjaTrace(reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.PlacePortal:
                    RPCProcedure.PlacePortal(reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.UsePortal:
                    RPCProcedure.UsePortal(reader.ReadByte(), reader.ReadByte());
                    break;
                case (byte)CustomRPC.PlaceJackInTheBox:
                    RPCProcedure.PlaceJackInTheBox(reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.LightsOut:
                    RPCProcedure.LightsOut();
                    break;
                case (byte)CustomRPC.PlaceCamera:
                    RPCProcedure.PlaceCamera(reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.SealVent:
                    RPCProcedure.SealVent(reader.ReadPackedInt32());
                    break;
                case (byte)CustomRPC.ArsonistWin:
                    RPCProcedure.ArsonistWin();
                    break;
                case (byte)CustomRPC.GuesserShoot:
                    byte killerId = reader.ReadByte();
                    byte dyingTarget = reader.ReadByte();
                    byte guessedTarget = reader.ReadByte();
                    byte guessedRoleId = reader.ReadByte();
                    RPCProcedure.GuesserShoot(killerId, dyingTarget, guessedTarget, guessedRoleId);
                    break;
                case (byte)CustomRPC.LawyerSetTarget:
                    RPCProcedure.LawyerSetTarget(reader.ReadByte()); 
                    break;
                case (byte)CustomRPC.LawyerChangeRole:
                    RPCProcedure.LawyerChangeRole();
                    break;
                case (byte)CustomRPC.RomanticChangeRole:
                    RPCProcedure.RomanticChangeRole();
                    break;
                case (byte)CustomRPC.TurnPestilence:
                    RPCProcedure.PlaguebearerTurnPestilence();
                    break;
                case (byte)CustomRPC.AgentTurnIntoHitman:
                    RPCProcedure.AgentTurnIntoHitman();
                    break;
                case (byte)CustomRPC.SetBlanked:
                    var pid = reader.ReadByte();
                    var blankedValue = reader.ReadByte();
                    RPCProcedure.SetBlanked(pid, blankedValue);
                    break;
                case (byte)CustomRPC.SetFutureSpelled:
                    RPCProcedure.SetFutureSpelled(reader.ReadByte());
                    break;
                case (byte)CustomRPC.Bloody:
                    byte bloodyKiller = reader.ReadByte();
                    byte bloodyDead = reader.ReadByte();
                    RPCProcedure.Bloody(bloodyKiller, bloodyDead);
                    break;
                case (byte)CustomRPC.SetFirstKill:
                    byte firstKill = reader.ReadByte();
                    RPCProcedure.SetFirstKill(firstKill);
                    break;
                case (byte)CustomRPC.SetTiebreak:
                    RPCProcedure.SetTiebreak();
                    break;
                case (byte)CustomRPC.SetInvisible:
                    byte invisiblePlayer = reader.ReadByte();
                    byte invisibleFlag = reader.ReadByte();
                    RPCProcedure.SetInvisible(invisiblePlayer, invisibleFlag);
                    break;
                case (byte)CustomRPC.ThiefStealsRole:
                    byte thiefTargetId = reader.ReadByte();
                    RPCProcedure.ThiefStealsRole(thiefTargetId);
                    break;
                case (byte)CustomRPC.SetTrap:
                    RPCProcedure.SetTrap(reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.TriggerTrap:
                    byte trappedPlayer = reader.ReadByte();
                    byte trapId = reader.ReadByte();
                    RPCProcedure.TriggerTrap(trappedPlayer, trapId);
                    break;
                case (byte)CustomRPC.StopStart:
                    RPCProcedure.StopStart(reader.ReadByte());
                    break;
                case (byte)CustomRPC.YoyoMarkLocation:
                    RPCProcedure.YoyoMarkLocation(reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.YoyoBlink:
                    RPCProcedure.YoyoBlink(reader.ReadByte() == byte.MaxValue, reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.BreakArmor:
                    RPCProcedure.BreakArmor();
                    break;

                // Game mode
                case (byte)CustomRPC.SetGuessers:
                    byte Guesser = reader.ReadByte();
                    RPCProcedure.SetGuessers(Guesser);
                    break;
                case (byte)CustomRPC.ShareGhostInfo:
                    RPCProcedure.ReceiveGhostInfo(reader.ReadByte(), reader);
                    break;


                case (byte)CustomRPC.ShareRoom:
                    byte roomPlayer = reader.ReadByte();
                    byte roomId = reader.ReadByte();
                    RPCProcedure.ShareRoom(roomPlayer, roomId);
                    break;
            }
        }
    }
} 
