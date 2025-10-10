using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class VeteranAlertButton : TownOfSushiRoleButton<VeteranRole>
{
    public override string Name => "Alert";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Veteran;
    public override float Cooldown => OptionGroupSingleton<VeteranOptions>.Instance.AlertCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<VeteranOptions>.Instance.AlertDuration;
    public override int MaxUses => (int)OptionGroupSingleton<VeteranOptions>.Instance.MaxNumAlerts;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.AlertSprite;
    public int ExtraUses { get; set; }

    protected override void OnClick()
    {
        PlayerControl.LocalPlayer.RpcAddModifier<VeteranAlertModifier>();
        OverrideName("Alerting");
    }

    public override void OnEffectEnd()
    {
        OverrideName("Alert");
    }
}