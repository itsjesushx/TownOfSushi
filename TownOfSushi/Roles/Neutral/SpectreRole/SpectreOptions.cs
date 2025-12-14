using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Neutral;

public sealed class SpectreOptions : AbstractOptionGroup<SpectreRole>
{
    public override string GroupName => "Spectre";

    [ModdedNumberOption("Tasks Left Before Clickable", 1, 15)]
    public float NumTasksLeftBeforeClickable { get; set; } = 3f;

    [ModdedEnumOption("Spectre Win", typeof(SpectreWinOptions), ["Ends Game", "Spooks", "Nothing"])]
    public SpectreWinOptions SpectreWin { get; set; } = SpectreWinOptions.Nothing;
}

public enum SpectreWinOptions
{
    EndsGame,
    Spooks,
    Nothing
}