using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;


using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class MediumRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public override bool IsAffectedByComms => false;

    [HideFromIl2Cpp] public List<MediatedModifier> MediatedPlayers { get; } = new();

    public MysticClueType MysticHintType => MysticClueType.Death;

    public string RoleName => "Medium";
    public string RoleDescription => "Watch the spooky ghosts";
    public string RoleLongDescription => "Follow ghosts to get clues from them";
    public Color RoleColor => TownOfSushiColors.Medium;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Medium,
        IntroSound = TOSAudio.MediumIntroSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "The Medium is a Crewmate Support role who can Mediate to see one ghost per use. Both the Medium and Ghost then have an arrow showing them where each other are at all times."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Mediate",
            "Communicate with the dead, which may lead you to the killers.",
            TOSCrewAssets.MediateSprite)
    ];

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        MediatedPlayers.ForEach(mod => mod.Player?.GetModifierComponent()?.RemoveModifier(mod));
    }

    [MethodRpc((uint)TownOfSushiRpc.Mediate, LocalHandling = RpcLocalHandling.Before, SendImmediately = true)]
    public static void RpcMediate(PlayerControl source, PlayerControl target)
    {
        if ((!source.AmOwner && !target.AmOwner) || (source.Data.Role is not MediumRole && !target.Data.IsDead))
        {
            return;
        }

        var modifier = new MediatedModifier(source.PlayerId);
        target.GetModifierComponent()?.AddModifier(modifier);
    }
}