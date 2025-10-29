using Reactor.Utilities;
using TownOfSushi.Buttons;
using TownOfSushi.Modifiers;
using TownOfSushi.Modules;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfSushi.Roles.Crewmate;

public sealed class SecurityButton : TownOfSushiButton
{
    public Minigame? securityMinigame;
    public override string Name => "Security";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Operative;
    public override float Cooldown => OptionGroupSingleton<OperativeOptions>.Instance.DisplayCooldown + MapCooldown;
    public float AvailableCharge { get; set; } = OptionGroupSingleton<OperativeOptions>.Instance.StartingCharge;

    public override float EffectDuration
    {
        get
        {
            if (OptionGroupSingleton<OperativeOptions>.Instance.DisplayDuration == 0)
            {
                return AvailableCharge;
            }

            return AvailableCharge < OptionGroupSingleton<OperativeOptions>.Instance.DisplayDuration
                ? AvailableCharge
                : OptionGroupSingleton<OperativeOptions>.Instance.DisplayDuration;
        }
    }
    public override LoadableAsset<Sprite> Sprite => TOSAssets.CameraSprite;
    public bool canMoveWithMinigame { get; set; }

    public override bool Enabled(RoleBehaviour? role)
    {
        return PlayerControl.LocalPlayer != null &&
               PlayerControl.LocalPlayer.GetRoleWhenAlive() is OperativeRole &&
               !PlayerControl.LocalPlayer.Data.IsDead;
    }

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);
        // this is so you can see it through cams
        Button!.transform.localPosition =
            new Vector3(Button.transform.localPosition.x, Button.transform.localPosition.y, -150f);
        AvailableCharge = OptionGroupSingleton<OperativeOptions>.Instance.StartingCharge;
    }

    private void RefreshAbilityButton()
    {
        if (AvailableCharge > 0f && !PlayerControl.LocalPlayer.AreCommsAffected())
        {
            DestroyableSingleton<HudManager>.Instance.AbilityButton.SetEnabled();
            return;
        }

        DestroyableSingleton<HudManager>.Instance.AbilityButton.SetDisabled();
    }

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        if (!playerControl.AmOwner || MeetingHud.Instance)
        {
            return;
        }

        if (securityMinigame != null)
        {
            AvailableCharge -= Time.deltaTime;
            if (AvailableCharge <= 0f)
            {
                securityMinigame.Close();
                RefreshAbilityButton();
                ResetCooldownAndOrEffect();
                canMoveWithMinigame = false;
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
        if (securityMinigame == null && EffectActive)
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
        // Logger<TownOfSushiPlugin>.Warning($"Checking Base Conditions");
        var mapId = (MapNames)GameOptionsManager.Instance.currentNormalGameOptions.MapId;
        if (TutorialManager.InstanceExists)
        {
            mapId = (MapNames)AmongUsClient.Instance.TutorialMapId;
        }

        canMoveWithMinigame = true;
        var basicCams = Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x =>
            x.gameObject.name.Contains("Surv_Panel") || x.name.Contains("Cam") ||
            x.name.Contains("BinocularsSecurityConsole"));
        if (mapId is MapNames.Airship)
        {
            // Logger<TownOfSushiPlugin>.Warning($"Checking Airship Conditions");
            basicCams = Object.FindObjectsOfType<SystemConsole>()
                .FirstOrDefault(x => x.gameObject.name.Contains("task_cams"));
            PlayerControl.LocalPlayer.NetTransform.Halt();
            canMoveWithMinigame = false;
        }
        else if (mapId is MapNames.Skeld or MapNames.Dleks)
        {
            // Logger<TownOfSushiPlugin>.Warning($"Checking Skeld Conditions");
            basicCams = Object.FindObjectsOfType<SystemConsole>()
                .FirstOrDefault(x => x.gameObject.name.Contains("SurvConsole"));
            PlayerControl.LocalPlayer.NetTransform.Halt();
            canMoveWithMinigame = false;
        }
        else if (mapId is MapNames.MiraHQ)
        {
            // Logger<TownOfSushiPlugin>.Warning($"Checking Mira HQ Conditions");
            basicCams = Object.FindObjectsOfType<SystemConsole>()
                .FirstOrDefault(x => x.gameObject.name.Contains("SurvLogConsole"));
            if (!OptionGroupSingleton<OperativeOptions>.Instance.MoveOnMira)
            {
                PlayerControl.LocalPlayer.NetTransform.Halt();
                canMoveWithMinigame = false;
            }
        }
        else if (mapId is MapNames.Fungle)
        {
            // Logger<TownOfSushiPlugin>.Warning($"Checking Fungle Conditions");
            PlayerControl.LocalPlayer.NetTransform.Halt();
            canMoveWithMinigame = false;
        }

        if (basicCams == null)
        {
            Logger<TownOfSushiPlugin>.Error($"No Camera System Found!");
            return;
        }

        securityMinigame = Object.Instantiate(basicCams.MinigamePrefab, Camera.main.transform, false);
        securityMinigame.transform.SetParent(Camera.main.transform, false);
        securityMinigame.transform.localPosition = new Vector3(0f, 0f, -50f);
        securityMinigame.Begin(null);
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();
        canMoveWithMinigame = false;

        if (securityMinigame != null)
        {
            securityMinigame.Close();
            RefreshAbilityButton();
            securityMinigame = null;
        }
    }
}