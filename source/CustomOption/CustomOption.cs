using System.IO;
using System.Text;
using BepInEx.Configuration;
using TMPro;
using static TownOfSushi.CustomOption.CustomOption;

namespace TownOfSushi.CustomOption 
{
    public class CustomOption 
    {
        public enum CustomOptionType 
        {
            General,
            Crewmate,
            Neutral,
            Impostor,
            NK,
            ModifierAbility,
        }

        public static List<CustomOption> Options = new List<CustomOption>();
        public static int preset = 0;
        public static ConfigEntry<string> vanillaSettings;

        public int id;
        public string name;
        public string format;
        public System.Object[] selections;

        public int defaultSelection;
        public ConfigEntry<int> entry;
        public int selection;
        public OptionBehaviour optionBehaviour;
        public CustomOption parent;
        public bool isHeader;
        public CustomOptionType type;
        public Action onChange = null;
        public string heading = "";
        public Func<bool> extraCondition;

        // Option creation

        public CustomOption(int id, CustomOptionType type, string name, System.Object[] selections, System.Object defaultValue, CustomOption parent, bool isHeader, Action onChange = null, string heading = "", string format = "", Func<bool> extraCondition = null) {
            this.id = id;
            this.name = parent == null ? name : "- " + name;
            this.selections = selections;
            int index = Array.IndexOf(selections, defaultValue);
            this.defaultSelection = index >= 0 ? index : 0;
            this.parent = parent;
            this.isHeader = isHeader;
            this.type = type;
            this.onChange = onChange;
            this.heading = heading;
            this.format = format;
            selection = 0;
            if (id != 0) {
                entry = TownOfSushi.Instance.Config.Bind($"Preset{preset}", id.ToString(), defaultSelection);
                selection = Mathf.Clamp(entry.Value, 0, selections.Length - 1);
            }
            Options.Add(this);
        }

        public static CustomOption Create(int id, CustomOptionType type, string name, string[] selections, CustomOption parent = null, bool isHeader = false, Action onChange = null, string heading = "") {
            return new CustomOption(id, type, name, selections, "", parent, isHeader, onChange, heading, "");
        }

        public static CustomOption Create(int id, CustomOptionType type, string name, float defaultValue, float min, float max, float step, CustomOption parent = null, bool isHeader = false, Action onChange = null, string heading = "", string format = "") {
            List<object> selections = new();
            for (float s = min; s <= max; s += step)
                selections.Add(s);
            return new CustomOption(id, type, name, selections.ToArray(), defaultValue, parent, isHeader, onChange, heading, format);
        }

        public static CustomOption Create(int id, CustomOptionType type, string name, bool defaultValue, CustomOption parent = null, bool isHeader = false, Action onChange = null, string heading = "") 
        {
            return new CustomOption(id, type, name, new string[]{"Off", "On"}, defaultValue ? "On" : "Off", parent, isHeader, onChange, heading, "");
        }

        // Static behaviour

        public static void SwitchPreset(int newPreset) 
        {
            SaveVanillaOptions();
            preset = newPreset;
            vanillaSettings = TownOfSushi.Instance.Config.Bind($"Preset{preset}", "GameOptions", "");
            LoadVanillaOptions();
            foreach (CustomOption option in CustomOption.Options) {
                if (option.id == 0) continue;

                option.entry = TownOfSushi.Instance.Config.Bind($"Preset{preset}", option.id.ToString(), option.defaultSelection);
                option.selection = Mathf.Clamp(option.entry.Value, 0, option.selections.Length - 1);
                if (option.optionBehaviour != null && option.optionBehaviour is StringOption stringOption) {
                    stringOption.oldValue = stringOption.Value = option.selection;
                    stringOption.ValueText.text = $"{option.selections[option.selection].ToString()}{option.format}";
                }
            }
        }

        public static void SaveVanillaOptions() 
        {
            vanillaSettings.Value = Convert.ToBase64String(GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameManager.Instance.LogicOptions.currentGameOptions, false));
        }

        public static bool LoadVanillaOptions() {
            string optionsString = vanillaSettings.Value;
            if (optionsString == "") return false;
            IGameOptions gameOptions = GameOptionsManager.Instance.gameOptionsFactory.FromBytes(Convert.FromBase64String(optionsString));
            if (gameOptions.Version < 8) {
                TownOfSushi.Logger.LogMessage("tried to paste old settings, not doing this!");
                return false;
            } 
            GameOptionsManager.Instance.GameHostOptions = gameOptions;
            GameOptionsManager.Instance.CurrentGameOptions = GameOptionsManager.Instance.GameHostOptions;
            GameManager.Instance.LogicOptions.SetGameOptions(GameOptionsManager.Instance.CurrentGameOptions);
            GameManager.Instance.LogicOptions.SyncOptions();
            return true;
        }

        public static void ShareOptionChange(uint optionId) 
        {
            var option = Options.FirstOrDefault(x => x.id == optionId);
            if (option == null) return;
            var writer = AmongUsClient.Instance!.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareOptions, SendOption.Reliable, -1);
            writer.Write((byte)1);
            writer.WritePacked((uint)option.id);
            writer.WritePacked(Convert.ToUInt32(option.selection));
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void ShareOptionSelections() {
            if (PlayerControl.AllPlayerControls.Count <= 1 || AmongUsClient.Instance!.AmHost == false && PlayerControl.LocalPlayer == null) return;
            var optionsList = new List<CustomOption>(CustomOption.Options);
            while (optionsList.Any())
            {
                byte amount = (byte) Math.Min(optionsList.Count, 200); // takes less than 3 bytes per option on average
                var writer = AmongUsClient.Instance!.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareOptions, SendOption.Reliable, -1);
                writer.Write(amount);
                for (int i = 0; i < amount; i++)
                {
                    var option = optionsList[0];
                    optionsList.RemoveAt(0);
                    writer.WritePacked((uint) option.id);
                    writer.WritePacked(Convert.ToUInt32(option.selection));
                }
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }

        // Getter

        public int GetSelection() {
            return selection;
        }

        public bool GetBool() {
            return selection > 0;
        }

        public float GetFloat() {
            return (float)selections[selection];
        }

        public float GetInt() {
            return (int)selections[selection];
        }

        public int GetQuantity() {
            return selection + 1;
        }


