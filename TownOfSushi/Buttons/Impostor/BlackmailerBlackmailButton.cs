using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using TownOfSushi.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modifiers.Impostor;
using TownOfSushi.Options.Roles.Impostor;
using TownOfSushi.Roles.Impostor;
using UnityEngine;

namespace TownOfSushi.Buttons.Impostor;

public sealed class BlackmailerBlackmailButton : TownOfSushiRoleButton<BlackmailerRole, PlayerControl>, IAftermathablePlayerButton
{
    public override string Name => "Blackmail";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<BlackmailerOptions>.Instance.BlackmailCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<BlackmailerOptions>.Instance.MaxBlackmails;
    public override LoadableAsset<Sprite> Sprite => TosImpAssets.BlackmailSprite;

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
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, player => (!player.HasModifier<BlackmailedModifier>()) && !player.HasModifier<BlackmailSparedModifier>());
    }
}
