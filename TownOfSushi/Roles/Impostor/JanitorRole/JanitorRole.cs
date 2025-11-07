using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using TownOfSushi.Events.TOSEvents;
using TownOfUs.Modules.Components;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class JanitorRole(IntPtr cppPtr)
    : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, ICrewVariant, IMysticClue
{
    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not JanitorRole || Player.HasDied() || !Player.AmOwner ||
            MeetingHud.Instance || (!HudManager.Instance.UseButton.isActiveAndEnabled &&
                !HudManager.Instance.PetButton.isActiveAndEnabled))
        {
            return;
        }

        HudManager.Instance.KillButton.ToggleVisible(OptionGroupSingleton<JanitorOptions>.Instance.JanitorKill ||
         (Player != null && Player.GetModifiers<BaseModifier>().Any(x => x is ICachedRole)) || (Player != null && MiscUtils.ImpAliveCount == 1));
    }

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<InspectorRole>());
    public string RoleName => "Janitor";
    public string RoleDescription => "Sanitize The Ship";

    public string RoleLongDescription => "Clean bodies to hide kills" +
                                         (OptionGroupSingleton<JanitorOptions>.Instance.CleanDelay == 0
                                             ? string.Empty
                                             : "\n<b>You must stay next to the body while cleaning.</b>");
    public MysticClueType MysticHintType => MysticClueType.Death;

    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = true,
        Icon = TOSRoleIcons.Janitor,
        IntroSound = TOSAudio.JanitorCleanSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return "The Janitor is an Impostor Support role that can clean dead bodies." +
               MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Clean",
            "Clean a dead body, making it disapear and making it unreportable.",
            TOSImpAssets.CleanButtonSprite)
    ];

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
            var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.JanitorClean, player, body);
            MiraEventManager.InvokeEvent(TOSAbilityEvent);

            Coroutines.Start(body.CoClean());
            Coroutines.Start(CrimeSceneComponent.CoClean(body));
        }
    }
}