using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class ImitatorRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Imitator";
    public string RoleDescription => "Use dead roles to benefit crewmates";
    public string RoleLongDescription => "Use the true-hearted dead to benefit the crew once more";
    public Color RoleColor => TownOfSushiColors.Imitator;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateSupport;
    public MysticClueType MysticHintType => MysticClueType.Perception;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Imitator,
        IntroSound = TOSAudio.AdministratorIntroSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return "The Imitator is a Crewmate Support role that can select a dead crewmate to imitate their role. " +
               "They will become their role and abilities until they change targets. " +
               "Certain roles are innacessible if there are multiple living imitators."
               + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Crewmate Imitation",
            $"All crewmate roles are available besides Imitator, and Crewmate. Politician, Mayor, Prosecutor and Jailor are limited," + " as they can only be selected if no other Imitators exist. Jailor and Prosecutor cannot use their meeting abilities, and Vigi does not get safe shots.",
            TOSCrewAssets.InspectSprite),
        new("Neutral Counterparts",
            $"Amnesiac ⇨ Medic | "
            + $"Glitch ⇨ Vigilante | "
            + "GA ⇨ Cleric | "
            + "Inquis ⇨ Oracle\n"
            + $"Jester ⇨ Plumber | "
            + "Pb/Pest ⇨ Aurial | "
            + "SC ⇨ Medium | "
            + "WW ⇨ Hunter",
            TOSNeutAssets.GuardSprite),
        new("Impostor Counterparts",
            $"Bomber ⇨ Trapper | "
            + $"Escapist ⇨ Transporter\n"
            + "Hypnotist ⇨ Lookout | "
            + "Janitor ⇨ Inspector\n"
            + $"Miner ⇨ Engineer | "
            + "BountyHunter ⇨ Sonar\n"
            + "Undertaker ⇨ Retributionist | "
            + $"Hexblade ⇨ Veteran",
            TOSImpAssets.DragSprite),
    ];

    public string SecondTabName => "Role Guide";

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        player.AddModifier<ImitatorCacheModifier>();
    }
}