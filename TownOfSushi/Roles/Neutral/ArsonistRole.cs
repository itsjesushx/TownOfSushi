using System.Globalization;
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfSushi.Buttons.Neutral;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Roles.Neutral;

using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class ArsonistRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable
{
    public string RoleName => "Arsonist";
    public string RoleDescription => "Douse Players And Ignite The Light";
    public string RoleLongDescription => OptionGroupSingleton<ArsonistOptions>.Instance.LegacyArsonist ? "Douse players and ignite the closest one to kill all doused targets" : "Douse players and ignite to kill all nearby doused targets";
    public Color RoleColor => TownOfSushiColors.Arsonist;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;
    public DoomableType DoomHintType => DoomableType.Fearmonger;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<ArsonistOptions>.Instance.CanVent,
        IntroSound = TosAudio.ArsoIgniteSound,
        MaxRoleCount = 1,
        Icon = TosRoleIcons.Arsonist,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };
    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TosNeutAssets.ArsoVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Arsonist);
        }
    }
    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TosAssets.VentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Impostor);
        }
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);

        var allDoused = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x.GetModifier<ArsonistDousedModifier>()?.ArsonistId == Player.PlayerId);

        if (allDoused.Any())
        {
            stringB.Append("\n<b>Players Doused:</b>");
            foreach (var plr in allDoused)
            {
                stringB.Append(CultureInfo.InvariantCulture, $"\n{Color.white.ToTextColor()}{plr.Data.PlayerName}</color>");
            }
        }

        return stringB;
    }

    public override void OnDeath(DeathReason reason)
    {
        var button = CustomButtonSingleton<ArsonistIgniteButton>.Instance;

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

    public string GetAdvancedDescription()
    {
        return "The Arsonist is a Neutral Killing role that wins by being the last killer alive. " + (OptionGroupSingleton<ArsonistOptions>.Instance.LegacyArsonist ? "They can douse players and ignite one of them to ignite all doused players on the map." : "They can douse players and ignite them when close.") + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Douse",
            "Douse a player in gasoline",
            TosNeutAssets.DouseButtonSprite),
        new("Ignite",
            OptionGroupSingleton<ArsonistOptions>.Instance.LegacyArsonist ? "Kill every doused player on the map as long as you ignite one player close by." : "Kill multiple doused players around you, given that they are within your radius.",
            TosNeutAssets.IgniteButtonSprite)
    ];
}
