using HarmonyLib;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class InquisitorInquireButton : TownOfSushiRoleButton<InquisitorRole, PlayerControl>
{
    public override string Name => "Inquire";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override int MaxUses => (int)OptionGroupSingleton<InquisitorOptions>.Instance.MaxUses;
    public override Color TextOutlineColor => TownOfSushiColors.Inquisitor;

    public override float Cooldown =>
        OptionGroupSingleton<InquisitorOptions>.Instance.InquireCooldown.Value + MapCooldown;

    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.InquireSprite;

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && !OptionGroupSingleton<InquisitorOptions>.Instance.CantInquire;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<InquisitorInquiredModifier>());
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        if (ModifierUtils.GetActiveModifiers<InquisitorInquiredModifier>().Any())
        {
            ++UsesLeft;
            SetUses(UsesLeft);
        }

        ModifierUtils.GetPlayersWithModifier<InquisitorInquiredModifier>()
            .Do(x => x.RemoveModifier<InquisitorInquiredModifier>());

        Target.AddModifier<InquisitorInquiredModifier>();

        var notif1 = Helpers.CreateAndShowNotification(
            MiscUtils.ColorString(TownOfSushiColors.Inquisitor, $"<b>You will know if {Target.Data.PlayerName} is a heretic during the next meeting.</b>"),
            Color.white, spr: TOSRoleIcons.Inquisitor.LoadAsset());

        
        notif1.AdjustNotification();
    }
}