
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class JuggernautRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITOSRole, IWikiDiscoverable, IMysticClue
{
    public int KillCount { get; set; }
    public string RoleName => "Juggernaut";
    public string RoleDescription => "Your power grows with every kill";
    public string RoleLongDescription => "With each kill your kill cooldown decreases";
    public MysticClueType MysticHintType => MysticClueType.Relentless;
    public Color RoleColor => TownOfSushiColors.Juggernaut;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;
    public Factions Faction => Factions.Neutral;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<JuggernautOptions>.Instance.CanVent,
        IntroSound = TownOfSushiAudio.HexbladeIntroSound,
        Icon = TownOfSushiAssets.Juggernaut,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    public bool HasImpostorVision => true;

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITOSRole.SetNewTabText(this);

        stringB.Append(TownOfSushiPlugin.Culture, $"\n<b>Kill Count:</b> {KillCount}");

        return stringB;
    }

    public bool WinConditionMet()
    {
        var roleCount = CustomRoleUtils.GetActiveRolesOfType<JuggernautRole>().Count(x => !x.Player.HasDied());

        if (Utils.KillersAliveCount > roleCount)
        {
            return false;
        }

        return roleCount >= Helpers.GetAlivePlayers().Count - roleCount;
    }

    public string GetAdvancedDescription()
    {
        return
            "The Juggernaut is a Neutral Killing role that wins by being the last killer alive. For each kill they get, their kill cooldown gets reduced." +
            Utils.AppendOptionsText(GetType());
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TownOfSushiAssets.JuggVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Juggernaut);
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