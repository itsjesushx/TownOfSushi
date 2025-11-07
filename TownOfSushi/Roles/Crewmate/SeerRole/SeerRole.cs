using System.Text;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class SeerRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Seer";
    public string RoleDescription => "Check the faction of 2 players";
    public string RoleLongDescription => "Check the faction of 2 players to see if they match";
    public MysticClueType MysticHintType => MysticClueType.Perception;
    public Color RoleColor => TownOfSushiColors.Seer;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;
    public bool InvestigatedFirst { get; set; }

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Detective,
        IntroSound = TOSAudio.QuestionSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return $"The {RoleName} is able to choose two targets, upon a meeting starts, the {RoleName} will be notified wether the targets are on the same faction or not, in the voting screen the {RoleName} will see a green Y if they are, else they will have a red X next to their names."
               + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Check",
            "Check the faction between 2 players",
            TOSCrewAssets.DetectiveSprite)
    ];
}