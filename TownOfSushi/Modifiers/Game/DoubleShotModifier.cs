using TownOfSushi.Options.Modifiers;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game;

public class DoubleShotModifier : TOSGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Double Shot";
    public override string IntroInfo => "You have a second chance when guessing";

    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.DoubleShot;
    public override ModifierFaction FactionType => ModifierFaction.AssailantUtility;
    
    // YES this is scuffed, a better solution will be used at a later time
    public override bool ShowInFreeplay => false;

    public bool Used { get; set; }

    public string GetAdvancedDescription()
    {
        return
            "You get a second chance when you fail to guess a player in a meeting.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "You have an extra chance when assassinating";
    }

    public override int GetAssignmentChance()
    {
        return 0;
    }

    public override int GetAmountPerGame()
    {
        return 0;
    }
    public override int CustomAmount =>
        (int)OptionGroupSingleton<ImpostorDoubleShotOptions>.Instance.ImpostorDoubleShotAmount + (int)OptionGroupSingleton<NeutralModifierOptions>.Instance.DoubleShotAmount;

    public override int CustomChance
    {
        get
        {
            var neutOpt = OptionGroupSingleton<NeutralModifierOptions>.Instance;
            var impOpt = OptionGroupSingleton<ImpostorDoubleShotOptions>.Instance;
            var impChance = (int)OptionGroupSingleton<ImpostorDoubleShotOptions>.Instance.ImpostorDoubleShotChance;
            var neutChance = (int)neutOpt.DoubleShotChance;
            if ((int)impOpt.ImpostorDoubleShotAmount > 0 && (int)neutOpt.DoubleShotAmount > 0)
            {
                return (impChance + neutChance) / 2;
            }
            if ((int)impOpt.ImpostorDoubleShotAmount > 0)
            {
                return impChance;
            }
            else if ((int)neutOpt.DoubleShotAmount > 0)
            {
                return neutChance;
            }
            return 0;
        }
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return false;
    }
}