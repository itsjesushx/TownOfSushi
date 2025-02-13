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
            Silhouette.clearSilhouettes();
            Portal.ClearPortals();
            Bloodytrail.ResetSprites();
            Trap.clearTraps();
            ClearAndReloadMapOptions();
            ClearAndReloadRoles();
            ClearGameHistory();
            SetCustomButtonCooldowns();
            ReloadPluginOptions();
            Helpers.ToggleZoom(reset : true);
            GameStartManagerPatch.GameStartManagerUpdatePatch.startingTimer = 0;
            SurveillanceMinigamePatch.nightVisionOverlays = null;
            EventUtility.ClearAndReload();
            MapBehaviourPatch.ClearAndReload();
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
                        Jester.jester = player;
                        break;
                    case RoleId.Mayor:
                        Mayor.mayor = player;
                        break;
                    case RoleId.Portalmaker:
                        Portalmaker.portalmaker = player;
                        break;
                    case RoleId.Engineer:
                        Engineer.engineer = player;
                        break;
                    case RoleId.Sheriff:
                        Sheriff.sheriff = player;
                        break;
                    case RoleId.Glitch:
                        Glitch.Player = player;
                        break;
                    case RoleId.Werewolf:
                        Werewolf.Player = player;
                        break;
                    case RoleId.Lighter:
                        Lighter.lighter = player;
                        break;
                    case RoleId.Godfather:
                        Godfather.godfather = player;
                        break;
                    case RoleId.Mafioso:
                        Mafioso.mafioso = player;
                        break;
                    case RoleId.Janitor:
                        Janitor.janitor = player;
                        break;
                    case RoleId.Detective:
                        Detective.detective = player;
                        break;
                    case RoleId.TimeMaster:
                        TimeMaster.timeMaster = player;
                        break;
                    case RoleId.Veteran:
                        Veteran.Player = player;
                        break;
                    case RoleId.Medic:
                        Medic.medic = player;
                        break;
                    case RoleId.Shifter:
                        Shifter.shifter = player;
                        break;
                    case RoleId.Swapper:
                        Swapper.swapper = player;
                        break;
                    case RoleId.Mystic:
                        Mystic.Player = player;
                        break;
                    case RoleId.Morphling:
                        Morphling.morphling = player;
                        break;
                    case RoleId.Camouflager:
                        Camouflager.camouflager = player;
                        break;
                    case RoleId.SerialKiller:
                        SerialKiller.Player = player;
                        break;
                    case RoleId.Hacker:
                        Hacker.hacker = player;
                        break;
                    case RoleId.Tracker:
                        Tracker.tracker = player;
                        break;
                    case RoleId.Vampire:
                        Vampire.vampire = player;
                        break;
                    case RoleId.Snitch:
                        Snitch.snitch = player;
                        break;
                    case RoleId.Jackal:
                        Jackal.jackal = player;
                        break;
                    case RoleId.Sidekick:
                        Sidekick.sidekick = player;
                        break;
                    case RoleId.Eraser:
                        Eraser.eraser = player;
                        break;
                    case RoleId.Spy:
                        Spy.spy = player;
                        break;
                    case RoleId.Trickster:
                        Trickster.trickster = player;
                        break;
                    case RoleId.Cleaner:
                        Cleaner.cleaner = player;
                        break;
                    case RoleId.Warlock:
                        Warlock.warlock = player;
                        break;
                    case RoleId.SecurityGuard:
                        SecurityGuard.securityGuard = player;
                        break;
                    case RoleId.Arsonist:
                        Arsonist.arsonist = player;
                        break;
                    case RoleId.BountyHunter:
                        BountyHunter.bountyHunter = player;
                        break;
                    case RoleId.Vulture:
                        Vulture.vulture = player;
                        break;
                    case RoleId.Medium:
                        Medium.medium = player;
                        break;
                    case RoleId.Trapper:
                        Trapper.trapper = player;
                        break;
                    case RoleId.Lawyer:
                        Lawyer.lawyer = player;
                        break;
                    case RoleId.Prosecutor:
                        Lawyer.lawyer = player;
                        Lawyer.isProsecutor = true;
                        break;
                    case RoleId.Pursuer:
                        Pursuer.pursuer = player;
                        break;
                    case RoleId.Witch:
                        Witch.witch = player;
                        break;
                    case RoleId.Ninja:
                        Ninja.ninja = player;
                        break;
                    case RoleId.Thief:
                        Thief.thief = player;
                        break;
                    case RoleId.Bomber:
                        Bomber.bomber = player;
                        break;
                    case RoleId.Yoyo:
                        Yoyo.yoyo = player;
                        break;
                    }
                    if (AmongUsClient.Instance.AmHost && Helpers.RoleCanUseVents(player) && !player.Data.Role.IsImpostor) {
                        player.RpcSetRole(RoleTypes.Engineer);
                        player.CoSetRole(RoleTypes.Engineer, true);
                    }                   
                }
            }
        }

        public static void SetModifier(byte modifierId, byte playerId, byte flag) {
            PlayerControl player = Helpers.PlayerById(playerId); 
            switch ((RoleId)modifierId) {
                case RoleId.Bait:
                    Bait.bait.Add(player);
                    break;
                case RoleId.Lover:
                    if (flag == 0) Lovers.lover1 = player;
                    else Lovers.lover2 = player;
                    break;
                case RoleId.Bloody:
                    global::TownOfSushi.Bloody.bloody.Add(player);
                    break;
                case RoleId.AntiTeleport:
                    AntiTeleport.antiTeleport.Add(player);
                    break;
                case RoleId.Tiebreaker:
                    Tiebreaker.tiebreaker = player;
                    break;
                case RoleId.Sunglasses:
                    Sunglasses.sunglasses.Add(player);
                    break;
                case RoleId.Mini:
                    Mini.mini = player;
                    break;
                case RoleId.Vip:
                    Vip.vip.Add(player);
                    break;
                case RoleId.Invert:
                    Invert.invert.Add(player);
                    break;
                case RoleId.Chameleon:
                    Chameleon.chameleon.Add(player);
                    break;
                case RoleId.Armored:
                    Armored.armored = player;
                    break;
                case RoleId.Shifter:
                    Shifter.shifter = player;
                    break;
            }
        }

        public static void VersionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId) {
            System.Version ver;
            if (revision < 0) 
                ver = new System.Version(major, minor, build);
            else 
                ver = new System.Version(major, minor, build, revision);
            GameStartManagerPatch.playerVersions[clientId] = new GameStartManagerPatch.PlayerVersion(ver, guid);
        }

        public static void UseUncheckedVent(int ventId, byte playerId, byte isEnter) {
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

        public static void UncheckedMurderPlayer(byte sourceId, byte targetId, byte showAnimation) {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            PlayerControl source = Helpers.PlayerById(sourceId);
            PlayerControl target = Helpers.PlayerById(targetId);
            if (source != null && target != null) {
                if (showAnimation == 0) KillAnimationCoPerformKillPatch.hideNextAnimation = true;
                source.MurderPlayer(target);
            }
        }

        public static void UncheckedCmdReportDeadBody(byte sourceId, byte targetId) {
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
                Helpers.ShowFlash(Engineer.color, 0.5f, "Engineer Fix"); ;
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
            if (Vulture.vulture != null && cleaningPlayerId == Vulture.vulture.PlayerId) 
            {
                Vulture.eatenBodies++;
                if (Vulture.eatenBodies == Vulture.vultureNumberToWin) 
                {
                    Vulture.triggerVultureWin = true;
                }
            }
        }

        public static void TimeMasterRewindTime() 
        {
            TimeMaster.shieldActive = false; // Shield is no longer active when rewinding
            SoundEffectsManager.Stop("timemasterShield");  // Shield sound stopped when rewinding
            if(TimeMaster.timeMaster != null && TimeMaster.timeMaster == PlayerControl.LocalPlayer) {
                ResetTimeMasterButton();
            }
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.color = new Color(0f, 0.5f, 0.8f, 0.3f);
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = true;
            FastDestroyableSingleton<HudManager>.Instance.FullScreen.gameObject.SetActive(true);
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(TimeMaster.rewindTime / 2, new Action<float>((p) => {
                if (p == 1f) FastDestroyableSingleton<HudManager>.Instance.FullScreen.enabled = false;
            })));

            if (TimeMaster.timeMaster == null || PlayerControl.LocalPlayer == TimeMaster.timeMaster) return; // Time Master himself does not rewind

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
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Veteran.Duration, new Action<float>((p) => {
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
        public static void WerewolfMaul() 
        {
           var nearbyPlayers = Helpers.GetClosestPlayers(Werewolf.Player.GetTruePosition(), Werewolf.Radius);

            foreach (var player in nearbyPlayers)
            {
                if (Werewolf.Player == player || player.Data.IsDead || player == MapOptions.firstKillPlayer)
                    continue;
                    
                if (Veteran.Player != null && Veteran.Player == player && Veteran.AlertActive)
                {
                    Helpers.CheckMurderAttemptAndKill(player, Werewolf.Player, showAnimation: false);
                }
                else
                {
                    Helpers.CheckMurderAttemptAndKill(Werewolf.Player, player, showAnimation: false);
                }
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(player.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                writer.Write(player.PlayerId);
                writer.Write((byte)GhostInfoTypes.DeathReasonAndKiller);
                writer.Write(player.PlayerId);
                writer.Write((byte)DeadPlayer.CustomDeathReason.Maul);
                writer.Write(Werewolf.Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                OverrideDeathReasonAndKiller(player, DeadPlayer.CustomDeathReason.Maul, killer: Werewolf.Player);
            }
        }

        public static void ShieldedMurderAttempt() 
        {
            if (Medic.shielded == null || Medic.medic == null) return;
            
            bool isShieldedAndShow = Medic.shielded == PlayerControl.LocalPlayer && Medic.showAttemptToShielded;
            isShieldedAndShow = isShieldedAndShow && (Medic.meetingAfterShielding || !Medic.showShieldAfterMeeting);  // Dont show attempt, if shield is not shown yet
            bool isMedicAndShow = Medic.medic == PlayerControl.LocalPlayer && Medic.showAttemptToMedic;

            if (isShieldedAndShow || isMedicAndShow || Helpers.ShouldShowGhostInfo()) Helpers.ShowFlash(Palette.ImpostorRed, duration: 0.5f, "Failed Murder Attempt on Shielded Player");
        }

        public static void ShifterShift(byte targetId) 
        {
            PlayerControl oldShifter = Shifter.shifter;
            PlayerControl player = Helpers.PlayerById(targetId);
            if (player == null || oldShifter == null) return;

            Shifter.futureShift = null;
            Shifter.ClearAndReload();

            // Suicide (exile) when impostor or impostor variants
            if ((player.Data.Role.IsImpostor || Helpers.IsNeutral(player)) && !oldShifter.Data.IsDead) {
                oldShifter.Exiled();
                GameHistory.OverrideDeathReasonAndKiller(oldShifter, DeadPlayer.CustomDeathReason.Shift, player);
                if (oldShifter == Lawyer.target && AmongUsClient.Instance.AmHost && Lawyer.lawyer != null) {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.LawyerPromotesToPursuer();
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
            if (MeetingHud.Instance) {
                Swapper.playerId1 = playerId1;
                Swapper.playerId2 = playerId2;
            }
        }

        public static void MorphlingMorph(byte playerId) 
        {
            PlayerControl target = Helpers.PlayerById(playerId);
            if (Morphling.morphling == null || target == null) return;

            Morphling.morphTimer = Morphling.duration;
            Morphling.morphTarget = target;
            if (Camouflager.camouflageTimer <= 0f)
                Morphling.morphling.SetLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
        }

        public static void GlitchMimic(byte playerId) 
        {
            PlayerControl target = Helpers.PlayerById(playerId);
            if (Glitch.Player == null || target == null) return;

            Glitch.MimicTimer = Glitch.MimicDuration;
            Glitch.MimicTarget = target;
            if (Camouflager.camouflageTimer <= 0f)
                Glitch.Player.SetLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
        }

        public static void CamouflagerCamouflage() 
        {
            if (Camouflager.camouflager == null) return;

            Camouflager.camouflageTimer = Camouflager.duration;
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

            if (Vampire.vampire == null) return;
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

        public static void trackerUsedTracker(byte targetId) {
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
            if (Lawyer.target == player && Lawyer.isProsecutor && Lawyer.lawyer != null && !Lawyer.lawyer.Data.IsDead) Lawyer.isProsecutor = false;

            if (!Jackal.canCreateSidekickFromImpostor && player.Data.Role.IsImpostor) 
            {
                Jackal.fakeSidekick = player;
            } 
            else 
            {
                bool wasSpy = Spy.spy != null && player == Spy.spy;
                bool wasImpostor = player.Data.Role.IsImpostor;  // This can only be reached if impostors can be sidekicked.
                FastDestroyableSingleton<RoleManager>.Instance.SetRole(player, RoleTypes.Crewmate);
                if (player == Lawyer.lawyer && Lawyer.target != null)
                {
                    Transform playerInfoTransform = Lawyer.target.cosmetics.nameText.transform.parent.FindChild("Info");
                    TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                    if (playerInfo != null) playerInfo.text = "";
                }
                ErasePlayerRoles(player.PlayerId, true);
                Sidekick.sidekick = player;
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
            Jackal.jackal = Sidekick.sidekick;
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
            if (player == Mayor.mayor) Mayor.ClearAndReload();
            if (player == Portalmaker.portalmaker) Portalmaker.ClearAndReload();
            if (player == Engineer.engineer) Engineer.ClearAndReload();
            if (player == Sheriff.sheriff) Sheriff.ClearAndReload();
            if (player == Lighter.lighter) Lighter.ClearAndReload();
            if (player == Detective.detective) Detective.ClearAndReload();
            if (player == TimeMaster.timeMaster) TimeMaster.ClearAndReload();
            if (player == Veteran.Player) Veteran.ClearAndReload();
            if (player == Medic.medic) Medic.ClearAndReload();
            if (player == Shifter.shifter) Shifter.ClearAndReload();
            if (player == Mystic.Player) Mystic.ClearAndReload();
            if (player == Hacker.hacker) Hacker.ClearAndReload();
            if (player == Tracker.tracker) Tracker.ClearAndReload();
            if (player == Snitch.snitch) Snitch.ClearAndReload();
            if (player == Swapper.swapper) Swapper.ClearAndReload();
            if (player == Spy.spy) Spy.ClearAndReload();
            if (player == SecurityGuard.securityGuard) SecurityGuard.ClearAndReload();
            if (player == Medium.medium) Medium.ClearAndReload();
            if (player == Trapper.trapper) Trapper.ClearAndReload();

            // Impostor roles
            if (player == Morphling.morphling) Morphling.ClearAndReload();
            if (player == Camouflager.camouflager) Camouflager.ClearAndReload();
            if (player == Godfather.godfather) Godfather.ClearAndReload();
            if (player == Mafioso.mafioso) Mafioso.ClearAndReload();
            if (player == Janitor.janitor) Janitor.ClearAndReload();
            if (player == Vampire.vampire) Vampire.ClearAndReload();
            if (player == Eraser.eraser) Eraser.ClearAndReload();
            if (player == Trickster.trickster) Trickster.ClearAndReload();
            if (player == Cleaner.cleaner) Cleaner.ClearAndReload();
            if (player == Warlock.warlock) Warlock.ClearAndReload();
            if (player == Witch.witch) Witch.ClearAndReload();
            if (player == Ninja.ninja) Ninja.ClearAndReload();
            if (player == Bomber.bomber) Bomber.ClearAndReload();
            if (player == Yoyo.yoyo) Yoyo.ClearAndReload();

            // Other roles
            if (player == Jester.jester) Jester.ClearAndReload();
            if (player == Glitch.Player) Glitch.ClearAndReload();
            if (player == Werewolf.Player) Werewolf.ClearAndReload();
            if (player == SerialKiller.Player) SerialKiller.ClearAndReload();
            if (player == Arsonist.arsonist) Arsonist.ClearAndReload();
            if (Guesser.IsGuesser(player.PlayerId)) Guesser.Clear(player.PlayerId);
            if (player == Jackal.jackal) 
            { // Promote Sidekick and hence override the the Jackal or erase Jackal
                if (Sidekick.promotesToJackal && Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead) 
                {
                    SidekickPromotes();
                }
                else 
                {
                    Jackal.ClearAndReload();
                }
            }
            if (player == Sidekick.sidekick) Sidekick.ClearAndReload();
            if (player == BountyHunter.bountyHunter) BountyHunter.ClearAndReload();
            if (player == Vulture.vulture) Vulture.ClearAndReload();
            if (player == Lawyer.lawyer) Lawyer.ClearAndReload();
            if (player == Pursuer.pursuer) Pursuer.ClearAndReload();
            if (player == Thief.thief) Thief.ClearAndReload();

            // Modifier
            if (!ignoreModifier)
            {
                if (player == Lovers.lover1 || player == Lovers.lover2) Lovers.ClearAndReload(); // The whole Lover couple is being erased
                if (Bait.bait.Any(x => x.PlayerId == player.PlayerId)) Bait.bait.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (global::TownOfSushi.Bloody.bloody.Any(x => x.PlayerId == player.PlayerId)) global::TownOfSushi.Bloody.bloody.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (AntiTeleport.antiTeleport.Any(x => x.PlayerId == player.PlayerId)) AntiTeleport.antiTeleport.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (Sunglasses.sunglasses.Any(x => x.PlayerId == player.PlayerId)) Sunglasses.sunglasses.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (player == Tiebreaker.tiebreaker) Tiebreaker.ClearAndReload();
                if (player == Mini.mini) Mini.ClearAndReload();
                if (Vip.vip.Any(x => x.PlayerId == player.PlayerId)) Vip.vip.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (Invert.invert.Any(x => x.PlayerId == player.PlayerId)) Invert.invert.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (Chameleon.chameleon.Any(x => x.PlayerId == player.PlayerId)) Chameleon.chameleon.RemoveAll(x => x.PlayerId == player.PlayerId);
                if (player == Armored.armored) Armored.ClearAndReload();
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

        public static void PlaceNinjaTrace(byte[] buff) {
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

                if (Camouflager.camouflageTimer <= 0 && !Helpers.MushroomSabotageActive()) target.SetDefaultLook();
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

            SecurityGuard.remainingScrews -= SecurityGuard.camPrice;
            SecurityGuard.placedCameras++;

            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0*sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1*sizeof(float));

            var camera = UnityEngine.Object.Instantiate<SurvCamera>(referenceCamera);
            camera.transform.position = new Vector3(position.x, position.y, referenceCamera.transform.position.z - 1f);
            camera.CamName = $"Security Camera {SecurityGuard.placedCameras}";
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


            if (PlayerControl.LocalPlayer == SecurityGuard.securityGuard) 
            {
                camera.gameObject.SetActive(true);
                camera.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
            } else {
                camera.gameObject.SetActive(false);
            }
            MapOptions.camerasToAdd.Add(camera);
        }

        public static void SealVent(int ventId) 
        {
            Vent vent = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault((x) => x != null && x.Id == ventId);
            if (vent == null) return;

            SecurityGuard.remainingScrews -= SecurityGuard.ventPrice;
            if (PlayerControl.LocalPlayer == SecurityGuard.securityGuard) {
                PowerTools.SpriteAnim animator = vent.GetComponent<PowerTools.SpriteAnim>(); 
                
                vent.EnterVentAnim = vent.ExitVentAnim = null;
                Sprite newSprite = animator == null ? SecurityGuard.getStaticVentSealedSprite() : SecurityGuard.getAnimatedVentSealedSprite();
                SpriteRenderer rend = vent.myRend;
                if (Helpers.IsFungle()) {
                    newSprite = SecurityGuard.getFungleVentSealedSprite();
                    rend = vent.transform.GetChild(3).GetComponent<SpriteRenderer>();
                    animator = vent.transform.GetChild(3).GetComponent<PowerTools.SpriteAnim>();
                }
                animator?.Stop();
                rend.sprite = newSprite;
                if (SubmergedCompatibility.IsSubmerged && vent.Id == 0) vent.myRend.sprite = SecurityGuard.getSubmergedCentralUpperSealedSprite();
                if (SubmergedCompatibility.IsSubmerged && vent.Id == 14) vent.myRend.sprite = SecurityGuard.getSubmergedCentralLowerSealedSprite();
                rend.color = new Color(1f, 1f, 1f, 0.5f);
                vent.name = "FutureSealedVent_" + vent.name;
            }

            MapOptions.ventsToSeal.Add(vent);
        }

        public static void ArsonistWin() 
        {
            Arsonist.triggerArsonistWin = true;
            foreach (PlayerControl p in PlayerControl.AllPlayerControls) {
                if (p != Arsonist.arsonist && !p.Data.IsDead) {
                    p.Exiled();
                    OverrideDeathReasonAndKiller(p, DeadPlayer.CustomDeathReason.Arson, Arsonist.arsonist);
                }
            }
        }

        public static void LawyerSetTarget(byte playerId) 
        {
            Lawyer.target = Helpers.PlayerById(playerId);
        }

        public static void LawyerPromotesToPursuer() 
        {
            PlayerControl player = Lawyer.lawyer;
            PlayerControl client = Lawyer.target;
            Lawyer.ClearAndReload(false);

            Pursuer.pursuer = player;

            if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId && client != null) 
            {
                    Transform playerInfoTransform = client.cosmetics.nameText.transform.parent.FindChild("Info");
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
            if (Thief.thief != null && Thief.thief.PlayerId == killerId && Thief.canStealWithGuess) {
                RoleInfo roleInfo = RoleInfo.allRoleInfos.FirstOrDefault(x => (byte)x.roleId == guessedRoleId);
                if (!Thief.thief.Data.IsDead && !Thief.IsFailedThiefKill(dyingTarget, guesser, roleInfo)) {
                    RPCProcedure.ThiefStealsRole(dyingTarget.PlayerId);
                }
            }

            bool lawyerDiedAdditionally = false;
            if (Lawyer.lawyer != null && !Lawyer.isProsecutor && Lawyer.lawyer.PlayerId == killerId && Lawyer.target != null && Lawyer.target.PlayerId == dyingTargetId) 
            {
                // Lawyer guessed client.
                if (PlayerControl.LocalPlayer == Lawyer.lawyer) {
                    FastDestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(Lawyer.lawyer.Data, Lawyer.lawyer.Data);
                    if (MeetingHudPatch.guesserUI != null) MeetingHudPatch.guesserUIExitButton.OnClick.Invoke();
                }
                Lawyer.lawyer.Exiled();
                lawyerDiedAdditionally = true;
                GameHistory.OverrideDeathReasonAndKiller(Lawyer.lawyer, DeadPlayer.CustomDeathReason.LawyerSuicide, guesser);
            }

            dyingTarget.Exiled();
            GameHistory.OverrideDeathReasonAndKiller(dyingTarget, DeadPlayer.CustomDeathReason.Guess, guesser);
            byte partnerId = dyingLoverPartner != null ? dyingLoverPartner.PlayerId : dyingTargetId;

            HandleGuesser.RemainingShots(killerId, true);
            if (Constants.ShouldPlaySfx()) SoundManager.Instance.PlaySound(dyingTarget.KillSfx, false, 0.8f);
            if (MeetingHud.Instance) 
            {
                foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates) 
                {
                    if (pva.TargetPlayerId == dyingTargetId || pva.TargetPlayerId == partnerId || lawyerDiedAdditionally && Lawyer.lawyer.PlayerId == pva.TargetPlayerId) 
                    {
                        pva.SetDead(pva.DidReport, true);
                        pva.Overlay.gameObject.SetActive(true);
                        MeetingHudPatch.swapperCheckAndReturnSwap(MeetingHud.Instance, pva.TargetPlayerId);
                    }

                    //Give players back their vote if target is shot dead
                    if (pva.VotedFor != dyingTargetId && pva.VotedFor != partnerId && (!lawyerDiedAdditionally || Lawyer.lawyer.PlayerId != pva.VotedFor)) continue;
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
                } else if (dyingLoverPartner != null && PlayerControl.LocalPlayer == dyingLoverPartner) {
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
                RoleInfo roleInfo = RoleInfo.allRoleInfos.FirstOrDefault(x => (byte)x.roleId == guessedRoleId);
                string msg = $"{guesser.Data.PlayerName} guessed the role {roleInfo?.name ?? ""} for {guessedTarget.Data.PlayerName}!";
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
            global::TownOfSushi.Bloody.active.Add(killerPlayerId, global::TownOfSushi.Bloody.duration);
            global::TownOfSushi.Bloody.bloodyKillerMap.Add(killerPlayerId, bloodyPlayerId);
        }

        public static void SetFirstKill(byte playerId) 
        {
            PlayerControl target = Helpers.PlayerById(playerId);
            if (target == null) return;
            MapOptions.firstKillPlayer = target;
        }

        public static void SetTiebreak() 
        {
            Tiebreaker.isTiebreak = true;
        }

        public static void ThiefStealsRole(byte playerId) 
        {
            PlayerControl target = Helpers.PlayerById(playerId);
            PlayerControl thief = Thief.thief;
            if (target == null) return;
            if (target == Sheriff.sheriff) Sheriff.sheriff = thief;
            if (target == Glitch.Player) Glitch.Player = thief;
            if (target == SerialKiller.Player) SerialKiller.Player = thief;
            if (target == Werewolf.Player) Werewolf.Player = thief;
            if (target == Jackal.jackal)
            {
                Jackal.jackal = thief;
                Jackal.formerJackals.Add(target);
            }
            if (target == Sidekick.sidekick) 
            {
                Sidekick.sidekick = thief;
                Jackal.formerJackals.Add(target);
                if (CustomOptionHolder.GuesserSidekickIsAlwaysGuesser.GetBool() && !HandleGuesser.IsGuesser(thief.PlayerId))
                    SetGuessers(thief.PlayerId);
            }
            if (target == Godfather.godfather) Godfather.godfather = thief;
            if (target == Mafioso.mafioso) Mafioso.mafioso = thief;
            if (target == Janitor.janitor) Janitor.janitor = thief;
            if (target == Morphling.morphling) Morphling.morphling = thief;
            if (target == Camouflager.camouflager) Camouflager.camouflager = thief;
            if (target == Vampire.vampire) Vampire.vampire = thief;
            if (target == Eraser.eraser) Eraser.eraser = thief;
            if (target == Trickster.trickster) Trickster.trickster = thief;
            if (target == Cleaner.cleaner) Cleaner.cleaner = thief;
            if (target == Warlock.warlock) Warlock.warlock = thief;
            if (target == BountyHunter.bountyHunter) BountyHunter.bountyHunter = thief;
            if (target == Witch.witch) 
            {
                Witch.witch = thief;
                if (MeetingHud.Instance) 
                    if (Witch.witchVoteSavesTargets)  // In a meeting, if the thief guesses the witch, all targets are saved or no target is saved.
                        Witch.futureSpelled = new();
                else  // If thief kills witch during the round, remove the thief from the list of spelled people, keep the rest
                    Witch.futureSpelled.RemoveAll(x => x.PlayerId == thief.PlayerId);
            }
            if (target == Ninja.ninja) Ninja.ninja = thief;
            if (target == Bomber.bomber) Bomber.bomber = thief;
            if (target == Yoyo.yoyo) {
                Yoyo.yoyo = thief;
                Yoyo.markedLocation = null;
            }
            if (target.Data.Role.IsImpostor) 
            {
                RoleManager.Instance.SetRole(Thief.thief, RoleTypes.Impostor);
                FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(Thief.thief.killTimer, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            }
            if (Lawyer.lawyer != null && target == Lawyer.target)
                Lawyer.target = thief;
            if (Thief.thief == PlayerControl.LocalPlayer) CustomButton.ResetAllCooldowns();
            Thief.ClearAndReload();
            Thief.formerThief = thief;  // After ClearAndReload, else it would get reset...
        }
        
        public static void SetTrap(byte[] buff) 
        {
            if (Trapper.trapper == null) return;
            Trapper.charges -= 1;
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            new Trap(position);
        }

        public static void TriggerTrap(byte playerId, byte trapId) 
        {
            Trap.triggerTrap(playerId, trapId);
        }

        public static void SetGuessers (byte playerId) 
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

        public static void PlaceBomb(byte[] buff) 
        {
            if (Bomber.bomber == null) return;
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            new Bomb(position);
        }

        public static void DefuseBomb() 
        {
            try {
                SoundEffectsManager.PlayAtPosition("bombDefused", Bomber.bomb.bomb.transform.position, range: Bomber.hearRange);
            } catch { }
            Bomber.ClearBomb();
            bomberButton.Timer = bomberButton.MaxTimer;
            bomberButton.isEffectActive = false;
            bomberButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
        }

        public static void ShareRoom(byte playerId, byte roomId) 
        {
            if (Snitch.playerRoomMap.ContainsKey(playerId)) Snitch.playerRoomMap[playerId] = roomId;
            else Snitch.playerRoomMap.Add(playerId, roomId);
        }

        public static void YoyoMarkLocation(byte[] buff) 
        {
            if (Yoyo.yoyo == null) return;
            Vector3 position = Vector3.zero;
            position.x = BitConverter.ToSingle(buff, 0 * sizeof(float));
            position.y = BitConverter.ToSingle(buff, 1 * sizeof(float));
            Yoyo.MarkLocation(position);
            new Silhouette(position, -1, false);
        }

        public static void YoyoBlink(bool isFirstJump, byte[] buff) 
        {
            if (Yoyo.yoyo == null || Yoyo.markedLocation == null) return;
            var markedPos = (Vector3)Yoyo.markedLocation;
            Yoyo.yoyo.NetTransform.SnapTo(markedPos);

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
            if (Chameleon.chameleon.Any(x => x.PlayerId == Yoyo.yoyo.PlayerId)) // Make the Yoyo visible if chameleon!
                Chameleon.lastMoved[Yoyo.yoyo.PlayerId] = Time.time;            
        }

        public static void BreakArmor() 
        {
            if (Armored.armored == null || Armored.isBrokenArmor) return;
            Armored.isBrokenArmor = true;
            if (PlayerControl.LocalPlayer.Data.IsDead) 
            {
                Armored.armored.ShowFailedMurder();
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
                case (byte)CustomRPC.ShieldedMurderAttempt:
                    RPCProcedure.ShieldedMurderAttempt();
                    break;
                case (byte)CustomRPC.ShifterShift:
                    RPCProcedure.ShifterShift(reader.ReadByte());
                    break;
                case (byte)CustomRPC.SwapperSwap:
                    byte playerId1 = reader.ReadByte();
                    byte playerId2 = reader.ReadByte();
                    RPCProcedure.SwapperSwap(playerId1, playerId2);
                    break;
                case (byte)CustomRPC.MayorSetVoteTwice:
                    Mayor.voteTwice = reader.ReadBoolean();
                    break;
                case (byte)CustomRPC.MorphlingMorph:
                    RPCProcedure.MorphlingMorph(reader.ReadByte());
                    break;
                case (byte)CustomRPC.GlitchMimic:
                    RPCProcedure.GlitchMimic(reader.ReadByte());
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
                    RPCProcedure.trackerUsedTracker(reader.ReadByte());
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
                case (byte)CustomRPC.LawyerPromotesToPursuer:
                    RPCProcedure.LawyerPromotesToPursuer();
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
                case (byte)CustomRPC.PlaceBomb:
                    RPCProcedure.PlaceBomb(reader.ReadBytesAndSize());
                    break;
                case (byte)CustomRPC.DefuseBomb:
                    RPCProcedure.DefuseBomb();
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
