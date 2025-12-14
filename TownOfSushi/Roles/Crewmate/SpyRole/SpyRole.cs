using System.Text;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class SpyRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Spy";
    public string RoleDescription => "Confuse the Impostors into trusting you";
    public string RoleLongDescription => "A Crewmate who is seen as an Impostor by the Impostors.";
    public MysticClueType MysticHintType => MysticClueType.Trickster;
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateSupport;
    public bool HasImpostorVision => OptionGroupSingleton<SpyOptions>.Instance.SpyHasImpVision;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Administrator,
        IntroSound = CustomRoleUtils.GetIntroSound(AmongUs.GameOptions.RoleTypes.Impostor),
        CanUseVent = OptionGroupSingleton<SpyOptions>.Instance.SpyCanHideInVents,
        MaxRoleCount = 1,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return "The Spy is a Crewmate Support role that appears as an Impostor to the Impostors. " +
               "They can use this to their advantage by tricking the Impostors into thinking they are on their side. " +
               "However, they must be careful not to reveal their true identity to the Crewmates, as this could lead to their downfall."
               + MiscUtils.AppendOptionsText(GetType());
    }
}