using HarmonyLib;
using InnerNet;
using MiraAPI.GameOptions;
using TMPro;
using TownOfSushi.Options;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Roles.Neutral;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TownOfSushi.Patches;

[HarmonyPatch]
public static class LocalSettings
{
    private static readonly SelectionBehaviour[] AllOptions =
    [
        new()
        {
            Title = "Show Other Ghosts When Dead",
            ObjName = "VisibleGhostsToggle",
            OnClick = () => { return TownOfSushiPlugin.DeadSeeGhosts.Value = !TownOfSushiPlugin.DeadSeeGhosts.Value; },
            DefaultValue = TownOfSushiPlugin.DeadSeeGhosts.Value
        },
        new()
        {
            Title = "Show Vents On Map",
            ObjName = "ShowVentsToggle",
            OnClick = () => { return TownOfSushiPlugin.ShowVents.Value = !TownOfSushiPlugin.ShowVents.Value; },
            DefaultValue = TownOfSushiPlugin.ShowVents.Value
        },
        new()
        {
            Title = "Show Welcome Msg",
            ObjName = "WelcomeMsgToggle",
            OnClick = () =>
            {
                return TownOfSushiPlugin.ShowWelcomeMessage.Value = !TownOfSushiPlugin.ShowWelcomeMessage.Value;
            },
            DefaultValue = TownOfSushiPlugin.ShowWelcomeMessage.Value
        },
        new()
        {
            Title = "Show Summary Msg",
            ObjName = "SummaryMsgToggle",
            OnClick = () =>
            {
                return TownOfSushiPlugin.ShowSummaryMessage.Value = !TownOfSushiPlugin.ShowSummaryMessage.Value;
            },
            DefaultValue = TownOfSushiPlugin.ShowSummaryMessage.Value
        },
        new()
        {
            Title = "Use Basic Crew Colors",
            ObjName = "BasicCrewColorsToggle",
            Enabled = Palette.CrewmateBlue,
            //Disabled = new(0.4f, 0f, 0.6f, 1f),
            Hover = Palette.CrewmateRoleBlue,
            OnClick = () =>
            {
                TownOfSushiColors.UseBasic = !TownOfSushiPlugin.UseCrewmateTeamColor.Value;
                return TownOfSushiPlugin.UseCrewmateTeamColor.Value = !TownOfSushiPlugin.UseCrewmateTeamColor.Value;
            },
            DefaultValue = TownOfSushiPlugin.UseCrewmateTeamColor.Value
        },
        new()
        {
            Title = "Show Shields On Hud",
            //Enabled = new(0f, 1f, 0.7f, 1f),
            //Hover = new(0f, 0.4f, 0f, 1f),
            ObjName = "ShieldsHudToggle",
            OnClick = () => { return TownOfSushiPlugin.ShowShieldHud.Value = !TownOfSushiPlugin.ShowShieldHud.Value; },
            DefaultValue = TownOfSushiPlugin.ShowShieldHud.Value
        },
        new()
        {
            Title = $"Button Scale Factor: {Math.Round(TownOfSushiPlugin.ButtonUIFactor.Value, 2)}x",
            ObjName = "ButtonScaleFloat",
            Enabled = TownOfSushiColors.Inquisitor,
            Disabled = TownOfSushiColors.Juggernaut,
            Hover = TownOfSushiColors.Vampire,
            OnClick = () =>
            {
                if (HudManager.InstanceExists)
                {
                    HudManagerPatches.ResizeUI(1f / TownOfSushiPlugin.ButtonUIFactor.Value);
                }

                var newVal = TownOfSushiPlugin.ButtonUIFactor.Value + 0.1f;
                if (newVal is <= 0.5f or >= 1.6f)
                {
                    newVal = 0.5f;
                }

                TownOfSushiPlugin.ButtonUIFactor.Value = newVal;
                if (HudManager.InstanceExists)
                {
                    HudManagerPatches.ResizeUI(TownOfSushiPlugin.ButtonUIFactor.Value);
                }

                var optionsMenu = GameObject.Find("Menu(Clone)") ? GameObject.Find("Menu(Clone)") : GameObject.Find("OptionsMenu(Clone)");

                if (optionsMenu != null)
                {
                    var title = optionsMenu.transform.FindChild("ButtonScaleFloat");
                    if (title != null && title.transform.GetChild(2).TryGetComponent<TextMeshPro>(out var txt))
                    {
                        txt.text = $"Button Scale Factor: {Math.Round(newVal, 2)}x";
                    }
                }

                return TownOfSushiPlugin.ButtonUIFactor.Value < 1f;
            },
            DefaultValue = TownOfSushiPlugin.ButtonUIFactor.Value < 1f
        },
        new()
        {
            Title = "Offset Buttons If You Can't Vent",
            ObjName = "OffsetButtons",
            OnClick = () => { return TownOfSushiPlugin.OffsetButtons.Value = !TownOfSushiPlugin.OffsetButtons.Value; },
            DefaultValue = TownOfSushiPlugin.OffsetButtons.Value
        },
        new()
        {
            Title = "Sort Guessing By Alignment",
            ObjName = "SortGuessingByAlignment",
            OnClick = () =>
            {
                return TownOfSushiPlugin.SortGuessingByAlignment.Value = !TownOfSushiPlugin.SortGuessingByAlignment.Value;
            },
            DefaultValue = TownOfSushiPlugin.SortGuessingByAlignment.Value
        },
        new()
        {
            Title = "Button Cooldowns are in Decimal Under 10s",
            ObjName = "PreciseCooldowns",
            OnClick = () => { return TownOfSushiPlugin.PreciseCooldowns.Value = !TownOfSushiPlugin.PreciseCooldowns.Value; },
            DefaultValue = TownOfSushiPlugin.PreciseCooldowns.Value
        },
        new()
        {
            Title = $"Arrow Style: {TOSAssets.ArrowSpriteName}",
            ObjName = "ArrowStyle",
            OnClick = () =>
            {
                var newVal = TownOfSushiPlugin.ArrowStyle.Value + 1;
                if (newVal is < 0 or > 3)
                {
                    newVal = 0;
                }

                TownOfSushiPlugin.ArrowStyle.Value = newVal;

                var optionsMenu = GameObject.Find("Menu(Clone)") ? GameObject.Find("Menu(Clone)") : GameObject.Find("OptionsMenu(Clone)");

                if (optionsMenu != null)
                {
                    var title = optionsMenu.transform.FindChild("ArrowStyle");
                    if (title != null && title.transform.GetChild(2).TryGetComponent<TextMeshPro>(out var txt))
                    {
                        txt.text = $"Arrow Style: {TOSAssets.ArrowSpriteName}";
                    }
                }

                return TownOfSushiPlugin.ArrowStyle.Value <= 0;
            },
            DefaultValue = TownOfSushiPlugin.ArrowStyle.Value <= 0
        },
        new()
        {
            Title = "Disable Lobby Music",
            ObjName = "DisableLobbyMusic",
            OnClick = () => { return TownOfSushiPlugin.DisableLobbyMusic.Value = !TownOfSushiPlugin.DisableLobbyMusic.Value; },
            DefaultValue = TownOfSushiPlugin.DisableLobbyMusic.Value
        },
        new()
        {
            Title = "Enable Dark Mode",
            ObjName = "EnableDarkMode",
            OnClick = () => { return TownOfSushiPlugin.EnableDarkMode.Value = !TownOfSushiPlugin.EnableDarkMode.Value; },
            DefaultValue = TownOfSushiPlugin.EnableDarkMode.Value
        },
        new()
        {
            Title = "Disable Nameplates",
            ObjName = "DisableNameplates",
            OnClick = () => { return TownOfSushiPlugin.DisableNameplates.Value = !TownOfSushiPlugin.DisableNameplates.Value; },
            DefaultValue = TownOfSushiPlugin.DisableNameplates.Value
        },
        new()
        {
            Title = "Disable Level Indicators",
            ObjName = "DisableLevelIndicators",
            OnClick = () => { return TownOfSushiPlugin.DisableLevelIndicators.Value = !TownOfSushiPlugin.DisableLevelIndicators.Value; },
            DefaultValue = TownOfSushiPlugin.DisableLevelIndicators.Value
        }
    ];

