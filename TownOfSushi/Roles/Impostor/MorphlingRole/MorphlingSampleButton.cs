using MiraAPI.Hud;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class MorphlingSampleButton : TownOfSushiRoleButton<MorphlingRole, PlayerControl>, IAftermathablePlayerButton
{
    public override string Name => "Sample";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => 0.001f;
    public override float InitialCooldown => 0.001f;
    public override int MaxUses => (int)OptionGroupSingleton<MorphlingOptions>.Instance.MaxSamples;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.SampleSprite;

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Role is { Sampled: null };
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        Role.Sampled = Target;

        var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.ImpSoft,
            $"<b>You have sampled {Target.Data.PlayerName}. The sample will be reset after this round.</b>"),
            Color.white, spr: TOSRoleIcons.Morphling.LoadAsset());
        
        notif1.AdjustNotification();

        CustomButtonSingleton<MorphlingMorphButton>.Instance.SetActive(true, Role);
        CustomButtonSingleton<MorphlingMorphButton>.Instance.ResetCooldownAndOrEffect();
        SetActive(false, Role);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }
}