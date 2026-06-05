using System.Text;
using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.Hud;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using TownOfSushi.Events.TOSEvents;

using TownOfSushi.Options;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class ThiefRole(IntPtr cppPtr)
    : NeutralRole(cppPtr), ITOSRole, IWikiDiscoverable, ICrewVariant, IGuessable, IMysticClue
{
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<SheriffRole>());
    public string RoleName => "Thief";
    public string RoleDescription => "Murder an evildoer to gain their role";
    public string RoleLongDescription => "Murder evildoers but not crewmates to get a new role";
    public MysticClueType MysticHintType => MysticClueType.Hunter;
    public Color RoleColor => TownOfSushiColors.Thief;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralBenign;
    public Factions Faction => Factions.Neutral;
    // This is so the role can be guessed without requiring it to be enabled normally
    public bool CanBeGuessed =>
        (Utils.GetPotentialRoles()
                .Contains(RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<ExecutionerRole>())) &&
            OptionGroupSingleton<ExecutionerOptions>.Instance.OnTargetDeath is BecomeOptions.Thief)
        || (Utils.GetPotentialRoles()
                .Contains(RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<RomanticRole>())) &&
            OptionGroupSingleton<RomanticOptions>.Instance.OnTargetDeath is BecomeOptions.Thief);

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<ThiefOptions>.Instance.CanVent,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Impostor),
        Icon = TownOfSushiAssets.Thief,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        TasksCountForProgress = OptionGroupSingleton<ThiefOptions>.Instance.CanStealSheriff
    };

    public bool HasImpostorVision => OptionGroupSingleton<ThiefOptions>.Instance.HasImpostorVision;

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITOSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "The Thief is a Neutral Benign role that can kill evildoers to gain their role. If they kill a crewmate, they will die instead."
            + Utils.AppendOptionsText(GetType());
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TownOfSushiAssets.VentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Thief);
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.StealRole, SendImmediately = true)]
    public static void RpcStealRole(PlayerControl player, PlayerControl target)
    {
        var roleWhenAlive = target.GetRoleWhenAlive();

        if (roleWhenAlive is ThiefRole)
        {
            if (player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    Utils.ColorString(TownOfSushiColors.Thief, $"<b>{target.CachedPlayerData.PlayerName} is a Thief") +" as well! so their role cannot be stolen.</b>",
                    Color.white, new Vector3(0f, 1f, -20f), spr: TownOfSushiAssets.Thief.LoadAsset());
                notif1.AdjustNotification();
            }
            return;
        }

        if (!target.IsKillerRole() && roleWhenAlive is not SheriffRole)
        {
            if (player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    Utils.ColorString(TownOfSushiColors.Thief, $"<b>{target.CachedPlayerData.PlayerName} was not a killer so you can't steal their role!</b>"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: TownOfSushiAssets.Thief.LoadAsset());
                notif1.AdjustNotification();
            }
            return;
        }
        
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.ThiefPreSteal, player, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);

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
        if (player.Data.Role is PlaguebearerRole)
        {
            ModifierUtils.GetActiveModifiers<PlaguebearerInfectedModifier>().Do(x => x.ModifierComponent?.RemoveModifier(x));
        }
        else if (player.Data.Role is ArsonistRole)
        {
            ModifierUtils.GetActiveModifiers<ArsonistDousedModifier>().Do(x => x.ModifierComponent?.RemoveModifier(x));
        }

        if (player.AmOwner)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{target.Data.PlayerName}'s role was {player.Data.Role.TeamColor.ToTextColor()}{player.Data.Role.NiceName}</color>. You have stolen their role!</b>",
                Color.white, new Vector3(0f, 1f, -20f), spr: TownOfSushiAssets.Thief.LoadAsset());
            notif1.AdjustNotification();
        }

        if (target.Is(Factions.Crewmate))
        {
            target.ChangeRole((ushort)RoleTypes.Crewmate);
        }
        else if (target.Is(Factions.Impostor))
        {
            target.ChangeRole((ushort)RoleTypes.Impostor);
        }
        else
        {
            target.ChangeRole(RoleId.Get<JesterRole>());
        }

        if (player.IsImpostor() && OptionGroupSingleton<AssassinOptions>.Instance.ThiefTurnAssassin)
        {
            player.AddModifier<ImpostorAssassinModifier>();
        }
        else if (player.Is(RoleAlignment.NeutralKilling) && OptionGroupSingleton<AssassinOptions>.Instance.ThiefTurnAssassin)
        {
            player.AddModifier<NeutralKillerAssassinModifier>();
        }
        else if (player.Is(Factions.Crewmate) && OptionGroupSingleton<AssassinOptions>.Instance.ThiefCrewTurnAssassin)
        {
            player.AddModifier<CrewmateAssassinModifier>();
        }

        var TOSAbilityEvent2 = new TOSAbilityEvent(AbilityType.ThiefPostSteal, player, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent2);
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TownOfSushiAssets.VentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Impostor);
        }
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return false;
    }

    public override bool CanUse(IUsable usable)
    {
        if (!GameManager.Instance.LogicUsables.CanUse(usable, Player))
        {
            return false;
        }

        var console = usable.TryCast<Console>()!;
        return console == null || console.AllowImpostor || OptionGroupSingleton<ThiefOptions>.Instance.CanStealSheriff;
    }
}