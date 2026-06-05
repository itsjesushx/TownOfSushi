
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using TownOfSushi.Modifiers;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class PestilenceRole(IntPtr cppPtr)
    : NeutralRole(cppPtr), ITOSRole, IWikiDiscoverable, IUnguessable, ICrewVariant, IMysticClue
{
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<VeteranRole>());
    public MysticClueType MysticHintType => MysticClueType.Fearmonger;
    public string RoleName => "Pestilence";
    public string RoleDescription => "Horseman of the apocalypse!";
    public string RoleLongDescription => "Kill everyone in your path that interacts with you!";
    public Color RoleColor => TownOfSushiColors.Pestilence;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;
    public Factions Faction => Factions.Neutral;
    public bool HasImpostorVision => true;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<PlaguebearerOptions>.Instance.CanVent,
        HideSettings = true,
        CanModifyChance = false,
        DefaultChance = 0,
        DefaultRoleCount = 0,
        MaxRoleCount = 0,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Phantom),
        Icon = TownOfSushiAssets.Pestilence,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    public bool WinConditionMet()
    {
        var roleCount = CustomRoleUtils.GetActiveRolesOfType<PestilenceRole>().Count(x => !x.Player.HasDied());

        if (Utils.KillersAliveCount > roleCount)
        {
            return false;
        }

        return roleCount >= Helpers.GetAlivePlayers().Count - roleCount;
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = new StringBuilder();
        stringB.AppendLine(CultureInfo.InvariantCulture,
            $"{RoleColor.ToTextColor()}You are The<b> {RoleName},\n<size=80%>{RoleDescription}</size></b></color>");
        stringB.AppendLine(CultureInfo.InvariantCulture,
            $"<size=60%>Alignment: <b>{Utils.GetParsedRoleAlignment(RoleAlignment, true)}</b></size>");
        stringB.Append("<size=70%>");
        stringB.AppendLine(CultureInfo.InvariantCulture, $"{RoleLongDescription}");

        return stringB;
    }

    public bool IsGuessable => false;
    public RoleBehaviour AppearAs => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<PlaguebearerRole>());

    public string GetAdvancedDescription()
    {
        return
            "The Pestillence is a Neutral Killing role that can kill and is invincible to everything but being exiled or guessing incorrectly. They win by being the last killer alive." +
            Utils.AppendOptionsText(GetType());
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        player.AddModifier<InvulnerabilityModifier>(true, true, false);

        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TownOfSushiAssets.PestVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Pestilence);
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        targetPlayer.RemoveModifier<InvulnerabilityModifier>();

        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TownOfSushiAssets.VentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Impostor);
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
}