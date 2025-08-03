using AmongUs.GameOptions;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Modifiers.Impostor;
using TownOfSushi.Modules;
using TownOfUs.Modules.Components;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Roles.Crewmate;

using TownOfSushi.Roles.Impostor;
using TownOfSushi.Utilities;
using UnityEngine;
using TownOfSushi.Modifiers.Game;
using Reactor.Utilities;
using System.Globalization;

namespace TownOfSushi.Roles.Crewmate;

public sealed class VigilanteRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable
{
    public string RoleName => "Vigilante";
    public string RoleDescription => "Kill Impostors If You Can Guess Their Roles";
    public string RoleLongDescription => "Guess the roles of impostors mid-meeting to kill them!";
    public Color RoleColor => TownOfSushiColors.Vigilante;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateKilling;
    public DoomableType DoomHintType => DoomableType.Relentless;
    public bool IsPowerCrew => MaxKills > 0; // Always disable end game checks with a vigi running around
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TosRoleIcons.Vigilante,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Impostor),
    };

    public int MaxKills { get; set; }
    public int SafeShotsLeft { get; set; }

    private MeetingMenu meetingMenu;

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        MaxKills = (int)OptionGroupSingleton<VigilanteOptions>.Instance.VigilanteKills;
        SafeShotsLeft = (int)OptionGroupSingleton<VigilanteOptions>.Instance.MultiShots;
        if (Player.HasModifier<ImitatorCacheModifier>()) SafeShotsLeft = 0;

        if (Player.AmOwner)
        {
            meetingMenu = new MeetingMenu(
                this,
                ClickGuess,
                MeetingAbilityType.Click,
                TosAssets.Guess,
                null!,
                IsExempt);
        }
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        if (Player.AmOwner)
        {
            meetingMenu.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && MaxKills > 0 && !Player.HasModifier<JailedModifier>());
        }
    }

    public override void OnVotingComplete()
    {
        RoleBehaviourStubs.OnVotingComplete(this);

        if (Player.AmOwner)
        {
            meetingMenu.HideButtons();
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        if (Player.AmOwner)
        {
            meetingMenu?.Dispose();
            meetingMenu = null!;
        }
    }

    public void ClickGuess(PlayerVoteArea voteArea, MeetingHud meetingHud)
    {
        if (meetingHud.state == MeetingHud.VoteStates.Discussion)
        {
            return;
        }

        var player = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId).Object;

        var shapeMenu = GuesserMenu.Create();
        shapeMenu.Begin(IsRoleValid, ClickRoleHandle, IsModifierValid, ClickModifierHandle);

        void ClickRoleHandle(RoleBehaviour role)
        {
            var pickVictim = role.Role == player.Data.Role.Role;

            if (player.IsImpostor())
            {
                if (role.Role == player.Data.Role.Role && !player.HasModifier<TraitorCacheModifier>())
                    pickVictim = true;
                else if (role is TraitorRole && player.HasModifier<TraitorCacheModifier>())
                    pickVictim = true;
                else
                    pickVictim = false;
            }

            var victim = pickVictim ? player : Player;

            ClickHandler(victim);
        }

        void ClickModifierHandle(BaseModifier modifier)
        {
            var pickVictim = player.HasModifier(modifier.TypeId);
            var victim = pickVictim ? player : Player;

            ClickHandler(victim);
        }

        void ClickHandler(PlayerControl victim)
        {
            if (!OptionGroupSingleton<VigilanteOptions>.Instance.VigilanteMultiKill || MaxKills == 0 || victim == Player)
            {
                meetingMenu?.HideButtons();
            }

            if (victim == Player && SafeShotsLeft != 0)
            {
                SafeShotsLeft--;
                Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Impostor));

                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{TownOfSushiColors.Vigilante.ToTextColor()}Your Multi Shot has prevented you from dying this meeting! You have {SafeShotsLeft} safe shot(s) left!</color></b>", Color.white, spr: TosRoleIcons.Vigilante.LoadAsset());

                notif1.Text.SetOutlineThickness(0.35f);
                notif1.transform.localPosition = new Vector3(0f, 1f, -20f);

                shapeMenu.Close();

                return;
            }
            else
            {
                Player.RpcCustomMurder(victim, createDeadBody: false, teleportMurderer: false, showKillAnim: false, playKillSound: false);

                if (victim != Player)
                    meetingMenu?.HideSingle(victim.PlayerId);

                MaxKills--;
            }

            shapeMenu.Close();
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea?.TargetPlayerId == Player.PlayerId ||
        Player.Data.IsDead || voteArea!.AmDead ||
        voteArea.GetPlayer()?.HasModifier<JailedModifier>() == true ||
        voteArea.GetPlayer()?.Data.Role is MayorRole mayor && mayor.Revealed ||
        Player.IsLover() && voteArea.GetPlayer()?.IsLover() == true;
    }

    private static bool IsRoleValid(RoleBehaviour role)
    {
        if (role.IsDead)
        {
            return false;
        }

        var options = OptionGroupSingleton<VigilanteOptions>.Instance;
        var tosRole = role as ITownOfSushiRole;
        var unguessableRole = role as IUnguessable;

        if (unguessableRole != null && !unguessableRole.IsGuessable)
        {
            return false;
        }

        if (role.IsCrewmate() && !(PlayerControl.LocalPlayer.TryGetModifier<AllianceGameModifier>(out var allyMod) && !allyMod.GetsPunished))
        {
            return false;
        }
        else if (role.IsCrewmate())
        {
            return true;
        }

        if (role.IsImpostor())
        {
            return true;
        }

        if (tosRole?.RoleAlignment == RoleAlignment.NeutralBenign)
        {
            return options.VigilanteGuessNeutralBenign;
        }

        if (tosRole?.RoleAlignment == RoleAlignment.NeutralEvil)
        {
            return options.VigilanteGuessNeutralEvil;
        }

        if (tosRole?.RoleAlignment == RoleAlignment.NeutralKilling)
        {
            return options.VigilanteGuessNeutralKilling;
        }

        return false;
    }

    private static bool IsModifierValid(BaseModifier modifier)
    {
        var isValid = true;
        // This will remove modifiers that alter their chance/amount
        if ((modifier is TosGameModifier tosMod && (tosMod.CustomAmount <= 0 || tosMod.CustomChance <= 0)) ||
            (modifier is AllianceGameModifier allyMod && (allyMod.CustomAmount <= 0 || allyMod.CustomChance <= 0)) ||
            (modifier is UniversalGameModifier uniMod && (uniMod.CustomAmount <= 0 || uniMod.CustomChance <= 0)))
        {
            isValid = false;
        }
        
        if (!isValid)
        {
            return false;
        }

        if (OptionGroupSingleton<VigilanteOptions>.Instance.VigilanteGuessAlliances && modifier is AllianceGameModifier)
        {
            return true;
        }
        var impMod = modifier as TosGameModifier;
        if (impMod != null && (impMod.FactionType.ToDisplayString().Contains("Imp") || impMod.FactionType.ToDisplayString().Contains("Killer")) && !impMod.FactionType.ToDisplayString().Contains("Non"))
        {
            return OptionGroupSingleton<VigilanteOptions>.Instance.VigilanteGuessKillerMods;
        }

        return false;
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);
        if (PlayerControl.LocalPlayer.TryGetModifier<AllianceGameModifier>(out var allyMod) && !allyMod.GetsPunished)
        {
            stringB.AppendLine(CultureInfo.InvariantCulture, $"You can also guess Crewmates.");
        }
        if ((int)OptionGroupSingleton<VigilanteOptions>.Instance.MultiShots > 0)
        {
            var newText = SafeShotsLeft == 0 ? $"You have no more safe shots left." : $"{SafeShotsLeft} safe shot(s) left.";
            stringB.AppendLine(CultureInfo.InvariantCulture, $"{newText}");
        }

        return stringB;
    }

    public string GetAdvancedDescription()
    {
        return
            "The Vigilante is a Crewmate Killing role that can guess players roles in meetings. " +
            "If they guess correctly, the other player will die. If not, they will die. "
            + MiscUtils.AppendOptionsText(GetType());
    }
}
