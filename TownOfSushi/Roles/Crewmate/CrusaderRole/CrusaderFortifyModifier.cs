using MiraAPI.Events;
using PowerTools;
using Reactor.Utilities.Extensions;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modifiers;
using TownOfSushi.Modules.Anims;
using TownOfSushi.Options;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class CrusaderFortifiedModifier(PlayerControl Crusader) : BaseShieldModifier
{
    public override string ModifierName => "Fortified";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSRoleIcons.Crusader;
    public override string ShieldDescription => "You are fortified by a Crusader!\nNo one can interact with you.";
    public GameObject? CrusaderFort { get; set; }
    public bool ShowFort { get; set; }

    public override bool HideOnUi
    {
        get
        {
            var showFort = OptionGroupSingleton<CrusaderOptions>.Instance.ShowFortified;
            return !MiraAPI.LocalSettings.LocalSettingsTabSingleton<TownOfSushiLocalSettings>.Instance.ShowShieldHudToggle.Value || showFort is FortifyOptions.Crusader;
        }
    }

    public override bool VisibleSymbol
    {
        get
        {
            var show = OptionGroupSingleton<CrusaderOptions>.Instance.ShowFortified;
            var showShieldedEveryone = show == FortifyOptions.Everyone;
            var showShieldedSelf = PlayerControl.LocalPlayer.PlayerId == Player.PlayerId &&
                                   show is FortifyOptions.Self or FortifyOptions.SelfAndCrusader;
            return showShieldedSelf || showShieldedEveryone;
        }
    }

    public PlayerControl Crusader { get; } = Crusader;

    public override void OnActivate()
    {
        base.OnActivate();
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.CrusaderFortify, Crusader, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
        
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        var show = OptionGroupSingleton<CrusaderOptions>.Instance.ShowFortified;

        var showShieldedEveryone = show == FortifyOptions.Everyone;
        var showShieldedSelf = PlayerControl.LocalPlayer.PlayerId == Player.PlayerId &&
                               show is FortifyOptions.Self or FortifyOptions.SelfAndCrusader;
        var showShieldedCrusader = PlayerControl.LocalPlayer.PlayerId == Crusader.PlayerId &&
                                 show is FortifyOptions.Crusader or FortifyOptions.SelfAndCrusader;

        var body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x =>
            x.ParentId == PlayerControl.LocalPlayer.PlayerId && !TutorialManager.InstanceExists);
        
        
        ShowFort = showShieldedEveryone || showShieldedSelf || showShieldedCrusader || (PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !body);
        
        CrusaderFort = AnimStore.SpawnAnimBody(Player, TOSAssets.WardenFort.LoadAsset(), false, -1.1f, -0.35f, 1.5f)!;
        CrusaderFort.GetComponent<SpriteAnim>().SetSpeed(0.75f);
        CrusaderFort.GetComponentsInChildren<SpriteAnim>().FirstOrDefault()?.SetSpeed(0.75f);
    }

    public override void OnDeactivate()
    {
        if (CrusaderFort?.gameObject != null)
        {
            CrusaderFort.gameObject.Destroy();
        }
    }
    
    public override void Update()
    {
        if (!MeetingHud.Instance && CrusaderFort?.gameObject != null)
        {
            CrusaderFort?.SetActive(!Player.IsConcealed() && IsVisible && ShowFort);
        }
    }
}