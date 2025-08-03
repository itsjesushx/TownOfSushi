using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfSushi.Modifiers.Game.Universal;
using UnityEngine;

namespace TownOfSushi.Options.Modifiers.Universal
{
    public sealed class ArmoredOptions : AbstractOptionGroup
    {
        public override string GroupName => "Armored";
        public override uint GroupPriority => 29;
        public override Color GroupColor => ArmoredModifier.Color;

        [ModdedNumberOption("Armored Reset Cooldown", 10f, 60f, 5f, MiraNumberSuffixes.Seconds)]
        public float ResetCooldown { get; set; } = 10f;
    }
}