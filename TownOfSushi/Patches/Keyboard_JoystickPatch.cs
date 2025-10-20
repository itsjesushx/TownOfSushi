using AmongUs.GameOptions;
using HarmonyLib;
using InnerNet;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using UnityEngine;

namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static class Keyboard_Joystick
    {
        private static bool GetKeysDown(params KeyCode[] keys)
        {
            if (keys.Any(Input.GetKeyDown) && keys.All(Input.GetKey))
            {
                Logger<TownOfSushiPlugin>.Info($"Shortcut Key{keys.First(Input.GetKeyDown)} in [{string.Join(",", keys)}]");
                return true;
            }
            return false;
        }
        public static void Postfix()
        {
            if (!AmongUsClient.Instance.AmHost && AmongUsClient.Instance?.GameState != InnerNetClient.GameStates.Started)
            {
                return;
            }

            if (GetKeysDown(KeyCode.LeftControl, KeyCode.LeftShift, KeyCode.M))
            {
                if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return;

                // if in a meeting, close it, else start one.
                if (MeetingHud.Instance)
                {
                    foreach (var pva in MeetingHud.Instance.playerStates)
                    {
                        if (pva == null) continue;

                        if (pva.VotedFor < 253)
                            MeetingHud.Instance.RpcClearVote(pva.TargetPlayerId);
                    }
                    List<MeetingHud.VoterState> statesList = [];
                    MeetingHud.Instance.RpcVotingComplete(statesList.ToArray(), null, true);
                    MeetingHud.Instance.RpcClose();
                }
                else
                {
                    DevPatches.StartMeeting(PlayerControl.LocalPlayer);
                }
            }
            if (GetKeysDown(KeyCode.LeftControl, KeyCode.LeftShift, KeyCode.E))
            {
                MiscUtils.RpcHostSuicide(PlayerControl.LocalPlayer);
            }
        }
    }

    // Will add more to this class later on
    [HarmonyPatch]
    public static class DevPatches
    {
        public static bool HostEndedGame;

        [MethodRpc((uint)TownOfSushiRpc.HostCallMeeting, SendImmediately = true)]
        public static void StartMeeting(PlayerControl player)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                MeetingRoomManager.Instance.AssignSelf(player, null);

                if (GameManager.Instance.CheckTaskCompletion())
                {
                    return;
                }

                HudManager.Instance.OpenMeetingRoom(player);
                player.RpcStartMeeting(null);
            }
        }
    }
}