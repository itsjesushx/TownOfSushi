using TownOfSushi.Buttons;

using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class TrapperTrapButton : TownOfSushiRoleButton<TrapperRole>
{
    public override string Name => "Trap";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Trapper;
    public override float Cooldown => OptionGroupSingleton<TrapperOptions>.Instance.TrapCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<TrapperOptions>.Instance.MaxTraps;
    public override LoadableAsset<Sprite> Sprite => TownOfSushiAssets.TrapSprite;
    public int ExtraUses { get; set; }

    protected override void OnClick()
    {
        var role = PlayerControl.LocalPlayer.GetRole<TrapperRole>();

        if (role == null)
        {
            return;
        }

        var pos = PlayerControl.LocalPlayer.transform.position;
        pos.z += 0.001f;

        Trap.CreateTrap(role, pos);

        TownOfSushiAudio.PlaySound(TownOfSushiAudio.TrapperPlaceSound);
    }
}