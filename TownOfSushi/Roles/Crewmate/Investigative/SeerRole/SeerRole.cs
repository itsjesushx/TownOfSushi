using System.Text;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class SeerRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITOSRole, IWikiDiscoverable, IMysticClue
{
    public override bool IsAffectedByComms => false;
    public string RoleName => "Seer";
    public string RoleDescription => "Reveal the alliance of everyone";
    public string RoleLongDescription => "Reveal alliances of other players to find the Impostors";
    public MysticClueType MysticHintType => MysticClueType.Insight;
    public Color RoleColor => TownOfSushiColors.Seer;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;
    public Factions Faction => Factions.Crewmate;

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
        return "The Seer is a Crewmate Investigative role that can reveal the alliance of other players."
               + Utils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Reveal",
            "Reveal the faction of a player",
            TownOfSushiAssets.SeerSprite)
    ];
}