    private static GameObject popUp;
    private static TextMeshPro titleText;

    private static ToggleButtonBehaviour buttonPrefab;
    private static Vector3? _origin;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static void MainMenuManager_StartPostfix(MainMenuManager __instance)
    {
        // Prefab for the title
        var go = new GameObject("TitleText");
        var tmp = go.AddComponent<TextMeshPro>();
        tmp.fontSize = 4;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.transform.localPosition += Vector3.left * 0.2f;
        titleText = Object.Instantiate(tmp);
        titleText.gameObject.SetActive(false);
        Object.DontDestroyOnLoad(titleText);
    }

    [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
    [HarmonyPostfix]
    public static void OptionsMenuBehaviour_StartPostfix(OptionsMenuBehaviour __instance)
    {
        if (!__instance.CensorChatButton)
        {
            return;
        }

        if (!popUp)
        {
            CreateCustom(__instance);
        }

        if (!buttonPrefab)
        {
            buttonPrefab = Object.Instantiate(__instance.CensorChatButton);
            Object.DontDestroyOnLoad(buttonPrefab);
            buttonPrefab.name = "CensorChatPrefab";
            buttonPrefab.gameObject.SetActive(false);
        }

        SetUpOptions();
        InitializeMoreButton(__instance);
    }

    private static void CreateCustom(OptionsMenuBehaviour prefab)
    {
        popUp = Object.Instantiate(prefab.gameObject);
        Object.DontDestroyOnLoad(popUp);
        var transform = popUp.transform;
        var pos = transform.localPosition;
        pos.z = -810f;
        transform.localPosition = pos;

        Object.Destroy(popUp.GetComponent<OptionsMenuBehaviour>());
        foreach (var gObj in popUp.gameObject.GetAllChildren())
        {
            if (gObj.name == "Background")
            {
                var scale = gObj.transform.localScale;
                gObj.transform.localScale = new Vector3(scale.x * 1.35f, scale.y, scale.z);
            }
            else if (gObj.name == "CloseButton")
            {
                gObj.transform.localPosition += Vector3.left * 0.45f;
            }
            else
            {
                Object.Destroy(gObj);
            }
        }

        popUp.SetActive(false);
    }

    private static void InitializeMoreButton(OptionsMenuBehaviour __instance)
    {
        var moreOptions = Object.Instantiate(buttonPrefab, __instance.CensorChatButton.transform.parent);
        var transform = __instance.CensorChatButton.transform;
        __instance.CensorChatButton.Text.transform.localScale = new Vector3(1 / 0.66f, 1, 1);
        _origin ??= transform.localPosition;

        transform.localPosition = _origin.Value + Vector3.left * 0.45f;
        transform.localScale = new Vector3(0.66f, 1, 1);
        __instance.EnableFriendInvitesButton.transform.localScale = new Vector3(0.66f, 1, 1);
        __instance.EnableFriendInvitesButton.transform.localPosition += Vector3.right * 0.5f;
        __instance.EnableFriendInvitesButton.Text.transform.localScale = new Vector3(1.2f, 1, 1);

        moreOptions.transform.localPosition = _origin.Value + Vector3.right * 4f / 3f;
        moreOptions.transform.localScale = new Vector3(0.66f, 1, 1);

        moreOptions.gameObject.SetActive(true);
        moreOptions.Text.text = "TOS Client Options";
        moreOptions.Text.transform.localScale = new Vector3(1 / 0.66f, 1, 1);
        var moreOptionsButton = moreOptions.GetComponent<PassiveButton>();
        moreOptionsButton.OnClick = new Button.ButtonClickedEvent();
        moreOptionsButton.OnClick.AddListener((Action)(() =>
        {
            var closeUnderlying = false;
            if (!popUp)
            {
                return;
            }

            if (__instance.transform.parent && __instance.transform.parent == HudManager.Instance.transform)
            {
                popUp.transform.SetParent(HudManager.Instance.transform);
                popUp.transform.localPosition = new Vector3(0, 0, -800f);
                closeUnderlying = true;
            }
            else
            {
                popUp.transform.SetParent(null);
                Object.DontDestroyOnLoad(popUp);
            }

            CheckSetTitle();
            RefreshOpen();
            if (closeUnderlying)
            {
                __instance.Close();
            }
        }));
    }

    private static void RefreshOpen()
    {
        popUp.gameObject.SetActive(false);
        popUp.gameObject.SetActive(true);
        SetUpOptions();
    }

    private static void CheckSetTitle()
    {
        if (!popUp || popUp.GetComponentInChildren<TextMeshPro>() || !titleText)
        {
            return;
        }

        var title = Object.Instantiate(titleText, popUp.transform);
        title.GetComponent<RectTransform>().localPosition = Vector3.up * 2.3f;
        title.gameObject.SetActive(true);
        title.gameObject.layer = LayerMask.NameToLayer("UI");
        title.text = "<size=80%>Town of Sushi\n</size><size=60%>Client Options</size>\n";
        title.name = "TitleText";
    }

    private static void SetUpOptions()
    {
        if (popUp.transform.GetComponentInChildren<ToggleButtonBehaviour>())
        {
            return;
        }

        for (var i = 0; i < AllOptions.Length; i++)
        {
            var info = AllOptions[i];

            var button = Object.Instantiate(buttonPrefab, popUp.transform);
            var pos = new Vector3(-2.34f + i % 3 * 2.34f, 1.3f - i / 3 * 0.8f, -.5f);

            var transform = button.transform;
            transform.localPosition = pos;

            button.onState = info.DefaultValue;
            button.Background.color = button.onState ? info.Enabled : info.Disabled;

            button.Text.text = info.Title;
            button.Text.fontSizeMin = button.Text.fontSizeMax = 1.8f;
            button.Text.font = Object.Instantiate(titleText.font);
            button.Text.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);

            button.name = info.ObjName;
            button.gameObject.SetActive(true);

            var passiveButton = button.GetComponent<PassiveButton>();
            var colliderButton = button.GetComponent<BoxCollider2D>();

            colliderButton.size = new Vector2(2.2f, .7f);

            passiveButton.OnClick = new Button.ButtonClickedEvent();
            passiveButton.OnMouseOut = new UnityEvent();
            passiveButton.OnMouseOver = new UnityEvent();

            passiveButton.OnClick.AddListener((Action)(() =>
            {
                button.onState = info.OnClick();
                button.Background.color = button.onState ? info.Enabled : info.Disabled;
            }));

            passiveButton.OnMouseOver.AddListener((Action)(() => button.Background.color = info.Hover));
            passiveButton.OnMouseOut.AddListener((Action)(() =>
                button.Background.color = button.onState ? info.Enabled : info.Disabled));

            foreach (var spr in button.gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                spr.size = new Vector2(2.2f, .7f);
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    [HarmonyPostfix]
    public static void HideGhosts()
    {
        if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started)
        {
            return;
        }

        if (!PlayerControl.LocalPlayer.Data.IsDead)
        {
            return;
        }

        if (MeetingHud.Instance)
        {
            return;
        }

        if (!OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow)
        {
            return;
        }

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.AmOwner)
            {
                continue;
            }

            if (!player.Data.IsDead)
            {
                continue;
            }

            switch (player.Data.Role)
            {
                case PhantomTOSRole { Caught: false }:
                case HaunterRole { Caught: false }:
                    continue;
            }

            var show = TownOfSushiPlugin.DeadSeeGhosts.Value;
            var bodyForms = player.gameObject.transform.GetChild(1).gameObject;

            foreach (var form in bodyForms.GetAllChildren())
            {
                if (form.activeSelf)
                {
                    form.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, show ? 1f : 0f);
                }
            }

            if (player.cosmetics.HasPetEquipped())
            {
                player.cosmetics.CurrentPet.Visible = show;
            }

            player.cosmetics.gameObject.SetActive(show);
            player.gameObject.transform.GetChild(3).gameObject.SetActive(show);
        }
    }

    public static IEnumerable<GameObject> GetAllChildren(this GameObject go)
    {
        for (var i = 0; i < go.transform.childCount; i++)
        {
            yield return go.transform.GetChild(i).gameObject;
        }
    }

    public sealed class SelectionBehaviour
    {
        public bool DefaultValue;
        public string ObjName;

        public Func<bool> OnClick;
        public string Title;
        public Color Enabled { get; set; } = Color.green;
        public Color Disabled { get; set; } = Palette.ImpostorRed;
        public Color Hover { get; set; } = new Color32(34, 139, 34, 255);
    }
}