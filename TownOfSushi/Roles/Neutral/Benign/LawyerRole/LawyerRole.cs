using System.Collections;
using AmongUs.GameOptions;
using InnerNet;
using MiraAPI.Modifiers.Types;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Modifiers.Game;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class LawyerRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITOSRole, IMysticClue, IAssignableTargets, IWikiDiscoverable
{
    public string RoleName => "Lawyer";
    public string RoleDescription => TargetString();
    public string RoleLongDescription => TargetString();
    public MysticClueType MysticHintType => MysticClueType.Protective;
    public PlayerControl? Target { get; set; }
    public Color RoleColor => TownOfSushiColors.Lawyer;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralBenign;
    public Factions Faction => Factions.Neutral;
    public int Priority { get; set; } = 1;
    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TownOfSushiAudio.ArsoIgniteSound,
        MaxRoleCount = 1,
        Icon = TownOfSushiAssets.Lawyer,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        TasksCountForProgress = false
    };

    public void CheckTargetDeath(PlayerControl victim)
    {
        if (Player.HasDied()) return;

        if (Target == null || victim == Target)
        {
            var roleType = OptionGroupSingleton<LawyerOptions>.Instance.OnTargetDeath switch
            {
                BecomeOptions.Crew => (ushort)RoleTypes.Crewmate,
                BecomeOptions.Jester => RoleId.Get<JesterRole>(),
                BecomeOptions.Amnesiac => RoleId.Get<AmnesiacRole>(),
                BecomeOptions.Thief => RoleId.Get<ThiefRole>(),
                _ => (ushort)RoleTypes.Crewmate,
            };

            Player.ChangeRole(roleType);
        }
    }
    public override bool DidWin(GameOverReason gameOverReason)
    {
        var clientMod = ModifierUtils.GetActiveModifiers<LawyerClientModifier>().FirstOrDefault(x => x.OwnerId == Player.PlayerId);
        if (clientMod == null)
        {
            return false;
        }
        return clientMod.Player.Data.Role.DidWin(gameOverReason) || clientMod.Player.GetModifiers<GameModifier>().Any(x => x.DidWin(gameOverReason) == true);
    }


    private string TargetString()
    {
        if (!Target)
            return "Defend your client";

        return $"Defend {Target?.Data.PlayerName} at all costs";
    }

    public static bool LawyerSeesRoleVisibilityFlag(PlayerControl player)
    {
        var lawyerKnowsClientRole =
            OptionGroupSingleton<LawyerOptions>.Instance.LawyerKnowsTargetRole &&
            PlayerControl.LocalPlayer.IsRole<LawyerRole>() &&
            PlayerControl.LocalPlayer.GetRole<LawyerRole>()!.Target == player;

        var clientKnowsLawyer =
            OptionGroupSingleton<LawyerOptions>.Instance.LawyerTargetKnows &&
            player.IsRole<LawyerRole>() &&
            player.GetRole<LawyerRole>()!.Target == PlayerControl.LocalPlayer;

        return clientKnowsLawyer || lawyerKnowsClientRole;
    }
    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (!OptionGroupSingleton<LawyerOptions>.Instance.CanButton)
        {
            player.RemainingEmergencies = 0;
        }

        if (TutorialManager.InstanceExists && Target == null &&
            AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started && Player.AmOwner &&
            Player.IsHost())
        {
            Coroutines.Start(SetTutorialTargets(this));
        }
    }
    private static IEnumerator SetTutorialTargets(LawyerRole law)
    {
        yield return new WaitForSeconds(0.01f);
        law.AssignTargets();
    }
    public void AssignTargets()
    {
        var lawyers = PlayerControl.AllPlayerControls.ToArray()
            .Where(x => x.IsRole<LawyerRole>() && !x.HasDied());

        foreach (var lawyer in lawyers)
        {
            var filtered = PlayerControl.AllPlayerControls.ToArray()
                .Where(x => !x.IsRole<LawyerRole>() && !x.HasDied() && x.IsKillerRole() && !x.HasModifier<AllianceGameModifier>()).ToList();

            if (filtered.Count > 0)
            {
                System.Random rndIndex = new();
                var randomTarget = filtered[rndIndex.Next(0, filtered.Count)];

                RpcSetLawyerTarget(lawyer, randomTarget);
            }
            else
            {
                lawyer.GetRole<LawyerRole>()!.CheckTargetDeath(null);
            }
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.SetLawyerTarget, SendImmediately = true)]
    public static void RpcSetLawyerTarget(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not LawyerRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcSetLawyerTarget - Invalid lawyer");
            return;
        }

        if (target == null)
        {
            return;
        }

        var role = player.GetRole<LawyerRole>();

        if (role == null)
        {
            return;
        }
        role.Target = target;

        target.AddModifier<LawyerClientModifier>(player.PlayerId);
    }

    public string GetAdvancedDescription()
    {
        return
            "The Lawyer is a Neutral Benign Role that needs to protect their client from dying. " +
            "The Lawyer wins with the client as long as their client is the winner." +
            Utils.AppendOptionsText(GetType());
    }
}