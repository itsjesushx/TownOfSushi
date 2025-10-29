using AmongUs.GameOptions;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using MiraAPI.Networking;
using TownOfSushi.Events.TOSEvents;
using MiraAPI.Events;

namespace TownOfSushi.Roles.Impostor;

public sealed class PoisonerRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Poisoner";
    public string RoleDescription => "Poison other players and kill them after some seconds.";
    public string RoleLongDescription => "Poison other players to let them die after a few seconds.";
    public MysticClueType MysticHintType => MysticClueType.Relentless;
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorKilling;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<PoisonerOptions>.Instance.CanUseVents,
        Icon = TOSImpAssets.PoisonSprite,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Shapeshifter),
    };
    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not PoisonerRole || Player.HasDied() || !Player.AmOwner || MeetingHud.Instance || (!HudManager.Instance.UseButton.isActiveAndEnabled && !HudManager.Instance.PetButton.isActiveAndEnabled)) return;
        HudManager.Instance.KillButton.ToggleVisible(false);
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }
    
    [MethodRpc((uint)TownOfSushiRpc.MurderPoisonedPlayer, SendImmediately = true)]
    public static void RpcMurderPoisoned(PlayerControl source, PlayerControl target)
    {
        if (source.Data.Role is not PoisonerRole || !target.HasModifier<PoisonerPoisonedModifier>())
        {
            Logger<TownOfSushiPlugin>.Error("RpcMurderPoisonedPlayer - Invalid Poisoner/Poisoned");
            return;
        }

        if (target != null && !target.Data.IsDead && source.AmOwner)
        {
            if (target.HasModifier<ArmoredModifier>() && target.TryGetModifier<ArmoredModifier>(out var armor) && armor.isActive)
            {
                var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.ImpSoft,
                $"<b>{target.Data.PlayerName} was protected because of their armour!.</b>"),
                Color.white, spr: TOSImpAssets.PoisonSprite.LoadAsset());

                
                notif1.AdjustNotification();
            }
            else
            {
                var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.ImpSoft,
                $"<b>{target.Data.PlayerName}, has been successfully poisoned. They are dead.</b>"),
                Color.white, spr: TOSImpAssets.PoisonSprite.LoadAsset());

                
                notif1.AdjustNotification();
            }
            source.RpcCustomMurder(target, teleportMurderer: false);
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.SetPoisonedPlayer, SendImmediately = true)]
    public static void RpcSetPoisoned(PlayerControl source, PlayerControl target)
    {
        var modifier = new PoisonerPoisonedModifier(source.PlayerId);
        target.AddModifier(modifier);
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.Poison, source, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public string GetAdvancedDescription()
    {
        return $"The Poisoner is an Impostor killing role that can poison other players, poisoned players will die after {OptionGroupSingleton<PoisonerOptions>.Instance.PoisonDelay} second(s)."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Poison",
            "A poisoned player will die after  second(s).",
            TOSImpAssets.PoisonSprite)
    ];
}