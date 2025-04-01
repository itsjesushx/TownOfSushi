using System.Linq;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using TownOfSushi.Objects;
using TownOfSushi.Utilities;
using static TownOfSushi.TownOfSushi;
using AmongUs.Data;
using Hazel;
using Reactor.Utilities.Extensions;

namespace TownOfSushi
{
    [HarmonyPatch]
    public static class TownOfSushi
    {
        public static System.Random rnd = new System.Random((int)DateTime.Now.Ticks);

        public static void ClearAndReloadRoles() 
        {
            Jester.ClearAndReload();
            Mayor.ClearAndReload();
            Portalmaker.ClearAndReload();
            Engineer.ClearAndReload();
            Werewolf.ClearAndReload();
            Sheriff.ClearAndReload();
            Glitch.ClearAndReload();
            Lighter.ClearAndReload();
            Godfather.ClearAndReload();
            Mafioso.ClearAndReload();
            Janitor.ClearAndReload();
            Detective.ClearAndReload();
            TimeMaster.ClearAndReload();
            Medic.ClearAndReload();
            Shifter.ClearAndReload();
            Swapper.ClearAndReload();
            Lovers.ClearAndReload();
            Mystic.ClearAndReload();
            Morphling.ClearAndReload();
            Camouflager.ClearAndReload();
            Hacker.ClearAndReload();
            Juggernaut.ClearAndReload();
            Tracker.ClearAndReload();
            Vampire.ClearAndReload();
            Snitch.ClearAndReload();
            Jackal.ClearAndReload();
            Sidekick.ClearAndReload();
            Eraser.ClearAndReload();
            Spy.ClearAndReload();
            Trickster.ClearAndReload();
            Cleaner.ClearAndReload();
            Pestilence.ClearAndReload();
            Warlock.ClearAndReload();
            Veteran.ClearAndReload();
            Vigilante.ClearAndReload();
            SerialKiller.ClearAndReload();
            Arsonist.ClearAndReload();
            BountyHunter.ClearAndReload();
            Amnesiac.ClearAndReload();
            Vulture.ClearAndReload();
            Medium.ClearAndReload();
            Oracle.ClearAndReload();
            Crusader.ClearAndReload();
            Plaguebearer.ClearAndReload();
            Lawyer.ClearAndReload();
            Pursuer.ClearAndReload();
            Witch.ClearAndReload();
            Undertaker.ClearAndReload();
            Ninja.ClearAndReload();
            Thief.ClearAndReload();
            Trapper.ClearAndReload();
            Yoyo.ClearAndReload();
            Romantic.ClearAndReload();
            VengefulRomantic.ClearAndReload();

            // Modifier
            Bait.ClearAndReload();
            Bloody.ClearAndReload();
            AntiTeleport.ClearAndReload();
            Tiebreaker.ClearAndReload();
            Sunglasses.ClearAndReload();
            Mini.ClearAndReload();
            Disperser.ClearAndReload();
            Vip.ClearAndReload();
            Invert.ClearAndReload();
            Chameleon.ClearAndReload();
            Armored.ClearAndReload();
            Sleuth.ClearAndReload();
            HandleGuesser.ClearAndReload();

           Modules.BetterMaps.BetterPolus.ClearAndReload();
        }

        public static class Jester 
        {
            public static PlayerControl Player;
            public static Color Color = new Color32(236, 98, 165, byte.MaxValue);
            public static bool IsJesterWin = false;
            public static bool canCallEmergency = true;
            public static bool hasImpostorVision = false;
            public static bool CanUseVents;
            public static bool CanMoveInVents;
            public static void ClearAndReload() 
            {
                Player = null;
                CanUseVents = CustomOptionHolder.jesterCanHideInVents.GetBool();
                CanMoveInVents = CustomOptionHolder.jesterCanMoveInVents.GetBool();
                IsJesterWin = false;
                canCallEmergency = CustomOptionHolder.jesterCanCallEmergency.GetBool();
                hasImpostorVision = CustomOptionHolder.jesterHasImpostorVision.GetBool();
            }
        }
        public static class SerialKiller
        {
            public static PlayerControl Player;
            public static PlayerControl CurrentTarget;
            public static bool HasImpostorVision;
            public static Color Color = new Color32(51, 110, 255, byte.MaxValue);           
            public static float StabCooldown;
            public static float StabDuration;
            public static float StabKillCooldown;
            public static bool CanUseVents;
            public static bool Stabbing;
            private static Sprite ButtonSprite;
            public static Sprite GetButtonSprite() 
            {
                if (ButtonSprite) return ButtonSprite;
                ButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Stab.png", 80f);
                return ButtonSprite;
            }
            public static void ClearAndReload()
            {
                CurrentTarget = null;
                Player = null;
                Stabbing = false;
                HasImpostorVision = false;
                StabCooldown = CustomOptionHolder.SerialKillerStabCooldown.GetFloat();
                StabDuration = CustomOptionHolder.SerialKillerStabDuration.GetFloat();
                StabKillCooldown = CustomOptionHolder.SerialKillerStabKillCooldown.GetFloat();
                CanUseVents = CustomOptionHolder.SerialKillerCanUseVents.GetBool();
            }
        }
        
        public static class Portalmaker 
        {
            public static PlayerControl Player;
            public static Color Color = new Color32(69, 69, 169, byte.MaxValue);

            public static float Cooldown;
            public static float usePortalCooldown;
            public static bool logOnlyHasColors;
            public static bool logShowsTime;
            public static bool canPortalFromAnywhere;

            private static Sprite placePortalButtonSprite;
            private static Sprite usePortalButtonSprite;
            private static Sprite usePortalSpecialButtonSprite1;
            private static Sprite usePortalSpecialButtonSprite2;
            private static Sprite logSprite;

            public static Sprite GetPlacePortalButtonSprite() 
            {
                if (placePortalButtonSprite) return placePortalButtonSprite;
                placePortalButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.PlacePortalButton.png", 115f);
                return placePortalButtonSprite;
            }

            public static Sprite getUsePortalButtonSprite() 
            {
                if (usePortalButtonSprite) return usePortalButtonSprite;
                usePortalButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.UsePortalButton.png", 115f);
                return usePortalButtonSprite;
            }

            public static Sprite GetUsePortalSpecialButtonSprite(bool first) 
            {
                if (first) {
                    if (usePortalSpecialButtonSprite1) return usePortalSpecialButtonSprite1;
                    usePortalSpecialButtonSprite1 = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.UsePortalSpecialButton1.png", 115f);
                    return usePortalSpecialButtonSprite1;
                } else {
                    if (usePortalSpecialButtonSprite2) return usePortalSpecialButtonSprite2;
                    usePortalSpecialButtonSprite2 = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.UsePortalSpecialButton2.png", 115f);
                    return usePortalSpecialButtonSprite2;
                }
            }

            public static Sprite GetLogSprite() 
            {
                if (logSprite) return logSprite;
                logSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image;
                return logSprite;
            }

            public static void ClearAndReload() 
            {
                Player = null;
                Cooldown = CustomOptionHolder.portalmakerCooldown.GetFloat();
                usePortalCooldown = CustomOptionHolder.portalmakerUsePortalCooldown.GetFloat();
                logOnlyHasColors = CustomOptionHolder.portalmakerLogOnlyColorType.GetBool();
                logShowsTime = CustomOptionHolder.portalmakerLogHasTime.GetBool();
                canPortalFromAnywhere = CustomOptionHolder.portalmakerCanPortalFromAnywhere.GetBool();
            }
        }

        public static class Amnesiac
        {
            public static PlayerControl Player;
            public static Color Color = new Color32(34, 255, 255, byte.MaxValue);
            public static List<Arrow> AmnesiacArrows = new List<Arrow>();
            public static bool Remembered;
            private static Sprite ButtonSprite;
            public static bool ShowArrows = true;
            public static Sprite GetButtonSprite()
            {
                if (ButtonSprite) return ButtonSprite;
                ButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Remember.png", 115f);
                return ButtonSprite;
            }
            public static void ClearAndReload()
            {
                Player = null;
                Remembered = false;
                ShowArrows = CustomOptionHolder.AmnesiacHasArrows.GetBool();
                if (AmnesiacArrows != null) 
                {
                    foreach (Arrow arrow in AmnesiacArrows)
                        if (arrow?.arrow != null)
                            UnityEngine.Object.Destroy(arrow.arrow);
                }
                AmnesiacArrows = new List<Arrow>();
            }
        }

        public static class Mayor 
        {
            public static PlayerControl Player;
            public static Color Color = new Color32(32, 77, 66, byte.MaxValue);
            public static Minigame emergency = null;
            public static Sprite emergencySprite = null;
            public static int remoteMeetingsLeft = 1;

            public static bool canSeeVoteColors = false;
            public static int tasksNeededToSeeVoteColors;
            public static bool meetingButton = true;
            public static int mayorChooseSingleVote;

            public static bool voteTwice = true;

            public static Sprite GetMeetingSprite()
            {
                if (emergencySprite) return emergencySprite;
                emergencySprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.EmergencyButton.png", 550f);
                return emergencySprite;
            }

            public static void ClearAndReload() 
            {
                Player = null;
                emergency = null;
                emergencySprite = null;
		        remoteMeetingsLeft = Mathf.RoundToInt(CustomOptionHolder.mayorMaxRemoteMeetings.GetFloat()); 
                canSeeVoteColors = CustomOptionHolder.mayorCanSeeVoteColors.GetBool();
                tasksNeededToSeeVoteColors = (int)CustomOptionHolder.mayorTasksNeededToSeeVoteColors.GetFloat();
                meetingButton = CustomOptionHolder.mayorMeetingButton.GetBool();
                mayorChooseSingleVote = CustomOptionHolder.mayorChooseSingleVote.GetSelection();
                voteTwice = true;
            }
        }

        public static class Engineer 
        {
            public static PlayerControl Player;
            public static Color Color = new Color32(0, 40, 245, byte.MaxValue);

            public static int remainingFixes = 1;           
            public static bool highlightForImpostors = true;
            public static bool highlightForTeamJackal = true; 

            private static Sprite ButtonSprite;
            public static Sprite GetButtonSprite() 
            {
                if (ButtonSprite) return ButtonSprite;
                ButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.RepairButton.png", 115f);
                return ButtonSprite;
            }

            public static void ClearAndReload() 
            {
                Player = null;
                remainingFixes = Mathf.RoundToInt(CustomOptionHolder.engineerNumberOfFixes.GetFloat());
                highlightForImpostors = CustomOptionHolder.engineerHighlightForImpostors.GetBool();
                highlightForTeamJackal = CustomOptionHolder.engineerHighlightForTeamJackal.GetBool();
            }
        }

        public static class Godfather 
        {
            public static PlayerControl Player;
            public static Color Color = Palette.ImpostorRed;

            public static void ClearAndReload() 
            {
                Player = null;
            }
        }

        public static class Mafioso 
        {
            public static PlayerControl Player;
            public static Color Color = Palette.ImpostorRed;

            public static void ClearAndReload() 
            {
                Player = null;
            }
        }


        public static class Janitor 
        {
            public static PlayerControl Player;
            public static Color Color = Palette.ImpostorRed;

            public static float Cooldown = 30f;

            private static Sprite buttonSprite;
            public static Sprite GetButtonSprite()
             {
                if (buttonSprite) return buttonSprite;
                buttonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.CleanButton.png", 115f);
                return buttonSprite;
            }

            public static void ClearAndReload() 
            {
                Player = null;
                Cooldown = CustomOptionHolder.janitorCooldown.GetFloat();
            }
        }

        public static class Sheriff 
        {
            public static PlayerControl Player;
            public static Color Color = new Color32(248, 205, 70, byte.MaxValue);
            public static float Cooldown = 30f;
            public static bool canKillNeutrals = false;
            public static bool spyCanDieToSheriff = false;

            public static PlayerControl CurrentTarget;
            public static void ClearAndReload() 
            {
                Player = null;
                CurrentTarget = null;
                Cooldown = CustomOptionHolder.sheriffCooldown.GetFloat();
                canKillNeutrals = CustomOptionHolder.sheriffCanKillNeutrals.GetBool();
                spyCanDieToSheriff = CustomOptionHolder.spyCanDieToSheriff.GetBool();
            }
        }

