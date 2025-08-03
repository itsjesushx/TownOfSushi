using MiraAPI.Modifiers;

namespace TownOfSushi.Modifiers.Crewmate;

public sealed class DetectiveGoodRevealModifier : BaseModifier
{
    public override string ModifierName => "DetectiveGoodReveal";
    public override bool HideOnUi => true;

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}