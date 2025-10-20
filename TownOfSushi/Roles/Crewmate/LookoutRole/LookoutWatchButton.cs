using Reactor.Utilities;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class WatchButton : TownOfSushiRoleButton<LookoutRole, PlayerControl>
{
    public override string Name => "Watch";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Lookout;
    public override float Cooldown => OptionGroupSingleton<LookoutOptions>.Instance.WatchCooldown + MapCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<LookoutOptions>.Instance.MaxWatches;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.WatchSprite;
    public int ExtraUses { get; set; }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target!.HasModifier<LookoutWatchedModifier>(x => x.Lookout.AmOwner);
    }

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

        var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Lookout,
            $"<b>You will know what roles interact with {Target.Data.PlayerName} if they are not dead by next meeting.</b>"),
            Color.white, new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Lookout.LoadAsset());
        notif1.AdjustNotification();
    }
}