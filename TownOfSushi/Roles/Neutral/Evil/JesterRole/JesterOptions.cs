using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Neutral;

public sealed class JesterOptions : AbstractOptionGroup<JesterRole>
{
    public override string GroupName => "Jester";

    [ModdedToggleOption("Can Use Button")]
    public bool CanButton { get; set; } = true;

    [ModdedToggleOption("Can Hide In Vents")]
    public bool CanVent { get; set; } = true;

    [ModdedToggleOption("Has Impostor Vision")]
    public bool ImpostorVision { get; set; } = true;
    [ModdedToggleOption("Jester Can Kill Once")]
    public bool JesterCanKill { get; set; } = false;

    [ModdedEnumOption("After Win Type", typeof(JestWinOptions), ["Ends Game", "Haunts", "Nothing"])]
    public JestWinOptions JestWin { get; set; } = JestWinOptions.EndsGame;
}

public enum JestWinOptions
{
    EndsGame,
    Haunts,
    Nothing
}