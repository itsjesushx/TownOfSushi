using MiraAPI.Networking;
using Reactor.Utilities;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class JesterKillButton : TownOfSushiRoleButton<JesterRole, PlayerControl>, IKillButton
{
    public override string Name => "Kill";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Jester;
    public override float Cooldown => 20f;
    public override int MaxUses => 1;
    public override LoadableAsset<Sprite> Sprite => TownOfSushiAssets.KillSprite;
    public override PlayerControl? GetTarget()
    {
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, x => !x.IsLover());
        }
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }
    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && OptionGroupSingleton<JesterOptions>.Instance.JesterCanKill && MaxUses > 0;
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Jester Shoot: Target is null");
            return;
        }

        PlayerControl.LocalPlayer.RpcCustomMurder(Target);
    }
}