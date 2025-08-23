using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class InspectorOptions : AbstractOptionGroup<InspectorRole>
{
    public override string GroupName => "Inspector";

    [ModdedNumberOption("Examine Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float ExamineCooldown { get; set; } = 25f;

    [ModdedToggleOption("Show Inspector Reports")]
    public bool InspectorReportOn { get; set; } = true;

    public ModdedNumberOption InspectorRoleDuration { get; set; } = new("Time Where Inspector Will Have Role", 7.5f, 0f,
        60f, 2.5f, MiraNumberSuffixes.Seconds)
    {
        Visible = () => OptionGroupSingleton<InspectorOptions>.Instance.InspectorReportOn
    };

    public ModdedNumberOption InspectorFactionDuration { get; set; } = new("Time Where Inspector Will Have Faction",
        30f, 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)
    {
        Visible = () => OptionGroupSingleton<InspectorOptions>.Instance.InspectorReportOn
    };
}