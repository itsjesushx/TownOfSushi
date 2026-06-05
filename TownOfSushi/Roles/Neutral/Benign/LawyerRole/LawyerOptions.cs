using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Neutral;

public sealed class LawyerOptions : AbstractOptionGroup<LawyerRole>
{
    public override string GroupName => "Lawyer";

    [ModdedEnumOption("On Client Death, Lawyer Becomes", typeof(BecomeOptions))]
    public BecomeOptions OnTargetDeath { get; set; } = BecomeOptions.Amnesiac;

    [ModdedToggleOption("Lawyer Knows Client's Role")]
    public bool LawyerKnowsTargetRole { get; set; } = true;
    
    [ModdedToggleOption("Lawyer Can Button")]
    public bool CanButton { get; set; } = true;

    [ModdedToggleOption("Client Knows Lawyer Exists")]
    public bool LawyerTargetKnows { get; set; } = true;
}