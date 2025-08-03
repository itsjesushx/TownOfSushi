﻿using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using TownOfSushi.Utilities.Appearances;
using UnityEngine;

namespace TownOfSushi.Buttons.Crewmate;

public sealed class TrackerTrackButton : TownOfSushiRoleButton<TrackerTOSRole, PlayerControl>
{
    public override string Name => "Track";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Tracker;
    public override float Cooldown => OptionGroupSingleton<TrackerOptions>.Instance.TrackCooldown + MapCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<TrackerOptions>.Instance.MaxTracks;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.TrackSprite;
    public int ExtraUses { get; set; }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target!.HasModifier<TrackerArrowTargetModifier>();
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Track: Target is null");
            return;
        }

        Color color = Palette.PlayerColors[Target.GetDefaultAppearance().ColorId];
        var update = OptionGroupSingleton<TrackerOptions>.Instance.UpdateInterval;

        Target.AddModifier<TrackerArrowTargetModifier>(PlayerControl.LocalPlayer, color, update);

        TOSAudio.PlaySound(TOSAudio.TrackerActivateSound);
    }
}