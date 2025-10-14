using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class BodyGuardOptions : AbstractOptionGroup<BodyGuardRole>
{
    public override string GroupName => "BodyGuard";

    [ModdedEnumOption("Show Protected Player", typeof(BGProtectOptions), ["Protected", "BodyGuard", "Protected + BodyGuard", "Everyone"])]
    public BGProtectOptions ShowGuarded { get; set; } = BGProtectOptions.SelfAndBodyGuard;
}

public enum BGProtectOptions
{
    Self,
    BodyGuard,
    SelfAndBodyGuard,
    Everyone
}