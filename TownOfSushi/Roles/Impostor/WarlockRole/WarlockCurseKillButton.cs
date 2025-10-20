using MiraAPI.Hud;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class WarlockCurseKillButton : TownOfSushiRoleButton<WarlockRole, PlayerControl>
{
    public override string Name => "Kill";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => 0.001f;
    public override float InitialCooldown => 0.001f;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.WarlockCurseKillButton;
    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Role is { CursedPlayer: not null };
    }
    protected override void OnClick()
    {
        if (Target == null || Role.CursedPlayer!.HasDied())
        {
            return;
        }

        WarlockRole.RpcCurseKill(Role.CursedPlayer!, Target, PlayerControl.LocalPlayer);

        CustomButtonSingleton<WarlockCurseButton>.Instance.SetActive(true, Role);
        CustomButtonSingleton<WarlockCurseButton>.Instance.ResetCooldownAndOrEffect();
        SetActive(false, Role);
    }
    
    public override PlayerControl? GetTarget()
    {
        return Role.CursedPlayer!.GetClosestLivingPlayer(true, Distance, false, predicate: plr => !plr.HasModifier<WarlockCursedModifier>());
    }
}