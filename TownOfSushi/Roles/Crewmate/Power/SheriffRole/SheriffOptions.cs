using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class SheriffOptions : AbstractOptionGroup<SheriffRole>
{
    public override string GroupName => "Sheriff";

    [ModdedNumberOption("Kill Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float KillCooldown { get; set; } = 25f;

    [ModdedToggleOption("Sheriff Can Self Report")]
    public bool SheriffBodyReport { get; set; } = false;

    [ModdedToggleOption("Allow Shooting In First Round")]
    public bool FirstRoundUse { get; set; } = false;

    [ModdedToggleOption("Sheriff Can Shoot Neutral Evil Roles")]
    public bool ShootNeutralEvil { get; set; } = true;

    [ModdedEnumOption("Sheriff Misfire Kills", typeof(MisfireOptions), ["Self", "Target (Loses Ability)", "Self & Target", "No One"])]
    public MisfireOptions MisfireType { get; set; } = MisfireOptions.Sheriff;
}
public enum MisfireOptions
{
    Sheriff,
    Target,
    Both,
    Nobody
}