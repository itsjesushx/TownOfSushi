using AmongUs.GameOptions;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using UnityEngine;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using MiraAPI.Modifiers;
using TownOfUs.Modules.Wiki;
using MiraAPI.GameOptions;
using TownOfSushi.Utilities;
using TownOfSushi.Modifiers.Impostor;
using TownOfSushi.Options.Roles.Impostor;

namespace TownOfSushi.Roles.Impostor;

public sealed class ViperRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable
{
    public string RoleName => "Viper";
    public string RoleDescription => "Poison other players and kill them after some seconds.";
    public string RoleLongDescription => "Poison other players to let them die after a few seconds.";
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorKilling;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<ViperOptions>.Instance.CanUseVents,
        Icon = TOSImpAssets.PoisonSprite,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Shapeshifter),
    };
    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not ViperRole || Player.HasDied() || !Player.AmOwner || MeetingHud.Instance || (!HudManager.Instance.UseButton.isActiveAndEnabled && !HudManager.Instance.PetButton.isActiveAndEnabled)) return;
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
        if (source.Data.Role is not ViperRole || !target.HasModifier<PoisonModifier>())
        {
            Logger<TownOfSushiPlugin>.Error("RpcMurderPoisonedPlayer - Invalid Viper/Poisoned");
            return;
        }

        if (target != null && !target.Data.IsDead)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{TownOfSushiColors.ImpSoft.ToTextColor()}{target.Data.PlayerName}, has been successfully poisoned. They are dead.</b></color>",
                Color.white, spr: TOSImpAssets.PoisonSprite.LoadAsset());

            notif1.Text.SetOutlineThickness(0.35f);
            notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
            source.RpcCustomMurder(target, teleportMurderer: false);
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.SetPoisonedPlayer, SendImmediately = true)]
    public static void RpcSetPoisoned(byte vPlayer, byte playerId)
    {
        var player = MiscUtils.PlayerById(vPlayer);
        if (player == null || player.Data.IsDead) return;

        if (player.Data.Role is not ViperRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcSetPoisoned - Invalid Viper");
            return;
        }

        var target = MiscUtils.PlayerById(playerId);

        if (target == null || target.Data.IsDead) return;

        target.RpcAddModifier<PoisonModifier>();
    }

    public string GetAdvancedDescription()
    {
        return $"The Viper is an Impostor killing role that can poison other players, poisoned players will die after {OptionGroupSingleton<ViperOptions>.Instance.PoisonDelay} second(s)."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Poison",
            "A poisoned player will die after  second(s).",
            TOSImpAssets.PoisonSprite)
    ];
}