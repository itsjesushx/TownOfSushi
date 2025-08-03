using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Neutral;

public sealed class AmnesiacRememberButton : TownOfSushiRoleButton<AmnesiacRole, DeadBody>
{
    public override string Name => "Remember";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Amnesiac;
    public override float Cooldown => 0.001f + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TosNeutAssets.RememberButtonSprite;

    public override DeadBody? GetTarget() => PlayerControl.LocalPlayer.GetNearestDeadBody(Distance);

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        var targetId = Target.ParentId;
        var targetPlayer = MiscUtils.PlayerById(targetId);

        if (targetPlayer == null) return; // Someone may have left mid game or something and gc just vacuumed, but idk. better safe than sorry ig.

        AmnesiacRole.RpcRemember(PlayerControl.LocalPlayer, targetPlayer);
    }
}
