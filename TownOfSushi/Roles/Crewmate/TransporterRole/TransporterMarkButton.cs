using MiraAPI.Hud;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class TransporterMarkButton : TownOfSushiRoleButton<TransporterRole>
{
    public override string Name => "Mark Location";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Transporter;
    public override float Cooldown => 0.001f;
    public override float InitialCooldown => 0.001f;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.MarkSprite;

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Role is { MarkedLocation: null }
        && OptionGroupSingleton<TransporterOptions>.Instance.TransportSelf;
    }

    public override bool CanUse()
    {
        return base.CanUse() && Role is { MarkedLocation: null };
    }

    protected override void OnClick()
    {
        TransporterRole.RpcMarkLocation(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.transform.position);

        // TOSAudio.PlaySound(TOSAudio.TransporterMarkSound);
        CustomButtonSingleton<TransporterRecallButton>.Instance.SetActive(true, Role);
        CustomButtonSingleton<TransporterRecallButton>.Instance.ResetCooldownAndOrEffect();
        SetActive(false, Role);
    }
}