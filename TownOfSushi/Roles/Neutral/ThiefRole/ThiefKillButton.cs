using MiraAPI.Networking;
using Reactor.Utilities;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class ThiefKillButton : TownOfSushiRoleButton<ThiefRole, PlayerControl>, IDiseaseableButton,
    IKillButton
{
    public override string Name => "Kill";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Thief;
    public override float Cooldown => OptionGroupSingleton<ThiefOptions>.Instance.KillCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSAssets.KillSprite;
    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    public override PlayerControl? GetTarget()
    {
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, x => !x.IsLover());
        }
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Thief Shoot: Target is null");
            return;
        }

        if (Target.Data.Role is ThiefRole)
        {
            return;
        }

        if (Target.IsCrewmate() || Target.IsNeutral() && !Target.Is(RoleAlignment.NeutralKilling))
        {
            PlayerControl.LocalPlayer.RpcCustomMurder(PlayerControl.LocalPlayer);
        }
        else
        {
            PlayerControl.LocalPlayer.RpcCustomMurder(Target);
        }
    }
}