using MiraAPI.Hud;
using Reactor.Utilities;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class ClericCleanseButton : TownOfSushiRoleButton<ClericRole, PlayerControl>
{
    public override string Name => "Cleanse";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Cleric;
    public override float Cooldown => OptionGroupSingleton<ClericOptions>.Instance.CleanseCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.CleanseSprite;

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

        if (Target.HasModifier<ClericCleanseModifier>())
        {
            Target.RpcRemoveModifier<ClericCleanseModifier>();
        }

        Target.RpcAddModifier<ClericCleanseModifier>(PlayerControl.LocalPlayer);

        if (ClericCleanseModifier.FindNegativeEffects(Target).Count > 0)
        {
            Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Cleric));
        }

        CustomButtonSingleton<ClericBarrierButton>.Instance.ResetCooldownAndOrEffect();
    }
}