using HarmonyLib;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class JailorJailButton : TownOfSushiRoleButton<JailorRole, PlayerControl>
{
    public override string Name => "Jail";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
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