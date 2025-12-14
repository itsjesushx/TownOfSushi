using BepInEx.Unity.IL2CPP.Utils.Collections;
using Reactor.Utilities;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class GrenadierFlashButton : TownOfSushiRoleButton<GrenadierRole>, IAftermathableButton
{
    public override string Name => "Flash";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<GrenadierOptions>.Instance.GrenadeCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<GrenadierOptions>.Instance.GrenadeDuration;
    public override int MaxUses => (int)OptionGroupSingleton<GrenadierOptions>.Instance.MaxFlashes;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.FlashSprite;

    public void AftermathHandler()
    {
        ClickHandler();
    }

    protected override void OnClick()
    {
        var flashRadius = OptionGroupSingleton<GrenadierOptions>.Instance.FlashRadius;
        var flashedPlayers =
            Helpers.GetClosestPlayers(PlayerControl.LocalPlayer, flashRadius * ShipStatus.Instance.MaxLightRadius);

        foreach (var player in flashedPlayers)
        {
            player.RpcAddModifier<GrenadierFlashModifier>(PlayerControl.LocalPlayer);
        }

        PlayerControl.LocalPlayer.RpcAddModifier<GrenadierFlashModifier>(PlayerControl.LocalPlayer);
        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>{TownOfSushiColors.ImpSoft.ToTextColor()}All players around you are now flashbanged!</color></b>",
            Color.white, new Vector3(0f, 1f, -150f),
            spr: TOSRoleIcons.Grenadier.LoadAsset());
        notif1.AdjustNotification();

        Coroutines.Start(
            Effects.Shake(HudManager.Instance.PlayerCam.transform, 0.2f, 0.1f, true, true).WrapToManaged());

        SoundManager.Instance.PlaySound(TOSAudio.GrenadeSound.LoadAsset(), false);
    }
}