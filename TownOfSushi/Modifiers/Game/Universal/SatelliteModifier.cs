﻿using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using TownOfSushi.Buttons.Modifiers;
using TownOfSushi.Modules.Anims;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Options.Modifiers.Universal;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class SatelliteModifier : UniversalGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Satellite";
    public override LoadableAsset<Sprite>? ModifierIcon => TosModifierIcons.Satellite;
    public override string GetDescription() => "You can broadcast a signal to detect all dead bodies on the map.";
    public override int GetAssignmentChance() => (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.SatelliteChance;
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.SatelliteAmount;
    public override ModifierFaction FactionType => ModifierFaction.UniversalUtility;
    private readonly List<PlayerControl> CastedPlayers = [];
    private readonly List<SpriteRenderer> CastedIcons = [];
    public int Priority { get; set; } = 5;

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role is not MysticRole;
    }
    public void OnRoundStart()
    {
        CustomButtonSingleton<SatelliteButton>.Instance.Usable = true;
        ClearMapIcons();
    }

    public void NewMapIcon(PlayerControl player)
    {
        if (!CastedPlayers.Contains(player))
        {
            var newIcon = UnityEngine.Object.Instantiate(MapBehaviour.Instance.TrackedHerePoint);
            newIcon.material = AnimStore.SetSpriteColourMatch(player, newIcon.material);

            Vector3 vector = player.transform.position;
            vector /= ShipStatus.Instance.MapScale;
            vector.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
            vector.z = -1f;

            newIcon.transform.localPosition = vector;

            CastedPlayers.Add(player);
            CastedIcons.Add(newIcon);
        }
    }

    public void ClearMapIcons()
    {
        foreach (var gameObject in CastedIcons.Select(icon => icon.gameObject).Where(gameObject => gameObject != null))
        {
            gameObject.Destroy();
        }
        CastedIcons.Clear();
    }
    public string GetAdvancedDescription()
    {
        return "You can broadcast a signal to know where dead bodies are."
               + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Broadcast",
            $"You can check for bodies on the map, which you can do {OptionGroupSingleton<SatelliteOptions>.Instance.MaxNumCast} time(s) per game.",
            TosAssets.BroadcastSprite),
    ];
}
