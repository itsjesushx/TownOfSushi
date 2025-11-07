using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class SpyOptions : AbstractOptionGroup<SpyRole>
{
    public override string GroupName => "Spy";

    [ModdedToggleOption("Spy Can Enter Vents")]
    public bool SpyCanHideInVents { get; set; } = false;
    public ModdedToggleOption SpyChangeVents { get; } = new("Spy Can Move In Vents", false)
    {
        Visible = () => OptionGroupSingleton<SpyOptions>.Instance.SpyCanHideInVents
    };

    [ModdedToggleOption("Spy Has Impostor Vision")]
    public bool SpyHasImpVision { get; set; } = true;

    [ModdedToggleOption("Spy Can Be Shot By A Vigilante")]
    public bool VigilanteKillsSpy { get; set; } = false;

    [ModdedToggleOption("Impostors Can Kill Anyone If There Is A Spy")]
    public bool SpyImpsKillEachOther { get; set; } = false;
}