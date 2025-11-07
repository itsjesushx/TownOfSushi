using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class SeerRevealButton : TownOfSushiRoleButton<SeerRole, PlayerControl>
{
    public override string Name => "Check";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Seer;
    public override float Cooldown => OptionGroupSingleton<SeerOptions>.Instance.SeerCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.SeerButton;

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target!.HasModifier<SeerFirstCheckModifier>() &&
               !target!.HasModifier<SeerSecondCheckModifier>();
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null || Role == null) return;

        if (!PlayerControl.AllPlayerControls.ToArray().Any(p => !p.HasDied() && p.HasModifier<SeerFirstCheckModifier>()))
        {
            Target.AddModifier<SeerFirstCheckModifier>();
            Role.InvestigatedFirst = true;

            var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Seer,
            $"<b>You selected {Target.Data.PlayerName} as your <color=green>first target.</b>"),
            Color.white, spr: TOSModifierIcons.Aftermath.LoadAsset());
            notif1.AdjustNotification();    
        }
        else if (!PlayerControl.AllPlayerControls.ToArray().Any(p => !p.HasDied() && p.HasModifier<SeerSecondCheckModifier>()))
        {
            Target.AddModifier<SeerSecondCheckModifier>();

            var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Seer,
                $"<b>You selected {Target.Data.PlayerName} as your <color=green>second target.</b>"),
                Color.white, spr: TOSModifierIcons.Aftermath.LoadAsset());
            notif1.AdjustNotification();    
        }

        TOSAudio.PlaySound(TOSAudio.QuestionSound);
    }
}