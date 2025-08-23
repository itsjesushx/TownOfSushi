using HarmonyLib;
using Reactor.Utilities;
using TownOfSushi.Buttons;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TownOfSushi.Roles.Crewmate;

public sealed class OracleConfessButton : TownOfSushiRoleButton<OracleRole, PlayerControl>
{
    public override string Name => "Confess";
    public override Color TextOutlineColor => TownOfSushiColors.Oracle;
    public override string Keybind => Keybinds.PrimaryAction;
    public override float Cooldown => OptionGroupSingleton<OracleOptions>.Instance.ConfessCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.ConfessSprite;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<OracleConfessModifier>());
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error($"{Name}: Target is null");
            return;
        }

        var players = ModifierUtils.GetPlayersWithModifier<OracleConfessModifier>(x => x.Oracle.AmOwner);
        players.Do(x => x.RpcRemoveModifier<OracleConfessModifier>());

        var faction = ChooseRevealedFaction(Target);

        Target.RpcAddModifier<OracleConfessModifier>(PlayerControl.LocalPlayer, faction);
    }

    private static int ChooseRevealedFaction(PlayerControl target)
    {
        var faction = 1;

        var num = Random.RandomRangeInt(1, 101);

        var options = OptionGroupSingleton<OracleOptions>.Instance;

        if (num <= options.RevealAccuracyPercentage)
        {
            if (target.IsCrewmate())
            {
                faction = 0;
            }
            else if (target.IsImpostor())
            {
                faction = 2;
            }
        }
        else
        {
            var num2 = Random.RandomRangeInt(0, 2);

            if (target.IsImpostor())
            {
                faction = num2;
            }
            else if (target.IsCrewmate())
            {
                faction = num2 + 1;
            }
            else if (num2 == 1)
            {
                faction = 2;
            }
            else
            {
                faction = 0;
            }
        }

        return faction;
    }
}