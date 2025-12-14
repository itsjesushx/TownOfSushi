using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using TownOfSushi.Modifiers;
using TownOfSushi.Modules;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class JesterRole(IntPtr cppPtr)
    : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, ICrewVariant, IGuessable, IMysticClue
{
    public bool Voted { get; set; }
    public bool AboutToWin { get; set; }
    public bool SentWinMsg { get; set; }

    [HideFromIl2Cpp] public List<byte> Voters { get; } = [];

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<PlumberRole>());
    public MysticClueType MysticHintType => MysticClueType.Trickster;
    public string RoleName => "Jester";
    public string RoleDescription => "Get voted out!";
    public string RoleLongDescription => "Be as suspicious as possible, and get voted out!";
    public Color RoleColor => TownOfSushiColors.Jester;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralEvil;
    // This is so the role can be guessed without requiring it to be enabled normally
    public bool CanBeGuessed =>
        (MiscUtils.GetPotentialRoles()
                .Contains(RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<ExecutionerRole>())) &&
            OptionGroupSingleton<ExecutionerOptions>.Instance.OnTargetDeath is BecomeOptions.Jester)
        || (MiscUtils.GetPotentialRoles()
                .Contains(RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<RomanticRole>())) &&
            OptionGroupSingleton<RomanticOptions>.Instance.OnTargetDeath is BecomeOptions.Jester);
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<JesterOptions>.Instance.CanVent,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Noisemaker),
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = TOSRoleIcons.Jester
    };

    public bool MetWinCon => Voted;

    public bool HasImpostorVision => OptionGroupSingleton<JesterOptions>.Instance.ImpostorVision;

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public bool WinConditionMet()
    {
        if (OptionGroupSingleton<JesterOptions>.Instance.JestWin is not JestWinOptions.EndsGame)
        {
            return false;
        }

        return Voted ||
               GameHistory.DeathHistory.Exists(x => x.Item1 == Player.PlayerId && x.Item2 == DeathReason.Exile);
    }

    public string GetAdvancedDescription()
    {
        return $"The {RoleName} is a Neutral Evil role that wins by getting themselves ejected." +
               MiscUtils.AppendOptionsText(GetType());
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (!OptionGroupSingleton<JesterOptions>.Instance.CanButton)
        {
            player.RemainingEmergencies = 0;
        }

        if (Player.AmOwner)
        {
            if (OptionGroupSingleton<JesterOptions>.Instance.ScatterOn)
            {
                Player.AddModifier<ScatterModifier>(OptionGroupSingleton<JesterOptions>.Instance.ScatterTimer);
            }

            HudManager.Instance.ImpostorVentButton.graphic.sprite = TOSNeutAssets.JesterVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Jester);
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        if (Player.AmOwner)
        {
            if (OptionGroupSingleton<JesterOptions>.Instance.ScatterOn)
            {
                Player.RemoveModifier<ScatterModifier>();
            }

            HudManager.Instance.ImpostorVentButton.graphic.sprite = TOSAssets.VentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Impostor);
        }
        if (!Player.HasModifier<BasicGhostModifier>() && Voted)
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
        //Logger<TownOfSushiPlugin>.Message($"JesterRole.DidWin - Voted: '{Voted}', Exists: '{GameHistory.DeathHistory.Exists(x => x.Item1 == Player.PlayerId && x.Item2 == DeathReason.Exile)}'");

        return Voted ||
               GameHistory.DeathHistory.Exists(x => x.Item1 == Player.PlayerId && x.Item2 == DeathReason.Exile);
    }
}