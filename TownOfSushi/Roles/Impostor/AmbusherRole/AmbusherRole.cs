using System.Collections;
using System.Globalization;
using System.Text;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
using MiraAPI.Networking;
using MiraAPI.Patches.Stubs;
using PowerTools;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfSushi.Events;
using TownOfSushi.Modifiers;
using TownOfSushi.Modules.Anims;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class AmbusherRole(IntPtr cppPtr)
    : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable
{
    public string RoleName => "Ambusher";
    public string RoleDescription => "Kidnap Crewmates Into The Shadows";
    public string RoleLongDescription => "Pursue a player, then ambush the closest player to them.\nIf the player you ambush dies, then take their body with you.";
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorKilling;
    [HideFromIl2Cpp]
    public PlayerControl? Pursued { get; set; }

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Ambusher,
        CanUseVent = OptionGroupSingleton<AmbusherOptions>.Instance.CanVent
    };

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is an Impostor Killing role that can pursue a player, getting an arrow to them. They may ambush the closest player next to them. If they manage to kill the player, they will drag their body into the shadows, teleporting back with the {RoleName}." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Pursue",
            "Pursue a player to be able to ambush another player next to them at a later time.",
            TOSImpAssets.PursueSprite),
        new("Ambush",
        "Ambush the closest player to the pursued target to kill them.",
        TOSImpAssets.AmbushSprite)
    ];
    
    public void LobbyStart()
    {
        Clear();
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);

        if (Pursued)
        {
            stringB.Append(CultureInfo.InvariantCulture,
                $"\n<b>Pursuing:</b> {Pursued!.Data.Color.ToTextColor()}{Pursued.Data.PlayerName}</color>");
        }

        return stringB;
    }
    
    public override void OnVotingComplete()
    {
        RoleBehaviourStubs.OnVotingComplete(this);

        Clear();
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        CustomButtonSingleton<AmbusherAmbushButton>.Instance.SetActive(false, this);
    }
    
    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        Clear();
    }

    public void Clear()
    {
        Pursued = null;
    }
    public void CheckDeadPursued()
    {
        if (Pursued != null && Pursued.HasDied())
        {
            Pursued = null;
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.AmbushPlayer, SendImmediately = true)]
    public static void RpcAmbushPlayer(PlayerControl ambusher, PlayerControl target)
    {
        if (ambusher.Data.Role is not AmbusherRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcAmbushPlayer - Invalid ambusher");
            return;
        }
        ambusher.AddModifier<IndirectAttackerModifier>(false);
        
        var murderResultFlags = MurderResultFlags.Succeeded;

        var beforeMurderEvent = new BeforeMurderEvent(ambusher, target);
        MiraEventManager.InvokeEvent(beforeMurderEvent);

        if (beforeMurderEvent.IsCancelled)
        {
            murderResultFlags = MurderResultFlags.FailedError;
        }

        var murderResultFlags2 = MurderResultFlags.DecisionByHost | murderResultFlags;

        ambusher.CustomMurder(
            target,
            murderResultFlags2,
            true,
            true,
            false);
        Coroutines.Start(CoSetBodyReportable(ambusher, target));
    }
    
    private static IEnumerator CoSetBodyReportable(PlayerControl ambusher, PlayerControl target)
    {
        var ogPos = ambusher.transform.position;
        yield return new WaitForSeconds(0.01f);
        if (!target.HasDied())
        {
            yield break;
        }
        var bodyId = target.PlayerId;
        var waitDelegate =
            DelegateSupport.ConvertDelegate<Il2CppSystem.Func<bool>>(() => Helpers.GetBodyById(bodyId) != null);
        yield return new WaitUntil(waitDelegate);
        var body = Helpers.GetBodyById(bodyId);

        if (body != null)
        {
            DeathHandlerModifier.UpdateDeathHandler(target, "Ambushed", DeathEventHandlers.CurrentRound,
                DeathHandlerOverride.SetTrue, $"By {ambusher.Data.PlayerName}", lockInfo: DeathHandlerOverride.SetTrue);
            
            var bodyPos = body.transform.position;
            if (MeetingHud.Instance == null && ambusher.AmOwner)
            {
                ambusher.moveable = false;
                ambusher.MyPhysics.ResetMoveState();
                ambusher.NetTransform.SetPaused(true);
                bodyPos.y += 0.175f;
                bodyPos.z = bodyPos.y / 1000f;
                ambusher.RpcSetPos(bodyPos);
            }

            // Hide real player
            ambusher.Visible = false;
            foreach (var shield in ambusher.GetModifiers<BaseShieldModifier>())
            {
                shield.IsVisible = false;
                shield.SetVisible();
            }

            if (ambusher.HasModifier<FirstDeadShield>())
            {
                ambusher.GetModifier<FirstDeadShield>()!.IsVisible = false;
                ambusher.GetModifier<FirstDeadShield>()!.SetVisible();
            }
            var bodySprite = body.transform.GetChild(1).gameObject;
            var ambushAnim = AnimStore.SpawnFliplessAnimBody(ambusher, TOSAssets.AmbushPrefab.LoadAsset());
            ambushAnim.SetActive(false);
            
            yield return new WaitForSeconds(1.3f);
            
            ambushAnim.SetActive(true);
            var spriteAnim = ambushAnim.GetComponent<SpriteAnim>();
            var animationRend = ambushAnim.transform.GetChild(0).GetComponent<SpriteRenderer>();
            animationRend.material = bodySprite.GetComponent<SpriteRenderer>().material;
            body.gameObject.transform.position = new Vector3(bodyPos.x, bodyPos.y, bodyPos.z + 1000f);
            
            if (ambusher.HasModifier<GiantModifier>())
            {
                ambushAnim.transform.localScale *= 0.7f;
            }
            else if (ambusher.HasModifier<MiniModifier>())
            {
                ambushAnim.transform.localScale /= 0.7f;
            }
            
            if (target.HasModifier<MiniModifier>())
            {
                ambushAnim.transform.localScale *= 0.7f;
            }
            else if (target.HasModifier<GiantModifier>())
            {
                ambushAnim.transform.localScale /= 0.7f;
            }
            
            yield return new WaitForSeconds(spriteAnim.m_defaultAnim.length);
            
            ambushAnim.gameObject.Destroy();
            
            if (MeetingHud.Instance == null)
            {
                if (ambusher.AmOwner) ambusher.RpcSetPos(ogPos);
                var targetPos = ogPos + new Vector3(-0.05f, 0.175f, 0f);
                targetPos.z = targetPos.y / 1000f;
                body.transform.position = targetPos;
            }
            
            ambusher.Visible = true;
            ambusher.RemoveModifier<IndirectAttackerModifier>();

            foreach (var shield in ambusher.GetModifiers<BaseShieldModifier>())
            {
                shield.IsVisible = true;
                shield.SetVisible();
            }

            if (ambusher.HasModifier<FirstDeadShield>())
            {
                ambusher.GetModifier<FirstDeadShield>()!.IsVisible = true;
                ambusher.GetModifier<FirstDeadShield>()!.SetVisible();
            }

            if (!ambusher.AmOwner) 
                yield break;

            ambusher.moveable = true;
            ambusher.NetTransform.SetPaused(false);
        }
    }
}