using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class BlackmailerBlackmailButton : TownOfSushiRoleButton<BlackmailerRole, PlayerControl>,
    IAftermathablePlayerButton
{
    public override string Name => "Blackmail";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<BlackmailerOptions>.Instance.BlackmailCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<BlackmailerOptions>.Instance.MaxBlackmails;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.BlackmailSprite;
    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        BlackmailerRole.RpcBlackmail(PlayerControl.LocalPlayer, Target);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false,
            player => !player.HasModifier<BlackmailedModifier>() && !player.HasModifier<BlackmailSparedModifier>());
    }
}