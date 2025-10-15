using MiraAPI.Events;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Options;
using TownOfSushi.Modifiers;
using UnityEngine;
using MiraAPI.LocalSettings;

namespace TownOfSushi.Roles.Neutral;

public sealed class RomanticProtectModifier(PlayerControl romantic) : BaseShieldModifier
{
    public override float Duration => OptionGroupSingleton<RomanticOptions>.Instance.ProtectDuration;
    public override string ModifierName => "Protected";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSRoleIcons.Romantic;
    public override string ShieldDescription => "You are protected by your Romantic!\nYou cannot be killed.";
    public override bool AutoStart => true;
    public PlayerControl Romantic => romantic;
    public override bool HideOnUi
    {
        get
        {
            var showProtect = OptionGroupSingleton<RomanticOptions>.Instance.ShowProtect;
            var showProtectEveryone = showProtect == RomanticProtectOptions.Everyone;
            var showProtectSelf = PlayerControl.LocalPlayer.PlayerId == Player.PlayerId &&
                showProtect is RomanticProtectOptions.SelfAndRomantic;
            return !LocalSettingsTabSingleton<TownOfSushiLocalSettings>.Instance.ShowShieldHudToggle.Value && (!showProtectEveryone || !showProtectSelf);
        }
    }

    public override void OnActivate()
    {
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.RomanticProtect, Romantic, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);

        var showProtect = OptionGroupSingleton<RomanticOptions>.Instance.ShowProtect;
        var rom = CustomRoleUtils.GetActiveRolesOfType<RomanticRole>().FirstOrDefault(x => x.Target == Player);

        var showProtectEveryone = showProtect == RomanticProtectOptions.Everyone;
        var showProtectSelf = PlayerControl.LocalPlayer.PlayerId == Player.PlayerId &&
            showProtect is RomanticProtectOptions.SelfAndRomantic;
        var showProtectRom = PlayerControl.LocalPlayer.PlayerId == rom?.Player.PlayerId &&
                 showProtect is RomanticProtectOptions.Romantic or RomanticProtectOptions.SelfAndRomantic;

        if (showProtectEveryone || showProtectSelf || showProtectRom || (PlayerControl.LocalPlayer.HasDied() && OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow))
        {
            var roleEffectAnimation = UnityEngine.Object.Instantiate(DestroyableSingleton<RoleManager>.Instance.protectLoopAnim,
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
        for (int i = Player.currentRoleAnimations.Count - 1; i >= 0; i--)
        {
            if (Player.currentRoleAnimations[i] != null && Player.currentRoleAnimations[i].effectType == RoleEffectAnimation.EffectType.ProtectLoop)
            {
                UnityEngine.Object.Destroy(Player.currentRoleAnimations[i].gameObject);
                Player.currentRoleAnimations.RemoveAt(i);
            }
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        for (int i = Player.currentRoleAnimations.Count - 1; i >= 0; i--)
        {
            if (Player.currentRoleAnimations[i] != null && Player.currentRoleAnimations[i].effectType == RoleEffectAnimation.EffectType.ProtectLoop)
            {
                UnityEngine.Object.Destroy(Player.currentRoleAnimations[i].gameObject);
                Player.currentRoleAnimations.RemoveAt(i);
            }
        }
    }
}