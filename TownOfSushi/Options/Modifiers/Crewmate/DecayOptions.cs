using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfSushi.Modifiers.Game.Crewmate;
using UnityEngine;

namespace TownOfSushi.Options.Modifiers.Crewmate;

public sealed class DecayOptions : AbstractOptionGroup<DecayModifier>
{
    public override string GroupName => "Decay";
    public override uint GroupPriority => 36;
    public override Color GroupColor => TownOfSushiColors.Decay;

    [ModdedNumberOption("Time Before Body Rots Away", 0f, 25f, 1f, MiraNumberSuffixes.Seconds)]
    public float RotDelay { get; set; } = 5f;
}
