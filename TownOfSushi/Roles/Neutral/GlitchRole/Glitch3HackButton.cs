using MiraAPI.Hud;
using Reactor.Utilities;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class GlitchHackButton : TownOfSushiRoleButton<GlitchRole, PlayerControl>, IAftermathablePlayerButton
{
    public override string Name => "Hack";
    public override BaseKeybind Keybind => Keybinds.TertiaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Glitch;
    public override float Cooldown => OptionGroupSingleton<GlitchOptions>.Instance.HackCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.HackSprite;
    public override ButtonLocation Location => ButtonLocation.BottomRight;
    public override bool ShouldPauseInVent => false;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Glitch Hack: Target is null");
            return;
        }

        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>Once {Target.Data.PlayerName} attempts to use an ability, all their abilities will get disabled.</b>",
            Color.white, new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Glitch.LoadAsset());
        notif1.AdjustNotification();

        TOSAudio.PlaySound(TOSAudio.HackedSound);
        Target.RpcAddModifier<GlitchHackedModifier>(PlayerControl.LocalPlayer.PlayerId);
    }
}