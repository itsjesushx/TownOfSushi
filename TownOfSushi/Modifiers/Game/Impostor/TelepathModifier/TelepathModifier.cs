using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Impostor;

public sealed class TelepathModifier : TOSGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Telepath";
    public override Color FreeplayFileColor => new Color32(255, 25, 25, 255);

    public override string IntroInfo => "Know information about teammates' kills" +
                                        (OptionGroupSingleton<TelepathOptions>.Instance.KnowDeath
                                            ? " and deaths."
                                            : ".");

    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Telepath;
    public override ModifierFaction FactionType => ModifierFaction.ImpostorPostmortem;

    public float Timer { get; set; }

    public string GetAdvancedDescription()
    {
        var options = OptionGroupSingleton<TelepathOptions>.Instance;
        return
            (options.KnowKillLocation ? "Know when & where your teammate kills" : "Know when your teammate kills")
            + (options.KnowDeath && !options.KnowDeathLocation ? ", know when they die." : string.Empty)
            + (options.KnowDeath && options.KnowDeathLocation ? ", know when & where they die." : string.Empty)
            + MiscUtils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return (OptionGroupSingleton<TelepathOptions>.Instance.KnowKillLocation
                   ? "Know when & where your teammate kills"
                   : "Know when your teammate kills")
               + (OptionGroupSingleton<TelepathOptions>.Instance.KnowDeath &&
                  !OptionGroupSingleton<TelepathOptions>.Instance.KnowDeathLocation
                   ? ", know when they die"
                   : string.Empty)
               + (OptionGroupSingleton<TelepathOptions>.Instance.KnowDeath &&
                  OptionGroupSingleton<TelepathOptions>.Instance.KnowDeathLocation
                   ? ", know when & where they die."
                   : string.Empty);
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<TelepathOptions>.Instance.TelepathChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<TelepathOptions>.Instance.TelepathAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsImpostor() &&
               PlayerControl.AllPlayerControls.ToArray().Count(x => x.IsImpostor() && !x.HasDied()) != 1;
    }
}