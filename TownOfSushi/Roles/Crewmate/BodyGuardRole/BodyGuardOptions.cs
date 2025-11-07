using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class BodyguardOptions : AbstractOptionGroup<BodyguardRole>
{
    public override string GroupName => "Bodyguard";

    [ModdedEnumOption("Show Protected Player", typeof(BGProtectOptions), ["Protected", "Bodyguard", "Protected + Bodyguard", "Everyone"])]
    public BGProtectOptions ShowGuarded { get; set; } = BGProtectOptions.SelfAndBodyguard;
}

public enum BGProtectOptions
{
    Self,
    Bodyguard,
    SelfAndBodyguard,
    Everyone
}