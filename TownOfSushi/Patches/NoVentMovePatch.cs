using HarmonyLib;

namespace TownOfSushi.Patches;

[HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
public static class EnterVentPatch
{
    public static bool Prefix(Vent __instance)
    {
        var player = PlayerControl.LocalPlayer;

        if (player.Data.Role is JesterRole)
            return false;
        else if (player.Data.Role is SpyRole)
            return OptionGroupSingleton<SpyOptions>.Instance.SpyChangeVents;
        else
            return true;
    }
}