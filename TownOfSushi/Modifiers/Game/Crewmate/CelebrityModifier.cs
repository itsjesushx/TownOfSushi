﻿using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Roles.Impostor;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Utilities;
using TownOfSushi.Utilities.Appearances;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class CelebrityModifier : TosGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Celebrity";
    public override LoadableAsset<Sprite>? ModifierIcon => TosModifierIcons.Celebrity;
    public override string GetDescription() => "Announce how you died on your passing.";
    public override ModifierFaction FactionType => ModifierFaction.CrewmatePostmortem;
    public override int GetAssignmentChance() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.CelebrityChance;
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.CelebrityAmount != 0 ? 1 : 0;
    public List<CustomButtonWikiDescription> Abilities { get; } = [];
    
    public DateTime DeathTime { get; set; }
    public float DeathTimeMilliseconds { get; set; }
    public string DeathMessage { get; set; }
    public string AnnounceMessage { get; set; }
    public string StoredRoom { get; set; }
    public bool Announced { get; set; }

    public string GetAdvancedDescription()
    {
        return "After you die, details about your death will be revealed such as where you were killed and which role killed you during the meeting.";
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsCrewmate();
    }

    public static void CelebrityKilled(PlayerControl source, PlayerControl player, string customDeath = "")
    {
        if (!player.HasModifier<CelebrityModifier>())
        {
            Logger<TownOfSushiPlugin>.Error("RpcCelebrityKilled - Invalid Celebrity");
            return;
        }

        PlainShipRoom? plainShipRoom = null;

        var allRooms2 = ShipStatus.Instance.FastRooms;
        foreach (PlainShipRoom plainShipRoom2 in allRooms2.Values)
        {
            if (plainShipRoom2.roomArea && plainShipRoom2.roomArea.OverlapPoint(player.GetTruePosition()))
            {
                plainShipRoom = plainShipRoom2;
            }
        }

        var room = plainShipRoom != null ? TranslationController.Instance.GetString(plainShipRoom.RoomId) : "Outside/Hallway";

        var celeb = player.GetModifier<CelebrityModifier>()!;
        celeb.StoredRoom = room;
        celeb.DeathTime = DateTime.UtcNow;

        celeb.AnnounceMessage = $"<size=90%>The Celebrity, {player.GetDefaultAppearance().PlayerName}, has died!</size>\n<size=70%>(Details in chat)</size>";

        var cod = "killed";
        switch (source.Data.Role)
        {
            case SheriffRole or HunterRole or VeteranRole:
                cod = "shot";
                break;
            case InquisitorRole:
                cod = "vanquished";
                break;
            case ArsonistRole:
                cod = "ignited";
                break;
            case GlitchRole:
                cod = "bugged";
                break;
            case JuggernautRole:
                cod = "destroyed";
                break;
            case PestilenceRole:
                cod = "diseased";
                break;
            case SoulCollectorRole:
                cod = "reaped";
                break;
            case VampireRole:
                cod = "bit";
                break;
            case PredatorRole:
                cod = "terminated";
                break;
            case WerewolfRole:
                cod = "mauled";
                break;
            case ViperRole:
                cod = "poisoned";
                break;
        }
        if (customDeath != string.Empty && customDeath != "") cod = customDeath;
        if (MeetingHud.Instance)
        {
            celeb.Announced = true;
        }

        if (source == player)
            celeb.DeathMessage = $"The Celebrity, {player.GetDefaultAppearance().PlayerName}, was killed! Location: {celeb.StoredRoom}, Death: By Suicide, Time: ";
        else
            celeb.DeathMessage = $"The Celebrity, {player.GetDefaultAppearance().PlayerName}, was {cod}! Location: {celeb.StoredRoom}, Death: By the {source.Data.Role.NiceName}, Time: ";
    }

    [MethodRpc((uint)TownOfSushiRpc.UpdateCelebrityKilled, SendImmediately = true)]
    public static void RpcUpdateCelebrityKilled(PlayerControl player, float milliseconds)
    {
        if (!player.HasModifier<CelebrityModifier>())
        {
            Logger<TownOfSushiPlugin>.Error("RpcUpdateCelebrityKilled - Invalid Celebrity");
            return;
        }

        Logger<TownOfSushiPlugin>.Error($"RpcUpdateCelebrityKilled milliseconds: {milliseconds}");

        var celeb = player.GetModifier<CelebrityModifier>()!;

        celeb.DeathTimeMilliseconds = milliseconds;
    }
}
