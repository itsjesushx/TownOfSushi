using TownOfSushi.Buttons;
using TownOfSushi.Modules;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class TrapperTrapButton : TownOfSushiRoleButton<TrapperRole>
{
    public override string Name => "Trap";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Trapper;
    public override float Cooldown => OptionGroupSingleton<TrapperOptions>.Instance.TrapCooldown + MapCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<TrapperOptions>.Instance.MaxTraps;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.TrapSprite;
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

        TOSAudio.PlaySound(TOSAudio.TrapperPlaceSound);
    }
}