        public void UpdateSelection(int newSelection, bool notifyUsers = true) {
            newSelection = Mathf.Clamp((newSelection + selections.Length) % selections.Length, 0, selections.Length - 1);
            if (AmongUsClient.Instance?.AmClient == true && notifyUsers && selection != newSelection) {
                DestroyableSingleton<HudManager>.Instance.Notifier.AddSettingsChangeMessage((StringNames)(this.id + 6000), $"{selections[newSelection].ToString()}{format}", false);
                try {
                    if (GameStartManager.Instance != null && GameStartManager.Instance.LobbyInfoPane != null && GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane != null && GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane.gameObject.activeSelf) {
                        LobbyViewSettingsPaneChangeTabPatch.Postfix(GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane, GameStartManager.Instance.LobbyInfoPane.LobbyViewSettingsPane.currentTab);
                    }
                } catch { }
            }
            selection = newSelection;
            try {
                if (onChange != null) onChange();
            } catch { }
            if (optionBehaviour != null && optionBehaviour is StringOption stringOption) {
                stringOption.oldValue = stringOption.Value = selection;
                stringOption.ValueText.text = $"{selections[selection].ToString()}{format}";
                if (AmongUsClient.Instance?.AmHost == true && PlayerControl.LocalPlayer) {
                    if (id == 0 && selection != preset) {
                        SwitchPreset(selection); // Switch presets
                        ShareOptionSelections();
                    } else if (entry != null) {
                        entry.Value = selection; // Save selection to config
                        ShareOptionChange((uint)id);// Share single selection
                    }
                }
            } else if (id == 0 && AmongUsClient.Instance?.AmHost == true && PlayerControl.LocalPlayer) {  // Share the preset switch for random maps, even if the menu isnt open!
                SwitchPreset(selection);
                ShareOptionSelections();// Share all selections
            }

        }

