using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using TownOfSushi.Events.TOSEvents;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class ConsigliereRole(IntPtr cppPtr)
    : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, ICrewVariant, IMysticClue
{

    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not ConsigliereRole || Player.HasDied() || !Player.AmOwner ||
            MeetingHud.Instance || (!HudManager.Instance.UseButton.isActiveAndEnabled &&
                                    !HudManager.Instance.PetButton.isActiveAndEnabled))
        {
            return;
        }

        HudManager.Instance.KillButton.ToggleVisible(
            OptionGroupSingleton<ConsigliereOptions>.Instance.ConsigliereKill ||
            (Player != null && Player.GetModifiers<BaseModifier>().Any(x => x is ICachedRole)) ||
            (Player != null && MiscUtils.ImpAliveCount == 1));
    }

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<AnalyzerRole>());
    public string RoleName => "Consigliere";
    public string RoleDescription => "Reveal player's roles";
    public string RoleLongDescription => "Examine players to find out their role";
    public MysticClueType MysticHintType => MysticClueType.Insight;
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorConcealing;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Consigliere,
        IntroSound = TOSAudio.AdministratorIntroSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }
    [MethodRpc((uint)TownOfSushiRpc.AddReveal, LocalHandling = RpcLocalHandling.Before, SendImmediately = true)]
    public static void RpcReveal(PlayerControl source, PlayerControl target)
    {
        var modifier = new ConsigliereRevealedModifier(source.PlayerId);
        target.AddModifier(modifier);

        source.SetKillTimer(source.GetKillCooldown());
        
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.ConsigliereReveal, source, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public static bool ConsigliereSeesRoleVisibilityFlag(PlayerControl player)
    {
        var consigKnows = PlayerControl.LocalPlayer.IsRole<ConsigliereRole>()
        && player.HasModifier<ConsigliereRevealedModifier>();

        var impsKnow = PlayerControl.LocalPlayer.IsImpostor()
        && !PlayerControl.LocalPlayer.IsRole<ConsigliereRole>()
        && OptionGroupSingleton<ConsigliereOptions>.Instance.ConsigliereShowRoleImp 
        && player.HasModifier<ConsigliereRevealedModifier>();

        return consigKnows || impsKnow;
    }

    public string GetAdvancedDescription()
    {
        return
            "The Consigliere is an Impostor Concealing role that can examine other players in order to see their role."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Reveal",
            "Reveal the role of any player of your choosing.",
            TOSImpAssets.MarkSprite)
    ];
}