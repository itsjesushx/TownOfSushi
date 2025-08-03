using System.Collections;
using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfUs.Modules.Components;
using TownOfUs.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Options.Modifiers.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class DecayModifier : TOSGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Decay";
    public override string IntroInfo => "Your body will also rot away upon death.";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Decay;
    public override Color FreeplayFileColor => new Color32(140, 255, 255, 255);

    public override ModifierFaction FactionType => ModifierFaction.CrewmatePostmortem;

    public string GetAdvancedDescription()
    {
        return
            $"After {OptionGroupSingleton<DecayOptions>.Instance.RotDelay} second(s), your body will rot away, preventing you from being reported";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return $"Your body will rot away after {OptionGroupSingleton<DecayOptions>.Instance.RotDelay} second(s).";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.DecayChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.DecayAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsCrewmate();
    }

    public static IEnumerator StartDecay(PlayerControl player)
    {
        yield return new WaitForSeconds(OptionGroupSingleton<DecayOptions>.Instance.RotDelay);
        var Decay = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == player.PlayerId);
        if (Decay == null)
        {
            yield break;
        }

        Coroutines.Start(Decay.CoClean());
        Coroutines.Start(CrimeSceneComponent.CoClean(Decay));
    }
}