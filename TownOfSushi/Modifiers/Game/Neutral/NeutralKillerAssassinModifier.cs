using TownOfSushi.Options;

namespace TownOfSushi.Modifiers.Game.Neutral;

public sealed class NeutralKillerAssassinModifier : AssassinModifier
{
    public override string ModifierName => "Assassin";

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<AssassinOptions>.Instance.NumberOfNeutralAssassins;
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<AssassinOptions>.Instance.NeutAssassinChance;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return role is ITownOfSushiRole { RoleAlignment: RoleAlignment.NeutralKilling } || role is ThiefRole && OptionGroupSingleton<ThiefOptions>.Instance.GuessToSteal;
    }
}