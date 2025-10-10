using MiraAPI.Hud;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class RomanticPickButton : TownOfSushiRoleButton<RomanticRole, PlayerControl>
{
    public override string Name => "Create Lover";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Romantic;
    public override float Cooldown => OptionGroupSingleton<RomanticOptions>.Instance.PickCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.RomanticPick;

    public override bool Enabled(RoleBehaviour? role) => role is RomanticRole { HasBeloved: false };
    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        RomanticRole.RpcSetRomanticBeloved(PlayerControl.LocalPlayer, Target);
        
        var notif1 = Helpers.CreateAndShowNotification(
            MiscUtils.ColorString(TownOfSushiColors.Romantic, $"<b>You fell in love with {Target.Data.PlayerName}. Protect them at any cost!</b>"), Color.white, spr: TOSRoleIcons.Romantic.LoadAsset());
        notif1.Text.SetOutlineThickness(0.35f);
            notif1.transform.localPosition = new Vector3(0f, 1f, -20f);

        SetActive(false, Role);
        CustomButtonSingleton<RomanticProtectButton>.Instance.SetActive(true, Role);
    }

    public override PlayerControl? GetTarget() => PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
}