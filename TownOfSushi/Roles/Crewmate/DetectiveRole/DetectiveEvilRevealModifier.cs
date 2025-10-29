namespace TownOfSushi.Roles.Crewmate;

public sealed class DetectiveEvilRevealModifier : BaseModifier
{
    public override string ModifierName => "Detective Evil Reveal";
    public override bool HideOnUi => true;

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}