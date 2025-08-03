using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Neutral;
using UnityEngine;

namespace TownOfSushi.Buttons.Neutral;

public sealed class RomanticProtectButton : TownOfSushiRoleButton<RomanticRole>
{
    public override string Name => "Protect";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Romantic;
    public override float Cooldown => OptionGroupSingleton<RomanticOptions>.Instance.ProtectCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<RomanticOptions>.Instance.ProtectDuration;
    public override int MaxUses => (int)OptionGroupSingleton<RomanticOptions>.Instance.MaxProtects;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.RomanticProtect;
    public override bool Enabled(RoleBehaviour? role) => role is RomanticRole { HasBeloved: true };

    protected override void OnClick() => Role.Target?.RpcAddModifier<RomanticProtectModifier>(PlayerControl.LocalPlayer);
}