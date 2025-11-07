using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace TownOfSushi.Patches;

[HarmonyPatch(typeof(MeetingHud))]
public static class MeetingHudGetVotesPatch
{
    public static MeetingHud.VoterState[] States { get; private set; } = [];

    [HarmonyPrefix]
    [HarmonyPatch(nameof(MeetingHud.VotingComplete))]
    public static void VotingCompletePrefix(Il2CppStructArray<MeetingHud.VoterState> states)
    {
        States = states;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MeetingHud.OnDestroy))]
    public static void OnDestroyPostfix()
    {
        States = [];
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(MeetingHud.Start))]
    public static void StartPostfix()
    {
        States = [];
    }
}