using MiraAPI.Hud;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class AdministratorAdminTableModifierButton : TownOfSushiButton
{
    public override string Name => "Admin";
    public override BaseKeybind Keybind => Keybinds.ModifierAction;
    public override Color TextOutlineColor => TownOfSushiColors.Administrator;
    public override float Cooldown => OptionGroupSingleton<AdministratorOptions>.Instance.DisplayCooldown + MapCooldown;
    public float AvailableCharge { get; set; } = OptionGroupSingleton<AdministratorOptions>.Instance.StartingCharge;
    public bool usingPortable { get; set; }

    public override float EffectDuration
    {
        get
        {
            if (OptionGroupSingleton<AdministratorOptions>.Instance.DisplayDuration == 0)
            {
                return AvailableCharge;
            }

            return AvailableCharge < OptionGroupSingleton<AdministratorOptions>.Instance.DisplayDuration
                ? AvailableCharge
                : OptionGroupSingleton<AdministratorOptions>.Instance.DisplayDuration;
        }
    }

    public override ButtonLocation Location => ButtonLocation.BottomLeft;
    public override LoadableAsset<Sprite> Sprite => TOSAssets.AdminSprite;

    private void RefreshAbilityButton()
    {
        if (AvailableCharge > 0f && !PlayerControl.LocalPlayer.AreCommsAffected())
        {
            Button?.SetEnabled();
            return;
        }

        Button?.SetDisabled();
    }

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        if (!playerControl.AmOwner || MeetingHud.Instance)
        {
            return;
        }

        if (usingPortable && !MapBehaviour.Instance.gameObject.activeSelf)
        {
            RefreshAbilityButton();
            ResetCooldownAndOrEffect();
            usingPortable = false;
            return;
        }

        if (usingPortable)
        {
            AvailableCharge -= Time.deltaTime;
            if (AvailableCharge <= 0f)
            {
                MapBehaviour.Instance.Close();
                RefreshAbilityButton();
                ResetCooldownAndOrEffect();
                usingPortable = false;
                return;
            }
        }
        else
        {
            RefreshAbilityButton();
        }

        Button?.usesRemainingText.gameObject.SetActive(true);
        Button?.usesRemainingSprite.gameObject.SetActive(true);
        Button!.usesRemainingText.text = (int)AvailableCharge + "%";
        if (!usingPortable && EffectActive)
        {
            ResetCooldownAndOrEffect();
        }
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return PlayerControl.LocalPlayer != null &&
               PlayerControl.LocalPlayer.HasModifier<AdministratorModifier>() &&
               !PlayerControl.LocalPlayer.Data.IsDead;
    }

    public override bool CanUse()
    {
        return Timer <= 0 && !EffectActive && AvailableCharge > 0f &&
               !PlayerControl.LocalPlayer.HasModifier<DisabledModifier>() &&
               !PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>();
    }

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);
        AvailableCharge = OptionGroupSingleton<AdministratorOptions>.Instance.StartingCharge;
        Button!.transform.localPosition =
            new Vector3(Button.transform.localPosition.x, Button.transform.localPosition.y, -150f);
    }

    protected override void OnClick()
    {
        if (!OptionGroupSingleton<AdministratorOptions>.Instance.MoveWithMenu)
        {
            PlayerControl.LocalPlayer.NetTransform.Halt();
        }

        usingPortable = true;
        ToggleMapVisible(OptionGroupSingleton<AdministratorOptions>.Instance.MoveWithMenu);
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();

        if (usingPortable)
        {
            MapBehaviour.Instance.Close();
            RefreshAbilityButton();
            usingPortable = false;
        }
    }

    public override void ClickHandler()
    {
        if (!CanUse() || Minigame.Instance != null)
        {
            return;
        }

        OnClick();
        Button?.SetDisabled();
        if (EffectActive)
        {
            Timer = Cooldown;
            EffectActive = false;
        }
        else if (HasEffect)
        {
            EffectActive = true;
            Timer = EffectDuration;
        }
        else
        {
            Timer = Cooldown;
        }
    }

    private static void ToggleMapVisible(bool canMove = false)
    {
        if (MapBehaviour.Instance && MapBehaviour.Instance.gameObject.activeSelf)
        {
            MapBehaviour.Instance.Close();
            return;
        }

        if (!ShipStatus.Instance)
        {
            return;
        }

        HudManager.Instance.InitMap();
        if (!PlayerControl.LocalPlayer.CanMove && !MeetingHud.Instance)
        {
            return;
        }

        var opts = GameManager.Instance.GetMapOptions();
        var portableAdmin = MapBehaviour.Instance;

        portableAdmin.GenericShow();
        portableAdmin.countOverlay.gameObject.SetActive(true);
        portableAdmin.countOverlay.SetOptions(opts.ShowLivePlayerPosition, opts.IncludeDeadBodies);
        portableAdmin.countOverlayAllowsMovement = canMove;
        portableAdmin.taskOverlay.Hide();
        portableAdmin.HerePoint.enabled = !opts.ShowLivePlayerPosition;
        portableAdmin.TrackedHerePoint.gameObject.SetActive(false);
        if (portableAdmin.HerePoint.enabled)
        {
            PlayerControl.LocalPlayer.SetPlayerMaterialColors(portableAdmin.HerePoint);
        }
    }
}