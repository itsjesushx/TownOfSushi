﻿using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modules;
using TownOfSushi.Options;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfSushi.Modifiers.Neutral;

public sealed class GuardianAngelProtectModifier(PlayerControl guardianAngel) : BaseShieldModifier
{
    public override float Duration => OptionGroupSingleton<GuardianAngelOptions>.Instance.ProtectDuration;
    public override string ModifierName => "Protected";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSRoleIcons.GuardianAngel;
    public override string ShieldDescription => "You are protected by your Guardian Angel!\nYou cannot be killed.";
    public override bool AutoStart => true;
    public PlayerControl Guardian => guardianAngel;

    public override bool HideOnUi
    {
        get
        {
            var showProtect = OptionGroupSingleton<GuardianAngelOptions>.Instance.ShowProtect;
            return !TownOfSushiPlugin.ShowShieldHud.Value || !OptionGroupSingleton<GuardianAngelOptions>.Instance.GATargetKnows || showProtect is ProtectOptions.GA;
        }
    }

    public override void OnActivate()
    {
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.GuardianAngelProtect, Guardian, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);

        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        var showProtect = OptionGroupSingleton<GuardianAngelOptions>.Instance.ShowProtect;
        var ga = CustomRoleUtils.GetActiveRolesOfType<GuardianAngelTOSRole>().FirstOrDefault(x => x.Target == Player);

        var showProtectEveryone = showProtect == ProtectOptions.Everyone;
        var showProtectSelf = PlayerControl.LocalPlayer.PlayerId == Player.PlayerId &&
                              showProtect is ProtectOptions.SelfAndGA && OptionGroupSingleton<GuardianAngelOptions>.Instance.GATargetKnows;
        var showProtectGA = PlayerControl.LocalPlayer.PlayerId == ga?.Player.PlayerId &&
                            showProtect is ProtectOptions.GA or ProtectOptions.SelfAndGA;

        var body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x =>
            x.ParentId == PlayerControl.LocalPlayer.PlayerId && !TutorialManager.InstanceExists);
        var fakePlayer = FakePlayer.FakePlayers.FirstOrDefault(x =>
            x.PlayerId == PlayerControl.LocalPlayer.PlayerId && !TutorialManager.InstanceExists);
        
        if (showProtectEveryone || showProtectSelf || showProtectGA || (PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !body && !fakePlayer?.body))
        {
            var roleEffectAnimation = Object.Instantiate(DestroyableSingleton<RoleManager>.Instance.protectLoopAnim,
                Player.gameObject.transform);
            roleEffectAnimation
                .SetMaterialColor(7); // This is white, if it's not, make sure it is set to white from the int
            roleEffectAnimation.SetMaskLayerBasedOnWhoShouldSee(true);
            roleEffectAnimation.Play(Player, new Action(OnDeactivate), Player.cosmetics.FlipX,
                RoleEffectAnimation.SoundType.Local, Duration);
        }
    }

    public override void OnDeactivate()
    {
        for (var i = Player.currentRoleAnimations.Count - 1; i >= 0; i--)
        {
            if (Player.currentRoleAnimations[i] != null && Player.currentRoleAnimations[i].effectType ==
                RoleEffectAnimation.EffectType.ProtectLoop)
            {
                Object.Destroy(Player.currentRoleAnimations[i].gameObject);
                Player.currentRoleAnimations.RemoveAt(i);
            }
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        for (var i = Player.currentRoleAnimations.Count - 1; i >= 0; i--)
        {
            if (Player.currentRoleAnimations[i] != null && Player.currentRoleAnimations[i].effectType ==
                RoleEffectAnimation.EffectType.ProtectLoop)
            {
                Object.Destroy(Player.currentRoleAnimations[i].gameObject);
                Player.currentRoleAnimations.RemoveAt(i);
            }
        }
    }
}