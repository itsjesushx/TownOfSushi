using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Buttons.Modifiers;
using TownOfSushi.Modifiers.Game.Universal;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Options.Modifiers.Crewmate;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class ScientistModifier : TosGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Scientist";
    public override LoadableAsset<Sprite>? ModifierIcon => TosModifierIcons.Scientist;
    public override string GetDescription() => $"Access vitals anytime, anywhere, as long as you have charge";
    public override ModifierFaction FactionType => ModifierFaction.CrewmateUtility;
    public override void OnActivate()
    {
        base.OnActivate();

        if (!Player.AmOwner) return;
        CustomButtonSingleton<ScientistButton>.Instance.AvailableCharge = OptionGroupSingleton<ScientistOptions>.Instance.StartingCharge;
    }
    public static void OnRoundStart()
    {
        CustomButtonSingleton<ScientistButton>.Instance.AvailableCharge += OptionGroupSingleton<ScientistOptions>.Instance.RoundCharge;
    }
    public static void OnTaskComplete()
    {
        CustomButtonSingleton<ScientistButton>.Instance.AvailableCharge += OptionGroupSingleton<ScientistOptions>.Instance.TaskCharge;
    }

    public override int GetAssignmentChance() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.ScientistChance;
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.ScientistAmount;

    public override bool IsModifierValidOn(RoleBehaviour role)
	{
        if (role is TransporterRole && !OptionGroupSingleton<TransporterOptions>.Instance.CanUseVitals)
        {
            return false;
        }

        return base.IsModifierValidOn(role) && role.IsCrewmate() && role is not ScientistRole
            && !role.Player.GetModifierComponent().HasModifier<SatelliteModifier>(true)
            && !role.Player.GetModifierComponent().HasModifier<ButtonBarryModifier>(true);
    }
    public string GetAdvancedDescription()
    {
        return
            $"Access Vitals at anytime with a limited battery charge."
               + MiscUtils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];
}
