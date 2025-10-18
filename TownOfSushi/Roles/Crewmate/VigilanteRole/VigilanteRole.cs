using System.Globalization;
using System.Text;
using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Networking;
using MiraAPI.Patches.Stubs;
using Reactor.Utilities;
using TownOfSushi.Events;
using TownOfSushi.Modifiers;
using TownOfSushi.Modifiers.Game;
using TownOfSushi.Modules;
using TownOfUs.Modules.Components;
using UnityEngine;
using TownOfSushi.Options;
using MiraAPI.Hud;
using Reactor.Networking.Attributes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class VigilanteRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITOSCrewRole, IWikiDiscoverable, IMysticClue
{
    private MeetingMenu meetingMenu;
    public override bool IsAffectedByComms => false;
    public bool HasMisfired { get; set; }
    public int MaxKills { get; set; }
    public int SafeShotsLeft { get; set; }
    public string RoleName => "Vigilante";
    public string RoleDescription => "Kill evildoers by any means possible";
    public string RoleLongDescription => "Guess the roles of impostors mid-meeting to kill them! Or shoot them mid round!";
    public MysticClueType MysticHintType => MysticClueType.Relentless;
    public Color RoleColor => TownOfSushiColors.Vigilante;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateKilling;
    public bool IsPowerCrew => !HasMisfired || MaxKills > 0;
    // Always disable end game checks if the Vigilante hasn't misfired or if they still have guesses left

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Vigilante,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Impostor)
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);
        var missType = OptionGroupSingleton<VigilanteOptions>.Instance.MisfireType;

        if (CustomButtonSingleton<VigilanteShootButton>.Instance.FailedShot)
        {
            stringB.AppendLine(CultureInfo.InvariantCulture, $"<b>You can no longer shoot.</b>");
        }
        else
        {
            switch (missType)
            {
                case MisfireOptions.Both:
                    stringB.AppendLine(CultureInfo.InvariantCulture, $"<b>Misfiring kills you and your target.</b>");
                    break;
                case MisfireOptions.Vigilante:
                    stringB.AppendLine(CultureInfo.InvariantCulture, $"<b>Misfiring will lead to suicide.</b>");
                    break;
                case MisfireOptions.Target:
                    stringB.AppendLine(CultureInfo.InvariantCulture,
                        $"<b>Misfiring will lead to your target's death,\nat the cost of your ability to kill. You can still guess.</b>");
                    break;
                default:
                    stringB.AppendLine(CultureInfo.InvariantCulture,
                        $"<b>Misfiring will prevent you from shooting again. You can still guess.</b>");
                    break;
            }
        }

        if (PlayerControl.LocalPlayer.TryGetModifier<AllianceGameModifier>(out var allyMod) && !allyMod.GetsPunished)
        {
            stringB.AppendLine(CultureInfo.InvariantCulture, $"<b>You may shoot without repercussions.</b>");
            stringB.AppendLine(CultureInfo.InvariantCulture, $"You can also guess Crewmates.");
        }

        if ((int)OptionGroupSingleton<VigilanteOptions>.Instance.MultiShots > 0)
        {
            var newText = SafeShotsLeft == 0
                ? "You have no more safe shots left."
                : $"{SafeShotsLeft} safe shot(s) left.";
            stringB.AppendLine(CultureInfo.InvariantCulture, $"{newText}");
        }

        return stringB;
    }

    public static void OnRoundStart()
    {
        CustomButtonSingleton<VigilanteShootButton>.Instance.Usable = true;
    }

    public string GetAdvancedDescription()
    {
        return
            "The Vigilante is a Crewmate Killing role that can guess players roles in meetings. " +
            "If they guess correctly, the other player will die. If not, they will die. "
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Shoot",
            "Shoot a player to kill them, misfiring if they aren't a Impostor or one of the other selected shootable factions",
            TOSCrewAssets.VigilanteShootSprite)
    ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        MaxKills = (int)OptionGroupSingleton<VigilanteOptions>.Instance.VigilanteKills;
        SafeShotsLeft = (int)OptionGroupSingleton<VigilanteOptions>.Instance.MultiShots;
        if (Player.HasModifier<ImitatorCacheModifier>())
        {
            SafeShotsLeft = 0;
        }

        if (Player.AmOwner)
        {
            meetingMenu = new MeetingMenu(
                this,
                ClickGuess,
                MeetingAbilityType.Click,
                TOSAssets.Guess,
                null!,
                IsExempt);
        }
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        if (Player.AmOwner)
        {
            meetingMenu.GenButtons(MeetingHud.Instance,
                Player.AmOwner && !Player.HasDied() && MaxKills > 0 && !Player.HasModifier<JailedModifier>());
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
                {
                    pickVictim = true;
                }
                else if (role is TraitorRole && player.HasModifier<TraitorCacheModifier>())
                {
                    pickVictim = true;
                }
                else
                {
                    pickVictim = false;
                }
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
            if (!OptionGroupSingleton<VigilanteOptions>.Instance.VigilanteMultiKill || MaxKills == 0 ||
                victim == Player)
            {
                meetingMenu?.HideButtons();
            }

            if (victim.TryGetModifier<OracleBlessedModifier>(out var oracleMod))
            {
                OracleRole.RpcOracleBlessNotify(oracleMod.Oracle, PlayerControl.LocalPlayer, victim);

                MeetingMenu.Instances.Do(x => x.HideSingle(victim.PlayerId));

                shapeMenu.Close();

                return;
            }

            if (victim == Player && SafeShotsLeft != 0)
            {
                SafeShotsLeft--;
                Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Impostor));

                var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Vigilante,
                    $"<b>Your Multi Shot has prevented you from dying this meeting! You have {SafeShotsLeft} safe shot(s) left!</color></b>"),
                    Color.white, spr: TOSRoleIcons.Vigilante.LoadAsset());

                
                notif1.AdjustNotification();

                shapeMenu.Close();

                return;
            }

            Player.RpcCustomMurder(victim, createDeadBody: false, teleportMurderer: false, showKillAnim: false,
                playKillSound: false);

            if (victim != Player)
            {
                meetingMenu?.HideSingle(victim.PlayerId);
                DeathHandlerModifier.RpcUpdateDeathHandler(victim, "Guessed", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse, $"By {Player.Data.PlayerName}", lockInfo: DeathHandlerOverride.SetTrue);
            }
            else
            {
                DeathHandlerModifier.RpcUpdateDeathHandler(victim, "Misguessed", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse, lockInfo: DeathHandlerOverride.SetTrue);
            }

            MaxKills--;

            shapeMenu.Close();
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea?.TargetPlayerId == Player.PlayerId ||
               Player.Data.IsDead || voteArea!.AmDead ||
               voteArea.GetPlayer()?.HasModifier<JailedModifier>() == true ||
               (voteArea.GetPlayer()?.Data.Role is MayorRole mayor && mayor.Revealed) ||
               (voteArea.GetPlayer()?.GetModifiers<RevealModifier>().Any(x => x.Visible && x.RevealRole) == true) ||
               (Player.IsLover() && voteArea.GetPlayer()?.IsLover() == true);
    }

    private static bool IsRoleValid(RoleBehaviour role)
    {
        if (role.IsDead)
        {
            return false;
        }

        var options = OptionGroupSingleton<VigilanteOptions>.Instance;
        var touRole = role as ITownOfSushiRole;
        var unguessableRole = role as IUnguessable;

        if (unguessableRole != null && !unguessableRole.IsGuessable)
        {
            return false;
        }

        if (role.IsCrewmate() && !(PlayerControl.LocalPlayer.TryGetModifier<AllianceGameModifier>(out var allyMod)
        && !allyMod.GetsPunished))
        {
            return false;
        }
        // If Vigilante is Egotist, then guessing investigative roles is based off assassin settings
        if (!OptionGroupSingleton<AssassinOptions>.Instance.AssassinGuessInvest && touRole?.RoleAlignment == RoleAlignment.CrewmateInvestigative)
        {
            return false;
        }

        if (role.IsCrewmate())
        {
            return true;
        }

        if (role.IsImpostor())
        {
            return true;
        }

        if (touRole?.RoleAlignment == RoleAlignment.NeutralBenign)
        {
            return options.VigilanteGuessNeutralBenign;
        }

        if (touRole?.RoleAlignment == RoleAlignment.NeutralEvil)
        {
            return options.VigilanteGuessNeutralEvil;
        }

        if (touRole?.RoleAlignment == RoleAlignment.NeutralKilling)
        {
            return true;
        }

        return false;
    }

    private static bool IsModifierValid(BaseModifier modifier)
    {
        var isValid = true;
        // This will remove modifiers that alter their chance/amount
        if ((modifier is TOSGameModifier touMod && (touMod.CustomAmount <= 0 || touMod.CustomChance <= 0)) ||
            (modifier is AllianceGameModifier allyMod && (allyMod.CustomAmount <= 0 || allyMod.CustomChance <= 0)) ||
            (modifier is UniversalGameModifier uniMod && (uniMod.CustomAmount <= 0 || uniMod.CustomChance <= 0)))
        {
            isValid = false;
        }

        if (!isValid)
        {
            return false;
        }

        if (OptionGroupSingleton<VigilanteOptions>.Instance.VigilanteGuessAlliances &&
            modifier is AllianceGameModifier)
        {
            return true;
        }

        var impMod = modifier as TOSGameModifier;
        if (impMod != null &&
            (impMod.FactionType.ToDisplayString().Contains("Imp") ||
             impMod.FactionType.ToDisplayString().Contains("Killer")) &&
            !impMod.FactionType.ToDisplayString().Contains("Non"))
        {
            return OptionGroupSingleton<VigilanteOptions>.Instance.VigilanteGuessKillerMods;
        }

        return false;
    }
    
    [MethodRpc((uint)TownOfSushiRpc.VigilanteMisfire, SendImmediately = true)]
    public static void RpcVigilanteMisfire(PlayerControl Vigilante)
    {
        if (Vigilante.Data.Role is not VigilanteRole role)
        {
            Logger<TownOfSushiPlugin>.Error("RpcVigilanteMisfire - Invalid Vigilante");
            return;
        }

        role.HasMisfired = true;

        if (GameHistory.PlayerStats.TryGetValue(Vigilante.PlayerId, out var stats))
        {
            stats.IncorrectKills += 1;
        }
    }
}