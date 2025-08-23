using System.Text;
using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.Hud;
using MiraAPI.Networking;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modules;
using TownOfSushi.Options;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class ThiefRole(IntPtr cppPtr)
    : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, ICrewVariant
{
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<VigilanteRole>());
    public string RoleName => "Thief";
    public string RoleDescription => "Murder an evildoer to gain their role.";
    public string RoleLongDescription => "Murder evildoers but not crewmates to get a new role";
    public Color RoleColor => TownOfSushiColors.Thief;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralBenign;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<ThiefOptions>.Instance.CanVent,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Phantom),
        Icon = TOSRoleIcons.Thief,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    public bool HasImpostorVision => OptionGroupSingleton<ThiefOptions>.Instance.HasImpostorVision;

    public bool WinConditionMet()
    {
        return false;
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "The Thief is a Neutral Benign role that can kill evildoers to gain their role. If they kill a crewmate, they will die instead."
            + MiscUtils.AppendOptionsText(GetType());
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TOSNeutAssets.ReaperVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Thief);
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.StealRole, SendImmediately = true)]
    public static void RpcStealRole(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not ThiefRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcStealRole - Invalid Thief");
            return;
        }

        var roleWhenAlive = target.GetRoleWhenAlive();

        if (roleWhenAlive is ThiefRole)
        {
            if (player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{target.CachedPlayerData.PlayerName} is a {TownOfSushiColors.Thief.ToTextColor()}Thief</color> as well! so their role cannot be stolen.</b>",
                    Color.white, new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Thief.LoadAsset());
                notif1.Text.SetOutlineThickness(0.35f);
            }
            return;
        }

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.ThiefPreSteal, player, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);

        if (target.IsCrewmate() || target.IsNeutral() && !target.Is(RoleAlignment.NeutralKilling))
        {
            player.RpcCustomMurder(player, teleportMurderer: false);
        }
        else
        {
            player.RpcCustomMurder(target);

            player.ChangeRole((ushort)roleWhenAlive.Role);
            
            if (player.IsImpostor())
            {
                player.SetKillTimer(GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown));
            }
            foreach (var button in CustomButtonManager.Buttons.Where(x => x.Enabled(player.Data.Role)))
            {
                button.SetTimer(OptionGroupSingleton<GeneralOptions>.Instance.GameStartCd);
            }

            if (player.Data.Role is HitmanRole)
            {
                player.ChangeRole(RoleId.Get<AgentRole>());
            }
            if (player.Data.Role is PlaguebearerRole || player.Data.Role is PestilenceRole)
            {
                ModifierUtils.GetActiveModifiers<PlaguebearerInfectedModifier>().Do(x => x.ModifierComponent?.RemoveModifier(x));
            }
            else if (player.Data.Role is ArsonistRole)
            {
                ModifierUtils.GetActiveModifiers<ArsonistDousedModifier>().Do(x => x.ModifierComponent?.RemoveModifier(x));
            }
            else if (player.Data.Role is VampireRole)
            {
                if (target.HasModifier<VampireBittenModifier>())
                {
                    // Makes the amne stay with the bitten modifier
                    player.AddModifier<VampireBittenModifier>();
                }
                else
                {
                    // Makes the og vampire a bitten vampire so to speak, yes it makes it more confusing, but that's how it is, deal with it - Atony
                    target.AddModifier<VampireBittenModifier>();
                }
            }

            if (player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{target.Data.PlayerName}'s role was {player.Data.Role.TeamColor.ToTextColor()}{player.Data.Role.NiceName}</color>. You have stolen their role!</b>",
                    Color.white, new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Thief.LoadAsset());
                notif1.Text.SetOutlineThickness(0.35f);
            }

            if (roleWhenAlive is not VampireRole && (roleWhenAlive.MaxCount <= 1 || (roleWhenAlive.MaxCount <= PlayerControl.AllPlayerControls
                    .ToArray().Count(x => x.Data.Role.Role == roleWhenAlive.Role))))
            {
                if (target.IsCrewmate())
                {
                    target.ChangeRole((ushort)RoleTypes.Crewmate);
                }
                else if (target.IsImpostor())
                {
                    target.ChangeRole((ushort)RoleTypes.Impostor);
                }
                else
                {
                    target.ChangeRole(RoleId.Get<JesterRole>());
                }
            }

            if (player.IsImpostor() && OptionGroupSingleton<AssassinOptions>.Instance.ThiefTurnAssassin)
            {
                player.AddModifier<ImpostorAssassinModifier>();
            }
            else if (player.IsNeutral() && player.Is(RoleAlignment.NeutralKilling) &&
                     OptionGroupSingleton<AssassinOptions>.Instance.ThiefTurnAssassin)
            {
                player.AddModifier<NeutralKillerAssassinModifier>();
            }

            var TOSAbilityEvent2 = new TOSAbilityEvent(AbilityType.ThiefPostSteal, player, target);
            MiraEventManager.InvokeEvent(TOSAbilityEvent2);
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TOSAssets.VentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Impostor);
        }
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    public override bool CanUse(IUsable usable)
    {
        if (!GameManager.Instance.LogicUsables.CanUse(usable, Player))
        {
            return false;
        }

        var console = usable.TryCast<Console>()!;
        return console == null || console.AllowImpostor;
    }
}