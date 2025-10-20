using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class PoliticianCampaignButton : TownOfSushiRoleButton<PoliticianRole, PlayerControl>
{
    public override string Name => "Campaign";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override float Cooldown => OptionGroupSingleton<PoliticianOptions>.Instance.CampaignCooldown + MapCooldown;
    public override Color TextOutlineColor => TownOfSushiColors.Politician;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.CampaignButtonSprite;

    public override bool CanUse()
    {
        return base.CanUse() && Role is { CanCampaign: true };
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<PoliticianCampaignedModifier>());
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        Target?.RpcAddModifier<PoliticianCampaignedModifier>(PlayerControl.LocalPlayer);
    }
}