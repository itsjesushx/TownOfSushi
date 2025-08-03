using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using TownOfSushi.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Neutral;
using UnityEngine;
using MiraAPI.Utilities;

namespace TownOfSushi.Buttons.Neutral;

public sealed class MercenaryGuardButton : TownOfSushiRoleButton<MercenaryRole, PlayerControl>
{
    public override string Name => "Guard";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Mercenary;
    public override float Cooldown => OptionGroupSingleton<MercenaryOptions>.Instance.GuardCooldown + MapCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<MercenaryOptions>.Instance.MaxUses;
    public override LoadableAsset<Sprite> Sprite => TosNeutAssets.GuardSprite;

    public override bool Enabled(RoleBehaviour? role) => base.Enabled(role) && !PlayerControl.LocalPlayer.Data.IsDead;
    
    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Mercenary Guard: Target is null");
            return;
        }

        Target.RpcAddModifier<MercenaryGuardModifier>(PlayerControl.LocalPlayer);
        var notif1 = Helpers.CreateAndShowNotification($"<b>Once {Target.Data.PlayerName} is interacted with, you will get one gold.</b>", Color.white, new Vector3(0f, 1f, -20f), spr: TosRoleIcons.Mercenary.LoadAsset());
        notif1.Text.SetOutlineThickness(0.35f);
    }

    public override PlayerControl? GetTarget() => PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, predicate: x => !x.HasModifier<MercenaryGuardModifier>());
}
