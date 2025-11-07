using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class SeerOptions : AbstractOptionGroup<SeerRole>
{
    public override string GroupName => "Seer";

    [ModdedNumberOption("Seer Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float SeerCooldown { get; set; } = 25f;
    
    [ModdedToggleOption("Show Factions Found On Checking")]
    public bool ShowFaction { get; set; } = false;
}