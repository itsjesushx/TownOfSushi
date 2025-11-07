using Reactor.Utilities;
using UnityEngine;
using TownOfSushi.Buttons;

namespace TownOfSushi.Roles.Crewmate;

public sealed class KnightButton : TownOfSushiRoleButton<MonarchRole, PlayerControl>
{
    public override string Name => "Knight";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Monarch;
    public override float Cooldown => OptionGroupSingleton<MonarchOptions>.Instance.KnightCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.KnightSprite;
    public override int MaxUses => (int)OptionGroupSingleton<MonarchOptions>.Instance.MaxKnights;
    public bool Usable = true;
    public int ExtraUses { get; set; }
    public override bool CanUse()
    {
        return base.CanUse() && Usable;
    }
    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target?.HasModifier<MonarchKnightedModifier>() == true;
    }

    public override PlayerControl? GetTarget() => PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Knight: Target is null");
            return;
        }

        var player = ModifierUtils.GetPlayersWithModifier<MonarchKnightedModifier>(x => x.Monarch.AmOwner).FirstOrDefault();

        if (player != null)
        {
            player.RpcRemoveModifier<MonarchKnightedModifier>();
        }

        Target.RpcAddModifier<MonarchKnightedModifier>(PlayerControl.LocalPlayer);
        Usable = false;
        var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Monarch,$"<b>You have knighted{Target.Data.PlayerName}, they will have an extra vote each meeting as long as you're alive.</color></b>"), Color.white, new Vector3(0f, 1f, -20f), spr: TOSCrewAssets.KnightSprite.LoadAsset());
        notif1.AdjustNotification();
    }
}