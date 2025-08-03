using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using TownOfSushi.Roles.Crewmate;

namespace TownOfSushi.Options.Roles.Crewmate;

public sealed class SwapperOptions : AbstractOptionGroup<SwapperRole>
{
    public override string GroupName => "Swapper";

    [ModdedToggleOption("Swapper Can Call Button")]
    public bool CanButton { get; set; } = true;
}
