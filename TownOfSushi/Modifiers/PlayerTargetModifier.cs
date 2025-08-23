using MiraAPI.PluginLoading;

namespace TownOfSushi.Modifiers;

[MiraIgnore]
public abstract class PlayerTargetModifier(byte ownerId) : BaseModifier
{
    public override string ModifierName => "Target";
    public override bool HideOnUi => true;

    public byte OwnerId { get; set; } = ownerId;
}