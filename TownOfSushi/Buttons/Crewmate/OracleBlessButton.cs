using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Crewmate;

public sealed class OracleBlessButton : TownOfSushiRoleButton<OracleRole, PlayerControl>
{
    public override string Name => "Bless";
    public override Color TextOutlineColor => TownOfSushiColors.Oracle;
    public override string Keybind => Keybinds.SecondaryAction;
    public override float Cooldown => OptionGroupSingleton<OracleOptions>.Instance.BlessCooldown;
    public override LoadableAsset<Sprite> Sprite => TosCrewAssets.BlessSprite;

    public override PlayerControl? GetTarget() => PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, predicate: x => !x.HasModifier<OracleBlessedModifier>());

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
