﻿using MiraAPI.GameOptions;
using MiraAPI.Networking;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfSushi.Options.Modifiers.Alliance;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Neutral;

public sealed class PredatorKillButton : TownOfSushiRoleButton<PredatorRole, PlayerControl>, IDiseaseableButton,
    IKillButton
{
    public override string Name => "Kill";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Predator;
    public override float Cooldown => OptionGroupSingleton<PredatorOptions>.Instance.TerminateKillCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.PredatorKillSprite;

    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is PredatorRole { Terminating: true };
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Predator Shoot: Target is null");
            return;
        }

        PlayerControl.LocalPlayer.RpcCustomMurder(Target);
    }

    public override PlayerControl? GetTarget()
    {
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, x => !x.IsLover());
        }
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }
}