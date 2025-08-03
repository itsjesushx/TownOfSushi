using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Crewmate;

public sealed class DetectiveExamineButton : TownOfSushiRoleButton<DetectiveRole, PlayerControl>
{
    public override string Name => "Examine";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Detective;
    public override float Cooldown => OptionGroupSingleton<DetectiveOptions>.Instance.ExamineCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.ExamineSprite;

    public override bool CanUse()
    {
        return base.CanUse() && Role.InvestigatingScene;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        Role.ExaminePlayer(Target);
    }
}