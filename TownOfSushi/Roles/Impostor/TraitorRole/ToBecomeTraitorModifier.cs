using AmongUs.GameOptions;
using Reactor.Networking.Attributes;
using TownOfSushi.Modifiers;
using TownOfSushi.Options;
using UnityEngine;
using Random = System.Random;

namespace TownOfSushi.Roles.Impostor;

public sealed class ToBecomeTraitorModifier : ExcludedGameModifier, IAssignableTargets
{
    public override string ModifierName => "Possible Traitor";
    public override bool HideOnUi => true;

    public int Priority { get; set; } = 3;
    public override Color FreeplayFileColor => new Color32(255, 25, 25, 255);

    public void AssignTargets()
    {
        if (GameOptionsManager.Instance.CurrentGameOptions.RoleOptions
            .GetNumPerGame((RoleTypes)RoleId.Get<TraitorRole>()) == 0)
        {
            return;
        }

        Random rnd = new();
        var chance = rnd.Next(1, 101);

        if (chance <=
            GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetChancePerGame(
                (RoleTypes)RoleId.Get<TraitorRole>()))
        {
            var filtered = PlayerControl.AllPlayerControls.ToArray()
                .Where(x => x.Is(ModdedRoleTeams.Crewmate) &&
                            !x.Data.IsDead &&
                            !x.Data.Disconnected &&
                            !x.HasModifier<ExecutionerTargetModifier>() &&
                            !x.HasModifier<EgotistModifier>() &&
                            x.Data.Role is not MayorRole).ToList();

            if (filtered.Count == 0)
            {
                return;
            }

            var randomTarget = filtered[rnd.Next(0, filtered.Count)];

            randomTarget.RpcAddModifier<ToBecomeTraitorModifier>();
        }
    }

    public override int GetAmountPerGame()
    {
        return 0;
    }

    public override int GetAssignmentChance()
    {
        return 0;
    }

    public void Clear()
    {
        AssignTargets();
        ModifierComponent?.RemoveModifier(this);
    }

    [MethodRpc((uint)TownOfSushiRpc.SetTraitor, SendImmediately = true)]
    public static void RpcSetTraitor(PlayerControl player)
    {
        if (!player.HasModifier<ToBecomeTraitorModifier>())
        {
            return;
        }

        player.ChangeRole(RoleId.Get<TraitorRole>());
        player.RemoveModifier<ToBecomeTraitorModifier>();

        if (OptionGroupSingleton<AssassinOptions>.Instance.TraitorCanAssassin)
        {
            player.AddModifier<ImpostorAssassinModifier>();
        }
        CustomRoleUtils.GetActiveRolesOfType<InformantRole>().ToList()
            .ForEach(snitch => snitch.AddInformantTraitorArrows());
    }
}