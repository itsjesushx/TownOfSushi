using System.Text;
using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Modules;
using TownOfUs.Modules.Components;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Roles.Neutral;

using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class DoomsayerRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable, ICrewVariant
{
    public string RoleName => "Doomsayer";
    public string RoleDescription => "Guess People's Roles To Win!";
    public string RoleLongDescription => $"Win by guessing the roles of {(int)OptionGroupSingleton<DoomsayerOptions>.Instance.DoomsayerGuessesToWin} players";
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<VigilanteRole>());
    public Color RoleColor => TownOfSushiColors.Doomsayer;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralEvil;
    public DoomableType DoomHintType => DoomableType.Insight;
    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TosAudio.QuestionSound,
        Icon = TosRoleIcons.Doomsayer,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };

    public int NumberOfGuesses { get; set; }
    public int IncorrectGuesses { get; set; }
    public bool AllGuessesCorrect { get; set; }
    [HideFromIl2Cpp]
    public List<PlayerControl> AllVictims { get; } = [];

    private MeetingMenu meetingMenu;
    public bool MetWinCon => AllGuessesCorrect;

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

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
            meetingMenu.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && !Player.HasModifier<JailedModifier>());

            if (OptionGroupSingleton<DoomsayerOptions>.Instance.DoomsayerGuessAllAtOnce) NumberOfGuesses = 0;
            IncorrectGuesses = 0;
            AllVictims.Clear();
            AllGuessesCorrect = false;
        }

        GenerateReport();
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

    private void GenerateReport()
    {
        Logger<TownOfSushiPlugin>.Info($"Generating Doomsayer report");

        var reportBuilder = new StringBuilder();

        if (Player == null) return;
        if (!Player.AmOwner) return;

        foreach (var player in GameData.Instance.AllPlayers.ToArray().Where(x => !x.Object.HasDied() && x.Object.HasModifier<DoomsayerObservedModifier>()))
        {
            var role = player.Object.Data.Role;
            var doomableRole = role as IDoomable;
            var hintType = DoomableType.Default;
            var cachedMod = player.Object.GetModifiers<BaseModifier>().FirstOrDefault(x => x is ICachedRole) as ICachedRole;
            if (cachedMod != null)
            {
                role = cachedMod.CachedRole;
                doomableRole = role as IDoomable;
            }
            var unguessableMod = player.Object.GetModifiers<BaseModifier>().FirstOrDefault(x => x is IUnguessable) as IUnguessable;
            if (unguessableMod != null)
            {
                role = unguessableMod.AppearAs;
                doomableRole = role as IDoomable;
            }

            if (doomableRole != null)
            {
                hintType = doomableRole.DoomHintType;
            }

            if (hintType == DoomableType.Default) continue;

            switch (hintType)
            {
                case DoomableType.Perception:
                    reportBuilder.AppendLine(TownOfSushiPlugin.Culture, $"You observe that {player.PlayerName} has an altered perception of reality\n");
                    break;
                case DoomableType.Insight:
                    reportBuilder.AppendLine(TownOfSushiPlugin.Culture, $"You observe that {player.PlayerName} has an insight for private information\n");
                    break;
                case DoomableType.Death:
                    reportBuilder.AppendLine(TownOfSushiPlugin.Culture, $"You observe that {player.PlayerName} has an unusual obsession with dead bodies\n");
                    break;
                case DoomableType.Hunter:
                    reportBuilder.AppendLine(TownOfSushiPlugin.Culture, $"You observe that {player.PlayerName} is well trained in hunting down prey\n");
                    break;
                case DoomableType.Fearmonger:
                    reportBuilder.AppendLine(TownOfSushiPlugin.Culture, $"You observe that {player.PlayerName} spreads fear amonst the group\n");
                    break;
                case DoomableType.Protective:
                    reportBuilder.AppendLine(TownOfSushiPlugin.Culture, $"You observe that {player.PlayerName} hides to guard themself or others\n");
                    break;
                case DoomableType.Trickster:
                    reportBuilder.AppendLine(TownOfSushiPlugin.Culture, $"You observe that {player.PlayerName} has a trick up their sleeve\n");
                    break;
                case DoomableType.Relentless:
                    reportBuilder.AppendLine(TownOfSushiPlugin.Culture, $"You observe that {player.PlayerName} is capable of performing relentless attacks\n");
                    break;
            }
            var roles = MiscUtils.AllRoles.Where(x => x is IDoomable doomRole && doomRole.DoomHintType == hintType && x is not IUnguessable).OrderBy(x => x.NiceName).ToList();
            var lastRole = roles[roles.Count - 1];
            roles.Remove(roles[roles.Count - 1]);

            if (roles.Count != 0)
            {
                reportBuilder.Append(TownOfSushiPlugin.Culture, $"(");
                foreach (var role2 in roles)
                {
                    reportBuilder.Append(TownOfSushiPlugin.Culture, $"{role2.NiceName}, ");
                }
                reportBuilder = reportBuilder.Remove(reportBuilder.Length - 2, 2);
                reportBuilder.Append(TownOfSushiPlugin.Culture, $" or {lastRole.NiceName})");
            }

            player.Object.RemoveModifier<DoomsayerObservedModifier>();
        }

        var report = reportBuilder.ToString();

        if (HudManager.Instance && report.Length > 0)
        {
            var title = $"<color=#{TownOfSushiColors.Doomsayer.ToHtmlStringRGBA()}>Doomsayer Report</color>";
            MiscUtils.AddFakeChat(Player.Data, title, report, false, true);
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
        return AllGuessesCorrect;
    }

    public bool WinConditionMet()
    {
        if (Player.HasDied()) return false;
        if (OptionGroupSingleton<DoomsayerOptions>.Instance.DoomWin is not DoomWinOptions.EndsGame) return false;

        return AllGuessesCorrect;
    }

    public void ClickGuess(PlayerVoteArea voteArea, MeetingHud meetingHud)
    {
        if (meetingHud.state == MeetingHud.VoteStates.Discussion)
        {
            return;
        }

        var player = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId).Object;

        var shapeMenu = GuesserMenu.Create();
        shapeMenu.Begin(IsRoleValid, ClickRoleHandle);

        void ClickRoleHandle(RoleBehaviour role)
        {
            var realRole = player.Data.Role;

            var cachedMod = player.GetModifiers<BaseModifier>().FirstOrDefault(x => x is ICachedRole) as ICachedRole;
            if (cachedMod != null)
            {
                realRole = cachedMod.CachedRole;
            }

            var pickVictim = role.Role == realRole.Role;
            var victim = pickVictim ? player : Player;

            ClickHandler(victim, voteArea.TargetPlayerId);
        }

        void ClickHandler(PlayerControl victim, byte targetId)
        {
            var opts = OptionGroupSingleton<DoomsayerOptions>.Instance;

            if (opts.DoomsayerGuessAllAtOnce) NumberOfGuesses++;
            meetingMenu?.HideSingle(targetId);

            var playersAlive = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && !x.IsJailed() && x != Player);

            if (victim == Player)
            {
                IncorrectGuesses++;
                if (!opts.DoomsayerGuessAllAtOnce)
                {
                    Coroutines.Start(MiscUtils.CoFlash(Color.red));
                    meetingMenu?.HideButtons();
                    shapeMenu.Close();
                    return;
                }
            }
            else if (!opts.DoomsayerGuessAllAtOnce)
            {
                Coroutines.Start(MiscUtils.CoFlash(Color.green));
                NumberOfGuesses++;
            }
            else
            {
                AllVictims.Add(victim);
            }
            if (((NumberOfGuesses < 2 && playersAlive < 3) || (NumberOfGuesses < (int)opts.DoomsayerGuessesToWin && playersAlive > 2)) && opts.DoomsayerGuessAllAtOnce)
            {
                shapeMenu.Close();
                return;
            }

            if (IncorrectGuesses > 0 && opts.DoomsayerGuessAllAtOnce)
            {
                var text = (NumberOfGuesses - AllVictims.Count) == 1 ? $"<b>Only one guess was incorrect!</b>" : $"<b>{NumberOfGuesses - AllVictims.Count} guesses were incorrect.</b>";
                var notif1 = Helpers.CreateAndShowNotification(
                    text, Color.white, spr: TosRoleIcons.Doomsayer.LoadAsset());

                notif1.Text.SetOutlineThickness(0.35f);
                notif1.transform.localPosition = new Vector3(0f, 1f, -20f);

                Coroutines.Start(MiscUtils.CoFlash(Color.red));
            }
            else if (opts.DoomsayerGuessAllAtOnce)
            {
                if (opts.DoomsayerKillOnlyLast) Player.RpcCustomMurder(victim, createDeadBody: false, teleportMurderer: false);
                else AllVictims.Do(victim => Player.RpcCustomMurder(victim, createDeadBody: false, teleportMurderer: false));
            }
            else
            {
                // no incorrect guesses so this should be the target not the Doomsayer
                Player.RpcCustomMurder(victim, createDeadBody: false, teleportMurderer: false);
            }

            if (opts.DoomsayerGuessAllAtOnce || NumberOfGuesses == (int)opts.DoomsayerGuessesToWin) meetingMenu?.HideButtons();

            shapeMenu.Close();
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea.TargetPlayerId == Player.PlayerId ||
        Player.Data.IsDead || voteArea.AmDead ||
        voteArea.GetPlayer()?.HasModifier<JailedModifier>() == true ||
        voteArea.GetPlayer()?.Data.Role is MayorRole mayor && mayor.Revealed ||
        Player.IsLover() && voteArea.GetPlayer()?.IsLover() == true;
    }

    private static bool IsRoleValid(RoleBehaviour role)
    {
        var unguessableRole = role as IUnguessable;
        if (role.IsDead || role is IGhostRole || (unguessableRole != null && !unguessableRole.IsGuessable))
        {
            return false;
        }

        return true;
    }

    [MethodRpc((uint)TownOfSushiRpc.DoomsayerWin, SendImmediately = true)]
    public static void RpcDoomsayerWin(PlayerControl player)
    {
        if (player.Data.Role is not DoomsayerRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcDoomsayerWin - Invalid Doomsayer");
            return;
        }

        var doom = player.GetRole<DoomsayerRole>();
        doom!.AllGuessesCorrect = true;

        if (GameHistory.PlayerStats.TryGetValue(player.PlayerId, out var stats))
        {
            stats.CorrectAssassinKills++;
        }
    }

    public string GetAdvancedDescription()
    {
        return $"The Doomsayer is a Neutral Evil role that wins by guessing {(int)OptionGroupSingleton<DoomsayerOptions>.Instance.DoomsayerGuessesToWin} players' roles." + (OptionGroupSingleton<DoomsayerOptions>.Instance.CantObserve ? string.Empty : " They may observe players to get a hint of what their roles are the following meeting.") + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Observe",
            "Observe a player, gaining a hint in the next meeting what their role could be.",
            TosNeutAssets.Observe)
    ];
}
