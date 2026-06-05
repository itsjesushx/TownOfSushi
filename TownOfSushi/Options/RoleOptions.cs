using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;

namespace TownOfSushi.Options;

public sealed class RoleOptions : AbstractOptionGroup
{

    public override string GroupName => "Role";
    public override uint GroupPriority => 2;
    
    [ModdedNumberOption("Min Neutral Benign", 0f, 3f, 1f)]
    public float MinNeutralBenign { get; set; } = 0f;

    [ModdedNumberOption("Max Neutral Benign", 0f, 3f, 1f)]
    public float MaxNeutralBenign { get; set; } = 0f;

    [ModdedNumberOption("Min Neutral Evil", 0f, 3f, 1f)]
    public float MinNeutralEvil { get; set; } = 0f;

    [ModdedNumberOption("Max Neutral Evil", 0f, 3f, 1f)]
    public float MaxNeutralEvil { get; set; } = 0f;
    [ModdedNumberOption("Min Neutral Killer", 0f, 5f, 1f)]
    public float MinNeutralKiller { get; set; } = 0f;
    [ModdedNumberOption("Max Neutral Killer", 0f, 5f, 1f)]
    public float MaxNeutralKiller { get; set; } = 0f;
}