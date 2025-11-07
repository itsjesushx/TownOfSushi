using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class GuardianAngelProtectButton : TownOfSushiRoleButton<GuardianAngelTOSRole>
{
    public override string Name => "Protect";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.GuardianAngel;
    public override float Cooldown => OptionGroupSingleton<GuardianAngelOptions>.Instance.ProtectCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<GuardianAngelOptions>.Instance.ProtectDuration;
    public override int MaxUses => (int)OptionGroupSingleton<GuardianAngelOptions>.Instance.MaxProtects;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.ProtectSprite;

    protected override void OnClick()
    {
        if (Role.Target == null || Role.Target.HasDied())
        {
            return;
        }
        
        Role.Target.RpcAddModifier<GuardianAngelProtectModifier>(PlayerControl.LocalPlayer);
    }
}