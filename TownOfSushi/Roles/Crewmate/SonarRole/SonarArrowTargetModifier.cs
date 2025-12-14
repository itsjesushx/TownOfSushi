using TownOfSushi.Modifiers;
using TownOfSushi.Modules.RainbowMod;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class SonarArrowTargetModifier(PlayerControl owner, Color color, float update)
    : ArrowTargetModifier(owner, color, update)
{
    public override string ModifierName => "Sonar Arrow";

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
        if (OptionGroupSingleton<SonarOptions>.Instance.SoundOnDeactivate && Owner.AmOwner)
        {
            TOSAudio.PlaySound(TOSAudio.SonarDeactivateSound);
        }

        base.OnDeath(reason);
    }
}