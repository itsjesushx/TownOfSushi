using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using UnityEngine;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using TownOfSushi.Events.TOSEvents;
using MiraAPI.Events;
using TownOfUs.Modules.Wiki;
using TownOfSushi.Modifiers;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using TownOfSushi.Modifiers.Impostor;
using TownOfSushi.Events;

namespace TownOfSushi.Roles.Impostor;

public sealed class WitchRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable
{
    public string RoleName => "Witch";
    public string RoleDescription => "Curse other players to kill them after a meeting.";
    public string RoleLongDescription => "Curse other players which kills them after the next meeting.";
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorKilling;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSImpAssets.CurseSprite,
        IntroSound = TOSAudio.WitchIntro,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    [MethodRpc((uint)TownOfSushiRpc.WiitchSetCursedPlayer, SendImmediately = true)]
    public static void RpcWiitchSetCursedPlayer(PlayerControl source, PlayerControl target)
    {
        // Prevent duplicate modifiers
        if (!target.HasModifier<WitchSpelledModifier>())
        {
            target.AddModifier(new WitchSpelledModifier(source.PlayerId));
        }

        var modifier = new WitchSpelledModifier(source.PlayerId);
        target.AddModifier(modifier);
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.WitchCurse, source, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    [MethodRpc((uint)TownOfSushiRpc.MurderCursedPlayer, SendImmediately = true)]
    public static void RpcMurderCursedPlayer(PlayerControl player, PlayerControl target)
    {
        if (player == null || player.Data.IsDead || target.Data.IsDead || target == null) return;

        if (player.Data.Role is not WitchRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcMurderCursedPlayer - Invalid Witch");
            return;
        }
        if (target != null && !target.Data.IsDead)
        {
            if (target == player || target.Data.IsDead || target.HasModifier<MedicShieldModifier>()
                || player.HasModifier<MonarchKnightedModifier>() && target.Data.Role is MonarchRole
                || target.HasModifier<FirstDeadShield>())
            {
                return;
            }

            player.RpcCustomMurder(target, createDeadBody: false, teleportMurderer: false, playKillSound: true);
            DeathHandlerModifier.RpcUpdateDeathHandler(target, "Spelled", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse, $"By {player.Data.PlayerName}", lockInfo: DeathHandlerOverride.SetTrue);

            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{TownOfSushiColors.ImpSoft.ToTextColor()}{target.Data.PlayerName}, has been successfully cursed. They are dead.</b></color>",
                Color.white, spr: TOSImpAssets.CurseSprite.LoadAsset());

            notif1.Text.SetOutlineThickness(0.35f);
            notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
        }
    }

    public string GetAdvancedDescription()
    {
        return "The Witch is an Impostor Killing role that can cast spells on players. After each meeting, any cursed player will die."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Spell",
            "After each meeting, any cursed player will die.",
            TOSImpAssets.CurseSprite)
    ];
}