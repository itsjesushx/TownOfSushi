using TownOfSushi.Options;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class CrewmateAssassinModifier : AssassinModifier
{
    public override string ModifierName => "Assassin";

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<AssassinOptions>.Instance.NumberOfCrewmateAssassins;
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<AssassinOptions>.Instance.CrewAssassinChance;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return role.IsCrewmate() && !role.Player.HasModifier<SwapperModifier>();
    }
}