using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Modules;
using TownOfSushi.Options;

using TownOfSushi.Utilities;
using UnityEngine;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Modifiers.Game.Impostor;
using TownOfSushi.Modifiers.Game.Neutral;
using TownOfSushi.Events.TosEvents;
using MiraAPI.Events;

namespace TownOfSushi.Roles.Neutral;

public sealed class AmnesiacRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable, ICrewVariant
{
    public string RoleName => "Amnesiac";
    public string RoleDescription => "Remember A Role Of A Deceased Player";
    public string RoleLongDescription => "Find a dead body to remember and become their role";
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<MysticRole>());
    public Color RoleColor => TownOfSushiColors.Amnesiac;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralBenign;
    public DoomableType DoomHintType => DoomableType.Death;
    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TosAudio.MediumIntroSound,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = TosRoleIcons.Amnesiac,
    };

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        if (Player.HasModifier<AmnesiacArrowModifier>())
        {
            var mods = Player.GetModifiers<AmnesiacArrowModifier>();

            mods.Do([HideFromIl2Cpp] (x) => Player.RemoveModifier(x.UniqueId));
        }
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return false;
    }

    [MethodRpc((uint)TownOfSushiRpc.Remember, SendImmediately = true)]
    public static void RpcRemember(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not AmnesiacRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcRemember - Invalid amnesiac");
            return;
        }

        var TosAbilityEvent = new TosAbilityEvent(AbilityType.AmnesiacPreRemember, player, target);
        MiraEventManager.InvokeEvent(TosAbilityEvent);

        var roleWhenAlive = target.GetRoleWhenAlive();

        player.ChangeRole((ushort)roleWhenAlive.Role);
        if (player.Data.Role is InquisitorRole inquis)
        {
            inquis.Targets = ModifierUtils.GetPlayersWithModifier<InquisitorHereticModifier>().ToList();
            inquis.TargetRoles = ModifierUtils.GetActiveModifiers<InquisitorHereticModifier>().Select(x => x.TargetRole).OrderBy(x => x.NiceName).ToList();
        }
        if (player.Data.Role is MayorRole mayor)
            {
                mayor.Revealed = true;
            }

        if (target.IsImpostor() && OptionGroupSingleton<AssassinOptions>.Instance.AmneTurnImpAssassin)
        {
            player.AddModifier<ImpostorAssassinModifier>();
        }
        else if (target.IsNeutral() && target.Is(RoleAlignment.NeutralKilling) && OptionGroupSingleton<AssassinOptions>.Instance.AmneTurnNeutAssassin)
        {
            player.AddModifier<NeutralKillerAssassinModifier>();
        }
        
        if (target.Data.Role is not VampireRole && target.Data.Role.MaxCount <= PlayerControl.AllPlayerControls.ToArray().Count(x => x.Data.Role.Role == target.Data.Role.Role))
        {
            if (target.IsCrewmate())
            {
                target.ChangeRole((ushort)RoleTypes.Crewmate);
            }
            else if (target.IsImpostor())
            {
                target.ChangeRole((ushort)RoleTypes.Impostor);
            }
            else if (target.IsNeutral() && player.Data.Role is ITownOfSushiRole tosRole)
            {
                switch (tosRole.RoleAlignment)
                {
                    default:
                        target.ChangeRole(RoleId.Get<SurvivorRole>());
                        break;
                    case RoleAlignment.NeutralEvil:
                        target.ChangeRole(RoleId.Get<JesterRole>());
                        break;
                    case RoleAlignment.NeutralKilling:
                        target.ChangeRole(RoleId.Get<MercenaryRole>());
                        player.AddModifier<MercenaryBribedModifier>(target)!.alerted = true;
                        break;
                }
            }
            else
            {
                target.ChangeRole(RoleId.Get<SurvivorRole>());
            }
        }
        var TosAbilityEvent2 = new TosAbilityEvent(AbilityType.AmnesiacPostRemember, player, target);
        MiraEventManager.InvokeEvent(TosAbilityEvent2);
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return "The Amnesiac is a Neutral Benign role that gains access to a new role from remembering a dead body’s role. Use the role you remember to win the game." + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Remember",
            "Remember the role of a dead body. If the dead body's role is a unique role, you will remember the base faction's role instead.",
            TosNeutAssets.RememberButtonSprite)    
    ];
}
