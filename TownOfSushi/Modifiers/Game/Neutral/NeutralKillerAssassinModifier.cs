using MiraAPI.GameOptions;
using TownOfSushi.Options;
using TownOfSushi.Roles;

namespace TownOfSushi.Modifiers.Game.Neutral;

public sealed class NeutralKillerAssassinModifier : AssassinModifier
{
    public override string ModifierName => "Assassin (Neutral)";
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<AssassinOptions>.Instance.NumberOfNeutralAssassins;
    public override int GetAssignmentChance() => (int)OptionGroupSingleton<AssassinOptions>.Instance.NeutAssassinChance;

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return role is ITownOfSushiRole { RoleAlignment: RoleAlignment.NeutralKilling };
    }
}
