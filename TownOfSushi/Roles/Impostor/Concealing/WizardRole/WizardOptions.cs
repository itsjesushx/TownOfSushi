using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Impostor;

public sealed class WizardOptions : AbstractOptionGroup<WizardRole>
{
    public override string GroupName => "Wizard";

    [ModdedNumberOption("Spell Cooldown", 1f, 30f, suffixType: MiraNumberSuffixes.Seconds)]
    public float SpellCooldown { get; set; } = 20f;

    [ModdedNumberOption("Spell Duration", 1f, 10f, 1f, suffixType: MiraNumberSuffixes.Seconds)]
    public float SpellDuration { get; set; } = 2f;

    [ModdedToggleOption("Wizard Can Normally Kill With Teammate")]
    public bool WizardKill { get; set; } = true;
}