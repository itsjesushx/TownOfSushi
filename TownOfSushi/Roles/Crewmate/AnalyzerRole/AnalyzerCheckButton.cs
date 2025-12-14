using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class AnalyzerRevealButton : TownOfSushiRoleButton<AnalyzerRole, PlayerControl>
{
    public override string Name => "Check";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Analyzer;
    public override float Cooldown => OptionGroupSingleton<AnalyzerOptions>.Instance.AnalyzerCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.AnalyzerButton;

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target!.HasModifier<AnalyzerFirstCheckModifier>() &&
               !target!.HasModifier<AnalyzerSecondCheckModifier>();
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null || Role == null) return;

        if (!PlayerControl.AllPlayerControls.ToArray().Any(p => !p.HasDied() && p.HasModifier<AnalyzerFirstCheckModifier>()))
        {
            Target.AddModifier<AnalyzerFirstCheckModifier>(PlayerControl.LocalPlayer.PlayerId);
            Role.InvestigatedFirst = true;

            var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Analyzer,
            $"<b>You selected {Target.Data.PlayerName} as your <color=green>first target.</b>"),
            Color.white, spr: TOSModifierIcons.Aftermath.LoadAsset());
            notif1.AdjustNotification();
        }
        else if (!PlayerControl.AllPlayerControls.ToArray().Any(p => !p.HasDied() && p.HasModifier<AnalyzerSecondCheckModifier>()))
        {
            Target.AddModifier<AnalyzerSecondCheckModifier>(PlayerControl.LocalPlayer.PlayerId);

            var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Analyzer,
                $"<b>You selected {Target.Data.PlayerName} as your <color=green>second target.</b>"),
                Color.white, spr: TOSModifierIcons.Aftermath.LoadAsset());
            notif1.AdjustNotification();
        }

        TOSAudio.PlaySound(TOSAudio.QuestionSound);
    }
}