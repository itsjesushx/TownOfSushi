using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modifiers.Game.Alliance;
using TownOfSushi.Options.Modifiers.Alliance;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Crewmate;

public sealed class AltruistReviveButton : TownOfSushiRoleButton<AltruistRole>
{
    public override string Name => "Revive";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Altruist;
    public override float Cooldown => 0.001f + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<AltruistOptions>.Instance.ReviveDuration;
    public override int MaxUses => (int)OptionGroupSingleton<AltruistOptions>.Instance.MaxRevives;
    public override LoadableAsset<Sprite> Sprite => TosCrewAssets.ReviveSprite;

    public bool RevivedInRound { get; set; }
    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);

        Button!.usesRemainingSprite.sprite = TosAssets.AbilityCounterBodySprite.LoadAsset();
    }
    public override bool CanUse()
    {
        if (RevivedInRound) return false;

        var bodiesInRange = Helpers.GetNearestDeadBodies(
            PlayerControl.LocalPlayer.transform.position,
            OptionGroupSingleton<AltruistOptions>.Instance.ReviveRange * ShipStatus.Instance.MaxLightRadius,
            Helpers.CreateFilter(Constants.NotShipMask));

        return base.CanUse() && bodiesInRange.Count > 0;
    }

    protected override void OnClick()
    {
        var bodiesInRange = Helpers.GetNearestDeadBodies(
            PlayerControl.LocalPlayer.transform.position,
            OptionGroupSingleton<AltruistOptions>.Instance.ReviveRange * ShipStatus.Instance.MaxLightRadius,
            Helpers.CreateFilter(Constants.NotShipMask));

        var playersToRevive = bodiesInRange.Select(x => x.ParentId).ToList();

        foreach (var playerId in playersToRevive)
        {
            var player = MiscUtils.PlayerById(playerId);
            if (player != null)
            {
                if (player.IsLover() && OptionGroupSingleton<LoversOptions>.Instance.BothLoversDie)
                {
                    var other = player.GetModifier<LoverModifier>()!.GetOtherLover;
                    if (!playersToRevive.Contains(other()!.PlayerId) && other()!.Data.IsDead)
                    {
                        AltruistRole.RpcRevive(PlayerControl.LocalPlayer, other()!);
                    }
                }
                AltruistRole.RpcRevive(PlayerControl.LocalPlayer, player);
            }
        }
    }

    public override void OnEffectEnd()
    {
        RevivedInRound = true;
    }
}
