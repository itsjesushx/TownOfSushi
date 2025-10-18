using MiraAPI.Networking;
using Reactor.Utilities;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class SoulCollectorReapButton : TownOfSushiRoleButton<SoulCollectorRole, PlayerControl>, IDiseaseableButton,
    IKillButton
{
    public override string Name => "Reap";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.SoulCollector;
    public override float Cooldown => OptionGroupSingleton<SoulCollectorOptions>.Instance.KillCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.ReapSprite;

    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Soul Collector Reap: Target is null");
            return;
        }

        PlayerControl.LocalPlayer.RpcCustomMurder(Target, createDeadBody: false);

        var notif1 = Helpers.CreateAndShowNotification(
            MiscUtils.ColorString(TownOfSushiColors.SoulCollector, $"<b>You have taken {Target.Data.PlayerName}'s soul from their body, leaving a soulless player behind.</b>"),
            Color.white, spr: TOSRoleIcons.SoulCollector.LoadAsset());

        
        notif1.AdjustNotification();
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }
}