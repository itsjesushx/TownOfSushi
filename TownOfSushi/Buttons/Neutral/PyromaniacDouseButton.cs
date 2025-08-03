using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Options.Modifiers.Alliance;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Neutral;

public sealed class PyromaniacDouseButton : TownOfSushiRoleButton<PyromaniacRole, PlayerControl>
{
    public override string Name => "Douse";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Pyromaniac;
    public override float Cooldown => OptionGroupSingleton<PyromaniacOptions>.Instance.DouseCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.DouseButtonSprite;

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Pyromaniac Attack: Target is null");
            return;
        }

        Target.RpcAddModifier<PyromaniacDousedModifier>(PlayerControl.LocalPlayer.PlayerId);

        CustomButtonSingleton<PyromaniacIgniteButton>.Instance.SetTimer(CustomButtonSingleton<PyromaniacIgniteButton>
            .Instance.Cooldown);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target!.HasModifier<PyromaniacDousedModifier>();
    }

    public override PlayerControl? GetTarget()
    {
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, x => !x.IsLover() && !x.HasModifier<PyromaniacDousedModifier>());
        }
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<PyromaniacDousedModifier>());
    }
}