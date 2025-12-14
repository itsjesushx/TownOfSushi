using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class HaunterOptions : AbstractOptionGroup<HaunterRole>
{
    public override string GroupName => "Haunter";

    [ModdedNumberOption("Tasks Left Before Clickable", 0f, 5)]
    public float NumTasksLeftBeforeClickable { get; set; } = 3f;

    [ModdedNumberOption("Tasks Left Before Alerted", 0f, 15)]
    public float NumTasksLeftBeforeAlerted { get; set; } = 1f;

    [ModdedToggleOption("Reveal Neutral Roles")]
    public bool RevealNeutralRoles { get; set; } = true;

    [ModdedEnumOption("Can Be Clicked By", typeof(HaunterRoleClickableType),
        ["Everyone", "Non-Crew", "Impostors Only"])]
    public HaunterRoleClickableType HaunterCanBeClickedBy { get; set; } = HaunterRoleClickableType.NonCrew;
    [ModdedNumberOption("Arrow Update Interval", 0f, 15f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float UpdateInterval { get; set; } = 3f;
}

public enum HaunterRoleClickableType
{
    Everyone,
    NonCrew,
    ImpsOnly
}