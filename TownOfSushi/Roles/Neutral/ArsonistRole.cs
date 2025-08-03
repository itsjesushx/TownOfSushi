using System.Globalization;
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using TownOfSushi.Modifiers.Neutral;

using TownOfUs.Modules.Wiki;
using TownOfSushi.Utilities;
using UnityEngine;
using MiraAPI.Utilities;

namespace TownOfSushi.Roles.Neutral;

public sealed class ArsonistRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable
{
    public string RoleName => TOSLocale.Get(TOSNames.Arsonist, "Arsonist");
    public string RoleDescription => "Douse Players And Ignite The Light";

    public string RoleLongDescription => "Douse players in gasoline and ignite them to win with your abilities!";

    public Color RoleColor => TownOfSushiColors.Arsonist;
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
                stringB.Append(CultureInfo.InvariantCulture,
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
}