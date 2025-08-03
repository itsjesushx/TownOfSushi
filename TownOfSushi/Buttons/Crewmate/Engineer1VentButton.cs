using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modifiers;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;
using UnityEngine;

namespace TownOfSushi.Buttons.Crewmate;

public sealed class EngineerVentButton : TownOfSushiRoleButton<EngineerTouRole, Vent>
{
    public override string Name => "Vent";
    public override string Keybind => Keybinds.VentAction;
    public override Color TextOutlineColor => TownOfSushiColors.Engineer;
    public override float Cooldown => OptionGroupSingleton<EngineerOptions>.Instance.VentCooldown + 0.001f + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<EngineerOptions>.Instance.VentDuration;
    public override int MaxUses => (int)OptionGroupSingleton<EngineerOptions>.Instance.MaxVents;
    public override LoadableAsset<Sprite> Sprite => TosCrewAssets.EngiVentSprite;
    public int ExtraUses { get; set; }

    private static readonly ContactFilter2D Filter = Helpers.CreateFilter(Constants.NotShipMask);

    public override Vent? GetTarget() => PlayerControl.LocalPlayer.GetNearestObjectOfType<Vent>(Distance, Filter);

    public override bool CanUse()
    {
        var newTarget = GetTarget();
        if (newTarget != Target)
        {
            Target?.SetOutline(false, false);
        }

        Target = IsTargetValid(newTarget) ? newTarget : null;
        SetOutline(true);

        return ((Timer <= 0 && !PlayerControl.LocalPlayer.inVent && Target != null) || PlayerControl.LocalPlayer.inVent)
            && !PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>()
            && !PlayerControl.LocalPlayer.HasModifier<DisabledModifier>()
            && (MaxUses == 0 || UsesLeft > 0);
    }
    public override void ClickHandler()
    {
        if (!CanUse())
        {
            return;
        }

        OnClick();
        Button?.SetDisabled();
        if (EffectActive)
        {
            Timer = Cooldown;
            EffectActive = false;
            // Logger<TownOfSushiPlugin>.Error($"Effect is No Longer Active");
            // Logger<TownOfSushiPlugin>.Error($"Cooldown is active");
        }
        else if (HasEffect)
        {
            EffectActive = true;
            Timer = EffectDuration;
            // Logger<TownOfSushiPlugin>.Error($"Effect is Now Active");
        }
        else
        {
            Timer = !PlayerControl.LocalPlayer.inVent ? 0.001f : Cooldown;
            // Logger<TownOfSushiPlugin>.Error($"Cooldown is active");
        }
    }
    protected override void OnClick()
    {
        if (!PlayerControl.LocalPlayer.inVent)
        {
            // Logger<TownOfSushiPlugin>.Error($"Entering Vent");
            if (Target != null)
            {
                PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(Target.Id);
                Target.SetButtons(true);
            }
            // else Logger<TownOfSushiPlugin>.Error($"Vent is null...");
        }
        else if (Timer != 0)
        {
            // Logger<TownOfSushiPlugin>.Error($"Leaving Vent");
            OnEffectEnd();
            if (!HasEffect)
            {
                EffectActive = false;
                Timer = Cooldown;
            }
        }
    }
    public override void OnEffectEnd()
    {
        if (PlayerControl.LocalPlayer.inVent)
        {
            // Logger<TownOfSushiPlugin>.Error($"Left Vent");
            Vent.currentVent.SetButtons(false);
            PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
            UsesLeft--;
            if (MaxUses != 0) Button?.SetUsesRemaining(UsesLeft);
        }
    }
}
