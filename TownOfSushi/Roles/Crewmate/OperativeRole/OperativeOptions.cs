using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class OperativeOptions : AbstractOptionGroup<OperativeRole>
{
    public override string GroupName => "Operative";

    // THESE BREAK THE CAMERA MINIGAME!!
    /*
            [ModdedToggleOption("Move While Using Cameras")]
            public bool MoveWithCams { get; set; } = false;

            [ModdedToggleOption("Move While Using Fungle Binoculars")]
            public bool MoveOnFungle { get; set; } = false;
         */
    [ModdedToggleOption("Move While Using Mira Doorlog")]
    public bool MoveOnMira { get; set; } = true;

    [ModdedNumberOption("Starting Charge", 0f, 30f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float StartingCharge { get; set; } = 20f;

    [ModdedNumberOption("Battery Charged Each Round", 0f, 30f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float RoundCharge { get; set; } = 10f;

    [ModdedNumberOption("Battery Charged Per Task", 0f, 30f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float TaskCharge { get; set; } = 7.5f;

    [ModdedNumberOption("Security Display Cooldown", 0f, 30f, 5f, MiraNumberSuffixes.Seconds)]
    public float DisplayCooldown { get; set; } = 15f;

    [ModdedNumberOption("Max Security Display Duration", 0f, 30f, 5f, MiraNumberSuffixes.Seconds, zeroInfinity: true)]
    public float DisplayDuration { get; set; } = 15f;
    
    [ModdedToggleOption("Move While Using Vitals")]
    public bool MoveWithMenu { get; set; } = true;

    [ModdedNumberOption("Starting Charge", 0f, 30f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float VitalsStartingCharge { get; set; } = 20f;

    [ModdedNumberOption("Battery Charged Each Round", 0f, 30f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float VitalsRoundCharge { get; set; } = 15f;

    [ModdedNumberOption("Battery Charged Per Task", 0f, 30f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float VitalsTaskCharge { get; set; } = 10f;

    [ModdedNumberOption("Vitals Display Cooldown", 0f, 30f, 5f, MiraNumberSuffixes.Seconds)]
    public float VitalsDisplayCooldown { get; set; } = 15f;

    [ModdedNumberOption("Max Vitals Display Duration", 0f, 30f, 5f, MiraNumberSuffixes.Seconds, zeroInfinity: true)]
    public float VitalsDisplayDuration { get; set; } = 15f;
}