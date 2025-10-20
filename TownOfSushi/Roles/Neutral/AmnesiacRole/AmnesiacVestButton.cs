using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;
public sealed class AmnesiacVestButton : TownOfSushiRoleButton<AmnesiacRole>
{
    public override string Name => "Safeguard";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Amnesiac;
    public override float Cooldown => OptionGroupSingleton<AmnesiacOptions>.Instance.VestCooldown;
    public override float EffectDuration => OptionGroupSingleton<AmnesiacOptions>.Instance.VestDuration;
    public override int MaxUses => (int)OptionGroupSingleton<AmnesiacOptions>.Instance.MaxVests;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.VestSprite;

    protected override void OnClick()
    {
        PlayerControl.LocalPlayer.RpcAddModifier<AmnesiacVestModifier>();
    }
}