using MiraAPI.Events;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modifiers;
using TownOfSushi.Utilities.Appearances;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class PainterPaintedModifier: ConcealedModifier, IVisualAppearance
{
    public override float Duration => OptionGroupSingleton<PainterOptions>.Instance.PaintDuration;
    public override string ModifierName => "Painted";
    public override bool HideOnUi => true;
    public override bool AutoStart => true;
    public override bool VisibleToOthers => true;
    public bool VisualPriority => false;
    public VisualAppearance GetVisualAppearance()
    {
        int max = Palette.PlayerColors.Length;
        int colorNumber = Utils.random.Next(0, max);
        // Might need to change this in the future when i add/remove a custom color D: but works for now ig
        // Changed hehe
        return new VisualAppearance(Player.GetDefaultModifiedAppearance(), TownOfSushiAppearances.Paint)
        {
            Speed = 1f,
            Size = new Vector3(0.7f, 0.7f, 1f),
            ColorId = colorNumber,
            HatId = string.Empty,
            SkinId = string.Empty,
            VisorId = string.Empty,
            PlayerName = string.Empty,
            PetId = string.Empty,
            NameVisible = false,
            PlayerMaterialColor = Palette.PlayerColors[colorNumber]
        };
    }
    public override void OnActivate()
    {
        Player.RawSetAppearance(this);

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.PainterPaint, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void OnDeactivate()
    {
        Player.ResetAppearance(override_checks: true);

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.PainterUnPaint, Player);

        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }
}