using System.Globalization;
using HarmonyLib;
using MiraAPI.Hud;
using MiraAPI.PluginLoading;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfSushi.Modifiers;
using TownOfSushi.Options;
using UnityEngine;

namespace TownOfSushi.Buttons;

[MiraIgnore]
public abstract class TownOfSushiButton : CustomActionButton
{
    public override string Name => string.Empty;

    public static float MapCooldown =>
        OptionGroupSingleton<TownOfSushiMapOptions>.Instance.GetMapBasedCooldownDifference();

    public override float InitialCooldown => 10;
    public override ButtonLocation Location => ButtonLocation.BottomRight;

    public override string CooldownTimerFormatString =>
        Timer <= 10f && TownOfSushiPlugin.PreciseCooldowns.Value ? "0.0" : "0";

    public virtual bool UsableInDeath => false;
    public virtual bool ShouldPauseInVent => true;

    /// <summary>
    ///     Gets the keybind used for the button.<br />
    ///     Use ActionQuaternary for primary abilities, ActionSecondary for secondary abilities or kill buttons,
    ///     tou.ActionCustom for tertiary abilities, and tou.ActionCustom2 for modifier buttons.
    /// </summary>
    public virtual string Keybind => string.Empty;

    public PassiveButton PassiveComp { get; set; }

    public virtual int ConsoleBind()
    {
        return Keybind switch
        {
            Keybinds.PrimaryAction => Keybinds.PrimaryConsole,
            Keybinds.SecondaryAction => Keybinds.SecondaryConsole,
            Keybinds.ModifierAction => Keybinds.ModifierConsole,
            Keybinds.VentAction => Keybinds.VentConsole,
            _ => -1
        };
    }

    public override void FixedUpdateHandler(PlayerControl playerControl)
    {
        if (Timer >= 0)
        {
            if (!TimerPaused && (!(ShouldPauseInVent && PlayerControl.LocalPlayer.inVent) || EffectActive))
            {
                Timer -= Time.deltaTime;
            }
        }
        else if (HasEffect && EffectActive)
        {
            EffectActive = false;
            Timer = Cooldown;
            OnEffectEnd();
        }

        if (Button)
        {
            if (CanUse())
            {
                Button!.SetEnabled();
            }
            else
            {
                Button!.SetDisabled();
            }

            if (EffectActive)
            {
                Button.SetFillUp(Timer, EffectDuration);

                Button.cooldownTimerText.text =
                    Timer.ToString(CooldownTimerFormatString, NumberFormatInfo.InvariantInfo);
                Button.cooldownTimerText.gameObject.SetActive(true);
            }
            else
            {
                Button.SetCooldownFormat(Timer, Cooldown, CooldownTimerFormatString);
            }
        }

        FixedUpdate(playerControl);
    }

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

        Button.usesRemainingSprite.sprite = TOSAssets.AbilityCounterBasicSprite.LoadAsset();

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
        if (PlayerControl.LocalPlayer == null)
        {
            return false;
        }
        
        if (PlayerControl.LocalPlayer.HasDied() && !UsableInDeath)
        {
            return false;
        }

        if (!PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.GetModifiers<DisabledModifier>().Any(x => !x.CanUseAbilities))
        {
            return false;
        }

