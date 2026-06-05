
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using Reactor.Utilities;
using TownOfSushi.Modifiers.Game;
using UnityEngine;
using MiraAPI.Hud;
using Reactor.Networking.Attributes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class SheriffRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITOSCrewRole, IWikiDiscoverable, IMysticClue
{
    public override bool IsAffectedByComms => false;
    public bool HasMisfired { get; set; }
    public string RoleName => "Sheriff";
    public string RoleDescription => "Kill evildoers by any means possible.";
    public string RoleLongDescription => "Shoot killers mid round to get rid of them.";
    public MysticClueType MysticHintType => MysticClueType.Relentless;
    public Color RoleColor => TownOfSushiColors.Sheriff;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateKilling;
    public Factions Faction => Factions.Crewmate;
    public bool IsPowerCrew => !HasMisfired;
    // Always disable end game checks if the Sheriff hasn't misfired

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TownOfSushiAssets.Sheriff,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Impostor)
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITOSRole.SetNewTabText(this);
        var missType = OptionGroupSingleton<SheriffOptions>.Instance.MisfireType;

        if (CustomButtonSingleton<SheriffShootButton>.Instance.FailedShot)
        {
            stringB.AppendLine(TownOfSushiPlugin.Culture, $"<b>You can no longer shoot.</b>");
        }
        else
        {
            switch (missType)
            {
                case MisfireOptions.Both:
                    stringB.AppendLine(TownOfSushiPlugin.Culture, $"<b>Misfiring kills you and your target.</b>");
                    break;
                case MisfireOptions.Sheriff:
                    stringB.AppendLine(TownOfSushiPlugin.Culture, $"<b>Misfiring will lead to suicide.</b>");
                    break;
                case MisfireOptions.Target:
                    stringB.AppendLine(TownOfSushiPlugin.Culture,
                        $"<b>Misfiring will lead to your target's death,\nat the cost of your ability to kill.</b>");
                    break;
                default:
                    stringB.AppendLine(TownOfSushiPlugin.Culture,
                        $"<b>Misfiring will prevent you from shooting again.</b>");
                    break;
            }
        }

        if (PlayerControl.LocalPlayer.TryGetModifier<AllianceGameModifier>(out var allyMod) && !allyMod.GetsPunished)
        {
            stringB.AppendLine(TownOfSushiPlugin.Culture, $"<b>You may shoot without repercussions.</b>");
        }

        return stringB;
    }

    public static void OnRoundStart()
    {
        CustomButtonSingleton<SheriffShootButton>.Instance.Usable = true;
    }

    public string GetAdvancedDescription()
    {
        return
            "The Sheriff is a Crewmate Killing role that can kill players in rounds." +
            "If the person they shoot is a killer, the other player will die. If not, the Sheriff dies. "
            + Utils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Shoot",
            "Shoot a player to kill them, misfiring if they aren't a killer or one of the other selected shootable factions",
            TownOfSushiAssets.SheriffShootSprite)
    ];
    
    [MethodRpc((uint)TownOfSushiRpc.SheriffMisfire, SendImmediately = true)]
    public static void RpcSheriffMisfire(PlayerControl Sheriff)
    {
        if (Sheriff.Data.Role is not SheriffRole role)
        {
            Logger<TownOfSushiPlugin>.Error("RpcSheriffMisfire - Invalid Sheriff");
            return;
        }

        role.HasMisfired = true;

        if (GameHistory.PlayerStats.TryGetValue(Sheriff.PlayerId, out var stats))
        {
            stats.IncorrectKills += 1;
        }
    }
}