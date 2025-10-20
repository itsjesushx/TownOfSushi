using System.Text;
using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modules;
using TownOfSushi.Options;
using UnityEngine;
using MiraAPI.Hud;
using MiraAPI.Patches.Stubs;
using TownOfSushi.Modifiers;


namespace TownOfSushi.Roles.Neutral;

public sealed class AmnesiacRole(IntPtr cppPtr)
    : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, ICrewVariant, IGuessable, IMysticClue
{
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<MysticRole>());
    public string RoleName => "Amnesiac";
    public string RoleDescription => "Remember a role after a meeting or survive the game to win.";
    public string RoleLongDescription => "Remember a role after a meeting or survive until the end to win.";
    public MysticClueType MysticHintType => MysticClueType.Death;
    public Color RoleColor => TownOfSushiColors.Amnesiac;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralBenign;
    // This is so the role can be guessed without requiring it to be enabled normally
    public bool CanBeGuessed =>
        (MiscUtils.GetPotentialRoles()
             .Contains(RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<GuardianAngelTOSRole>())) &&
         OptionGroupSingleton<GuardianAngelOptions>.Instance.OnTargetDeath is BecomeOptions.Amnesiac)
        || (MiscUtils.GetPotentialRoles()
                .Contains(RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<ExecutionerRole>())) &&
            OptionGroupSingleton<ExecutionerOptions>.Instance.OnTargetDeath is BecomeOptions.Amnesiac)
        || (MiscUtils.GetPotentialRoles()
                .Contains(RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<RomanticRole>())) &&
            OptionGroupSingleton<RomanticOptions>.Instance.OnTargetDeath is BecomeOptions.Amnesiac);

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TOSAudio.MediumIntroSound,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = TOSRoleIcons.Amnesiac
    };

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (Player.AmOwner && OptionGroupSingleton<AmnesiacOptions>.Instance.ScatterOn)
        {
            Player.AddModifier<ScatterModifier>(OptionGroupSingleton<AmnesiacOptions>.Instance.ScatterTimer);
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        if (Player.AmOwner && OptionGroupSingleton<AmnesiacOptions>.Instance.ScatterOn)
        {
            Player.RemoveModifier<ScatterModifier>();
        }
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Neutral Benign role that gains access to a shapeshift panel after each exile scene (when the meeting ends) to pick a player and gain their role. Use the role you remember to win the game. Aside from this, the Amnesiac can also just survive till the end of the game to win with anyone." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Remember",
            "Remember the role of a dead player. If the dead player's role is a unique role, you will remember the base faction's role instead.",
            TOSNeutAssets.RememberButtonSprite),
        new("Vest",
            "Put on a Vest protecting you from attacks.",
            TOSNeutAssets.VestSprite)
    ];

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return !Player.HasDied();
    }

    [MethodRpc((uint)TownOfSushiRpc.Remember, SendImmediately = true)]
    public static void RpcRemember(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not AmnesiacRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcRemember - Invalid amnesiac");
            return;
        }

        var roleWhenAlive = target.GetRoleWhenAlive();

        if (roleWhenAlive is AmnesiacRole)
        {
            if (player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{target.CachedPlayerData.PlayerName} was an " + MiscUtils.ColorString(TownOfSushiColors.Amnesiac, $"<b>Amnesiac</b>") + ", so their role cannot be picked up.</b>",
                    Color.white, new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Amnesiac.LoadAsset());
                notif1.AdjustNotification();
            }

            return;
        }

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.AmnesiacPreRemember, player, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);

        player.ChangeRole((ushort)roleWhenAlive.Role);
        if (player.IsImpostor())
        {
            player.SetKillTimer(GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown));
        }
        foreach (var button in CustomButtonManager.Buttons.Where(x => x.Enabled(player.Data.Role)))
        {
            button.SetTimer(OptionGroupSingleton<GeneralOptions>.Instance.GameStartCd);
        }

        if (player.Data.Role is HitmanRole)
        {
            player.ChangeRole(RoleId.Get<AgentRole>());
        }
        if (player.Data.Role is InquisitorRole inquis)
        {
            if (player.HasModifier<InquisitorHereticModifier>())
            {
                player.RemoveModifier<InquisitorHereticModifier>();
            }
            inquis.Targets = ModifierUtils.GetPlayersWithModifier<InquisitorHereticModifier>().ToList();
            inquis.TargetRoles = ModifierUtils.GetActiveModifiers<InquisitorHereticModifier>().Select(x => x.TargetRole)
                .OrderBy(x => x.NiceName).ToList();
        }
        else if (player.Data.Role is PlaguebearerRole || player.Data.Role is PestilenceRole)
        {
            ModifierUtils.GetActiveModifiers<PlaguebearerInfectedModifier>().Do(x => x.ModifierComponent?.RemoveModifier(x));
        }
        else if (player.Data.Role is ArsonistRole)
        {
            ModifierUtils.GetActiveModifiers<ArsonistDousedModifier>().Do(x => x.ModifierComponent?.RemoveModifier(x));
        }
        else if (player.Data.Role is MayorRole mayor)
        {
            mayor.Revealed = false;
        }
        else if (player.Data.Role is GuardianAngelTOSRole ga)
        {
            var gaTarget = ModifierUtils.GetPlayersWithModifier<GuardianAngelTargetModifier>().FirstOrDefault(x => x.PlayerId == target.PlayerId);

            if (gaTarget != null && gaTarget.TryGetModifier<GuardianAngelTargetModifier>(out var gaMod))
            {
                ga.Target = gaTarget;
                gaMod.OwnerId = player.PlayerId;
            }
        }
        else if (player.Data.Role is ExecutionerRole exe)
        {
            var exeTarget = ModifierUtils.GetPlayersWithModifier<ExecutionerTargetModifier>().FirstOrDefault(x => x.PlayerId == target.PlayerId);

            if (exeTarget != null && exeTarget.TryGetModifier<ExecutionerTargetModifier>(out var exeMod))
            {
                exe.Target = exeTarget;
                exeMod.OwnerId = player.PlayerId;
            }
        }
        else if (player.Data.Role is VampireRole)
        {
            if (target.HasModifier<VampireBittenModifier>())
            {
                // Makes the amne stay with the bitten modifier
                player.AddModifier<VampireBittenModifier>();
            }
            else
            {
                // Makes the og vampire a bitten vampire so to speak, yes it makes it more confusing, but that's how it is, deal with it - Atony
                target.AddModifier<VampireBittenModifier>();
            }
        }

        if (player.AmOwner)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>You remembered that you were like {target.Data.PlayerName}, the {player.Data.Role.TeamColor.ToTextColor()}{player.Data.Role.NiceName}</color>.</b>",
                Color.white, new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Amnesiac.LoadAsset());
            notif1.AdjustNotification();
        }
        else
        {
            var notif1 = Helpers.CreateAndShowNotification(
                MiscUtils.ColorString(TownOfSushiColors.Amnesiac, $"<b>The Amnesiac has remembered a role from the death!</color>.</b>"),
                Color.white, new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Amnesiac.LoadAsset());
            notif1.AdjustNotification();
        }

        if (roleWhenAlive is not VampireRole && (roleWhenAlive.MaxCount <= 1 || (roleWhenAlive.MaxCount <= PlayerControl.AllPlayerControls
                .ToArray().Count(x => x.Data.Role.Role == roleWhenAlive.Role))))
        {
            if (target.IsCrewmate())
            {
                target.ChangeRole((ushort)RoleTypes.Crewmate);
            }
            else if (target.IsImpostor())
            {
                target.ChangeRole((ushort)RoleTypes.Impostor);
            }
            else
            {
                target.ChangeRole(RoleId.Get<JesterRole>());
            }
        }

        if (player.IsImpostor() && OptionGroupSingleton<AssassinOptions>.Instance.AmneTurnImpAssassin)
        {
            player.AddModifier<ImpostorAssassinModifier>();
        }
        else if (player.IsNeutral() && player.Is(RoleAlignment.NeutralKilling) &&
                 OptionGroupSingleton<AssassinOptions>.Instance.AmneTurnNeutAssassin)
        {
            player.AddModifier<NeutralKillerAssassinModifier>();
        }
        
        var TOSAbilityEvent2 = new TOSAbilityEvent(AbilityType.AmnesiacPostRemember, player, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent2);
    }
}