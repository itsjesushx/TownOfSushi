using MiraAPI.GameOptions;
using MiraAPI.Hud;
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

public sealed class GlitchHackButton : TownOfSushiRoleButton<GlitchRole, PlayerControl>, IAftermathablePlayerButton
{
    public override string Name => "Hack";
    public override string Keybind => "tos.ActionCustom";
    public override Color TextOutlineColor => TownOfSushiColors.Glitch;
    public override float Cooldown => OptionGroupSingleton<GlitchOptions>.Instance.HackCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TosNeutAssets.HackSprite;
    public override ButtonLocation Location => ButtonLocation.BottomRight;

    public override PlayerControl? GetTarget() => PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Glitch Hack: Target is null");
            return;
        }
        var notif1 = Helpers.CreateAndShowNotification($"<b>Once {Target.Data.PlayerName} attempts to use an ability, all their abilities will get disabled.</b>", Color.white, new Vector3(0f, 1f, -20f), spr: TosRoleIcons.Glitch.LoadAsset());
        notif1.Text.SetOutlineThickness(0.35f);

        TosAudio.PlaySound(TosAudio.HackedSound);
        Target.RpcAddModifier<GlitchHackedModifier>(PlayerControl.LocalPlayer.PlayerId);
    }
}
