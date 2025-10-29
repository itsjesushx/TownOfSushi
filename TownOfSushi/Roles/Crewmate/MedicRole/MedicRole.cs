
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Hud;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfSushi.Modules;


using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class MedicRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    private MeetingMenu meetingMenu;
    public override bool IsAffectedByComms => false;
    [HideFromIl2Cpp]
    public PlayerControl? Shielded { get; set; }

    public MysticClueType MysticHintType => MysticClueType.Protective;

    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not MedicRole)
        {
            return;
        }

        if (Shielded != null && Shielded.HasDied())
        {
            Clear();
        }
    }
    public string RoleName => "Medic";
    public string RoleDescription => "Create A Shield To Protect A Crewmate";
    public string RoleLongDescription => "Protect a crewmate with a shield";
    public Color RoleColor => TownOfSushiColors.Medic;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateProtective;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Scientist),
        Icon = TOSRoleIcons.Medic
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);

        if (Shielded != null)
        {
            stringB.Append(TownOfSushiPlugin.Culture,
                $"\n<b>Shielded: </b>{Color.white.ToTextColor()}{Shielded.Data.PlayerName}</color>");
        }

        return stringB;
    }

    public string GetAdvancedDescription()
    {
        return $"The {RoleName} is a Crewmate Protective role that can give a Shield to player."
               + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Shield",
            "Give a Shield to a player, protecting them from being killed by others",
            TOSCrewAssets.MedicSprite)
    ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (Player.AmOwner)
        {
            meetingMenu = new MeetingMenu(
                this,
                (_, _) => { },
                MeetingAbilityType.Click,
                TOSAssets.LighterSprite,
                null!,
                voteArea => { return Player.Data.IsDead || voteArea!.AmDead; },
                hoverColor: Color.white)
            {
                Position = new Vector3(1.1f, -0.18f, -3f)
            };
        }
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        if (Player.AmOwner)
        {
            meetingMenu.GenButtons(MeetingHud.Instance,
                Player.AmOwner && !Player.HasDied() && !Player.HasModifier<JailedModifier>());

            foreach (var button in meetingMenu.Buttons)
            {
                if (button.Value == null)
                {
                    continue;
                }

                button.Value.transform.localScale *= 0.8f;

                var player = MiscUtils.PlayerById(button.Key);

                if (player == null)
                {
                    continue;
                }

                var colorType = GetColorTypeForPlayer(player);

                var renderer = button.Value.GetComponent<SpriteRenderer>();

                if (renderer == null)
                {
                    continue;
                }

                renderer.sprite = colorType switch
                {
                    "lighter" => TOSAssets.LighterSprite.LoadAsset(),
                    _ => TOSAssets.DarkerSprite.LoadAsset()
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

    public void SetShieldedPlayer(PlayerControl? player)
    {
        Shielded?.RemoveModifier<MedicShieldModifier>();

        Shielded = player;

        Shielded?.AddModifier<MedicShieldModifier>(Player);
    }

    public void Report(byte deadPlayerId)
    {
        var areReportsEnabled = OptionGroupSingleton<MedicOptions>.Instance.ShowReports;

        if (!areReportsEnabled)
        {
            return;
        }

        var matches = GameHistory.KilledPlayers.Where(x => x.VictimId == deadPlayerId).ToArray();

        DeadPlayer? killer = null;

        if (matches.Length > 0)
        {
            killer = matches[0];
        }

        if (killer == null)
        {
            return;
        }

        // Logger<TownOfSushiPlugin>.Message($"CmdReportDeadBody");
        var br = new BodyReport
        {
            Killer = MiscUtils.PlayerById(killer.KillerId),
            Reporter = Player,
            Body = MiscUtils.PlayerById(killer.VictimId),
            KillAge = (float)(DateTime.UtcNow - killer.KillTime).TotalMilliseconds
        };

        var reportMsg = BodyReport.ParseMedicReport(br);

        if (string.IsNullOrWhiteSpace(reportMsg))
        {
            return;
        }

        var title = $"<color=#{TownOfSushiColors.Medic.ToHtmlStringRGBA()}>{RoleName} Report</color>";
        var reported = Player;
        if (br.Body != null)
        {
            reported = br.Body;
        }

        MiscUtils.AddFakeChat(reported.Data, title, reportMsg, false, true);
    }

    public static string GetColorTypeForPlayer(PlayerControl player)
    {
        var colors = new Dictionary<int, string>
        {
            { 0, "darker" }, // Red
            { 1, "darker" }, // Blue
            { 2, "darker" }, // Green
            { 3, "lighter" }, // Pink
            { 4, "lighter" }, // Orange
            { 5, "lighter" }, // Yellow
            { 6, "darker" }, // Black
            { 7, "lighter" }, // White
            { 8, "darker" }, // Purple
            { 9, "darker" }, // Brown
            { 10, "lighter" }, // Cyan
            { 11, "lighter" }, // Lime
            { 12, "darker" }, // Maroon
            { 13, "lighter" }, // Rose
            { 14, "lighter" }, // Banana
            { 15, "darker" }, // Gray
            { 16, "darker" }, // Tan
            { 17, "lighter" }, // Coral

            { 18, "darker" }, // Watermelon
            { 19, "darker" }, // Chocolate
            { 20, "lighter" }, // Sky Blue
            { 21, "lighter" }, // Beige
            { 22, "darker" }, // Magenta
            { 23, "lighter" }, // Sea Green
            { 24, "lighter" }, // Lilac
            { 25, "darker" }, // Olive
            { 26, "lighter" }, // Azure
            { 27, "darker" }, // Plum
            { 28, "darker" }, // Jungle
            { 29, "lighter" }, // Mint
            { 30, "lighter" }, // Chartreuse
            { 31, "darker" }, // Macau
            { 32, "lighter" }, // Tawny
            { 33, "darker" }, // Gold

            { 34, "lighter" }, // Snow
            { 35, "lighter" }, // Turquoise
            { 36, "lighter" }, // Nacho
            { 37, "darker" }, // Blood
            { 38, "darker" }, // Grass
            { 39, "lighter" }, // Mandarin
            { 40, "lighter" }, // Glass
            { 41, "darker" }, // Ash
            { 42, "darker" }, // Midnight
            { 43, "darker" }, // Steel
            { 44, "lighter" }, // Silver
            { 45, "lighter" }, // Shimmer
            { 46, "darker" }, // Crimson
            { 47, "darker" }, // Charcoal
            { 48, "darker" }, // Violet
            { 49, "darker" }, // Denim
            { 50, "lighter" }, // Cotton Candy

            { 51, "lighter" }, // Rainbow
            { 52, "darker" }, // Tamarind
            { 53, "darker" }, // Army
            { 54, "lighter" }, // Lavender
            { 55, "darker" }, // Nougat
            { 56, "lighter" }, // Peach
            { 57, "darker" }, // Wasabi
            { 58, "lighter" }, // Hot Pink
            { 59, "darker" }, // Petrol
            { 60, "lighter" }, // Lemon
            { 61, "lighter" }, // Teal
            { 62, "darker" }, // Blurple
            { 63, "lighter" }, // Sunrise
            { 64, "lighter" }, // Ice
            { 65, "darker" }, // Fuchsia
            { 66, "darker" }, // Royal Green
            { 67, "lighter" }, // Slime
            { 68, "darker" }, // Navy
            { 69, "darker" }, // Darkness
            { 70, "lighter" }, // Ocean
            { 71, "lighter" }, // Sundown
            { 72, "darker" }, // Cherry
            { 73, "darker" }, // Void
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
        CustomButtonSingleton<MedicShieldButton>.Instance.CanChangeTarget =
            OptionGroupSingleton<MedicOptions>.Instance.ChangeTarget;
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
        {
            Coroutines.Start(MiscUtils.CoFlash(new Color(0f, 0.5f, 0f, 1f)));
        }

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
}