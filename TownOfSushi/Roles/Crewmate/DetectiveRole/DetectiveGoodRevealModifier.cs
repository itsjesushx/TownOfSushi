namespace TownOfSushi.Roles.Crewmate;

public sealed class DetectiveGoodRevealModifier : BaseModifier
{
    public override string ModifierName => "Detective Good Reveal";
    public override bool HideOnUi => true;

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}