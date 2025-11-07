using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Impostor;
public sealed class ConsigliereOptions : AbstractOptionGroup<ConsigliereRole>
{
    public override string GroupName => "Consigliere";

    [ModdedNumberOption("Number Of Reveal Uses Per Game", 1f, 7f, 1f, MiraNumberSuffixes.None, "0", true)]
    public float MaxConsiglieres { get; set; } = 0f;

    [ModdedNumberOption("Reveal Cooldown", 25f, 60f, 5f, suffixType: MiraNumberSuffixes.Seconds)]
    public float RevealCooldown { get; set; } = 30f;

    [ModdedNumberOption("Reveal Duration", 3f, 15f, 1f, MiraNumberSuffixes.Seconds)]
    public float RevealDuration { get; set; } = 5f;

    [ModdedToggleOption("Consigliere Can Kill With Teammate")]
    public bool ConsigliereKill { get; set; } = true;
    [ModdedToggleOption("Consigliere Teammates Can See Role On Reveal")]
    public bool ConsigliereShowRoleImp { get; set; } = true;
}