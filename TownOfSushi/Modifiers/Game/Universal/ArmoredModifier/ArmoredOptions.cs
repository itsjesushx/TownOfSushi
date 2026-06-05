using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal
{
    public sealed class ArmoredOptions : AbstractOptionGroup<ArmoredModifier>
    {
        public override string GroupName => "Armored";
        public override uint GroupPriority => 29;
        public override Color GroupColor => TownOfSushiColors.Armored;
        [ModdedNumberOption("Armored Amount", 0, 5)]
        public float ArmoredAmount { get; set; } = 0;

        public ModdedNumberOption ArmoredChance { get; } = new("Armored Chance", 0f, 0f, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<ArmoredOptions>.Instance.ArmoredAmount > 0
        };

        [ModdedNumberOption("Armored Reset Cooldown", 10f, 60f, 5f, MiraNumberSuffixes.Seconds)]
        public float ResetCooldown { get; set; } = 10f;
    }
}