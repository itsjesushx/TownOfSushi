using System.Collections;
using Reactor.Utilities;
using TownOfSushi.Buttons;
using TownOfSushi.Modules;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class TransporterRecallButton : TownOfSushiRoleButton<TransporterRole>
{
    public override string Name => "Recall";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Transporter;
    public override float Cooldown => OptionGroupSingleton<TransporterOptions>.Instance.RecallCooldown + MapCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<TransporterOptions>.Instance.MaxEscapes;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.RecallSprite;

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Role is not { MarkedLocation: null } 
        && OptionGroupSingleton<TransporterOptions>.Instance.TransportSelf;
    }

    public override bool CanUse()
    {
        return base.CanUse() && Role is not { MarkedLocation: null };
    }

    protected override void OnClick()
    {
        if (Role.MarkedLocation != null)
        {
            Coroutines.Start(CoRecall(Role.MarkedLocation.Value));
        }
        // TOSAudio.PlaySound(TOSAudio.TransporterRecallSound);
    }

    private static IEnumerator CoRecall(Vector2 location)
    {
        yield return HudManager.Instance.CoFadeFullScreen(Color.clear, TownOfSushiColors.Transporter);
        PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(location);

        if (ModCompatibility.IsSubmerged())
        {
            ModCompatibility.ChangeFloor(PlayerControl.LocalPlayer.GetTruePosition().y > -7);
            ModCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
        }

        TransporterRole.RpcRecall(PlayerControl.LocalPlayer);
        yield return HudManager.Instance.CoFadeFullScreen(TownOfSushiColors.Transporter, Color.clear);
    }
}