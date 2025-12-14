using Reactor.Utilities;
using TownOfSushi.Buttons;
using TownOfSushi.Utilities.Appearances;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class SonarTrackButton : TownOfSushiRoleButton<SonarRole, PlayerControl>
{
    public override string Name => "Track";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Sonar;
    public override float Cooldown => OptionGroupSingleton<SonarOptions>.Instance.TrackCooldown + MapCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<SonarOptions>.Instance.MaxTracks;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.TrackSprite;
    public int ExtraUses { get; set; }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target!.HasModifier<SonarArrowTargetModifier>();
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Track: Target is null");
            return;
        }

        Color color = Palette.PlayerColors[Target.GetDefaultAppearance().ColorId];
        var update = OptionGroupSingleton<SonarOptions>.Instance.UpdateInterval;

        Target.AddModifier<SonarArrowTargetModifier>(PlayerControl.LocalPlayer, color, update);

        TOSAudio.PlaySound(TOSAudio.SonarActivateSound);
    }
}