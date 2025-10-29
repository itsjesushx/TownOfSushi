using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class MonarchRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Monarch";
    public string RoleDescription => "Knight Players To Help The Crew";
    public string RoleLongDescription => "Knight players to give them an extra vote during meetings.";
    public Color RoleColor => TownOfSushiColors.Monarch;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateSupport;
    public MysticClueType MysticHintType => MysticClueType.Perception;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSCrewAssets.KnightSprite,
        MaxRoleCount = 1,
        DefaultRoleCount = 0,
        DefaultChance = 0,
    };
    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
    }
    public override void OnDeath(DeathReason reason)
    {
        RoleBehaviourStubs.OnDeath(this, reason);

        var player = ModifierUtils.GetPlayersWithModifier<MonarchKnightedModifier>(x => x.Monarch.AmOwner).FirstOrDefault();

        player?.RpcRemoveModifier<MonarchKnightedModifier>();
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
    }
    public string GetAdvancedDescription()
    {
        return "The Monarch is a Crewmate Support role that can give players another vote by knighting them.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Knight", 
            "Gives a player an extra vote during meetings",
            TOSCrewAssets.KnightSprite)
    ];
}