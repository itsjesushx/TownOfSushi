using Reactor.Utilities;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class HunterStalkButton : TownOfSushiRoleButton<HunterRole, PlayerControl>
{
    public override string Name => "Stalk";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Hunter;
    public override float Cooldown => OptionGroupSingleton<HunterOptions>.Instance.HunterStalkCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<HunterOptions>.Instance.HunterStalkDuration;
    public override int MaxUses => (int)OptionGroupSingleton<HunterOptions>.Instance.StalkUses;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.StalkButtonSprite;
    public int ExtraUses { get; set; }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Stalk: Target is null");
            return;
        }

        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>If {Target.Data.PlayerName} uses an ability, you will be able to kill them at any time in the round.</b>",
            Color.white, new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Hunter.LoadAsset());
        notif1.AdjustNotification();

        Target.RpcAddModifier<HunterStalkedModifier>(PlayerControl.LocalPlayer);
        OverrideName("Stalking");
    }

    public override void OnEffectEnd()
    {
        OverrideName("Stalk");
    }

    public override PlayerControl? GetTarget()
    {
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, x => !x.IsLover());
        }
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }
}