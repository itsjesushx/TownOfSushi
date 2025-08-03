using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;
using UnityEngine;

namespace TownOfSushi.Buttons.Crewmate;

public sealed class VeteranAlertButton : TownOfSushiRoleButton<VeteranRole>
{
    public override string Name => "Alert";
    public override string Keybind => Keybinds.SecondaryAction;
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