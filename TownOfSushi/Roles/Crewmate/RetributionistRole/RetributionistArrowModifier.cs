using TownOfSushi.Modifiers;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class RetributionistArrowModifier(PlayerControl owner, Color color) : ArrowTargetModifier(owner, color, 0)
{
    public override string ModifierName => "Retributionist Arrow";

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent?.RemoveModifier(this);
    }
}