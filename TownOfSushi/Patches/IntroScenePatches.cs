using TownOfSushi.Options;
using HarmonyLib;

namespace TownOfSushi.Patches;
[HarmonyPatch]
public static class IntroPatch
{
    public static void SetupSpyTeamIcons(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
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
                if (PlayerControl.LocalPlayer != p && (p == spy || p.Data.Role.IsImpostor))
                    fakeImpostorTeam.Add(p);
            }
            yourTeam = fakeImpostorTeam;
        }
    }
    public static void SetHiddenImpostors(IntroCutscene __instance)
    {
        var amount = Helpers.GetAlivePlayers().Count(x => x.IsImpostor());
        __instance.ImpostorText.text =
            DestroyableSingleton<TranslationController>.Instance.GetString(
                amount == 1 ? StringNames.NumImpostorsS : StringNames.NumImpostorsP, amount);
        __instance.ImpostorText.text = __instance.ImpostorText.text.Replace("[FF1919FF]", "<color=#FF1919FF>");
        __instance.ImpostorText.text = __instance.ImpostorText.text.Replace("[]", "</color>");

        if (!OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled)
        {
            return;
        }

        var players = GameData.Instance.PlayerCount;

        if (players < 7)
        {
            return;
        }

        var list = OptionGroupSingleton<RoleOptions>.Instance;

        int maxSlots = players < 15 ? players : 15;

        List<RoleListOption> buckets = [];
        if (list.RoleListEnabled)
        {
            for (int i = 0; i < maxSlots; i++)
            {
                int slotValue = i switch
                {
                    0 => list.Slot1,
                    1 => list.Slot2,
                    2 => list.Slot3,
                    3 => list.Slot4,
                    4 => list.Slot5,
                    5 => list.Slot6,
                    6 => list.Slot7,
                    7 => list.Slot8,
                    8 => list.Slot9,
                    9 => list.Slot10,
                    10 => list.Slot11,
                    11 => list.Slot12,
                    12 => list.Slot13,
                    13 => list.Slot14,
                    14 => list.Slot15,
                    _ => -1
                };

                buckets.Add((RoleListOption)slotValue);
            }
        }

        if (!buckets.Any(x => x is RoleListOption.Any))
        {
            return;
        }


        __instance.ImpostorText.text =
            DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.NumImpostorsP, 256);
        __instance.ImpostorText.text = __instance.ImpostorText.text.Replace("[FF1919FF]", "<color=#FF1919FF>");
        __instance.ImpostorText.text = __instance.ImpostorText.text.Replace("[]", "</color>");
        __instance.ImpostorText.text = __instance.ImpostorText.text.Replace("256", "???");
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
            SetHiddenImpostors(__instance);
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
            SetHiddenImpostors(__instance);
        }
    }
}