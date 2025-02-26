using HarmonyLib;
using UnityEngine;
using static TownOfSushi.TownOfSushi;
using TownOfSushi.Objects;
using System.Collections.Generic;
using System.Linq;
using TownOfSushi.Utilities;
using AmongUs.GameOptions;

namespace TownOfSushi.Patches 
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    class HudManagerUpdatePatch
    {
        private static Dictionary<byte, (string name, Color color)> TagColorDict = new();
        static void ResetNameTagsAndColors() 
        {
            var localPlayer = PlayerControl.LocalPlayer;
            var myData = PlayerControl.LocalPlayer.Data;
            var amImpostor = myData.Role.IsImpostor;
            var morphTimerNotUp = Morphling.morphTimer > 0f;
            var morphTargetNotNull = Morphling.morphTarget != null;

            var glitchTimerNotUp = Glitch.MimicTimer > 0f;
            var glitchTargetNotNull = Glitch.MimicTarget != null;

            var dict = TagColorDict;
            dict.Clear();
            
            foreach (var data in GameData.Instance.AllPlayers.GetFastEnumerator())
            {
                var player = data.Object;
                string text = data.PlayerName;
                Color color;
                if (player)
                {
                    var playerName = text;
                    if (morphTimerNotUp && morphTargetNotNull && Morphling.Player == player) playerName = Morphling.morphTarget.Data.PlayerName;
                    if (glitchTimerNotUp && glitchTargetNotNull && Glitch.Player == player) playerName = Glitch.MimicTarget.Data.PlayerName;
                    var nameText = player.cosmetics.nameText;
                
                    nameText.text = Helpers.HidePlayerName(localPlayer, player) ? "" : playerName;
                    nameText.color = color = amImpostor && data.Role.IsImpostor ? Palette.ImpostorRed : Color.white;
                    nameText.color = nameText.color.SetAlpha(Chameleon.Visibility(player.PlayerId));
                }
                else
                {
                    color = Color.white;
                }
                
                
                dict.Add(data.PlayerId, (text, color));
            }
            
            if (MeetingHud.Instance != null) 
            {
                foreach (PlayerVoteArea playerVoteArea in MeetingHud.Instance.playerStates)
                {
                    var data = dict[playerVoteArea.TargetPlayerId];
                    var text = playerVoteArea.NameText;
                    text.text = data.name;
                    text.color = data.color;
                }
            }
        }

        static void SetPlayerNameColor(PlayerControl p, Color color) 
        {
            p.cosmetics.nameText.color = color.SetAlpha(Chameleon.Visibility(p.PlayerId));
            if (MeetingHud.Instance != null)
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    if (player.NameText != null && p.PlayerId == player.TargetPlayerId)
                        player.NameText.color = color;
        }

        static void UpdateOracle(MeetingHud __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    if (player == Oracle.Confessor)
                    {
                        if (Oracle.RevealedFaction == Factions.Crewmate) state.NameText.text = state.NameText.text + $" <size=60%>(<color=#00FFFFFF>{Oracle.Accuracy}% Crew</color>) </size>";
                        else if (Oracle.RevealedFaction == Factions.Impostor) state.NameText.text = state.NameText.text + $" <size=60%>(<color=#FF0000FF>{Oracle.Accuracy}% Imp</color>) </size>";
                        else state.NameText.text = state.NameText.text + $" <size=60%>(<color=#808080FF>{Oracle.Accuracy}% Neut</color>) </size>";
                    }
                }
            }
        }

        static void SetNameColors() 
        {
            var localPlayer = PlayerControl.LocalPlayer;
            var localRole = RoleInfo.GetRoleInfoForPlayer(localPlayer, false).FirstOrDefault();
            SetPlayerNameColor(localPlayer, localRole.color);
            if (Jackal.Player != null && Jackal.Player == localPlayer) 
            {
                // Jackal can see his sidekick
                SetPlayerNameColor(Jackal.Player, Jackal.color);
                if (Sidekick.Player != null) 
                {
                    SetPlayerNameColor(Sidekick.Player, Jackal.color);
                }
                if (Jackal.fakeSidekick != null) 
                {
                    SetPlayerNameColor(Jackal.fakeSidekick, Jackal.color);
                }
            }

            // No else if here, as a Lover of team Jackal needs the colors
            if (Sidekick.Player != null && Sidekick.Player == localPlayer) 
            {
                // Sidekick can see the jackal
                SetPlayerNameColor(Sidekick.Player, Sidekick.color);
                if (Jackal.Player != null) {
                    SetPlayerNameColor(Jackal.Player, Jackal.color);
                }
            }

            // No else if here, as the Impostors need the Spy name to be colored
            if (Spy.Player != null && localPlayer.Data.Role.IsImpostor) {
                SetPlayerNameColor(Spy.Player, Spy.color);
            }
            if (Sidekick.Player != null && Sidekick.wasTeamRed && localPlayer.Data.Role.IsImpostor) 
            {
                SetPlayerNameColor(Sidekick.Player, Spy.color);
            }
            if (Jackal.Player != null && Jackal.wasTeamRed && localPlayer.Data.Role.IsImpostor) 
            {
                SetPlayerNameColor(Jackal.Player, Spy.color);
            }

            // Crewmate roles with no changes: Mini
            // Impostor roles with no changes: Morphling, Camouflager, Vampire, Godfather, Eraser, Janitor, Cleaner, Warlock, BountyHunter,  Witch and Mafioso
        }

        static void SetNameTags() 
        {
            // Mafia
            if (PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Data.Role.IsImpostor) 
            {
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                    if (Godfather.Player != null && Godfather.Player == player)
                            player.cosmetics.nameText.text = player.Data.PlayerName + " (G)";
                    else if (Mafioso.mafioso != null && Mafioso.mafioso == player)
                            player.cosmetics.nameText.text = player.Data.PlayerName + " (M)";
                    else if (Janitor.Player != null && Janitor.Player == player)
                            player.cosmetics.nameText.text = player.Data.PlayerName + " (J)";
                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (Godfather.Player != null && Godfather.Player.PlayerId == player.TargetPlayerId)
                            player.NameText.text = Godfather.Player.Data.PlayerName + " (G)";
                        else if (Mafioso.mafioso != null && Mafioso.mafioso.PlayerId == player.TargetPlayerId)
                            player.NameText.text = Mafioso.mafioso.Data.PlayerName + " (M)";
                        else if (Janitor.Player != null && Janitor.Player.PlayerId == player.TargetPlayerId)
                            player.NameText.text = Janitor.Player.Data.PlayerName + " (J)";
            }

            // Lovers
            if (Lovers.Lover1 != null && Lovers.Lover2 != null && (Lovers.Lover1 == PlayerControl.LocalPlayer || Lovers.Lover2 == PlayerControl.LocalPlayer)) 
            {
                string suffix = Helpers.ColorString(Lovers.color, " ♥");
                Lovers.Lover1.cosmetics.nameText.text += suffix;
                Lovers.Lover2.cosmetics.nameText.text += suffix;

                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (Lovers.Lover1.PlayerId == player.TargetPlayerId || Lovers.Lover2.PlayerId == player.TargetPlayerId)
                            player.NameText.text += suffix;
            }

            // Lawyer or Prosecutor
            if ((Lawyer.Player != null && Lawyer.target != null && Lawyer.Player == PlayerControl.LocalPlayer)) 
            {
                Color color = Lawyer.color;
                PlayerControl target = Lawyer.target;
                string suffix = Helpers.ColorString(color, " §");
                target.cosmetics.nameText.text += suffix;

                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (player.TargetPlayerId == target.PlayerId)
                            player.NameText.text += suffix;
            }

            // Former Thief
            if (Thief.formerThief != null && (Thief.formerThief == PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Data.IsDead)) 
            {
                string suffix = Helpers.ColorString(Thief.color, " $");
                Thief.formerThief.cosmetics.nameText.text += suffix;
                if (MeetingHud.Instance != null)
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        if (player.TargetPlayerId == Thief.formerThief.PlayerId)
                            player.NameText.text += suffix;
            }

            // Display lighter / darker color for all alive players
            if (PlayerControl.LocalPlayer != null && MeetingHud.Instance != null && MapOptions.showLighterDarker) 
            {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates) {
                    var target = Helpers.PlayerById(player.TargetPlayerId);
                    if (target != null)  player.NameText.text += $" ({(Helpers.IsLighterColor(target) ? "L" : "D")})";
                }
            }

            // Add medic shield info:
            if (MeetingHud.Instance != null && Medic.Player != null && Medic.shielded != null && Medic.ShieldVisible(Medic.shielded)) 
            {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    if (player.TargetPlayerId == Medic.shielded.PlayerId) 
                    {
                        player.NameText.text = Helpers.ColorString(Medic.color, "[") + player.NameText.text + Helpers.ColorString(Medic.color, "]");
                        // player.HighlightedFX.color = Medic.color;
                        // player.HighlightedFX.enabled = true;
                    }
            }
        }

        static void UpdateShielded() 
        {
            if (Medic.shielded == null) return;

            if (Medic.shielded.Data.IsDead || Medic.Player == null || Medic.Player.Data.IsDead) 
            {
                Medic.shielded = null;
            }
            else if (Medic.Player.Data.IsDead) 
            {
                Medic.shielded.ShowFailedMurder();
            }
        }

        static void TimerUpdate() 
        {
            var dt = Time.deltaTime;
            Hacker.hackerTimer -= dt;
            Trickster.lightsOutTimer -= dt;
            Tracker.corpsesTrackingTimer -= dt;
            Ninja.invisibleTimer -= dt;
            foreach (byte key in Glitch.HackedKnows.Keys)
                Glitch.HackedKnows[key] -= dt;
        }

        public static void MiniUpdate() 
        {
            if (Mini.Player == null || Camouflager.camouflageTimer > 0f || Helpers.MushroomSabotageActive() || Mini.Player == Morphling.Player && Morphling.morphTimer > 0f || Mini.Player == Glitch.Player && Glitch.MimicTimer > 0f || Mini.Player == Ninja.ninja && Ninja.isInvisble || SurveillanceMinigamePatch.nightVisionIsActive) return;
                
            float growingProgress = Mini.GrowingProgress();
            float scale = growingProgress * 0.35f + 0.35f;
            string suffix = "";
            if (growingProgress != 1f)
                suffix = " <color=#FAD934FF>(" + Mathf.FloorToInt(growingProgress * 18) + ")</color>"; 
            if (!Mini.isGrowingUpInMeeting && MeetingHud.Instance != null && Mini.ageOnMeetingStart != 0 && !(Mini.ageOnMeetingStart >= 18))
                suffix = " <color=#FAD934FF>(" + Mini.ageOnMeetingStart + ")</color>";

            Mini.Player.cosmetics.nameText.text += suffix;
            if (MeetingHud.Instance != null) 
            {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    if (player.NameText != null && Mini.Player.PlayerId == player.TargetPlayerId)
                        player.NameText.text += suffix;
            }

            if (Morphling.Player != null && Morphling.morphTarget == Mini.Player && Morphling.morphTimer > 0f)
                Morphling.Player.cosmetics.nameText.text += suffix;
            if (Glitch.Player != null && Glitch.MimicTarget == Mini.Player && Glitch.MimicTimer > 0f)
                Glitch.Player.cosmetics.nameText.text += suffix;
        }

        static void UpdateImpostorKillButton(HudManager __instance) 
        {
            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;
            if (MeetingHud.Instance) {
                __instance.KillButton.Hide();
                return;
            }
            bool enabled = true;
            if (Vampire.Player != null && Vampire.Player == PlayerControl.LocalPlayer)
                enabled = false;
            else if (Mafioso.mafioso != null && Mafioso.mafioso == PlayerControl.LocalPlayer && Godfather.Player != null && !Godfather.Player.Data.IsDead)
                enabled = false;
            else if (Janitor.Player != null && Janitor.Player == PlayerControl.LocalPlayer)
                enabled = false;
            
            if (enabled) __instance.KillButton.Show();
            else __instance.KillButton.Hide();

            if (Glitch.HackedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId) && Glitch.HackedKnows[PlayerControl.LocalPlayer.PlayerId] > 0) __instance.KillButton.Hide();
        }

        static void UpdateReportButton(HudManager __instance) 
        {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
            if (Glitch.HackedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId) && Glitch.HackedKnows[PlayerControl.LocalPlayer.PlayerId] > 0 || MeetingHud.Instance || Helpers.TwoPlayersAlive() && MapOptions.LimitAbilities) __instance.ReportButton.Hide();
            else if (!__instance.ReportButton.isActiveAndEnabled) __instance.ReportButton.Show();
        }
         
        static void UpdateVentButton(HudManager __instance)
        {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
            if (Glitch.HackedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId) && Glitch.HackedKnows[PlayerControl.LocalPlayer.PlayerId] > 0 || MeetingHud.Instance || Helpers.TwoPlayersAlive() && MapOptions.LimitAbilities) __instance.ImpostorVentButton.Hide();
            else if (PlayerControl.LocalPlayer.RoleCanUseVents() && !__instance.ImpostorVentButton.isActiveAndEnabled) __instance.ImpostorVentButton.Show();
        }

        static void UpdateUseButton(HudManager __instance) 
        {
            if (MeetingHud.Instance) __instance.UseButton.Hide();
        }

        static void UpdateSabotageButton(HudManager __instance) 
        {
            if (MeetingHud.Instance || Helpers.TwoPlayersAlive() && MapOptions.LimitAbilities) __instance.SabotageButton.Hide();
            if (PlayerControl.LocalPlayer.Data.IsDead && CustomOptionHolder.deadImpsBlockSabotage.GetBool()) __instance.SabotageButton.Hide();
        }

        static void UpdateMapButton(HudManager __instance) 
        {
            if (Trapper.Player == null || !(PlayerControl.LocalPlayer.PlayerId == Trapper.Player.PlayerId) || __instance == null || __instance.MapButton.HeldButtonSprite == null) return;
            __instance.MapButton.HeldButtonSprite.color = Trapper.playersOnMap.Any() ? Trapper.color : Color.white;
        }

        static void Postfix(HudManager __instance)
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started || GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
            
            EventUtility.Update();

            CustomButton.HudUpdate();
            ResetNameTagsAndColors();
            SetNameColors();
            UpdateShielded();
            SetNameTags();

            // Impostors
            UpdateImpostorKillButton(__instance);

            if (Oracle.Player != null && Oracle.Player.Data.IsDead && Oracle.Confessor != null) UpdateOracle(MeetingHud.Instance);

            // Timer updates
            TimerUpdate();
            // Mini
            MiniUpdate();

            // Glitch Sabotage, Use and Vent Button Disabling
            UpdateReportButton(__instance);
            UpdateVentButton(__instance);
            // Meeting hide buttons if needed (used for the map usage, because closing the map would show buttons)
            UpdateSabotageButton(__instance);
            UpdateUseButton(__instance);
            UpdateMapButton(__instance);
            if (!MeetingHud.Instance) __instance.AbilityButton?.Update();

            // Fix dead player's pets being visible by just always updating whether the pet should be visible at all.
            foreach (PlayerControl target in PlayerControl.AllPlayerControls) 
            {
                var pet = target.GetPet();
                if (pet != null) 
                {
                    pet.Visible = (PlayerControl.LocalPlayer.Data.IsDead && target.Data.IsDead || !target.Data.IsDead) && !target.inVent;
                }
            }
        }
    }
    //Reports can't happen by clicking the corpse
    [HarmonyPatch(typeof(DeadBody), nameof(DeadBody.OnClick))]
    public static class DeadBodyOnClickUpdate
    {
        public static bool Prefix(DeadBody __instance) 
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return false;
            if (Helpers.TwoPlayersAlive() && MapOptions.LimitAbilities)  return false;
            return true;
        }
    }
}
