using MiraAPI.Modifiers;
using TownOfSushi.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Roles.Neutral;
using UnityEngine;
using MiraAPI.Utilities;

namespace TownOfSushi.Buttons.Neutral;

public sealed class MercenaryBribeButton : TownOfSushiRoleButton<MercenaryRole, PlayerControl>
{
    public override string Name => "Bribe";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Mercenary;
    public override float Cooldown => 0.001f + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TosNeutAssets.BribeSprite;

    public override bool Enabled(RoleBehaviour? role) => base.Enabled(role) && Role.CanBribe;

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Mercenary Bribed: Target is null");
            return;
        }

        Target.RpcAddModifier<MercenaryBribedModifier>(PlayerControl.LocalPlayer);
        var notif1 = Helpers.CreateAndShowNotification($"<b>If {Target.Data.PlayerName} wins, you will win as well.</b>", Color.white, new Vector3(0f, 1f, -20f), spr: TosRoleIcons.Mercenary.LoadAsset());
        notif1.Text.SetOutlineThickness(0.35f);

        Role.Gold -= MercenaryRole.BrideCost;

        SetActive(false, Role);
    }

    public override PlayerControl? GetTarget() => PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, predicate: x => !x.HasModifier<MercenaryBribedModifier>());
}
