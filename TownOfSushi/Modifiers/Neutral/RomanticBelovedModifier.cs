namespace TownOfSushi.Modifiers.Neutral;

public sealed class RomanticBelovedModifier(byte romId) : PlayerTargetModifier(romId)
{
    public override string ModifierName => "Romantic Target";
    public byte RomanticId { get; } = romId;
}