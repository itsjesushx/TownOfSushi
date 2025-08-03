using AmongUs.GameOptions;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using TownOfSushi.Modifiers.Game.Impostor;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Options;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Roles.Impostor;
using TownOfSushi.Utilities;

namespace TownOfSushi.Modifiers.Crewmate;

public sealed class ToBecomeTraitorModifier : ExcludedGameModifier, IAssignableTargets
{
    public override string ModifierName => "Possible Traitor";
    public override bool HideOnUi => true;
    public override int GetAmountPerGame() => 0;
    public override int GetAssignmentChance() => 0;
    public int Priority { get; set; } = 3;

    public void Clear()
    {
        AssignTargets();
        ModifierComponent?.RemoveModifier(this);
    }

    public void AssignTargets()
    {
        Random rnd = new();
        var chance = rnd.Next(0, 100);

        if (chance <= GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetChancePerGame((RoleTypes)RoleId.Get<TraitorRole>()))
        {
            var filtered = PlayerControl.AllPlayerControls.ToArray()
                .Where(x => x.Is(ModdedRoleTeams.Crewmate) && 
                                    !x.Data.IsDead && 
                                    !x.Data.Disconnected && 
                                    !x.HasModifier<ExecutionerTargetModifier>() &&
                                    x.Data.Role is not MayorRole).ToList();

            Random rndIndex = new();
            if (filtered.Count == 0) return;
            var randomTarget = filtered[rndIndex.Next(0, filtered.Count)];

            randomTarget.RpcAddModifier<ToBecomeTraitorModifier>();
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.SetTraitor, SendImmediately = true)]
    public static void RpcSetTraitor(PlayerControl player)
    {
        if (!player.HasModifier<ToBecomeTraitorModifier>()) return;

        player.ChangeRole(RoleId.Get<TraitorRole>());
        player.RemoveModifier<ToBecomeTraitorModifier>();

        if (OptionGroupSingleton<AssassinOptions>.Instance.TraitorCanAssassin)
        {
            player.AddModifier<ImpostorAssassinModifier>();
        }
        
        if (SnitchRole.IsTargetOfSnitch(player))
        {
            CustomRoleUtils.GetActiveRolesOfType<SnitchRole>().ToList().ForEach(snitch => snitch.AddSnitchTraitorArrows());
        }
    }
}
