using MiraAPI.Hud;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class WarlockCurseButton : TownOfSushiRoleButton<WarlockRole, PlayerControl>
{
    public override string Name => "Curse";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<WarlockOptions>.Instance.CurseCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.WarlockCurseButton;
    public override bool ShouldPauseInVent => false;
    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        WarlockRole.RpcCurse(PlayerControl.LocalPlayer, Target);
        TOSAudio.PlaySound(TOSAudio.WarlockCurse);
        var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.ImpSoft,
            $"<b>You have cursed {Target.Data.PlayerName}. Once you click your custom kill button, they will be forced to kill the nearest player. But beware, it can also be you!</b>"),
            Color.white, spr: TOSRoleIcons.Warlock.LoadAsset());
        
        notif1.AdjustNotification();

        CustomButtonSingleton<WarlockCurseKillButton>.Instance.SetActive(true, Role);
        CustomButtonSingleton<WarlockCurseKillButton>.Instance.ResetCooldownAndOrEffect();
        SetActive(false, Role);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false,
            player => !player.HasModifier<WarlockCursedModifier>());
    }
}