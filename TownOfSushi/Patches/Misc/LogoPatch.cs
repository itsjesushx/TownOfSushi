using AmongUs.GameOptions;
using HarmonyLib;
using Reactor.Localization.Utilities;
using TownOfSushi.Modules.Wiki;

namespace TownOfSushi.Patches.Misc;

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
public static class LogoPatch
{
    public static void Postfix()
    {
        RoleManager.Instance.GetRole(RoleTypes.CrewmateGhost).StringName =
            CustomStringName.CreateAndRegister("Crewmate Ghost");
        RoleManager.Instance.GetRole(RoleTypes.ImpostorGhost).StringName =
            CustomStringName.CreateAndRegister("Impostor Ghost");

        var roles = MiscUtils.AllRoles.Where(x =>
            x is not IWikiDiscoverable || x is ICustomRole custom && !custom.Configuration.HideSettings);

        if (roles.Any())
        {
            foreach (var role in roles)
            {
                SoftWikiEntries.RegisterRoleEntry(role);
            }
        }

        List<RoleBehaviour> vanillaRoles = new()
        {
            // RoleManager.Instance.GetRole(RoleTypes.Crewmate),
            RoleManager.Instance.GetRole(RoleTypes.Scientist),
            RoleManager.Instance.GetRole(RoleTypes.Noisemaker),
            RoleManager.Instance.GetRole(RoleTypes.Engineer),
            RoleManager.Instance.GetRole(RoleTypes.Tracker),
            RoleManager.Instance.GetRole(RoleTypes.GuardianAngel),
            RoleManager.Instance.GetRole(RoleTypes.Detective),
            // RoleManager.Instance.GetRole(RoleTypes.Impostor),
            RoleManager.Instance.GetRole(RoleTypes.Shapeshifter),
            RoleManager.Instance.GetRole(RoleTypes.Phantom),
            RoleManager.Instance.GetRole(RoleTypes.Viper),
        };
        foreach (var role in vanillaRoles)
        {
            SoftWikiEntries.RegisterVanillaRoleEntry(role);
        }
    }
}