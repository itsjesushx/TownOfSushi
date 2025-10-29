using MiraAPI.Networking;
using Reactor.Networking.Attributes;
using TownOfSushi.Buttons;
using TownOfSushi.Modifiers;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class ArsonistIgniteButton : TownOfSushiRoleButton<ArsonistRole>
{
    public override string Name => "Ignite";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Arsonist;
    public override float Cooldown => 10f;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.IgniteButtonSprite;
    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Role is { DousedEveryone: true };
    }

    protected override void OnClick()
    {
        PlayerControl.LocalPlayer.RpcAddModifier<IndirectAttackerModifier>(false);
        var dousedPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()
        && x.HasModifier<ArsonistDousedModifier>()).ToList();

        foreach (var doused in dousedPlayers)
        {
            if (doused.HasModifier<FirstDeadShield>())
            {
                continue;
            }

            if (doused.HasModifier<BaseShieldModifier>())
            {
                continue;
            }

            PlayerControl.LocalPlayer.RpcCustomMurder(doused, resetKillTimer: false, teleportMurderer: false,
                playKillSound: false);
            RpcIgniteSound(doused);
        }

        PlayerControl.LocalPlayer.RpcRemoveModifier<IndirectAttackerModifier>();

        TOSAudio.PlaySound(TOSAudio.ArsoIgniteSound);
        Role.Wins = true;
    }

    [MethodRpc((uint)TownOfSushiRpc.IgniteSound2, SendImmediately = true)]
    public static void RpcIgniteSound(PlayerControl player)
    {
        if (player.AmOwner)
        {
            TOSAudio.PlaySound(TOSAudio.ArsoIgniteSound);
        }
    }
}