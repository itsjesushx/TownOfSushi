using System.Collections;
using System.Text;
using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Buttons.Neutral;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Roles.Neutral;

using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class PlaguebearerRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable, ICrewVariant
{
    public string RoleName => "Plaguebearer";
    public string RoleDescription => "Infect Everyone To Become <color=#4D4D4DFF>Pestilence</color>";
    public string RoleLongDescription => "Infect everyone to become <color=#4D4D4DFF>Pestilence</color>";
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<AurialRole>());
    public Color RoleColor => TownOfSushiColors.Plaguebearer;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;
    public DoomableType DoomHintType => DoomableType.Fearmonger;
    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Phantom),
        Icon = TosRoleIcons.Plaguebearer,
        MaxRoleCount = 1,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };
    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner && (int)OptionGroupSingleton<PlaguebearerOptions>.Instance.PestChance != 0)
        {
            Coroutines.Start(CheckForPestChance(Player));
        }
    }
    private static IEnumerator CheckForPestChance(PlayerControl player)
    {
        yield return new WaitForSeconds(0.01f);

        System.Random rnd = new();
        var chance = rnd.Next(0, 100);

        if (chance <= OptionGroupSingleton<PlaguebearerOptions>.Instance.PestChance)
        {
            player.RpcChangeRole(RoleId.Get<PestilenceRole>());
            CustomButtonSingleton<PestilenceKillButton>.Instance.SetTimer(OptionGroupSingleton<PlaguebearerOptions>.Instance.PestKillCooldown);
        }
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);

        var allInfected = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x != Player && x.GetModifier<PlaguebearerInfectedModifier>()?.PlagueBearerId == Player.PlayerId);

        if (allInfected.Any())
        {
            stringB.Append("\n<b>Players Infected:</b>");
            foreach (var plr in allInfected)
            {
                stringB.Append(TownOfSushiPlugin.Culture, $"\n{Color.white.ToTextColor()}{plr.Data.PlayerName}</color>");
            }
        }

        var notInfected = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x != Player && !x.HasModifier<PlaguebearerInfectedModifier>());
        stringB.Append(TownOfSushiPlugin.Culture, $"\n\n<b>Players Left To Infect: {notInfected.Count()}</b>");

        return stringB;
    }

    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not PlaguebearerRole || Player.HasDied()) return;

        var allInfected = ModifierUtils.GetPlayersWithModifier<PlaguebearerInfectedModifier>([HideFromIl2Cpp] (x) => x.PlagueBearerId == Player.PlayerId && !x.Player.HasDied());

        if (allInfected.Count() >= Helpers.GetAlivePlayers().Count - 1 && (!MeetingHud.Instance || Helpers.GetAlivePlayers().Count > 2))
        {
            var players = ModifierUtils.GetPlayersWithModifier<PlaguebearerInfectedModifier>([HideFromIl2Cpp] (x) => x.PlagueBearerId == Player.PlayerId);

            players.Do(x => x.RemoveModifier<PlaguebearerInfectedModifier>([HideFromIl2Cpp] (x) => x.PlagueBearerId == Player.PlayerId));

            Player.ChangeRole(RoleId.Get<PestilenceRole>());

            CustomButtonSingleton<PestilenceKillButton>.Instance.SetTimer(OptionGroupSingleton<PlaguebearerOptions>.Instance.PestKillCooldown);
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

    public static void CheckInfected(PlayerControl source, PlayerControl target)
    {
        if (source.Data.Role is PlaguebearerRole)
        {
            target.AddModifier<PlaguebearerInfectedModifier>(source.PlayerId);
        }
        else if (target.Data.Role is PlaguebearerRole)
        {
            source.AddModifier<PlaguebearerInfectedModifier>(target.PlayerId);
        }
        else if (source.TryGetModifier<PlaguebearerInfectedModifier>(out var mod) && !target.HasModifier<PlaguebearerInfectedModifier>())
        {
            target.AddModifier<PlaguebearerInfectedModifier>(mod.PlagueBearerId);
        } 
        else if (target.TryGetModifier<PlaguebearerInfectedModifier>(out var mod2) && !source.HasModifier<PlaguebearerInfectedModifier>())
        {
            source.AddModifier<PlaguebearerInfectedModifier>(mod2.PlagueBearerId);
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.CheckInfected, SendImmediately = true)]
    public static void RpcCheckInfected(PlayerControl source, PlayerControl target)
    {
        CheckInfected(source, target);
    }

    public string GetAdvancedDescription()
    {
        return "The Plaguebearer is a Neutral Killing role that needs to infect all other players to turn into the Pestilence." + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Infect",
            "Infect a player, causing them to be infected. When a infected player or dead body interacts or get interacted with the infection will spread to all non-infected players.",
            TosNeutAssets.InfectSprite)    
    ];
}
