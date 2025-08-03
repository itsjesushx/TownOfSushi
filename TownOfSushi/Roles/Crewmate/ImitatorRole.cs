using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Modules.Wiki;

using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class ImitatorRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable
{
    public string RoleName => "Imitator";
    public string RoleDescription => "Use Dead Roles To Benefit The Crew";
    public string RoleLongDescription => "Use the true-hearted dead to benefit the crew once more";
    public Color RoleColor => TownOfSushiColors.Imitator;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateSupport;
    public DoomableType DoomHintType => DoomableType.Perception;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TosRoleIcons.Imitator,
        IntroSound = TosAudio.SpyIntroSound,
    };
    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        player.AddModifier<ImitatorCacheModifier>();
    }

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
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Crewmate Imitation",
            $"All crewmate roles are available besides Imitator, and Crewmate. Politician, Mayor, Prosecutor and Jailor are limited,"
            + " as they can only be selected if no other Imitators exist. Jailor and Prosecutor cannot use their meeting abilities, and Vigi does not get safe shots.",
            TosCrewAssets.InspectSprite),
        new("Neutral Counterparts",
            "Amne ⇨ Mystic | "
            + "Doom ⇨ Vigi | "
            + "Exe ⇨ Snitch\n"
            + "Glitch ⇨ Sheriff | "
            + "GA ⇨ Cleric | "
            + "Inquis ⇨ Oracle\n"
            + "Jester ⇨ Swapper | "
            + "Merc ⇨ Warden\n"
            + "Pb/Pest ⇨ Aurial | "
            + "SC ⇨ Medium | "
            + "WW ⇨ Hunter",
            TosNeutAssets.GuardSprite),
        new("Impostor Counterparts",
            "Bomber ⇨ Trapper | "
            + "Escapist ⇨ Transporter\n"
            + "Hypnotist ⇨ Lookout | "
            + "Janitor ⇨ Detective\n"
            + "Miner ⇨ Engineer | "
            + "Scavenger ⇨ Tracker\n"
            + "Undertaker ⇨ Altruist | "
            + "Warlock ⇨ Veteran",
            TosImpAssets.DragSprite),
    ];
    public string SecondTabName => "Role Guide";
}
