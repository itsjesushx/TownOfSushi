using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class GlitchRole(IntPtr cppPtr)
    : NeutralRole(cppPtr), ITOSRole, IWikiDiscoverable, ICrewVariant, IMysticClue
{
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<SheriffRole>());
    public string RoleName => "The Glitch";
    public string RoleDescription => "Murder, Mimic and Hack players to eliminate them";
    public string RoleLongDescription => "Murder everyone to win with your abilities!";
    public Color RoleColor => TownOfSushiColors.Glitch;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;
    public Factions Faction => Factions.Neutral;
    public MysticClueType MysticHintType => MysticClueType.Perception;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<GlitchOptions>.Instance.CanVent,
        IntroSound = TownOfSushiAudio.GlitchSound,
        Icon = TownOfSushiAssets.Glitch,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    public bool HasImpostorVision => true;

    public bool WinConditionMet()
    {
        var roleCount = CustomRoleUtils.GetActiveRolesOfType<GlitchRole>().Count(x => !x.Player.HasDied());

        if (Utils.KillersAliveCount > roleCount)
        {
            return false;
        }

        return roleCount >= Helpers.GetAlivePlayers().Count - roleCount;
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITOSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Neutral Killing role that wins by being the last killer alive. They can Mimic into another player or they can hack a player." +
            Utils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Mimic",
            "Mimic the appearance of another player, taking on their whole look.",
            TownOfSushiAssets.MimicSprite),
        new("Hack",
            "Disable a player's abilities.",
            TownOfSushiAssets.HackSprite)
    ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TownOfSushiAssets.GlitchVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Glitch);
        }
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