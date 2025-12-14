using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using TownOfSushi.Events.TOSEvents;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class BlackmailerRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public MysticClueType MysticHintType => MysticClueType.Insight;
    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not BlackmailerRole || Player.HasDied() || !Player.AmOwner ||
            MeetingHud.Instance || (!HudManager.Instance.UseButton.isActiveAndEnabled &&
                                    !HudManager.Instance.PetButton.isActiveAndEnabled))
        {
            return;
        }

        HudManager.Instance.KillButton.ToggleVisible(
            OptionGroupSingleton<BlackmailerOptions>.Instance.BlackmailerKill ||
            (Player != null && Player.GetModifiers<BaseModifier>().Any(x => x is ICachedRole)) ||
            (Player != null && MiscUtils.ImpAliveCount == 1));
    }
    public string RoleName => "Blackmailer";
    public string RoleDescription => "Silence players during meetings";
    public string RoleLongDescription => "Silence a player for the next meeting";
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = true,
        Icon = TOSRoleIcons.Blackmailer
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "The Blackmailer is an Impostor Support role that can stop a player from speaking (marked with <color=#2A1119>M</color>) in the next meeting" +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities =>
    [
        new("Blackmail",
            "Silence a player for the next meeting. They will be unable to speak." +
            "They also will not be able to vote until less or equal amount of people are alive than the blackmailer settings allow." +
            "The blackmail will be visible to other players only if the setting is toggled",
            TOSImpAssets.BlackmailSprite)
    ];

    [MethodRpc((uint)TownOfSushiRpc.Blackmail, LocalHandling = RpcLocalHandling.Before, SendImmediately = true)]
    public static void RpcBlackmail(PlayerControl source, PlayerControl target)
    {
        var existingBmed = PlayerControl.AllPlayerControls.ToArray()
            .FirstOrDefault(x => x.GetModifier<BlackmailedModifier>()?.BlackMailerId == source.PlayerId);

        existingBmed?.RemoveModifier<BlackmailedModifier>();

        var modifier = new BlackmailedModifier(source.PlayerId);
        target.AddModifier(modifier);
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.BlackmailerBlackmail, source, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }
}