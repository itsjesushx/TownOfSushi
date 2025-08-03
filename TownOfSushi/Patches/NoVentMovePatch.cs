using HarmonyLib;
using MiraAPI.GameOptions;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Roles.Neutral;

namespace TownOfSushi.Patches;

[HarmonyPatch]
public static class NoVentMovePatch
{
    [HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
    [HarmonyPrefix]
    public static bool EnterVent()
    {
        if (PlayerControl.LocalPlayer == null)
        {
            return true;
        }

        if (PlayerControl.LocalPlayer.Data == null)
        {
            return true;
        }

        if (PlayerControl.LocalPlayer.Data.Role is JesterRole)
        {
            return false;
        }
        if (PlayerControl.LocalPlayer.Data.Role is SpyRole && !OptionGroupSingleton<SpyOptions>.Instance.SpyChangeVents)
        {
            return false;
        }

        return true;
    }
}