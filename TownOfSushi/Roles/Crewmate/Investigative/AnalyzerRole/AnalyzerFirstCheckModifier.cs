namespace TownOfSushi.Roles.Crewmate;

public sealed class AnalyzerFirstCheckModifier(byte analyzerId) : BaseModifier
{
    public override string ModifierName => "AnalyzerFirstCheck";
    public override bool HideOnUi => true;
    public byte AnalyzerId { get; } = analyzerId;
    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}