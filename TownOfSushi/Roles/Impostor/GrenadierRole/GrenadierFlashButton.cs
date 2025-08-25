﻿using BepInEx.Unity.IL2CPP.Utils.Collections;
using Reactor.Utilities;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class GrenadierFlashButton : TownOfSushiRoleButton<GrenadierRole>, IAftermathableButton
{
    public override string Name => "Flash";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<GrenadierOptions>.Instance.GrenadeCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<GrenadierOptions>.Instance.GrenadeDuration;
    public override int MaxUses => (int)OptionGroupSingleton<GrenadierOptions>.Instance.MaxFlashes;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.FlashSprite;

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
        var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.ImpSoft,
            $"<b>All players around you are now flashbanged!</b>"), Color.white,
            spr: TOSRoleIcons.Grenadier.LoadAsset());
        
        notif1.Text.SetOutlineThickness(0.35f);
        notif1.transform.localPosition = new Vector3(0f, 1f, -150f);

        Coroutines.Start(
            Effects.Shake(HudManager.Instance.PlayerCam.transform, 0.2f, 0.1f, true, true).WrapToManaged());

        SoundManager.Instance.PlaySound(TOSAudio.GrenadeSound.LoadAsset(), false);
    }
}