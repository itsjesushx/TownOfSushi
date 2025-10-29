using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class VeteranRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITOSCrewRole, IWikiDiscoverable, IMysticClue
{
    public override bool IsAffectedByComms => false;

    public int Alerts { get; set; }
    public string RoleName => "Veteran";
    public string RoleDescription => "Alert To Kill Anyone Who Interacts With You";
    public string RoleLongDescription => "Alert to kill whoever who interacts with you.";
    public MysticClueType MysticHintType => MysticClueType.Trickster;
    public Color RoleColor => TownOfSushiColors.Veteran;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateKilling;
    public bool IsPowerCrew => Alerts > 0; // Stop end game checks if the veteran can still alert

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Veteran,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Impostor)
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Crewmate Killing role that can go on alert and kill anyone who interacts with them."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Alert",
            $"When the Veteran is on alert, any player who interacts with them will be instantly killed, with the exception of Pestilence and shielded players, who will ignore the attack.",
            TOSCrewAssets.AlertSprite)
    ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        Alerts = (int)OptionGroupSingleton<VeteranOptions>.Instance.MaxNumAlerts;
    }
}