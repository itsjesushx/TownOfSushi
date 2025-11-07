using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class MysticOptions : AbstractOptionGroup<MysticRole>
{
    public override string GroupName => "Mystic";

    [ModdedNumberOption("Dead Body Arrow Duration", 0f, 3f, 0.05f, MiraNumberSuffixes.Seconds, "0.00")]
    public float MysticArrowDuration { get; set; } = 1f;
    
    [ModdedNumberOption("Observe Cooldown", 1f, 30f, 1f, MiraNumberSuffixes.Seconds)]
    public float ObserveCooldown { get; set; } = 20f;
}