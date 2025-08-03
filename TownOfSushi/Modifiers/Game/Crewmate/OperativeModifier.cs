using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Buttons.Modifiers;
using TownOfSushi.Modifiers.Game.Universal;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Options.Modifiers.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class OperativeModifier : TosGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Operative";
    public override LoadableAsset<Sprite>? ModifierIcon => TosModifierIcons.Operative;
    public override string GetDescription() => $"Utilize the Cameras from anywhere";
    public override ModifierFaction FactionType => ModifierFaction.CrewmateUtility;

    public override void OnActivate()
    {
        base.OnActivate();

        if (!Player.AmOwner) return;
        CustomButtonSingleton<SecurityButton>.Instance.AvailableCharge = OptionGroupSingleton<OperativeOptions>.Instance.StartingCharge;
    }
    public static void OnRoundStart()
    {
        CustomButtonSingleton<SecurityButton>.Instance.AvailableCharge += OptionGroupSingleton<OperativeOptions>.Instance.RoundCharge;
    }
    public static void OnTaskComplete()
    {
        CustomButtonSingleton<SecurityButton>.Instance.AvailableCharge += OptionGroupSingleton<OperativeOptions>.Instance.TaskCharge;
    }

    public override int GetAssignmentChance() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.OperativeChance;
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.OperativeAmount;
	
    public override bool IsModifierValidOn(RoleBehaviour role)
	{
		return base.IsModifierValidOn(role) && role.IsCrewmate() && !role.Player.GetModifierComponent().HasModifier<SatelliteModifier>(true) && !role.Player.GetModifierComponent().HasModifier<ButtonBarryModifier>(true);
	}
    public string GetAdvancedDescription()
    {
        return
            $"Use cameras at anytime with a limited battery charge."
               + MiscUtils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];
}
