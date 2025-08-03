using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfSushi.Roles.Impostor;

namespace TownOfSushi.Options.Roles.Impostor;

public sealed class WitchOptions : AbstractOptionGroup<WitchRole>
{
    public override string GroupName => "Witch";
    [ModdedNumberOption("Spell Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float SpellCooldown { get; set; } = 40f;
    [ModdedNumberOption("Spell Duration", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float SpellDuration { get; set; } = 2.5f;

    [ModdedToggleOption("Voting The Witch Saves Cursed Players")]
    public bool WitchVotingSafesCursed { get; set; } = false;

    [ModdedToggleOption("Witch Can Spell Their Imp Partner")]
    public bool WitchCanSpellImpostors { get; set; } = false;

    [ModdedToggleOption("Cursed Players Only Die If Witch Is Alive")]
    public bool OnlyKillIfAlive { get; set; } = true;
}