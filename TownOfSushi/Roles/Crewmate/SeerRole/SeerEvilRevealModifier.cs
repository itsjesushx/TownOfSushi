namespace TownOfSushi.Roles.Crewmate;

public sealed class SeerEvilRevealModifier : BaseModifier
{
    public override string ModifierName => "Seer Evil Reveal";
    public override bool HideOnUi => true;

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}