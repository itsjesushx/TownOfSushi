using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;

namespace TownOfSushi.Roles.Impostor;

public sealed class AmbassadorOptions : AbstractOptionGroup<AmbassadorRole>
{
    public override string GroupName => "Ambassador";

    [ModdedNumberOption("Max Retrains Available", 1, 3)]
    public float MaxRetrains { get; set; } = 2f;
    
    [ModdedToggleOption("Retrain Requires Confirmation")]
    public bool RetrainConfirmation { get; set; } = true;
    
    [ModdedNumberOption("Kills Needed By Ambassador Or Teammate To Retrain", 0, 4)]
    public float KillsNeeded { get; set; } = 2f;
    
    [ModdedNumberOption("Round In Which Retraining Is Possible", 1, 5)]
    public float RoundWhenAvailable { get; set; } = 2f;
    
    public ModdedNumberOption RoundCooldown { get; } =
        new("Rounds Needed To Retrain Again", 2f, 1f, 5f, 1f, MiraNumberSuffixes.None)
        {
            Visible = () => (int)OptionGroupSingleton<AmbassadorOptions>.Instance.MaxRetrains > 1
        };
}