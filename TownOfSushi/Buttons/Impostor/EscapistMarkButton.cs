using MiraAPI.Hud;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Roles.Impostor;
using UnityEngine;

namespace TownOfSushi.Buttons.Impostor;

public sealed class EscapistMarkButton : TownOfSushiRoleButton<EscapistRole>, IAftermathableButton
{
    public override string Name => "Mark Location";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => 0;
    public override LoadableAsset<Sprite> Sprite => TosImpAssets.MarkSprite;

    public override bool Enabled(RoleBehaviour? role) => base.Enabled(role) && Role is { MarkedLocation: null };

    public override bool CanUse()
    {
        return base.CanUse() && Role is { MarkedLocation: null };
    }

    protected override void OnClick()
    {
        EscapistRole.RpcMarkLocation(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.transform.position);

        // TosAudio.PlaySound(TosAudio.EscapistMarkSound);
        CustomButtonSingleton<EscapistRecallButton>.Instance.SetActive(true, Role);
        SetActive(false, Role);
    }
}
