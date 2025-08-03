using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Neutral;
using UnityEngine;

namespace TownOfSushi.Buttons.Neutral;

public sealed class GuardianAngelProtectButton : TownOfSushiRoleButton<GuardianAngelTOSRole>
{
    public override string Name => "Protect";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.GuardianAngel;
    public override float Cooldown => OptionGroupSingleton<GuardianAngelOptions>.Instance.ProtectCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<GuardianAngelOptions>.Instance.ProtectDuration;
    public override int MaxUses => (int)OptionGroupSingleton<GuardianAngelOptions>.Instance.MaxProtects;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.ProtectSprite;

    protected override void OnClick()
    {
        Role.Target?.RpcAddModifier<GuardianAngelProtectModifier>(PlayerControl.LocalPlayer);
    }
}