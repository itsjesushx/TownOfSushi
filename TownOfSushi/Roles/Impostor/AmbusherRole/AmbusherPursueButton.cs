using MiraAPI.Hud;
using TownOfSushi.Buttons;
using TownOfSushi.Utilities.Appearances;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class AmbusherPursueButton : TownOfSushiRoleButton<AmbusherRole, PlayerControl>
{
    public override string Name => "Pursue";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => 0.001f;
    public override float InitialCooldown => 0.001f;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.PursueSprite;

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Role is { Pursued: null };
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        Role.Pursued = Target;
        
        Color color = Palette.PlayerColors[Target.GetDefaultAppearance().ColorId];
        var update = OptionGroupSingleton<AmbusherOptions>.Instance.UpdateInterval;

        Target.AddModifier<AmbusherArrowTargetModifier>(PlayerControl.LocalPlayer, color, update);

        TOSAudio.PlaySound(TOSAudio.TrackerActivateSound);

        var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Impostor,
            $"<b>You are now pursuing {Target.Data.PlayerName}. Ambush anyone near them at any time you wish.</b>"),
            Color.white, spr: TOSRoleIcons.Ambusher.LoadAsset());
        
        notif1.AdjustNotification();

        CustomButtonSingleton<AmbusherAmbushButton>.Instance.SetActive(true, Role);
        CustomButtonSingleton<AmbusherAmbushButton>.Instance.ResetCooldownAndOrEffect();
        SetActive(false, Role);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }
}