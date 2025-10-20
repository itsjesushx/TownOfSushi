using MiraAPI.Hud;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class EscapistMarkButton : TownOfSushiRoleButton<EscapistRole>, IAftermathableButton
{
    public override string Name => "Mark Location";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => 0.001f;
    public override float InitialCooldown => 0.001f;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.MarkSprite;

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Role is { MarkedLocation: null };
    }

    public override bool CanUse()
    {
        return base.CanUse() && Role is { MarkedLocation: null };
    }

    protected override void OnClick()
    {
        EscapistRole.RpcMarkLocation(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.transform.position);

        // TOSAudio.PlaySound(TOSAudio.EscapistMarkSound);
        CustomButtonSingleton<EscapistRecallButton>.Instance.SetActive(true, Role);
        CustomButtonSingleton<EscapistRecallButton>.Instance.ResetCooldownAndOrEffect();
        SetActive(false, Role);
    }
}