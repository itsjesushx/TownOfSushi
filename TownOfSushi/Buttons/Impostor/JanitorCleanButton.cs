using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Options.Roles.Impostor;
using TownOfSushi.Roles.Impostor;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Impostor;

public sealed class JanitorCleanButton : TownOfSushiRoleButton<JanitorRole, DeadBody>, IAftermathableBodyButton, IDiseaseableButton
{
    public override string Name => "Clean";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<JanitorOptions>.Instance.CleanCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<JanitorOptions>.Instance.CleanDelay + 0.001f;
    public override int MaxUses => (int)OptionGroupSingleton<JanitorOptions>.Instance.MaxClean;
    public override LoadableAsset<Sprite> Sprite => TosImpAssets.CleanButtonSprite;

    public override DeadBody? GetTarget() => PlayerControl.LocalPlayer.GetNearestDeadBody(Distance);
    public DeadBody? CleaningBody { get; set; }
    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }
        CleaningBody = Target;
    }
    public override void OnEffectEnd()
    {
        if (CleaningBody == Target && CleaningBody != null)
        {
            JanitorRole.RpcCleanBody(PlayerControl.LocalPlayer, CleaningBody.ParentId);
            TosAudio.PlaySound(TosAudio.JanitorCleanSound);
        }
        CleaningBody = null;
        if (OptionGroupSingleton<JanitorOptions>.Instance.ResetCooldowns)
        {
            PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.LocalPlayer.GetKillCooldown());
        }
    }
}
