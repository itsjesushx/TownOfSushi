using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Options.Modifiers.Impostor;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Impostor;

public sealed class TelepathModifier : TosGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Telepath";
    public override string GetDescription() =>
            (OptionGroupSingleton<TelepathOptions>.Instance.KnowKillLocation ? "Know when & where your teammate kills" : "Know when your teammate kills")
            + (OptionGroupSingleton<TelepathOptions>.Instance.KnowDeath && !OptionGroupSingleton<TelepathOptions>.Instance.KnowDeathLocation ? ", know when they die" : string.Empty)
            + (OptionGroupSingleton<TelepathOptions>.Instance.KnowDeath && OptionGroupSingleton<TelepathOptions>.Instance.KnowDeathLocation ? ", know when & where they die." : string.Empty);
    public override LoadableAsset<Sprite>? ModifierIcon => TosModifierIcons.Telepath;
    public override ModifierFaction FactionType => ModifierFaction.ImpostorPostmortem;

    public float Timer { get; set; }

    public override int GetAssignmentChance() => (int)OptionGroupSingleton<ImpostorModifierOptions>.Instance.TelepathChance;
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<ImpostorModifierOptions>.Instance.TelepathAmount;

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsImpostor() && PlayerControl.AllPlayerControls.ToArray().Count(x => x.IsImpostor() && !x.HasDied()) != 1;
    }
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
}
