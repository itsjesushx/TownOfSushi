﻿using AmongUs.Data;
using TownOfSushi.Utilities.Appearances;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class ShyModifier : UniversalGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Shy";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Shy;

    public override ModifierFaction FactionType => ModifierFaction.UniversalVisibility;
    public override Color FreeplayFileColor => new Color32(180, 180, 180, 255);

    private static float FinalTransparency => OptionGroupSingleton<ShyOptions>.Instance.FinalTransparency;
    private static float InvisDelay => OptionGroupSingleton<ShyOptions>.Instance.InvisDelay + 0.01f;

    private static float TransformInvisDuration =>
        OptionGroupSingleton<ShyOptions>.Instance.TransformInvisDuration + 0.01f;

    private DateTime LastMoved { get; set; }
    private bool StopShy { get; set; }

    public string GetAdvancedDescription()
    {
        return
            "You blend in with the environment, becoming transparent when staying still."
            + MiscUtils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "You become transparent when \nstanding still for a short duration.";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<ShyOptions>.Instance.ShyChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<ShyOptions>.Instance.ShyAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        var isValid = true;
        if ((role is JesterRole && OptionGroupSingleton<JesterOptions>.Instance.ScatterOn) ||
            (role is AmnesiacRole && OptionGroupSingleton<AmnesiacOptions>.Instance.ScatterOn))
        {
            isValid = false;
        }

        return base.IsModifierValidOn(role) && isValid;
    }

    public override void OnDeactivate()
    {
        if (Player == null)
        {
            return;
        }

        SetVisibility(Player, 1f);
    }

    public void OnRoundStart()
    {
        if (Player.HasDied())
        {
            return;
        }

        LastMoved = DateTime.UtcNow;
        SetVisibility(Player, 1f);
    }

    public override void Update()
    {
        if (IntroCutscene.Instance)
        {
            return;
        }

        if (Player == null)
        {
            return;
        }

        if (PlayerControl.LocalPlayer == null)
        {
            return;
        }

        if (Player.HasDied())
        {
            if (!StopShy)
            {
                StopShy = true;
                SetVisibility(Player, 1f);
            }
            return;
        }

        StopShy = false;

        // check movement by animation
        var playerPhysics = Player.MyPhysics;
        var currentPhysicsAnim = playerPhysics.Animations.Animator.GetCurrentAnimation();
        if (currentPhysicsAnim != playerPhysics.Animations.group.IdleAnim)
        {
            LastMoved = DateTime.UtcNow;
        }

        if (Player.GetAppearanceType() == TownOfSushiAppearances.Swooper)
        {
            var opacity = 0f;

            if ((PlayerControl.LocalPlayer.IsImpostor() && Player.Data.Role is SwooperRole) ||
                (Player.AmOwner && Player.Data.Role is SwooperRole))
            {
                opacity = 0.1f;
            }

            SetVisibility(Player, opacity, true);
        }
        else if (Player.GetAppearanceType() == TownOfSushiAppearances.Camouflage)
        {
            SetVisibility(Player, 1f, true);
        }
        else if (Player.GetAppearanceType() == TownOfSushiAppearances.Morph || Player.GetAppearanceType() == TownOfSushiAppearances.Mimic)
        {
            SetVisibility(Player, 1f);
        }
        else
        {
            var timeSpan = DateTime.UtcNow - LastMoved;

            if (timeSpan.TotalMilliseconds / 1000f < InvisDelay)
            {
                SetVisibility(Player, 1f);
            }
            else if (timeSpan.TotalMilliseconds / 1000f < TransformInvisDuration + InvisDelay)
            {
                timeSpan = DateTime.UtcNow - LastMoved.AddSeconds(InvisDelay);
                var opacity = 1f - (float)timeSpan.TotalMilliseconds / 1000f / TransformInvisDuration *
                    (100f - FinalTransparency) / 100f;
                SetVisibility(Player, opacity);
            }
            else
            {
                var opacity = FinalTransparency / 100;
                SetVisibility(Player, opacity);
            }
        }
        if (Player.HasDied())
        {
            SetVisibility(Player, 1f);
        }
    }

    public static void SetVisibility(PlayerControl player, float transparency, bool hideName = false)
    {
        var colour = player.cosmetics.currentBodySprite.BodySprite.color;
        var cosmetics = player.cosmetics;

        colour.a = transparency;
        player.cosmetics.currentBodySprite.BodySprite.color = colour;

        if (hideName)
        {
            transparency = 0f;
        }

        cosmetics.nameText.color = cosmetics.nameText.color.SetAlpha(transparency);

        if (DataManager.Settings.Accessibility.ColorBlindMode)
        {
            cosmetics.colorBlindText.color = cosmetics.colorBlindText.color.SetAlpha(transparency);
        }

        player.SetHatAndVisorAlpha(transparency);
        cosmetics.skin.layer.color = cosmetics.skin.layer.color.SetAlpha(transparency);
        if (player.cosmetics.currentPet != null)
        {
            foreach (var rend in player.cosmetics.currentPet.renderers)
            {
                rend.color = rend.color.SetAlpha(transparency);
            }

            foreach (var shadow in player.cosmetics.currentPet.shadows)
            {
                shadow.color = shadow.color.SetAlpha(transparency);
            }
        }

        foreach (var animation in player.transform.GetChild(2).GetComponentsInParent<SpriteRenderer>())
        {
            animation.color = animation.color.SetAlpha(transparency);
        }

        foreach (var animation in player.transform.GetChild(2).GetComponentsInChildren<SpriteRenderer>())
        {
            animation.color = animation.color.SetAlpha(transparency);
        }
    }
}