using MiraAPI.Networking;
using Reactor.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Neutral;

public sealed class WerewolfMaulButton : TownOfSushiRoleButton<WerewolfRole, PlayerControl>, IDiseaseableButton, IKillButton
{
    public override string Name => "Maul";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Werewolf;
    public override float Cooldown => OptionGroupSingleton<WerewolfOptions>.Instance.MaulCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.MaulSprite;
    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    public override bool Enabled(RoleBehaviour? role) => role is WerewolfRole;
    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Werewolf Shoot: Target is null");
            return;
        }

        PlayerControl.LocalPlayer.RpcCustomMurder(Target);

        WerewolfRole.RpcMaulMurder(Role.Player);
    }
    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }
}