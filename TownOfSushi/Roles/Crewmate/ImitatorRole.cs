using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using TownOfSushi.Modifiers.Crewmate;

using TownOfUs.Modules.Wiki;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class ImitatorRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable
{
    public string RoleName => "Imitator";
    public string RoleDescription => "Use Dead Roles To Benefit The Crew";
    public string RoleLongDescription => "Use the true-hearted dead to benefit the crew once more";
    public Color RoleColor => TownOfSushiColors.Imitator;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateSupport;

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
            $"All crewmate roles are available besides Imitator, and Crewmate. {TOSLocale.Get(TOSNames.Politician, "Politician")}, {TOSLocale.Get(TOSNames.Mayor, "Mayor")}, Prosecutor and Jailor are limited," + " as they can only be selected if no other Imitators exist. Jailor and Prosecutor cannot use their meeting abilities, and Vigi does not get safe shots.",
            TOSCrewAssets.InspectSprite),
        new("Neutral Counterparts",
            $"{TOSLocale.Get(TOSNames.Amnesiac, "Amnesiac")} ⇨ {TOSLocale.Get(TOSNames.Medic, "Medic")} | "
            + "Exe ⇨ Snitch\n"
            + $"{TOSLocale.Get(TOSNames.Glitch, "Glitch")} ⇨ {TOSLocale.Get(TOSNames.Sheriff, "Sheriff")} | "
            + "GA ⇨ Cleric | "
            + "Inquis ⇨ Oracle\n"
            + $"{TOSLocale.Get(TOSNames.Jester, "Jester")} ⇨ Plumber | "
            + "Pb/Pest ⇨ Aurial | "
            + "SC ⇨ Medium | "
            + "WW ⇨ Hunter",
            TOSNeutAssets.GuardSprite),
        new("Impostor Counterparts",
            $"{TOSLocale.Get(TOSNames.Bomber, "Bomber")} ⇨ {TOSLocale.Get(TOSNames.Trapper, "Trapper")} | "
            + $"Escapist ⇨ {TOSLocale.Get(TOSNames.Transporter, "Transporter")}\n"
            + "Hypnotist ⇨ Lookout | "
            + "Janitor ⇨ Detective\n"
            + $"Miner ⇨ {TOSLocale.Get(TOSNames.Engineer, "Engineer")} | "
            + "BountyHunter ⇨ Tracker\n"
            + "Undertaker ⇨ Altruist | "
            + $"Hexblade ⇨ {TOSLocale.Get(TOSNames.Veteran, "Veteran")}",
            TOSImpAssets.DragSprite),
    ];

    public string SecondTabName => "Role Guide";

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        player.AddModifier<ImitatorCacheModifier>();
    }
}