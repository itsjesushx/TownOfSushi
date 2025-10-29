using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class ScavengerEatButton : TownOfSushiRoleButton<ScavengerRole, DeadBody>
{
    public override string Name => "Eat";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Scavenger;
    public override float Cooldown => OptionGroupSingleton<ScavengerOptions>.Instance.EatCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<ScavengerOptions>.Instance.EatDuration + 0.001f;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.EatSprite;

    public override DeadBody? GetTarget() => PlayerControl.LocalPlayer.GetNearestDeadBody(Distance);
    public DeadBody? EatingBody { get; set; }
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
        EatingBody = Target;
    }
    public override void OnEffectEnd()
    {
        if (EatingBody == Target && EatingBody != null)
        {
            ScavengerRole.RpcEatBody(PlayerControl.LocalPlayer, EatingBody.ParentId);
            // I know this doesn't fit, I'll add a proper sound later
            TOSAudio.PlaySound(TOSAudio.JanitorCleanSound);
        }
        EatingBody = null;
    }
}