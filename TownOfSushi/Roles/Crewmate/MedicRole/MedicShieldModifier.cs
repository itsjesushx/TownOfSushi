using MiraAPI.Events;
using MiraAPI.LocalSettings;
using Reactor.Utilities.Extensions;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modifiers;
using TownOfSushi.Modules.Anims;
using TownOfSushi.Options;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class MedicShieldModifier(PlayerControl medic) : BaseShieldModifier
{
    public override string ModifierName => $"Medic Shield";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSRoleIcons.Medic;
    public override string ShieldDescription => $"You are shielded by a Medic !\nYou may not die to other players";
    public PlayerControl Medic { get; } = medic;
    public GameObject? MedicShield { get; set; }
    public bool ShowShield { get; set; }

    public override bool HideOnUi
    {
        get
        {
            var showShielded = OptionGroupSingleton<MedicOptions>.Instance.ShowShielded;
            return !LocalSettingsTabSingleton<TownOfSushiLocalSettings>.Instance.ShowShieldHudToggle.Value || (showShielded is MedicOption.Medic or MedicOption.Nobody);
        }
    }

    public override bool VisibleSymbol
    {
        get
        {
            var showShielded = OptionGroupSingleton<MedicOptions>.Instance.ShowShielded;
            var showShieldedEveryone = showShielded == MedicOption.Everyone;
            var showShieldedSelf = PlayerControl.LocalPlayer.PlayerId == Player.PlayerId &&
                                   showShielded is MedicOption.Shielded or MedicOption.ShieldedAndMedic;
            return showShieldedSelf || showShieldedEveryone;
        }
    }

    public override void OnActivate()
    {
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.MedicShield, Medic, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);

        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        var showShielded = OptionGroupSingleton<MedicOptions>.Instance.ShowShielded;

        var showShieldedEveryone = showShielded == MedicOption.Everyone;
        var showShieldedSelf = PlayerControl.LocalPlayer.PlayerId == Player.PlayerId &&
                               showShielded is MedicOption.Shielded or MedicOption.ShieldedAndMedic;
        var showShieldedMedic = PlayerControl.LocalPlayer.PlayerId == Medic.PlayerId &&
                                showShielded is MedicOption.Medic or MedicOption.ShieldedAndMedic;

        var body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x =>
            x.ParentId == PlayerControl.LocalPlayer.PlayerId && !TutorialManager.InstanceExists);
        
        
        ShowShield = showShieldedEveryone || showShieldedSelf || showShieldedMedic || (PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !body);
        
        MedicShield = AnimStore.SpawnAnimBody(Player, TOSAssets.MedicShield.LoadAsset(), false, -1.1f, -0.1f, 1.5f)!;
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
            MedicShield?.SetActive(!Player.IsConcealed() && IsVisible && ShowShield);
        }
    }
}