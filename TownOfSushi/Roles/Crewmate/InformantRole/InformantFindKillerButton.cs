using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class InformantMediateButton : TownOfSushiRoleButton<InformantRole>
{
    public override string Name => "Find Killer";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Informant;
    public override float Cooldown => OptionGroupSingleton<InformantOptions>.Instance.InformantCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<InformantOptions>.Instance.FindKillerDuration;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.MediateSprite;
    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && role is InformantRole snitch && snitch.CanUseButton;
    }
    protected override void OnClick()
    {
        InformantRole.RpcFindKillers(PlayerControl.LocalPlayer);
    }
    public override void OnEffectEnd()
    {
        Role!.ClearArrows();
    }
}
