using System.Globalization;
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Events;
using TownOfSushi.Modules;
using TownOfUs.Modules.Components;
using TownOfSushi.Options;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class AmbassadorRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable
{
    public string RoleName => "Ambassador";
    public string RoleDescription => "Lead The Impostors To Victory";
    public string RoleLongDescription => "Retrain yourself or fellow impostors into other roles\n<b>Imp Killing cannot be a Power role</b>\n<b>Imp Concealing/Support cannot be a Killing/Power role</b>";
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorPower;
    public NetworkedPlayerInfo? SelectedPlr { get; private set; }
    public RoleBehaviour? SelectedRole { get; private set; }
    public int RetrainsAvailable { get; set; }
    public int RoundsCooldown { get; set; }
    private MeetingMenu meetingMenu;

    public void Clear()
    {
        SelectedPlr = null;
        SelectedRole = null;
    }

    public CustomRoleConfiguration Configuration => new(this)
    {
        MaxRoleCount = 1,
        Icon = TOSRoleIcons.Ambassador
    };
    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);

        stringB.AppendLine(CultureInfo.InvariantCulture, $"{RetrainsAvailable} / {OptionGroupSingleton<AmbassadorOptions>.Instance.MaxRetrains} Retrains Remaining");

        return stringB;
    }
    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        RetrainsAvailable = (int)OptionGroupSingleton<AmbassadorOptions>.Instance.MaxRetrains;

        SelectedPlr = null;
        SelectedRole = null;

        if (Player.AmOwner)
        {
            meetingMenu = new MeetingMenu(
                this,
                Click,
                MeetingAbilityType.Toggle,
                TOSAssets.RetrainSprite,
                TOSAssets.RetrainSprite,
                IsExempt,
                activeColor: new Color32(200, 80, 80, 255))
                {
                    Position = new Vector3(-0.40f, 0f, -3f),
                };
        }
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        if (RetrainsAvailable <= 0) return;
        if (DeathEventHandlers.CurrentRound < (int)OptionGroupSingleton<AmbassadorOptions>.Instance.RoundWhenAvailable) return;
        if (RoundsCooldown > 0) return;

        if (Player.AmOwner)
        {
            meetingMenu?.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && !Player.HasModifier<JailedModifier>());
        }
    }

    public override void OnVotingComplete()
    {
        RoleBehaviourStubs.OnVotingComplete(this);

        if (Player.AmOwner)
        {
            meetingMenu?.HideButtons();
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        SelectedPlr = null;

        if (Player.AmOwner)
        {
            meetingMenu?.Dispose();
            meetingMenu = null!;
        }

        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
    }

    public void Click(PlayerVoteArea voteArea, MeetingHud __)
    {
        var player = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId);

        if (SelectedPlr == player)
        {
            RpcRetrain(PlayerControl.LocalPlayer);
            meetingMenu.Actives[voteArea.TargetPlayerId] = false;
            return;
        }

        if (SelectedPlr != null)
        {
            meetingMenu.Actives[voteArea.TargetPlayerId] = false;
            meetingMenu.Actives[SelectedPlr.PlayerId] = false;
            RpcRetrain(PlayerControl.LocalPlayer);
        }
        
        var opt = OptionGroupSingleton<AmbassadorOptions>.Instance;
        if ((int)opt.KillsNeeded > 0)
        {
            var killedAmbassPlayers = GameHistory.KilledPlayers.Count(x =>
                x.KillerId == Player.PlayerId && x.VictimId != Player.PlayerId);
            
            var killedPlayerPlayers = GameHistory.KilledPlayers.Count(x =>
                x.KillerId == voteArea.GetPlayer()?.PlayerId && x.VictimId != voteArea.GetPlayer()?.PlayerId);
            
            if (killedAmbassPlayers < (int)opt.KillsNeeded && killedPlayerPlayers < (int)opt.KillsNeeded)
            {
                var text =
                    $"<b>You or the impostor you are trying to retrain must have at least {(int)opt.KillsNeeded} kills.</b>";
                var notif1 = Helpers.CreateAndShowNotification(text, Color.white, spr: TOSRoleIcons.Ambassador.LoadAsset());

                notif1.Text.SetOutlineThickness(0.35f);
                notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
                return;
            }
        }
        
        var excluded = MiscUtils.AllRoles.Where(x => x is ISpawnChange { NoSpawn: true } || x is ITownOfSushiRole { RoleAlignment: RoleAlignment.ImpostorPower }).Select(x => x.Role).ToList();
        var impRoles = MiscUtils.GetRolesToAssign(ModdedRoleTeams.Impostor, x => !excluded.Contains(x.Role)).Select(x => x.RoleType).ToList();

        foreach (var player2 in PlayerControl.AllPlayerControls)
        {
            if (player2.IsImpostor() && !player2.AmOwner)
            {
                var role = player2.GetRoleWhenAlive();
                if (role)
                {
                    impRoles.Remove((ushort)role!.Role);
                }
                if (player2.TryGetModifier<AmbassadorRetrainedModifier>(out var retrained))
                {
                    impRoles.Remove((ushort)retrained.PreviousRole.Role);
                }
            }
        }
        
        var roleList = MiscUtils.GetPotentialRoles()
            .Where(role => role is ICustomRole)
            .Where(role => impRoles.Contains(RoleId.Get(role.GetType())))
            .ToList();

        if (!player._object.Is(RoleAlignment.ImpostorKilling) && !player._object.Is(RoleAlignment.ImpostorPower))
        {
            var curRoleList = MiscUtils.GetPotentialRoles()
                .Where(role => role is ICustomRole)
                .Where(role => impRoles.Contains(RoleId.Get(role.GetType())))
                .ToList();
            foreach (var roleBehaviour in curRoleList)
            {
                if (roleBehaviour is ITownOfSushiRole TOSRole && TOSRole.RoleAlignment == RoleAlignment.ImpostorKilling)
                {
                    roleList.Remove(roleBehaviour);
                }
            }
        }

            
        var traitorMenu = AmbassadorSelectionMinigame.Create();
        traitorMenu.Open(
            roleList,
            role =>
            {
                if (role != null)
                {
                    meetingMenu.Actives[voteArea.TargetPlayerId] = true;
                    RpcRetrain(PlayerControl.LocalPlayer, player.PlayerId, (ushort)role.Role);
                }
                traitorMenu.Close();
            }
        );
    }

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        return Player.Data.IsDead || voteArea.AmDead || voteArea.GetPlayer()?.IsImpostor() == false ||
               voteArea.GetPlayer()?.HasModifier<AmbassadorRetrainedModifier>() == true
               || OptionGroupSingleton<GeneralOptions>.Instance.FFAImpostorMode && !Player.AmOwner;
    }

    [MethodRpc((uint)TownOfSushiRpc.RetrainConfirm, SendImmediately = true)]
    public static void RpcRetrainConfirm(PlayerControl ambassador, PlayerControl player, int cooldown, ushort role = 0, bool accepted = false)
    {
        if (ambassador.Data.Role is not AmbassadorRole ambassadorRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcRetrainConfirm - Invalid ambassador");
            return;
        }
        if (player != ambassadorRole.SelectedPlr?._object)
        {
            Logger<TownOfSushiPlugin>.Error("RpcRetrainConfirm - Retrainee is not valid!");
            return;
        }
        if (ambassadorRole.SelectedPlr == null || ambassadorRole.SelectedRole == null || ambassadorRole.Player.Data.IsDead || ambassadorRole.SelectedPlr.IsDead)
        {
            ambassadorRole.Clear();
            Logger<TownOfSushiPlugin>.Error("RpcRetrainConfirm - A player or role check failed");
            return;
        }
        if (MeetingHud.Instance || ExileController.Instance)
        {
            Logger<TownOfSushiPlugin>.Error("RpcRetrainConfirm - You thought you were slick, huh? No, you can't retrain outside of rounds!");
            return;
        }
        
        ambassadorRole.Clear();
        var newRole = RoleManager.Instance.GetRole((RoleTypes)role)!;

        if (accepted)
        {
            --ambassadorRole.RetrainsAvailable;
            ambassadorRole.RoundsCooldown = cooldown;
            var currentTime = 0f;
            if (player.AmOwner) currentTime = player.killTimer;

            player.AddModifier<AmbassadorRetrainedModifier>((ushort)player.Data.Role.Role);
            player.ChangeRole(role);

            if (PlayerControl.LocalPlayer.IsImpostor() &&
                (!OptionGroupSingleton<GeneralOptions>.Instance.FFAImpostorMode || ambassador.AmOwner))
            {
                var text =
                    $"<b>{player.Data.PlayerName} has now been retrained into {newRole.NiceName}!</b>";

                if (player.AmOwner)
                {
                    player.SetKillTimer(currentTime);
                    text =
                        $"<b>You have accepted your retrain into the {newRole.NiceName}!</b>";
                }
                var notif1 = Helpers.CreateAndShowNotification(text, Color.white, spr: newRole.RoleIconWhite ?? TOSRoleIcons.Ambassador.LoadAsset());

                notif1.Text.SetOutlineThickness(0.35f);
                notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
            }
        }
        else if (PlayerControl.LocalPlayer.IsImpostor() &&
                 (!OptionGroupSingleton<GeneralOptions>.Instance.FFAImpostorMode || ambassador.AmOwner))
        {
            var text =
                $"<b>{player.Data.PlayerName} has denied their retrain into {newRole.NiceName}!</b>";

            if (player.AmOwner)
            {
                text =
                    $"<b>You have denied your retrain into the {newRole.NiceName}!</b>";
            }
            var notif1 = Helpers.CreateAndShowNotification(text, Color.white, spr: newRole.RoleIconWhite ?? TOSRoleIcons.Ambassador.LoadAsset());

            notif1.Text.SetOutlineThickness(0.35f);
            notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.RetrainImpostor, SendImmediately = true)]
    private static void RpcRetrain(PlayerControl player, byte playerId = byte.MaxValue, ushort role = 0)
    {
        if (player.Data.Role is not AmbassadorRole ambassador)
        {
            Logger<TownOfSushiPlugin>.Error("RpcRetrain - Invalid ambassador");
            return;
        }

        if (playerId == byte.MaxValue || role == 0)
        {
            if (PlayerControl.LocalPlayer.IsImpostor() && ambassador.SelectedPlr != null &&
                (!OptionGroupSingleton<GeneralOptions>.Instance.FFAImpostorMode || player.AmOwner))
            {
                var text =
                    $"<b>Ambassador retraining for {ambassador.SelectedPlr.PlayerName} was cancelled</b>";
                var notif1 = Helpers.CreateAndShowNotification(text, Color.white, spr: TOSRoleIcons.Ambassador.LoadAsset());

                notif1.Text.SetOutlineThickness(0.35f);
                notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
                if (ambassador.Player.AmOwner)
                {
                    ambassador.meetingMenu.Actives[ambassador.SelectedPlr.PlayerId] = false;
                }
            }

            ambassador.SelectedPlr = null;
            ambassador.SelectedRole = null;
            return;
        }

        ambassador.SelectedPlr = GameData.Instance.GetPlayerById(playerId);
        ambassador.SelectedRole = RoleManager.Instance.GetRole((RoleTypes)role);
        if (PlayerControl.LocalPlayer.IsImpostor() &&
            (!OptionGroupSingleton<GeneralOptions>.Instance.FFAImpostorMode || player.AmOwner))
        {
            var text =
                MiscUtils.ColorString(TownOfSushiColors.Impostor, $"<b>The Ambassador has decided to retrain {ambassador.SelectedPlr.PlayerName} into {ambassador.SelectedRole.NiceName}</b>");
            if (ambassador.SelectedPlr.Object.AmOwner && player.AmOwner)
            {
                text =
                    MiscUtils.ColorString(TownOfSushiColors.Impostor, $"<b>You have decided to retrain yourself into {ambassador.SelectedRole.NiceName}</b>");
            }
            else if (ambassador.SelectedPlr.Object == player)
            {
                text =
                    MiscUtils.ColorString(TownOfSushiColors.Impostor, $"<b>The Ambassador has decided to retrain themselves into {ambassador.SelectedRole.NiceName}</b>");

            }
            else if (ambassador.SelectedPlr.Object.AmOwner)
            {
                text =
                    MiscUtils.ColorString(TownOfSushiColors.Impostor, $"<b>The Ambassador has decided to retrain you into {ambassador.SelectedRole.NiceName}</b>");

            }
            var notif1 = Helpers.CreateAndShowNotification(text, Color.white, spr: ambassador.SelectedRole.RoleIconWhite != null ? ambassador.SelectedRole.RoleIconWhite : TOSRoleIcons.Ambassador.LoadAsset());

            notif1.Text.SetOutlineThickness(0.35f);
            notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
        }
    }

    public string GetAdvancedDescription()
    {
        return $"The {RoleName} is an Impostor Power role that can retrain impostors into other impostor roles."
               + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Retrain (Meeting)",
            "Retrain yourself or other impostors into a role within their alignment. Impostor Killing roles can be turned into any non-Power roles, and Concealing/Support roles can become Concealing or Support roles.",
            TOSAssets.RetrainCleanSprite)
    ];
}