namespace TownOfSushi.Roles.Crewmate;

public sealed class SeerSecondCheckModifier : BaseModifier
{
    public override string ModifierName => "SeerSecondCheck";
    public override bool HideOnUi => true;

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}