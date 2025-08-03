﻿using AmongUs.GameOptions;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using TownOfSushi.Modules;
using TownOfSushi.Options;

namespace TownOfSushi.Patches.Options;

[HarmonyPatch]
public static class MapPatches
{
    [MethodRpc((uint)TownOfSushiRpc.SetMap, SendImmediately = true, LocalHandling = RpcLocalHandling.Before)]
    public static void RpcSetMap(PlayerControl player, byte mapId)
    {
        if (!player.IsHost())
        {
            Logger<TownOfSushiPlugin>.Error("Only the host can change the map.");
            return;
        }

        GameOptionsManager.Instance.CurrentGameOptions.SetByte(ByteOptionNames.MapId, mapId);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.ReallyBegin))]
    public static void SetNewMapPrefix()
    {
        var map = GetSelectedMap();
        RpcSetMap(PlayerControl.LocalPlayer, map);
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.ToggleNeighborVentBeingCleaned))]
    [HarmonyPatch(typeof(Vent), nameof(Vent.UpdateArrows))]
    [HarmonyPrefix]
    public static bool DisableVentCleaning()
    {
        return GameOptionsManager.Instance.CurrentGameOptions.MapId != 3;
    }

    private static byte GetSelectedMap()
    {
        if (!OptionGroupSingleton<TownOfSushiMapOptions>.Instance.RandomMaps)
        {
            return GameOptionsManager.Instance.CurrentGameOptions.MapId;
        }

        var skeldChance = OptionGroupSingleton<TownOfSushiMapOptions>.Instance.SkeldChance.Value;
        var miraChance = OptionGroupSingleton<TownOfSushiMapOptions>.Instance.MiraChance.Value;
        var polusChance = OptionGroupSingleton<TownOfSushiMapOptions>.Instance.PolusChance.Value;
        var airshipChance = OptionGroupSingleton<TownOfSushiMapOptions>.Instance.AirshipChance.Value;
        var fungleChance = OptionGroupSingleton<TownOfSushiMapOptions>.Instance.FungleChance.Value;
        var submergedChance = OptionGroupSingleton<TownOfSushiMapOptions>.Instance.SubmergedChance.Value;
        var liChance = OptionGroupSingleton<TownOfSushiMapOptions>.Instance.LevelImpostorChance.Value;

        Random rnd = new();
        float totalWeight = 0;

        totalWeight += skeldChance;
        totalWeight += miraChance;
        totalWeight += polusChance;
        totalWeight += airshipChance;
        totalWeight += fungleChance;

        // totalWeight += ModCompatibility.UnlockDleksLoaded ? OptionGroupSingleton<Options.MapOptions>.Instance.dlekSChance : 0;
        totalWeight += ModCompatibility.SubLoaded ? submergedChance : 0;
        totalWeight += ModCompatibility.LILoaded ? liChance : 0;

        if ((int)totalWeight == 0)
        {
            return GameOptionsManager.Instance.currentNormalGameOptions.MapId;
        }

        float randomNumber = rnd.Next(0, (int)totalWeight);

        if (randomNumber < skeldChance)
        {
            return 0;
        }

        randomNumber -= skeldChance;

        if (randomNumber < miraChance)
        {
            return 1;
        }

        randomNumber -= miraChance;

        if (randomNumber < polusChance)
        {
            return 2;
        }

        randomNumber -= polusChance;

        if (randomNumber < airshipChance)
        {
            return 4;
        }

        randomNumber -= airshipChance;

        if (randomNumber < fungleChance)
        {
            return 5;
        }

        randomNumber -= fungleChance;

        // if (ModCompatibility.UnlockDleksLoaded && randomNumber < OptionGroupSingleton<Options.MapOptions>.Instance.dlekSChance) return 3;
        // randomNumber -= OptionGroupSingleton<Options.MapOptions>.Instance.dlekSChance;

        if (ModCompatibility.SubLoaded && randomNumber < submergedChance)
        {
            return 6;
        }

        randomNumber -= submergedChance;

        if (ModCompatibility.LILoaded && randomNumber < liChance)
        {
            return 7;
        }

        return GameOptionsManager.Instance.currentNormalGameOptions.MapId;
    }
}