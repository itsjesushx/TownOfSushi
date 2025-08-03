using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Extensions;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Roles.Crewmate;

using TownOfSushi.Utilities;
using TownOfSushi.Utilities.Appearances;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class OracleRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable
{
    public string RoleName => "Oracle";
    public string RoleDescription => "Get Other Player's To Confess Their Sins";
    public string RoleLongDescription => "Get another player to confess on your passing";
    public Color RoleColor => TownOfSushiColors.Oracle;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateProtective;
    public DoomableType DoomHintType => DoomableType.Insight;
    public override bool IsAffectedByComms => false;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TosRoleIcons.Oracle,
        IntroSound = TosAudio.GuardianAngelSound,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public override void OnDeath(DeathReason reason)
    {
        RoleBehaviourStubs.OnDeath(this, reason);

        RpcOracleConfess(Player);
    }

    public void ReportOnConfession()
    {
        if (!Player.AmOwner) return;

        var confessing = ModifierUtils.GetPlayersWithModifier<OracleConfessModifier>([HideFromIl2Cpp] (x) => x.Oracle == Player).FirstOrDefault();

        if (confessing == null) return;

        var report = BuildReport(confessing);

        var title = $"<color=#{TownOfSushiColors.Oracle.ToHtmlStringRGBA()}>Oracle Confession</color>";
        MiscUtils.AddFakeChat(confessing.Data, title, report, false, true);
    }

    public static string BuildReport(PlayerControl player)
    {
        if (player.HasDied())
            return "Your confessor failed to survive so you received no confession";

        var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x != PlayerControl.LocalPlayer && x != player).ToList();
        if (allPlayers.Count < 2)
            return "Too few people alive to receive a confessional";

        var options = OptionGroupSingleton<OracleOptions>.Instance;

        var evilPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() &&
            (x.IsImpostor() ||
            (x.Is(RoleAlignment.NeutralKilling) && options.ShowNeutralKillingAsEvil) ||
            (x.Is(RoleAlignment.NeutralEvil) && options.ShowNeutralEvilAsEvil) ||
            (x.Is(RoleAlignment.NeutralBenign) && options.ShowNeutralBenignAsEvil))).ToList();

        if (evilPlayers.Count == 0)
            return $"{player.GetDefaultAppearance().PlayerName} confesses to knowing that there are no more evil players!";

        allPlayers.Shuffle();
        evilPlayers.Shuffle();
        var secondPlayer = allPlayers[0];
        var firstTwoEvil = evilPlayers.Any(plr => plr == player || plr == secondPlayer);

        if (firstTwoEvil)
        {
            var thirdPlayer = allPlayers[1];

            return $"{player.GetDefaultAppearance().PlayerName} confesses to knowing that they, {secondPlayer.GetDefaultAppearance().PlayerName} and/or {thirdPlayer.GetDefaultAppearance().PlayerName} is evil!";
        }
        else
        {
            var thirdPlayer = evilPlayers[0];

            return $"{player.GetDefaultAppearance().PlayerName} confesses to knowing that they, {secondPlayer.GetDefaultAppearance().PlayerName} and/or {thirdPlayer.GetDefaultAppearance().PlayerName} is evil!";
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
        {
            // Logger<TownOfSushiPlugin>.Message($"RpcOracleBless exiled '{exiled.Data.PlayerName}' SavedFromExile");
            mod.SavedFromExile = true;
        }
    }
    public string GetAdvancedDescription()
    {
        return
            $"The Oracle is a Crewmate Protective role that can get another player to confess (revealing their faction with {OptionGroupSingleton<OracleOptions>.Instance.RevealAccuracyPercentage}% accuracy if the Oracle dies) or can protect a player from meeting abilities."
               + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Bless",
            $"Blessing a player prevents any harm from being done to them in the meeting.",
            TosCrewAssets.BlessSprite),
        new("Confess",
            $"Make a player confess in a meeting, giving a vision of 3 possible evils (including the confessor), and also reveal their faction to everyone with {OptionGroupSingleton<OracleOptions>.Instance.RevealAccuracyPercentage}% accuracy when the Oracle dies.",
            TosCrewAssets.ConfessSprite),
    ];
}
