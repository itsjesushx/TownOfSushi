﻿using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Networking.Attributes;
using TownOfSushi.Modifiers.Game.Universal;
using TownOfSushi.Options.Modifiers.Universal;
using UnityEngine;

namespace TownOfSushi.Buttons.Modifiers;

public sealed class BarryButton : TownOfSushiButton
{
    public override string Name => "Button";
    public override string Keybind => Keybinds.ModifierAction;
    public override Color TextOutlineColor => TownOfSushiColors.ButtonBarry;
    public override float Cooldown => OptionGroupSingleton<ButtonBarryOptions>.Instance.Cooldown + MapCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<ButtonBarryOptions>.Instance.MaxNumButtons;
    public override ButtonLocation Location => ButtonLocation.BottomLeft;
    public override LoadableAsset<Sprite> Sprite => TosAssets.BarryButtonSprite;
    public bool Usable { get; set; } = OptionGroupSingleton<ButtonBarryOptions>.Instance.FirstRoundUse;

    public override bool Enabled(RoleBehaviour? role)
    {
        return PlayerControl.LocalPlayer != null &&
               PlayerControl.LocalPlayer.HasModifier<ButtonBarryModifier>() &&
               PlayerControl.LocalPlayer.RemainingEmergencies > 0 &&
               !PlayerControl.LocalPlayer.Data.IsDead;
    }

    public override bool CanUse()
    {
        var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
        return base.CanUse() && Usable && PlayerControl.LocalPlayer.RemainingEmergencies > 0 && (OptionGroupSingleton<ButtonBarryOptions>.Instance.IgnoreSabo || system is { AnyActive: false });
    }

    protected override void OnClick()
    {
        CallButtonBarry(PlayerControl.LocalPlayer);
    }

    [MethodRpc((uint)TownOfSushiRpc.ButtonBarry, SendImmediately = true)]

    public static void CallButtonBarry(PlayerControl player)
    {
        if (AmongUsClient.Instance.AmHost)
        {
            MeetingRoomManager.Instance.AssignSelf(player, null);

            if (GameManager.Instance.CheckTaskCompletion())
            {
                return;
            }

            HudManager.Instance.OpenMeetingRoom(player);
            player.RpcStartMeeting(null);
        }
    }
}
