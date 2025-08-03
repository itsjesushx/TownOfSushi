﻿using System.Collections;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfSushi.Events;
using TownOfSushi.Modifiers;
using TownOfSushi.Options.Roles.Impostor;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modules;

// Code Review: Should be using a MonoBehaviour
public sealed class Bomb : IDisposable
{
    private PlayerControl? _bomber;
    private GameObject? _obj;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Detonate()
    {
        Coroutines.Start(CoDetonate());
    }

    private IEnumerator CoDetonate()
    {
        _bomber?.RpcAddModifier<IndirectAttackerModifier>(false);
        yield return new WaitForSeconds(0.1f);

        var radius = OptionGroupSingleton<BomberOptions>.Instance.DetonateRadius * ShipStatus.Instance.MaxLightRadius;

        var affected = Helpers.GetClosestPlayers(_obj!.transform.position, radius);

        affected.Shuffle();

        while (affected.Count > OptionGroupSingleton<BomberOptions>.Instance.MaxKillsInDetonation)
        {
            affected.Remove(affected[^1]);
        }

        if (MeetingHud.Instance || ExileController.Instance)
        {
            _bomber?.RpcRemoveModifier<IndirectAttackerModifier>();

            _obj.Destroy();
            yield return new WaitForSeconds(0.1f);
            foreach (var player in affected)
            {
                if (!player.HasDied() || _bomber == null)
                    continue;
                DeathHandlerModifier.RpcUpdateDeathHandler(player, "Bombed", DeathEventHandlers.CurrentRound,
                    DeathHandlerOverride.SetTrue, $"By {_bomber.Data.PlayerName}", lockInfo: DeathHandlerOverride.SetTrue);
            }
            yield break;
        }

        foreach (var player in affected)
        {
            if (player.HasDied())
            {
                continue;
            }

            if (player.HasModifier<BaseShieldModifier>() && _bomber == player)
            {
                continue;
            }

            if (player.HasModifier<FirstDeadShield>() && _bomber == player)
            {
                continue;
            }

            _bomber?.RpcCustomMurder(player, teleportMurderer: false);
        }

        _bomber?.RpcRemoveModifier<IndirectAttackerModifier>();

        _obj.Destroy();
    }

    public static Bomb CreateBomb(PlayerControl player, Vector3 location)
    {
        return new Bomb
        {
            _obj = MiscUtils.CreateSpherePrimitive(location,
                OptionGroupSingleton<BomberOptions>.Instance.DetonateRadius),
            _bomber = player
        };
    }

    public static IEnumerator BombShowTeammate(PlayerControl player, Vector3 location)
    {
        var bomb = CreateBomb(player, location);

        yield return new WaitForSeconds(OptionGroupSingleton<BomberOptions>.Instance.DetonateDelay);

        try
        {
            bomb.Destroy();
        }
        catch
        {
            /* ignored */
        }
    }

    public void Destroy()
    {
        Dispose();
    }

    private void Dispose(bool disposing)
    {
        if (disposing && _obj != null)
        {
            _obj.Destroy();
        }
    }
}