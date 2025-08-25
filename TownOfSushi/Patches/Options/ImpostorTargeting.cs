using HarmonyLib;
using TownOfSushi.Modifiers;
using TownOfSushi.Options;
using TownOfSushi.Utilities.Appearances;

namespace TownOfSushi.Patches.Options;

// Is there a better way I can do this??
[HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.IsValidTarget))]
public static class ImpostorTargeting
{
    public static void Postfix(ImpostorRole __instance, NetworkedPlayerInfo target, ref bool __result)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        var loveOpt = OptionGroupSingleton<LoversOptions>.Instance;

        __result &=
            !(!loveOpt.LoversKillEachOther && target?.Object?.IsLover() == true && PlayerControl.LocalPlayer.IsLover()) &&
            !(target?.Object?.TryGetModifier<DisabledModifier>(out var mod) == true && !mod.CanBeInteractedWith) &&
            (target?.Object?.IsImpostor() == false ||
             genOpt.FFAImpostorMode ||
             (PlayerControl.LocalPlayer.IsLover() && loveOpt.LoverKillTeammates) ||
             (genOpt.KillDuringCamoComms && target?.Object?.GetAppearanceType() == TownOfSushiAppearances.Camouflage));
    }
}