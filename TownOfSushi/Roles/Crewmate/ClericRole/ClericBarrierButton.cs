using MiraAPI.Hud;
using Reactor.Utilities;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class ClericBarrierButton : TownOfSushiRoleButton<ClericRole, PlayerControl>
{
    public override string Name => "Barrier";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Cleric;
    public override float Cooldown => OptionGroupSingleton<ClericOptions>.Instance.BarrierCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<ClericOptions>.Instance.BarrierDuration;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.BarrierSprite;

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target!.HasModifier<ClericBarrierModifier>();
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error($"{Name}: Target is null");
            return;
        }

        Target?.RpcAddModifier<ClericBarrierModifier>(PlayerControl.LocalPlayer);

        CustomButtonSingleton<ClericCleanseButton>.Instance.ResetCooldownAndOrEffect();
    }
}