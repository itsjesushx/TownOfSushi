using System.Text;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfSushi.Utilities.Appearances;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class OracleRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public override bool IsAffectedByComms => false;
    public string RoleName => "Oracle";
    public string RoleDescription => "Get Other Player's To Confess Their Sins";
    public string RoleLongDescription => "Get another player to confess on your passing";
    public MysticClueType MysticHintType => MysticClueType.Perception;
    public Color RoleColor => TownOfSushiColors.Oracle;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateProtective;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Oracle,
        IntroSound = TOSAudio.GuardianAngelSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The Oracle is a Crewmate Protective role that can get another player to confess (revealing their faction with {OptionGroupSingleton<OracleOptions>.Instance.RevealAccuracyPercentage}% accuracy if the Oracle dies) or can protect a player from meeting abilities."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Bless",
            "Blessing a player prevents any harm from being done to them in the meeting.",
            TOSCrewAssets.BlessSprite),
        new("Confess",
            $"Make a player confess in a meeting, giving a vision of 3 possible evils (including the confessor), and also reveal their faction to everyone with {OptionGroupSingleton<OracleOptions>.Instance.RevealAccuracyPercentage}% accuracy when the Oracle dies.",
            TOSCrewAssets.ConfessSprite)
    ];

    public override void OnDeath(DeathReason reason)
    {
        RoleBehaviourStubs.OnDeath(this, reason);

        RpcOracleConfess(Player);
    }

    public void ReportOnConfession()
    {
        if (!Player.AmOwner)
        {
            return;
        }

        var confessing = ModifierUtils
            .GetPlayersWithModifier<OracleConfessModifier>([HideFromIl2Cpp](x) => x.Oracle == Player).FirstOrDefault();

        if (confessing == null)
        {
            return;
        }

        var report = BuildReport(confessing);

        var title = $"<color=#{TownOfSushiColors.Oracle.ToHtmlStringRGBA()}>Oracle Confession</color>";
        MiscUtils.AddFakeChat(confessing.Data, title, report, false, true);
    }

    public static string BuildReport(PlayerControl player)
    {
        if (player.HasDied())
        {
            return "Your confessor failed to survive so you received no confession";
        }

        var allPlayers = PlayerControl.AllPlayerControls.ToArray()
            .Where(x => !x.HasDied() && x != PlayerControl.LocalPlayer && x != player).ToList();
        if (allPlayers.Count < 2)
        {
            return "Too few people alive to receive a confessional";
        }

        var options = OptionGroupSingleton<OracleOptions>.Instance;

        var evilPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && (x.IsImpostor() ||
            (x.Is(RoleAlignment.NeutralKilling) && options.ShowNeutralKillingAsEvil) ||
            (x.Is(RoleAlignment.NeutralEvil) && options.ShowNeutralEvilAsEvil) ||
            (x.Is(RoleAlignment.NeutralBenign) && options.ShowNeutralBenignAsEvil))).ToList();

        if (evilPlayers.Count == 0)
        {
            return
                $"{player.GetDefaultAppearance().PlayerName} confesses to knowing that there are no more evil players!";
        }

        allPlayers.Shuffle();
        evilPlayers.Shuffle();
        var secondPlayer = allPlayers[0];
        var firstTwoEvil = evilPlayers.Any(plr => plr == player || plr == secondPlayer);

        if (firstTwoEvil)
        {
            var thirdPlayer = allPlayers[1];

            return
                $"{player.GetDefaultAppearance().PlayerName} confesses to knowing that they, {secondPlayer.GetDefaultAppearance().PlayerName} and/or {thirdPlayer.GetDefaultAppearance().PlayerName} is evil!";
        }
        else
        {
            var thirdPlayer = evilPlayers[0];

            return
                $"{player.GetDefaultAppearance().PlayerName} confesses to knowing that they, {secondPlayer.GetDefaultAppearance().PlayerName} and/or {thirdPlayer.GetDefaultAppearance().PlayerName} is evil!";
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.OracleConfess, SendImmediately = true)]
    public static void RpcOracleConfess(PlayerControl player)
    {
        var mod = ModifierUtils.GetActiveModifiers<OracleConfessModifier>(x => x.Oracle == player).FirstOrDefault();

        if (mod != null)
        {
            mod.ConfessToAll = true;
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.OracleBless, SendImmediately = true)]
    public static void RpcOracleBless(PlayerControl exiled)
    {
        // Logger<TownOfSushiPlugin>.Message($"RpcOracleBless exiled '{exiled.Data.PlayerName}'");
        var mod = exiled.GetModifier<OracleBlessedModifier>();

        if (mod != null)
            // Logger<TownOfSushiPlugin>.Message($"RpcOracleBless exiled '{exiled.Data.PlayerName}' SavedFromExile");
        {
            mod.SavedFromExile = true;
        }
    }
    [MethodRpc((uint)TownOfSushiRpc.OracleBlessNotify, SendImmediately = true)]
    public static void RpcOracleBlessNotify(PlayerControl oracle, PlayerControl source, PlayerControl target)
    {
        if (oracle.Data.Role is not OracleRole || !source.AmOwner && !oracle.AmOwner)
        {
            Logger<TownOfSushiPlugin>.Error("RpcOracleBlessNotify - Invalid oracle");
            return;
        }

        if (oracle.AmOwner)
        {
            Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Oracle));
            var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Oracle,
                $"<b>Your blessing has saved {target.Data.PlayerName} from getting guessed!</b>"),
                Color.white, spr: TOSRoleIcons.Oracle.LoadAsset());
            
            notif1.AdjustNotification();
        }
        else if (source.AmOwner)
        {
            Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Oracle));
            var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Oracle,
                $"<b>{target.Data.PlayerName}</color> survived due to being blessed by an Oracle!</b>"),
                Color.white, spr: TOSRoleIcons.Oracle.LoadAsset());
            
            notif1.AdjustNotification();
        }
    }
}