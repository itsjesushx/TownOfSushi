using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using Reactor.Networking.Attributes;
using TownOfSushi.Events.TOSEvents;
using MiraAPI.Events;
using Reactor.Networking.Rpc;
using MiraAPI.Networking;
using TownOfSushi.Events;
using TownOfSushi.Modifiers;

namespace TownOfSushi.Roles.Impostor;

public sealed class WizardRole(IntPtr cppPtr)
    : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, ICrewVariant, IMysticClue
{
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<TransporterRole>());
    public string RoleName => "Wizard";
    public string RoleDescription => "Cast a spell on someone to kill them after a meeting";
    public string RoleLongDescription => "Cast a spell on someone to kill them after a meeting";
    public MysticClueType MysticHintType => MysticClueType.Trickster;
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorConcealing;
    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not WizardRole || Player.HasDied() || !Player.AmOwner ||
            MeetingHud.Instance || (!HudManager.Instance.UseButton.isActiveAndEnabled &&
                                    !HudManager.Instance.PetButton.isActiveAndEnabled))
        {
            return;
        }

        HudManager.Instance.KillButton.ToggleVisible(
            OptionGroupSingleton<WizardOptions>.Instance.WizardKill ||
            (Player != null && Player.GetModifiers<BaseModifier>().Any(x => x is ICachedRole)) ||
            (Player != null && MiscUtils.ImpAliveCount == 1));
    }

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = true,
        IntroSound = TOSAudio.WitchIntro,
        Icon = TOSImpAssets.SpellSprite
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    [MethodRpc((uint)TownOfSushiRpc.WizardCastSpell, LocalHandling = RpcLocalHandling.Before, SendImmediately = true)]
    public static void RpcCastSpell(PlayerControl source, PlayerControl target)
    {
        var modifier = new WizardSpelledModifier(source.PlayerId);
        target.AddModifier(modifier);
        
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.WizardCastSpell, source, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    [MethodRpc((uint)TownOfSushiRpc.WizardMurderSpelled, SendImmediately = true)]
    public static void RpcMurderSpelled(PlayerControl source, PlayerControl target)
    {
        if (target == null || source.HasDied() || target.IsProtected() || target.HasDied() || !target.HasModifier<WizardSpelledModifier>()) return;

        // IsProtected() doesn't cover fortify because interactions against fortified players depend on the intentions
        if (target.HasModifier<CrusaderFortifiedModifier>())
        {
            target.RpcCustomMurder(source, teleportMurderer: true);
            if (source.AmOwner)
            {
                var notif2 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Crusader,
                $"<b>{target.Data.PlayerName} is fortified. Your murder attempt backfired.</b>"),
                Color.white, spr: TOSRoleIcons.Crusader.LoadAsset());
                notif2.AdjustNotification();
            }
        }

        source.RpcCustomMurder(target, teleportMurderer: false, createDeadBody: false);
        DeathHandlerModifier.RpcUpdateDeathHandler(target, "Spelled", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse, $"By {source.Data.PlayerName}", lockInfo: DeathHandlerOverride.SetTrue);

        // Reset state
        source.SetKillTimer(source.GetKillCooldown());
        source.RemoveModifier<WizardSpelledModifier>();

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.WizardCurseKill, source, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is an Impostor Concealing role that can cast a spell on another player of their choosing. After the next meeting, the spelled player will die, unless they are protected by some means or if the Wizard is dead. "
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Spell",
            "Cast a spell on a player and let them die after next meeting",
            TOSImpAssets.SpellSprite)
    ];
}