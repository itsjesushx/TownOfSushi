using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Options.Roles.Impostor;
using TownOfSushi.Roles.Impostor;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Impostor;

public sealed class MorphlingSampleButton : TownOfSushiRoleButton<MorphlingRole, PlayerControl>, IAftermathablePlayerButton
{
    public override string Name => "Sample";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<MorphlingOptions>.Instance.MorphlingCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<MorphlingOptions>.Instance.MaxSamples;
    public override LoadableAsset<Sprite> Sprite => TosImpAssets.SampleSprite;

    public override bool Enabled(RoleBehaviour? role) => base.Enabled(role) && Role is { Sampled: null };

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        Role.Sampled = Target;
        
        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>{TownOfSushiColors.ImpSoft.ToTextColor()}You have sampled {Target.Data.PlayerName}. The sample will be reset after this round.</b></color>", Color.white, spr: TosRoleIcons.Morphling.LoadAsset());
        notif1.Text.SetOutlineThickness(0.35f);
            notif1.transform.localPosition = new Vector3(0f, 1f, -20f);

        CustomButtonSingleton<MorphlingMorphButton>.Instance.SetActive(true, Role);
        SetActive(false, Role);
    }

    public override PlayerControl? GetTarget() => PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
}
