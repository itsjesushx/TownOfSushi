using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using System.Globalization;
using System.Text;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modules;
using TownOfUs.Modules.Components;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class ScavengerRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Scavenger";
    public string RoleDescription => "Eat dead bodies around the map in order to get your win!";
    public string RoleLongDescription => $"Eat {OptionGroupSingleton<ScavengerOptions>.Instance.EatNeed} bodies to win";
    public Color RoleColor => TownOfSushiColors.Scavenger;
    public MysticClueType MysticHintType => MysticClueType.Death;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralEvil;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<ScavengerOptions>.Instance.CanVent,
        IntroSound = TOSAudio.JanitorCleanSound,
        Icon = TOSNeutAssets.EatSprite,
        MaxRoleCount = 1,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };
    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TOSNeutAssets.JuggVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Scavenger);
        }
    }
    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TOSAssets.VentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Scavenger);
        }
    }

    public bool HasImpostorVision => OptionGroupSingleton<ScavengerOptions>.Instance.HasImpostorVision;

    public int EatenBodies { get; set; }

    [MethodRpc((uint)TownOfSushiRpc.ScavengerEatBody, LocalHandling = RpcLocalHandling.Before, SendImmediately = true)]
    public static void RpcEatBody(PlayerControl player, byte bodyId)
    {
        if (player.GetRoleWhenAlive() is not ScavengerRole vult)
        {
            Logger<TownOfSushiPlugin>.Error("RpcCleanBody - Invalid Scavenger");
            return;
        }

        var body = Helpers.GetBodyById(bodyId);

        if (body != null)
        {
            var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.ScavengerEat, player, body);
            MiraEventManager.InvokeEvent(TOSAbilityEvent);

            Coroutines.Start(body.CoClean());
            Coroutines.Start(CrimeSceneComponent.CoClean(body));
            vult!.EatenBodies += 1;
        }
    }



    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);

        stringB.Append(CultureInfo.InvariantCulture, $"\n<b>Eaten bodies:</b> {EatenBodies}");

        return stringB;
    }
    public bool WinConditionMet()
    {
        return EatenBodies == OptionGroupSingleton<ScavengerOptions>.Instance.EatNeed;
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
        Console console = usable.TryCast<Console>()!;
        return (console == null) || console.AllowImpostor;
    }

    public string GetAdvancedDescription()
    {
        return $"The Scavenger is a Neutral Evil role that wins by being eating corpses. If they each {OptionGroupSingleton<ScavengerOptions>.Instance.EatNeed} bodies, they win." + MiscUtils.AppendOptionsText(GetType());
    }
    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Scavenge",
            "Once you activate your scavenge button, you'll get arrows pointing to all bodies on the map.",
            TOSNeutAssets.ScavengeSprite)
    ];
}