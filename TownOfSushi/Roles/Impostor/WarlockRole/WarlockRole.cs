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

public sealed class WarlockRole(IntPtr cppPtr)
    : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, ICrewVariant, IMysticClue
{
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<TransporterRole>());
    public string RoleName => "Warlock";
    public string RoleDescription => "Curse someone to make them kill someone else";
    public string RoleLongDescription => "Curse a player and force them to kill for you.";
    public MysticClueType MysticHintType => MysticClueType.Fearmonger;
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorConcealing;
    public PlayerControl? CursedPlayer { get; set; }
    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not WarlockRole || Player.HasDied() || !Player.AmOwner ||
            MeetingHud.Instance || (!HudManager.Instance.UseButton.isActiveAndEnabled &&
                                    !HudManager.Instance.PetButton.isActiveAndEnabled))
        {
            return;
        }

        HudManager.Instance.KillButton.ToggleVisible(
            OptionGroupSingleton<WarlockOptions>.Instance.WarlockKill ||
            (Player != null && Player.GetModifiers<BaseModifier>().Any(x => x is ICachedRole)) ||
            (Player != null && MiscUtils.ImpAliveCount == 1));
    }

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = true,
        IntroSound = TOSAudio.WitchIntro,
        Icon = TOSRoleIcons.Warlock
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    [MethodRpc((uint)TownOfSushiRpc.WarlockCurse, LocalHandling = RpcLocalHandling.Before, SendImmediately = true)]
    public static void RpcCurse(PlayerControl source, PlayerControl target)
    {
        if (source.Data.Role is not WarlockRole war)
        {
            return;
        }

        var existingCursed = PlayerControl.AllPlayerControls.ToArray()
            .FirstOrDefault(x => x.GetModifier<WarlockCursedModifier>()?.WarlockId == source.PlayerId);

        existingCursed?.RemoveModifier<WarlockCursedModifier>();

        var modifier = new WarlockCursedModifier(source.PlayerId);
        target.AddModifier(modifier);
        war.CursedPlayer = target;
        
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.WarlockCurse, source, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    [MethodRpc((uint)TownOfSushiRpc.WarlockCurseKill, SendImmediately = true)]
    public static void RpcCurseKill(PlayerControl source, PlayerControl target, PlayerControl player)
    {
        if (player.Data.Role is not WarlockRole warlock) return;
        if (target == null || target.IsProtected() || target.HasDied()) return;

        player.RpcAddModifier<IndirectAttackerModifier>(false);

        // IsProtected() doesn't cover fortify because interactions against fortified players depend on the intentions
        if (target.HasModifier<CrusaderFortifiedModifier>())
        {
            target.RpcCustomMurder(player, teleportMurderer: true);
            if (player.AmOwner)
            {
                var notif2 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Crusader,
                $"<b>{source.Data.PlayerName} has tried to kill {target.Data.PlayerName}, but they are fortified! Your murder attempt backfired.</b>"),
                Color.white, spr: TOSRoleIcons.Crusader.LoadAsset());
                
                notif2.AdjustNotification();
            }
        }

        player.RpcCustomMurder(target, teleportMurderer: false);
        DeathHandlerModifier.RpcUpdateDeathHandler(target, "Cursed", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse, $"By {player.Data.PlayerName}", lockInfo: DeathHandlerOverride.SetTrue);

        if (player.AmOwner)
        {
            var notif3 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.ImpSoft,
            $"<b>{source.Data.PlayerName} has killed {target.Data.PlayerName}.</b>"),
            Color.white, spr: TOSRoleIcons.Warlock.LoadAsset());
            notif3.AdjustNotification();
        }

        // Root the warlock
        if (OptionGroupSingleton<WarlockOptions>.Instance.RootTimeDuration > 0)
        {
            player.moveable = false;
            player.NetTransform.Halt();

            DestroyableSingleton<HudManager>.Instance.StartCoroutine(
                Effects.Lerp(OptionGroupSingleton<WarlockOptions>.Instance.RootTimeDuration, new Action<float>((p) =>
                {
                    if (p == 1f)
                    {
                        player.moveable = true;
                    }
                }))
            );
        }

        // Reset state
        player.SetKillTimer(player.GetKillCooldown());
        source.RemoveModifier<WarlockCursedModifier>();
        player.RpcRemoveModifier<IndirectAttackerModifier>();
        warlock.CursedPlayer = null;

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.WarlockCurseKill, source, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is an Impostor Concealing role that can curse another player of their choosing, once said player get close to another player, the {RoleName} can force the player to kill the nearest player. Doing so will make the {RoleName} unable to move for a set amount of time, and the cursed player will be unable to kill for a set amount of time."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Curse",
            "Cast a Curse on a player to force them to kill later",
            TOSImpAssets.WarlockCurseButton)
    ];
}