        public static class Glitch
        {
            public static PlayerControl Player;
            public static Color Color = Color.green;
            public static PlayerControl CurrentTarget;
            public static bool canEnterVents;
            public static float KillCooldown;
            public static List<byte> HackedPlayers = new List<byte>();
            public static float HackDuration;
            public static float remainingHacks;
            public static float HackCooldown;
            private static Sprite SampleSprite;
            private static Sprite MimicSprite;
            public static float MimicCooldown = 30f;
            public static float MimicDuration = 10f;
            public static PlayerControl sampledTarget;
            public static PlayerControl MimicTarget;
            public static float MimicTimer = 0f;
            public static Dictionary<byte, float> HackedKnows = new Dictionary<byte, float>();

            private static Sprite buttonSprite;
            private static Sprite HackedSprite;
            
            public static Sprite GetButtonSprite()
            {
                if (buttonSprite) return buttonSprite;
                buttonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Hack.png", 110f);
                return buttonSprite;
            }

            public static Sprite GetHackedButtonSprite()
            {
                if (HackedSprite) return HackedSprite;
                HackedSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Hack.png", 110f);
                return HackedSprite;
            }
            public static Sprite GetSampleSprite() 
            {
                if (SampleSprite) return SampleSprite;
                SampleSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.SampleButton.png", 115f);
                return SampleSprite;
            }

            public static Sprite GetMorphSprite() 
            {
                if (MimicSprite) return MimicSprite;
                MimicSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.MorphButton.png", 115f);
                return MimicSprite;
            }

            // Can be used to enable / disable the Hack effect on the target's buttons
            public static void SetHackedKnows(bool active = true, byte playerId = Byte.MaxValue)
            {
                if (playerId == Byte.MaxValue)
                    playerId = PlayerControl.LocalPlayer.PlayerId;

                if (active && playerId == PlayerControl.LocalPlayer.PlayerId) 
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)GhostInfoTypes.HackNoticed);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                if (active) 
                {
                    HackedKnows.Add(playerId, HackDuration);
                    HackedPlayers.RemoveAll(x => x == playerId);
               }

                if (playerId == PlayerControl.LocalPlayer.PlayerId) 
                {
                    HudManagerStartPatch.SetAllButtonsHackedStatus(active);
                    SoundEffectsManager.Play("deputyHandcuff");
		        }
	        }
            public static void ResetMimic() 
            {
                MimicTarget = null;
                MimicTimer = 0f;
                if (Player == null) return;
                Player.SetDefaultLook();
            }

