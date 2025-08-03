using MiraAPI.GameOptions;
using TownOfSushi.Options.Roles.Crewmate;
using UnityEngine;

namespace TownOfSushi.Modifiers.Crewmate;

public sealed class TrackerArrowTargetModifier(PlayerControl owner, Color color, float update) : ArrowTargetModifier(owner, color, update)
{
    public override string ModifierName => "Tracker Arrow";

    public override void OnDeath(DeathReason reason)
    {
        if (OptionGroupSingleton<TrackerOptions>.Instance.SoundOnDeactivate && Owner.AmOwner)
            TosAudio.PlaySound(TosAudio.TrackerDeactivateSound);

        base.OnDeath(reason);
    }
}
