using System.Collections;
using HarmonyLib;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfSushi.Modifiers.Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TownOfSushi.Patches;

[HarmonyPatch(typeof(DummyBehaviour))]
public static class DummyBehaviourPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(DummyBehaviour.Start))]
    public static void DummyStartPatch(DummyBehaviour __instance)
    {
        var dum = __instance.myPlayer;
        Coroutines.Start(TOSDummyMode(dum));
    }

    private static IEnumerator TOSDummyMode(PlayerControl dummy)
    {
        while (PlayerControl.LocalPlayer == null)
        {
            yield return null;
        }

        while (PlayerControl.LocalPlayer.Data == null)
        {
            yield return null;
        }

        while (PlayerControl.LocalPlayer.Data.Role == null)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.01f + 0.01f * dummy.PlayerId);
        var roleList = MiscUtils.AllRoles
            .Where(role => role is ICustomRole)
            .Where(role => !role.IsImpostor())
            .Where(role => role is not NeutralGhostRole)
            .Where(role => role is not CrewmateGhostRole)
            .Where(role => role is not ImpostorGhostRole)
            .Where(role => !role.TryCast<CrewmateGhostRole>())
            .Where(role => !role.TryCast<ImpostorGhostRole>())
            .ToList();

        PlayerControl.AllPlayerControls
            .ToArray()
            .Where(player => roleList.Contains(player.Data.Role))
            .ToList()
            .ForEach(player => roleList.Remove(player.Data.Role));

        var roleType = RoleId.Get(roleList.Random()!.GetType());
        dummy.RpcChangeRole(roleType);

        dummy.RpcSetName(AccountManager.Instance.GetRandomName());

        dummy.SetSkin(HatManager.Instance.allSkins[Random.Range(0, HatManager.Instance.allSkins.Count)].ProdId, 0);
        dummy.SetNamePlate(HatManager.Instance
            .allNamePlates[Random.RandomRangeInt(0, HatManager.Instance.allNamePlates.Count)].ProdId);
        dummy.SetPet(HatManager.Instance.allPets[Random.RandomRangeInt(0, HatManager.Instance.allPets.Count)].ProdId);
        var colorId = Random.Range(0, Palette.PlayerColors.Length);
        dummy.SetColor(colorId);
        dummy.SetHat(HatManager.Instance.allHats[Random.RandomRangeInt(0, HatManager.Instance.allHats.Count)].ProdId,
            colorId);
        dummy.SetVisor(
            HatManager.Instance.allVisors[Random.RandomRangeInt(0, HatManager.Instance.allVisors.Count)].ProdId,
            colorId);

        var randomUniMod = MiscUtils.AllModifiers.Where(x =>
            x is UniversalGameModifier touGameMod && touGameMod.IsModifierValidOn(dummy.Data.Role)).Random();
        if (randomUniMod != null)
        {
            dummy.RpcAddModifier(randomUniMod.GetType());
        }

        var randomTeamMod = MiscUtils.AllModifiers
            .Where(x => x is TOSGameModifier touGameMod && touGameMod.IsModifierValidOn(dummy.Data.Role)).Random();
        if (randomTeamMod != null)
        {
            dummy.RpcAddModifier(randomTeamMod.GetType());
        }
    }
}