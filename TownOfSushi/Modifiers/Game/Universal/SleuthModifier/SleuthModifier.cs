using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class SleuthModifier : UniversalGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Sleuth";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Sleuth;

    public override ModifierFaction FactionType => ModifierFaction.UniversalPassive;
    public override Color FreeplayFileColor => new Color32(180, 180, 180, 255);
    public List<byte> Reported { get; set; } = [];

    public string GetAdvancedDescription()
    {
        return
            "You will see the roles of bodies you report.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "Know the roles of bodies you report.";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<SleuthOptions>.Instance.SleuthChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<SleuthOptions>.Instance.SleuthAmount;
    }

    public static bool SleuthVisibilityFlag(PlayerControl player)
    {
        if (PlayerControl.LocalPlayer.HasModifier<SleuthModifier>())
        {
            var mod = PlayerControl.LocalPlayer.GetModifier<SleuthModifier>()!;
            return mod.Reported.Contains(player.PlayerId);
        }

        return false;
    }
}