using MiraAPI.Hud;
using UnityEngine;
using static ShipStatus;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class AdministratorModifier : TOSGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Administrator";
    public override string IntroInfo => "You have extra information on the Admin Table";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSRoleIcons.Administrator;
    public override Color FreeplayFileColor => new Color32(140, 255, 255, 255);

    public override ModifierFaction FactionType => ModifierFaction.CrewmateUtility;

    public string GetAdvancedDescription()
    {
        return
            "The Administrator gains extra information on the admin table. They now not only see how many people are in a room, but will also see who is in every room."
            + MiscUtils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "Gain extra information on the Admin Table.";
    }

    public override void OnActivate()
    {
        base.OnActivate();

        if (!Player.AmOwner)
        {
            return;
        }

        CustomButtonSingleton<AdministratorAdminTableModifierButton>.Instance.AvailableCharge =
            OptionGroupSingleton<AdministratorOptions>.Instance.StartingCharge;
    }

    public static void OnRoundStart()
    {
        CustomButtonSingleton<AdministratorAdminTableModifierButton>.Instance.AvailableCharge +=
            OptionGroupSingleton<AdministratorOptions>.Instance.RoundCharge;
    }

    public static void OnTaskComplete()
    {
        CustomButtonSingleton<AdministratorAdminTableModifierButton>.Instance.AvailableCharge +=
            OptionGroupSingleton<AdministratorOptions>.Instance.TaskCharge;
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<AdministratorOptions>.Instance.AdministratorChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<AdministratorOptions>.Instance.AdministratorAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsCrewmate() &&
               Instance.Type != MapType.Fungle;
    }
}