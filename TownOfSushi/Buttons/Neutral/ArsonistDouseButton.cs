using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Options.Modifiers.Alliance;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Neutral;

public sealed class ArsonistDouseButton : TownOfSushiRoleButton<ArsonistRole, PlayerControl>
{
    public override string Name => "Douse";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Arsonist;
    public override float Cooldown => OptionGroupSingleton<ArsonistOptions>.Instance.DouseCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<ArsonistOptions>.Instance.DouseDuration;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.DouseButtonSprite;
    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Role is { DousedEveryone: false };
    }
    protected override void OnClick()
    {
        // don't do anything, add the douse after the effect ends
    }
    public override void OnEffectEnd()
    {
        Target!.RpcAddModifier<ArsonistDousedModifier>(PlayerControl.LocalPlayer.PlayerId);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target!.HasModifier<ArsonistDousedModifier>();
    }

    public override PlayerControl? GetTarget()
    {
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, x => !x.IsLover() && !x.HasModifier<ArsonistDousedModifier>());
        }
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<ArsonistDousedModifier>());
    }
}