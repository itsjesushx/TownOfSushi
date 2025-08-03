using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using PowerTools;
using Reactor.Utilities.Extensions;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Modules.Anims;
using TownOfSushi.Options;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Crewmate;

public sealed class ClericBarrierModifier(PlayerControl cleric) : BaseShieldModifier
{
    public override string ModifierName => "Barrier";
    public override LoadableAsset<Sprite>? ModifierIcon => TosRoleIcons.Cleric;
    public override string ShieldDescription => "You are shielded by a Cleric!\nNo one can interact with you.";
    public override float Duration => OptionGroupSingleton<ClericOptions>.Instance.BarrierCooldown;
    public override bool AutoStart => true;
    public override bool HideOnUi
    {
        get
        {
            var showBarrier = OptionGroupSingleton<ClericOptions>.Instance.ShowBarriered;
            var showBarrierSelf = PlayerControl.LocalPlayer.PlayerId == Player.PlayerId &&
                (showBarrier == BarrierOptions.Self || showBarrier == BarrierOptions.SelfAndCleric);
            return !TownOfSushiPlugin.ShowShieldHud.Value && !showBarrierSelf;
        }
    }
    public override bool VisibleSymbol
    {
        get
        {
            var showBarrier = OptionGroupSingleton<ClericOptions>.Instance.ShowBarriered;
            var showBarrierSelf = PlayerControl.LocalPlayer.PlayerId == Player.PlayerId &&
                (showBarrier == BarrierOptions.Self || showBarrier == BarrierOptions.SelfAndCleric);
            return showBarrierSelf;
        }
    }
    public PlayerControl Cleric { get; } = cleric;
    public GameObject? ClericBarrier { get; set; }


    public override void OnActivate()
    {
        var TosAbilityEvent = new TosAbilityEvent(AbilityType.ClericBarrier, Cleric, Player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);

        var showBarrier = OptionGroupSingleton<ClericOptions>.Instance.ShowBarriered;

        var showBarrierSelf = PlayerControl.LocalPlayer.PlayerId == Player.PlayerId &&
            (showBarrier == BarrierOptions.Self || showBarrier == BarrierOptions.SelfAndCleric);
        var showBarrierCleric = PlayerControl.LocalPlayer.PlayerId == Cleric.PlayerId &&
                 (showBarrier == BarrierOptions.Cleric || showBarrier == BarrierOptions.SelfAndCleric);

        if (showBarrierSelf || showBarrierCleric || (PlayerControl.LocalPlayer.HasDied() && OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow))
        {
            ClericBarrier = AnimStore.SpawnAnimBody(Player, TosAssets.ClericBarrier.LoadAsset(), false, -1.1f, -0.35f)!;
            ClericBarrier.GetComponent<SpriteAnim>().SetSpeed(2f);
        }
    }
    public override void OnDeactivate()
    {
        if (ClericBarrier?.gameObject != null)
        {
            ClericBarrier.gameObject.Destroy();
        }
    }
}
