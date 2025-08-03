using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;

using TownOfSushi.Roles.Crewmate;

namespace TownOfSushi.Options.Roles.Crewmate;

public sealed class MysticOptions : AbstractOptionGroup<MysticRole>
{
    public override string GroupName => TOSLocale.Get(TOSNames.Mystic, "Mystic");

    [ModdedNumberOption("Dead Body Arrow Duration", 0f, 1f, 0.05f, MiraNumberSuffixes.Seconds, "0.00")]
    public float MysticArrowDuration { get; set; } = 0.1f;
}