
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modifiers;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class VampireRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Vampire";
    public string RoleDescription => "Convert Crewmates And Kill The Rest";
    public string RoleLongDescription => "Bite all other players";
    public string YouAreText => "You Are A";
    public string YouWereText => "You Were A";
    public MysticClueType MysticHintType => MysticClueType.Death;
    public Color RoleColor => TownOfSushiColors.Vampire;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<VampireOptions>.Instance.CanVent,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Phantom),
        Icon = TOSRoleIcons.Vampire,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        MaxRoleCount = 1
    };

    public bool HasImpostorVision => OptionGroupSingleton<VampireOptions>.Instance.HasVision;

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var alignment = RoleAlignment.ToDisplayString().Replace("Neutral", "<color=#8A8A8AFF>Neutral");

        var stringB = new StringBuilder();
        stringB.AppendLine(TownOfSushiPlugin.Culture,
            $"{RoleColor.ToTextColor()}You are a<b> {RoleName}.</b></color>");
        stringB.AppendLine(TownOfSushiPlugin.Culture, $"<size=60%>Alignment: <b>{alignment}</color></b></size>");
        stringB.Append("<size=70%>");
        stringB.AppendLine(TownOfSushiPlugin.Culture, $"{RoleLongDescription}");

        return stringB;
    }

    public bool WinConditionMet()
    {
        var vampireCount = CustomRoleUtils.GetActiveRolesOfType<VampireRole>().Count(x => !x.Player.HasDied());

        if (MiscUtils.KillersAliveCount > vampireCount)
        {
            return false;
        }

        return vampireCount >= Helpers.GetAlivePlayers().Count - vampireCount;
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Bite",
            "Bite a player. If the bitten player is a Crewmate and you have not exceeded the maximum amount of vampires in a game yet. You convert them into a vampire. Otherwise they just get killed.",
            TOSNeutAssets.BiteSprite)
    ];

    public string GetAdvancedDescription()
    {
        return
            "The Vampire is a Neutral Killing role that wins by being the last killer(s) alive. They can bite, changing others into Vampires, or kill players." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TOSNeutAssets.VampVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Vampire);
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
        if (!Player.HasModifier<BasicGhostModifier>())
        {
            Player.AddModifier<BasicGhostModifier>();
        }
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

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    [MethodRpc((uint)TownOfSushiRpc.VampireBite, SendImmediately = true)]
    public static void RpcVampireBite(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not VampireRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcVampireBite - Invalid vampire");
            return;
        }

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.VampireBite, player, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);

        target.ChangeRole(RoleId.Get<VampireRole>());
        target.AddModifier<VampireBittenModifier>();

        if (OptionGroupSingleton<VampireOptions>.Instance.CanGuessAsNewVamp)
        {
            target.AddModifier<NeutralKillerAssassinModifier>();
        }
    }
}