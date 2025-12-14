using System.Collections;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.Hud;
using MiraAPI.Modifiers.Types;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modules;
using TownOfSushi.Modules.Anims;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class RetributionistRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public override bool IsAffectedByComms => false;
    public string RoleName => "Retributionist";
    public string RoleDescription => "Revive dead players";
    public MysticClueType MysticHintType => MysticClueType.Death;
    public string RoleLongDescription => "Revive dead crewmates in groups";
    public Color RoleColor => TownOfSushiColors.Retributionist;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TOSAudio.RetributionistReviveSound,
        Icon = TOSRoleIcons.Retributionist
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "The Retributionist is a Crewmate Protective role can revive dead players in groups. However, their location and the revived players' locations will be revealed to all Impostors." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Revive",
            "Revive a group of dead bodies near you. You will be frozen during the revival and you will be unable to move until the revival is complete." +
            " Impostors will also have an arrow pointing towards you during the revival, so be cautious.",
            TOSCrewAssets.ReviveSprite)
    ];

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        Logger<TownOfSushiPlugin>.Error($"RetributionistRole.OnMeetingStart");

        ClearArrows();
    }

    public override void OnVotingComplete()
    {
        RoleBehaviourStubs.OnVotingComplete(this);

        CustomButtonSingleton<RetributionistReviveButton>.Instance.RevivedInRound = false;
    }

    public override void OnDeath(DeathReason reason)
    {
        RoleBehaviourStubs.OnDeath(this, reason);

        ClearArrows();
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        ClearArrows();
    }

    [HideFromIl2Cpp]
    public static void ClearArrows()
    {
        Logger<TownOfSushiPlugin>.Error($"RetributionistRole.ClearArrows");

        if (PlayerControl.LocalPlayer.IsImpostor() || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKilling))
        {
            Logger<TownOfSushiPlugin>.Error($"RetributionistRole.ClearArrows BadGuys Only");

            foreach (var playerWithArrow in ModifierUtils.GetPlayersWithModifier<RetributionistArrowModifier>())
            {
                playerWithArrow.RemoveModifier<RetributionistArrowModifier>();
            }
        }
    }

    [HideFromIl2Cpp]
    public IEnumerator CoRevivePlayer(PlayerControl dead)
    {
        var roleWhenAlive = dead.GetRoleWhenAlive();

        //if (roleWhenAlive == null)
        //{
        //    Logger<TownOfSushiPlugin>.Error($"CoRevivePlayer - Dead player {dead.PlayerId} does not have a role when alive, cannot revive");
        //    yield break; // cannot revive if no role when alive
        //}

        Player.moveable = false;
        Player.NetTransform.Halt();

        var body = FindObjectsOfType<DeadBody>()
            .FirstOrDefault(b => b.ParentId == dead.PlayerId);
        var position = new Vector2(Player.transform.localPosition.x, Player.transform.localPosition.y);

        if (body != null)
        {
            position = new Vector2(body.transform.localPosition.x, body.transform.localPosition.y + 0.3636f);
            if (OptionGroupSingleton<RetributionistOptions>.Instance.HideAtBeginningOfRevive)
            {
                Destroy(body.gameObject);
            }
        }

        yield return new WaitForSeconds(OptionGroupSingleton<RetributionistOptions>.Instance.ReviveDuration);

        if (!MeetingHud.Instance && !Player.HasDied())
        {
            GameHistory.ClearMurder(dead);

            dead.Revive();

            dead.transform.position = new Vector2(position.x, position.y);
            if (dead.AmOwner)
            {
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(new Vector2(position.x, position.y));
            }

            if (ModCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == dead.PlayerId)
            {
                ModCompatibility.ChangeFloor(dead.transform.position.y > -7);
            }

            if (dead.AmOwner && !dead.HasModifier<LoverModifier>())
            {
                HudManager.Instance.Chat.gameObject.SetActive(false);
            }

            // return player from ghost role back to what they were when alive
            dead.ChangeRole((ushort)roleWhenAlive!.Role, false);

            if (dead.Data.Role is IAnimated animated)
            {
                animated.IsVisible = true;
                animated.SetVisible();
            }

            foreach (var button in CustomButtonManager.Buttons.Where(x => x.Enabled(dead.Data.Role))
                         .OfType<IAnimated>())
            {
                button.IsVisible = true;
                button.SetVisible();
            }

            foreach (var modifier in dead.GetModifiers<GameModifier>().Where(x => x is IAnimated))
            {
                var animatedMod = modifier as IAnimated;
                if (animatedMod != null)
                {
                    animatedMod.IsVisible = true;
                    animatedMod.SetVisible();
                }
            }

            dead.RemainingEmergencies = 0;

            Player.RemainingEmergencies = 0;

            body = FindObjectsOfType<DeadBody>()
                .FirstOrDefault(b => b.ParentId == dead.PlayerId);
            if (!OptionGroupSingleton<RetributionistOptions>.Instance.HideAtBeginningOfRevive && body != null)
            {
                Destroy(body.gameObject);
            }

            if (PlayerControl.LocalPlayer.IsImpostor() || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKilling))
            {
                if (Player.HasModifier<RetributionistArrowModifier>())
                {
                    Player.RemoveModifier<RetributionistArrowModifier>();
                }

                if (!dead.HasModifier<RetributionistArrowModifier>() && dead != PlayerControl.LocalPlayer)
                {
                    dead.AddModifier<RetributionistArrowModifier>(PlayerControl.LocalPlayer, Color.white);
                }
            }
        }

        Player.moveable = true;
    }

    [MethodRpc((uint)TownOfSushiRpc.RetributionistRevive, SendImmediately = true)]
    public static void RpcRevive(PlayerControl alt, PlayerControl target)
    {
        if (alt.Data.Role is not RetributionistRole role)
        {
            Logger<TownOfSushiPlugin>.Error("RpcRevive - Invalid retributionist");
            return;
        }

        if (PlayerControl.LocalPlayer.IsImpostor() || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKilling))
        {
            Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Retributionist));

            if (!alt.HasModifier<RetributionistArrowModifier>())
            {
                alt.AddModifier<RetributionistArrowModifier>(PlayerControl.LocalPlayer, TownOfSushiColors.Impostor);
            }
        }

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.RetributionistRevive, alt, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
        Coroutines.Start(role.CoRevivePlayer(target));
    }
}