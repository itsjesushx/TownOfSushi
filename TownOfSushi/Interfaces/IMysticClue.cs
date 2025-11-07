namespace TownOfUs.Extensions;

public interface IMysticClue
{
    MysticClueType MysticHintType { get; }
}

public enum MysticClueType
{
    Default,
    Perception,
    Insight,
    Death,
    Hunter,
    Fearmonger,
    Protective,
    Trickster,
    Relentless
}