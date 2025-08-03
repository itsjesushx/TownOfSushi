using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modules.Anims;
using Reactor.Utilities.Extensions;
using TownOfSushi.Options.Roles.Crewmate;
using UnityEngine;
using TownOfSushi.Utilities;
using TownOfSushi.Options;
using TownOfSushi.Events.TosEvents;
using MiraAPI.Events;

namespace TownOfSushi.Modifiers.Crewmate;

public sealed class MedicShieldModifier(PlayerControl medic) : BaseShieldModifier
{
    public override string ModifierName => "Medic Shield";
    public override LoadableAsset<Sprite>? ModifierIcon => TosRoleIcons.Medic;
    public override string ShieldDescription => "You are shielded by a Medic!\nYou may not die to other players";
    public PlayerControl Medic { get; } = medic;
    public GameObject? MedicShield { get; set; }
    public override bool HideOnUi
    {
        get
        {
            var showShielded = OptionGroupSingleton<MedicOptions>.Instance.ShowShielded;
            var showShieldedEveryone = showShielded == MedicOption.Everyone;
            var showShieldedSelf = PlayerControl.LocalPlayer.PlayerId == Player.PlayerId &&
                (showShielded is MedicOption.Shielded or MedicOption.ShieldedAndMedic);
            return !TownOfSushiPlugin.ShowShieldHud.Value && (!showShieldedSelf || !showShieldedEveryone);
        }
    }
    public override bool VisibleSymbol
    {
        get
        {
            var showShielded = OptionGroupSingleton<MedicOptions>.Instance.ShowShielded;
            var showShieldedEveryone = showShielded == MedicOption.Everyone;
            var showShieldedSelf = PlayerControl.LocalPlayer.PlayerId == Player.PlayerId &&
                (showShielded is MedicOption.Shielded or MedicOption.ShieldedAndMedic);
            return showShieldedSelf || showShieldedEveryone;
        }
    }

    public override void OnActivate()
    {
        var TosAbilityEvent = new TosAbilityEvent(AbilityType.MedicShield, Medic, Player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);

        var showShielded = OptionGroupSingleton<MedicOptions>.Instance.ShowShielded;

        var showShieldedEveryone = showShielded == MedicOption.Everyone;
        var showShieldedSelf = PlayerControl.LocalPlayer.PlayerId == Player.PlayerId &&
            (showShielded is MedicOption.Shielded or MedicOption.ShieldedAndMedic);
        var showShieldedMedic = PlayerControl.LocalPlayer.PlayerId == Medic.PlayerId &&
                 (showShielded is MedicOption.Medic or MedicOption.ShieldedAndMedic);

        if (showShieldedEveryone || showShieldedSelf || showShieldedMedic || (PlayerControl.LocalPlayer.HasDied() && OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow))
        {
            MedicShield = AnimStore.SpawnAnimBody(Player, TosAssets.MedicShield.LoadAsset(), false, -1.1f, -0.1f)!;
        }
    }
    public override void OnDeactivate()
    {
        if (MedicShield?.gameObject != null)
        {
            MedicShield.gameObject.Destroy();
        }
    }
    public override void Update()
    {
        if (!MeetingHud.Instance && MedicShield?.gameObject != null)
        {
            MedicShield?.SetActive(!Player.IsConcealed() && IsVisible);
        }
    }
}
