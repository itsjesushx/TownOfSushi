using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Neutral;
public sealed class ExecutionerOptions : AbstractOptionGroup<ExecutionerRole>
{
    public override string GroupName => "Executioner";

    [ModdedEnumOption("On Target Death, Executioner Becomes", typeof(BecomeOptions))]
    public BecomeOptions OnTargetDeath { get; set; } = BecomeOptions.Jester;

    [ModdedToggleOption("Executioner Can Button")]
    public bool CanButton { get; set; } = true;

    [ModdedEnumOption("Executioner Win", typeof(ExeWinOptions), ["Ends Game", "Leaves & Torments", "Nothing"])]
    public ExeWinOptions ExeWin { get; set; } = ExeWinOptions.Torments;
}

public enum ExeWinOptions
{
    EndsGame,
    Torments,
    Nothing
}