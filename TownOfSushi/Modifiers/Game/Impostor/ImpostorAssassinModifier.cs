using MiraAPI.GameOptions;
using TownOfSushi.Options;

namespace TownOfSushi.Modifiers.Game.Impostor;

public sealed class ImpostorAssassinModifier : AssassinModifier
{
    public override string ModifierName => "Assassin (Impostor)";
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<AssassinOptions>.Instance.NumberOfImpostorAssassins;
    public override int GetAssignmentChance() => (int)OptionGroupSingleton<AssassinOptions>.Instance.ImpAssassinChance;

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return role.TeamType == RoleTeamTypes.Impostor;
    }
}
