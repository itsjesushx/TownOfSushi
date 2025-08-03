using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.PluginLoading;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using Rewired;
using TownOfSushi.Modifiers;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Options;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons;

[MiraIgnore]
public abstract class TownOfSushiButton : CustomActionButton
{
    public override string Name => string.Empty;
    public static float MapCooldown => OptionGroupSingleton<TownOfSushiMapOptions>.Instance.GetMapBasedCooldownDifference();
    public override float InitialCooldown => 10;
    public override ButtonLocation Location => ButtonLocation.BottomRight;
    public override string CooldownTimerFormatString => Timer <= 10f && TownOfSushiPlugin.PreciseCooldowns.Value ? "0.0" : "0";

    /// <summary>
    /// Gets the keybind used for the button.<br/>
    /// Use ActionQuaternary for primary abilities, ActionSecondary for secondary abilities or kill buttons, tos.ActionCustom for tertiary abilities, and tos.ActionCustom2 for modifier buttons.
    /// </summary>
    public virtual string Keybind => string.Empty;

    public virtual int ConsoleBind()
    {
        return Keybind switch
        {
            Keybinds.PrimaryAction => Keybinds.PrimaryConsole,
            Keybinds.SecondaryAction => Keybinds.SecondaryConsole,
            Keybinds.ModifierAction => Keybinds.ModifierConsole,
            Keybinds.VentAction => Keybinds.VentConsole,
            _ => -1,
        };
    }

    private PassiveButton PassiveComp { get; set; }

    public override void SetActive(bool visible, RoleBehaviour role)
    {
        Button?.ToggleVisible(visible && Enabled(role) && !role.Player.HasDied());
    }
    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);
        if (Button == null)
        {
            Logger<TownOfSushiPlugin>.Error($"Button is null for {GetType().FullName}");
            return;
        }
        Button.usesRemainingSprite.sprite = TosAssets.AbilityCounterBasicSprite.LoadAsset();

        TownOfSushiColors.UseBasic = false;
        if (TextOutlineColor != Color.clear)
        {
            SetTextOutline(TextOutlineColor);
            Button.usesRemainingSprite.color = TextOutlineColor;
        }

        TownOfSushiColors.UseBasic = TownOfSushiPlugin.UseCrewmateTeamColor.Value;

        PassiveComp = Button.GetComponent<PassiveButton>();
    }
    public override void SetUses(int amount)
    {
        base.SetUses(amount);
        TownOfSushiColors.UseBasic = false;
        if (TextOutlineColor != Color.clear)
        {
            SetTextOutline(TextOutlineColor);
            Button!.usesRemainingSprite.color = TextOutlineColor;
        }

        TownOfSushiColors.UseBasic = TownOfSushiPlugin.UseCrewmateTeamColor.Value;
    }

    public override bool CanUse()
    {
        if (PlayerControl.LocalPlayer == null) return false;
        if (!PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.HasModifier<DisabledModifier>()) return false;
        return base.CanUse();
    }

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        if (MeetingHud.Instance)
        {
            return;
        }

        Button?.gameObject.SetActive(HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled);

        if (CanUse() && Keybind != string.Empty && (ReInput.players.GetPlayer(0).GetButtonDown(Keybind) || ConsoleJoystick.player.GetButtonDown(ConsoleBind())))
        {
            PassiveComp.OnClick.Invoke();
        }
    }
    public override void ClickHandler()
    {
        if (!CanClick() || PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>() || PlayerControl.LocalPlayer.HasModifier<DisabledModifier>())
        {
            return;
        }

        if (LimitedUses)
        {
            UsesLeft--;
            Button?.SetUsesRemaining(UsesLeft);
            TownOfSushiColors.UseBasic = false;
            if (TextOutlineColor != Color.clear)
            {
                SetTextOutline(TextOutlineColor);
                if (Button != null) Button.usesRemainingSprite.color = TextOutlineColor;
            }

            TownOfSushiColors.UseBasic = TownOfSushiPlugin.UseCrewmateTeamColor.Value;
        }

        OnClick();

        if (HasEffect)
        {
            EffectActive = true;
            Timer = EffectDuration;
        }
        else
        {
            Timer = Cooldown;
        }
    }
}

[MiraIgnore]
public abstract class TownOfSushiTargetButton<T> : CustomActionButton<T> where T : MonoBehaviour
{
    public override string Name => string.Empty;
    public static float MapCooldown => OptionGroupSingleton<TownOfSushiMapOptions>.Instance.GetMapBasedCooldownDifference();
    public override float InitialCooldown => 10;
    public override ButtonLocation Location => ButtonLocation.BottomRight;
    public override string CooldownTimerFormatString => Timer <= 10f && TownOfSushiPlugin.PreciseCooldowns.Value ? "0.0" : "0";

    /// <summary>
    /// Gets the keybind used for the button.
    /// Use ActionQuaternary for primary abilities, ActionSecondary for secondary abilities or kill buttons, tos.ActionCustom for tertiary abilities, and tos.ActionCustom2 for modifier buttons.
    /// </summary>
    public virtual string Keybind => string.Empty;

    public virtual int ConsoleBind()
    {
        return Keybind switch
        {
            Keybinds.PrimaryAction => Keybinds.PrimaryConsole,
            Keybinds.SecondaryAction => Keybinds.SecondaryConsole,
            Keybinds.ModifierAction => Keybinds.ModifierConsole,
            Keybinds.VentAction => Keybinds.VentConsole,
            _ => -1,
        };
    }

