using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class InspectorExamineButton : TownOfSushiRoleButton<InspectorRole, PlayerControl>
{
    public override string Name => "Examine";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Inspector;
    public override float Cooldown => OptionGroupSingleton<InspectorOptions>.Instance.ExamineCooldown + MapCooldown;
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