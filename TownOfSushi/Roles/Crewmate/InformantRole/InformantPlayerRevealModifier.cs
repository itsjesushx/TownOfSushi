using TownOfSushi.Modifiers;

namespace TownOfSushi.Roles.Crewmate;

public sealed class InformantPlayerRevealModifier(RoleBehaviour role)
    : RevealModifier((int)ChangeRoleResult.Nothing, true, role)
{
    public override string ModifierName => "Revealed Informant";
}