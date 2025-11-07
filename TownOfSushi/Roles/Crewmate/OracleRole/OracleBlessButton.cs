using HarmonyLib;
using Reactor.Utilities;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class OracleBlessButton : TownOfSushiRoleButton<OracleRole, PlayerControl>
{
    public override string Name => "Bless";
    public override Color TextOutlineColor => TownOfSushiColors.Oracle;
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override float Cooldown => OptionGroupSingleton<OracleOptions>.Instance.BlessCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.BlessSprite;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<OracleBlessedModifier>());
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error($"{Name}: Target is null");
            return;
        }

        var players = ModifierUtils.GetPlayersWithModifier<OracleBlessedModifier>(x => x.Oracle.AmOwner);
        players.Do(x => x.RpcRemoveModifier<OracleBlessedModifier>());

        Target.RpcAddModifier<OracleBlessedModifier>(PlayerControl.LocalPlayer);
    }
}