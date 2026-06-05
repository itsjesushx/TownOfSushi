using System.Text;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class AnalyzerRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITOSRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Analyzer";
    public string RoleDescription => "Analyze the faction of two players";
    public string RoleLongDescription => "Analyze the faction of 2 players to see if they match";
    public MysticClueType MysticHintType => MysticClueType.Perception;
    public Color RoleColor => TownOfSushiColors.Analyzer;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;
    public Factions Faction => Factions.Crewmate;
    public bool InvestigatedFirst { get; set; }
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TownOfSushiAssets.Seer,
        IntroSound = TownOfSushiAudio.QuestionSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITOSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return $"The {RoleName} is able to choose two targets, upon a meeting starts, everyone will be notified on chat about the target's Faction and/or Alignment."
               + Utils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Check",
            "Check the faction between 2 players",
            TownOfSushiAssets.SeerSprite)
    ];
}