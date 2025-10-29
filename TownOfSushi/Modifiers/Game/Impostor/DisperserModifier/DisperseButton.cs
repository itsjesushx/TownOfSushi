using MiraAPI.Hud;
using Reactor.Networking.Rpc;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Impostor;

public sealed class DisperseButton : TownOfSushiButton
{
    public override string Name => "Disperse";
    public override BaseKeybind Keybind => Keybinds.ModifierAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => 0.001f + MapCooldown;
    public override int MaxUses => 1;
    public override ButtonLocation Location => ButtonLocation.BottomLeft;
    public override LoadableAsset<Sprite> Sprite => TOSAssets.DisperseSprite;

    public override bool Enabled(RoleBehaviour? role)
    {
        return PlayerControl.LocalPlayer != null &&
               PlayerControl.LocalPlayer.HasModifier<DisperserModifier>() &&
               !PlayerControl.LocalPlayer.Data.IsDead;
    }

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);

        Button!.usesRemainingSprite.sprite = TOSAssets.AbilityCounterVentSprite.LoadAsset();
    }

    protected override void OnClick()
    {
        var coords = DisperserModifier.GenerateDisperseCoordinates();

        Rpc<DisperseRpc>.Instance.Send(PlayerControl.LocalPlayer, coords);
    }
}