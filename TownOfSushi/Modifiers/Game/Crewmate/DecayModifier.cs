using System.Collections;
using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfUs.Modules.Components;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Options.Modifiers.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class DecayModifier : TosGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Decay";
    public override LoadableAsset<Sprite>? ModifierIcon => TosModifierIcons.Decay;
    public override string GetDescription() => $"Your body will rot away after {OptionGroupSingleton<DecayOptions>.Instance.RotDelay} second(s).";
    public override ModifierFaction FactionType => ModifierFaction.CrewmatePostmortem;

    public override int GetAssignmentChance() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.DecayChance;
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.DecayAmount;

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsCrewmate();
    }
    public static IEnumerator StartDecay(PlayerControl player)
    {
        yield return new WaitForSeconds(OptionGroupSingleton<DecayOptions>.Instance.RotDelay);
        var Decay = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == player.PlayerId);
        if (Decay == null) yield break;
        Coroutines.Start(Decay.CoClean());
        Coroutines.Start(CrimeSceneComponent.CoClean(Decay));
    }
    public string GetAdvancedDescription()
    {
        return
            $"After {OptionGroupSingleton<DecayOptions>.Instance.RotDelay} second(s), your body will rot away, preventing you from being reported";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];
}
