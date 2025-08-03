using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;
using UnityEngine;

namespace TownOfSushi.Buttons.Crewmate;

public sealed class MediumMediateButton : TownOfSushiRoleButton<MediumRole>
{
    public override string Name => "Mediate";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Medium;
    public override float Cooldown => OptionGroupSingleton<MediumOptions>.Instance.MediateCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TosCrewAssets.MediateSprite;

    protected override void OnClick()
    {
        var deadPlayers = PlayerControl.AllPlayerControls.ToArray()
            .Where(plr => plr.Data.IsDead && !plr.Data.Disconnected &&
            UnityEngine.Object.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == plr.PlayerId)
            && !plr.HasModifier<MediatedModifier>()).ToList();

        if (deadPlayers.Count == 0)
        {
            return;
        }

        var targets = OptionGroupSingleton<MediumOptions>.Instance.WhoIsRevealed switch
        {
            MediateRevealedTargets.NewestDead => [deadPlayers[0]],
            MediateRevealedTargets.AllDead => deadPlayers,
            MediateRevealedTargets.OldestDead => [deadPlayers[^1]],
            MediateRevealedTargets.RandomDead => deadPlayers.Randomize(),
            _ => [],
        };

        foreach (var plr in targets)
        {
            MediumRole.RpcMediate(PlayerControl.LocalPlayer, plr);
        }
    }
}
