using MiraAPI.Events;
using MiraAPI.Hud;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modifiers;
using TownOfSushi.Utilities.Appearances;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfSushi.Roles.Impostor;

public sealed class SwoopModifier : ConcealedModifier, IVisualAppearance
{
    public override string ModifierName => "Swooped";
    public override float Duration => OptionGroupSingleton<SwooperOptions>.Instance.SwoopDuration;
    public override bool HideOnUi => true;
    public override bool AutoStart => true;
    public bool VisualPriority => true;

    public VisualAppearance GetVisualAppearance()
    {
        var playerColor = PlayerControl.LocalPlayer.IsImpostor()
            ? new Color(0f, 0f, 0f, 0.1f)
            : Color.clear;

        return new VisualAppearance(Player.GetDefaultModifiedAppearance(), TownOfSushiAppearances.Swooper)
        {
            HatId = string.Empty,
            SkinId = string.Empty,
            VisorId = string.Empty,
            PlayerName = string.Empty,
            PetId = string.Empty,
            RendererColor = playerColor,
            NameColor = Color.clear,
            ColorBlindTextColor = Color.clear
        };
    }

    public override void OnDeath(DeathReason reason)
    {
        Player.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        Player.RemoveModifier(this);
    }

    public override void OnActivate()
    {
        if (Player.AmOwner)
        {
            TOSAudio.PlaySound(TOSAudio.SwooperActivateSound);
        }

        Player.RawSetAppearance(this);
        Player.cosmetics.ToggleNameVisible(false);

        var button = CustomButtonSingleton<SwooperSwoopButton>.Instance;
        button.OverrideSprite(TOSImpAssets.UnswoopSprite.LoadAsset());
        button.OverrideName("Unswoop");

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.SwooperSwoop, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        var mushroom = Object.FindObjectOfType<MushroomMixupSabotageSystem>();
        if (mushroom && mushroom.IsActive)
        {
            Player.RawSetAppearance(this);
            Player.cosmetics.ToggleNameVisible(false);
        }
    }

    public override void OnDeactivate()
    {
        Player.ResetAppearance();
        Player.cosmetics.ToggleNameVisible(true);

        var button = CustomButtonSingleton<SwooperSwoopButton>.Instance;
        button.OverrideSprite(TOSImpAssets.SwoopSprite.LoadAsset());
        button.OverrideName("Swoop");

        if (Player.AmOwner)
        {
            TOSAudio.PlaySound(TOSAudio.SwooperDeactivateSound);
        }

        var mushroom = Object.FindObjectOfType<MushroomMixupSabotageSystem>();
        if (mushroom && mushroom.IsActive)
        {
            MushroomMixUp(mushroom, Player);
        }

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.SwooperUnswoop, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public static void MushroomMixUp(MushroomMixupSabotageSystem instance, PlayerControl player)
    {
        if (player != null && !player.Data.IsDead && instance.currentMixups.ContainsKey(player.PlayerId))
        {
            var condensedOutfit = instance.currentMixups[player.PlayerId];
            var playerOutfit = instance.ConvertToPlayerOutfit(condensedOutfit);
            playerOutfit.NamePlateId = player.Data.DefaultOutfit.NamePlateId;

            player.MixUpOutfit(playerOutfit);
        }
    }
}