        public static byte[] SerializeOptions() 
        {
            using (MemoryStream memoryStream = new MemoryStream()) {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream)) {
                    int lastId = -1;
                    foreach (var option in CustomOption.Options.OrderBy(x => x.id)) {
                        if (option.id == 0) continue;
                        bool consecutive = lastId + 1 == option.id;
                        lastId = option.id;

                        binaryWriter.Write((byte)(option.selection + (consecutive ? 128 : 0)));
                        if (!consecutive) binaryWriter.Write((ushort)option.id);
                    }
                    binaryWriter.Flush();
                    memoryStream.Position = 0L;
                    return memoryStream.ToArray();
                }
            }
        }

        public static int DeserializeOptions(byte[] inputValues) {
            BinaryReader reader = new BinaryReader(new MemoryStream(inputValues));
            int lastId = -1;
            bool somethingApplied = false;
            int errors = 0;
            while (reader.BaseStream.Position < inputValues.Length) {
                try {
                    int selection = reader.ReadByte();
                    int id = -1;
                    bool consecutive = selection >= 128;
                    if (consecutive) {
                        selection -= 128;
                        id = lastId + 1;
                    } else {
                        id = reader.ReadUInt16();
                    }
                    if (id == 0) continue;
                    lastId = id;
                    CustomOption option = Options.First(option => option.id == id);
                    option.entry = TownOfSushi.Instance.Config.Bind($"Preset{preset}", option.id.ToString(), option.defaultSelection);
                    option.selection = selection;
                    if (option.optionBehaviour != null && option.optionBehaviour is StringOption stringOption) {
                        stringOption.oldValue = stringOption.Value = option.selection;
                        stringOption.ValueText.text = option.selections[option.selection].ToString();
                    }
                    somethingApplied = true;
                } catch (Exception e) {
                    TownOfSushi.Logger.LogWarning($"id:{lastId}:{e}: while deserializing - tried to paste invalid settings!");
                    errors++;
                }
            }
            return Convert.ToInt32(somethingApplied) + (errors > 0 ? 0 : 1);
        }

        // Copy to or paste from clipboard (as string)
        public static void copyToClipboard() {
            GUIUtility.systemCopyBuffer = $"{TownOfSushi.VersionString}!{Convert.ToBase64String(SerializeOptions())}!{vanillaSettings.Value}";
        }

        public static int pasteFromClipboard() {
            string allSettings = GUIUtility.systemCopyBuffer;
            int torOptionsFine = 0;
            bool vanillaOptionsFine = false;
            try {
                var settingsSplit = allSettings.Split("!");
                Version versionInfo = System.Version.Parse(settingsSplit[0]);
                string torSettings = settingsSplit[1];
                string vanillaSettingsSub = settingsSplit[2];
                torOptionsFine = DeserializeOptions(Convert.FromBase64String(torSettings));
                ShareOptionSelections();
                if (TownOfSushi.Version > versionInfo && versionInfo < System.Version.Parse("1.0.0")) 
                {
                    vanillaOptionsFine = false;
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Host Info: Pasting vanilla settings failed, TOS Options applied!");
                }
                else 
                {
                    vanillaSettings.Value = vanillaSettingsSub;
                    vanillaOptionsFine = LoadVanillaOptions();
                }
            } catch (Exception e) 
            {
                TownOfSushi.Logger.LogWarning($"{e}: tried to paste invalid settings!\n{allSettings}");
                string errorStr = allSettings.Length > 2 ? allSettings.Substring(0, 3) : "(empty clipboard) ";
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"Host Info: You tried to paste invalid settings: \"{errorStr}...\"");
            }
            return Convert.ToInt32(vanillaOptionsFine) + torOptionsFine;
        }
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.ChangeTab))]
    class GameOptionsMenuChangeTabPatch 
    {
        public static void Postfix(GameSettingMenu __instance, int tabNum, bool previewOnly) 
        {
            if (previewOnly) return;
            foreach (var tab in GameOptionsMenuStartPatch.currentTabs) 
            {
                if (tab != null)
                    tab.SetActive(false);
            }
            foreach (var pbutton in GameOptionsMenuStartPatch.currentButtons) 
            {
                pbutton.SelectButton(false);
            }
            if (tabNum > 2) 
            {
                tabNum -= 3;
                GameOptionsMenuStartPatch.currentTabs[tabNum].SetActive(true);
                GameOptionsMenuStartPatch.currentButtons[tabNum].SelectButton(true);
            }
        }
    }

    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.SetTab))]
    class LobbyViewSettingsPaneRefreshTabPatch 
    {
        public static bool Prefix(LobbyViewSettingsPane __instance) 
        {
            if ((int)__instance.currentTab < 15) 
            {
                LobbyViewSettingsPaneChangeTabPatch.Postfix(__instance, __instance.currentTab);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.ChangeTab))]
    class LobbyViewSettingsPaneChangeTabPatch 
    {
        public static void Postfix(LobbyViewSettingsPane __instance, StringNames category) 
        {
            int tabNum = (int)category;

            foreach (var pbutton in LobbyViewSettingsPatch.currentButtons) 
            {
                pbutton.SelectButton(false);
            }
            if (tabNum > 20) // StringNames are in the range of 3000+ 
                return;
            __instance.taskTabButton.SelectButton(false);

            if (tabNum > 2)
            {
                tabNum -= 3;
                //GameOptionsMenuStartPatch.currentTabs[tabNum].SetActive(true);
                LobbyViewSettingsPatch.currentButtons[tabNum].SelectButton(true);
                LobbyViewSettingsPatch.DrawTab(__instance, LobbyViewSettingsPatch.currentButtonTypes[tabNum]);
            }
        }
    }

    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.Update))]
    class LobbyViewSettingsPaneUpdatePatch 
    {
        public static void Postfix(LobbyViewSettingsPane __instance) 
        {
            if (LobbyViewSettingsPatch.currentButtons.Count == 0) 
            {
                LobbyViewSettingsPatch.gameModeChangedFlag = true;
                LobbyViewSettingsPatch.Postfix(__instance);
                
            }
        }
    }


    [HarmonyPatch(typeof(LobbyViewSettingsPane), nameof(LobbyViewSettingsPane.Awake))]
    class LobbyViewSettingsPatch
    {
        public static List<PassiveButton> currentButtons = new();
        public static List<CustomOption.CustomOptionType> currentButtonTypes = new();
        public static bool gameModeChangedFlag = false;

        public static void CreateCustomButton(LobbyViewSettingsPane __instance, int targetMenu, string buttonName, string buttonText, CustomOption.CustomOptionType optionType) 
        {
            buttonName = "View" + buttonName;
            var buttonTemplate = GameObject.Find("OverviewTab");
            var torSettingsButton = GameObject.Find(buttonName);
            if (torSettingsButton == null) {
                torSettingsButton = GameObject.Instantiate(buttonTemplate, buttonTemplate.transform.parent);
                torSettingsButton.transform.localPosition += Vector3.right * 1.75f * (targetMenu - 2);
                torSettingsButton.name = buttonName;
                __instance.StartCoroutine(Effects.Lerp(2f, new Action<float>(p => { torSettingsButton.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = buttonText; })));
                var torSettingsPassiveButton = torSettingsButton.GetComponent<PassiveButton>();
                torSettingsPassiveButton.OnClick.RemoveAllListeners();
                torSettingsPassiveButton.OnClick.AddListener((System.Action)(() => {
                    __instance.ChangeTab((StringNames)targetMenu);
                }));
                torSettingsPassiveButton.OnMouseOut.RemoveAllListeners();
                torSettingsPassiveButton.OnMouseOver.RemoveAllListeners();
                torSettingsPassiveButton.SelectButton(false);
                currentButtons.Add(torSettingsPassiveButton);
                currentButtonTypes.Add(optionType);
            }
        }

        public static void Postfix(LobbyViewSettingsPane __instance) 
        {
            currentButtons.ForEach(x => x?.Destroy());
            currentButtons.Clear();
            currentButtonTypes.Clear();

            removeVanillaTabs(__instance);

            CreateSettingTabs(__instance);

        }

        public static void removeVanillaTabs(LobbyViewSettingsPane __instance) 
        {
            GameObject.Find("RolesTabs")?.Destroy();
            var overview = GameObject.Find("OverviewTab");
            if (!gameModeChangedFlag) {
                overview.transform.localScale = new Vector3(0.5f * overview.transform.localScale.x, overview.transform.localScale.y, overview.transform.localScale.z);
                overview.transform.localPosition += new Vector3(-1.2f, 0f, 0f);
                
            }
            overview.transform.Find("FontPlacer").transform.localScale = new Vector3(1.35f, 1f, 1f);
            overview.transform.Find("FontPlacer").transform.localPosition = new Vector3(-0.6f, -0.1f, 0f);
            gameModeChangedFlag = false;
        }

        public static void DrawTab(LobbyViewSettingsPane __instance, CustomOption.CustomOptionType optionType) 
        {

            var relevantOptions = CustomOption.Options.Where(x => x.type == optionType).ToList();
           
            if ((int)optionType == 99) 
            {
                // Create 4 Groups with Role settings only
                relevantOptions.Clear();
                relevantOptions.AddRange(Options.Where(x => x.type == CustomOption.CustomOptionType.Crewmate && x.isHeader));
                relevantOptions.AddRange(Options.Where(x => x.type == CustomOption.CustomOptionType.Neutral && x.isHeader));
                relevantOptions.AddRange(Options.Where(x => x.type == CustomOption.CustomOptionType.NK && x.isHeader));
                relevantOptions.AddRange(Options.Where(x => x.type == CustomOption.CustomOptionType.Impostor && x.isHeader));
                relevantOptions.AddRange(Options.Where(x => x.type == CustomOption.CustomOptionType.ModifierAbility && x.isHeader));
            }

            for (int j = 0; j < __instance.settingsInfo.Count; j++) {
                __instance.settingsInfo[j].gameObject.Destroy();
            }
            __instance.settingsInfo.Clear();

            float num = 1.44f;
            int i = 0;
            int singles = 0;
            int headers = 0;
            int lines = 0;
            var curType = CustomOptionType.ModifierAbility;

            foreach (var option in relevantOptions) 
            {
                if (option.isHeader && (int)optionType != 99 || (int)optionType == 99 && curType != option.type) {
                    curType = option.type;
                    if (i != 0) num -= 0.59f;
                    if (i % 2 != 0) singles++;
                    headers++; // for header
                    CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate<CategoryHeaderMasked>(__instance.categoryHeaderOrigin);
                    categoryHeaderMasked.SetHeader(StringNames.ImpostorsCategory, 61);
                    categoryHeaderMasked.Title.text = option.heading != "" ? option.heading : option.name;
                    if ((int)optionType == 99)
                        categoryHeaderMasked.Title.text = new Dictionary<CustomOptionType, string>() { 
                            { CustomOptionType.Crewmate, "Crewmate Roles" }, 
                            { CustomOptionType.Neutral, "Neutral Roles" }, 
                            { CustomOptionType.NK, "Neutral Killing Roles" }, 
                            { CustomOptionType.Impostor, "Impostor Roles" }, 
                            { CustomOptionType.ModifierAbility, "Modifiers & Abilities" }}[curType];
                    categoryHeaderMasked.Title.outlineColor = Color.white;
                    categoryHeaderMasked.Title.outlineWidth = 0.2f;
                    categoryHeaderMasked.transform.SetParent(__instance.settingsContainer);
                    categoryHeaderMasked.transform.localScale = Vector3.one;
                    categoryHeaderMasked.transform.localPosition = new Vector3(-9.77f, num, -2f);
                    __instance.settingsInfo.Add(categoryHeaderMasked.gameObject);
                    num -= 0.85f;
                    i = 0;
                }

                ViewSettingsInfoPanel viewSettingsInfoPanel = UnityEngine.Object.Instantiate<ViewSettingsInfoPanel>(__instance.infoPanelOrigin);
                viewSettingsInfoPanel.transform.SetParent(__instance.settingsContainer);
                viewSettingsInfoPanel.transform.localScale = Vector3.one;
                float num2;
                if (i % 2 == 0) {
                    lines++;
                    num2 = -8.95f;
                    if (i > 0) {
                        num -= 0.59f;
                    }
                } else {
                    num2 = -3f;
                }
                viewSettingsInfoPanel.transform.localPosition = new Vector3(num2, num, -2f);
                int value = option.GetSelection();
                viewSettingsInfoPanel.SetInfo(StringNames.ImpostorsCategory, $"{option.selections[value].ToString()}{option.format}", 61);
                viewSettingsInfoPanel.titleText.text = option.name;
                viewSettingsInfoPanel.background.gameObject.SetActive(true);
                if (option.isHeader && (int)optionType != 99 && option.heading == "" && (option.type == CustomOptionType.Neutral || option.type == CustomOptionType.Crewmate || option.type == CustomOptionType.Impostor || option.type == CustomOptionType.NK || option.type == CustomOptionType.ModifierAbility)) {
                    viewSettingsInfoPanel.titleText.text = "Spawn Chance";
                }
                if ((int)optionType == 99) {
                    viewSettingsInfoPanel.titleText.outlineColor = Color.white;
                    viewSettingsInfoPanel.titleText.outlineWidth = 0.2f;
                    if (option.type == CustomOptionType.ModifierAbility)
                        viewSettingsInfoPanel.settingText.text = viewSettingsInfoPanel.settingText.text + GameOptionsDataPatch.BuildModifierExtras(option);
                }
                __instance.settingsInfo.Add(viewSettingsInfoPanel.gameObject);

                i++;
            }
            float actual_spacing = (headers * 0.85f + lines * 0.59f) / (headers + lines);
            __instance.scrollBar.CalculateAndSetYBounds((float)(__instance.settingsInfo.Count + singles * 2 + headers), 2f, 6f, actual_spacing);

        }

        public static void CreateSettingTabs(LobbyViewSettingsPane __instance) 
        {
            // Handle different gamemodes and tabs needed therein.
            int next = 3;

            // create TOS settings
            CreateCustomButton(__instance, next++, "TORSettings", "TOS Settings", CustomOptionType.General);
            // Crew
            CreateCustomButton(__instance, next++, "CrewmateSettings", "Crewmate Roles", CustomOptionType.Crewmate);
            // Neutral
            CreateCustomButton(__instance, next++, "NeutralSettings", "Neutral Roles", CustomOptionType.Neutral);
            // NK
            CreateCustomButton(__instance, next++, "NeutralKSettings", "NK Roles", CustomOptionType.NK);
            // IMp
            CreateCustomButton(__instance, next++, "ImpostorSettings", "Impostor Roles", CustomOptionType.Impostor);
            // Modifier
            CreateCustomButton(__instance, next++, "ModifierSettings", "Modifiers/Abilities", CustomOptionType.ModifierAbility);
        }
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.CreateSettings))]
    class GameOptionsMenuCreateSettingsPatch 
    {
        public static void Postfix(GameOptionsMenu __instance) 
        {
            if (__instance.gameObject.name == "GAME SETTINGS TAB")
                adaptTaskCount(__instance);
        }

        private static void adaptTaskCount(GameOptionsMenu __instance) 
        {
            // Adapt task count for main options
            var commonTasksOption = __instance.Children.ToArray().FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumCommonTasks).Cast<NumberOption>();
            if (commonTasksOption != null) commonTasksOption.ValidRange = new FloatRange(0f, 4f);
            var shortTasksOption = __instance.Children.ToArray().FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumShortTasks).TryCast<NumberOption>();
            if (shortTasksOption != null) shortTasksOption.ValidRange = new FloatRange(0f, 23f);
            var longTasksOption = __instance.Children.ToArray().FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumLongTasks).TryCast<NumberOption>();
            if (longTasksOption != null) longTasksOption.ValidRange = new FloatRange(0f, 15f);
        }
    }


    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    class GameOptionsMenuStartPatch 
    {
        public static List<GameObject> currentTabs = new();
        public static List<PassiveButton> currentButtons = new();

        public static void Postfix(GameSettingMenu __instance) 
        {
            currentTabs.ForEach(x => x?.Destroy());
            currentButtons.ForEach(x => x?.Destroy());
            currentTabs = new();
            currentButtons = new();

            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;

            RemoveVanillaTabs(__instance);

            CreateSettingTabs(__instance);

            var GOMGameObject = GameObject.Find("GAME SETTINGS TAB");


            // create copy to clipboard and paste from clipboard buttons.
            var template = GameObject.Find("PlayerOptionsMenu(Clone)").transform.Find("CloseButton").gameObject;
            var holderGO = new GameObject("copyPasteButtonParent");
            var bgrenderer = holderGO.AddComponent<SpriteRenderer>();
            bgrenderer.sprite = LoadSpriteFromResources("TownOfSushi.Resources.CopyPasteBG.png", 175f);
            holderGO.transform.SetParent(template.transform.parent, false);
            holderGO.transform.localPosition = template.transform.localPosition + new Vector3(-8.3f, 0.73f, -2f);
            holderGO.layer = template.layer;
            holderGO.SetActive(true);
            var copyButton = GameObject.Instantiate(template, holderGO.transform);
            copyButton.transform.localPosition = new Vector3(-0.3f, 0.02f, -2f);
            var copyButtonPassive = copyButton.GetComponent<PassiveButton>();
            var copyButtonRenderer = copyButton.GetComponentInChildren<SpriteRenderer>();
            var copyButtonActiveRenderer = copyButton.transform.GetChild(1).GetComponent<SpriteRenderer>();
            copyButtonRenderer.sprite = LoadSpriteFromResources("TownOfSushi.Resources.Copy.png", 100f);
            copyButton.transform.GetChild(1).transform.localPosition = Vector3.zero;
            copyButtonActiveRenderer.sprite = LoadSpriteFromResources("TownOfSushi.Resources.CopyActive.png", 100f);
            copyButtonPassive.OnClick.RemoveAllListeners();
            copyButtonPassive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            copyButtonPassive.OnClick.AddListener((System.Action)(() => {
                copyToClipboard();
                copyButtonRenderer.color = Color.green;
                copyButtonActiveRenderer.color = Color.green;
                __instance.StartCoroutine(Effects.Lerp(1f, new System.Action<float>((p) => {
                    if (p > 0.95) {
                        copyButtonRenderer.color = Color.white;
                        copyButtonActiveRenderer.color = Color.white;
                    }
                })));
            }));
            var pasteButton = GameObject.Instantiate(template, holderGO.transform);
            pasteButton.transform.localPosition = new Vector3(0.3f, 0.02f, -2f);
            var pasteButtonPassive = pasteButton.GetComponent<PassiveButton>();
            var pasteButtonRenderer = pasteButton.GetComponentInChildren<SpriteRenderer>();
            var pasteButtonActiveRenderer = pasteButton.transform.GetChild(1).GetComponent<SpriteRenderer>();
            pasteButtonRenderer.sprite = LoadSpriteFromResources("TownOfSushi.Resources.Paste.png", 100f);
            pasteButtonActiveRenderer.sprite = LoadSpriteFromResources("TownOfSushi.Resources.PasteActive.png", 100f);
            pasteButtonPassive.OnClick.RemoveAllListeners();
            pasteButtonPassive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            pasteButtonPassive.OnClick.AddListener((System.Action)(() => {
                pasteButtonRenderer.color = Color.yellow;
                int success = pasteFromClipboard();
                pasteButtonRenderer.color = success == 3 ? Color.green : success == 0 ? Color.red : Color.yellow;
                pasteButtonActiveRenderer.color = success == 3 ? Color.green : success == 0 ? Color.red : Color.yellow;
                __instance.StartCoroutine(Effects.Lerp(1f, new System.Action<float>((p) => {
                    if (p > 0.95) {
                        pasteButtonRenderer.color = Color.white;
                        pasteButtonActiveRenderer.color = Color.white;
                    }
                })));
            }));
        }

        private static void CreateSettings(GameOptionsMenu menu, List<CustomOption> options) 
        {
            float num = 1.5f;
            foreach (CustomOption option in options) {
                if (option.isHeader) {
                    CategoryHeaderMasked categoryHeaderMasked = UnityEngine.Object.Instantiate<CategoryHeaderMasked>(menu.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                    categoryHeaderMasked.SetHeader(StringNames.ImpostorsCategory, 20);
                    categoryHeaderMasked.Title.text = option.heading != "" ? option.heading : option.name;
                    categoryHeaderMasked.Title.outlineColor = Color.white;
                    categoryHeaderMasked.Title.outlineWidth = 0.2f;
                    categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
                    categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, num, -2f);
                    num -= 0.63f;
                }
                OptionBehaviour optionBehaviour = UnityEngine.Object.Instantiate<StringOption>(menu.stringOptionOrigin, Vector3.zero, Quaternion.identity, menu.settingsContainer);
                optionBehaviour.transform.localPosition = new Vector3(0.952f, num, -2f);
                optionBehaviour.SetClickMask(menu.ButtonClickMask);

                // "SetUpFromData"
                SpriteRenderer[] componentsInChildren = optionBehaviour.GetComponentsInChildren<SpriteRenderer>(true);
                for (int i = 0; i < componentsInChildren.Length; i++) {
                    componentsInChildren[i].material.SetInt(PlayerMaterial.MaskLayer, 20);
                }
                foreach (TextMeshPro textMeshPro in optionBehaviour.GetComponentsInChildren<TextMeshPro>(true)) {
                    textMeshPro.fontMaterial.SetFloat("_StencilComp", 3f);
                    textMeshPro.fontMaterial.SetFloat("_Stencil", (float)20);
                }

                var stringOption = optionBehaviour as StringOption;
                stringOption.OnValueChanged = new Action<OptionBehaviour>((o) => { });
                stringOption.TitleText.text = option.name;
                if (option.isHeader && option.heading == "" && (option.type == CustomOptionType.Neutral || option.type == CustomOptionType.NK || option.type == CustomOptionType.Crewmate || option.type == CustomOptionType.Impostor || option.type == CustomOptionType.ModifierAbility)) {
                    stringOption.TitleText.text = "Spawn Chance";
                }
                if (stringOption.TitleText.text.Length > 25)
                    stringOption.TitleText.fontSize = 2.2f;
                if (stringOption.TitleText.text.Length > 40)
                    stringOption.TitleText.fontSize = 2f;
                stringOption.Value = stringOption.oldValue = option.selection;
                stringOption.ValueText.text = $"{option.selections[option.selection].ToString()}{option.format}";
                option.optionBehaviour = stringOption;

                menu.Children.Add(optionBehaviour);
                num -= 0.45f;
                menu.scrollBar.SetYBoundsMax(-num - 1.65f);
            }

            for (int i = 0; i < menu.Children.Count; i++) {
                OptionBehaviour optionBehaviour = menu.Children[i];
                if (AmongUsClient.Instance && !AmongUsClient.Instance.AmHost) {
                    optionBehaviour.SetAsPlayer();
                }
            }
        }

        private static void RemoveVanillaTabs(GameSettingMenu __instance) {
            GameObject.Find("What Is This?")?.Destroy();

            GameObject.Find("GamePresetButton")?.Destroy();
            GameObject.Find("RoleSettingsButton")?.Destroy();
            __instance.ChangeTab(1, false);
        }

        public static void CreateCustomButton(GameSettingMenu __instance, int targetMenu, string buttonName, string buttonText) {
            var leftPanel = GameObject.Find("LeftPanel");
            var buttonTemplate = GameObject.Find("GameSettingsButton");
            if (targetMenu == 3) {
                buttonTemplate.transform.localPosition -= Vector3.up * 0.85f;
                buttonTemplate.transform.localScale *= Vector2.one * 0.75f;
            }
            var torSettingsButton = GameObject.Find(buttonName);
            if (torSettingsButton == null) {
                torSettingsButton = GameObject.Instantiate(buttonTemplate, leftPanel.transform);
                torSettingsButton.transform.localPosition += Vector3.up * 0.5f * (targetMenu - 2);
                torSettingsButton.name = buttonName;
                __instance.StartCoroutine(Effects.Lerp(2f, new Action<float>(p => { torSettingsButton.transform.FindChild("FontPlacer").GetComponentInChildren<TextMeshPro>().text = buttonText; })));
                var torSettingsPassiveButton = torSettingsButton.GetComponent<PassiveButton>();
                torSettingsPassiveButton.OnClick.RemoveAllListeners();
                torSettingsPassiveButton.OnClick.AddListener((System.Action)(() => {
                    __instance.ChangeTab(targetMenu, false);
                }));
                torSettingsPassiveButton.OnMouseOut.RemoveAllListeners();
                torSettingsPassiveButton.OnMouseOver.RemoveAllListeners();
                torSettingsPassiveButton.SelectButton(false);
                currentButtons.Add(torSettingsPassiveButton);
            }
        }

        public static void CreateGameOptionsMenu(GameSettingMenu __instance, CustomOptionType optionType, string settingName) 
        {
            var tabTemplate = GameObject.Find("GAME SETTINGS TAB");
            currentTabs.RemoveAll(x => x == null);

            var torSettingsTab = GameObject.Instantiate(tabTemplate, tabTemplate.transform.parent);
            torSettingsTab.name = settingName;
                
            var torSettingsGOM = torSettingsTab.GetComponent<GameOptionsMenu>();
            foreach (var child in torSettingsGOM.Children) {
                child.Destroy();
            }
            torSettingsGOM.scrollBar.transform.FindChild("SliderInner").DestroyChildren();
            torSettingsGOM.Children.Clear();
            var relevantOptions = Options.Where(x => x.type == optionType).ToList();
            CreateSettings(torSettingsGOM, relevantOptions);

            currentTabs.Add(torSettingsTab);
            torSettingsTab.SetActive(false);
        }

        private static void CreateSettingTabs(GameSettingMenu __instance) {
            // Handle different gamemodes and tabs needed therein.
            int next = 3;

            // create TOS settings
            CreateCustomButton(__instance, next++, "TORSettings", "TOS Settings");
            CreateGameOptionsMenu(__instance, CustomOptionType.General, "TORSettings");
            // Crew
            CreateCustomButton(__instance, next++, "CrewmateSettings", "Crewmate Roles");
            CreateGameOptionsMenu(__instance, CustomOptionType.Crewmate, "CrewmateSettings");
            // Neutral
            CreateCustomButton(__instance, next++, "NeutralSettings", "Neutral Roles");
            CreateGameOptionsMenu(__instance, CustomOptionType.Neutral, "NeutralSettings");
            // NK
            CreateCustomButton(__instance, next++, "NeutralKSettings", "Neutral Killing Roles");
            CreateGameOptionsMenu(__instance, CustomOptionType.NK, "NeutralKSettings");
            // IMp
            CreateCustomButton(__instance, next++, "ImpostorSettings", "Impostor Roles");
            CreateGameOptionsMenu(__instance, CustomOptionType.Impostor, "ImpostorSettings");
            // Modifier
            CreateCustomButton(__instance, next++, "ModifierSettings", "Modifiers & Abilities");
            CreateGameOptionsMenu(__instance, CustomOptionType.ModifierAbility, "ModifierSettings");
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Initialize))]
    public class StringOptionEnablePatch 
    {
        public static bool Prefix(StringOption __instance) 
        {
            CustomOption option = Options.FirstOrDefault(option => option.optionBehaviour == __instance);
            if (option == null) return true;

            __instance.OnValueChanged = new Action<OptionBehaviour>((o) => {});
            //__instance.TitleText.text = option.name;
            __instance.Value = __instance.oldValue = option.selection;
            __instance.ValueText.text = $"{option.selections[option.selection].ToString()}{option.format}";
            
            return false;
        }
    }
    
    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Increase))]
    public class StringOptionIncreasePatch
    {
        public static bool Prefix(StringOption __instance)
        {
            CustomOption option = CustomOption.Options.FirstOrDefault(option => option.optionBehaviour == __instance);
            if (option == null) return true;
            option.UpdateSelection(option.selection + 1);
            return false;
        }
    }

    [HarmonyPatch(typeof(StringOption), nameof(StringOption.Decrease))]
    public class StringOptionDecreasePatch
    {
        public static bool Prefix(StringOption __instance)
        {
            CustomOption option = CustomOption.Options.FirstOrDefault(option => option.optionBehaviour == __instance);
            if (option == null) return true;
            option.UpdateSelection(option.selection - 1);
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
    public class RpcSyncSettingsPatch
    {
        public static void Postfix()
        {
            //ShareOptionSelections();
            SaveVanillaOptions();
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoSpawnPlayer))]
    public class AmongUsClientOnPlayerJoinedPatch 
    {
        public static void Postfix() 
        {
            if (PlayerControl.LocalPlayer != null && AmongUsClient.Instance.AmHost) 
            {
                GameManager.Instance.LogicOptions.SyncOptions();
                ShareOptionSelections();
            }
        }
    }


    [HarmonyPatch] 
    class GameOptionsDataPatch
    {
        private static string BuildRoleOptions() 
        {
            var impRoles = buildOptionsOfType(CustomOptionType.Impostor, true) + "\n";
            var neutralRoles = buildOptionsOfType(CustomOptionType.Neutral, true) + "\n";
            var NKRoles = buildOptionsOfType(CustomOptionType.NK, true) + "\n";
            var crewRoles = buildOptionsOfType(CustomOptionType.Crewmate, true) + "\n";
            var modifiers = buildOptionsOfType(CustomOptionType.ModifierAbility, true);
            return impRoles + neutralRoles + NKRoles + crewRoles + modifiers;
        }
        public static string BuildModifierExtras(CustomOption customOption) 
        {
            // find options children with quantity
            var children = Options.Where(o => o.parent == customOption);
            var quantity = children.Where(o => o.name.Contains("Quantity")).ToList();
            if (customOption.GetSelection() == 0) return "";
            if (quantity.Count == 1) return $" ({quantity[0].GetQuantity()})";
            return "";
        }

        private static string buildOptionsOfType(CustomOption.CustomOptionType type, bool headerOnly) 
        {
            StringBuilder sb = new StringBuilder("\n");
            var options = CustomOption.Options.Where(o => o.type == type);

            foreach (var option in options) {
                if (option.parent == null) {
                    string line = $"{option.name}: {option.selections[option.selection].ToString()}{option.format}";
                    if (type == CustomOption.CustomOptionType.ModifierAbility) line += BuildModifierExtras(option);
                    sb.AppendLine(line);
                }
            }
            if (headerOnly) return sb.ToString();
            else sb = new StringBuilder();

            foreach (CustomOption option in options) {
                if (option.parent != null) {
                    bool isIrrelevant = option.parent.GetSelection() == 0 || (option.parent.parent != null && option.parent.parent.GetSelection() == 0);

                    Color c = isIrrelevant ? Color.grey : Color.white;  // No use for now
                    if (isIrrelevant) continue;
                    sb.AppendLine(ColorString(c, $"{option.name}: {option.selections[option.selection].ToString()}{option.format}"));
                } else 
                {
                    if (option == CustomOptionHolder.MinNeutralBenignRoles) 
                    {
                        var optionName = "Neutral Benign Roles";
                        var min = CustomOptionHolder.MinNeutralBenignRoles.GetSelection();
                        var max = CustomOptionHolder.MaxNeutralBenignRoles.GetSelection();
                        if (min > max) min = max;
                        var optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine($"{optionName}: {optionValue}");
                    } 
                    else if (option == CustomOptionHolder.MinNeutralKillingRoles) 
                    {
                        var optionName = "Neutral Killing Roles";
                        var min = CustomOptionHolder.MinNeutralKillingRoles.GetSelection();
                        var max = CustomOptionHolder.MaxNeutralKillingRoles.GetSelection();
                        if (min > max) min = max;
                        var optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine($"{optionName}: {optionValue}");
                    }
                    else if (option == CustomOptionHolder.MinNeutralEvilRoles) 
                    {
                        var optionName = "Neutral Evil Roles";
                        var min = CustomOptionHolder.MinNeutralEvilRoles.GetSelection();
                        var max = CustomOptionHolder.MaxNeutralEvilRoles.GetSelection();
                        if (min > max) min = max;
                        var optionValue = (min == max) ? $"{max}" : $"{min} - {max}";
                        sb.AppendLine($"{optionName}: {optionValue}");
                    } 
                    else if ((option == CustomOptionHolder.MaxNeutralBenignRoles) || (option == CustomOptionHolder.MaxNeutralKillingRoles) || (option == CustomOptionHolder.MaxNeutralEvilRoles)) 
                    {
                        continue;
                    } else {
                        sb.AppendLine($"\n{option.name}: {option.selections[option.selection].ToString()}{option.format}");
                    }
                }
            }
            return sb.ToString();
        }


        public static int maxPage = 7;
        public static string BuildAllOptions(string vanillaSettings = "", bool hideExtras = false) 
        {
            if (vanillaSettings == "")
                vanillaSettings = GameOptionsManager.Instance.CurrentGameOptions.ToHudString(PlayerControl.AllPlayerControls.Count);
            int counter = TownOfSushi.optionsPage;
            string hudString = counter != 0 && !hideExtras ? ColorString(DateTime.Now.Second % 2 == 0 ? Color.white : Color.red, "(Use scroll wheel if necessary)\n\n") : "";
            
            maxPage = 8;
            switch (counter) {
                case 0:
                    hudString += (!hideExtras ? "" : "Page 1: Vanilla Settings \n\n") + vanillaSettings;
                    break;
                case 1:
                    hudString += "Page 2: Town Of Sushi Settings \n" + buildOptionsOfType(CustomOptionType.General, false);
                    break;
                case 2:
                    hudString += "Page 3: Role and Modifier Rates \n" + BuildRoleOptions();
                    break;
                case 3:
                    hudString += "Page 4: Crewmate Role Settings \n" + buildOptionsOfType(CustomOptionType.Crewmate, false);
                    break;
                case 4:
                    hudString += "Page 5: Neutral Role Settings \n" + buildOptionsOfType(CustomOptionType.Neutral, false);
                    break;
                case 5:
                    hudString += "Page 6: Neutral Killing Role Settings \n" + buildOptionsOfType(CustomOptionType.NK, false);
                    break;
                case 6:
                    hudString += "Page 7: Impostor Role Settings \n" + buildOptionsOfType(CustomOptionType.Impostor, false);
                    break;
                case 7:
                    hudString += "Page 8: Modifier Settings \n" + buildOptionsOfType(CustomOptionType.ModifierAbility, false);
                    break;
            }

            if (!hideExtras || counter != 0) hudString += $"\n Press TAB or Page Number for more... ({counter + 1}/{maxPage})";
            return hudString;
        }


        [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.ToHudString))]
        private static void Postfix(ref string __result)
        {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return; // Allow Vanilla Hide N Seek
            __result = BuildAllOptions(vanillaSettings:__result);
        }
    }

    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public static class GameOptionsNextPagePatch
    {
        public static void Postfix(KeyboardJoystick __instance)
        {
            if (Input.GetKeyDown(KeyCode.Tab)) 
            {
                TownOfSushi.optionsPage = (TownOfSushi.optionsPage + 1) % 7;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) 
            {
                TownOfSushi.optionsPage = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) 
            {
                TownOfSushi.optionsPage = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) 
            {
                TownOfSushi.optionsPage = 2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) 
            {
                TownOfSushi.optionsPage = 3;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) 
            {
                TownOfSushi.optionsPage = 4;
            }
            if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) 
            {
                TownOfSushi.optionsPage = 5;
            }
            if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) 
            {
                TownOfSushi.optionsPage = 6;
            }
            if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) 
            {
                TownOfSushi.optionsPage = 7;
            }
            if (Input.GetKeyDown(KeyCode.F2) && !LobbyBehaviour.Instance)
                HudManagerUpdate.ToggleSettings(HudManager.Instance);
            if (TownOfSushi.optionsPage >= GameOptionsDataPatch.maxPage) TownOfSushi.optionsPage = 0;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate 
    {
        public static TextMeshPro GameSettings;
        public static float
            MinX,
            OriginalY = 2.9F,
            MinY = 2.9F;

        public static Scroller Scroller;
        private static Vector3 LastPosition;
        private static float lastAspect;
        private static bool setLastPosition = false;
        public static void Prefix(HudManager __instance) 
        {
            if (GameSettings?.transform == null) return;

            // Sets the MinX position to the left edge of the screen + 0.1 units
            Rect safeArea = Screen.safeArea;
            float aspect = Mathf.Min((Camera.main).aspect, safeArea.width / safeArea.height);
            float safeOrthographicSize = CameraSafeArea.GetSafeOrthographicSize(Camera.main);
            MinX = 0.1f - safeOrthographicSize * aspect;

            if (!setLastPosition || aspect != lastAspect) {
                LastPosition = new Vector3(MinX, MinY);
                lastAspect = aspect;
                setLastPosition = true;
                if (Scroller != null) Scroller.ContentXBounds = new FloatRange(MinX, MinX);                
            }

            CreateScroller(__instance);

            Scroller.gameObject.SetActive(GameSettings.gameObject.activeSelf);

            if (!Scroller.gameObject.active) return;

            var rows = GameSettings.text.Count(c => c == '\n');
            float LobbyTextRowHeight = 0.06F;
            var maxY = Mathf.Max(MinY, rows * LobbyTextRowHeight + (rows - 38) * LobbyTextRowHeight);

            Scroller.ContentYBounds = new FloatRange(MinY, maxY);

            // Prevent scrolling when the player is interacting with a menu
            if (PlayerControl.LocalPlayer?.CanMove != true) 
            {
                GameSettings.transform.localPosition = LastPosition;

                return;
            }

            if (GameSettings.transform.localPosition.x != MinX ||
                GameSettings.transform.localPosition.y < MinY) return;

            LastPosition = GameSettings.transform.localPosition;
        }

        private static void CreateScroller(HudManager __instance) 
        {
            if (Scroller != null) return;
            Transform target = GameSettings.transform;

            Scroller = new GameObject("SettingsScroller").AddComponent<Scroller>();
            Scroller.transform.SetParent(GameSettings.transform.parent);
            Scroller.gameObject.layer = 5;

            Scroller.transform.localScale = Vector3.one;
            Scroller.allowX = false;
            Scroller.allowY = true;
            Scroller.active = true;
            Scroller.velocity = new Vector2(0, 0);
            Scroller.ScrollbarYBounds = new FloatRange(0, 0);
            Scroller.ContentXBounds = new FloatRange(MinX, MinX);
            Scroller.enabled = true;

            Scroller.Inner = target;
            target.SetParent(Scroller.transform);
        }

        [HarmonyPrefix]
        public static void Prefix2(HudManager __instance) 
        {
            if (!settingsTMPs[0]) return;
            foreach (var tmp in settingsTMPs) tmp.text = "";
            var settingsString = GameOptionsDataPatch.BuildAllOptions(hideExtras: true);
            var blocks = settingsString.Split("\n\n", StringSplitOptions.RemoveEmptyEntries); ;
            string curString = "";
            string curBlock;
            int j = 0;
            for (int i = 0; i < blocks.Length; i++) {
                curBlock = blocks[i];
                if (LineCount(curBlock) + LineCount(curString) < 43) 
                {
                    curString += curBlock + "\n\n";
                } 
                else 
                {
                    settingsTMPs[j].text = curString;
                    j++;

                    curString = "\n" + curBlock + "\n\n";
                    if (curString.Substring(0, 2) != "\n\n") curString = "\n" + curString;
                }
            }
            if (j < settingsTMPs.Length) settingsTMPs[j].text = curString;
            int blockCount = 0;
            foreach (var tmp in settingsTMPs) {
                if (tmp.text != "")
                    blockCount++;
            }
            for (int i = 0; i < blockCount; i++) {
                settingsTMPs[i].transform.localPosition = new Vector3(- blockCount * 1.2f + 2.7f * i, 2.2f, -500f);
            }
        }

        private static TextMeshPro[] settingsTMPs = new TMPro.TextMeshPro[4];
        private static GameObject settingsBackground;
        public static void OpenSettings(HudManager __instance) 
        {
            if (__instance.FullScreen == null || MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) return;
            settingsBackground = GameObject.Instantiate(__instance.FullScreen.gameObject, __instance.transform);
            settingsBackground.SetActive(true);
            var renderer = settingsBackground.GetComponent<SpriteRenderer>();
            renderer.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            renderer.enabled = true;

            for (int i = 0; i < settingsTMPs.Length; i++) {
                settingsTMPs[i] = GameObject.Instantiate(__instance.KillButton.cooldownTimerText, __instance.transform);
                settingsTMPs[i].alignment = TMPro.TextAlignmentOptions.TopLeft;
                settingsTMPs[i].enableWordWrapping = false;
                settingsTMPs[i].transform.localScale = Vector3.one * 0.25f; 
                settingsTMPs[i].gameObject.SetActive(true);
            }
        }

        public static void CloseSettings() 
        {
            foreach (var tmp in settingsTMPs)
                if (tmp) tmp.gameObject.Destroy();

            if (settingsBackground) settingsBackground.Destroy();
        }

        public static void ToggleSettings(HudManager __instance) 
        {
            if (settingsTMPs[0]) CloseSettings();
            else OpenSettings(__instance);
        }

        static PassiveButton toggleSettingsButton;
        static GameObject toggleSettingsButtonObject;

        static GameObject toggleZoomButtonObject;
        static PassiveButton toggleZoomButton;

        [HarmonyPostfix]
        public static void Postfix(HudManager __instance) 
        {
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            if (!toggleSettingsButton || !toggleSettingsButtonObject) 
            {
                // add a special button for settings viewing:
                toggleSettingsButtonObject = GameObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                toggleSettingsButtonObject.transform.localPosition = __instance.MapButton.transform.localPosition + new Vector3(0, -1.25f, -500f);
                toggleSettingsButtonObject.name = "TOGGLESETTINGSBUTTON";
                SpriteRenderer renderer = toggleSettingsButtonObject.transform.Find("Inactive").GetComponent<SpriteRenderer>();
                SpriteRenderer rendererActive = toggleSettingsButtonObject.transform.Find("Active").GetComponent<SpriteRenderer>();
                toggleSettingsButtonObject.transform.Find("Background").localPosition = Vector3.zero;
                renderer.sprite = LoadSpriteFromResources("TownOfSushi.Resources.Settings_Button.png", 100f);
                rendererActive.sprite = LoadSpriteFromResources("TownOfSushi.Resources.Settings_ButtonActive.png", 100);
                toggleSettingsButton = toggleSettingsButtonObject.GetComponent<PassiveButton>();
                toggleSettingsButton.OnClick.RemoveAllListeners();
                toggleSettingsButton.OnClick.AddListener((Action)(() => ToggleSettings(__instance)));
            }
            toggleSettingsButtonObject.SetActive(__instance.MapButton.gameObject.active && !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) && GameOptionsManager.Instance.currentGameOptions.GameMode != GameModes.HideNSeek);
            toggleSettingsButtonObject.transform.localPosition = __instance.MapButton.transform.localPosition + new Vector3(0, -0.8f, -500f);


            if (!toggleZoomButton || !toggleZoomButtonObject) 
            {
                // add a special button for settings viewing:
                toggleZoomButtonObject = GameObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                toggleZoomButtonObject.transform.localPosition = __instance.MapButton.transform.localPosition + new Vector3(0, -1.25f, -500f);
                toggleZoomButtonObject.name = "TOGGLEZOOMBUTTON";
                SpriteRenderer tZrenderer = toggleZoomButtonObject.transform.Find("Inactive").GetComponent<SpriteRenderer>();
                SpriteRenderer tZArenderer = toggleZoomButtonObject.transform.Find("Active").GetComponent<SpriteRenderer>();
                toggleZoomButtonObject.transform.Find("Background").localPosition = Vector3.zero;
                tZrenderer.sprite = LoadSpriteFromResources("TownOfSushi.Resources.Minus_Button.png", 100f);
                tZArenderer.sprite = LoadSpriteFromResources("TownOfSushi.Resources.Minus_ButtonActive.png", 100);
                toggleZoomButton = toggleZoomButtonObject.GetComponent<PassiveButton>();
                toggleZoomButton.OnClick.RemoveAllListeners();
                toggleZoomButton.OnClick.AddListener((Action)(() => ToggleZoom()));
            }

        bool zoomButtonActive =  PlayerControl.LocalPlayer.Data.IsDead  && !MeetingHud.Instance && !ExileController.Instance;
        toggleZoomButtonObject.SetActive(zoomButtonActive);
        var posOffset = zoomOutStatus ? new Vector3(-1.27f, -7.92f, -52f) : new Vector3(0, -1.6f, -52f);
        toggleZoomButtonObject.transform.localPosition = HudManager.Instance.MapButton.transform.localPosition + posOffset;
        }
    }
}