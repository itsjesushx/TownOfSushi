using AmongUs.GameOptions;
using TownOfSushi.Buttons;
using TownOfSushi.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfSushi.Roles.Crewmate;

public sealed class OperativeButton : TownOfSushiButton
{
    public VitalsMinigame? vitals;
    public override string Name => "Vitals";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Operative;
    public override float Cooldown => OptionGroupSingleton<OperativeOptions>.Instance.VitalsDisplayCooldown + MapCooldown;
    public float AvailableCharge { get; set; } = OptionGroupSingleton<OperativeOptions>.Instance.VitalsStartingCharge;

    public override float EffectDuration
    {
        get
        {
            if (OptionGroupSingleton<OperativeOptions>.Instance.VitalsDisplayDuration == 0)
            {
                return AvailableCharge;
            }

            return AvailableCharge < OptionGroupSingleton<OperativeOptions>.Instance.VitalsDisplayDuration
                ? AvailableCharge
                : OptionGroupSingleton<OperativeOptions>.Instance.VitalsDisplayDuration;
        }
    }
    public override LoadableAsset<Sprite> Sprite => TOSAssets.VitalsSprite;

    public override bool Enabled(RoleBehaviour? role)
    {
        return PlayerControl.LocalPlayer != null &&
               PlayerControl.LocalPlayer.Data.Role is OperativeRole &&
               !PlayerControl.LocalPlayer.Data.IsDead &&
               !MeetingHud.Instance && !ExileController.Instance; // for some reason you can open vitals during meetings or exiles;
    }

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);
        AvailableCharge = OptionGroupSingleton<OperativeOptions>.Instance.VitalsStartingCharge;
    }

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

        if (vitals != null)
        {
            AvailableCharge -= Time.deltaTime;
            vitals.BatteryText.text = $"{(int)AvailableCharge}";
            if (AvailableCharge <= 0f)
            {
                vitals.Close();
                RefreshAbilityButton();
                ResetCooldownAndOrEffect();
                vitals = null;
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
        if (vitals == null && EffectActive)
        {
            ResetCooldownAndOrEffect();
        }
    }

    public override bool CanUse()
    {
        return Timer <= 0 && !EffectActive && AvailableCharge > 0f &&
               !PlayerControl.LocalPlayer.HasModifier<DisabledModifier>() &&
               !PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>();
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

    protected override void OnClick()
    {
        if (!OptionGroupSingleton<OperativeOptions>.Instance.MoveWithMenu)
        {
            PlayerControl.LocalPlayer.NetTransform.Halt();
        }

        vitals = Object.Instantiate<VitalsMinigame>(RoleManager.Instance.GetRole(RoleTypes.Scientist)
            .Cast<ScientistRole>().VitalsPrefab);
        vitals.transform.SetParent(Camera.main.transform, false);
        vitals.transform.localPosition = new Vector3(0f, 0f, -50f);
        vitals.BatteryText.gameObject.SetActive(true);
        vitals.Begin(null);
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();

        if (vitals != null)
        {
            vitals.Close();
            RefreshAbilityButton();
            vitals = null;
        }
    }
}