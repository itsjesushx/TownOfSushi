using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class RetributionistReviveButton : TownOfSushiRoleButton<RetributionistRole>
{
    public override string Name => "Revive";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Retributionist;
    public override float Cooldown => 0.001f + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<RetributionistOptions>.Instance.ReviveDuration;
    public override int MaxUses => (int)OptionGroupSingleton<RetributionistOptions>.Instance.MaxRevives;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.ReviveSprite;

    public bool RevivedInRound { get; set; }

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);

        Button!.usesRemainingSprite.sprite = TOSAssets.AbilityCounterBodySprite.LoadAsset();
    }

    public override bool CanUse()
    {
        if (RevivedInRound)
        {
            return false;
        }

        var bodiesInRange = Helpers.GetNearestDeadBodies(
            PlayerControl.LocalPlayer.transform.position,
            OptionGroupSingleton<RetributionistOptions>.Instance.ReviveRange * ShipStatus.Instance.MaxLightRadius,
            Helpers.CreateFilter(Constants.NotShipMask));

        return base.CanUse() && bodiesInRange.Count > 0;
    }

    protected override void OnClick()
    {
        var bodiesInRange = Helpers.GetNearestDeadBodies(
            PlayerControl.LocalPlayer.transform.position,
            OptionGroupSingleton<RetributionistOptions>.Instance.ReviveRange * ShipStatus.Instance.MaxLightRadius,
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
                        RetributionistRole.RpcRevive(PlayerControl.LocalPlayer, other()!);
                    }
                }

                RetributionistRole.RpcRevive(PlayerControl.LocalPlayer, player);
            }
        }
        OverrideName("Reviving");
    }

    public override void OnEffectEnd()
    {
        RevivedInRound = true;
        OverrideName("Revive");
    }
}