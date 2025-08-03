using MiraAPI.GameOptions;
using TownOfSushi.Modules.RainbowMod;
using TownOfSushi.Options.Roles.Crewmate;
using UnityEngine;

namespace TownOfSushi.Modifiers.Crewmate;

public sealed class TrackerArrowTargetModifier(PlayerControl owner, Color color, float update)
    : ArrowTargetModifier(owner, color, update)
{
    public override string ModifierName => "Tracker Arrow";

    public override void OnActivate()
    {
        base.OnActivate();

        if (Arrow == null)
        {
            return;
        }

        var spr = Arrow.gameObject.GetComponent<SpriteRenderer>();
        var r = Arrow.gameObject.AddComponent<BasicRainbowBehaviour>();

        r.AddRend(spr, Player.cosmetics.ColorId);
    }

    public override void OnDeath(DeathReason reason)
    {
        if (OptionGroupSingleton<TrackerOptions>.Instance.SoundOnDeactivate && Owner.AmOwner)
        {
            TOSAudio.PlaySound(TOSAudio.TrackerDeactivateSound);
        }

        base.OnDeath(reason);
    }
}