using Reactor.Utilities;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class WatchButton : TownOfSushiRoleButton<LookoutRole, PlayerControl>
{
    public override string Name => "Watch";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Lookout;
    public override float Cooldown => OptionGroupSingleton<LookoutOptions>.Instance.WatchCooldown;
    public override LoadableAsset<Sprite> Sprite => TownOfSushiAssets.WatchSprite;
    public int ExtraUses { get; set; }
    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Watch: Target is null");
            return;
        }

        Target.RpcAddModifier<LookoutWatchedModifier>(PlayerControl.LocalPlayer);
        Role.ObservedPlayer = Target;
    }

}