            public static void ClearAndReload()
            {
                ResetMimic();
                Player = null;
                sampledTarget = null;
                MimicTarget = null;
                MimicTimer = 0f;
                CurrentTarget = null;
                HackedPlayers = new List<byte>();
                HackedKnows = new Dictionary<byte, float>();
                HudManagerStartPatch.SetAllButtonsHackedStatus(false, true);
                remainingHacks = CustomOptionHolder.GlitchNumberOfHacks.GetFloat();
                HackCooldown = CustomOptionHolder.GlitchHackCooldown.GetFloat();
                HackDuration = CustomOptionHolder.GlitchHackDuration.GetFloat();
                KillCooldown = CustomOptionHolder.GlitchKillCooldowm.GetFloat();
                canEnterVents = CustomOptionHolder.GlitchCanUseVents.GetBool();
                MimicCooldown = CustomOptionHolder.GlitchMimicCooldown.GetFloat();
                MimicDuration = CustomOptionHolder.GlitchMimicDuration.GetFloat();
            }
        }

        public static class Lighter 
        {
            public static PlayerControl Player;
            public static Color Color = new Color32(238, 229, 190, byte.MaxValue);
            
            public static float lighterModeLightsOnVision = 2f;
            public static float lighterModeLightsOffVision = 0.75f;
            public static float flashlightWidth = 0.75f;

            public static void ClearAndReload() 
            {
                Player = null;
                flashlightWidth = CustomOptionHolder.lighterFlashlightWidth.GetFloat();
                lighterModeLightsOnVision = CustomOptionHolder.lighterModeLightsOnVision.GetFloat();
                lighterModeLightsOffVision = CustomOptionHolder.lighterModeLightsOffVision.GetFloat();
            }
        }

        public static class Detective 
        {
            public static PlayerControl Player;
            public static Color Color = new Color32(45, 106, 165, byte.MaxValue);

            public static float footprintIntervall = 1f;
            public static float footprintDuration = 1f;
            public static bool anonymousFootprints = false;
            public static float reportNameDuration = 0f;
            public static float reportColorDuration = 20f;
            public static float timer = 6.2f;

            public static void ClearAndReload() 
            {
                Player = null;
                anonymousFootprints = CustomOptionHolder.detectiveAnonymousFootprints.GetBool();
                footprintIntervall = CustomOptionHolder.detectiveFootprintIntervall.GetFloat();
                footprintDuration = CustomOptionHolder.detectiveFootprintDuration.GetFloat();
                reportNameDuration = CustomOptionHolder.detectiveReportNameDuration.GetFloat();
                reportColorDuration = CustomOptionHolder.detectiveReportColorDuration.GetFloat();
                timer = 6.2f;
            }
        }
    }

    public static class TimeMaster 
    {
        public static PlayerControl Player;
        public static Color Color = new Color32(112, 142, 239, byte.MaxValue);

        public static bool reviveDuringRewind = false;
        public static float rewindTime = 3f;
        public static float shieldDuration = 3f;
        public static float Cooldown = 30f;

        public static bool shieldActive = false;
        public static bool isRewinding = false;

        private static Sprite buttonSprite;
        public static Sprite GetButtonSprite() 
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.TimeShieldButton.png", 115f);
            return buttonSprite;
        }

        public static void ClearAndReload() 
        {
            Player = null;
            isRewinding = false;
            shieldActive = false;
            rewindTime = CustomOptionHolder.timeMasterRewindTime.GetFloat();
            shieldDuration = CustomOptionHolder.timeMasterShieldDuration.GetFloat();
            Cooldown = CustomOptionHolder.timeMasterCooldown.GetFloat();
        }
    }

    public static class Medic 
    {
        public static PlayerControl Player;
        public static PlayerControl shielded;
        public static PlayerControl futureShielded;
        
        public static Color Color = new Color32(126, 251, 194, byte.MaxValue);
        public static bool usedShield;

        public static int showShielded = 0;
        public static bool showAttemptToShielded = false;
        public static bool showAttemptToMedic = false;
        public static bool setShieldAfterMeeting = false;
        public static bool showShieldAfterMeeting = false;
        public static bool meetingAfterShielding = false;

        public static Color shieldedColor = new Color32(0, 221, 255, byte.MaxValue);
        public static PlayerControl CurrentTarget;

        private static Sprite buttonSprite;
        public static Sprite GetButtonSprite() 
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.ShieldButton.png", 115f);
            return buttonSprite;
        }

        public static bool ShieldVisible(PlayerControl target) 
        {
            bool hasVisibleShield = false;

            bool isMorphedMorphling = target == Morphling.Player && Morphling.morphTarget != null && Morphling.morphTimer > 0f;
            bool isMimicGlitch = target == Glitch.Player && Glitch.MimicTarget != null && Glitch.MimicTimer > 0f;
            if (Medic.shielded != null && ((target == Medic.shielded && !isMorphedMorphling) || (target == Medic.shielded && !isMimicGlitch) || (isMorphedMorphling && Morphling.morphTarget == Medic.shielded) || (isMimicGlitch && Glitch.MimicTarget == Medic.shielded))) 
            {
                hasVisibleShield = Medic.showShielded == 0 || Helpers.ShouldShowGhostInfo() // Everyone or Ghost info
                    || (Medic.showShielded == 1 && (PlayerControl.LocalPlayer == Medic.shielded || PlayerControl.LocalPlayer == Medic.Player)) // Shielded + Medic
                    || (Medic.showShielded == 2 && PlayerControl.LocalPlayer == Medic.Player); // Medic only
                // Make shield invisible till after the next meeting if the option is set (the medic can already see the shield)
                hasVisibleShield = hasVisibleShield && (Medic.meetingAfterShielding || !Medic.showShieldAfterMeeting || PlayerControl.LocalPlayer == Medic.Player || Helpers.ShouldShowGhostInfo());
            }
            return hasVisibleShield;
        }

        public static void ClearAndReload() 
        {
            Player = null;
            shielded = null;
            futureShielded = null;
            CurrentTarget = null;
            usedShield = false;
            showShielded = CustomOptionHolder.medicShowShielded.GetSelection();
            showAttemptToShielded = CustomOptionHolder.medicShowAttemptToShielded.GetBool();
            showAttemptToMedic = CustomOptionHolder.medicShowAttemptToMedic.GetBool();
            setShieldAfterMeeting = CustomOptionHolder.medicSetOrShowShieldAfterMeeting.GetSelection() == 2;
            showShieldAfterMeeting = CustomOptionHolder.medicSetOrShowShieldAfterMeeting.GetSelection() == 1;
            meetingAfterShielding = false;
        }
    }

    public static class Swapper 
    {
        public static PlayerControl Player;
        public static Color Color = new Color32(134, 55, 86, byte.MaxValue);
        private static Sprite spriteCheck;
        public static bool canCallEmergency = false;
        public static bool canOnlySwapOthers = false;
        public static int charges;
        public static float rechargeTasksNumber;
        public static float rechargedTasks;
 
        public static byte playerId1 = Byte.MaxValue;
        public static byte playerId2 = Byte.MaxValue;

        public static Sprite GetCheckSprite() 
        {
            if (spriteCheck) return spriteCheck;
            spriteCheck = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.SwapperCheck.png", 150f);
            return spriteCheck;
        }

        public static void ClearAndReload() 
        {
            Player = null;
            playerId1 = Byte.MaxValue;
            playerId2 = Byte.MaxValue;
            canCallEmergency = CustomOptionHolder.swapperCanCallEmergency.GetBool();
            canOnlySwapOthers = CustomOptionHolder.swapperCanOnlySwapOthers.GetBool();
            charges = Mathf.RoundToInt(CustomOptionHolder.swapperSwapsNumber.GetFloat());
            rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.swapperRechargeTasksNumber.GetFloat());
            rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.swapperRechargeTasksNumber.GetFloat());
        }
    }

    public static class Lovers 
    {
        public static PlayerControl Lover1;
        public static PlayerControl Lover2;
        public static Color Color = new Color32(232, 57, 185, byte.MaxValue);

        public static bool bothDie = true;
        public static bool enableChat = true;
        // Lovers save if next to be exiled is a lover, because RPC of ending game comes before RPC of exiled
        public static bool notAckedExiledIsLover = false;

        public static bool Existing() 
        {
            return Lover1 != null && Lover2 != null && !Lover1.Data.Disconnected && !Lover2.Data.Disconnected;
        }

        public static bool ExistingAndAlive() 
        {
            return Existing() && !Lover1.Data.IsDead && !Lover2.Data.IsDead && !notAckedExiledIsLover; // ADD NOT ACKED IS LOVER
        }

        public static PlayerControl OtherLover(PlayerControl oneLover) 
        {
            if (!ExistingAndAlive()) return null;
            if (oneLover == Lover1) return Lover2;
            if (oneLover == Lover2) return Lover1;
            return null;
        }

        public static bool ExistingWithKiller() 
        {
            return Existing() && (Lover1.IsNeutralKiller() || Lover2.IsNeutralKiller() || Lover1.Data.Role.IsImpostor || Lover2.Data.Role.IsImpostor);
        }

        public static bool HasAliveKillingLover(this PlayerControl player) 
        {
            if (!Lovers.ExistingAndAlive() || !ExistingWithKiller())
                return false;
            return (player != null && (player == Lover1 || player == Lover2));
        }

        public static void ClearAndReload() 
        {
            Lover1 = null;
            Lover2 = null;
            notAckedExiledIsLover = false;
            bothDie = CustomOptionHolder.modifierLoverBothDie.GetBool() && !(Lover1 == Pestilence.Player || Lover2 == Pestilence.Player);
            enableChat = CustomOptionHolder.modifierLoverEnableChat.GetBool();
        }

        public static PlayerControl GetPartner(this PlayerControl player) 
        {
            if (player == null)
                return null;
            if (Lover1 == player)
                return Lover2;
            if (Lover2 == player)
                return Lover1;
            return null;
        }
    }

    public static class Mystic 
    {
        public static bool Investigated;
        public static PlayerControl Player;
        public static Color Color = new Color32(77, 154, 230, byte.MaxValue);
        public static List<Vector3> deadBodyPositions = new List<Vector3>();
        public static PlayerControl CurrentTarget;
        public static float Cooldown;
        public static float soulDuration = 15f;
        public static bool limitSoulDuration = false;
        public static int mode = 0;
        private static Sprite ButtonSprite;
        public static Sprite GetButtonSprite() 
        {
            if (ButtonSprite) return ButtonSprite;
            ButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Mystic.png", 115f);
            return ButtonSprite;
        }
        public static string GetInfo(PlayerControl target)
        {
            var role = RoleInfo.GetRoleInfoForPlayer(target);
            if (target == null) return "";
            if (role == null) return "";

            string message = "";
            foreach (RoleInfo roleInfo in role) 
            {

                if (roleInfo.RoleId == RoleId.Jester || roleInfo.RoleId == RoleId.Prosecutor 
                ||roleInfo.RoleId == RoleId.Mayor || roleInfo.RoleId == RoleId.Ninja 
                ||roleInfo.RoleId == RoleId.Swapper || roleInfo.RoleId == RoleId.Spy)
                {
                    message =  "They will never know that I have a trick up my sleeve! \n\n(Jester, Mayor, Prosecutor, Ninja or Swapper)";
                }

                else if (roleInfo.RoleId == RoleId.Witch || roleInfo.RoleId == RoleId.Tracker && !Tracker.canTrackCorpses ||
                roleInfo.RoleId == RoleId.Trapper || roleInfo.RoleId == RoleId.Detective ||roleInfo.RoleId == RoleId.Hacker)
                {
                    message =  "Hmm, this player must be bad, but I can't share my info or I will be guessed! \n\n(Witch, Tracker, Trapper, Hacker or Detective)";
                }
                
                else if (roleInfo.RoleId == RoleId.Morphling || roleInfo.RoleId == RoleId.Medium ||
                roleInfo.RoleId == RoleId.Glitch || roleInfo.RoleId == RoleId.Engineer || roleInfo.RoleId == RoleId.Snitch)
                {
                    message =  "Why is everything in my reality so weird looking?! \n\n(Glitch, Morphling, Snitch, Medium or Engineer)";
                }
                
                else if (roleInfo.RoleId == RoleId.Vulture || roleInfo.RoleId == RoleId.Undertaker || roleInfo.RoleId == RoleId.Tracker && Tracker.canTrackCorpses ||
                roleInfo.RoleId == RoleId.Cleaner || roleInfo.RoleId == RoleId.Janitor)
                {
                    message =  "BODIES?? WHERE!? \n\n(Cleaner, Undertaker, Vulture, Tracker or Janitor)";
                }

                else if (roleInfo.RoleId == RoleId.Sheriff ||  roleInfo.RoleId == RoleId.Godfather || roleInfo.RoleId == RoleId.Veteran || roleInfo.RoleId == RoleId.BountyHunter || 
                roleInfo.RoleId == RoleId.Warlock || roleInfo.RoleId == RoleId.Vampire || roleInfo.RoleId == RoleId.Pestilence || roleInfo.RoleId == RoleId.Sidekick || roleInfo.RoleId == RoleId.Jackal)
                {
                    message =  "I've been nearby too many kills! They must think I'm evil. \n\n(Sheriff, Pestilence, Warlock, Jackal, Sidekick, Godfather, Bounty Hunter, Vampire or Veteran)";
                }
                    
                else if (roleInfo.RoleId == RoleId.Lawyer || roleInfo.RoleId == RoleId.TimeMaster || roleInfo.RoleId == RoleId.Vigilante ||
                roleInfo.RoleId == RoleId.Medic || roleInfo.RoleId == RoleId.Crusader || roleInfo.RoleId == RoleId.Mafioso || roleInfo.RoleId == RoleId.Pursuer || roleInfo.RoleId == RoleId.Romantic)
                {
                    message =  "I'm scared. I'm going to hide and protect myself or others! \n\n(Medic, Crusader, Romantic, Vigilante, Lawyer, Time Master, Mafioso or Pursuer)";
                }
                
                else if (roleInfo.RoleId == RoleId.Arsonist || roleInfo.RoleId == RoleId.Camouflager || roleInfo.RoleId == RoleId.Portalmaker ||
                roleInfo.RoleId == RoleId.Thief ||  roleInfo.RoleId == RoleId.Lighter || roleInfo.RoleId == RoleId.Plaguebearer || roleInfo.RoleId == RoleId.Trickster)
                {
                    message =  "I love playing among the group! \n\n(Portalmaker, Lighter, Arsonist, Plaguebearer, Camouflager, Thief or Trickster)";
                }
                    
                else if (roleInfo.RoleId == RoleId.Crewmate || roleInfo.RoleId == RoleId.Impostor)
                {
                    message =  "I'm roleless. this sucks! \n\n(Plain Impostor or Crewmate)";
                }
                
                else 
                {
                    message = "Error";
                }
            }

            return CurrentTarget.Data.PlayerName + "'s Mind:\n" + message;
        }

        public static int Charges;
        public static int rechargeTasksNumber;
        public static int rechargedTasks;

        private static Sprite soulSprite;
        public static Sprite GetSoulSprite() 
        {
            if (soulSprite) return soulSprite;
            soulSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Soul.png", 500f);
            return soulSprite;
        }

        public static void ClearAndReload() 
        {
            Player = null;
            CurrentTarget = null;
            Investigated = false;
            deadBodyPositions = new List<Vector3>();
            Cooldown = CustomOptionHolder.MysticCooldown.GetFloat();
            limitSoulDuration = CustomOptionHolder.MysticLimitSoulDuration.GetBool();
            soulDuration = CustomOptionHolder.MysticSoulDuration.GetFloat();
            mode = CustomOptionHolder.MysticMode.GetSelection();
            Charges = Mathf.RoundToInt(CustomOptionHolder.MysticCharges.GetFloat());
            rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.MysticRechargeTasksNumber.GetFloat());
            rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.MysticRechargeTasksNumber.GetFloat());
        }
    }
    public static class Crusader
    {
        public static PlayerControl Player;
        public static Color Color = new Color32(255, 134, 69, byte.MaxValue);
        public static PlayerControl CurrentTarget;
        public static bool Fortified;
        public static float Cooldown;
        public static PlayerControl FortifiedPlayer;
        private static Sprite ButtonSprite;
        public static Sprite GetButtonSprite() 
        {
            if (ButtonSprite) return ButtonSprite;
            ButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Fortify.png", 115f);
            return ButtonSprite;
        }
        public static int Charges;
        public static int rechargeTasksNumber;
        public static int rechargedTasks;
        public static void ClearAndReload()
        {
            Player = null;
            CurrentTarget = null;
            Fortified = false;
            Cooldown = CustomOptionHolder.CrusaderCooldown.GetFloat();
            FortifiedPlayer = null;
            Charges = Mathf.RoundToInt(CustomOptionHolder.CrusaderCharges.GetFloat());
            rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.CrusaderRechargeTasksNumber.GetFloat());
            rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.CrusaderRechargeTasksNumber.GetFloat());
        }
    }

    public static class Morphling 
    {
        public static PlayerControl Player;
        public static Color Color = Palette.ImpostorRed;
        private static Sprite sampleSprite;
        private static Sprite morphSprite;
        public static float Cooldown = 30f;
        public static float Duration = 10f;
        public static PlayerControl sampledTarget;
        public static PlayerControl morphTarget;
        public static float morphTimer = 0f;
        public static PlayerControl CurrentTarget;

        public static void ResetMorph() 
        {
            morphTarget = null;
            morphTimer = 0f;
            if (Player == null) return;
            Player.SetDefaultLook();
        }

        public static void ClearAndReload() 
        {
            ResetMorph();
            Player = null;
            CurrentTarget = null;
            sampledTarget = null;
            morphTarget = null;
            morphTimer = 0f;
            Cooldown = CustomOptionHolder.morphlingCooldown.GetFloat();
            Duration = CustomOptionHolder.morphlingDuration.GetFloat();
        }

        public static Sprite GetSampleSprite() 
        {
            if (sampleSprite) return sampleSprite;
            sampleSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.SampleButton.png", 115f);
            return sampleSprite;
        }

        public static Sprite GetMorphSprite() 
        {
            if (morphSprite) return morphSprite;
            morphSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.MorphButton.png", 115f);
            return morphSprite;
        }
    }

    public static class Camouflager 
    {
        public static PlayerControl Player;
        public static Color Color = Palette.ImpostorRed;
    
        public static float Cooldown = 30f;
        public static float Duration = 10f;
        public static float CamouflageTimer = 0f;

        private static Sprite ButtonSprite;
        public static Sprite GetButtonSprite() 
        {
            if (ButtonSprite) return ButtonSprite;
            ButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.CamoButton.png", 115f);
            return ButtonSprite;
        }

        public static void ResetCamouflage() 
        {
            CamouflageTimer = 0f;
            foreach (PlayerControl p in PlayerControl.AllPlayerControls) 
            {
                if (p == Ninja.ninja && Ninja.isInvisble)
                    continue;
                p.SetDefaultLook();
            }
        }

        public static void ClearAndReload() 
        {
            ResetCamouflage();
            Player = null;
            CamouflageTimer = 0f;
            Cooldown = CustomOptionHolder.camouflagerCooldown.GetFloat();
            Duration = CustomOptionHolder.camouflagerDuration.GetFloat();
        }
    }

    public static class Hacker 
    {
        public static PlayerControl Player;
        public static Minigame vitals = null;
        public static Minigame doorLog = null;
        public static Color Color = new Color32(117, 250, 76, byte.MaxValue);

        public static float Cooldown = 30f;
        public static float Duration = 10f;
        public static float toolsNumber = 5f;
        public static bool onlyColorType = false;
        public static float hackerTimer = 0f;
        public static int rechargeTasksNumber = 2;
        public static int rechargedTasks = 2;
        public static int chargesVitals = 1;
        public static int chargesAdminTable = 1;
        public static bool cantMove = true;

        private static Sprite buttonSprite;
        private static Sprite vitalsSprite;
        private static Sprite logSprite;
        private static Sprite adminSprite;

        public static Sprite GetButtonSprite() 
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.HackerButton.png", 115f);
            return buttonSprite;
        }

        public static Sprite GetVitalsSprite() 
        {
            if (vitalsSprite) return vitalsSprite;
            vitalsSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.VitalsButton].Image;
            return vitalsSprite;
        }

        public static Sprite GetLogSprite() 
        {
            if (logSprite) return logSprite;
            logSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image;
            return logSprite;
        }

        public static Sprite GetAdminSprite() 
        {
            byte mapId = GameOptionsManager.Instance.currentNormalGameOptions.MapId;
            UseButtonSettings button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.PolusAdminButton]; // Polus
            if (Helpers.IsSkeld() || mapId == 3) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton]; // Skeld || Dleks
            else if (Helpers.IsMira()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.MIRAAdminButton]; // Mira HQ
            else if (Helpers.IsAirship()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AirshipAdminButton]; // Airship
            else if (Helpers.IsFungle()) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton];  // Hacker can Access the Admin panel on Fungle
            adminSprite = button.Image;
            return adminSprite;
        }

        public static void ClearAndReload() 
        {
            Player = null;
            vitals = null;
            doorLog = null;
            hackerTimer = 0f;
            adminSprite = null;
            Cooldown = CustomOptionHolder.hackerCooldown.GetFloat();
            Duration = CustomOptionHolder.hackerHackeringDuration.GetFloat();
            onlyColorType = CustomOptionHolder.hackerOnlyColorType.GetBool();
            toolsNumber = CustomOptionHolder.hackerToolsNumber.GetFloat();
            rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.hackerRechargeTasksNumber.GetFloat());
            rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.hackerRechargeTasksNumber.GetFloat());
            chargesVitals = Mathf.RoundToInt(CustomOptionHolder.hackerToolsNumber.GetFloat()) / 2;
            chargesAdminTable = Mathf.RoundToInt(CustomOptionHolder.hackerToolsNumber.GetFloat()) / 2;
            cantMove = CustomOptionHolder.hackerNoMove.GetBool();
        }
    }

    public static class Tracker 
    {
        public static PlayerControl Player;
        public static Color Color = new Color32(100, 58, 220, byte.MaxValue);
        public static List<Arrow> localArrows = new();

        public static float updateIntervall = 5f;
        public static bool resetTargetAfterMeeting = false;
        public static bool canTrackCorpses = false;
        public static float corpsesTrackingCooldown = 30f;
        public static float corpsesTrackingDuration = 5f;
        public static float corpsesTrackingTimer = 0f;
        public static int trackingMode = 0;
        public static List<Vector3> deadBodyPositions = new();
        public static PlayerControl CurrentTarget;
        public static PlayerControl tracked;
        public static bool usedTracker = false;
        public static float timeUntilUpdate = 0f;
        public static Arrow arrow = new(Color.blue);

        public static GameObject DangerMeterParent;
        public static DangerMeter Meter;

        private static Sprite trackCorpsesButtonSprite;
        public static Sprite GetTrackCorpsesButtonSprite()
        {
            if (trackCorpsesButtonSprite) return trackCorpsesButtonSprite;
            trackCorpsesButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.PathfindButton.png", 115f);
            return trackCorpsesButtonSprite;
        }

        private static Sprite buttonSprite;
        public static Sprite GetButtonSprite() 
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.TrackerButton.png", 115f);
            return buttonSprite;
        }

        public static void ResetTracked() 
        {
            CurrentTarget = tracked = null;
            usedTracker = false;
            if (arrow?.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
            arrow = new Arrow(Color.blue);
            if (arrow.arrow != null) arrow.arrow.SetActive(false);
        }

        public static void ClearAndReload() 
        {
            Player = null;
            ResetTracked();
            timeUntilUpdate = 0f;
            updateIntervall = CustomOptionHolder.trackerUpdateIntervall.GetFloat();
            resetTargetAfterMeeting = CustomOptionHolder.trackerResetTargetAfterMeeting.GetBool();
            if (localArrows != null) 
            {
                foreach (Arrow arrow in localArrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            }
            deadBodyPositions = new List<Vector3>();
            corpsesTrackingTimer = 0f;
            corpsesTrackingCooldown = CustomOptionHolder.trackerCorpsesTrackingCooldown.GetFloat();
            corpsesTrackingDuration = CustomOptionHolder.trackerCorpsesTrackingDuration.GetFloat();
            canTrackCorpses = CustomOptionHolder.trackerCanTrackCorpses.GetBool();
            trackingMode = CustomOptionHolder.trackerTrackingMethod.GetSelection();
            if (DangerMeterParent) 
            {
                Meter.gameObject.Destroy();
                DangerMeterParent.Destroy();
            }
        }
    }

    public static class Vampire 
    {
        public static PlayerControl Player;
        public static Color Color = Palette.ImpostorRed;

        public static float delay = 10f;
        public static float Cooldown = 30f;
        public static bool canKillNearGarlics = true;
        public static bool localPlacedGarlic = false;
        public static bool garlicsActive = true;

        public static PlayerControl CurrentTarget;
        public static PlayerControl bitten; 
        public static bool targetNearGarlic = false;

        private static Sprite buttonSprite;
        public static Sprite GetButtonSprite() 
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.VampireButton.png", 115f);
            return buttonSprite;
        }

        private static Sprite garlicButtonSprite;
        public static Sprite GetGarlicButtonSprite() 
        {
            if (garlicButtonSprite) return garlicButtonSprite;
            garlicButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.GarlicButton.png", 115f);
            return garlicButtonSprite;
        }

        public static void ClearAndReload() 
        {
            Player = null;
            bitten = null;
            targetNearGarlic = false;
            localPlacedGarlic = false;
            CurrentTarget = null;
            garlicsActive = CustomOptionHolder.vampireSpawnRate.GetSelection() > 0;
            delay = CustomOptionHolder.vampireKillDelay.GetFloat();
            Cooldown = CustomOptionHolder.vampireCooldown.GetFloat();
            canKillNearGarlics = CustomOptionHolder.vampireCanKillNearGarlics.GetBool();
        }
    }

    public static class Snitch 
    {
        public static PlayerControl Player;
        public static Color Color = new Color32(184, 251, 79, byte.MaxValue);
        public enum Mode 
        {
            Chat = 0,
            Map = 1,
            ChatAndMap = 2
        }
        public enum Targets 
        {
            EvilPlayers = 0,
            Killers = 1
        }

        public static Mode mode = Mode.Chat;
        public static Targets targets = Targets.EvilPlayers;
        public static int taskCountForReveal = 1;

        public static bool isRevealed = false;
        public static Dictionary<byte, byte> playerRoomMap = new Dictionary<byte, byte>();
        public static TMPro.TextMeshPro text = null;
        public static bool needsUpdate = true;

        public static void ClearAndReload() 
        {
            taskCountForReveal = Mathf.RoundToInt(CustomOptionHolder.snitchLeftTasksForReveal.GetFloat());
            Player = null;
            isRevealed = false;
            playerRoomMap = new Dictionary<byte, byte>();
            if (text != null) UnityEngine.Object.Destroy(text);
            text = null;
            needsUpdate = true;
            mode = (Mode) CustomOptionHolder.snitchMode.GetSelection();
            targets = (Targets) CustomOptionHolder.snitchTargets.GetSelection();
        }
    }
    

    public static class Veteran
    {
        public static PlayerControl Player;
        public static Color Color = new Color32(153, 128, 64, byte.MaxValue);
        public static float Cooldown;
        public static float Duration;
        public static bool AlertActive;
        public static int Charges;
        public static int rechargeTasksNumber;
        public static int rechargedTasks;
        private static Sprite ButtonSprite;
        public static Sprite GetButtonSprite() 
        {
            if (ButtonSprite) return ButtonSprite;
            ButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Alert.png", 115f);
            return ButtonSprite;
        }
        public static void ClearAndReload()
        {
            Player = null;
            Cooldown = CustomOptionHolder.VeteranCooldown.GetFloat();
            Duration = CustomOptionHolder.VeteranDuration.GetFloat();
            AlertActive = false;
            Charges = Mathf.RoundToInt(CustomOptionHolder.VeteranCharges.GetFloat());
            rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.VeteranRechargeTasksNumber.GetFloat());
            rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.VeteranRechargeTasksNumber.GetFloat());
        }
    }

    public static class Jackal {
        public static PlayerControl Player;
        public static Color Color = new Color32(0, 180, 235, byte.MaxValue);
        public static PlayerControl fakeSidekick;
        public static PlayerControl CurrentTarget;
        public static List<PlayerControl> formerJackals = new List<PlayerControl>();
        
        public static float Cooldown = 30f;
        public static float createSidekickCooldown = 30f;
        public static bool canUseVents = true;
        public static bool canCreateSidekick = true;
        public static Sprite buttonSprite;
        public static bool jackalPromotedFromSidekickCanCreateSidekick = true;
        public static bool canCreateSidekickFromImpostor = true;
        public static bool wasTeamRed;
        public static bool wasImpostor;
        public static bool wasSpy;
        public static bool canSabotageLights;

        public static Sprite getSidekickButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.SidekickButton.png", 115f);
            return buttonSprite;
        }

        public static void removeCurrentJackal() {
            if (!formerJackals.Any(x => x.PlayerId == Player.PlayerId)) formerJackals.Add(Player);
            Player = null;
            CurrentTarget = null;
            fakeSidekick = null;
            Cooldown = CustomOptionHolder.jackalKillCooldown.GetFloat();
            createSidekickCooldown = CustomOptionHolder.jackalCreateSidekickCooldown.GetFloat();
        }

        public static void ClearAndReload() 
        {
            Player = null;
            CurrentTarget = null;
            fakeSidekick = null;
            Cooldown = CustomOptionHolder.jackalKillCooldown.GetFloat();
            createSidekickCooldown = CustomOptionHolder.jackalCreateSidekickCooldown.GetFloat();
            canUseVents = CustomOptionHolder.jackalCanUseVents.GetBool();
            canCreateSidekick = CustomOptionHolder.jackalCanCreateSidekick.GetBool();
            jackalPromotedFromSidekickCanCreateSidekick = CustomOptionHolder.jackalPromotedFromSidekickCanCreateSidekick.GetBool();
            canCreateSidekickFromImpostor = CustomOptionHolder.jackalCanCreateSidekickFromImpostor.GetBool();
            formerJackals.Clear();
            wasTeamRed = wasImpostor = wasSpy = false;
            canSabotageLights = CustomOptionHolder.jackalCanSabotageLights.GetBool();
        }
        
    }

    public static class Pestilence
    {
        public static PlayerControl Player;
        public static Color Color = new Color32(66, 66, 66, byte.MaxValue);
        public static PlayerControl CurrentTarget;
        public static float Cooldown = 30f;
        public static bool CanUseVents;
        public static void ClearAndReload()
        {
            Player = null;
            Cooldown = CustomOptionHolder.PestilenceCooldown.GetFloat();
            CurrentTarget = null;
            CanUseVents = CustomOptionHolder.PestilenceCanUseVents.GetBool();
        }
    }

    public static class Plaguebearer
    {
        public static PlayerControl Player;
        public static PlayerControl InfectTarget;
        public static Color Color = new Color32(230, 255, 179, byte.MaxValue);
        public static PlayerControl CurrentTarget;
        public static float Cooldown = 30f;
        public static List<byte> InfectedPlayers = new List<byte>();
        public static bool CanTransform()
        {
            var alivePlayerIds = PlayerControl.AllPlayerControls
                 .ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x != Player) // Get all alive players except PB
                .Select(x => x.PlayerId) // Convert to Player IDs
                .ToList();

            return alivePlayerIds.All(playerId => InfectedPlayers.Contains(playerId)); // Compare IDs
        }
        private static Sprite ButtonSprite;
        public static Sprite GetButtonSprite() 
        {
            if (ButtonSprite) return ButtonSprite;
            ButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Infect.png", 115f);
            return ButtonSprite;
        }
        public static bool IsInfected(PlayerControl target) => InfectedPlayers.Contains(target.PlayerId);
        public static void ClearAndReload()
        {
            Player = null;
            Cooldown = CustomOptionHolder.PlaguebearerCooldown.GetFloat();
            CurrentTarget = null;
            InfectedPlayers = new List<byte>();
            InfectTarget = null;
        }
    }

    public static class Sidekick 
    {
        public static PlayerControl Player;
        public static Color Color = new Color32(0, 180, 235, byte.MaxValue);

        public static PlayerControl CurrentTarget;

        public static bool wasTeamRed;
        public static bool wasImpostor;
        public static bool wasSpy;

        public static float Cooldown = 30f;
        public static bool canUseVents = true;
        public static bool canKill = true;
        public static bool promotesToJackal = true;
        public static bool canSabotageLights;

        public static void ClearAndReload() 
        {
            Player = null;
            CurrentTarget = null;
            Cooldown = CustomOptionHolder.jackalKillCooldown.GetFloat();
            canUseVents = CustomOptionHolder.sidekickCanUseVents.GetBool();
            canKill = CustomOptionHolder.sidekickCanKill.GetBool();
            promotesToJackal = CustomOptionHolder.sidekickPromotesToJackal.GetBool();
            wasTeamRed = wasImpostor = wasSpy = false;
            canSabotageLights = CustomOptionHolder.sidekickCanSabotageLights.GetBool();
        }
    }

    public static class Eraser 
    {
        public static PlayerControl Player;
        public static Color Color = Palette.ImpostorRed;

        public static List<byte> alreadyErased = new List<byte>();

        public static List<PlayerControl> futureErased = new List<PlayerControl>();
        public static PlayerControl CurrentTarget;
        public static float Cooldown = 30f;
        public static bool canEraseAnyone = false; 

        private static Sprite ButtonSprite;
        public static Sprite GetButtonSprite() 
        {
            if (ButtonSprite) return ButtonSprite;
            ButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.EraserButton.png", 115f);
            return ButtonSprite;
        }

        public static void ClearAndReload() 
        {
            Player = null;
            futureErased = new List<PlayerControl>();
            CurrentTarget = null;
            Cooldown = CustomOptionHolder.eraserCooldown.GetFloat();
            canEraseAnyone = CustomOptionHolder.eraserCanEraseAnyone.GetBool();
            alreadyErased = new List<byte>();
        }
    }
    
    public static class Spy 
    {
        public static PlayerControl Player;
        public static Color Color = Palette.ImpostorRed;

        public static bool impostorsCanKillAnyone = true;
        public static bool canEnterVents = false;
        public static bool hasImpostorVision = false;

        public static void ClearAndReload() 
        {
            Player = null;
            impostorsCanKillAnyone = CustomOptionHolder.spyImpostorsCanKillAnyone.GetBool();
            canEnterVents = CustomOptionHolder.spyCanEnterVents.GetBool();
            hasImpostorVision = CustomOptionHolder.spyHasImpostorVision.GetBool();
        }
    }

    public static class Trickster 
    {
        public static PlayerControl Player;
        public static Color Color = Palette.ImpostorRed;
        public static float placeBoxCooldown = 30f;
        public static float lightsOutCooldown = 30f;
        public static float lightsOutDuration = 10f;
        public static float lightsOutTimer = 0f;

        private static Sprite placeBoxButtonSprite;
        private static Sprite lightOutButtonSprite;
        private static Sprite tricksterVentButtonSprite;

        public static Sprite GetPlaceBoxButtonSprite() 
        {
            if (placeBoxButtonSprite) return placeBoxButtonSprite;
            placeBoxButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.PlaceJackInTheBoxButton.png", 115f);
            return placeBoxButtonSprite;
        }

        public static Sprite GetLightsOutButtonSprite() 
        {
            if (lightOutButtonSprite) return lightOutButtonSprite;
            lightOutButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.LightsOutButton.png", 115f);
            return lightOutButtonSprite;
        }

        public static Sprite GetTricksterVentButtonSprite() 
        {
            if (tricksterVentButtonSprite) return tricksterVentButtonSprite;
            tricksterVentButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.TricksterVentButton.png", 115f);
            return tricksterVentButtonSprite;
        }

        public static void ClearAndReload() 
        {
            Player = null;
            lightsOutTimer = 0f;
            placeBoxCooldown = CustomOptionHolder.tricksterPlaceBoxCooldown.GetFloat();
            lightsOutCooldown = CustomOptionHolder.tricksterLightsOutCooldown.GetFloat();
            lightsOutDuration = CustomOptionHolder.tricksterLightsOutDuration.GetFloat();
            JackInTheBox.UpdateStates(); // if the role is erased, we might have to update the state of the created objects
        }

    }

    public static class Cleaner 
    {
        public static PlayerControl Player;
        public static Color Color = Palette.ImpostorRed;

        public static float Cooldown = 30f;

        private static Sprite buttonSprite;
        public static Sprite GetButtonSprite() 
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.CleanButton.png", 115f);
            return buttonSprite;
        }

        public static void ClearAndReload() 
        {
            Player = null;
            Cooldown = CustomOptionHolder.cleanerCooldown.GetFloat();
        }
    }

    public static class Warlock 
    {

        public static PlayerControl Player;
        public static Color Color = Palette.ImpostorRed;

        public static PlayerControl CurrentTarget;
        public static PlayerControl curseVictim;
        public static PlayerControl curseVictimTarget;

        public static float Cooldown = 30f;
        public static float rootTime = 5f;

        private static Sprite curseButtonSprite;
        private static Sprite curseKillButtonSprite;

        public static Sprite GetCurseButtonSprite() 
        {
            if (curseButtonSprite) return curseButtonSprite;
            curseButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.CurseButton.png", 115f);
            return curseButtonSprite;
        }

        public static Sprite GetCurseKillButtonSprite() 
        {
            if (curseKillButtonSprite) return curseKillButtonSprite;
            curseKillButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.CurseKillButton.png", 115f);
            return curseKillButtonSprite;
        }

        public static void ClearAndReload() 
        {
            Player = null;
            CurrentTarget = null;
            curseVictim = null;
            curseVictimTarget = null;
            Cooldown = CustomOptionHolder.warlockCooldown.GetFloat();
            rootTime = CustomOptionHolder.warlockRootTime.GetFloat();
        }

        public static void ResetCurse() 
        {
            HudManagerStartPatch.warlockCurseButton.Timer = HudManagerStartPatch.warlockCurseButton.MaxTimer;
            HudManagerStartPatch.warlockCurseButton.Sprite = Warlock.GetCurseButtonSprite();
            HudManagerStartPatch.warlockCurseButton.actionButton.cooldownTimerText.color = Palette.EnabledColor;
            CurrentTarget = null;
            curseVictim = null;
            curseVictimTarget = null;
        }
    }

    public static class Vigilante 
    {
        public static PlayerControl Player;
        public static Color Color = new Color32(195, 178, 95, byte.MaxValue);

        public static float Cooldown = 30f;
        public static int remainingScrews = 7;
        public static int totalScrews = 7;
        public static int ventPrice = 1;
        public static int camPrice = 2;
        public static int placedCameras = 0;
        public static float Duration = 10f;
        public static int maxCharges = 5;
        public static int rechargeTasksNumber = 3;
        public static int rechargedTasks = 3;
        public static int charges = 1;
        public static bool cantMove = true;
        public static Vent ventTarget = null;
        public static Minigame minigame = null;

        private static Sprite closeVentButtonSprite;
        public static Sprite GetCloseVentButtonSprite() 
        {
            if (closeVentButtonSprite) return closeVentButtonSprite;
            closeVentButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.CloseVentButton.png", 115f);
            return closeVentButtonSprite;
        }

        private static Sprite placeCameraButtonSprite;
        public static Sprite GetPlaceCameraButtonSprite() 
        {
            if (placeCameraButtonSprite) return placeCameraButtonSprite;
            placeCameraButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.PlaceCameraButton.png", 115f);
            return placeCameraButtonSprite;
        }

        private static Sprite animatedVentSealedSprite;
        private static float lastPPU;
        public static Sprite getAnimatedVentSealedSprite() 
        {
            float ppu = 185f;
            if (SubmergedCompatibility.IsSubmerged) ppu = 120f;
            if (lastPPU != ppu) 
            {
                animatedVentSealedSprite = null;
                lastPPU = ppu;
            }
            if (animatedVentSealedSprite) return animatedVentSealedSprite;
            animatedVentSealedSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.AnimatedVentSealed.png", ppu);
            return animatedVentSealedSprite;
        }

        private static Sprite staticVentSealedSprite;
        public static Sprite GetStaticVentSealedSprite() 
        {
            if (staticVentSealedSprite) return staticVentSealedSprite;
            staticVentSealedSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.StaticVentSealed.png", 160f);
            return staticVentSealedSprite;
        }

        private static Sprite fungleVentSealedSprite;
        public static Sprite GetFungleVentSealedSprite() 
        {
            if (fungleVentSealedSprite) return fungleVentSealedSprite;
            fungleVentSealedSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.FungleVentSealed.png", 160f);
            return fungleVentSealedSprite;
        }


        private static Sprite submergedCentralUpperVentSealedSprite;
        public static Sprite GetSubmergedCentralUpperSealedSprite() 
        {
            if (submergedCentralUpperVentSealedSprite) return submergedCentralUpperVentSealedSprite;
            submergedCentralUpperVentSealedSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.CentralUpperBlocked.png", 145f);
            return submergedCentralUpperVentSealedSprite;
        }

        private static Sprite submergedCentralLowerVentSealedSprite;
        public static Sprite GetSubmergedCentralLowerSealedSprite() 
        {
            if (submergedCentralLowerVentSealedSprite) return submergedCentralLowerVentSealedSprite;
            submergedCentralLowerVentSealedSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.CentralLowerBlocked.png", 145f);
            return submergedCentralLowerVentSealedSprite;
        }

        private static Sprite camSprite;
        public static Sprite GetCamSprite() 
        {
            if (camSprite) return camSprite;
            camSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.CamsButton].Image;
            return camSprite;
        }

        private static Sprite logSprite;
        public static Sprite GetLogSprite() 
        {
            if (logSprite) return logSprite;
            logSprite = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image;
            return logSprite;
        }

        public static void ClearAndReload() 
        {
            Player = null;
            ventTarget = null;
            minigame = null;
            Duration = CustomOptionHolder.VigilanteCamDuration.GetFloat();
            maxCharges = Mathf.RoundToInt(CustomOptionHolder.VigilanteCamMaxCharges.GetFloat());
            rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.VigilanteCamRechargeTasksNumber.GetFloat());
            rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.VigilanteCamRechargeTasksNumber.GetFloat());
            charges = Mathf.RoundToInt(CustomOptionHolder.VigilanteCamMaxCharges.GetFloat()) /2;
            placedCameras = 0;
            Cooldown = CustomOptionHolder.VigilanteCooldown.GetFloat();
            totalScrews = remainingScrews = Mathf.RoundToInt(CustomOptionHolder.VigilanteTotalScrews.GetFloat());
            camPrice = Mathf.RoundToInt(CustomOptionHolder.VigilanteCamPrice.GetFloat());
            ventPrice = Mathf.RoundToInt(CustomOptionHolder.VigilanteVentPrice.GetFloat());
            cantMove = CustomOptionHolder.VigilanteNoMove.GetBool();
        }
    }

    public static class Arsonist 
    {
        public static PlayerControl Player;
        public static Color Color = new Color32(238, 112, 46, byte.MaxValue);

        public static float Cooldown = 30f;
        public static float Duration = 3f;
        public static bool IsArsonistWin = false;

        public static PlayerControl CurrentTarget;
        public static PlayerControl douseTarget;
        public static List<PlayerControl> dousedPlayers = new List<PlayerControl>();

        private static Sprite douseSprite;
        public static Sprite GetDouseSprite() 
        {
            if (douseSprite) return douseSprite;
            douseSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.DouseButton.png", 115f);
            return douseSprite;
        }

        private static Sprite igniteSprite;
        public static Sprite GetIgniteSprite() 
        {
            if (igniteSprite) return igniteSprite;
            igniteSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.IgniteButton.png", 115f);
            return igniteSprite;
        }

        public static bool DousedEveryoneAlive() 
        {
            return PlayerControl.AllPlayerControls.ToArray().All(x => { return x == Arsonist.Player || x.Data.IsDead || x.Data.Disconnected || Arsonist.dousedPlayers.Any(y => y.PlayerId == x.PlayerId); });
        }

        public static void ClearAndReload() 
        {
            Player = null;
            CurrentTarget = null;
            douseTarget = null; 
            IsArsonistWin = false;
            dousedPlayers = new List<PlayerControl>();
            foreach (PoolablePlayer p in MapOptions.BeanIcons.Values) 
            {
                if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
            }
            Cooldown = CustomOptionHolder.arsonistCooldown.GetFloat();
            Duration = CustomOptionHolder.arsonistDuration.GetFloat();
        }
    }

    public static class BountyHunter 
    {
        public static PlayerControl Player;
        public static Color Color = Palette.ImpostorRed;

        public static Arrow arrow;
        public static float bountyDuration = 30f;
        public static bool showArrow = true;
        public static float bountyKillCooldown = 0f;
        public static float punishmentTime = 15f;
        public static float arrowUpdateIntervall = 10f;

        public static float arrowUpdateTimer = 0f;
        public static float bountyUpdateTimer = 0f;
        public static PlayerControl bounty;
        public static TMPro.TextMeshPro cooldownText;

        public static void ClearAndReload() 
        {
            arrow = new Arrow(Color);
            Player = null;
            bounty = null;
            arrowUpdateTimer = 0f;
            bountyUpdateTimer = 0f;
            if (arrow != null && arrow.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
            arrow = null;
            if (cooldownText != null && cooldownText.gameObject != null) UnityEngine.Object.Destroy(cooldownText.gameObject);
            cooldownText = null;
            foreach (PoolablePlayer p in MapOptions.BeanIcons.Values) 
            {
                if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
            }


            bountyDuration = CustomOptionHolder.bountyHunterBountyDuration.GetFloat();
            bountyKillCooldown = CustomOptionHolder.bountyHunterReducedCooldown.GetFloat();
            punishmentTime = CustomOptionHolder.bountyHunterPunishmentTime.GetFloat();
            showArrow = CustomOptionHolder.bountyHunterShowArrow.GetBool();
            arrowUpdateIntervall = CustomOptionHolder.bountyHunterArrowUpdateIntervall.GetFloat();
        }
    }

    public static class Vulture 
    {
        public static PlayerControl Player;
        public static Color Color = new Color32(139, 69, 19, byte.MaxValue);
        public static List<Arrow> localArrows = new List<Arrow>();
        public static float Cooldown = 30f;
        public static int vultureNumberToWin = 4;
        public static int eatenBodies = 0;
        public static bool IsVultureWin = false;
        public static bool canUseVents = true;
        public static bool showArrows = true;
        private static Sprite buttonSprite;
        public static Sprite GetButtonSprite() 
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.VultureButton.png", 115f);
            return buttonSprite;
        }

        public static void ClearAndReload() 
        {
            Player = null;
            vultureNumberToWin = Mathf.RoundToInt(CustomOptionHolder.vultureNumberToWin.GetFloat());
            eatenBodies = 0;
            Cooldown = CustomOptionHolder.vultureCooldown.GetFloat();
            IsVultureWin = false;
            canUseVents = CustomOptionHolder.vultureCanUseVents.GetBool();
            showArrows = CustomOptionHolder.vultureShowArrows.GetBool();
            if (localArrows != null) 
            {
                foreach (Arrow arrow in localArrows)
                    if (arrow?.arrow != null)
                        UnityEngine.Object.Destroy(arrow.arrow);
            }
            localArrows = new List<Arrow>();
        }
    }

    public static class Juggernaut
    {
        public static PlayerControl Player;
        public static PlayerControl CurrentTarget;
        public static float Cooldown = 30f;
        public static bool CanUseVents;
        public static Color Color = new Color32(140, 0, 77, byte.MaxValue);
        public static float ReducedCooldown = 5f;
        public static void FixCooldown()
        {
            Cooldown -= ReducedCooldown;
            if (Cooldown <= 0f) Cooldown = 0f;
        }
        public static void ClearAndReload()
        {
            Player = null;
            CurrentTarget = null;
            Cooldown = CustomOptionHolder.JuggernautCooldown.GetFloat();
            CanUseVents = CustomOptionHolder.JuggernautCanUseVents.GetBool();
            ReducedCooldown = CustomOptionHolder.JuggernautReducedCooldown.GetFloat();
        }
    }

    public static class Oracle
    {
        public static Color Color = new Color32(52, 79, 235, byte.MaxValue);
        public static PlayerControl Player;
        public static PlayerControl Confessor;
        public static PlayerControl CurrentTarget;
        public static Factions RevealedFaction;
        public static float Accuracy;
        public static bool NeutralBenignShowsEvil;
        public static bool Investigated; 
        public static bool NeutralEvilShowsEvil;
        public static float Cooldown;
        private static Sprite ButtonSprite;
        public static Sprite GetButtonSprite() 
        {
            if (ButtonSprite) return ButtonSprite;
            ButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Oracle.png", 115f);
            return ButtonSprite;
        }
        public static int Charges;
        public static int rechargeTasksNumber;
        public static int rechargedTasks;
        public static string GetInfo(PlayerControl target)
        {
            string msg = "";

            var neutralEvilRoles = new List<PlayerControl> { Jester.Player, Vulture.Player, Arsonist.Player };
            if (Lawyer.isProsecutor) neutralEvilRoles.Add(Lawyer.Player);

            var neutralBenignRoles = new List<PlayerControl> { Romantic.Player, Thief.Player };
            if (!Lawyer.isProsecutor) neutralBenignRoles.Add(Lawyer.Player);

            var evilPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.IsImp() 
                || x.IsNeutralKiller() || (neutralEvilRoles.Contains(x) && NeutralEvilShowsEvil)
                || (neutralBenignRoles.Contains(x) && NeutralBenignShowsEvil)).ToList();

            var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x != Oracle.Player && x != target).ToList();

            if (target.Data.IsDead || target.Data.Disconnected)
            {
                msg = Helpers.ColorString(Color.red, "Your confessor failed to survive so you received no confession");
            }

            else if (allPlayers.Count < 2)
            {
                msg = "Too few people alive to receive a confessional";
            }
                
            if (evilPlayers.Count == 0)
            {
                msg = $"{target.Data.PlayerName} " + Helpers.ColorString(Palette.CrewmateBlue, "confesses to knowing that there are no more evil players!"); 
            }

            allPlayers.Shuffle();
            evilPlayers.Shuffle();

            var secondPlayer = allPlayers[0];
            var firstTwoEvil = false;

            foreach (var evilPlayer in evilPlayers)
            {
                if (evilPlayer == target || evilPlayer == secondPlayer) firstTwoEvil = true;
            }
                
            if (firstTwoEvil)
            {
                var thirdPlayer = allPlayers[1];
                msg = $"{target.Data.PlayerName} confesses to knowing that they, {secondPlayer.Data.PlayerName} and/or {thirdPlayer.Data.PlayerName} is evil!";
            }
            else
            {
                var thirdPlayer = evilPlayers[0];
                msg =  $"{target.Data.PlayerName} confesses to knowing that they, {secondPlayer.Data.PlayerName} and/or {thirdPlayer.Data.PlayerName} is evil!";
            }

            return msg;
        }
        public static void ClearAndReload()
        {
            Player = null;
            Confessor = null;
            RevealedFaction = Factions.Other;
            CurrentTarget = null;
            Investigated = false;
            Accuracy = CustomOptionHolder.RevealAccuracy.GetFloat();
            NeutralBenignShowsEvil = CustomOptionHolder.NeutralBenignShowsEvil.GetBool();
            NeutralEvilShowsEvil = CustomOptionHolder.NeutralEvilShowsEvil.GetBool();
            Charges = Mathf.RoundToInt(CustomOptionHolder.OracleCharges.GetFloat());
            rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.OracleRechargeTasksNumber.GetFloat());
            rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.OracleRechargeTasksNumber.GetFloat());
        }
    }


    public static class Medium {
        public static PlayerControl medium;
        public static DeadPlayer target;
        public static DeadPlayer soulTarget;
        public static Color Color = new Color32(98, 120, 115, byte.MaxValue);
        public static List<Tuple<DeadPlayer, Vector3>> deadBodies = new List<Tuple<DeadPlayer, Vector3>>();
        public static List<Tuple<DeadPlayer, Vector3>> futureDeadBodies = new List<Tuple<DeadPlayer, Vector3>>();
        public static List<SpriteRenderer> souls = new List<SpriteRenderer>();
        public static DateTime meetingStartTime = DateTime.UtcNow;

        public static float Cooldown = 30f;
        public static float Duration = 3f;
        public static bool oneTimeUse = false;
        public static float chanceAdditionalInfo = 0f;

        private static Sprite soulSprite;

        enum SpecialMediumInfo {
            SheriffSuicide,
            ThiefSuicide,
            ActiveLoverDies,
            PassiveLoverSuicide,
            LawyerKilledByClient,
            JackalKillsSidekick,
            ImpostorTeamkill,
            SubmergedO2,
            WarlockSuicide,
            BodyCleaned,
        }

        public static Sprite getSoulSprite() {
            if (soulSprite) return soulSprite;
            soulSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Soul.png", 500f);
            return soulSprite;
        }

        private static Sprite question;
        public static Sprite getQuestionSprite() {
            if (question) return question;
            question = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.MediumButton.png", 115f);
            return question;
        }

        public static void ClearAndReload() {
            medium = null;
            target = null;
            soulTarget = null;
            deadBodies = new List<Tuple<DeadPlayer, Vector3>>();
            futureDeadBodies = new List<Tuple<DeadPlayer, Vector3>>();
            souls = new List<SpriteRenderer>();
            meetingStartTime = DateTime.UtcNow;
            Cooldown = CustomOptionHolder.mediumCooldown.GetFloat();
            Duration = CustomOptionHolder.mediumDuration.GetFloat();
            oneTimeUse = CustomOptionHolder.mediumOneTimeUse.GetBool();
            chanceAdditionalInfo = CustomOptionHolder.mediumChanceAdditionalInfo.GetSelection() / 10f;
        }

        public static string GetInfo(PlayerControl target, PlayerControl killer, DeadPlayer.CustomDeathReason deathReason) 
        {
            string msg = "";

            List<SpecialMediumInfo> infos = new List<SpecialMediumInfo>();
            // collect fitting death info types.
            // suicides:
            if (killer == target) 
            {
                if ((target == Sheriff.Player) && deathReason != DeadPlayer.CustomDeathReason.LoverSuicide) infos.Add(SpecialMediumInfo.SheriffSuicide);
                if (target == Lovers.Lover1 || target == Lovers.Lover2) infos.Add(SpecialMediumInfo.PassiveLoverSuicide);
                if (target == Thief.Player && deathReason != DeadPlayer.CustomDeathReason.LoverSuicide) infos.Add(SpecialMediumInfo.ThiefSuicide);
                if (target == Warlock.Player && deathReason != DeadPlayer.CustomDeathReason.LoverSuicide) infos.Add(SpecialMediumInfo.WarlockSuicide);
            } else {
                if (target == Lovers.Lover1 || target == Lovers.Lover2) infos.Add(SpecialMediumInfo.ActiveLoverDies);
                if (target.Data.Role.IsImpostor && killer.Data.Role.IsImpostor && Thief.formerThief != killer) infos.Add(SpecialMediumInfo.ImpostorTeamkill);
            }
            if (target == Sidekick.Player && (killer == Jackal.Player || Jackal.formerJackals.Any(x => x.PlayerId == killer.PlayerId))) infos.Add(SpecialMediumInfo.JackalKillsSidekick);
            if (target == Lawyer.Player && killer == Lawyer.target) infos.Add(SpecialMediumInfo.LawyerKilledByClient);
            if (Medium.target.wasCleaned) infos.Add(SpecialMediumInfo.BodyCleaned);
            
            if (infos.Count > 0) 
            {
                var selectedInfo = infos[rnd.Next(infos.Count)];
                switch (selectedInfo) 
                {
                    case SpecialMediumInfo.SheriffSuicide:
                        msg = "Yikes, that Sheriff shot backfired.";
                        break;
                    case SpecialMediumInfo.WarlockSuicide:
                        msg = "MAYBE I cursed the person next to me and killed myself. Oops.";
                        break;
                    case SpecialMediumInfo.ThiefSuicide:
                        msg = "I tried to steal the gun from their pocket, but they were just happy to see me.";
                        break;
                    case SpecialMediumInfo.ActiveLoverDies:
                        msg = "I wanted to get out of this toxic relationship anyways.";
                        break;
                    case SpecialMediumInfo.PassiveLoverSuicide:
                        msg = "The love of my life died, thus with a kiss I die.";
                        break;
                    case SpecialMediumInfo.LawyerKilledByClient:
                        msg = "My client killed me. Do I still get paid?";
                        break;
                    case SpecialMediumInfo.JackalKillsSidekick:
                        msg = "First they sidekicked me, then they killed me. At least I don't need to do tasks anymore.";
                        break;
                    case SpecialMediumInfo.ImpostorTeamkill:
                        msg = "I guess they confused me for the Spy, is there even one?";
                        break;
                    case SpecialMediumInfo.BodyCleaned:
                        msg = "Is my dead body some kind of art now or... aaand it's gone.";
                        break;
                }
            }
            else 
            {
                int randomNumber = rnd.Next(4);
                string typeOfColor = Helpers.IsLighterColor(Medium.target.killerIfExisting) ? "lighter" : "darker";
                float timeSinceDeath = ((float)(Medium.meetingStartTime - Medium.target.timeOfDeath).TotalMilliseconds);
                var roleString = RoleInfo.GetRolesString(Medium.target.player, false);
                if (randomNumber == 0) {
                    if (!roleString.Contains("Impostor") && !roleString.Contains("Crewmate"))
                        msg = "If my role hasn't been saved, there's no " + roleString + " in the game anymore.";
                    else
                        msg = "I was a " + roleString + " without another role."; 
                } else if (randomNumber == 1) msg = "I'm not sure, but I guess a " + typeOfColor + " color killed me.";
                else if (randomNumber == 2) msg = "If I counted correctly, I died " + Math.Round(timeSinceDeath / 1000) + "s before the next meeting started.";
                else msg = "It seems like my killer is the " + RoleInfo.GetRolesString(Medium.target.killerIfExisting, false, false, true) + ".";
            }

            if (rnd.NextDouble() < chanceAdditionalInfo) 
            {
                int count = 0;
                string condition = "";
                var alivePlayersList = PlayerControl.AllPlayerControls.ToArray().Where(pc => !pc.Data.IsDead);
                switch (rnd.Next(3)) 
                {
                    case 0:
                        count = alivePlayersList.Where(pc => pc.Data.Role.IsImpostor || pc.IsNeutralKiller() || new List<RoleInfo>() { RoleInfo.sheriff, RoleInfo.veteran, RoleInfo.thief }.Contains(RoleInfo.GetRoleInfoForPlayer(pc, false).FirstOrDefault())).Count();
                        condition = "killer" + (count == 1 ? "" : "s");
                        break;
                    case 1:
                        count = alivePlayersList.Where(Helpers.IsVenter).Count();
                        condition = "player" + (count == 1 ? "" : "s") + " who can use vents";
                        break;
                    case 2:
                        count = alivePlayersList.Where(pc => Helpers.IsNeutral(pc) && pc != Thief.Player).Count();
                        condition = "player" + (count == 1 ? "" : "s") + " who " + (count == 1 ? "is" : "are") + " a passive neutral role";
                        break;
                    case 3:
                        break;               
                }
                msg += $"\nWhen you asked, {count} " + condition + (count == 1 ? " was" : " were") + " still alive";
            }

            return Medium.target.player.Data.PlayerName + "'s Soul:\n" + msg;
        }
    }

    public static class Lawyer 
    {
        public static PlayerControl Player;
        public static PlayerControl target;
        public static Color Color = new Color32(134, 153, 25, byte.MaxValue);
        public static bool IsProsecutorWin = false;
        public static bool isProsecutor = false;
        public static bool canCallEmergency = true;
        public static float vision = 1f;
        public static bool lawyerKnowsRole = false;
        public static bool targetCanBeJester = false;
        public static bool targetWasGuessed = false;

        public static void ClearAndReload(bool clearTarget = true) 
        {
            Player = null;
            if (clearTarget) {
                target = null;
                targetWasGuessed = false;
            }
            isProsecutor = false;
            IsProsecutorWin = false;
            vision = CustomOptionHolder.lawyerVision.GetFloat();
            lawyerKnowsRole = CustomOptionHolder.lawyerKnowsRole.GetBool();
            targetCanBeJester = CustomOptionHolder.lawyerTargetCanBeJester.GetBool();
            canCallEmergency = CustomOptionHolder.jesterCanCallEmergency.GetBool();
        }
    }
    public static class VengefulRomantic
    {
        public static PlayerControl Player;
        public static PlayerControl CurrentTarget;
        public static bool notAckedExiled = false;
        public static float Cooldown;
        public static bool CanUseVents;
        public static PlayerControl Lover;
        public static void ClearAndReload()
        {
            CurrentTarget = null;
            Player = null;
            Lover = null;
            notAckedExiled = false;
            Cooldown = CustomOptionHolder.VengefulRomanticCooldown.GetFloat();
            CanUseVents = CustomOptionHolder.VengefulRomanticCanUseVents.GetBool();
        }
    }
    public static class Romantic
    {
        public static PlayerControl Player;
        public static PlayerControl beloved;
        public static bool HasLover;
        public static PlayerControl CurrentTarget;
        public static Color Color = new Color32(255, 102, 204, byte.MaxValue);
        public static bool RomanticKnowsRole = false;
        private static Sprite ButtonSprite;
        public static Sprite GetButtonSprite() 
        {
            if (ButtonSprite) return ButtonSprite;
            ButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Romantic.png", 115f);
            return ButtonSprite;
        }
        public static void ClearAndReload(bool clearBeloved = true) 
        {
            Player = null;
            HasLover = false;
            if (clearBeloved) 
            {
                beloved = null;
            }
            RomanticKnowsRole = CustomOptionHolder.RomanticKnowsRole.GetBool();
        }
    }

    public static class Pursuer 
    {        
        public static PlayerControl Player;
        public static PlayerControl target;
        public static Color Color = Lawyer.Color;
        public static List<PlayerControl> blankedList = new List<PlayerControl>();
        public static int blanks = 0;
        public static Sprite blank;
        public static bool notAckedExiled = false;

        public static float Cooldown = 30f;
        public static int blanksNumber = 5;

        public static Sprite getTargetSprite() {
            if (blank) return blank;
            blank = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.PursuerButton.png", 115f);
            return blank;
        }

        public static void ClearAndReload() 
        {
            Player = null;
            target = null;
            blankedList = new List<PlayerControl>();
            blanks = 0;
            notAckedExiled = false;

            Cooldown = CustomOptionHolder.pursuerCooldown.GetFloat();
            blanksNumber = Mathf.RoundToInt(CustomOptionHolder.pursuerBlanksNumber.GetFloat());
        }
    }

    public static class Witch 
    {
        public static PlayerControl Player;
        public static Color Color = Palette.ImpostorRed;

        public static List<PlayerControl> futureSpelled = new List<PlayerControl>();
        public static PlayerControl CurrentTarget;
        public static PlayerControl spellCastingTarget;
        public static float Cooldown = 30f;
        public static float spellCastingDuration = 2f;
        public static float cooldownAddition = 10f;
        public static float currentCooldownAddition = 0f;
        public static bool canSpellAnyone = false;
        public static bool triggerBothCooldowns = true;
        public static bool witchVoteSavesTargets = true;

        private static Sprite buttonSprite;
        public static Sprite GetButtonSprite() {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.SpellButton.png", 115f);
            return buttonSprite;
        }

        private static Sprite spelledOverlaySprite;
        public static Sprite GetSpelledOverlaySprite() {
            if (spelledOverlaySprite) return spelledOverlaySprite;
            spelledOverlaySprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.SpellButtonMeeting.png", 225f);
            return spelledOverlaySprite;
        }


        public static void ClearAndReload() 
        {
            Player = null;
            futureSpelled = new List<PlayerControl>();
            CurrentTarget = spellCastingTarget = null;
            Cooldown = CustomOptionHolder.witchCooldown.GetFloat();
            cooldownAddition = CustomOptionHolder.witchAdditionalCooldown.GetFloat();
            currentCooldownAddition = 0f;
            canSpellAnyone = CustomOptionHolder.witchCanSpellAnyone.GetBool();
            spellCastingDuration = CustomOptionHolder.witchSpellCastingDuration.GetFloat();
            triggerBothCooldowns = CustomOptionHolder.witchTriggerBothCooldowns.GetBool();
            witchVoteSavesTargets = CustomOptionHolder.witchVoteSavesTargets.GetBool();
        }
    }

    public static class Ninja {
        public static PlayerControl ninja;
        public static Color Color = Palette.ImpostorRed;

        public static PlayerControl ninjaMarked;
        public static PlayerControl CurrentTarget;
        public static float Cooldown = 30f;
        public static float traceTime = 1f;
        public static bool knowsTargetLocation = false;
        public static float invisibleDuration = 5f;

        public static float invisibleTimer = 0f;
        public static bool isInvisble = false;
        private static Sprite markButtonSprite;
        private static Sprite killButtonSprite;
        public static Arrow arrow = new Arrow(Color.black);
        public static Sprite getMarkButtonSprite() {
            if (markButtonSprite) return markButtonSprite;
            markButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.NinjaMarkButton.png", 115f);
            return markButtonSprite;
        }

        public static Sprite getKillButtonSprite() {
            if (killButtonSprite) return killButtonSprite;
            killButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.NinjaAssassinateButton.png", 115f);
            return killButtonSprite;
        }

        public static void ClearAndReload() {
            ninja = null;
            CurrentTarget = ninjaMarked = null;
            Cooldown = CustomOptionHolder.ninjaCooldown.GetFloat();
            knowsTargetLocation = CustomOptionHolder.ninjaKnowsTargetLocation.GetBool();
            traceTime = CustomOptionHolder.ninjaTraceTime.GetFloat();
            invisibleDuration = CustomOptionHolder.ninjaInvisibleDuration.GetFloat();
            invisibleTimer = 0f;
            isInvisble = false;
            if (arrow?.arrow != null) UnityEngine.Object.Destroy(arrow.arrow);
            arrow = new Arrow(Color.black);
            if (arrow.arrow != null) arrow.arrow.SetActive(false);
        }
    }

    public static class Thief {
        public static PlayerControl Player;
        public static Color Color = new Color32(71, 99, 45, Byte.MaxValue);
        public static PlayerControl CurrentTarget;
        public static PlayerControl formerThief;

        public static float Cooldown = 30f;

        public static bool suicideFlag = false;  // Used as a flag for suicide

        public static bool hasImpostorVision;
        public static bool canUseVents;
        public static bool canKillSheriff;

        public static void ClearAndReload() 
        {
            Player = null;
            suicideFlag = false;
            CurrentTarget = null;
            formerThief = null;
            hasImpostorVision = CustomOptionHolder.thiefHasImpVision.GetBool();
            Cooldown = CustomOptionHolder.thiefCooldown.GetFloat();
            canUseVents = CustomOptionHolder.thiefCanUseVents.GetBool();
            canKillSheriff = CustomOptionHolder.thiefCanKillSheriff.GetBool();
        }

        public static bool IsFailedThiefKill(PlayerControl target, PlayerControl killer, RoleInfo targetRole) 
        {
            return killer == Thief.Player && !target.Data.Role.IsImpostor  && !target.IsNeutralKiller() && !new List<RoleInfo> { canKillSheriff ? RoleInfo.sheriff : null}.Contains(targetRole);
        }
    }

        public static class Trapper 
        {
        public static PlayerControl Player;
        public static Color Color = new Color32(110, 57, 105, byte.MaxValue);

        public static float Cooldown = 30f;
        public static int maxCharges = 5;
        public static int rechargeTasksNumber = 3;
        public static int rechargedTasks = 3;
        public static int charges = 1;
        public static int trapCountToReveal = 2;
        public static List<PlayerControl> playersOnMap = new List<PlayerControl>();
        public static bool anonymousMap = false;
        public static int infoType = 0; // 0 = Role, 1 = Good/Evil, 2 = Name
        public static float trapDuration = 5f; 

        private static Sprite trapButtonSprite;

        public static Sprite GetButtonSprite() 
        {
            if (trapButtonSprite) return trapButtonSprite;
            trapButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Trapper_Place_Button.png", 115f);
            return trapButtonSprite;
        }

        public static void ClearAndReload() 
        {
            Player = null;
            Cooldown = CustomOptionHolder.trapperCooldown.GetFloat();
            maxCharges = Mathf.RoundToInt(CustomOptionHolder.trapperMaxCharges.GetFloat());
            rechargeTasksNumber = Mathf.RoundToInt(CustomOptionHolder.trapperRechargeTasksNumber.GetFloat());
            rechargedTasks = Mathf.RoundToInt(CustomOptionHolder.trapperRechargeTasksNumber.GetFloat());
            charges = Mathf.RoundToInt(CustomOptionHolder.trapperMaxCharges.GetFloat()) / 2;
            trapCountToReveal = Mathf.RoundToInt(CustomOptionHolder.trapperTrapNeededTriggerToReveal.GetFloat());
            playersOnMap = new List<PlayerControl>();
            anonymousMap = CustomOptionHolder.trapperAnonymousMap.GetBool();
            infoType = CustomOptionHolder.trapperInfoType.GetSelection();
            trapDuration = CustomOptionHolder.trapperTrapDuration.GetFloat();
        }
    }

    public static class Werewolf
    {
        public static Color Color = new Color32(168, 102, 41, byte.MaxValue);
        public static PlayerControl Player;
        public static PlayerControl CurrentTarget;
        public static float Cooldown;
        public static float Radius;
        public static bool CanUseVents;
        private static Sprite buttonSprite;
        public static Sprite GetButtonSprite() 
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Maul.png", 115f);
            return buttonSprite;
        }
        public static void ClearAndReload()
        {
            CurrentTarget = null;
            Player = null;
            Cooldown = CustomOptionHolder.WerewolfCooldown.GetFloat();
            CanUseVents = CustomOptionHolder.WerewolfCanUseVents.GetBool();
            Radius = CustomOptionHolder.WerewolfMaulRadius.GetFloat();
        }
    }

    public static class Undertaker
    {
        public static PlayerControl Player;
        public static float Cooldown;
        public static DeadBody CurrentTarget;
        public static Sprite ButtonSprite;
        public static Sprite GetFirstButtonSprite()
        {
            if (ButtonSprite) return ButtonSprite;
            ButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Drag.png", 115f);
            return ButtonSprite;
        }
        public static Sprite ButtonSprite2;
        public static Sprite GetSecondButtonSprite()
        {
            if (ButtonSprite2) return ButtonSprite2;
            ButtonSprite2 = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Drop.png", 115f);
            return ButtonSprite2;
        }
        public static void ClearAndReload()
        {
            Player = null;
            CurrentTarget = null;
            Cooldown = CustomOptionHolder.UndertakerCooldown.GetFloat();
        }
    }

    public static class Yoyo 
    {
        public static PlayerControl Player = null;
        public static Color Color = Palette.ImpostorRed;

        public static float blinkDuration = 0;
        public static float markCooldown = 0;
        public static bool markStaysOverMeeting = false;
        public static bool hasAdminTable = false;
        public static float adminCooldown = 0;
        public static float SilhouetteVisibility => (silhouetteVisibility == 0 && (PlayerControl.LocalPlayer == Player || PlayerControl.LocalPlayer.Data.IsDead)) ? 0.1f : silhouetteVisibility;
        public static float silhouetteVisibility = 0;

        public static Vector3? markedLocation = null;

        private static Sprite markButtonSprite;

        public static Sprite GetMarkButtonSprite() 
        {
            if (markButtonSprite) return markButtonSprite;
            markButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.YoyoMarkButtonSprite.png", 115f);
            return markButtonSprite;
        }
        private static Sprite blinkButtonSprite;
        public static Sprite GetBlinkButtonSprite() 
        {
            if (blinkButtonSprite) return blinkButtonSprite;
            blinkButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.YoyoBlinkButtonSprite.png", 115f);
            return blinkButtonSprite;
        }

        public static void MarkLocation(Vector3 position) 
        {
            markedLocation = position;
        }

        public static void ClearAndReload() 
        {
            blinkDuration = CustomOptionHolder.yoyoBlinkDuration.GetFloat();
            markCooldown = CustomOptionHolder.yoyoMarkCooldown.GetFloat();
            markStaysOverMeeting = CustomOptionHolder.yoyoMarkStaysOverMeeting.GetBool();
            hasAdminTable = CustomOptionHolder.yoyoHasAdminTable.GetBool();
            adminCooldown = CustomOptionHolder.yoyoAdminTableCooldown.GetFloat();
            silhouetteVisibility = CustomOptionHolder.yoyoSilhouetteVisibility.GetSelection() / 10f;
            markedLocation = null;
        }
    }

    // Modifier

    public static class Sleuth
    {
        public static List<byte> Reported = new List<byte>();
        public static List<PlayerControl> Players = new List<PlayerControl>();
        public static void ClearAndReload()
        {
            Reported = new List<byte>();
            Players = new List<PlayerControl>();
        }
    }
    public static class Disperser
    {
        public static PlayerControl Player;
        public static float Cooldown = 30f;
        public static int Charges;
        public static int RechargeKillsCount;
        public static Sprite ButtonSprite;
        public static Sprite GetButtonSprite() 
        {
            if (ButtonSprite) return ButtonSprite;
            ButtonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.Disperse.png", 115f);
            return ButtonSprite;
        }
        public static void ClearAndReload()
        {
            Player = null;
            Charges = Mathf.RoundToInt(CustomOptionHolder.ModifierDisperserCharges.GetFloat());
            RechargeKillsCount = Mathf.RoundToInt(CustomOptionHolder.ModifierDisperserKillCharges.GetFloat());
            Cooldown = CustomOptionHolder.ModifierDisperserCooldown.GetFloat();
        }
    }
    public static class Bait 
    {
        public static List<PlayerControl> Players = new List<PlayerControl>();
        public static Dictionary<DeadPlayer, float> active = new Dictionary<DeadPlayer, float>();
        public static Color Color = new Color32(0, 247, 255, byte.MaxValue);

        public static float reportDelayMin = 0f;
        public static float reportDelayMax = 0f;
        public static bool showKillFlash = true;

        public static void ClearAndReload() 
        {
            Players = new List<PlayerControl>();
            active = new Dictionary<DeadPlayer, float>();
            reportDelayMin = CustomOptionHolder.modifierBaitReportDelayMin.GetFloat();
            reportDelayMax = CustomOptionHolder.modifierBaitReportDelayMax.GetFloat();
            if (reportDelayMin > reportDelayMax) reportDelayMin = reportDelayMax;
            showKillFlash = CustomOptionHolder.modifierBaitShowKillFlash.GetBool();
        }
    }

    public static class Bloody 
    {
        public static List<PlayerControl> Players = new List<PlayerControl>();
        public static Dictionary<byte, float> active = new Dictionary<byte, float>();
        public static Dictionary<byte, byte> bloodyKillerMap = new Dictionary<byte, byte>();
        public static float Duration = 5f;

        public static void ClearAndReload() 
        {
            Players = new List<PlayerControl>();
            active = new Dictionary<byte, float>();
            bloodyKillerMap = new Dictionary<byte, byte>();
            Duration = CustomOptionHolder.modifierBloodyDuration.GetFloat();
        }
    }

    public static class AntiTeleport 
    {
        public static List<PlayerControl> Players = new List<PlayerControl>();
        public static Vector3 position;

        public static void ClearAndReload() 
        {
            Players = new List<PlayerControl>();
            position = Vector3.zero;
        }

        public static void SetPosition() 
        {
            if (position == Vector3.zero) return;  // Check if this has been set, otherwise first spawn on submerged will fail
            if (Players.FindAll(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId).Count > 0) 
            {
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(position);
                if (SubmergedCompatibility.IsSubmerged) {
                    SubmergedCompatibility.ChangeFloor(position.y > -7);
                }
            }
        }
    }

    public static class Tiebreaker 
    {
        public static PlayerControl Player;

        public static bool isTiebreak = false;

        public static void ClearAndReload() 
        {
            Player = null;
            isTiebreak = false;
        }
    }

    public static class Sunglasses 
    {
        public static List<PlayerControl> Players = new List<PlayerControl>();
        public static int vision = 1;

        public static void ClearAndReload() 
        {
            Players = new List<PlayerControl>();
            vision = CustomOptionHolder.modifierSunglassesVision.GetSelection() + 1;
        }
    }
    public static class Mini 
    {
        public static PlayerControl Player;
        public static Color Color = Color.yellow;
        public const float defaultColliderRadius = 0.2233912f;
        public const float defaultColliderOffset = 0.3636057f;

        public static float growingUpDuration = 400f;
        public static bool isGrowingUpInMeeting = true;
        public static DateTime timeOfGrowthStart = DateTime.UtcNow;
        public static DateTime timeOfMeetingStart = DateTime.UtcNow;
        public static float ageOnMeetingStart = 0f;
        public static bool IsMiniLose = false;

        public static void ClearAndReload() 
        {
            Player = null;
            IsMiniLose = false;
            growingUpDuration = CustomOptionHolder.modifierMiniGrowingUpDuration.GetFloat();
            isGrowingUpInMeeting = CustomOptionHolder.modifierMiniGrowingUpInMeeting.GetBool();
            timeOfGrowthStart = DateTime.UtcNow;
        }

        public static float GrowingProgress() 
        {
            float timeSinceStart = (float)(DateTime.UtcNow - timeOfGrowthStart).TotalMilliseconds;
            return Mathf.Clamp(timeSinceStart / (growingUpDuration * 1000), 0f, 1f);
        }

        public static bool IsGrownUp => GrowingProgress() == 1f;
    }
    public static class Vip 
    {
        public static List<PlayerControl> Players = new List<PlayerControl>();
        public static bool showColor = true;

        public static void ClearAndReload() 
        {
            Players = new List<PlayerControl>();
            showColor = CustomOptionHolder.modifierVipShowColor.GetBool();
        }
    }

    public static class Invert 
    {
        public static List<PlayerControl> Players = new List<PlayerControl>();
        public static int meetings = 3;

        public static void ClearAndReload() 
        {
            Players = new List<PlayerControl>();
            meetings = (int) CustomOptionHolder.modifierInvertDuration.GetFloat();
        }
    }

    public static class Chameleon 
    {
        public static List<PlayerControl> Players = new List<PlayerControl>();
        public static float minVisibility = 0.2f;
        public static float holdDuration = 1f;
        public static float fadeDuration = 0.5f;
        public static Dictionary<byte, float> lastMoved;

        public static void ClearAndReload() 
        {
            Players = new List<PlayerControl>();
            lastMoved = new Dictionary<byte, float>();
            holdDuration = CustomOptionHolder.modifierChameleonHoldDuration.GetFloat();
            fadeDuration = CustomOptionHolder.modifierChameleonFadeDuration.GetFloat();
            minVisibility = CustomOptionHolder.modifierChameleonMinVisibility.GetSelection() / 10f;
        }

        public static float Visibility(byte playerId) 
        {
            float visibility = 1f;
            if (lastMoved != null && lastMoved.ContainsKey(playerId)) 
            {
                var tStill = Time.time - lastMoved[playerId];
                if (tStill > holdDuration) {
                    if (tStill - holdDuration > fadeDuration) visibility = minVisibility;
                    else visibility = (1 - (tStill - holdDuration) / fadeDuration) * (1 - minVisibility) + minVisibility;
                }
            }
            if (PlayerControl.LocalPlayer.Data.IsDead && visibility < 0.1f) {  // Ghosts can always see!
                visibility = 0.1f;
            }
            return visibility;
        }

        public static void Update() 
        {
            foreach (var chameleonPlayer in Players) 
            {
                if (chameleonPlayer == Ninja.ninja && Ninja.isInvisble) continue;  // Dont make Ninja visible...
                // check movement by animation
                PlayerPhysics playerPhysics = chameleonPlayer.MyPhysics;
                var currentPhysicsAnim = playerPhysics.Animations.Animator.GetCurrentAnimation();
                if (currentPhysicsAnim != playerPhysics.Animations.group.IdleAnim) {
                    lastMoved[chameleonPlayer.PlayerId] = Time.time;
                }
                // calculate and set visibility
                float visibility = Chameleon.Visibility(chameleonPlayer.PlayerId);
                float petVisibility = visibility;
                if (chameleonPlayer.Data.IsDead) {
                    visibility = 0.5f;
                    petVisibility = 1f;
                }

                try {  // Sometimes renderers are missing for weird reasons. Try catch to avoid exceptions
                    chameleonPlayer.cosmetics.currentBodySprite.BodySprite.color = chameleonPlayer.cosmetics.currentBodySprite.BodySprite.color.SetAlpha(visibility);
                    if (DataManager.Settings.Accessibility.ColorBlindMode) chameleonPlayer.cosmetics.colorBlindText.color = chameleonPlayer.cosmetics.colorBlindText.color.SetAlpha(visibility);
                    chameleonPlayer.SetHatAndVisorAlpha(visibility);
                    chameleonPlayer.cosmetics.skin.layer.color = chameleonPlayer.cosmetics.skin.layer.color.SetAlpha(visibility);
                    chameleonPlayer.cosmetics.nameText.color = chameleonPlayer.cosmetics.nameText.color.SetAlpha(visibility);
                    foreach (var rend in chameleonPlayer.cosmetics.currentPet.renderers)
                        rend.color = rend.color.SetAlpha(petVisibility);
                    foreach (var shadowRend in chameleonPlayer.cosmetics.currentPet.shadows)
                        shadowRend.color = shadowRend.color.SetAlpha(petVisibility);
                } catch { }
            }
                
        }
    }

    public static class Armored 
    {
        public static PlayerControl Player;
        
        public static bool isBrokenArmor = false;

        public static void ClearAndReload() 
        {
            Player = null;
            isBrokenArmor = false;
        }
    }

    public static class Shifter 
    {
        public static PlayerControl Player;

        public static PlayerControl futureShift;
        public static PlayerControl CurrentTarget;

        private static Sprite buttonSprite;
        public static Sprite GetButtonSprite() 
        {
            if (buttonSprite) return buttonSprite;
            buttonSprite = Helpers.LoadSpriteFromResources("TownOfSushi.Resources.ShiftButton.png", 115f);
            return buttonSprite;
        }

        public static void ShiftRole (PlayerControl player1, PlayerControl player2, bool repeat = true) 
        {
            if (Mayor.Player != null && Mayor.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Mayor.Player = player1;
            } 
            else if (Portalmaker.Player != null && Portalmaker.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Portalmaker.Player = player1;
            } 
            else if (Engineer.Player != null && Engineer.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Engineer.Player = player1;
            } 
            else if (Sheriff.Player != null && Sheriff.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Sheriff.Player = player1;
            }
            else if (Lighter.Player != null && Lighter.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Lighter.Player = player1;
            } 
            else if (Detective.Player != null && Detective.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Detective.Player = player1;
            } 
            else if (TimeMaster.Player != null && TimeMaster.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                TimeMaster.Player = player1;
            }
            else if (Mystic.Player != null && Mystic.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Mystic.Player = player1;
            }
            else if (Oracle.Player != null && Oracle.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Oracle.Player = player1;
            }
            else if (Veteran.Player != null && Veteran.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Veteran.Player = player1;
            }
            else if (Medic.Player != null && Medic.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Medic.Player = player1;
            }
            else if (Crusader.Player != null && Crusader.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Crusader.Player = player1;
            }
            else if (Swapper.Player != null && Swapper.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Swapper.Player = player1;
            } 
            else if (Mystic.Player != null && Mystic.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Mystic.Player = player1;
            } 
            else if (Hacker.Player != null && Hacker.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Hacker.Player = player1;
            } 
            else if (Tracker.Player != null && Tracker.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Tracker.Player = player1;
            } 
            else if (Snitch.Player != null && Snitch.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Snitch.Player = player1;
            } 
            else if (Spy.Player != null && Spy.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Spy.Player = player1;
            } 
            else if (Vigilante.Player != null && Vigilante.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Vigilante.Player = player1;
            } 
            else if (Medium.medium != null && Medium.medium == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Medium.medium = player1;
            } 
            else if (Pursuer.Player != null && Pursuer.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Pursuer.Player = player1;
            } 
            else if (Trapper.Player != null && Trapper.Player == player2) 
            {
                if (repeat) ShiftRole(player2, player1, false);
                Trapper.Player = player1;
            }
        }

        public static void ClearAndReload() 
        {
            Player = null;
            CurrentTarget = null;
            futureShift = null;
        }
    }
}
