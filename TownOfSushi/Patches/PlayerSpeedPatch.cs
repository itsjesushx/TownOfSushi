using HarmonyLib;
using TownOfSushi.Roles.Impostor.Venerer;
using TownOfSushi.Utilities.Appearances;

namespace TownOfSushi.Patches;

[HarmonyPatch(typeof(LogicOptions), nameof(LogicOptions.GetPlayerSpeedMod))]
public static class PlayerSpeedPatch
{
    public static void Postfix(PlayerControl pc, ref float __result)
    {
        __result *= pc.GetAppearance().Speed;

        if (pc.HasModifier<VenererSprintModifier>())
        {
            __result *= OptionGroupSingleton<VenererOptions>.Instance.NumSprintSpeed;
        }

        if (pc.TryGetModifier<VenererFreezeModifier>(out var freeze))
        {
            __result *= freeze.SpeedFactor;
        }
    }
}