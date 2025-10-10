using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class BomberPlantButton : TownOfSushiRoleButton<BomberRole>, IAftermathableButton, IDiseaseableButton
{
    public override string Name => "Place";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => PlayerControl.LocalPlayer.GetKillCooldown() + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<BomberOptions>.Instance.DetonateDelay;
    public override int MaxUses => (int)OptionGroupSingleton<BomberOptions>.Instance.MaxBombs;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.PlaceSprite;

    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    protected override void OnClick()
    {
        OverrideSprite(TOSImpAssets.DetonatingSprite.LoadAsset());
        OverrideName("Detonating");

        PlayerControl.LocalPlayer.killTimer = EffectDuration + 1f;

        BomberRole.RpcPlantBomb(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.transform.position);
    }

    public override void OnEffectEnd()
    {
        OverrideSprite(TOSImpAssets.PlaceSprite.LoadAsset());
        OverrideName("Place");

        PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.LocalPlayer.GetKillCooldown());

        Role.Bomb?.Detonate();
    }
}