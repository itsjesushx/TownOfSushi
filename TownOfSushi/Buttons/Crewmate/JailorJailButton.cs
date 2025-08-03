using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Crewmate;

public sealed class JailorJailButton : TownOfSushiRoleButton<JailorRole, PlayerControl>
{
    public override string Name => "Jail";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Jailor;
    public override float Cooldown => OptionGroupSingleton<JailorOptions>.Instance.JailCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.JailSprite;

    public bool ExecutedACrew { get; set; }

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && !ExecutedACrew;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: player => !player.HasModifier<JailedModifier>() && !player.HasModifier<JailSparedModifier>());
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        ModifierUtils.GetPlayersWithModifier<JailedModifier>().Do(x => x.RpcRemoveModifier<JailedModifier>());
        Target?.RpcAddModifier<JailedModifier>(PlayerControl.LocalPlayer.PlayerId);
        TOSAudio.PlaySound(TOSAudio.JailSound);
    }
}