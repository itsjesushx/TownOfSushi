using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class JanitorCleanButton : TownOfSushiRoleButton<JanitorRole, DeadBody>, IAftermathableBodyButton,
    IDiseaseableButton
{
    public override string Name => "Clean";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<JanitorOptions>.Instance.CleanCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<JanitorOptions>.Instance.CleanDelay + 0.001f;
    public override int MaxUses => (int)OptionGroupSingleton<JanitorOptions>.Instance.MaxClean;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.CleanButtonSprite;

    public DeadBody? CleaningBody { get; set; }

    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    public override DeadBody? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetNearestDeadBody(Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        CleaningBody = Target;
        OverrideName("Cleaning");
    }

    public override void OnEffectEnd()
    {
        OverrideName("Clean");
        if (CleaningBody == Target && CleaningBody != null)
        {
            JanitorRole.RpcCleanBody(PlayerControl.LocalPlayer, CleaningBody.ParentId);
            TOSAudio.PlaySound(TOSAudio.JanitorCleanSound);
        }

        CleaningBody = null;
        if (OptionGroupSingleton<JanitorOptions>.Instance.ResetCooldowns)
        {
            PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.LocalPlayer.GetKillCooldown());
        }
    }
}