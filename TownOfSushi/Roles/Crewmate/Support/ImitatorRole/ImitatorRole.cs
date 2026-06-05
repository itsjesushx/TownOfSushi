using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class ImitatorRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITOSRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Imitator";
    public string RoleDescription => "Use dead roles to benefit crewmates";
    public string RoleLongDescription => "Use the true-hearted dead to benefit the crew once more";
    public Color RoleColor => TownOfSushiColors.Imitator;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateSupport;
    public Factions Faction => Factions.Crewmate;
    public MysticClueType MysticHintType => MysticClueType.Perception;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TownOfSushiAssets.Imitator,
        IntroSound = TownOfSushiAudio.AdministratorIntroSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITOSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return "The Imitator is a Crewmate Support role that can select a dead crewmate to imitate their role. " +
               "They will become their role and abilities until they change targets. " +
               "Certain roles are innacessible if there are multiple living imitators."
               + Utils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Crewmate Imitation",
            $"All crewmate roles are available besides Imitator, and Crewmate, Prosecutor and Jailor are limited," + " as they can only be selected if no other Imitators exist. Jailor and Prosecutor cannot use their meeting abilities, and Vigi does not get safe shots.",
            TownOfSushiAssets.InspectSprite),
        new("Neutral Counterparts",
            $"Amnesiac ⇨ Medic | "
            + $"Glitch ⇨ Sheriff | "
            + $"Jester ⇨ Plumber | "
            + "Pb/Pest ⇨ Aurial | "
            + "WW ⇨ Hunter",
            TownOfSushiAssets.GuardSprite),
        new("Impostor Counterparts",
            $"Bomber ⇨ Trapper | "
            + $"Escapist ⇨ Transporter\n"
            + "Hypnotist ⇨ Lookout | "
            + "Janitor ⇨ Inspector\n"
            + $"Miner ⇨ Engineer | "
            + $"Hexblade ⇨ Veteran",
            TownOfSushiAssets.DragSprite),
    ];

    public string SecondTabName => "Role Guide";

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        player.AddModifier<ImitatorCacheModifier>();
    }
}