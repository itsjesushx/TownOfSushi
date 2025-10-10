using MiraAPI.Hud;
using Reactor.Utilities;
using TownOfSushi.Buttons;
using TownOfSushi.Modifiers;
using TownOfSushi.Modules;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class PlumberFlushButton : TownOfSushiRoleButton<PlumberRole, Vent>
{
    private static readonly ContactFilter2D Filter = Helpers.CreateFilter(Constants.Usables);
    public override string Name => "Flush";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Plumber;
    public override float Cooldown => OptionGroupSingleton<PlumberOptions>.Instance.FlushCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.FlushSprite;

    public override Vent? GetTarget()
    {
        var vent = PlayerControl.LocalPlayer.GetNearestObjectOfType<Vent>(Distance / 4, Filter);
        if (vent == null)
        {
            vent = PlayerControl.LocalPlayer.GetNearestObjectOfType<Vent>(Distance / 3, Filter);
        }

        if (vent == null)
        {
            vent = PlayerControl.LocalPlayer.GetNearestObjectOfType<Vent>(Distance / 2, Filter);
        }

        if (vent == null)
        {
            vent = PlayerControl.LocalPlayer.GetNearestObjectOfType<Vent>(Distance, Filter);
        }

        if (ModCompatibility.IsSubmerged() && vent != null && (vent.Id == 0 || vent.Id == 14))
        {
            vent = null;
        }

        if (vent != null && PlayerControl.LocalPlayer.CanUseVent(vent))
        {
            return vent;
        }

        return null;
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error($"{Name}: Target is null");
            return;
        }

        PlumberRole.RpcPlumberFlush(PlayerControl.LocalPlayer);

        var block = CustomButtonSingleton<PlumberBlockButton>.Instance;

        block?.SetTimer(block.Cooldown);
    }

    public override bool CanUse()
    {
        var newTarget = GetTarget();
        if (newTarget != Target)
        {
            Target?.SetOutline(false, false);
        }

        Target = IsTargetValid(newTarget) ? newTarget : null;
        SetOutline(true);

        return Timer <= 0 && Target != null
                          && !PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>()
                          && !PlayerControl.LocalPlayer.HasModifier<DisabledModifier>();
    }
}