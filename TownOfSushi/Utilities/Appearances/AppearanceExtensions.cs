﻿using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using TownOfSushi.Modifiers.Game.Universal;
using TownOfSushi.Options;
using UnityEngine;

namespace TownOfSushi.Utilities.Appearances;
public static class AppearanceExtensions
{
    public static void ResetAppearance(this PlayerControl player, bool override_checks = false, bool fullReset = false)
    {
        // swooper unswoop mid camo - needs testing
        if (OptionGroupSingleton<GeneralOptions>.Instance.CamouflageComms && player.GetAppearanceType() == TownOfSushiAppearances.Swooper)
        {
            var c = ShipStatus.Instance.Systems[SystemTypes.Comms];
            var active = c.TryCast<HudOverrideSystemType>()?.IsActive;
            if (active == null) active = c.TryCast<HqHudSystemType>()?.IsActive;
            if (active == true)
            {
                player.SetCamouflage(true);
                return;
            }
        }

        // preventing glitch from morphing -> camo -> unmorph early sorta thing...
        if (player.GetAppearanceType() == TownOfSushiAppearances.Camouflage && !override_checks) return;

        if (fullReset)
            player.RawSetAppearance(new VisualAppearance(player.GetDefaultAppearance(), TownOfSushiAppearances.Default)
            {
                Size = new Vector3(0.7f, 0.7f, 1f),
            });
        else player.RawSetAppearance(player.GetDefaultModifiedAppearance());

        // The "just in case" section
        player.SetHatAndVisorAlpha(1f);
        player.cosmetics.skin.layer.color = player.cosmetics.skin.layer.color.SetAlpha(1f);
        foreach (var rend in player.cosmetics.currentPet.renderers)
        {
            rend.color = rend.color.SetAlpha(1f);
        }

        foreach (var shadow in player.cosmetics.currentPet.shadows)
        {
            shadow.color = shadow.color.SetAlpha(1f);
        }
    }

    public static void SetCamouflage(this PlayerControl player, bool toggle = true)
    {
        if (toggle && player.GetAppearanceType() != TownOfSushiAppearances.Camouflage)
        {
            player.RawSetAppearance(new VisualAppearance(player.GetDefaultAppearance(), TownOfSushiAppearances.Camouflage)
            {
                ColorId = player.Data.DefaultOutfit.ColorId,
                HatId = string.Empty,
                SkinId = string.Empty,
                VisorId = string.Empty,
                PlayerName = string.Empty,
                PetId = string.Empty,
                NameVisible = false,
                PlayerMaterialColor = Color.grey,
                Size = player.GetAppearance().Size,
            });
        }
        else if (!toggle)
        {
            player.ResetAppearance(true);
            player.cosmetics.ToggleNameVisible(true);
        }
    }
    public static void RawSetAppearance(this PlayerControl player, IVisualAppearance iVisualAppearance)
    {
        player.RawSetAppearance(iVisualAppearance.GetVisualAppearance()!);
    }

    public static void RawSetAppearance(this PlayerControl player, VisualAppearance appearance)
    {
        player.RawSetName(appearance.PlayerName);
        player.RawSetColor(appearance.ColorId);
        player.RawSetHat(appearance.HatId, appearance.ColorId);
        player.RawSetSkin(appearance.SkinId, appearance.ColorId);
        player.RawSetVisor(appearance.VisorId, appearance.ColorId);
        player.RawSetPet(appearance.PetId, appearance.ColorId);

        player.cosmetics.currentBodySprite.BodySprite.color = appearance.RendererColor;

        if (appearance.PlayerMaterialColor != null)
        {
            PlayerMaterial.SetColors((Color)appearance.PlayerMaterialColor, player.cosmetics.currentBodySprite.BodySprite);
        }

        if (appearance.NameColor != null)
        {
            player.cosmetics.nameText.color = (Color)appearance.NameColor;
        }
        else if (player.IsImpostor())
        {
            player.cosmetics.nameText.color = TownOfSushiColors.Impostor;
        }

        player.cosmetics.ToggleNameVisible(appearance.NameVisible);

        player.cosmetics.colorBlindText.color = appearance.ColorBlindTextColor;

        player.transform.localScale = appearance.Size;

        if (player.CurrentOutfitType != 0)
        {
            player.Data.Outfits.Remove(player.CurrentOutfitType);
        }
        player.CurrentOutfitType = (PlayerOutfitType)appearance.AppearanceType;
        if (appearance.AppearanceType != 0)
        {
            player.Data.SetOutfit(player.CurrentOutfitType, appearance);
        }
    }

    public static TownOfSushiAppearances GetAppearanceType(this PlayerControl player)
    {
        return (TownOfSushiAppearances)player.CurrentOutfitType;
    }

    public static VisualAppearance GetAppearance(this PlayerControl player)
    {
        var appearance = player.GetDefaultModifiedAppearance();

        if (player.Data.Role is IVisualAppearance visualRole)
        {
            appearance = visualRole.GetVisualAppearance()!;
        }

        if (player.GetModifiers<BaseModifier>().FirstOrDefault(x => x is IVisualAppearance
            {
                VisualPriority: false
            }) is IVisualAppearance visualMod2 &&
            visualMod2.GetVisualAppearance() != null) appearance = visualMod2.GetVisualAppearance()!;

        if (player.GetModifiers<BaseModifier>().FirstOrDefault(x => x is IVisualAppearance { VisualPriority: true }) is
                IVisualAppearance { VisualPriority: true } visualMod &&
            visualMod.GetVisualAppearance() != null) appearance = visualMod.GetVisualAppearance()!;

        return appearance;
    }

    public static VisualAppearance GetDefaultAppearance(this PlayerControl playerControl)
    {
        return new VisualAppearance(playerControl.Data.DefaultOutfit, TownOfSushiAppearances.Default);
    }
    public static VisualAppearance GetDefaultModifiedAppearance(this PlayerControl playerControl)
    {
        var appearance = new VisualAppearance(playerControl.Data.DefaultOutfit, TownOfSushiAppearances.Default);
        if (playerControl.HasModifier<MiniModifier>()) appearance = playerControl.GetModifier<MiniModifier>()!.GetVisualAppearance()!;
        else if (playerControl.HasModifier<GiantModifier>()) appearance = playerControl.GetModifier<GiantModifier>()!.GetVisualAppearance()!;
        else if (playerControl.HasModifier<FlashModifier>()) appearance = playerControl.GetModifier<FlashModifier>()!.GetVisualAppearance();
        return appearance;
    }
}
