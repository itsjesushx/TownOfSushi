using HarmonyLib;
using UnityEngine;

namespace TownOfSushi.Buttons.Neutral;

public sealed class MysticObserveButton : TownOfSushiRoleButton<MysticRole, PlayerControl>
{
    public override string Name =>  "Observe";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Mystic;
    public override float Cooldown => OptionGroupSingleton<MysticOptions>.Instance.ObserveCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.Observe;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<MysticObservedModifier>());
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        ModifierUtils.GetPlayersWithModifier<MysticObservedModifier>()
            .Do(x => x.RemoveModifier<MysticObservedModifier>());

        Target.AddModifier<MysticObservedModifier>();

        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>{TownOfSushiColors.Mystic.ToTextColor()}You will recieve a report on {Target.Data.PlayerName} during the next meeting. This will help you narrow down their role.</color></b>",
            Color.white, new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Mystic.LoadAsset());

        notif1.AdjustNotification();
    }
}