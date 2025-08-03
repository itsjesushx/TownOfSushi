using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using TownOfUs.Modules.Wiki;
using TownOfSushi.Utilities;
using UnityEngine;
using MiraAPI.Hud;
using TownOfSushi.Options.Modifiers.Crewmate;
using MiraAPI.GameOptions;
using TownOfSushi.Buttons.Crewmate;

namespace TownOfSushi.Roles.Crewmate;

public sealed class OperativeRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable
{
    public override bool IsAffectedByComms => true;
    public string RoleName => "Operative";
    public string RoleDescription => "Acess Admin table and Vitals from anywhere";
    public string RoleLongDescription => "Acess devices around the map from anywhere";
    public Color RoleColor => TownOfSushiColors.Operative;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Operative,
        IntroSound = TOSAudio.TrackerActivateSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }
    public static void OnRoundStart()
    {
        CustomButtonSingleton<SecurityButton>.Instance.AvailableCharge +=
            OptionGroupSingleton<OperativeOptions>.Instance.RoundCharge;
        
        CustomButtonSingleton<OperativeButton>.Instance.AvailableCharge +=
            OptionGroupSingleton<OperativeOptions>.Instance.VitalsRoundCharge;
    }

    public static void OnTaskComplete()
    {
        CustomButtonSingleton<SecurityButton>.Instance.AvailableCharge +=
            OptionGroupSingleton<OperativeOptions>.Instance.TaskCharge;
        
        CustomButtonSingleton<OperativeButton>.Instance.AvailableCharge +=
            OptionGroupSingleton<OperativeOptions>.Instance.VitalsRoundCharge;
    }

    public string GetAdvancedDescription()
    {
        return
            $"The Operative is a Crewmate Investigative role that can use admin and vitals from anywhere around the map."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Vitals",
            "Acess Vitals panel from anywhere.",
            TOSAssets.VitalsSprite),
        new("Admin Table",
            $"Use Admin table from anywhewre.",
            TOSAssets.AdminSprite)
    ];
}