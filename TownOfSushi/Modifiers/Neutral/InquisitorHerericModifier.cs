using MiraAPI.Modifiers;
using TownOfSushi.Modules;

namespace TownOfSushi.Modifiers.Neutral;

public sealed class InquisitorHereticModifier : BaseModifier
{
    public override string ModifierName => "Inquisitor Heretic";
    public override bool HideOnUi => true;

    public RoleBehaviour TargetRole { get; set; }

    public override void OnActivate()
    {
        TargetRole = Player.GetRoleWhenAlive();
    }
}