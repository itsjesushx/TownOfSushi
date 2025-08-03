using System.Globalization;
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfSushi.Buttons.Crewmate;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Modules;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Roles.Crewmate;

using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class MedicRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable
{
    public string RoleName => "Medic";
    public string RoleDescription => "Create A Shield To Protect A Crewmate";
    public string RoleLongDescription => "Protect a crewmate with a shield";
    public Color RoleColor => TownOfSushiColors.Medic;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateProtective;
    public DoomableType DoomHintType => DoomableType.Protective;
    public override bool IsAffectedByComms => false;
    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Scientist),
        Icon = TosRoleIcons.Medic,
    };

    public PlayerControl? Shielded { get; set; }

    private MeetingMenu meetingMenu;

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (Player.AmOwner)
        {
            meetingMenu = new MeetingMenu(
                this,
                (PlayerVoteArea _, MeetingHud _) => { },
                MeetingAbilityType.Click,
                TosAssets.LighterSprite,
                null!,
                (PlayerVoteArea voteArea) => { return Player.Data.IsDead || voteArea!.AmDead; },
                hoverColor: Color.white)
                {
                    Position = new Vector3(1.1f, -0.18f, -3f),
                };
        }
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        if (Player.AmOwner)
        {
            meetingMenu.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && !Player.HasModifier<JailedModifier>());

            foreach (var button in meetingMenu.Buttons)
            {
                if (button.Value == null) continue;

                button.Value.transform.localScale *= 0.8f;

                var player = MiscUtils.PlayerById(button.Key);

                if (player == null ) continue;

                var colorType = GetColorTypeForPlayer(player);

                var renderer = button.Value.GetComponent<SpriteRenderer>();

                if (renderer == null) continue;

                renderer.sprite = colorType switch
                {
                    "lighter" => TosAssets.LighterSprite.LoadAsset(),
                    _ => TosAssets.DarkerSprite.LoadAsset(),
                };
            }
        }
    }

    public override void OnVotingComplete()
    {
        RoleBehaviourStubs.OnVotingComplete(this);

        if (Player.AmOwner)
        {
            meetingMenu.HideButtons();
        }
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);

        if (Shielded != null)
        {
            stringB.Append(CultureInfo.InvariantCulture, $"\n<b>Shielded: </b>{Color.white.ToTextColor()}{Shielded.Data.PlayerName}</color>");
        }

        return stringB;
    }

    public void Clear()
    {
        SetShieldedPlayer(null);
    }

    public override void OnDeath(DeathReason reason)
    {
        RoleBehaviourStubs.OnDeath(this, reason);

        Clear();
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        Clear();

        if (Player.AmOwner)
        {
            meetingMenu?.Dispose();
            meetingMenu = null!;
        }
    }

    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not MedicRole) return;
        if (Shielded != null && Shielded.HasDied())
            Clear();
    }

    public void SetShieldedPlayer(PlayerControl? player)
    {
        Shielded?.RemoveModifier<MedicShieldModifier>();

        Shielded = player;

        Shielded?.AddModifier<MedicShieldModifier>(Player);
    }

    public void Report(byte deadPlayerId)
    {
        var areReportsEnabled = OptionGroupSingleton<MedicOptions>.Instance.ShowReports;

        if (!areReportsEnabled) return;

        var matches = GameHistory.KilledPlayers.Where(x => x.VictimId == deadPlayerId).ToArray();

        DeadPlayer? killer = null;

        if (matches.Length > 0)
            killer = matches[0];

        if (killer == null)
            return;

        // Logger<TownOfSushiPlugin>.Message($"CmdReportDeadBody");
        var br = new BodyReport
        {
            Killer = MiscUtils.PlayerById(killer.KillerId),
            Reporter = Player,
            Body = MiscUtils.PlayerById(killer.VictimId),
            KillAge = (float)(DateTime.UtcNow - killer.KillTime).TotalMilliseconds,
        };

        var reportMsg = BodyReport.ParseMedicReport(br);

        if (string.IsNullOrWhiteSpace(reportMsg))
            return;

        var title = $"<color=#{TownOfSushiColors.Medic.ToHtmlStringRGBA()}>Medic Report</color>";
        var reported = Player;
        if (br.Body != null) reported = br.Body;
        MiscUtils.AddFakeChat(reported.Data, title, reportMsg, false, true);
    }

    public static string GetColorTypeForPlayer(PlayerControl player)
    {
        var colors = new Dictionary<int, string>
        {
            { 0, "darker" }, // red
            { 1, "darker" }, // blue
            { 2, "darker" }, // green
            { 3, "lighter" }, // pink
            { 4, "lighter" }, // orange
            { 5, "lighter" }, // yellow
            { 6, "darker" }, // black
            { 7, "lighter" }, // white
            { 8, "darker" }, // purple
            { 9, "darker" }, // brown
            { 10, "lighter" }, // cyan
            { 11, "lighter" }, // lime
            { 12, "darker" }, // maroon
            { 13, "lighter" }, // rose
            { 14, "lighter" }, // banana
            { 15, "darker" }, // gray
            { 16, "darker" }, // tan
            { 17, "lighter" }, // coral
            { 18, "darker" }, // watermelon
            { 19, "darker" }, // chocolate
            { 20, "lighter" }, // sky blue
            { 21, "lighter" }, // beige
            { 22, "darker" }, // magenta
            { 23, "lighter" }, // turquoise/Sea Green
            { 24, "lighter" }, // lilac
            { 25, "darker" }, // olive
            { 26, "lighter" }, // azure
            { 27, "darker" }, // plum
            { 28, "darker" }, // jungle
            { 29, "lighter" }, // mint
            { 30, "lighter" }, // chartreuse
            { 31, "darker" }, // macau
            { 32, "lighter" }, // gold
            { 33, "darker" }, // tawny
            { 34, "lighter" }, // rainbow
        };

        var typeOfColor = colors[player.Data.DefaultOutfit.ColorId];

        return typeOfColor;
    }

    public static void DangerAnim()
    {
        Coroutines.Start(MiscUtils.CoFlash(new Color(0f, 0.5f, 0f, 1f)));
    }
    public static void OnRoundStart()
    {
        CustomButtonSingleton<MedicShieldButton>.Instance.CanChangeTarget = OptionGroupSingleton<MedicOptions>.Instance.ChangeTarget;
    }

    [MethodRpc((uint)TownOfSushiRpc.MedicShield, SendImmediately = true)]
    public static void RpcMedicShield(PlayerControl medic, PlayerControl target)
    {
        if (medic.Data.Role is not MedicRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcMedicShield - Invalid medic");
            return;
        }

        var role = medic.GetRole<MedicRole>();

        role?.SetShieldedPlayer(target);
    }

    [MethodRpc((uint)TownOfSushiRpc.ClearMedicShield, SendImmediately = true)]
    public static void RpcClearMedicShield(PlayerControl medic)
    {
        ClearMedicShield(medic);
    }
    public static void ClearMedicShield(PlayerControl medic)
    {
        if (medic.Data.Role is not MedicRole)
        {
            Logger<TownOfSushiPlugin>.Error("ClearMedicShield - Invalid medic");
            return;
        }

        var role = medic.GetRole<MedicRole>();

        role?.SetShieldedPlayer(null);
    }

    [MethodRpc((uint)TownOfSushiRpc.MedicShieldAttacked, SendImmediately = true)]
    public static void RpcMedicShieldAttacked(PlayerControl medic, PlayerControl source, PlayerControl shielded)
    {
        if (medic.Data.Role is not MedicRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcMedicShieldAttacked - Invalid medic");
            return;
        }

        if (PlayerControl.LocalPlayer.PlayerId == source.PlayerId)
            Coroutines.Start(MiscUtils.CoFlash(new Color(0f, 0.5f, 0f, 1f)));

        var shieldNotify = OptionGroupSingleton<MedicOptions>.Instance.WhoGetsNotification;

        if (shielded.AmOwner && shieldNotify == MedicOption.Shielded)
        {
            DangerAnim();
        }

        if (medic.AmOwner && shieldNotify == MedicOption.Medic)
        {
            DangerAnim();
        }

        if (source.AmOwner)
        {
            DangerAnim();
        }

        if (shieldNotify == MedicOption.Everyone && !source.AmOwner)
        {
            DangerAnim();
        }

        var shieldBreaks = OptionGroupSingleton<MedicOptions>.Instance.ShieldBreaks;

        if (shieldBreaks)
        {
            var role = medic.GetRole<MedicRole>();
            role?.SetShieldedPlayer(null);
        }
    }

    public string GetAdvancedDescription()
    {
        return "The Medic is a Crewmate Protective role that can give a Shield to player."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new ("Shield",
            "Give a Shield to a player, protecting them from being killed by others",
            TosCrewAssets.MedicSprite)    
    ];
}