    private PassiveButton PassiveComp { get; set; }

    public override void SetActive(bool visible, RoleBehaviour role)
    {
        Button?.ToggleVisible(visible && Enabled(role) && !role.Player.HasDied());
    }

    public override bool CanUse()
    {
        if (!PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.HasModifier<DisabledModifier>()) return false;
        return base.CanUse();
    }

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);
        if (Button == null)
        {
            Logger<TownOfSushiPlugin>.Error($"Button is null for {GetType().FullName}");
            return;
        }

        switch (typeof(T))
        {
            case Type t when t == typeof(Vent):
                Button.usesRemainingSprite.sprite = TosAssets.AbilityCounterVentSprite.LoadAsset();
                break;
            case Type t when t == typeof(DeadBody):
                Button.usesRemainingSprite.sprite = TosAssets.AbilityCounterBodySprite.LoadAsset();
                break;
            case Type t when t == typeof(PlayerControl):
                Button.usesRemainingSprite.sprite = TosAssets.AbilityCounterPlayerSprite.LoadAsset();
                break;
            default:
                Button.usesRemainingSprite.sprite = TosAssets.AbilityCounterBasicSprite.LoadAsset();
                break;
        }
        TownOfSushiColors.UseBasic = false;
        if (TextOutlineColor != Color.clear)
        {
            SetTextOutline(TextOutlineColor);
            Button.usesRemainingSprite.color = TextOutlineColor;
        }

        TownOfSushiColors.UseBasic = TownOfSushiPlugin.UseCrewmateTeamColor.Value;
        
        PassiveComp = Button.GetComponent<PassiveButton>();
    }
    public override void SetUses(int amount)
    {
        base.SetUses(amount);
        TownOfSushiColors.UseBasic = false;
        if (TextOutlineColor != Color.clear)
        {
            SetTextOutline(TextOutlineColor);
            Button!.usesRemainingSprite.color = TextOutlineColor;
        }

        TownOfSushiColors.UseBasic = TownOfSushiPlugin.UseCrewmateTeamColor.Value;
    }

    public override void ClickHandler()
    {
        if (CanClick() && !PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>() && !PlayerControl.LocalPlayer.HasModifier<DisabledModifier>())
        {
            if (LimitedUses)
            {
                UsesLeft--;
                Button?.SetUsesRemaining(UsesLeft);
                TownOfSushiColors.UseBasic = false;
                if (TextOutlineColor != Color.clear)
                {
                    SetTextOutline(TextOutlineColor);
                    if (Button != null) Button.usesRemainingSprite.color = TextOutlineColor;
                }

                TownOfSushiColors.UseBasic = TownOfSushiPlugin.UseCrewmateTeamColor.Value;
            }

            OnClick();
            if (HasEffect)
            {
                EffectActive = true;
                Timer = EffectDuration;
            }
            else
            {
                Timer = Cooldown;
            }
        }
    }

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        if (MeetingHud.Instance)
        {
            return;
        }

        Button?.gameObject.SetActive(HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled);

        if (CanUse() && Keybind != string.Empty && (ReInput.players.GetPlayer(0).GetButtonDown(Keybind) || ConsoleJoystick.player.GetButtonDown(ConsoleBind())))
        {
            PassiveComp.OnClick.Invoke();
        }
    }
}

[MiraIgnore]
public abstract class TownOfSushiRoleButton<TRole> : TownOfSushiButton where TRole : RoleBehaviour
{
    public TRole Role => PlayerControl.LocalPlayer.GetRole<TRole>()!;

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is TRole;
    }
}

[MiraIgnore]
public abstract class TownOfSushiRoleButton<TRole, TTarget> : TownOfSushiTargetButton<TTarget> where TTarget : MonoBehaviour where TRole : RoleBehaviour
{
    public TRole Role => PlayerControl.LocalPlayer.GetRole<TRole>()!;

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is TRole;
    }

    public override void SetOutline(bool active)
    {
        if (Target != null && !PlayerControl.LocalPlayer.HasDied())
        {
            if (Target is PlayerControl target)
            {
                target.cosmetics.currentBodySprite.BodySprite.SetOutline(active ? Role.TeamColor : null);
            }
            else if (Target is DeadBody body)
            {
                body.bodyRenderers.Do(x => x.SetOutline(active ? Role.TeamColor : null));
            }
            else if (Target is Vent vent)
            {
                vent.SetOutline(active, true, Role.TeamColor);
            }
        }
    }
    public override bool IsTargetValid(TTarget? target)
    {
        if (target is PlayerControl playerTarget) return base.IsTargetValid(target) && !playerTarget.inVent && !(playerTarget.TryGetModifier<DisabledModifier>(out var mod) && !mod.CanBeInteractedWith);

        return base.IsTargetValid(target);
    }

}

public interface IAftermathablePlayerButton : IAftermathableButton
{
    PlayerControl? Target { get; set; }
}

public interface IAftermathableBodyButton : IAftermathableButton
{
    DeadBody? Target { get; set; }
}

public interface IAftermathableButton
{
    void ClickHandler();
}
public interface IDiseaseableButton
{
    void SetDiseasedTimer(float multiplier);
}
public interface IKillButton
{
    
}
