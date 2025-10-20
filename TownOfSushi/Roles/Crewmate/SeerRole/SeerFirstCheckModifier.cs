namespace TownOfSushi.Roles.Crewmate;

public sealed class SeerFirstCheckModifier : BaseModifier
{
    public override string ModifierName => "SeerFirstCheck";
    public override bool HideOnUi => true;

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}