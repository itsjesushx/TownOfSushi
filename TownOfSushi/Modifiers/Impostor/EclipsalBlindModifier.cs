﻿using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using Reactor.Utilities.Extensions;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Modules.Anims;
using TownOfSushi.Options;
using TownOfSushi.Options.Roles.Impostor;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Impostor;

public sealed class EclipsalBlindModifier(PlayerControl player) : DisabledModifier
{
    public override string ModifierName => "Blinded";
    public override bool HideOnUi => true;
    public override float Duration => OptionGroupSingleton<EclipsalOptions>.Instance.BlindDuration;
    public override bool AutoStart => true;
    public PlayerControl Eclipsal => player;
    public GameObject? EclipseBack { get; set; }

    public float VisionPerc { get; set; } = 1f;

    public override void OnActivate()
    {
        base.OnActivate();
        var TosAbilityEvent = new TosAbilityEvent(AbilityType.EclipsalBlind, Eclipsal, Player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);

        VisionPerc = 1f;
        
        if (PlayerControl.LocalPlayer.IsImpostor() || (PlayerControl.LocalPlayer.HasDied() && OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow))
        {
            Player.cosmetics.currentBodySprite.BodySprite.material.SetColor(ShaderID.VisorColor, Palette.VisorColor);
        }
        EclipseBack = AnimStore.SpawnAnimBody(Player, TosAssets.EclipsedPrefab.LoadAsset(), false, -1.1f, -0.575f)!;
        EclipseBack.SetActive(false);
        if (Player.AmOwner && !Eclipsal.AmOwner)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{TownOfSushiColors.ImpSoft.ToTextColor()}You were blinded by an Eclipsal!</color></b>", Color.white, spr: TosRoleIcons.Eclipsal.LoadAsset());

            notif1.Text.SetOutlineThickness(0.35f);
            notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        base.OnDeath(reason);

        ModifierComponent!.RemoveModifier(this);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        var opts = OptionGroupSingleton<EclipsalOptions>.Instance;

        if (PlayerControl.LocalPlayer.IsImpostor())
        {
            Player.cosmetics.currentBodySprite.BodySprite.material.SetColor(ShaderID.VisorColor, Color.black);
        }

        if (TimeRemaining > opts.BlindDuration - 1f)
        {
            VisionPerc = TimeRemaining - opts.BlindDuration + 1f;
        }
        else if (TimeRemaining < 1f)
        {
            VisionPerc = 1f - TimeRemaining;
        }
        else
        {
            VisionPerc = 0f;
        }
        EclipseBack?.SetActive(false);
        if ((PlayerControl.LocalPlayer.IsImpostor() || (PlayerControl.LocalPlayer.HasDied() && OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow)) && EclipseBack?.gameObject != null)
        {
            var visible = true;
            
            if (Player.HasModifier<ConcealedModifier>() || !Player.Visible || (Player.TryGetModifier<DisabledModifier>(out var mod) && !mod.IsConsideredAlive) || Player.inVent)
            {
                visible = false;
            }
            Player.cosmetics.currentBodySprite.BodySprite.material.SetColor(ShaderID.VisorColor, Color.black);
            EclipseBack?.SetActive(visible);
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();

        VisionPerc = 1f;

        if (PlayerControl.LocalPlayer.IsImpostor() || (PlayerControl.LocalPlayer.HasDied() && OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow))
        {
            Player.cosmetics.currentBodySprite.BodySprite.material.SetColor(ShaderID.VisorColor, Palette.VisorColor);
        }
        if (EclipseBack?.gameObject != null)
        {
            EclipseBack.gameObject.Destroy();
        }
    }
}
