using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Impostor;

public sealed class TelepathOptions : AbstractOptionGroup<TelepathModifier>
{
    public override string GroupName => "Telepath";
    public override Color GroupColor => Palette.ImpostorRoleHeaderRed;
    public override uint GroupPriority => 42;

    [ModdedNumberOption("Telepath Amount", 0, 5)]
    public float TelepathAmount { get; set; } = 0;

    public ModdedNumberOption TelepathChance { get; } =
        new("Telepath Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<TelepathOptions>.Instance.TelepathAmount > 0
        };


    [ModdedToggleOption("Know Where Teammate Kills")]
    public bool KnowKillLocation { get; set; } = true;

    [ModdedToggleOption("Know When Teammate Dies")]
    public bool KnowDeath { get; set; } = true;

    public ModdedToggleOption KnowDeathLocation { get; } = new("Know Where Teammate Dies", true)
    {
        Visible = () => OptionGroupSingleton<TelepathOptions>.Instance.KnowDeath
    };

    public ModdedNumberOption TelepathArrowDuration { get; } = new("Dead Body Arrow Duration", 2.5f, 0f, 5f, 0.5f,
        MiraNumberSuffixes.Seconds, "0.00")
    {
        Visible = () => OptionGroupSingleton<TelepathOptions>.Instance.KnowKillLocation ||
                        (OptionGroupSingleton<TelepathOptions>.Instance.KnowDeath &&
                         OptionGroupSingleton<TelepathOptions>.Instance.KnowDeathLocation)
    };

    [ModdedToggleOption("Know When Teammate Guesses Successfully")]
    public bool KnowCorrectGuess { get; set; } = true;

    [ModdedToggleOption("Know When Teammate Fails To Guess")]
    public bool KnowFailedGuess { get; set; } = true;
}