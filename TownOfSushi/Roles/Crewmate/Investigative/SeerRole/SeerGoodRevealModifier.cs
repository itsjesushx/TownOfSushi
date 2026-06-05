namespace TownOfSushi.Roles.Crewmate;

public sealed class SeerGoodRevealModifier : BaseModifier
{
    public override string ModifierName => "Seer Good Reveal";
    public override bool HideOnUi => true;

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}