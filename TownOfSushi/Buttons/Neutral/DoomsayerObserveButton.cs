using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Neutral;

public sealed class DoomsayerObserveButton : TownOfSushiRoleButton<DoomsayerRole, PlayerControl>
{
    public override string Name => "Observe";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Doomsayer;
    public override float Cooldown => OptionGroupSingleton<DoomsayerOptions>.Instance.ObserveCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TosNeutAssets.Observe;

    public override bool Enabled(RoleBehaviour? role) => base.Enabled(role) && !OptionGroupSingleton<DoomsayerOptions>.Instance.CantObserve;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, predicate: x => !x.HasModifier<DoomsayerObservedModifier>());
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }
        ModifierUtils.GetPlayersWithModifier<DoomsayerObservedModifier>().Do(x => x.RemoveModifier<DoomsayerObservedModifier>());

        Target.AddModifier<DoomsayerObservedModifier>();
        
        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>{TownOfSushiColors.Doomsayer.ToTextColor()}You will recieve a report on {Target.Data.PlayerName} during the next meeting. This will help you narrow down their role.</color></b>", Color.white, spr: TosRoleIcons.Doomsayer.LoadAsset());

        notif1.Text.SetOutlineThickness(0.35f);
            notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
    }
}
