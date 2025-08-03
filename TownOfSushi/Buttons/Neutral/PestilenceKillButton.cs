using MiraAPI.GameOptions;
using MiraAPI.Networking;
using TownOfSushi.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Neutral;
using UnityEngine;

namespace TownOfSushi.Buttons.Neutral;

public sealed class PestilenceKillButton : TownOfSushiRoleButton<PestilenceRole, PlayerControl>, IDiseaseableButton, IKillButton
{
    public override string Name => "Kill";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Pestilence;
    public override float Cooldown => OptionGroupSingleton<PlaguebearerOptions>.Instance.PestKillCooldown;
    public override LoadableAsset<Sprite> Sprite => TosNeutAssets.PestKillSprite;
    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    public override PlayerControl? GetTarget() => PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Pestilence Shoot: Target is null");
            return;
        }

        PlayerControl.LocalPlayer.RpcCustomMurder(Target);
    }
}
