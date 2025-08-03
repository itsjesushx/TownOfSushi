using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using TownOfSushi.Events.TosEvents;
using TownOfUs.Modules.Components;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Roles.Impostor;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class JanitorRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable, ICrewVariant
{
    public string RoleName => "Janitor";
    public string RoleDescription => "Sanitize The Ship";
    public string RoleLongDescription => "Clean bodies to hide kills" + (OptionGroupSingleton<JanitorOptions>.Instance.CleanDelay == 0 ? string.Empty : "\n<b>You must stay next to the body while cleaning.</b>");
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<DetectiveRole>());
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorSupport;
    public DoomableType DoomHintType => DoomableType.Death;
    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = true,
        Icon = TosRoleIcons.Janitor,
        IntroSound = TosAudio.JanitorCleanSound,
    };
    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not JanitorRole || Player.HasDied() || !Player.AmOwner || MeetingHud.Instance || (!HudManager.Instance.UseButton.isActiveAndEnabled && !HudManager.Instance.PetButton.isActiveAndEnabled)) return;
        HudManager.Instance.KillButton.ToggleVisible(OptionGroupSingleton<JanitorOptions>.Instance.JanitorKill || (Player != null && Player.GetModifiers<BaseModifier>().Any(x => x is ICachedRole)) || (Player != null && MiscUtils.ImpAliveCount == 1));
    }

    [MethodRpc((uint)TownOfSushiRpc.CleanBody, LocalHandling = RpcLocalHandling.Before, SendImmediately = true)]
    public static void RpcCleanBody(PlayerControl player, byte bodyId)
    {
        if (player.Data.Role is not JanitorRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcCleanBody - Invalid Janitor");
            return;
        }

        var body = Helpers.GetBodyById(bodyId);

        if (body != null)
        {
            var TosAbilityEvent = new TosAbilityEvent(AbilityType.JanitorClean, player, body);
            MiraEventManager.InvokeEvent(TosAbilityEvent);

            Coroutines.Start(body.CoClean());
            Coroutines.Start(CrimeSceneComponent.CoClean(body));
        }
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return "The Janitor is an Impostor Support role that can clean dead bodies." + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Clean",
            "Clean a dead body, making it disapear and making it unreportable.",
            TosImpAssets.CleanButtonSprite)
    ];
}
