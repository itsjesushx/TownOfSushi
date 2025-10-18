using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Networking;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class WerewolfRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, ICrewVariant, IMysticClue
{
    public string RoleName => "Werewolf";
    public string RoleDescription => "Maul To Kill Everyone";
    public string RoleLongDescription => "Maul to kill nearby players within a radius";
    public MysticClueType MysticHintType => MysticClueType.Fearmonger;
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<HunterRole>());
    public Color RoleColor => TownOfSushiColors.Werewolf;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<WerewolfOptions>.Instance.CanVent,
        IntroSound = TOSAudio.PredatorTerminateSound,
        Icon = TOSRoleIcons.Werewolf,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };
    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TOSNeutAssets.PredatorVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Werewolf);
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

    public bool HasImpostorVision => OptionGroupSingleton<WerewolfOptions>.Instance.HasImpostorVision;
    public override bool CanUse(IUsable usable)
    {
        if (!GameManager.Instance.LogicUsables.CanUse(usable, Player))
        {
            return false;
        }
        Console console = usable.TryCast<Console>()!;
        return (console == null) || console.AllowImpostor;
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    public bool WinConditionMet()
    {
        var roleCount = CustomRoleUtils.GetActiveRolesOfType<WerewolfRole>().Count(x => !x.Player.HasDied());

        if (MiscUtils.KillersAliveCount > roleCount)
        {
            return false;
        }

        return roleCount >= Helpers.GetAlivePlayers().Count - roleCount;
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    [MethodRpc((uint)TownOfSushiRpc.WerewolfMaul, SendImmediately = true)]
    public static void RpcMaulMurder(PlayerControl player)
    {
        if (player.Data.Role is not WerewolfRole role)
        {
            Logger<TownOfSushiPlugin>.Error("RpcMaulMurder - Invalid werewolf");
            return;
        }
        
        var nearbyPlayers = MiscUtils.GetClosestPlayers(role.Player.GetTruePosition(), OptionGroupSingleton<WerewolfOptions>.Instance.RaulRadius);

        foreach (var nearbyPlayer in nearbyPlayers)
        {
            if (nearbyPlayer == role.Player || nearbyPlayer.Data.IsDead || nearbyPlayer.IsProtected()
            || player.HasModifier<MonarchKnightedModifier>() && nearbyPlayer.Data.Role is MonarchRole) continue;

            player.RpcCustomMurder(nearbyPlayer, teleportMurderer: false);
        }
    }

    public string GetAdvancedDescription()
    {
        return "The Werewolf is a Neutral Killing role that wins by being the last killer alive. When they kill someone using their Maul button, any nearby player will die. The range between the players that can die and the Werewolf can be changed in settings." + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Maul",
            "Once you kill a player, any nearby player inbetween the range set in settings, is dying too.",
            TOSNeutAssets.MaulSprite)
    ];
}