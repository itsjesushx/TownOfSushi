namespace TownOfSushi.Roles.Impostor;

public sealed class ConsigliereRevealedModifier(byte consigliereId) : BaseModifier
{
    public override string ModifierName => "Revealed";
    public override bool HideOnUi => true;

    public byte ConsigliereId { get; } = consigliereId;
    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}