using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;

using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class LookoutRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITOSRole, IWikiDiscoverable, IMysticClue
{
    public override bool IsAffectedByComms => false;
    public string RoleName => "Lookout";
    public string RoleDescription => "Keep your eyes wide open";
    public string RoleLongDescription => "Watch other crewmates to see their every move.";
    public MysticClueType MysticHintType => MysticClueType.Hunter;
    public Color RoleColor => TownOfSushiColors.Lookout;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;
    public Factions Faction => Factions.Crewmate;
    public PlayerControl? ObservedPlayer { get; set; }

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TownOfSushiAssets.Lookout,
        IntroSound = TownOfSushiAudio.QuestionSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITOSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "The Lookout is a Crewmate Investigative role that can watch other players during rounds. They can see any movement that their target does, even venting. They cannot see if the player is doing a task or not."
            + Utils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Watch",
            "Watch a player to see their suspicious behaviour.",
            TownOfSushiAssets.WatchSprite)
    ];

    [MethodRpc((uint)TownOfSushiRpc.LookoutSeePlayer, SendImmediately = true)]
    public static void RpcSeePlayer(PlayerControl target, PlayerControl source)
    {
        if (!target.HasModifier<LookoutWatchedModifier>())
        {
            Logger<TownOfSushiPlugin>.Error("Not a watched player");
            return;
        }

        if (source.AmOwner && !target.HasDied())
        {
            PlayerControl.LocalPlayer.moveable = false;
            Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(target);
            var light = PlayerControl.LocalPlayer.lightSource;
            light.transform.SetParent(target.transform);
            light.transform.localPosition = target.Collider.offset;
        }
        else
        {
            PlayerControl.LocalPlayer.moveable = false;
            foreach (var body in UnityEngine.Object.FindObjectsOfType<DeadBody>())
            {
                if (body.ParentId == target.PlayerId)
                {
                   Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(body);
                    var light = PlayerControl.LocalPlayer.lightSource;
                    light.transform.SetParent(body.transform);
                    light.transform.localPosition = body.myCollider.offset;
                }
            }
        }

        if (target.AmOwner)
        {
            _ = new CustomMessage("You are being watched!", OptionGroupSingleton<LookoutOptions>.Instance.WatchDuration);
        }
    }
    [MethodRpc((uint)TownOfSushiRpc.LookoutUnSeePlayer, SendImmediately = true)]
    public static void RpcUnSeePlayer(PlayerControl target, PlayerControl source)
    {
        target.RpcRemoveModifier<LookoutWatchedModifier>();

        if (source.AmOwner)
        {
            PlayerControl.LocalPlayer.moveable = true;
            Camera.main.gameObject.GetComponent<FollowerCamera>().SetTarget(PlayerControl.LocalPlayer);
            var light = PlayerControl.LocalPlayer.lightSource;
            light.transform.SetParent(PlayerControl.LocalPlayer.transform);
            light.transform.localPosition = PlayerControl.LocalPlayer.Collider.offset;
        }
    }
    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);
        if (ObservedPlayer == null)
        {
            return;
        }
        RpcUnSeePlayer(ObservedPlayer, PlayerControl.LocalPlayer);
        ObservedPlayer = null;
    }
}