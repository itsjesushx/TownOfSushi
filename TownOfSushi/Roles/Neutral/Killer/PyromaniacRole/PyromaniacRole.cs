
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class PyromaniacRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITOSRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Pyromaniac";
    public string RoleDescription => "Douse players and blow up everyone";

    public string RoleLongDescription => "Douse players and kill doused players to get short cooldown";

    public MysticClueType MysticHintType => MysticClueType.Fearmonger;

    public Color RoleColor => TownOfSushiColors.Pyromaniac;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;
    public Factions Faction => Factions.Neutral;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<PyromaniacOptions>.Instance.CanVent,
        IntroSound = TownOfSushiAudio.ArsoIgniteSound,
        Icon = TownOfSushiAssets.Pyromaniac,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITOSRole.SetNewTabText(this);

        var allDoused = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && x.GetModifier<PyromaniacDousedModifier>()?.PyromaniacId == Player.PlayerId);

        if (allDoused.Any())
        {
            stringB.Append("\n<b>Players Doused:</b>");
            foreach (var plr in allDoused)
            {
                stringB.Append(TownOfSushiPlugin.Culture,
                    $"\n{Color.white.ToTextColor()}{plr.Data.PlayerName}</color>");
            }
        }

        return stringB;
    }

    public bool WinConditionMet()
    {
        var roleCount = CustomRoleUtils.GetActiveRolesOfType<PyromaniacRole>().Count(x => !x.Player.HasDied());

        if (Utils.KillersAliveCount > roleCount)
        {
            return false;
        }

        return roleCount >= Helpers.GetAlivePlayers().Count - roleCount;
    }

    public string GetAdvancedDescription()
    {
        return $"The {RoleName} is a Neutral Killing role that wins by being the last killer alive. They have to douse players to kill them after, if they kill who they got doused, their cooldown is shorter." + Utils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Douse",
            "Douse a player in gasoline",
            TownOfSushiAssets.DouseButtonSprite),
        new("Ignite",
            OptionGroupSingleton<PyromaniacOptions>.Instance.LegacyPyromaniac
                ? "Kill every doused player on the map as long as you ignite one player close by."
                : "Kill multiple doused players around you, given that they are within your radius.",
            TownOfSushiAssets.IgniteButtonSprite)
    ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TownOfSushiAssets.ArsoVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Pyromaniac);
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