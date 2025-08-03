using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using System.Globalization;
using System.Text;
using TownOfSushi.Modifiers;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Roles.Neutral;

using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class PestilenceRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable, IUnguessable, ICrewVariant
{
    public string RoleName => "Pestilence";
    public string RoleDescription => "Horseman Of The Apocalypse!";
    public string RoleLongDescription => "Kill everyone in your path that interacts with you!";
    public string YouAreText => "You are";
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<AurialRole>());
    public Color RoleColor => TownOfSushiColors.Pestilence;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;
    public DoomableType DoomHintType => DoomableType.Fearmonger;
    public bool IsGuessable => false;
    public RoleBehaviour AppearAs => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<PlaguebearerRole>());
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<PlaguebearerOptions>.Instance.CanVent,
        HideSettings = true,
        CanModifyChance = false,
        DefaultChance = 0,
        DefaultRoleCount = 0,
        MaxRoleCount = 0,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Phantom),
        Icon = TosRoleIcons.Pestilence,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };
    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        player.AddModifier<InvulnerabilityModifier>(true, true, false);

        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TosNeutAssets.PestVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Pestilence);
        }
    }
    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        targetPlayer.RemoveModifier<InvulnerabilityModifier>();

        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TosAssets.VentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Impostor);
        }
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

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    public bool WinConditionMet()
    {
        if (Player.HasDied()) return false;

        var result = Helpers.GetAlivePlayers().Count <= 2 && MiscUtils.KillersAliveCount == 1;

        return result;
    }
    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var alignment = RoleAlignment.ToDisplayString();

        alignment = alignment.Replace("Neutral", "<color=#8A8A8AFF>Neutral");

        var stringB = new StringBuilder();
        stringB.AppendLine(CultureInfo.InvariantCulture, $"{RoleColor.ToTextColor()}You are<b> {RoleName},\n<size=80%>Horseman of the Apocalypse.</size></b></color>");
        stringB.AppendLine(CultureInfo.InvariantCulture, $"<size=60%>Alignment: <b>{alignment}</color></b></size>");
        stringB.Append("<size=70%>");
        stringB.AppendLine(CultureInfo.InvariantCulture, $"{RoleLongDescription}");

        return stringB;
    }

    public string GetAdvancedDescription()
    {
        return "The Pestillence is a Neutral Killing role that can kill and is invincible to everything but being exiled or guessing incorrectly. They win by being the last killer alive." + MiscUtils.AppendOptionsText(GetType());
    }
}
