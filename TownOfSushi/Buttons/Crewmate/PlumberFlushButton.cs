using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfSushi.Modules;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;
using UnityEngine;

namespace TownOfSushi.Buttons.Crewmate;

public sealed class PlumberFlushButton : TownOfSushiRoleButton<PlumberRole, Vent>
{
    public override string Name => "Flush";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Plumber;
    public override float Cooldown => OptionGroupSingleton<PlumberOptions>.Instance.FlushCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TosCrewAssets.FlushSprite;
    private static readonly ContactFilter2D Filter = Helpers.CreateFilter(Constants.NotShipMask);

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

    public override Vent? GetTarget()
    {
        var vent = PlayerControl.LocalPlayer.GetNearestObjectOfType<Vent>(Distance, Filter);

        if (ModCompatibility.IsSubmerged() && vent != null && (vent.Id == 0 || vent.Id == 14))
        {
            vent = null;
        }

        return vent;
    }
}
