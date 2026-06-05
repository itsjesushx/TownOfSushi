using TownOfSushi.Options;

namespace TownOfSushi.Modifiers.Game.Neutral;

public sealed class NeutralKillerDoubleShotModifier : DoubleShotModifier, IWikiDiscoverable
{
    public override string ModifierName => "Double Shot";
    public override bool ShowInFreeplay => true;
    public bool IsHiddenFromList => true;
    // YES this is scuffed, a better solution will be used at a later time
    public uint FakeTypeId => ModifierManager.GetModifierTypeId(ModifierManager.Modifiers.FirstOrDefault(x => x.ModifierName == "Double Shot")!.GetType()) ?? throw new InvalidOperationException("Modifier is not registered.");

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<NeutralRolesOptions>.Instance.DoubleShotChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<NeutralRolesOptions>.Instance.DoubleShotAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        if (
            role is ITOSRole { RoleAlignment: RoleAlignment.NeutralKilling }
            && role.Player.GetModifierComponent().HasModifier<NeutralKillerAssassinModifier>(true)
            && !role.Player.GetModifierComponent().HasModifier<TOSGameModifier>(true)
        )
        {
            return true;
        }

        return false;
    }
}