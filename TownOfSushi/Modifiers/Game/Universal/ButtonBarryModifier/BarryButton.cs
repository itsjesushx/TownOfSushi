using MiraAPI.Hud;
using Reactor.Networking.Attributes;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class BarryButton : TownOfSushiButton
{
    public override string Name => "Button";
    public override BaseKeybind Keybind => Keybinds.ModifierAction;
    public override Color TextOutlineColor => TownOfSushiColors.ButtonBarry;
    public override float Cooldown => OptionGroupSingleton<ButtonBarryOptions>.Instance.Cooldown + MapCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<ButtonBarryOptions>.Instance.MaxNumButtons;
    public override ButtonLocation Location => ButtonLocation.BottomLeft;
    public override LoadableAsset<Sprite> Sprite => TOSAssets.BarryButtonSprite;

    public bool Usable { get; set; } = OptionGroupSingleton<ButtonBarryOptions>.Instance.FirstRoundUse ||
                                       TutorialManager.InstanceExists;

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
        return base.CanUse() && Usable && PlayerControl.LocalPlayer.RemainingEmergencies > 0 &&
               (OptionGroupSingleton<ButtonBarryOptions>.Instance.IgnoreSabo || system is { AnyActive: false });
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