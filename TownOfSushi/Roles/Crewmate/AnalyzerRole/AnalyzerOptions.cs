using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class AnalyzerOptions : AbstractOptionGroup<AnalyzerRole>
{
    public override string GroupName => "Analyzer";

    [ModdedNumberOption("Analyzer Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float AnalyzerCooldown { get; set; } = 25f;
    
    [ModdedEnumOption("Analyzer Extra Info", typeof(AnalyzerCheckType), [ "Faction", "Alignment", "None" ])]
    public AnalyzerCheckType ExtraInfoType { get; set; } = AnalyzerCheckType.None;
    public enum AnalyzerCheckType
    {
        Faction,
        Alignment,
        None
    }
}