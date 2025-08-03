using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Utilities;
using UnityEngine;


namespace TownOfSushi.Roles.Crewmate;

public sealed class MysticRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable
{
    public string RoleName => "Mystic";
    public string RoleDescription => "Know When and Where Kills Happen";
    public string RoleLongDescription => "Understand when and where kills happen";
    public Color RoleColor => TownOfSushiColors.Mystic;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;
    public DoomableType DoomHintType => DoomableType.Perception;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TosRoleIcons.Mystic,
        IntroSound = TosAudio.MediumIntroSound,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return "The Mystic is a Crewmate Investigative role that gets an alert when someone dies."
            + MiscUtils.AppendOptionsText(GetType());
    }
}
