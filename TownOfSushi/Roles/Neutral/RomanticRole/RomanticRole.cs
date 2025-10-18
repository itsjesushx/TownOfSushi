using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Modifiers.Types;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Modifiers;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class RomanticRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Romantic";
    public string RoleDescription => TargetString();
    public string RoleLongDescription => TargetString();
    public MysticClueType MysticHintType => MysticClueType.Protective;
    public PlayerControl? Target { get; set; }
    public bool HasBeloved { get; set; }
    public Color RoleColor => TownOfSushiColors.Romantic;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralBenign;
    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TOSAudio.ArsoIgniteSound,
        MaxRoleCount = 1,
        Icon = TOSRoleIcons.Romantic,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        TasksCountForProgress = false
    };
    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (!Player.HasModifier<BasicGhostModifier>() && Player.HasDied())
        {
            Player.AddModifier<BasicGhostModifier>();
        }
    }
    public void CheckTargetDeath(PlayerControl victim)
    {
        if (Player.HasDied()) return;

        // Logger<TownOfSushiPlugin>.Error($"OnPlayerDeath '{victim.Data.PlayerName}'");
        if (Target == null || victim == Target)
        {
            var roleType = OptionGroupSingleton<RomanticOptions>.Instance.OnTargetDeath switch
            {
                BecomeOptions.Crew => (ushort)RoleTypes.Crewmate,
                BecomeOptions.Jester => RoleId.Get<JesterRole>(),
                BecomeOptions.Amnesiac => RoleId.Get<AmnesiacRole>(),
                BecomeOptions.Thief => RoleId.Get<ThiefRole>(),
                _ => (ushort)RoleTypes.Crewmate,
            };

            // Logger<TownOfSushiPlugin>.Error($"OnPlayerDeath - ChangeRole: '{roleType}'");
            Player.ChangeRole(roleType);

            if ((roleType == RoleId.Get<JesterRole>() && OptionGroupSingleton<JesterOptions>.Instance.ScatterOn) ||
                (roleType == RoleId.Get<AmnesiacRole>() && OptionGroupSingleton<AmnesiacOptions>.Instance.ScatterOn))
            {
                StartCoroutine(Effects.Lerp(0.2f, new Action<float>((p) =>
                {
                    Player.GetModifier<ScatterModifier>()?.OnRoundStart();
                })));
            }
        }
    }
    private string TargetString()
    {
        if (!Target)
        {
            return "Protect Your Beloved With Your Life!";
        }

        return $"Protect {Target?.Data.PlayerName} With Your Life!";
    }

    [MethodRpc((uint)TownOfSushiRpc.SetRomanticTarget, SendImmediately = true)]
    public static void RpcSetRomanticBeloved(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not RomanticRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcSetRomanticBeloved - Invalid romantic");
            return;
        }

        if (target == null)
        {
            return;
        }

        var role = player.GetRole<RomanticRole>();

        if (role == null)
        {
            return;
        }
        role.Target = target;
        role.HasBeloved = true;

        target.AddModifier<RomanticBelovedModifier>(player.PlayerId);
    }
    public override bool DidWin(GameOverReason gameOverReason)
    {
        var romMod = ModifierUtils.GetActiveModifiers<RomanticBelovedModifier>().FirstOrDefault(x => x.OwnerId == Player.PlayerId);
        if (romMod == null)
        {
            return false;
        }
        return romMod.Player.Data.Role.DidWin(gameOverReason) || romMod.Player.GetModifiers<GameModifier>().Any(x => x.DidWin(gameOverReason) == true);
    }

    public static bool RomamticSeesRoleVisibilityFlag(PlayerControl player)
    {
        var romKnowsTargetRole = OptionGroupSingleton<RomanticOptions>.Instance.RomanticKnowsTargetRole &&
        PlayerControl.LocalPlayer.IsRole<RomanticRole>() &&
        PlayerControl.LocalPlayer.GetRole<RomanticRole>()!.Target == player;

        var romTargetKnows = OptionGroupSingleton<RomanticOptions>.Instance.RomanticTargetKnows &&
        player.IsRole<RomanticRole>() && player.GetRole<RomanticRole>()!.Target == PlayerControl.LocalPlayer;

        return romTargetKnows || romKnowsTargetRole;
    }

    public string GetAdvancedDescription()
    {
        return "The Romantic is a Neutral Benign role that can pick a target to be their beloved, once they gain a beloved they get a protect button that works like a Guardian Angel protect button. If the beloved wins, so does the Romantic." + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Protect",
            "Protect your beloved from attacks",
            TOSNeutAssets.RomanticPick),
        new("Pick",
            "Create a beloved in order to get a win condition",
            TOSNeutAssets.RomanticProtect)
    ];
}