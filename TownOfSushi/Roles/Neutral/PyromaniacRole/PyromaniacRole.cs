using System.Globalization;
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Hud;
using MiraAPI.Patches.Stubs;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class PyromaniacRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public string RoleName => "Pyromaniac";
    public string RoleDescription => "Douse Players And Ignite The Light";

    public string RoleLongDescription => OptionGroupSingleton<PyromaniacOptions>.Instance.LegacyPyromaniac
        ? "Douse players and ignite the closest one to kill all doused targets"
        : "Douse players and ignite to kill all nearby doused targets";

    public MysticClueType MysticHintType => MysticClueType.Fearmonger;

    public Color RoleColor => TownOfSushiColors.Pyromaniac;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<PyromaniacOptions>.Instance.CanVent,
        IntroSound = TOSAudio.ArsoIgniteSound,
        Icon = TOSRoleIcons.Pyromaniac,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);

        var allDoused = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && x.GetModifier<PyromaniacDousedModifier>()?.PyromaniacId == Player.PlayerId);

        if (allDoused.Any())
        {
            stringB.Append("\n<b>Players Doused:</b>");
            foreach (var plr in allDoused)
            {
                stringB.Append(CultureInfo.InvariantCulture,
                    $"\n{Color.white.ToTextColor()}{plr.Data.PlayerName}</color>");
            }
        }

        return stringB;
    }

    public bool WinConditionMet()
    {
        var roleCount = CustomRoleUtils.GetActiveRolesOfType<PyromaniacRole>().Count(x => !x.Player.HasDied());

        if (MiscUtils.KillersAliveCount > roleCount)
        {
            return false;
        }

        return roleCount >= Helpers.GetAlivePlayers().Count - roleCount;
    }

    public string GetAdvancedDescription()
    {
        return $"The {RoleName} is a Neutral Killing role that wins by being the last killer alive. " +
               (OptionGroupSingleton<PyromaniacOptions>.Instance.LegacyPyromaniac
                   ? "They can douse players and ignite one of them to ignite all doused players on the map."
                   : "They can douse players and ignite them when close.") + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Douse",
            "Douse a player in gasoline",
            TOSNeutAssets.DouseButtonSprite),
        new("Ignite",
            OptionGroupSingleton<PyromaniacOptions>.Instance.LegacyPyromaniac
                ? "Kill every doused player on the map as long as you ignite one player close by."
                : "Kill multiple doused players around you, given that they are within your radius.",
            TOSNeutAssets.IgniteButtonSprite)
    ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TOSNeutAssets.ArsoVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Pyromaniac);
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

    public override void OnDeath(DeathReason reason)
    {
        var button = CustomButtonSingleton<PyromaniacIgniteButton>.Instance;

        if (button != null && button.Ignite != null)
        {
            button.Ignite.Clear();
            button.Ignite = null;
        }

        RoleBehaviourStubs.OnDeath(this, reason);
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