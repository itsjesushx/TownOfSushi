namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class CrewmateDoubleShotModifier : DoubleShotModifier, IWikiDiscoverable
{
    public override string ModifierName => "Double Shot";
    public override bool ShowInFreeplay => true;
    public bool IsHiddenFromList => true;
    // YES this is scuffed, a better solution will be used at a later time
    public uint FakeTypeId => ModifierManager.GetModifierTypeId(ModifierManager.Modifiers.FirstOrDefault(x => x.ModifierName == "Double Shot")!.GetType()) ?? throw new InvalidOperationException("Modifier is not registered.");

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<CrewmateAssassinOptions>.Instance.DoubleShotChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<CrewmateAssassinOptions>.Instance.DoubleShotAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        if (
            role.IsCrewmate() && !role.Player.HasModifier<SwapperModifier>()
            && role.Player.GetModifierComponent().HasModifier<CrewmateAssassinModifier>(true)
            && !role.Player.GetModifierComponent().HasModifier<TOSGameModifier>(true)
        )
        {
            return true;
        }

        return false;
    }
}