using TownOfSushi.Modules.RainbowMod;
using UnityEngine;

namespace TownOfSushi.Modifiers.Impostor;

public sealed class AmbusherArrowTargetModifier(PlayerControl owner, Color color, float update)
    : ArrowTargetModifier(owner, color, update)
{
    public override string ModifierName => "Ambusher Arrow";

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
        TOSAudio.PlaySound(TOSAudio.TrackerDeactivateSound);
        base.OnDeath(reason);
    }
}