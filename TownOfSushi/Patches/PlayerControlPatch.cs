using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using static TownOfSushi.TownOfSushi;
using static TownOfSushi.GameHistory;
using TownOfSushi.Objects;
using TownOfSushi.Utilities;
using UnityEngine;
using AmongUs.GameOptions;
using Assets.CoreScripts;
using Sentry.Internal.Extensions;
using Reactor.Utilities.Extensions;

namespace TownOfSushi.Patches {
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class PlayerControlFixedUpdatePatch 
    {
        // Helpers

        static PlayerControl SetTarget(bool onlyCrewmates = false, bool targetPlayersInVents = false, List<PlayerControl> untargetablePlayers = null, PlayerControl targetingPlayer = null) 
        {
            PlayerControl result = null;
            float num = AmongUs.GameOptions.GameOptionsData.KillDistances[Mathf.Clamp(GameOptionsManager.Instance.currentNormalGameOptions.KillDistance, 0, 2)];
            if (!MapUtilities.CachedShipStatus) return result;
            if (targetingPlayer == null) targetingPlayer = PlayerControl.LocalPlayer;
            if (targetingPlayer.Data.IsDead) return result;

            Vector2 truePosition = targetingPlayer.GetTruePosition();
            foreach (var playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
            {
                if (!playerInfo.Disconnected && playerInfo.PlayerId != targetingPlayer.PlayerId && !playerInfo.IsDead && (!onlyCrewmates || !playerInfo.Role.IsImpostor)) {
                    PlayerControl @object = playerInfo.Object;
                    if (untargetablePlayers != null && untargetablePlayers.Any(x => x == @object)) 
                    {
                        // if that player is not targetable: skip check
                        continue;
                    }

                    if (@object && (!@object.inVent || targetPlayersInVents)) 
                    {
                        Vector2 vector = @object.GetTruePosition() - truePosition;
                        float magnitude = vector.magnitude;
                        if (magnitude <= num && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask)) {
                            result = @object;
                            num = magnitude;
                        }
                    }
                }
            }
            return result;
        }

        static void SetPlayerOutline(PlayerControl target, Color Color) 
        {
            if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) return;

            Color = Color.SetAlpha(Chameleon.Visibility(target.PlayerId));

            target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
            target.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", Color);
        }

        // Update functions

