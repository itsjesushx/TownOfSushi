using MiraAPI.Networking;
using Reactor.Utilities;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class PyromaniacKillButton : TownOfSushiRoleButton<PyromaniacRole, PlayerControl>, IDiseaseableButton,
    IKillButton
{
    public override string Name => "Blow Up";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Pyromaniac;
    public override float Cooldown => Target != null && Target.HasModifier<PyromaniacDousedModifier>()
        ? OptionGroupSingleton<PyromaniacOptions>.Instance.DousedKillCooldown
        : OptionGroupSingleton<PyromaniacOptions>.Instance.RegularKillCooldown;
    public override LoadableAsset<Sprite> Sprite => TownOfSushiAssets.IgniteButtonSprite;
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
            Logger<TownOfSushiPlugin>.Error("Pyromaniac Shoot: Target is null");
            return;
        }

        PlayerControl.LocalPlayer.RpcCustomMurder(Target);
    }
}