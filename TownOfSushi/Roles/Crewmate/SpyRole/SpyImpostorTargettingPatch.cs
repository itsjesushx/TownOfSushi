using AmongUs.GameOptions;
using HarmonyLib;
using UnityEngine; 

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type
#pragma warning disable CS8603 // Possible null reference return
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type

namespace TownOfSushi.Roles.Crewmate
{
    // I'm tired of those warnings smh
    // I'm aware there's already a patch for this but it doesn't work with Spy somewhy???
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class PlayerControlFixedUpdatePatch
    {
        static PlayerControl SetTarget(bool onlyCrewmates = false, bool targetPlayersInVents = false, List<PlayerControl> untargetablePlayers = null, PlayerControl targetingPlayer = null)
        {
            PlayerControl result = null;
            float num = LegacyGameOptions.KillDistances[Mathf.Clamp(GameOptionsManager.Instance.currentNormalGameOptions.KillDistance, 0, 2)];
            if (!ShipStatus.Instance) return result;
            if (targetingPlayer == null) targetingPlayer = PlayerControl.LocalPlayer;
            if (targetingPlayer.Data.IsDead) return result;

            Vector2 truePosition = targetingPlayer.GetTruePosition();
            foreach (var playerInfo in GameData.Instance.AllPlayers)
            {
                if (!playerInfo.Disconnected && playerInfo.PlayerId != targetingPlayer.PlayerId && !playerInfo.IsDead && (!onlyCrewmates || !playerInfo.Role.IsImpostor))
                {
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
                        if (magnitude <= num && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask))
                        {
                            result = @object;
                            num = magnitude;
                        }
                    }
                }
            }
            return result;
        }
        static void ImpostorKillTargetUpdate()
        {
            if (!PlayerControl.LocalPlayer.IsImpostor() || !PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.Data.IsDead)
            {
                DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
                return;
            }

            PlayerControl target;
            if (MiscUtils.SpyInGame())
            {
                if (OptionGroupSingleton<SpyOptions>.Instance.SpyImpsKillEachOther)
                {
                    target = SetTarget(false, true);
                }
                else
                {
                    target = SetTarget(true, true, untargetablePlayers: PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x.Data.Role is SpyRole).ToList());
                }
            }
            else
            {
                target = SetTarget(true, true);
            }

            DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(target);
        }
        public static void Postfix(PlayerControl __instance)
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started || GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;

            if (PlayerControl.LocalPlayer == __instance)
            {
                ImpostorKillTargetUpdate();
            }
        }
    }
#pragma warning restore CS8625
#pragma warning restore CS8603
#pragma warning restore CS8600
}