using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;

using TownOfUs.Modules.Wiki;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class MysticRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable
{
    public string RoleName => TOSLocale.Get(TOSNames.Mystic, "Mystic");
    public string RoleDescription => "Know When and Where Kills Happen";
    public string RoleLongDescription => "Understand when and where kills happen";
    public Color RoleColor => TownOfSushiColors.Mystic;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Mystic,
        IntroSound = TOSAudio.MediumIntroSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return $"The {RoleName} is a Crewmate Investigative role that gets an alert when someone dies."
               + MiscUtils.AppendOptionsText(GetType());
    }
}