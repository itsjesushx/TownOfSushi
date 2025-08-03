using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Modules.Anims;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options;
using TownOfSushi.Options.Roles.Impostor;

using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class EscapistRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable, ICrewVariant
{
    public string RoleName => "Escapist";
    public string RoleDescription => "Get Away From Kills With Ease";
    public string RoleLongDescription => "Teleport to get away from the scene of the crime";
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<TransporterRole>());
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorConcealing;
    public DoomableType DoomHintType => DoomableType.Protective;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TosRoleIcons.Escapist,
        IntroSound = TosAudio.TimeLordIntroSound,
        CanUseVent = OptionGroupSingleton<EscapistOptions>.Instance.CanVent,
    };

    public Vector2? MarkedLocation { get; set; }
    public GameObject? EscapeMark { get; set; }
    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        EscapeMark?.gameObject.Destroy();
    }

    [MethodRpc((uint)TownOfSushiRpc.Recall, SendImmediately = true)]
    public static void RpcRecall(PlayerControl player)
    {
        if (player.Data.Role is not EscapistRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcRecall - Invalid escapist");
            return;
        }
        var TosAbilityEvent = new TosAbilityEvent(AbilityType.EscapistRecall, player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }

    [MethodRpc((uint)TownOfSushiRpc.MarkLocation, SendImmediately = true)]
    public static void RpcMarkLocation(PlayerControl player, Vector2 pos)
    {
        if (player.Data.Role is not EscapistRole henry)
        {
            Logger<TownOfSushiPlugin>.Error("RpcRecall - Invalid escapist");
            return;
        }

        var TosAbilityEvent = new TosAbilityEvent(AbilityType.EscapistMark, player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);

        henry.MarkedLocation = pos;
        henry.EscapeMark = AnimStore.SpawnAnimAtPlayer(player, TosAssets.EscapistMarkPrefab.LoadAsset());
        henry.EscapeMark.transform.localPosition = new Vector3(pos.x, pos.y + 0.3f, 0.1f);
        henry.EscapeMark.SetActive(false);
    }
    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not EscapistRole || Player.HasDied()) return;
        if (EscapeMark != null)
        {
            EscapeMark.SetActive(PlayerControl.LocalPlayer.IsImpostor() || (PlayerControl.LocalPlayer.HasDied() && OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow));
            if (MarkedLocation == null)
            {
                EscapeMark.gameObject.Destroy();
                EscapeMark = null;
            }
        }
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return "The Escapist is an Impostor Concealing role that can mark a location and then recall (teleport) to that location."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Mark",
            "Mark a location for later use.",
            TosImpAssets.MarkSprite),
        new("Recall",
            "Recall to the marked location.",
            TosImpAssets.RecallSprite)
    ];
}
