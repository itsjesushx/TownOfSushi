using MiraAPI.Modifiers;

namespace TownOfSushi.Modifiers.Crewmate;

public sealed class DetectiveEvilRevealModifier : BaseModifier
{
    public override string ModifierName => "DetectiveEvilReveal";
    public override bool HideOnUi => true;

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}