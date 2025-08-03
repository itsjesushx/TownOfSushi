﻿using System.Collections;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using TownOfSushi.Modifiers.Game.Universal;
using TownOfSushi.Modules;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Impostor;

public sealed class DisperserModifier : TosGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Disperser";
    public override string GetDescription() => "Separate the Crew.";
    public override LoadableAsset<Sprite>? ModifierIcon => TosModifierIcons.Disperser;
    public override ModifierFaction FactionType => ModifierFaction.ImpostorUtility;

    public override int GetAssignmentChance() => (int)OptionGroupSingleton<ImpostorModifierOptions>.Instance.DisperserChance;
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<ImpostorModifierOptions>.Instance.DisperserAmount;

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsImpostor() && !role.Player.GetModifierComponent().HasModifier<SatelliteModifier>(true) && !role.Player.GetModifierComponent().HasModifier<ButtonBarryModifier>(true);
    }

    public static IEnumerator CoDisperse(Dictionary<byte, Vector2> coordinates)
    {
        yield return HudManager.Instance.CoFadeFullScreen(Color.clear, new Color(0.6f, 0.1f, 0.2f, 1f), 11f / 24f);
        yield return HudManager.Instance.CoFadeFullScreen(new Color(0.6f, 0.1f, 0.2f, 1f), Color.clear);

        DispersePlayersToCoordinates(coordinates);

        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>{TownOfSushiColors.ImpSoft.ToTextColor()}Everyone has been dispersed to a vent!</color></b>", Color.white, spr: TosModifierIcons.Disperser.LoadAsset());

        notif1.Text.SetOutlineThickness(0.35f);
        notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
    }

    public static void DispersePlayersToCoordinates(Dictionary<byte, Vector2> coordinates)
    {
        if (coordinates.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
        {
            if (Minigame.Instance)
            {
                try { Minigame.Instance.Close(); }
                catch { /* ignored */ }
            }

            if (PlayerControl.LocalPlayer.inVent)
            {
                PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
            }
        }

        foreach ((byte key, Vector2 value) in coordinates)
        {
            PlayerControl player = MiscUtils.PlayerById(key)!;
            player.transform.position = value;

            if (PlayerControl.LocalPlayer == player)
            {
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(value);
            }
        }

        if (PlayerControl.LocalPlayer.walkingToVent)
        {
            PlayerControl.LocalPlayer.inVent = false;
            Vent.currentVent = null;
            PlayerControl.LocalPlayer.moveable = true;
            PlayerControl.LocalPlayer.MyPhysics.StopAllCoroutines();
        }

        if (ModCompatibility.SubLoaded) ModCompatibility.ChangeFloor(PlayerControl.LocalPlayer.transform.position.y > -7f);
    }

    public static Dictionary<byte, Vector2> GenerateDisperseCoordinates()
    {
        var targets = PlayerControl.AllPlayerControls.ToArray().Where(player => !player.Data.IsDead && !player.Data.Disconnected).ToList();

        // players with the ImmovableModifier can't be dispersed
        targets.RemoveAll(x => x.HasModifier<ImmovableModifier>());

        var vents = UnityEngine.Object.FindObjectsOfType<Vent>();

        var coordinates = new Dictionary<byte, Vector2>(targets.Count);

        foreach (PlayerControl target in targets)
        {
            Vector3 destination = vents.Random()!.transform.position + new Vector3(0f, 0.3636f, 0f);
            coordinates.Add(target.PlayerId, destination);
        }

        return coordinates;
    }
    public string GetAdvancedDescription()
    {
        return
            "Disperse everyone on the map to a random vent, given that they are not Immovable. You cannot have any other button modifiers with Disperser.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Disperse",
            $"You can disperse players on the map to any vents, which you can do once per game.",
            TosAssets.DisperseSprite),
    ];
}
