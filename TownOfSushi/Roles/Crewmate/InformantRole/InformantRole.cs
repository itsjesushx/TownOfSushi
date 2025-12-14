using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class InformantRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    private Dictionary<byte, ArrowBehaviour>? _snitchArrows;
    public bool CanSeeRealKiller { get; set; }
    public bool CanUseButton { get; set; }

    [HideFromIl2Cpp] 
    public ArrowBehaviour? InformantRevealArrow { get; private set; }
    private void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not InformantRole)
        {
            return;
        }
        if (!Player.AmOwner)
        {
            if (InformantRevealArrow != null)
            {
                InformantRevealArrow.target = Player.transform.position;
            }
            return;
        }
        if (Player.AmOwner && _snitchArrows != null && _snitchArrows.Count > 0)
        {
            foreach (var arrow in _snitchArrows.Values)
            {
                if (arrow != null)
                {
                    arrow.target = arrow.transform.parent.position;
                }
            }
        }
    }

    public MysticClueType MysticHintType => MysticClueType.Insight;
    public string LocaleKey => "Informant";
    public string RoleName => "Informant";
    public string RoleDescription => "Finish tasks to try and find the Killers";
    public string RoleLongDescription => "Finish your tasks to be able to track down Killers.";

    public string GetAdvancedDescription()
    {
        return "The Informant is a Crewmate Investigative role that needs to finish tasks to gain their ability. Once they finish tasks, the Informant can try use their ability to find the Impostors, but careful, as Crewmates might show up as evil too!"
               + MiscUtils.AppendOptionsText(GetType());
    }

    public Color RoleColor => TownOfSushiColors.Informant;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Informant,
        IntroSound = TOSAudio.ToppatIntroSound
    };

    public void CheckTaskRequirements()
    {
        var realTasks = Player.myTasks.ToArray()
            .Where(x => !PlayerTask.TaskIsEmergency(x) && !x.TryCast<ImportantTextTask>()).ToList();
        if (realTasks.Count <= 1)
        {
            return;
        }

        var completedTasks = realTasks.Count(t => t.IsComplete);
        if (completedTasks == realTasks.Count)
        {
            CanUseButton = true;
        }

        if (completedTasks == realTasks.Count && CanSeeRealKiller)
        {
            if (Player.AmOwner)
            {
                var text = "You can reveal the impostors temporarily!";
                if (Player.HasModifier<EgotistModifier>())
                {
                    text = "You can reveal the impostors temporarily, who can help your win condition!";
                }

                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{TownOfSushiColors.Informant.ToTextColor()}{text}</color></b>", Color.white,
                    new Vector3(0f, 1f, -20f),
                    spr: TOSRoleIcons.Informant.LoadAsset());

                notif1.AdjustNotification();
            }
            else if (IsTargetOfInformant(PlayerControl.LocalPlayer) && CanSeeRealKiller)
            {
                Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Informant, alpha: 0.5f));
                var text = "The Informant can temporarily know what you are now!";
                if (Player.HasModifier<EgotistModifier>())
                {
                    text = "The Informant can now help you as the Egotist!";
                }

                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{TownOfSushiColors.Informant.ToTextColor()}{text}</color></b>", Color.white,
                    new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Informant.LoadAsset());

                notif1.AdjustNotification();
            }
        }
    }

    public static bool IsTargetOfInformant(PlayerControl player)
    {
        if (player == null || player.Data == null || player.Data.Role == null)
        {
            return false;
        }

        return (player.IsImpostor() && !player.IsTraitor()) ||
               player.IsTraitor() || player.Is(RoleAlignment.NeutralKilling);
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        ClearArrows();
        // incase amne becomes snitch or smth
        CheckTaskRequirements();
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        ClearArrows();
    }
    [MethodRpc((uint)TownOfSushiRpc.FindKillers, SendImmediately = true)]
    public static void RpcFindKillers(PlayerControl player)
    {
        if (player.Data.Role is not InformantRole snitchRole)
        {
            return;
        }

        var possibleKillers = Helpers.GetAlivePlayers().Where(x => !x.HasDied() && IsTargetOfInformant(x)).ToList();
        var possibleNonKillers = Helpers.GetAlivePlayers().Where(x => !x.HasDied() && !IsTargetOfInformant(x)).ToList();

        bool showCorrectKiller = UnityEngine.Random.Range(1, 101) <= OptionGroupSingleton<InformantOptions>.Instance.RevealAccuracyPercentage;
        PlayerControl revealedTarget = null;

        if (showCorrectKiller && possibleKillers.Count > 0)
        {
            revealedTarget = possibleKillers[UnityEngine.Random.Range(0, possibleKillers.Count)];
            snitchRole.CanSeeRealKiller = true;
        }
        else if (possibleNonKillers.Count > 0)
        {
            revealedTarget = possibleNonKillers[UnityEngine.Random.Range(0, possibleNonKillers.Count)];
        }

        if (revealedTarget != null && player.AmOwner)
        {
            snitchRole._snitchArrows ??= new Dictionary<byte, ArrowBehaviour>();
            if (!snitchRole._snitchArrows.ContainsKey(revealedTarget.PlayerId))
            {
                snitchRole._snitchArrows.Add(revealedTarget.PlayerId, 
                    MiscUtils.CreateArrow(revealedTarget.transform, TownOfSushiColors.Impostor));
            }
        }
    }

    public void RemoveArrowForPlayer(byte playerId)
    {
        if (_snitchArrows != null && _snitchArrows.TryGetValue(playerId, out var arrow))
        {
            arrow.gameObject.Destroy();
            _snitchArrows.Remove(playerId);
        }
    }

    private void CreateRevealingArrow()
    {
        if (Player == null) return;
        if (Player.AmOwner) return;
        if (!IsTargetOfInformant(PlayerControl.LocalPlayer)) return;

        if (InformantRevealArrow != null)
        {
            return;
        }

        Player.AddModifier<InformantPlayerRevealModifier>(
            RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<InformantRole>()));
        PlayerNameColor.Set(Player);
        Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Informant, alpha: 0.5f));

        // Create an arrow pointing to the Informant
        InformantRevealArrow = MiscUtils.CreateArrow(Player.transform, TownOfSushiColors.Informant);
    }

    private void CreateInformantArrows()
    {
        // Only the Informant owner should create and hold arrows to potential killers
        if (!Player.AmOwner) return;

        if (_snitchArrows != null)
        {
            return;
        }

        Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Informant, alpha: 0.5f));
        _snitchArrows = new Dictionary<byte, ArrowBehaviour>();
        var imps = Helpers.GetAlivePlayers().Where(plr => plr.Data.Role.IsImpostor && !plr.IsTraitor());
        var traitor = Helpers.GetAlivePlayers().FirstOrDefault(plr => plr.IsTraitor());
        imps.ToList().ForEach(imp =>
        {
            _snitchArrows.Add(imp.PlayerId, MiscUtils.CreateArrow(imp.transform, TownOfSushiColors.Impostor));
            PlayerNameColor.Set(imp);
            imp.AddModifier<InformantTargetRevealModifier>();
        });

        if (OptionGroupSingleton<InformantOptions>.Instance.InformantSeesTraitor && traitor != null && !_snitchArrows.ContainsKey(traitor.PlayerId))
        {
            _snitchArrows.Add(traitor.PlayerId, MiscUtils.CreateArrow(traitor.transform, TownOfSushiColors.Impostor));
            traitor.AddModifier<InformantTargetRevealModifier>();
        }

        var neutrals = MiscUtils.GetRoles(RoleAlignment.NeutralKilling)
            .Where(role => !role.Player.Data.IsDead && !role.Player.Data.Disconnected);
        neutrals.ToList().ForEach(neutral =>
        {
            if (!_snitchArrows.ContainsKey(neutral.Player.PlayerId))
            {
                _snitchArrows.Add(neutral.Player.PlayerId, MiscUtils.CreateArrow(neutral.Player.transform, TownOfSushiColors.Neutral));
                neutral.Player.AddModifier<InformantTargetRevealModifier>();
            }
        });
    }

    public void ClearArrows()
    {
        // Clear only Informant-owner arrows here
        if (_snitchArrows != null && _snitchArrows.Count > 0)
        {
            _snitchArrows.ToList().ForEach(arrow => arrow.Value.gameObject.Destroy());
            _snitchArrows.Clear();
            _snitchArrows = null;
        }

        // Clear reveal arrow for target
        if (InformantRevealArrow != null)
        {
            InformantRevealArrow.gameObject.Destroy();
            InformantRevealArrow = null;
        }

        // Remove reveal modifiers
        ModifierUtils.GetActiveModifiers<InformantTargetRevealModifier>()
            .Do(x => x.ModifierComponent?.RemoveModifier(x));
        ModifierUtils.GetActiveModifiers<InformantPlayerRevealModifier>().Do(x => x.ModifierComponent?.RemoveModifier(x));
    }

    public void AddInformantTraitorArrows()
    {
        if (CanSeeRealKiller && Player.AmOwner)
        {
            var traitor = Helpers.GetAlivePlayers().FirstOrDefault(plr => plr.IsTraitor());
            if (_snitchArrows == null || traitor == null ||
                (_snitchArrows.TryGetValue(traitor.PlayerId, out var arrow) && arrow != null))
            {
                return;
            }

            if (OptionGroupSingleton<InformantOptions>.Instance.InformantSeesTraitor && traitor != null)
            {
                _snitchArrows.Add(traitor.PlayerId, MiscUtils.CreateArrow(traitor.transform, TownOfSushiColors.Impostor));
                Player.AddModifier<InformantTargetRevealModifier>();
            }
        }
    }
}