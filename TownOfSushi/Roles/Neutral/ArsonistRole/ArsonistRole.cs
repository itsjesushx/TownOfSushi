
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;

using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class ArsonistRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Arsonist";
    public string RoleDescription => "Douse and ignite everyone to win";
    public string RoleLongDescription => "Douse players in gasoline and ignite them to win with your abilities!";

    public Color RoleColor => TownOfSushiColors.Arsonist;
    public MysticClueType MysticHintType => MysticClueType.Fearmonger;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralEvil;
    public bool Wins { get; set; }
    public bool DousedEveryone
    {
        get
        {
            var aliveOthers = PlayerControl.AllPlayerControls.ToArray().Where(p => !p.HasDied()
            && p.PlayerId != Player.PlayerId);
            int dousedCount = aliveOthers.Count(p => p.GetModifier<ArsonistDousedModifier>()?.ArsonistId ==
            Player.PlayerId);
            return dousedCount == aliveOthers.Count();
        }
    }

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TOSAudio.ArsoIgniteSound,
        Icon = TOSRoleIcons.Pyromaniac,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);

        var allDoused = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && x.GetModifier<ArsonistDousedModifier>()?.ArsonistId == Player.PlayerId);

        if (allDoused.Any())
        {
            stringB.Append("\n<b>Players Doused:</b>");
            foreach (var plr in allDoused)
            {
                stringB.Append(TownOfSushiPlugin.Culture,
                    $"\n{Color.white.ToTextColor()}{plr.Data.PlayerName}</color>");
            }
        }

        return stringB;
    }

    public bool WinConditionMet()
    {
        var roleCount = CustomRoleUtils.GetActiveRolesOfType<ArsonistRole>().Count(x => !x.Player.HasDied());

        if (MiscUtils.KillersAliveCount > roleCount)
        {
            return false;
        }

        return roleCount >= Helpers.GetAlivePlayers().Count - roleCount;
    }

    public string GetAdvancedDescription()
    {
        return $"The {RoleName} is a Neutral Evil role that wins by spreading gasoline to all players. Once everyone is doused, the {RoleName} can ignite and kill everyone to win."
        + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Douse",
            "Douse a player in gasoline",
            TOSNeutAssets.DouseButtonSprite),
        new("Ignite",
            "Ignite all doused players to win alone.",
            TOSNeutAssets.IgniteButtonSprite)
    ];

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }
    public override bool CanUse(IUsable usable)
    {
        if (!GameManager.Instance.LogicUsables.CanUse(usable, Player))
        {
            return false;
        }

        var console = usable.TryCast<Console>()!;
        return console == null || console.AllowImpostor;
    }
}