        static void SetBasePlayerOutlines() 
        {
            foreach (PlayerControl target in PlayerControl.AllPlayerControls) {
                if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) continue;

                bool isMorphedMorphling = target == Morphling.Player && Morphling.morphTarget != null && Morphling.morphTimer > 0f;
                bool isMimicGlitch = target == Glitch.Player && Glitch.MimicTarget != null && Glitch.MimicTimer > 0f;
                bool hasVisibleShield = false;
                Color Color = Medic.shieldedColor;
                if (Camouflager.CamouflageTimer <= 0f && !Helpers.MushroomSabotageActive() && Medic.ShieldVisible(target))
                    hasVisibleShield = true;

                if (Camouflager.CamouflageTimer <= 0f && !Helpers.MushroomSabotageActive() && MapOptions.FirstPlayerKilled != null && MapOptions.shieldFirstKill && ((target == MapOptions.FirstPlayerKilled && !isMorphedMorphling && !isMimicGlitch) || (isMorphedMorphling && Morphling.morphTarget == MapOptions.FirstPlayerKilled) || (isMimicGlitch && Glitch.MimicTarget == MapOptions.FirstPlayerKilled))) 
                {
                    hasVisibleShield = true;
                    Color = Color.blue;
                }

                if (PlayerControl.LocalPlayer.Data.IsDead && Armored.Player != null && target == Armored.Player && !Armored.isBrokenArmor && !hasVisibleShield) {
                    hasVisibleShield = true;
                    Color = Color.yellow;
                }

                if (hasVisibleShield) {
                target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 1f);
                target.cosmetics.currentBodySprite.BodySprite.material.SetColor("_OutlineColor", Color);
                }
                else {
                    target.cosmetics.currentBodySprite.BodySprite.material.SetFloat("_Outline", 0f);
                }                
            }
        }

        static void SetPetVisibility() 
        {
            bool localalive = PlayerControl.LocalPlayer.Data.IsDead;
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                bool playeralive = !player.Data.IsDead;
                player.cosmetics.SetPetVisible((localalive && playeralive) || !localalive);
            }
        }

        public static void BendTimeUpdate() 
        {
            if (TimeMaster.isRewinding) 
            {
                if (localPlayerPositions.Count > 0) 
                {
                    // Set position
                    var next = localPlayerPositions[0];
                    if (next.Item2 == true) 
                    {
                        // Exit current vent if necessary
                        if (PlayerControl.LocalPlayer.inVent) 
                        {
                            foreach (Vent vent in MapUtilities.CachedShipStatus.AllVents) 
                            {
                                bool canUse;
                                bool couldUse;
                                vent.CanUse(PlayerControl.LocalPlayer.Data, out canUse, out couldUse);
                                if (canUse) {
                                    PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(vent.Id);
                                    vent.SetButtons(false);
                                }
                            }
                        }
                        // Set position
                        PlayerControl.LocalPlayer.transform.position = next.Item1;
                    }
                    else if (localPlayerPositions.Any(x => x.Item2 == true)) 
                    {
                        PlayerControl.LocalPlayer.transform.position = next.Item1;
                    }
                    if (SubmergedCompatibility.IsSubmerged) 
                    {
                        SubmergedCompatibility.ChangeFloor(next.Item1.y > -7);
                    }

                    localPlayerPositions.RemoveAt(0);

                    if (localPlayerPositions.Count > 1) localPlayerPositions.RemoveAt(0); // Skip every second position to rewinde twice as fast, but never skip the last position
                }
                else {
                    TimeMaster.isRewinding = false;
                    PlayerControl.LocalPlayer.moveable = true;
                }
            }
            else 
            {
                while (localPlayerPositions.Count >= Mathf.Round(TimeMaster.rewindTime / Time.fixedDeltaTime)) localPlayerPositions.RemoveAt(localPlayerPositions.Count - 1);
                localPlayerPositions.Insert(0, new Tuple<Vector3, bool>(PlayerControl.LocalPlayer.transform.position, PlayerControl.LocalPlayer.CanMove)); // CanMove = CanMove
            }
        }

        static void MedicSetTarget() 
        {
            if (Medic.Player == null || Medic.Player != PlayerControl.LocalPlayer) return;
            Medic.CurrentTarget = SetTarget();
            if (!Medic.usedShield) SetPlayerOutline(Medic.CurrentTarget, Medic.shieldedColor);
        }

        static void CrusaderSetTarget() 
        {
            if (Crusader.Player == null || Crusader.Player != PlayerControl.LocalPlayer) return;
            Crusader.CurrentTarget = SetTarget();
            if (!Crusader.Fortified) SetPlayerOutline(Crusader.CurrentTarget, Crusader.Color);
        }

        static void RomanticSetTarget()
        {
            if (Romantic.Player == null || Romantic.Player != PlayerControl.LocalPlayer) return;
            Romantic.CurrentTarget = SetTarget();
            if (!Romantic.HasLover) SetPlayerOutline(Romantic.CurrentTarget, Romantic.Color);
        }

        static void ShifterSetTarget() 
        {
            if (Shifter.Player == null || Shifter.Player != PlayerControl.LocalPlayer) return;
            Shifter.CurrentTarget = SetTarget();
            if (Shifter.futureShift == null) SetPlayerOutline(Shifter.CurrentTarget, Color.yellow);
        }

        static void MysticSetTarget() 
        {
            if (Mystic.Player == null || Mystic.Player != PlayerControl.LocalPlayer) return;
            Mystic.CurrentTarget = SetTarget();
            SetPlayerOutline(Mystic.CurrentTarget, Mystic.Color);
        }

        static void MorphlingSetTarget() 
        {
            if (Morphling.Player == null || Morphling.Player != PlayerControl.LocalPlayer) return;
            Morphling.CurrentTarget = SetTarget();
            SetPlayerOutline(Morphling.CurrentTarget, Morphling.Color);
        }

        public static void PlaguebearerSetTarget()
        {
            if (Plaguebearer.Player == null || Plaguebearer.Player != PlayerControl.LocalPlayer) return;

            List<PlayerControl> untargetables;
            if (Plaguebearer.CurrentTarget != null)
            {
                untargetables = new();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    if (player.PlayerId != Plaguebearer.CurrentTarget.PlayerId)
                    {
                        untargetables.Add(player);
                    }
                }
            }
            else
            {
                // Convert AllPlayerControls to a List before using LINQ
                var allPlayers = PlayerControl.AllPlayerControls.ToArray().ToList(); 

                untargetables = Plaguebearer.InfectedPlayers
                    .Select(playerId => allPlayers.FirstOrDefault(p => p.PlayerId == playerId))
                    .Where(player => player != null)
                    .ToList();
            }

            Plaguebearer.CurrentTarget = SetTarget(untargetablePlayers: untargetables);
            if (Plaguebearer.CurrentTarget != null) 
                SetPlayerOutline(Plaguebearer.CurrentTarget, Plaguebearer.Color);
        }

        static void PestilenceSetTarget() 
        {
            if (Pestilence.Player == null || Pestilence.Player != PlayerControl.LocalPlayer) return;
            Pestilence.CurrentTarget = SetTarget();
            SetPlayerOutline(Pestilence.CurrentTarget, Pestilence.Color);
        }

        static void SheriffSetTarget() 
        {
            if (Sheriff.Player == null || Sheriff.Player != PlayerControl.LocalPlayer) return;
            Sheriff.CurrentTarget = SetTarget();
            SetPlayerOutline(Sheriff.CurrentTarget, Sheriff.Color);
        }

        static void GlitchSetTarget()
        {
            if (Glitch.Player == null || Glitch.Player != PlayerControl.LocalPlayer) return;
            Glitch.CurrentTarget = SetTarget();
            SetPlayerOutline(Glitch.CurrentTarget, Glitch.Color);
        }

        static void VengefulRomanticSetTarget()
        {
            if (VengefulRomantic.Player == null || VengefulRomantic.Player != PlayerControl.LocalPlayer) return;
            VengefulRomantic.CurrentTarget = SetTarget();
            SetPlayerOutline(VengefulRomantic.CurrentTarget, Romantic.Color);
        }

        static void TrackerSetTarget() 
        {
            if (Tracker.Player == null || Tracker.Player != PlayerControl.LocalPlayer) return;
            Tracker.CurrentTarget = SetTarget();
            if (!Tracker.usedTracker) SetPlayerOutline(Tracker.CurrentTarget, Tracker.Color);
        }

        static void DetectiveUpdateFootPrints() 
        {
            if (Detective.Player == null || Detective.Player != PlayerControl.LocalPlayer) return;

            Detective.timer -= Time.fixedDeltaTime;
            if (Detective.timer <= 0f) {
                Detective.timer = Detective.footprintIntervall;
                foreach (PlayerControl player in PlayerControl.AllPlayerControls) 
                {
                    if (player != null && player != PlayerControl.LocalPlayer && !player.Data.IsDead && !player.inVent) 
                    {
                        FootprintHolder.Instance.MakeFootprint(player);
                    }
                }
            }
        }

        static void VampireSetTarget() 
        {
            if (Vampire.Player == null || Vampire.Player != PlayerControl.LocalPlayer) return;

            PlayerControl target = null;
            if (Spy.Player != null || Sidekick.wasSpy || Jackal.wasSpy) 
            {
                if (Spy.impostorsCanKillAnyone) 
                {
                    target = SetTarget(false, true);
                }
                else {
                    target = SetTarget(true, true, new List<PlayerControl>() { Spy.Player, Sidekick.wasTeamRed ? Sidekick.Player : null, Jackal.wasTeamRed ? Jackal.Player : null });
                }
            }
            else {
                target = SetTarget(true, true, new List<PlayerControl>() { Sidekick.wasImpostor ? Sidekick.Player : null, Jackal.wasImpostor ? Jackal.Player : null });
            }

            bool targetNearGarlic = false;
            if (target != null) 
            {
                foreach (Garlic garlic in Garlic.garlics)
                {
                    if (Vector2.Distance(garlic.garlic.transform.position, target.transform.position) <= 1.91f) {
                        targetNearGarlic = true;
                    }
                }
            }
            Vampire.targetNearGarlic = targetNearGarlic;
            Vampire.CurrentTarget = target;
            SetPlayerOutline(Vampire.CurrentTarget, Vampire.Color);
        }

        static void JackalSetTarget() 
        {
            if (Jackal.Player == null || Jackal.Player != PlayerControl.LocalPlayer) return;
            var untargetablePlayers = new List<PlayerControl>();
            if (Jackal.canCreateSidekickFromImpostor)
            {
                // Only exclude sidekick from beeing targeted if the jackal can create sidekicks from impostors
                if (Sidekick.Player != null) untargetablePlayers.Add(Sidekick.Player);
            }
            if (Mini.Player != null && !Mini.IsGrownUp) untargetablePlayers.Add(Mini.Player); // Exclude Jackal from targeting the Mini unless it has grown up
            Jackal.CurrentTarget = SetTarget(untargetablePlayers: untargetablePlayers);
            SetPlayerOutline(Jackal.CurrentTarget, Jackal.Color);
        }

        static void JuggernautSetTarget()
        {
            if (Juggernaut.Player == null || Juggernaut.Player != PlayerControl.LocalPlayer) return;
            var untargetablePlayers = new List<PlayerControl>();
            if (Mini.Player != null && !Mini.IsGrownUp) untargetablePlayers.Add(Mini.Player); // Exclude Juggernaut from targeting the Mini unless it has grown up
            Juggernaut.CurrentTarget = SetTarget(untargetablePlayers: untargetablePlayers);
            SetPlayerOutline(Juggernaut.CurrentTarget, Juggernaut.Color);
        }

        static void SerialKillerSetTarget() 
        {
            if (SerialKiller.Player == null || SerialKiller.Player != PlayerControl.LocalPlayer) return;
            var untargetablePlayers = new List<PlayerControl>();
            if (Mini.Player != null && !Mini.IsGrownUp) untargetablePlayers.Add(Mini.Player); // Exclude Serial Killer from targeting the Mini unless it has grown up
            SerialKiller.CurrentTarget = SetTarget(untargetablePlayers: untargetablePlayers);
            SetPlayerOutline(SerialKiller.CurrentTarget, SerialKiller.Color);
        }

        static void WerewolfSetTarget() 
        {
            if (Werewolf.Player == null || Werewolf.Player != PlayerControl.LocalPlayer) return;
            var untargetablePlayers = new List<PlayerControl>();
            if (Mini.Player != null && !Mini.IsGrownUp) untargetablePlayers.Add(Mini.Player); // Exclude Werewolf from targeting the Mini unless it has grown up
            Werewolf.CurrentTarget = SetTarget(untargetablePlayers: untargetablePlayers);
            SetPlayerOutline(Werewolf.CurrentTarget, Werewolf.Color);
        }

        static void SidekickSetTarget() {
            if (Sidekick.Player == null || Sidekick.Player != PlayerControl.LocalPlayer) return;
            var untargetablePlayers = new List<PlayerControl>();
            if (Jackal.Player != null) untargetablePlayers.Add(Jackal.Player);
            if (Mini.Player != null && !Mini.IsGrownUp) untargetablePlayers.Add(Mini.Player); // Exclude Sidekick from targeting the Mini unless it has grown up
            Sidekick.CurrentTarget = SetTarget(untargetablePlayers: untargetablePlayers);
            if (Sidekick.canKill) SetPlayerOutline(Sidekick.CurrentTarget, Sidekick.Color);
        }

        static void SidekickCheckPromotion() {
            // If LocalPlayer is Sidekick, the Jackal is disconnected and Sidekick promotion is enabled, then trigger promotion
            if (Sidekick.Player == null || Sidekick.Player != PlayerControl.LocalPlayer) return;
            if (Sidekick.Player.Data.IsDead == true || !Sidekick.promotesToJackal) return;
            if (Jackal.Player == null || Jackal.Player?.Data?.Disconnected == true) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SidekickPromotes, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.SidekickPromotes();
            }
        }

        static void EraserSetTarget() {
            if (Eraser.Player == null || Eraser.Player != PlayerControl.LocalPlayer) return;

            List<PlayerControl> untargetables = new List<PlayerControl>();
            if (Spy.Player != null) untargetables.Add(Spy.Player);
            if (Sidekick.wasTeamRed) untargetables.Add(Sidekick.Player);
            if (Jackal.wasTeamRed) untargetables.Add(Jackal.Player);
            Eraser.CurrentTarget = SetTarget(onlyCrewmates: !Eraser.canEraseAnyone, untargetablePlayers: Eraser.canEraseAnyone ? new List<PlayerControl>() : untargetables);
            SetPlayerOutline(Eraser.CurrentTarget, Eraser.Color);
        }

        static void GlitchUpdate()
        {
            if (PlayerControl.LocalPlayer == null || !Glitch.HackedKnows.ContainsKey(PlayerControl.LocalPlayer.PlayerId)) return;
            
            if (Glitch.HackedKnows[PlayerControl.LocalPlayer.PlayerId] <= 0)
            {
                Glitch.HackedKnows.Remove(PlayerControl.LocalPlayer.PlayerId);
                // Resets the buttons
                Glitch.SetHackedKnows(false);
                
                // Ghost info
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write((byte)GhostInfoTypes.HackOver);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            
        }

        static void EngineerUpdate() 
        {
            bool jackalHighlight = Engineer.highlightForTeamJackal && (PlayerControl.LocalPlayer == Jackal.Player || PlayerControl.LocalPlayer == Sidekick.Player);
            bool impostorHighlight = Engineer.highlightForImpostors && PlayerControl.LocalPlayer.Data.Role.IsImpostor;
            if ((jackalHighlight || impostorHighlight) && MapUtilities.CachedShipStatus?.AllVents != null) {
                foreach (Vent vent in MapUtilities.CachedShipStatus.AllVents) {
                    try {
                        if (vent?.myRend?.material != null) {
                            if (Engineer.Player != null && Engineer.Player.inVent) {
                                vent.myRend.material.SetFloat("_Outline", 1f);
                                vent.myRend.material.SetColor("_OutlineColor", Engineer.Color);
                            }
                            else if (vent.myRend.material.GetColor("_AddColor") != Color.red) {
                                vent.myRend.material.SetFloat("_Outline", 0);
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        static void ImpostorSetTarget() 
        {
            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor ||!PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.Data.IsDead) { // !isImpostor || !canMove || isDead
                FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
                return;
            }

            PlayerControl target = null;
            if (Spy.Player != null || Sidekick.wasSpy || Jackal.wasSpy) {
                if (Spy.impostorsCanKillAnyone) {
                    target = SetTarget(false, true);
                }
                else {
                    target = SetTarget(true, true, new List<PlayerControl>() { Spy.Player, Sidekick.wasTeamRed ? Sidekick.Player : null, Jackal.wasTeamRed ? Jackal.Player : null});
                }
            }
            else {
                target = SetTarget(true, true, new List<PlayerControl>() { Sidekick.wasImpostor ? Sidekick.Player : null, Jackal.wasImpostor ? Jackal.Player : null});
            }

            FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(target); // Includes setPlayerOutline(target, Palette.ImpstorRed);
        }

        static void warlockSetTarget() {
            if (Warlock.Player == null || Warlock.Player != PlayerControl.LocalPlayer) return;
            if (Warlock.curseVictim != null && (Warlock.curseVictim.Data.Disconnected || Warlock.curseVictim.Data.IsDead)) {
                // If the cursed victim is disconnected or dead reset the curse so a new curse can be applied
                Warlock.ResetCurse();
            }
            if (Warlock.curseVictim == null) {
                Warlock.CurrentTarget = SetTarget();
                SetPlayerOutline(Warlock.CurrentTarget, Warlock.Color);
            }
            else {
                Warlock.curseVictimTarget = SetTarget(targetingPlayer: Warlock.curseVictim);
                SetPlayerOutline(Warlock.curseVictimTarget, Warlock.Color);
            }
        }

        static void ninjaUpdate()
        {
            if (Ninja.isInvisble && Ninja.invisibleTimer <= 0 && Ninja.ninja == PlayerControl.LocalPlayer)
            {
                MessageWriter invisibleWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetInvisible, Hazel.SendOption.Reliable, -1);
                invisibleWriter.Write(Ninja.ninja.PlayerId);
                invisibleWriter.Write(byte.MaxValue);
                AmongUsClient.Instance.FinishRpcImmediately(invisibleWriter);
                RPCProcedure.SetInvisible(Ninja.ninja.PlayerId, byte.MaxValue);
            }
            if (Ninja.arrow?.arrow != null)
            {
                if (Ninja.ninja == null || Ninja.ninja != PlayerControl.LocalPlayer || !Ninja.knowsTargetLocation) 
                {
                    Ninja.arrow.arrow.SetActive(false);
                    return;
                }
                if (Ninja.ninjaMarked != null && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    bool trackedOnMap = !Ninja.ninjaMarked.Data.IsDead;
                    Vector3 position = Ninja.ninjaMarked.transform.position;
                    if (!trackedOnMap)
                    { // Check for dead body
                        DeadBody body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Ninja.ninjaMarked.PlayerId);
                        if (body != null)
                        {
                            trackedOnMap = true;
                            position = body.transform.position;
                        }
                    }
                    Ninja.arrow.Update(position);
                    Ninja.arrow.arrow.SetActive(trackedOnMap);
                } 
                else
                {
                    Ninja.arrow.arrow.SetActive(false);
                }
            }
        }

        static void TrackerUpdate() 
        {
            // Handle player tracking
            if (Tracker.arrow?.arrow != null) 
            {
                if (Tracker.Player == null || PlayerControl.LocalPlayer != Tracker.Player) 
                {
                    Tracker.arrow.arrow.SetActive(false);
                    if (Tracker.DangerMeterParent) Tracker.DangerMeterParent.SetActive(false);
                    return;
                }

                if (Tracker.tracked != null && !Tracker.Player.Data.IsDead) 
                {
                    Tracker.timeUntilUpdate -= Time.fixedDeltaTime;

                    if (Tracker.timeUntilUpdate <= 0f) 
                    {
                        bool trackedOnMap = !Tracker.tracked.Data.IsDead;
                        Vector3 position = Tracker.tracked.transform.position;
                        if (!trackedOnMap) { // Check for dead body
                            DeadBody body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == Tracker.tracked.PlayerId);
                            if (body != null) 
                            {
                                trackedOnMap = true;
                                position = body.transform.position;
                            }
                        }

                        if (Tracker.trackingMode == 1 || Tracker.trackingMode == 2) Arrow.UpdateProximity(position);
                        if (Tracker.trackingMode == 0 || Tracker.trackingMode == 2) 
                        {
                            Tracker.arrow.Update(position);
                            Tracker.arrow.arrow.SetActive(trackedOnMap);
                        }
                        Tracker.timeUntilUpdate = Tracker.updateIntervall;
                    } 
                    else 
                    {
                        if (Tracker.trackingMode == 0 || Tracker.trackingMode == 2) Tracker.arrow.Update();
                    }
                } 
                else if (Tracker.Player.Data.IsDead) 
                {
                    Tracker.DangerMeterParent?.SetActive(false);
                    Tracker.Meter?.gameObject.SetActive(false);
                }
            }

            // Handle corpses tracking
            if (Tracker.Player != null && Tracker.Player == PlayerControl.LocalPlayer && Tracker.corpsesTrackingTimer >= 0f && !Tracker.Player.Data.IsDead) 
            {
                bool arrowsCountChanged = Tracker.localArrows.Count != Tracker.deadBodyPositions.Count();
                int index = 0;

                if (arrowsCountChanged) 
                {
                    foreach (Arrow arrow in Tracker.localArrows) UnityEngine.Object.Destroy(arrow.arrow);
                    Tracker.localArrows = new List<Arrow>();
                }
                foreach (Vector3 position in Tracker.deadBodyPositions) 
                {
                    if (arrowsCountChanged) {
                        Tracker.localArrows.Add(new Arrow(Tracker.Color));
                        Tracker.localArrows[index].arrow.SetActive(true);
                    }
                    if (Tracker.localArrows[index] != null) Tracker.localArrows[index].Update(position);
                    index++;
                }
            } 
            else if (Tracker.localArrows.Count > 0) 
            {
                foreach (Arrow arrow in Tracker.localArrows) UnityEngine.Object.Destroy(arrow.arrow);
                Tracker.localArrows = new List<Arrow>();
            }
        }

        public static void PlayerSizeUpdate(PlayerControl p) 
        {
            // Set default player size
            CircleCollider2D collider = p.Collider.CastFast<CircleCollider2D>();

            p.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
            collider.radius = Mini.defaultColliderRadius;
            collider.offset = Mini.defaultColliderOffset * Vector2.down;

            // Set adapted player size to Mini, Glitch and Morphling
            if (Mini.Player == null || Camouflager.CamouflageTimer > 0f || Helpers.MushroomSabotageActive()  || Mini.Player == Morphling.Player && Morphling.morphTimer > 0 || Mini.Player == Glitch.Player && Glitch.MimicTimer > 0) return;

            float growingProgress = Mini.GrowingProgress();
            float scale = growingProgress * 0.35f + 0.35f;
            float correctedColliderRadius = Mini.defaultColliderRadius * 0.7f / scale; // scale / 0.7f is the factor by which we decrease the player size, hence we need to increase the collider size by 0.7f / scale

            if (p == Mini.Player) {
                p.transform.localScale = new Vector3(scale, scale, 1f);
                collider.radius = correctedColliderRadius;
            }
            if (Morphling.Player != null && p == Morphling.Player && Morphling.morphTarget == Mini.Player && Morphling.morphTimer > 0f) {
                p.transform.localScale = new Vector3(scale, scale, 1f);
                collider.radius = correctedColliderRadius;
            }
            if (Glitch.Player != null && p == Glitch.Player && Glitch.MimicTarget == Mini.Player && Glitch.MimicTimer > 0f) {
                p.transform.localScale = new Vector3(scale, scale, 1f);
                collider.radius = correctedColliderRadius;
            }
        }

        public static void updatePlayerInfo() 
        {
            Vector3 colorBlindTextMeetingInitialLocalPos = new Vector3(0.3384f, -0.16666f, -0.01f);
            Vector3 colorBlindTextMeetingInitialLocalScale = new Vector3(0.9f, 1f, 1f);
            foreach (PlayerControl p in PlayerControl.AllPlayerControls) 
            {
                
                // Colorblind Text in Meeting
                PlayerVoteArea playerVoteArea = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == p.PlayerId);
                if (playerVoteArea != null && playerVoteArea.ColorBlindName.gameObject.active) 
                {
                    playerVoteArea.ColorBlindName.transform.localPosition = colorBlindTextMeetingInitialLocalPos + new Vector3(0f, 0.4f, 0f);
                    playerVoteArea.ColorBlindName.transform.localScale = colorBlindTextMeetingInitialLocalScale * 0.8f;
                }

                // Colorblind Text During the round
                if (p.cosmetics.colorBlindText != null && p.cosmetics.showColorBlindText && p.cosmetics.colorBlindText.gameObject.active) 
                {
                    p.cosmetics.colorBlindText.transform.localPosition = new Vector3(0, -1f, 0f);
                }

                p.cosmetics.nameText.transform.parent.SetLocalZ(-0.0001f);  // This moves both the name AND the colorblindtext behind objects (if the player is behind the object), like the rock on polus

                if ((Lawyer.lawyerKnowsRole && PlayerControl.LocalPlayer == Lawyer.Player && p == Lawyer.target) || (Romantic.RomanticKnowsRole && PlayerControl.LocalPlayer == Romantic.Player && p == Romantic.beloved) || (Romantic.RomanticKnowsRole && PlayerControl.LocalPlayer == Romantic.beloved && p == Romantic.Player) 
                || p == PlayerControl.LocalPlayer 
                || (Sleuth.Players.Any(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId) && Sleuth.Reported.Contains(p.PlayerId)) || PlayerControl.LocalPlayer.Data.IsDead) 
                {
                    Transform playerInfoTransform = p.cosmetics.nameText.transform.parent.FindChild("Info");
                    TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                    if (playerInfo == null) 
                    {
                        playerInfo = UnityEngine.Object.Instantiate(p.cosmetics.nameText, p.cosmetics.nameText.transform.parent);
                        playerInfo.transform.localPosition += Vector3.up * 0.225f;
                        playerInfo.fontSize *= 0.75f;
                        playerInfo.gameObject.name = "Info";
                        playerInfo.color = playerInfo.color.SetAlpha(1f);
                    }
    
                    Transform meetingInfoTransform = playerVoteArea != null ? playerVoteArea.NameText.transform.parent.FindChild("Info") : null;
                    TMPro.TextMeshPro meetingInfo = meetingInfoTransform != null ? meetingInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                    if (meetingInfo == null && playerVoteArea != null) 
                    {
                        meetingInfo = UnityEngine.Object.Instantiate(playerVoteArea.NameText, playerVoteArea.NameText.transform.parent);
                        meetingInfo.transform.localPosition += Vector3.down * 0.2f;
                        meetingInfo.fontSize *= 0.60f;
                        meetingInfo.gameObject.name = "Info";
                    }

                    // Set player name higher to align in middle
                    if (meetingInfo != null && playerVoteArea != null) 
                    {
                        var playerName = playerVoteArea.NameText;
                        playerName.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
                    }

                    var (tasksCompleted, tasksTotal) = TasksHandler.TaskInfo(p.Data);
                    string roleNames = RoleInfo.GetRolesString(p, true, false);
                    string roleText = RoleInfo.GetRolesString(p, true, MapOptions.ghostsSeeModifier);
                    string taskInfo = tasksTotal > 0 ? $"<color=#FAD934FF>({tasksCompleted}/{tasksTotal})</color>" : "";

                    string playerInfoText = "";
                    string meetingInfoText = "";                    
                    if (p == PlayerControl.LocalPlayer) 
                    {
                        if (p.Data.IsDead) roleNames = roleText;
                        playerInfoText = $"{roleNames}";
                        if (p == Swapper.Player) playerInfoText = $"{roleNames}" + Helpers.ColorString(Swapper.Color, $" ({Swapper.charges})");
                        if (HudManager.Instance.TaskPanel != null) 
                        {
                            TMPro.TextMeshPro tabText = HudManager.Instance.TaskPanel.tab.transform.FindChild("TabText_TMP").GetComponent<TMPro.TextMeshPro>();
                            tabText.SetText($"Tasks {taskInfo}");
                        }
                        meetingInfoText = $"{roleNames} {taskInfo}".Trim();
                    }
                    else if (MapOptions.ghostsSeeRoles && MapOptions.ghostsSeeInformation) 
                    {
                        playerInfoText = $"{roleText} {taskInfo}".Trim();
                        meetingInfoText = playerInfoText;
                    }
                    else if (MapOptions.ghostsSeeInformation) 
                    {
                        playerInfoText = $"{taskInfo}".Trim();
                        meetingInfoText = playerInfoText;
                    }
                    else if (MapOptions.ghostsSeeRoles || (Lawyer.lawyerKnowsRole && PlayerControl.LocalPlayer == Lawyer.Player && p == Lawyer.target) || (Romantic.RomanticKnowsRole && PlayerControl.LocalPlayer == Romantic.Player && p == Romantic.beloved) || (Romantic.RomanticKnowsRole && PlayerControl.LocalPlayer == Romantic.beloved && p == Romantic.Player)) 
                    {
                        playerInfoText = $"{roleText}";
                        meetingInfoText = playerInfoText;
                    }

                    playerInfo.text = playerInfoText;
                    playerInfo.gameObject.SetActive(p.Visible);
                    if (meetingInfo != null) meetingInfo.text = MeetingHud.Instance.state == MeetingHud.VoteStates.Results ? "" : meetingInfoText;
                }                
            }
        }

        public static void VigilanteSetTarget() 
        {
            if (Vigilante.Player == null || Vigilante.Player != PlayerControl.LocalPlayer || MapUtilities.CachedShipStatus == null || MapUtilities.CachedShipStatus.AllVents == null) return;

            Vent target = null;
            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            float closestDistance = float.MaxValue;
            for (int i = 0; i < MapUtilities.CachedShipStatus.AllVents.Length; i++) {
                Vent vent = MapUtilities.CachedShipStatus.AllVents[i];
                if (vent.gameObject.name.StartsWith("JackInTheBoxVent_") || vent.gameObject.name.StartsWith("SealedVent_") || vent.gameObject.name.StartsWith("FutureSealedVent_")) continue;
                if (SubmergedCompatibility.IsSubmerged && vent.Id == 9) continue; // cannot seal submergeds exit only vent!
                float distance = Vector2.Distance(vent.transform.position, truePosition);
                if (distance <= vent.UsableDistance && distance < closestDistance) {
                    closestDistance = distance;
                    target = vent;
                }
            }
            Vigilante.ventTarget = target;
        }

        public static void VigilanteUpdate() 
        {
            if (Vigilante.Player == null || PlayerControl.LocalPlayer != Vigilante.Player || Vigilante.Player.Data.IsDead) return;
            var (playerCompleted, _) = TasksHandler.TaskInfo(Vigilante.Player.Data);
            if (playerCompleted == Vigilante.rechargedTasks) {
                Vigilante.rechargedTasks += Vigilante.rechargeTasksNumber;
                if (Vigilante.maxCharges > Vigilante.charges) Vigilante.charges++;
            }
        }

        public static void ArsonistSetTarget() 
        {
            if (Arsonist.Player == null || Arsonist.Player != PlayerControl.LocalPlayer) return;
            List<PlayerControl> untargetables;
            if (Arsonist.douseTarget != null)
            {
                untargetables = new();
                foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                {
                    if (player.PlayerId != Arsonist.douseTarget.PlayerId)
                    {
                        untargetables.Add(player);
                    }
                }
            }
            else untargetables = Arsonist.dousedPlayers;
            Arsonist.CurrentTarget = SetTarget(untargetablePlayers: untargetables);
            if (Arsonist.CurrentTarget != null) SetPlayerOutline(Arsonist.CurrentTarget, Arsonist.Color);
        }

        static void SnitchUpdate() 
        {
            if (Snitch.Player == null) return;
            if (!Snitch.needsUpdate) return;

            bool snitchIsDead = Snitch.Player.Data.IsDead;
            var (playerCompleted, playerTotal) = TasksHandler.TaskInfo(Snitch.Player.Data);

            if (playerTotal == 0) return;
            PlayerControl local = PlayerControl.LocalPlayer;

            int numberOfTasks = playerTotal - playerCompleted;

            if (Snitch.isRevealed && ((Snitch.targets == Snitch.Targets.EvilPlayers && !local.IsCrew()) || (Snitch.targets == Snitch.Targets.Killers && Helpers.IsKiller(local)))) {
                if (Snitch.text == null)
                {
                    Snitch.text = GameObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.KillButton.cooldownTimerText, FastDestroyableSingleton<HudManager>.Instance.transform);
                    Snitch.text.enableWordWrapping = false;
                    Snitch.text.transform.localScale = Vector3.one * 0.75f;
                    Snitch.text.transform.localPosition += new Vector3(0f, 1.8f, -69f);
                    Snitch.text.gameObject.SetActive(true);
                } else {
                    Snitch.text.text = $"Snitch is alive: " + playerCompleted + "/" + playerTotal;
                    if (snitchIsDead) Snitch.text.text = $"Snitch is dead!";
                }
            } else if (Snitch.text != null)
                Snitch.text.Destroy();

            if (snitchIsDead) 
            {
                if (MeetingHud.Instance == null) Snitch.needsUpdate = false;
                return;
            }
            if (numberOfTasks <= Snitch.taskCountForReveal) Snitch.isRevealed = true;
        }

        static void BountyHunterUpdate() 
        {
            if (BountyHunter.Player == null || PlayerControl.LocalPlayer != BountyHunter.Player) return;

            if (BountyHunter.Player.Data.IsDead) 
            {
                if (BountyHunter.arrow != null || BountyHunter.arrow.arrow != null) UnityEngine.Object.Destroy(BountyHunter.arrow.arrow);
                BountyHunter.arrow = null;
                if (BountyHunter.cooldownText != null && BountyHunter.cooldownText.gameObject != null) UnityEngine.Object.Destroy(BountyHunter.cooldownText.gameObject);
                BountyHunter.cooldownText = null;
                BountyHunter.bounty = null;
                foreach (PoolablePlayer p in MapOptions.BeanIcons.Values) 
                {
                    if (p != null && p.gameObject != null) p.gameObject.SetActive(false);
                }
                return;
            }

            BountyHunter.arrowUpdateTimer -= Time.fixedDeltaTime;
            BountyHunter.bountyUpdateTimer -= Time.fixedDeltaTime;

            if (BountyHunter.bounty == null || BountyHunter.bountyUpdateTimer <= 0f) 
            {
                // Set new bounty
                BountyHunter.bounty = null;
                BountyHunter.arrowUpdateTimer = 0f; // Force arrow to update
                BountyHunter.bountyUpdateTimer = BountyHunter.bountyDuration;
                var possibleTargets = new List<PlayerControl>();
                foreach (PlayerControl p in PlayerControl.AllPlayerControls) 
                {
                    if (!p.Data.IsDead && !p.Data.Disconnected && p != p.Data.Role.IsImpostor && p != Spy.Player && (Romantic.beloved == BountyHunter.Player && p != Romantic.Player) && (p != Sidekick.Player || !Sidekick.wasTeamRed) && (p != Jackal.Player || !Jackal.wasTeamRed) && (p != Mini.Player || Mini.IsGrownUp) && (Lovers.GetPartner(BountyHunter.Player) == null || p != Lovers.GetPartner(BountyHunter.Player))) possibleTargets.Add(p);
                }
                BountyHunter.bounty = possibleTargets[TownOfSushi.rnd.Next(0, possibleTargets.Count)];
                if (BountyHunter.bounty == null) return;

                // Ghost Info
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write((byte)GhostInfoTypes.BountyTarget);
                writer.Write(BountyHunter.bounty.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                // Show poolable player
                if (FastDestroyableSingleton<HudManager>.Instance != null && FastDestroyableSingleton<HudManager>.Instance.UseButton != null) {
                    foreach (PoolablePlayer pp in MapOptions.BeanIcons.Values) pp.gameObject.SetActive(false);
                    if (MapOptions.BeanIcons.ContainsKey(BountyHunter.bounty.PlayerId) && MapOptions.BeanIcons[BountyHunter.bounty.PlayerId].gameObject != null)
                        MapOptions.BeanIcons[BountyHunter.bounty.PlayerId].gameObject.SetActive(true);
                }
            }

            // Hide in meeting
            if (MeetingHud.Instance && MapOptions.BeanIcons.ContainsKey(BountyHunter.bounty.PlayerId) && MapOptions.BeanIcons[BountyHunter.bounty.PlayerId].gameObject != null)
                MapOptions.BeanIcons[BountyHunter.bounty.PlayerId].gameObject.SetActive(false);

            // Update Cooldown Text
            if (BountyHunter.cooldownText != null) {
                BountyHunter.cooldownText.text = Mathf.CeilToInt(Mathf.Clamp(BountyHunter.bountyUpdateTimer, 0, BountyHunter.bountyDuration)).ToString();
                BountyHunter.cooldownText.gameObject.SetActive(!MeetingHud.Instance);  // Show if not in meeting
            }

            // Update Arrow
            if (BountyHunter.showArrow && BountyHunter.bounty != null) 
            {
                if (BountyHunter.arrow == null) BountyHunter.arrow = new Arrow(Color.red);
                if (BountyHunter.arrowUpdateTimer <= 0f) {
                    BountyHunter.arrow.Update(BountyHunter.bounty.transform.position);
                    BountyHunter.arrowUpdateTimer = BountyHunter.arrowUpdateIntervall;
                }
                BountyHunter.arrow.Update();
            }
        }

        static void AmnesiacUpdate() 
        {
            if (Amnesiac.Player == null || PlayerControl.LocalPlayer != Amnesiac.Player || Amnesiac.AmnesiacArrows == null || !Amnesiac.ShowArrows) return;
            if (Amnesiac.Player.Data.IsDead) 
            {
                foreach (Arrow arrow in Amnesiac.AmnesiacArrows) UnityEngine.Object.Destroy(arrow.arrow);
                Amnesiac.AmnesiacArrows = new List<Arrow>();
                return;
            }

            DeadBody[] deadBodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            bool arrowUpdate = Amnesiac.AmnesiacArrows.Count != deadBodies.Count();
            int index = 0;

            if (arrowUpdate) 
            {
                foreach (Arrow arrow in Amnesiac.AmnesiacArrows) UnityEngine.Object.Destroy(arrow.arrow);
                Amnesiac.AmnesiacArrows = new List<Arrow>();
            }

            foreach (DeadBody db in deadBodies) 
            {
                if (arrowUpdate) 
                {
                    Amnesiac.AmnesiacArrows.Add(new Arrow(Color.blue));
                    Amnesiac.AmnesiacArrows[index].arrow.SetActive(true);
                }
                if (Amnesiac.AmnesiacArrows[index] != null) Amnesiac.AmnesiacArrows[index].Update(db.transform.position);
                index++;
            }
        }

        static void UndertakerUpdate()
        {
            if (Undertaker.Player == null || Undertaker.Player.Data.IsDead) return;
            if (Undertaker.CurrentTarget != null)
            {
                Vector3 currentPosition = Undertaker.Player.transform.position;
                Undertaker.CurrentTarget.transform.position = currentPosition;
            }
        }

        static void VultureUpdate() 
        {
            if (Vulture.Player == null || PlayerControl.LocalPlayer != Vulture.Player || Vulture.localArrows == null || !Vulture.showArrows) return;
            if (Vulture.Player.Data.IsDead) 
            {
                foreach (Arrow arrow in Vulture.localArrows) UnityEngine.Object.Destroy(arrow.arrow);
                Vulture.localArrows = new List<Arrow>();
                return;
            }

            DeadBody[] deadBodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            bool arrowUpdate = Vulture.localArrows.Count != deadBodies.Count();
            int index = 0;

            if (arrowUpdate) {
                foreach (Arrow arrow in Vulture.localArrows) UnityEngine.Object.Destroy(arrow.arrow);
                Vulture.localArrows = new List<Arrow>();
            }

            foreach (DeadBody db in deadBodies) 
            {
                if (arrowUpdate) {
                    Vulture.localArrows.Add(new Arrow(Color.blue));
                    Vulture.localArrows[index].arrow.SetActive(true);
                }
                if (Vulture.localArrows[index] != null) Vulture.localArrows[index].Update(db.transform.position);
                index++;
            }
        }

        public static void MediumSetTarget() 
        {
            if (Medium.medium == null || Medium.medium != PlayerControl.LocalPlayer || Medium.medium.Data.IsDead || Medium.deadBodies == null || MapUtilities.CachedShipStatus?.AllVents == null) return;

            DeadPlayer target = null;
            Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            float closestDistance = float.MaxValue;
            float usableDistance = MapUtilities.CachedShipStatus.AllVents.FirstOrDefault().UsableDistance;
            foreach ((DeadPlayer dp, Vector3 ps) in Medium.deadBodies) {
                float distance = Vector2.Distance(ps, truePosition);
                if (distance <= usableDistance && distance < closestDistance) {
                    closestDistance = distance;
                    target = dp;
                }
            }
            Medium.target = target;
        }

        static bool mushroomSaboWasActive = false;
        static void MorphlingAndCamouflagerUpdate() 
        {
            bool mushRoomSaboIsActive = Helpers.MushroomSabotageActive();
            if (!mushroomSaboWasActive) mushroomSaboWasActive = mushRoomSaboIsActive;
            
            float oldCamouflageTimer = Camouflager.CamouflageTimer;
            float oldMorphTimer = Morphling.morphTimer;
            float oldGlitchTimer = Glitch.MimicTimer;

            Camouflager.CamouflageTimer = Mathf.Max(0f, Camouflager.CamouflageTimer - Time.fixedDeltaTime);
            Morphling.morphTimer = Mathf.Max(0f, Morphling.morphTimer - Time.fixedDeltaTime);
            Glitch.MimicTimer = Mathf.Max(0f, Glitch.MimicTimer - Time.fixedDeltaTime);

            if (mushRoomSaboIsActive) return;

            // Camouflage reset and set Morphling look if necessary
            if (oldCamouflageTimer > 0f && Camouflager.CamouflageTimer <= 0f) 
            {
                Camouflager.ResetCamouflage();
                if (Morphling.morphTimer > 0f && Morphling.Player != null && Morphling.morphTarget != null) 
                {
                    PlayerControl target = Morphling.morphTarget;
                    Morphling.Player.SetLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
                }
                if (Glitch.MimicTimer > 0f && Glitch.Player != null && Glitch.MimicTarget != null) 
                {
                    PlayerControl target = Glitch.MimicTarget;
                    Glitch.Player.SetLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
                }
            }

            // If the MushRoomSabotage ends while Morph is still active set the Morphlings look to the target's look
            if (mushroomSaboWasActive) 
            {
                if (Morphling.morphTimer > 0f && Morphling.Player != null && Morphling.morphTarget != null) 
                {
                    PlayerControl target = Morphling.morphTarget;
                    Morphling.Player.SetLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
                }
                if (Glitch.MimicTimer > 0f && Glitch.Player != null && Glitch.MimicTarget != null) 
                {
                    PlayerControl target = Glitch.MimicTarget;
                    Glitch.Player.SetLook(target.Data.PlayerName, target.Data.DefaultOutfit.ColorId, target.Data.DefaultOutfit.HatId, target.Data.DefaultOutfit.VisorId, target.Data.DefaultOutfit.SkinId, target.Data.DefaultOutfit.PetId);
                }
                if (Camouflager.CamouflageTimer > 0) 
                {
                    foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                        player.SetLook("", 6, "", "", "", "");
                }
            }

            // Morphling reset (only if camouflage is inactive)
            if (Camouflager.CamouflageTimer <= 0f && oldMorphTimer > 0f && Morphling.morphTimer <= 0f && Morphling.Player != null)
                Morphling.ResetMorph();
            if (Camouflager.CamouflageTimer <= 0f && oldGlitchTimer > 0f && Glitch.MimicTimer <= 0f && Glitch.Player != null)
                Glitch.ResetMimic();
            mushroomSaboWasActive = false;
        }

        public static void LawyerUpdate() 
        {
            if (Lawyer.Player == null || Lawyer.Player != PlayerControl.LocalPlayer) return;

            // Promote to Pursuer
            if (Lawyer.target != null && Lawyer.target.Data.Disconnected && !Lawyer.Player.Data.IsDead) 
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerChangeRole, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.LawyerChangeRole();
                return;
            }
        }

        public static void RomanticUpdate() 
        {
            if (Romantic.Player == null || Romantic.Player != PlayerControl.LocalPlayer) return;

            if (Romantic.beloved != null && Romantic.beloved.Data.Disconnected && !Romantic.Player.Data.IsDead) 
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.RomanticChangeRole, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.RomanticChangeRole();
                return;
            }
        }

        public static void HackerUpdate()
        {
            if (Hacker.Player == null || PlayerControl.LocalPlayer != Hacker.Player || Hacker.Player.Data.IsDead) return;
            var (playerCompleted, _) = TasksHandler.TaskInfo(Hacker.Player.Data);
            if (playerCompleted == Hacker.rechargedTasks) {
                Hacker.rechargedTasks += Hacker.rechargeTasksNumber;
                if (Hacker.toolsNumber > Hacker.chargesVitals) Hacker.chargesVitals++;
                if (Hacker.toolsNumber > Hacker.chargesAdminTable) Hacker.chargesAdminTable++;
            }
        }

        // For Charges
        public static void SwapperUpdate() 
        {
            if (Swapper.Player == null || PlayerControl.LocalPlayer != Swapper.Player || PlayerControl.LocalPlayer.Data.IsDead) return;
            var (playerCompleted, _) = TasksHandler.TaskInfo(PlayerControl.LocalPlayer.Data);
            if (playerCompleted == Swapper.rechargedTasks) {
                Swapper.rechargedTasks += Swapper.rechargeTasksNumber;
                Swapper.charges++;
            }
        }

        public static void VeteranUpdate()
        {
            if (Veteran.Player == null || PlayerControl.LocalPlayer != Veteran.Player || PlayerControl.LocalPlayer.Data.IsDead) return;
            var (playerCompleted, _) = TasksHandler.TaskInfo(PlayerControl.LocalPlayer.Data);
            if (playerCompleted == Veteran.rechargedTasks) 
            {
                Veteran.rechargedTasks += Veteran.rechargeTasksNumber;
                Veteran.Charges++;
            }
        }

        public static void MysticUpdate() 
        {
            if (Mystic.Player == null || PlayerControl.LocalPlayer != Mystic.Player || PlayerControl.LocalPlayer.Data.IsDead) return;
            var (playerCompleted, _) = TasksHandler.TaskInfo(PlayerControl.LocalPlayer.Data);
            if (playerCompleted == Mystic.rechargedTasks) 
            {
                Mystic.rechargedTasks += Mystic.rechargeTasksNumber;
                Mystic.Charges++;
            }
        }

        public static void OracleUpdate() 
        {
            if (Oracle.Player == null || PlayerControl.LocalPlayer != Oracle.Player || PlayerControl.LocalPlayer.Data.IsDead) return;
            var (playerCompleted, _) = TasksHandler.TaskInfo(PlayerControl.LocalPlayer.Data);
            if (playerCompleted == Oracle.rechargedTasks) 
            {
                Oracle.rechargedTasks += Oracle.rechargeTasksNumber;
                Oracle.Charges++;
            }
        }

        static void OracleSetTarget()
        {
            if (Oracle.Player == null || Oracle.Player != PlayerControl.LocalPlayer) return;
            Oracle.CurrentTarget = SetTarget();
            SetPlayerOutline(Oracle.CurrentTarget, Oracle.Color);
        }

        static void PursuerSetTarget() 
        {
            if (Pursuer.Player == null || Pursuer.Player != PlayerControl.LocalPlayer) return;
            Pursuer.target = SetTarget();
            SetPlayerOutline(Pursuer.target, Pursuer.Color);
        }

        static void WitchSetTarget() {
            if (Witch.Player == null || Witch.Player != PlayerControl.LocalPlayer) return;
            List<PlayerControl> untargetables;
            if (Witch.spellCastingTarget != null)
                untargetables = PlayerControl.AllPlayerControls.ToArray().Where(x => x.PlayerId != Witch.spellCastingTarget.PlayerId).ToList(); // Don't switch the target from the the one you're currently casting a spell on
            else {
                untargetables = new List<PlayerControl>(); // Also target players that have already been spelled, to hide spells that were blanks/blocked by shields
                if (Spy.Player != null && !Witch.canSpellAnyone) untargetables.Add(Spy.Player);
                if (Sidekick.wasTeamRed && !Witch.canSpellAnyone) untargetables.Add(Sidekick.Player);
                if (Jackal.wasTeamRed && !Witch.canSpellAnyone) untargetables.Add(Jackal.Player);
            }
            Witch.CurrentTarget = SetTarget(onlyCrewmates: !Witch.canSpellAnyone, untargetablePlayers: untargetables);
            SetPlayerOutline(Witch.CurrentTarget, Witch.Color);
        }

        static void NinjaSetTarget()
        {
            if (Ninja.ninja == null || Ninja.ninja != PlayerControl.LocalPlayer) return;
            List<PlayerControl> untargetables = new List<PlayerControl>();
            if (Spy.Player != null && !Spy.impostorsCanKillAnyone) untargetables.Add(Spy.Player);
            if (Mini.Player != null && !Mini.IsGrownUp) untargetables.Add(Mini.Player);
            if (Sidekick.wasTeamRed && !Spy.impostorsCanKillAnyone) untargetables.Add(Sidekick.Player);
            if (Jackal.wasTeamRed && !Spy.impostorsCanKillAnyone) untargetables.Add(Jackal.Player);
            Ninja.CurrentTarget = SetTarget(onlyCrewmates: Spy.Player == null || !Spy.impostorsCanKillAnyone, untargetablePlayers: untargetables);
            SetPlayerOutline(Ninja.CurrentTarget, Ninja.Color);
        }

        static void ThiefSetTarget() 
        {
            if (Thief.Player == null || Thief.Player != PlayerControl.LocalPlayer) return;
            List<PlayerControl> untargetables = new List<PlayerControl>();
            if (Mini.Player != null && !Mini.IsGrownUp) untargetables.Add(Mini.Player);
            Thief.CurrentTarget = SetTarget(onlyCrewmates: false, untargetablePlayers: untargetables);
            SetPlayerOutline(Thief.CurrentTarget, Thief.Color);
        }




        static void BaitUpdate() 
        {
            if (!Bait.active.Any()) return;

            // Bait report
            foreach (KeyValuePair<DeadPlayer, float> entry in new Dictionary<DeadPlayer, float>(Bait.active)) 
            {
                Bait.active[entry.Key] = entry.Value - Time.fixedDeltaTime;
                if (entry.Value <= 0) {
                    Bait.active.Remove(entry.Key);
                    if (entry.Key.killerIfExisting != null && entry.Key.killerIfExisting.PlayerId == PlayerControl.LocalPlayer.PlayerId) {
                        Helpers.HandleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called
                        RPCProcedure.UncheckedCmdReportDeadBody(entry.Key.killerIfExisting.PlayerId, entry.Key.player.PlayerId);

                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UncheckedCmdReportDeadBody, Hazel.SendOption.Reliable, -1);
                        writer.Write(entry.Key.killerIfExisting.PlayerId);
                        writer.Write(entry.Key.player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
            }
        }

        static void BloodyUpdate() 
        {
            if (!Bloody.active.Any()) return;
            foreach (KeyValuePair<byte, float> entry in new Dictionary<byte, float>(Bloody.active)) 
            {
                PlayerControl player = Helpers.PlayerById(entry.Key);
                PlayerControl bloodyPlayer = Helpers.PlayerById(Bloody.bloodyKillerMap[player.PlayerId]);      

                Bloody.active[entry.Key] = entry.Value - Time.fixedDeltaTime;
                if (entry.Value <= 0 || player.Data.IsDead)
                {
                    Bloody.active.Remove(entry.Key);
                    continue;  // Skip the creation of the next blood drop, if the killer is dead or the time is up
                }
                new Bloodytrail(player, bloodyPlayer);
            }
        }

        // Mini set adapted button Cooldown for Vampire, Sheriff, Jackal, Sidekick, Warlock, Cleaner
        public static void MiniCooldownUpdate() 
        {
            if (Mini.Player != null && PlayerControl.LocalPlayer == Mini.Player) 
            {
                var multiplier = Mini.IsGrownUp ? 0.66f : 2f;
                HudManagerStartPatch.sheriffKillButton.MaxTimer = Sheriff.Cooldown * multiplier;
                HudManagerStartPatch.vampireKillButton.MaxTimer = Vampire.Cooldown * multiplier;
                HudManagerStartPatch.jackalKillButton.MaxTimer = Jackal.Cooldown * multiplier;
                HudManagerStartPatch.PestilenceButton.MaxTimer = Pestilence.Cooldown * multiplier;
                HudManagerStartPatch.GlitchKillButton.MaxTimer = Glitch.KillCooldown * multiplier;
                HudManagerStartPatch.RomanticKillButton.MaxTimer = VengefulRomantic.Cooldown * multiplier;
                HudManagerStartPatch.SerialKillerStabButton.MaxTimer = SerialKiller.StabCooldown * multiplier;
                HudManagerStartPatch.WerewolfMaulButton.MaxTimer = Werewolf.Cooldown * multiplier;
                HudManagerStartPatch.sidekickKillButton.MaxTimer = Sidekick.Cooldown * multiplier;
                HudManagerStartPatch.warlockCurseButton.MaxTimer = Warlock.Cooldown * multiplier;
                HudManagerStartPatch.cleanerCleanButton.MaxTimer = Cleaner.Cooldown * multiplier;
                HudManagerStartPatch.witchSpellButton.MaxTimer = (Witch.Cooldown + Witch.currentCooldownAddition) * multiplier;
                HudManagerStartPatch.ninjaButton.MaxTimer = Ninja.Cooldown * multiplier;
                HudManagerStartPatch.thiefKillButton.MaxTimer = Thief.Cooldown * multiplier;
            }
        }

        public static void TrapperUpdate() 
        {
            if (Trapper.Player == null || PlayerControl.LocalPlayer != Trapper.Player || Trapper.Player.Data.IsDead) return;
            var (playerCompleted, _) = TasksHandler.TaskInfo(Trapper.Player.Data);
            if (playerCompleted == Trapper.rechargedTasks) {
                Trapper.rechargedTasks += Trapper.rechargeTasksNumber;
                if (Trapper.maxCharges > Trapper.charges) Trapper.charges++;
            }
        }

        public static void Postfix(PlayerControl __instance) {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started || GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;

            // Mini and Morphling shrink
            PlayerSizeUpdate(__instance);
            
            // set position of colorblind text
            foreach (var pc in PlayerControl.AllPlayerControls) 
            {
                //pc.cosmetics.colorBlindText.gameObject.transform.localPosition = new Vector3(0, 0, -0.0001f);
            }
            
            if (PlayerControl.LocalPlayer == __instance) 
            {
                // Update player outlines
                SetBasePlayerOutlines();

                // Update Role Description
                Helpers.RefreshRoleDescription(__instance);

                // Update Player Info
                updatePlayerInfo();

                //Update pet visibility
                SetPetVisibility();

                // Time Master
                BendTimeUpdate();
                // Morphling
                MorphlingSetTarget();
                // Medic
                MedicSetTarget();
                // Crusader
                CrusaderSetTarget();
                //Juggernaut
                JuggernautSetTarget();
                //Romantic
                RomanticSetTarget();
                VengefulRomanticSetTarget();
                // Shifter
                ShifterSetTarget();
                //Pesti/plague
                PestilenceSetTarget();
                PlaguebearerSetTarget();
                // Sheriff
                SheriffSetTarget();
                // Glitch
                GlitchSetTarget();
                GlitchUpdate();
                // Detective
                DetectiveUpdateFootPrints();
                // Tracker
                TrackerSetTarget();
                // Vampire
                VampireSetTarget();
                Garlic.UpdateAll();
                Trap.Update();
                // Eraser
                EraserSetTarget();
                // Engineer
                EngineerUpdate();
                // Tracker
                TrackerUpdate();
                // Jackal
                JackalSetTarget();
                //Serial Killer
                SerialKillerSetTarget();
                //Werewolf
                WerewolfSetTarget();
                // Sidekick
                SidekickSetTarget();
                // Impostor
                ImpostorSetTarget();
                // Warlock
                warlockSetTarget();
                // Check for sidekick promotion on Jackal disconnect
                SidekickCheckPromotion();
                // Vigilante
                VigilanteSetTarget();
                VigilanteUpdate();
                // Arsonist
                ArsonistSetTarget();
                //Mystic
                MysticSetTarget();
                MysticUpdate();
                //Oracle
                OracleUpdate();
                OracleSetTarget();
                // Snitch
                SnitchUpdate();
                // BountyHunter
                BountyHunterUpdate();
                // Vulture
                VultureUpdate();
                // Amnesiac
                AmnesiacUpdate();
                // Medium
                MediumSetTarget();
                // Morphling and Camouflager
                MorphlingAndCamouflagerUpdate();
                // Undertaker
                UndertakerUpdate();
                // Lawyer
                LawyerUpdate();
                //Romantic
                RomanticUpdate();
                // Pursuer
                PursuerSetTarget();
                // Witch
                WitchSetTarget();
                // Ninja
                NinjaSetTarget();
                NinjaTrace.UpdateAll();
                ninjaUpdate();
                // Thief
                ThiefSetTarget();
                // yoyo
                Silhouette.UpdateAll();

                HackerUpdate();
                SwapperUpdate();
                //Veteran
                VeteranUpdate();
                // Hacker
                HackerUpdate();
                // Trapper
                TrapperUpdate();

                // -- MODIFIER--
                // Bait
                BaitUpdate();
                // Bloody
                BloodyUpdate();
                // mini (for the cooldowns)
                MiniCooldownUpdate();
                // Chameleon (invis stuff, timers)
                Chameleon.Update();
            } 
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.WalkPlayerTo))]
    class PlayerPhysicsWalkPlayerToPatch 
    {
        private static Vector2 offset = Vector2.zero;
        public static void Prefix(PlayerPhysics __instance) 
        {
            bool correctOffset = Camouflager.CamouflageTimer <= 0f && !Helpers.MushroomSabotageActive() && (__instance.myPlayer == Mini.Player ||  (Morphling.Player != null && __instance.myPlayer == Morphling.Player && Morphling.morphTarget == Mini.Player && Morphling.morphTimer > 0f) || (Glitch.Player != null && __instance.myPlayer == Glitch.Player && Glitch.MimicTarget == Mini.Player && Glitch.MimicTimer > 0f));
            correctOffset = correctOffset && !(Mini.Player == Morphling.Player && Morphling.morphTimer > 0f);
            if (correctOffset) 
            {
            float currentScaling = (Mini.GrowingProgress() + 1) * 0.5f;
            __instance.myPlayer.Collider.offset = currentScaling * Mini.defaultColliderOffset * Vector2.down;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    class PlayerControlCmdReportDeadBodyPatch 
    {
        public static bool Prefix(PlayerControl __instance) 
        {
            Helpers.HandleVampireBiteOnBodyReport();
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.LocalPlayer.CmdReportDeadBody))]
    class BodyReportPatch
    {
        static void Postfix(PlayerControl __instance, [HarmonyArgument(0)]NetworkedPlayerInfo target)
        {
            // Medic or Detective report
            bool isMedicReport = Medic.Player != null && Medic.Player == PlayerControl.LocalPlayer && __instance.PlayerId == Medic.Player.PlayerId;
            bool isDetectiveReport = Detective.Player != null && Detective.Player == PlayerControl.LocalPlayer && __instance.PlayerId == Detective.Player.PlayerId;
            bool IsSleuthReport = Sleuth.Players.FindAll(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId && __instance.PlayerId == PlayerControl.LocalPlayer.PlayerId).Count > 0;

            DeadPlayer deadPlayer = deadPlayers?.Where(x => x.player?.PlayerId == target?.PlayerId)?.FirstOrDefault();
            if (isMedicReport || isDetectiveReport)
            {
                if (deadPlayer != null && deadPlayer.killerIfExisting != null) {
                    float timeSinceDeath = ((float)(DateTime.UtcNow - deadPlayer.timeOfDeath).TotalMilliseconds);
                    string msg = "";

                    if (isMedicReport) 
                    {
                        msg = $"Body Report: Killed {Math.Round(timeSinceDeath / 1000)}s ago!";
                    } else if (isDetectiveReport) 
                    {
                        if (timeSinceDeath < Detective.reportNameDuration * 1000) 
                        {
                            msg =  $"Body Report: The killer appears to be {deadPlayer.killerIfExisting.Data.PlayerName}!";
                        } 
                        else if (timeSinceDeath < Detective.reportColorDuration * 1000) 
                        {
                            var typeOfColor = Helpers.IsLighterColor(deadPlayer.killerIfExisting) ? "lighter" : "darker";
                            msg =  $"Body Report: The killer appears to be a {typeOfColor} Color!";
                        } else {
                            msg = $"Body Report: The corpse is too old to gain information from!";
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(msg))
                    {   
                        if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
                        {
                            FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, msg);

                            // Ghost Info
                            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                            writer.Write(PlayerControl.LocalPlayer.PlayerId);
                            writer.Write((byte)GhostInfoTypes.DetectiveOrMedicInfo);
                            writer.Write(msg);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                        }
                        if (msg.IndexOf("who", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            FastDestroyableSingleton<UnityTelemetry>.Instance.SendWho();
                        }
                    }
                }
            }
            if (IsSleuthReport)
            {
                Sleuth.Reported.Add(deadPlayer.player.PlayerId);
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public static class MurderPlayerPatch
    {
        public static bool resetToCrewmate = false;
        public static bool resetToDead = false;

        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)]PlayerControl target)
        {
            // Allow everyone to murder players
            resetToCrewmate = !__instance.Data.Role.IsImpostor;
            resetToDead = __instance.Data.IsDead;
            __instance.Data.Role.TeamType = RoleTeamTypes.Impostor;
            __instance.Data.IsDead = false;
        }

        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)]PlayerControl target)
        {
            // Collect dead player info
            DeadPlayer deadPlayer = new DeadPlayer(target, DateTime.UtcNow, DeadPlayer.CustomDeathReason.Kill, __instance);
            GameHistory.deadPlayers.Add(deadPlayer);

            // Reset killer to crewmate if resetToCrewmate
            if (resetToCrewmate) __instance.Data.Role.TeamType = RoleTeamTypes.Crewmate;
            if (resetToDead) __instance.Data.IsDead = true;

            // Remove fake tasks when player dies
            if (target.HasFakeTasks() || target == Lawyer.Player || target == Pursuer.Player || target == Thief.Player)
                target.ClearAllTasks();

            // First kill (set before lover suicide)
            if (MapOptions.FirstKillName == "") MapOptions.FirstKillName = target.Data.PlayerName;

            // Lover suicide trigger on murder
            if ((Lovers.Lover1 != null && target == Lovers.Lover1) || (Lovers.Lover2 != null && target == Lovers.Lover2)) {
                PlayerControl otherLover = target == Lovers.Lover1 ? Lovers.Lover2 : Lovers.Lover1;
                if (otherLover != null && !otherLover.Data.IsDead && Lovers.bothDie) {
                    otherLover.MurderPlayer(otherLover);
                    GameHistory.OverrideDeathReasonAndKiller(otherLover, DeadPlayer.CustomDeathReason.LoverSuicide);
                }
            }

            if (__instance.AmOwner)
            {
                if (PlayerControl.LocalPlayer == Disperser.Player)
                    Disperser.Charges += Disperser.RechargeKillsCount;
            }

            // Sidekick promotion trigger on murder
            if (Sidekick.promotesToJackal && Sidekick.Player != null && !Sidekick.Player.Data.IsDead && target == Jackal.Player && Jackal.Player == PlayerControl.LocalPlayer) {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SidekickPromotes, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.SidekickPromotes();
            }

            // Pursuer promotion trigger on murder (the host sends the call such that everyone recieves the update before a possible game End)
            if (target == Lawyer.target && AmongUsClient.Instance.AmHost && Lawyer.Player != null) 
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerChangeRole, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.LawyerChangeRole();
            }

            // Pursuer promotion trigger on murder (the host sends the call such that everyone recieves the update before a possible game End)
            if (target == Romantic.beloved && AmongUsClient.Instance.AmHost && Romantic.Player != null) 
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.RomanticChangeRole, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.RomanticChangeRole();
            }

            // Mystic show flash and add dead player position
            if (Mystic.Player != null && (PlayerControl.LocalPlayer == Mystic.Player || Helpers.ShouldShowGhostInfo()) && !Mystic.Player.Data.IsDead && Mystic.Player != target && Mystic.mode <= 1) 
            {
                Helpers.ShowFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f), message : "Mystic Info: Someone Died");
            }

            if (Mystic.deadBodyPositions != null) Mystic.deadBodyPositions.Add(target.transform.position);

            // Tracker store body positions
            if (Tracker.deadBodyPositions != null) Tracker.deadBodyPositions.Add(target.transform.position);

            // Medium add body
            if (Medium.deadBodies != null) 
            {
                Medium.futureDeadBodies.Add(new Tuple<DeadPlayer, Vector3>(deadPlayer, target.transform.position));
            }

            // Set bountyHunter Cooldown
            if (BountyHunter.Player != null && PlayerControl.LocalPlayer == BountyHunter.Player && __instance == BountyHunter.Player) {
                if (target == BountyHunter.bounty) 
                {
                    BountyHunter.Player.SetKillTimer(BountyHunter.bountyKillCooldown);
                    BountyHunter.bountyUpdateTimer = 0f; // Force bounty update
                }
                else
                    BountyHunter.Player.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + BountyHunter.punishmentTime); 
            }

            // Mini Set Impostor Mini kill timer (Due to mini being a modifier, all "SetKillTimers" must have happened before this!)
            if (Mini.Player != null && __instance == Mini.Player && __instance == PlayerControl.LocalPlayer) 
            {
                float multiplier = 1f;
                if (Mini.Player != null && PlayerControl.LocalPlayer == Mini.Player) multiplier = Mini.IsGrownUp ? 0.66f : 2f;
                Mini.Player.SetKillTimer(__instance.killTimer * multiplier);
            }

            // Cleaner Button Sync
            if (Cleaner.Player != null && PlayerControl.LocalPlayer == Cleaner.Player && __instance == Cleaner.Player && HudManagerStartPatch.cleanerCleanButton != null)
                HudManagerStartPatch.cleanerCleanButton.Timer = Cleaner.Player.killTimer;

            // Witch Button Sync
            if (Witch.triggerBothCooldowns && Witch.Player != null && PlayerControl.LocalPlayer == Witch.Player && __instance == Witch.Player && HudManagerStartPatch.witchSpellButton != null)
                HudManagerStartPatch.witchSpellButton.Timer = HudManagerStartPatch.witchSpellButton.MaxTimer;

            // Warlock Button Sync
            if (Warlock.Player != null && PlayerControl.LocalPlayer == Warlock.Player && __instance == Warlock.Player && HudManagerStartPatch.warlockCurseButton != null) {
                if (Warlock.Player.killTimer > HudManagerStartPatch.warlockCurseButton.Timer) {
                    HudManagerStartPatch.warlockCurseButton.Timer = Warlock.Player.killTimer;
                }
            }
            // Ninja Button Sync
            if (Ninja.ninja != null && PlayerControl.LocalPlayer == Ninja.ninja && __instance == Ninja.ninja && HudManagerStartPatch.ninjaButton != null)
                HudManagerStartPatch.ninjaButton.Timer = HudManagerStartPatch.ninjaButton.MaxTimer;

            // Bait
            if (Bait.Players.FindAll(x => x.PlayerId == target.PlayerId).Count > 0) 
            {
                float reportDelay = (float) rnd.Next((int)Bait.reportDelayMin, (int)Bait.reportDelayMax + 1);
                Bait.active.Add(deadPlayer, reportDelay);

                if (Bait.showKillFlash && __instance == PlayerControl.LocalPlayer) Helpers.ShowFlash(new Color(204f / 255f, 102f / 255f, 0f / 255f));
            }

            // Add Bloody Modifier
            if (Bloody.Players.FindAll(x => x.PlayerId == target.PlayerId).Count > 0) 
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Bloody, Hazel.SendOption.Reliable, -1);
                writer.Write(__instance.PlayerId);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.Bloody(__instance.PlayerId, target.PlayerId);
            }

            // VIP Modifier
            if (Vip.Players.FindAll(x => x.PlayerId == target.PlayerId).Count > 0) 
            {
                Color Color = Color.yellow;
                if (Vip.showColor) 
                {
                    Color = Color.white;
                    if (target.Data.Role.IsImpostor) Color = Color.red;
                    else if (target.IsNeutral() || target.IsNeutralKiller()) Color = Color.blue;
                }
                Helpers.ShowFlash(Color, 1.5f);
            }

            // Snitch
            if (Snitch.Player != null && PlayerControl.LocalPlayer.PlayerId == Snitch.Player.PlayerId && MapBehaviourPatch.herePoints.Keys.Any(x => x == target.PlayerId)) 
            {
                foreach (var a in MapBehaviourPatch.herePoints.Where(x => x.Key == target.PlayerId)) 
                {
                    UnityEngine.Object.Destroy(a.Value);
                    MapBehaviourPatch.herePoints.Remove(a.Key);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKillTimer))]
    class PlayerControlSetCoolDownPatch 
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)]float time) 
        {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return true;
            if (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown <= 0f) return false;
            float multiplier = 1f;
            float addition = 0f;
            if (Mini.Player != null && PlayerControl.LocalPlayer == Mini.Player) multiplier = Mini.IsGrownUp ? 0.66f : 2f;
            if (BountyHunter.Player != null && PlayerControl.LocalPlayer == BountyHunter.Player) addition = BountyHunter.punishmentTime;

            __instance.killTimer = Mathf.Clamp(time, 0f, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * multiplier + addition);
            FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(__instance.killTimer, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * multiplier + addition);
            return false;
        }
    }

    [HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.CoPerformKill))]
    class KillAnimationCoPerformKillPatch 
    {
        public static bool hideNextAnimation = false;
        public static void Prefix(KillAnimation __instance, [HarmonyArgument(0)]ref PlayerControl source, [HarmonyArgument(1)]ref PlayerControl target) 
        {
            if (hideNextAnimation)
                source = target;
            hideNextAnimation = false;
        }
    }

    [HarmonyPatch(typeof(KillAnimation), nameof(KillAnimation.SetMovement))]
    class KillAnimationSetMovementPatch {
        private static int? colorId = null;
        public static void Prefix(PlayerControl source, bool canMove) 
        {
            Color Color = source.cosmetics.currentBodySprite.BodySprite.material.GetColor("_BodyColor");
            if ((Morphling.Player != null && source.Data.PlayerId == Morphling.Player.PlayerId) || 
            (Glitch.Player != null && source.Data.PlayerId == Glitch.Player.PlayerId)) 
            {
            var index = Palette.PlayerColors.IndexOf(Color);
            if (index != -1) colorId = index;
            }
        }

        public static void Postfix(PlayerControl source, bool canMove) {
            if (colorId.HasValue) source.RawSetColor(colorId.Value);
            colorId = null;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
    public static class ExilePlayerPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            // Collect dead player info
            DeadPlayer deadPlayer = new DeadPlayer(__instance, DateTime.UtcNow, DeadPlayer.CustomDeathReason.Exile, null);
            GameHistory.deadPlayers.Add(deadPlayer);


            // Remove fake tasks when player dies
            if (__instance.HasFakeTasks() || __instance == Lawyer.Player || __instance == Pursuer.Player || __instance == Thief.Player)
                __instance.ClearAllTasks();

            // Lover suicide trigger on exile
            if ((Lovers.Lover1 != null && __instance == Lovers.Lover1) || (Lovers.Lover2 != null && __instance == Lovers.Lover2)) {
                PlayerControl otherLover = __instance == Lovers.Lover1 ? Lovers.Lover2 : Lovers.Lover1;
                if (otherLover != null && !otherLover.Data.IsDead && Lovers.bothDie) {
                    otherLover.Exiled();
                    GameHistory.OverrideDeathReasonAndKiller(otherLover, DeadPlayer.CustomDeathReason.LoverSuicide);
                }

            }            
            // Sidekick promotion trigger on exile
            if (Sidekick.promotesToJackal && Sidekick.Player != null && !Sidekick.Player.Data.IsDead && __instance == Jackal.Player && Jackal.Player == PlayerControl.LocalPlayer) 
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SidekickPromotes, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.SidekickPromotes();
            }

            if (Romantic.Player != null && !Romantic.Player.Data.IsDead && __instance == Romantic.beloved) 
            {
                if (AmongUsClient.Instance.AmHost && (Romantic.beloved != Jester.Player))
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.RomanticChangeRole, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.RomanticChangeRole();
                }
            }

            // Pursuer promotion trigger on exile & suicide (the host sends the call such that everyone recieves the update before a possible game End)
            if (Lawyer.Player != null && __instance == Lawyer.target) 
            {
                PlayerControl lawyer = Lawyer.Player;
                if (AmongUsClient.Instance.AmHost && ((Lawyer.target != Jester.Player && !Lawyer.isProsecutor) || Lawyer.targetWasGuessed)) 
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.LawyerChangeRole, Hazel.SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.LawyerChangeRole();
                }

                if (!Lawyer.targetWasGuessed && !Lawyer.isProsecutor) 
                {
                    if (Lawyer.Player != null) Lawyer.Player.Exiled();
                    if (Pursuer.Player != null) Pursuer.Player.Exiled();

                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareGhostInfo, Hazel.SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write((byte)GhostInfoTypes.DeathReasonAndKiller);
                    writer.Write(lawyer.PlayerId);
                    writer.Write((byte)DeadPlayer.CustomDeathReason.LawyerSuicide);
                    writer.Write(lawyer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    GameHistory.OverrideDeathReasonAndKiller(lawyer, DeadPlayer.CustomDeathReason.LawyerSuicide, lawyer);  // TODO: only executed on host?!
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public static class PlayerPhysicsFixedUpdate {
        public static void Postfix(PlayerPhysics __instance)
        {
            bool shouldInvert = Invert.Players.FindAll(x => x.PlayerId == PlayerControl.LocalPlayer.PlayerId).Count > 0 && Invert.meetings > 0;
            if (__instance.AmOwner &&
                AmongUsClient.Instance &&
                AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started &&
                !PlayerControl.LocalPlayer.Data.IsDead && 
                shouldInvert && 
                GameData.Instance && 
                __instance.myPlayer.CanMove)  
                __instance.body.velocity *= -1;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.IsFlashlightEnabled))]
    public static class IsFlashlightEnabledPatch {
        public static bool Prefix(ref bool __result) {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek)
                return true;
            __result = false;
            if (!PlayerControl.LocalPlayer.Data.IsDead && Lighter.Player != null && Lighter.Player.PlayerId == PlayerControl.LocalPlayer.PlayerId) {
                __result = true;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.AdjustLighting))]
    public static class AdjustLight {
        public static bool Prefix(PlayerControl __instance) {
            if (__instance == null || PlayerControl.LocalPlayer == null || Lighter.Player == null) return true;

            bool hasFlashlight = !PlayerControl.LocalPlayer.Data.IsDead && Lighter.Player.PlayerId == PlayerControl.LocalPlayer.PlayerId;
            __instance.SetFlashlightInputMethod();
            __instance.lightSource.SetupLightingForGameplay(hasFlashlight, Lighter.flashlightWidth, __instance.TargetFlashlight.transform);

            return false;
        }
    }
    
    [HarmonyPatch(typeof(GameData), nameof(GameData.HandleDisconnect), new[] {typeof(PlayerControl), typeof(DisconnectReasons) })]
    public static class GameDataHandleDisconnectPatch {
        public static void Prefix(GameData __instance, PlayerControl player, DisconnectReasons reason) {
            if (MeetingHud.Instance) {
                MeetingHudPatch.SwapperCheckAndReturnSwap(MeetingHud.Instance, player.PlayerId);
            }
        }
    }
}
