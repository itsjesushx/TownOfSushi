using HarmonyLib;

namespace TownOfSushi.Patches;
[HarmonyPatch]
public static class IntroPatch
{
    public static void SetupSpyTeamIcons(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        if (PlayerControl.LocalPlayer.Data.Role is not ICustomRole customRole)
        {
            return;
        }
        __instance.ImpostorText.gameObject.SetActive(true);
        __instance.BackgroundBar.material.color = customRole.RoleColor;
        __instance.TeamTitle.color = customRole.RoleColor;
        if (PlayerControl.LocalPlayer.IsImpostor())
        {
            __instance.ImpostorText.text = "Sabotage and Kill everyone";
        }

        var spy = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => p.IsRole<SpyRole>() && !p.HasDied());
        if (spy == null)
        {
            return;
        }
        // Add the Spy to the Impostor team (for the Impostors)
        if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
        {
            List<PlayerControl> players = PlayerControl.AllPlayerControls.ToArray().OrderBy(x => Guid.NewGuid()).ToList();
            var fakeImpostorTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>(); // The local player always has to be the first one in the list (to be displayed in the center)
            fakeImpostorTeam.Add(PlayerControl.LocalPlayer);
            foreach (PlayerControl p in players)
            {
                if (!p.AmOwner && (p == spy || p.Data.Role.IsImpostor))
                    fakeImpostorTeam.Add(p);
            }
            yourTeam = fakeImpostorTeam;
        }
    }
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.CreatePlayer))]
    static class CreatePlayerPatch
    {
        static void Postfix(bool impostorPositioning, ref PoolablePlayer __result)
        {
            if (impostorPositioning)
            {
                __result.SetNameColor(Palette.ImpostorRed);
            }
        }
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
    static class BeginCrewmatePatch
    {
        public static void Prefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay)
        {
            SetupSpyTeamIcons(__instance, ref teamToDisplay);
        }

        public static void Postfix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> teamToDisplay)
        {
            SetupSpyTeamIcons(__instance, ref teamToDisplay);
        }
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
    static class BeginImpostorPatch
    {
        public static void Prefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
        {
            SetupSpyTeamIcons(__instance, ref yourTeam);
        }

        public static void Postfix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
        {
            SetupSpyTeamIcons(__instance, ref yourTeam);
        }
    }
}