        return base.CanUse();
    }

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        if (MeetingHud.Instance)
        {
            return;
        }

        Button?.gameObject.SetActive(HudManager.Instance.UseButton.isActiveAndEnabled ||
                                     HudManager.Instance.PetButton.isActiveAndEnabled);
    }

    public override void ClickHandler()
    {
        if (!CanClick() || PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>() ||
            PlayerControl.LocalPlayer.GetModifiers<DisabledModifier>().Any(x => !x.CanUseAbilities))
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
                if (Button != null)
                {
                    Button.usesRemainingSprite.color = TextOutlineColor;
                }
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

    public static float MapCooldown =>
        OptionGroupSingleton<TownOfSushiMapOptions>.Instance.GetMapBasedCooldownDifference();

    public override float InitialCooldown => 10;
    public override ButtonLocation Location => ButtonLocation.BottomRight;

    public override string CooldownTimerFormatString =>
        Timer <= 10f && TownOfSushiPlugin.PreciseCooldowns.Value ? "0.0" : "0";

    public virtual bool ShouldPauseInVent => true;
    public virtual bool UsableInDeath => false;

    /// <summary>
    ///     Gets the keybind used for the button.
    ///     Use ActionQuaternary for primary abilities, ActionSecondary for secondary abilities or kill buttons,
    ///     tou.ActionCustom for tertiary abilities, and tou.ActionCustom2 for modifier buttons.
    /// </summary>
    public virtual string Keybind => string.Empty;

    public PassiveButton PassiveComp { get; set; }

    public virtual int ConsoleBind()
    {
        return Keybind switch
        {
            Keybinds.PrimaryAction => Keybinds.PrimaryConsole,
            Keybinds.SecondaryAction => Keybinds.SecondaryConsole,
            Keybinds.ModifierAction => Keybinds.ModifierConsole,
            Keybinds.VentAction => Keybinds.VentConsole,
            _ => -1
        };
    }

    public override void FixedUpdateHandler(PlayerControl playerControl)
    {
        if (Timer >= 0)
        {
            if (!TimerPaused && (!(ShouldPauseInVent && PlayerControl.LocalPlayer.inVent) || EffectActive))
            {
                Timer -= Time.deltaTime;
            }
        }
        else if (HasEffect && EffectActive)
        {
            EffectActive = false;
            Timer = Cooldown;
            OnEffectEnd();
        }

        if (Button)
        {
            if (CanUse())
            {
                Button!.SetEnabled();
            }
            else
            {
                Button!.SetDisabled();
            }

            if (EffectActive)
            {
                Button.SetFillUp(Timer, EffectDuration);

                Button.cooldownTimerText.text =
                    Timer.ToString(CooldownTimerFormatString, NumberFormatInfo.InvariantInfo);
                Button.cooldownTimerText.gameObject.SetActive(true);
            }
            else
            {
                Button.SetCooldownFormat(Timer, Cooldown, CooldownTimerFormatString);
            }
        }

        FixedUpdate(playerControl);
    }

    public override void SetActive(bool visible, RoleBehaviour role)
    {
        Button?.ToggleVisible(visible && Enabled(role) && !role.Player.HasDied());
    }

    public override bool CanUse()
    {
        if (PlayerControl.LocalPlayer.HasDied() && !UsableInDeath)
        {
            return false;
        }
        
        if (!PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.GetModifiers<DisabledModifier>().Any(x => !x.CanUseAbilities))
        {
            return false;
        }

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
                Button.usesRemainingSprite.sprite = TOSAssets.AbilityCounterVentSprite.LoadAsset();
                break;
            case Type t when t == typeof(DeadBody):
                Button.usesRemainingSprite.sprite = TOSAssets.AbilityCounterBodySprite.LoadAsset();
                break;
            case Type t when t == typeof(PlayerControl):
                Button.usesRemainingSprite.sprite = TOSAssets.AbilityCounterPlayerSprite.LoadAsset();
                break;
            default:
                Button.usesRemainingSprite.sprite = TOSAssets.AbilityCounterBasicSprite.LoadAsset();
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
        if (CanClick() && !PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>() &&
            !PlayerControl.LocalPlayer.HasModifier<DisabledModifier>())
        {
            if (LimitedUses)
            {
                UsesLeft--;
                Button?.SetUsesRemaining(UsesLeft);
                TownOfSushiColors.UseBasic = false;
                if (TextOutlineColor != Color.clear)
                {
                    SetTextOutline(TextOutlineColor);
                    if (Button != null)
                    {
                        Button.usesRemainingSprite.color = TextOutlineColor;
                    }
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

        Button?.gameObject.SetActive(HudManager.Instance.UseButton.isActiveAndEnabled ||
                                     HudManager.Instance.PetButton.isActiveAndEnabled);
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
public abstract class TownOfSushiRoleButton<TRole, TTarget> : TownOfSushiTargetButton<TTarget>
    where TTarget : MonoBehaviour where TRole : RoleBehaviour
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
        if (target is PlayerControl playerTarget)
        {
            return base.IsTargetValid(target) && !playerTarget.inVent &&
                   !playerTarget.GetModifiers<DisabledModifier>().Any(mod => !mod.CanBeInteractedWith);
        }

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