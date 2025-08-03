using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using TownOfSushi.Modifiers;
using TownOfSushi.Options;
using TownOfSushi.Options.Modifiers.Alliance;
using TownOfSushi.Utilities;
using TownOfSushi.Utilities.Appearances;

namespace TownOfSushi.Patches.Options;

// Is there a better way I can do this??
[HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.IsValidTarget))]
public static class ImpostorTargeting
{
    public static void Postfix(ImpostorRole __instance, NetworkedPlayerInfo target, ref bool __result)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;

        __result &= !(target?.Object?.TryGetModifier<DisabledModifier>(out var mod) == true && !mod.CanBeInteractedWith) &&
            (target?.Object?.IsImpostor() == false ||
            genOpt.FFAImpostorMode ||
            (PlayerControl.LocalPlayer.IsLover() && OptionGroupSingleton<LoversOptions>.Instance.LoverKillTeammates) ||
            (genOpt.KillDuringCamoComms && target?.Object?.GetAppearanceType() == TownOfSushiAppearances.Camouflage));
    }
}
