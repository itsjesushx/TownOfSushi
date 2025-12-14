using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class InformantOptions : AbstractOptionGroup<InformantRole>
{
    public override string GroupName => "Informant";

    [ModdedNumberOption("Reveal Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float InformantCooldown { get; set; } = 25f;
    [ModdedNumberOption("Find Killer Duration", 5f, 60f, 5f, MiraNumberSuffixes.Seconds)]
    public float FindKillerDuration { get; set; } = 30f;

    [ModdedNumberOption("Odds Of Revealing Real Killers", 10f, 50f, 5f, suffixType: MiraNumberSuffixes.Percent)]
    public float RevealAccuracyPercentage { get; set; } = 30f;
    [ModdedToggleOption("Informant Sees Traitor")]
    public bool InformantSeesTraitor { get; set; } = true;

}