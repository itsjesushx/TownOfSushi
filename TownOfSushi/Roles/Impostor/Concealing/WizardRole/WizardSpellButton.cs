using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class WizardSpellButton : TownOfSushiRoleButton<WizardRole, PlayerControl>
{
    public override string Name => "Spell";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<WizardOptions>.Instance.SpellCooldown;
    public override float EffectDuration => OptionGroupSingleton<WizardOptions>.Instance.SpellDuration;
    public override LoadableAsset<Sprite> Sprite => TownOfSushiAssets.SpellSprite;
    protected override void OnClick()
    {
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();

        WizardRole.RpcCastSpell(PlayerControl.LocalPlayer, Target);
        TownOfSushiAudio.PlaySound(TownOfSushiAudio.WitchIntro);

        var notif1 = Helpers.CreateAndShowNotification(Utils.ColorString(TownOfSushiColors.ImpSoft,
        $"<b>You have spelled {Target.Data.PlayerName}. They will die after next meeting.</b>"),
        Color.white, spr: TownOfSushiAssets.SpellSprite.LoadAsset());
        notif1.AdjustNotification();
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(includeImpostors: Utils.SpyInGame(), Distance, false,
            player => !player.HasModifier<WizardSpelledModifier>());
    }
}