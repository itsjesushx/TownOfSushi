using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class TrackerOptions : AbstractOptionGroup<TrackerTOSRole>
{
    public override string GroupName => "Tracker";

    [ModdedNumberOption("Track Cooldown", 1f, 30f, 1f, MiraNumberSuffixes.Seconds)]
    public float TrackCooldown { get; set; } = 20f;

    [ModdedNumberOption("Max Number Of Tracks", 1f, 15f, 1f, MiraNumberSuffixes.None, "0")]
    public float MaxTracks { get; set; } = 5f;

    [ModdedNumberOption("Arrow Update Interval", 0f, 15f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float UpdateInterval { get; set; } = 5f;

    [ModdedToggleOption("Tracker Arrows Make Sound On Death")]
    public bool SoundOnDeactivate { get; set; } = true;

    [ModdedToggleOption("Tracker Arrows Reset After Each Round")]
    public bool ResetOnNewRound { get; set; } = true;

    public ModdedToggleOption TaskUses { get; } = new("Get More Uses From Completing Tasks", false)
    {
        Visible = () => !OptionGroupSingleton<TrackerOptions>.Instance.ResetOnNewRound
    };
}