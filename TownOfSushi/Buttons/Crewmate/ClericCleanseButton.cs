using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Crewmate;

public sealed class ClericCleanseButton : TownOfSushiRoleButton<ClericRole, PlayerControl>
{
    public override string Name => "Cleanse";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Cleric;
    public override float Cooldown => OptionGroupSingleton<ClericOptions>.Instance.CleanseCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TosCrewAssets.CleanseSprite;

    public override PlayerControl? GetTarget() => PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);

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
