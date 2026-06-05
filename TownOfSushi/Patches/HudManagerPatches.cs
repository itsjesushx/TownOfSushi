using HarmonyLib;
using InnerNet;
using TMPro;
using TownOfSushi.Modifiers;
using TownOfSushi.Options;
using TownOfSushi.Patches.Options;
using TownOfSushi.Utilities.Appearances;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Object = UnityEngine.Object;

namespace TownOfSushi.Patches;

[HarmonyPatch]
public static class HudManagerPatches
{
    public static GameObject ZoomButton;
    public static GameObject WikiButton;
    public static GameObject TeamChatButton;
    public static GameObject SubmergedFloorButton;

    public static bool Zooming;

    public static void AdjustCameraSize(float size)
    {
        Camera.main!.orthographicSize = size;
        foreach (var cam in Camera.allCameras)
        {
            cam.orthographicSize = Camera.main!.orthographicSize;
        }

        ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height,
            Screen.fullScreen);

        if (size <= 3f)
        {
            Zooming = false;
            HudManager.Instance.ShadowQuad.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead);
        }
        else
        {
            Zooming = true;
            HudManager.Instance.ShadowQuad.gameObject.SetActive(false);
        }

        ZoomButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().sprite =
            Zooming ? TownOfSushiAssets.ZoomPlus.LoadAsset() : TownOfSushiAssets.ZoomMinus.LoadAsset();
        ZoomButton.transform.Find("Active").GetComponent<SpriteRenderer>().sprite =
            Zooming ? TownOfSushiAssets.ZoomPlusActive.LoadAsset() : TownOfSushiAssets.ZoomMinusActive.LoadAsset();
    }

    public static void ButtonClickZoom()
    {
        if (MeetingHud.Instance || ExileController.Instance)
        {
            ZoomButton.SetActive(false);
            return;
        }

        AdjustCameraSize(!Zooming ? 12f : 3f);
    }

    public static void ScrollZoom(bool zoomOut = false)
    {
        if (MeetingHud.Instance || ExileController.Instance)
        {
            ZoomButton.SetActive(false);
            return;
        }

        var size = Camera.main!.orthographicSize;
        size = zoomOut ? size * 1.25f : size / 1.25f;
        size = Mathf.Clamp(size, 3, 15);

        AdjustCameraSize(size);
    }

    public static void ResetZoom()
    {
        ZoomButton.SetActive(false);

        AdjustCameraSize(3f);
    }

    public static void CheckForScrollZoom()
    {
        var scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        var axisRaw = ConsoleJoystick.player.GetAxisRaw(55);

        if (scrollWheel > 0 || axisRaw > 0)
        {
            ScrollZoom();
        }
        else if (scrollWheel < 0 || axisRaw < 0)
        {
            ScrollZoom(true);
        }
    }

    public static void UpdateTeamChat()
    {
        // Ensure built-in chats are registered
        TeamChatPatches.TeamChatManager.RegisterBuiltInChats();

        var availableChats = TeamChatPatches.TeamChatManager.GetAllAvailableChats();
        var isValid = MeetingHud.Instance != null && availableChats.Count > 0;

        // Don't show team chat button for lover chat (it's handled separately outside meetings)
        // Lover chat is always active when available and doesn't need the team chat button

        if (!TeamChatPatches.TeamChatButton)
        {
            return;
        }

        if (TeamChatPatches.TeamChatActive && !isValid && HudManager.Instance.Chat.IsOpenOrOpening)
        {
            TeamChatPatches.TeamChatActive = false;
            TeamChatPatches.CurrentChatIndex = -1;
            TeamChatPatches.UpdateChat();
        }

        TeamChatPatches.TeamChatButton.SetActive(isValid);
    }
    public static void UpdateRoleNameText()
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        var taskOpt = OptionGroupSingleton<TaskTrackingOptions>.Instance;

        if (MeetingHud.Instance)
        {
            foreach (var playerVA in MeetingHud.Instance.playerStates)
            {
                var player = Utils.PlayerById(playerVA.TargetPlayerId);
                playerVA.ColorBlindName.transform.localPosition = new Vector3(-0.93f, -0.2f, -0.1f);

                if (player == null || player.Data == null || player.Data.Role == null)
                {
                    continue;
                }
                var revealMods = player.GetModifiers<RevealModifier>();

                var playerName = player.GetDefaultAppearance().PlayerName ?? "Unknown";
                var playerColor = Color.white;

                if (PlayerControl.LocalPlayer.IsImpostor() && PlayerControl.LocalPlayer != player && (player.IsImpostor() || player.Data.Role is SpyRole))
                {
                    playerColor = Color.red;
                }

                playerColor = playerColor.UpdateTargetColor(player);
                playerName = playerName.UpdateTargetSymbols(player);
                playerName = playerName.UpdateProtectionSymbols(player);
                playerName = playerName.UpdateAllianceSymbols(player);
                playerName = playerName.UpdateStatusSymbols(player);

                var role = player.Data.Role;

                if (role == null)
                {
                    continue;
                }

                var color = Color.white;

                var roleName = "";

                if (player.AmOwner ||
                    (PlayerControl.LocalPlayer.IsImpostor() && player.IsImpostor() && !Utils.SpyInGame()) ||
                    (PlayerControl.LocalPlayer.GetRoleWhenAlive() is VampireRole && role is VampireRole) ||
                    (PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow) ||
                    LoverModifier.LoverSeesRoleVisibilityFlag(player) ||
                    RomanticRole.RomanticSeesRoleVisibilityFlag(player) ||
                    LawyerRole.LawyerSeesRoleVisibilityFlag(player) ||
                    ConsigliereRole.ConsigliereSeesRoleVisibilityFlag(player) ||
                    SleuthModifier.SleuthVisibilityFlag(player) ||
                    revealMods.Any(x => x.Visible && x.RevealRole))
                {
                    color = role.TeamColor;
                    roleName = $"<size=80%>{color.ToTextColor()}{player.Data.Role.NiceName}</color></size>";

                    var revealedRole = revealMods.FirstOrDefault(x => x.Visible && x.RevealRole && x.ShownRole != null);
                    if (revealedRole != null)
                    {
                        color = revealedRole.ShownRole!.TeamColor;
                        roleName = $"<size=80%>{color.ToTextColor()}{revealedRole.ShownRole!.NiceName}</color></size>";
                    }

                    if (!player.HasModifier<VampireBittenModifier>() && role is VampireRole)
                    {
                        roleName += "<size=80%><color=#FFFFFF> (<color=#262626FF>OG</color>)</color></size>";
                    }

                    var cachedMod = player.GetModifiers<BaseModifier>().FirstOrDefault(x => x is ICachedRole);
                    if (cachedMod is ICachedRole cache && cache.Visible &&
                        player.Data.Role.GetType() != cache.CachedRole.GetType())
                    {
                        roleName = cache.ShowCurrentRoleFirst
                            ? $"<size=80%>{color.ToTextColor()}{player.Data.Role.NiceName}</color> ({cache.CachedRole.TeamColor.ToTextColor()}{cache.CachedRole.NiceName}</color>)</size>"
                            : $"<size=80%>{cache.CachedRole.TeamColor.ToTextColor()}{cache.CachedRole.NiceName}</color> ({color.ToTextColor()}{player.Data.Role.NiceName}</color>)</size>";
                    }

                    if (player.Data.IsDead && role is GuardianAngelRole gaRole)
                    {
                        roleName = $"<size=80%>{gaRole.TeamColor.ToTextColor()}{gaRole?.NiceName}</color></size>";
                    }

                    if (SleuthModifier.SleuthVisibilityFlag(player) || (player.Data.IsDead &&
                                                                        role is not GuardianAngelRole))
                    {
                        var roleWhenAlive = player.GetRoleWhenAlive();
                        color = roleWhenAlive.TeamColor;

                        roleName = $"<size=80%>{color.ToTextColor()}{roleWhenAlive.NiceName}</color></size>";
                        if (PlayerControl.LocalPlayer.HasDied() && !player.HasModifier<VampireBittenModifier>() && roleWhenAlive is VampireRole)
                        {
                            roleName += "<size=80%><color=#FFFFFF> (<color=#262626FF>OG</color>)</color></size>";
                        }
                    }
                    if (PlayerControl.LocalPlayer.HasDied() && player.TryGetModifier<DeathHandlerModifier>(out var deathMod))
                    {
                        var deathReason =
                            $"<size=60%>『{Color.yellow.ToTextColor()}{deathMod.CauseOfDeath}</color>』</size>\n";

                        roleName = $"{deathReason}{roleName}";
                    }
                }

                if (((taskOpt.ShowTaskInMeetings && player.AmOwner) ||
                     (PlayerControl.LocalPlayer.HasDied() && taskOpt.ShowTaskDead)) &&
                    (player.Is(Factions.Crewmate)))
                {
                    if (roleName != string.Empty)
                    {
                        roleName += " ";
                    }
                    roleName += $"<size=80%>{player.TaskInfo()}</size>";
                }

                if (player.TryGetModifier<OracleConfessModifier>(out var confess, x => x.ConfessToAll))
                {
                    var accuracy = OptionGroupSingleton<OracleOptions>.Instance.RevealAccuracyPercentage;
                    var revealText = confess.RevealedFaction switch
                    {
                        ModdedRoleTeams.Crewmate =>
                            Utils.ColorString(TownOfSushiColors.Crewmate, $"\n<size=75%>({accuracy}% Crew) </color></size>"),
                        ModdedRoleTeams.Custom =>
                            Utils.ColorString(TownOfSushiColors.Neutral, $"\n<size=75%>({accuracy}% Neut)</size>"),
                        ModdedRoleTeams.Impostor =>
                            Utils.ColorString(TownOfSushiColors.ImpSoft, $"\n<size=75%>({accuracy}% Imp)</size>"),
                        _ => string.Empty
                    };

                    playerName += revealText;
                }

                var revealedColorMod = revealMods.FirstOrDefault(x => x.Visible && x.NameColor != null);
                if (revealedColorMod != null)
                {
                    playerColor = (Color)revealedColorMod.NameColor!;
                    playerName = $"{playerColor.ToTextColor()}{playerName}</color>";
                }
                
                var addedRoleNameText = revealMods.FirstOrDefault(x => x.Visible && x.ExtraRoleText != string.Empty);
                if (addedRoleNameText != null)
                {
                    roleName += $"<size=80%>{addedRoleNameText.ExtraRoleText}</size>";
                }
                
                var addedPlayerNameText = revealMods.FirstOrDefault(x => x.Visible && x.ExtraNameText != string.Empty);
                if (addedPlayerNameText != null)
                {
                    playerName += addedPlayerNameText.ExtraNameText;
                }

                if (player?.Data?.Disconnected == true)
                {
                    if (!((PlayerControl.LocalPlayer.IsImpostor() && player.IsImpostor() && !Utils.SpyInGame()) ||
                          (PlayerControl.LocalPlayer.GetRoleWhenAlive() is VampireRole && role is VampireRole) ||
                          (!TutorialManager.InstanceExists &&
                           ((PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow) ||
                            RomanticRole.RomanticSeesRoleVisibilityFlag(player) ||
                            LoverModifier.LoverSeesRoleVisibilityFlag(player) ||
                            LawyerRole.LawyerSeesRoleVisibilityFlag(player) ||
                            ConsigliereRole.ConsigliereSeesRoleVisibilityFlag(player) ||
                            SleuthModifier.SleuthVisibilityFlag(player) ||
                           revealMods.Any(x => x.Visible && x.RevealRole)))))
                    {
                        roleName = "";
                        color = Color.white;
                        playerColor = Color.white;
                    }

                    var dash = "";
                    if (!string.IsNullOrEmpty(roleName))
                    {
                        dash = " - ";
                    }

                    roleName = $"{roleName}<size=80%>{dash}Disconnected</size>";
                }

                if (!string.IsNullOrEmpty(roleName))
                {
                    playerName = $"{color.ToTextColor()}<size=92%>{playerName}</size></color>\n{roleName}";
                }

                playerVA.NameText.text = playerName;
                playerVA.NameText.color = playerColor;
            }
        }
        else
        {
            var isVisible = (PlayerControl.LocalPlayer.TryGetModifier<DeathHandlerModifier>(out var deathHandler) &&
                            !deathHandler.DiedThisRound) || TutorialManager.InstanceExists;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player == null || player.Data == null || player.Data.Role == null)
                {
                    continue;
                }

                var revealMods = player.GetModifiers<RevealModifier>();

                var playerName = player.GetAppearance().PlayerName ?? "Unknown";
                var playerColor = Color.white;

                if (PlayerControl.LocalPlayer.IsImpostor() && PlayerControl.LocalPlayer != player && (player.IsImpostor() || player.Data.Role is SpyRole))
                {
                    playerColor = Color.red;
                }

                playerColor = playerColor.UpdateTargetColor(player, !isVisible);
                playerName = playerName.UpdateTargetSymbols(player, !isVisible);
                playerName = playerName.UpdateProtectionSymbols(player, !isVisible);
                playerName = playerName.UpdateAllianceSymbols(player, !isVisible);
                playerName = playerName.UpdateStatusSymbols(player, !isVisible);

                var role = player.Data.Role;
                var color = Color.white;

                if (role == null)
                {
                    continue;
                }
                var roleName = "";
                var canSeeDeathReason = false;
                if (player.AmOwner ||
                    (PlayerControl.LocalPlayer.IsImpostor() && player.IsImpostor() && !Utils.SpyInGame()) ||
                    (PlayerControl.LocalPlayer.GetRoleWhenAlive() is VampireRole && role is VampireRole) ||
                    (PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && isVisible) ||
                    RomanticRole.RomanticSeesRoleVisibilityFlag(player) ||
                    LawyerRole.LawyerSeesRoleVisibilityFlag(player) ||
                    LoverModifier.LoverSeesRoleVisibilityFlag(player) ||
                    ConsigliereRole.ConsigliereSeesRoleVisibilityFlag(player) ||
                    revealMods.Any(x => x.Visible && x.RevealRole))
                {
                    color = role.TeamColor;
                    roleName = $"<size=80%>{color.ToTextColor()}{player.Data.Role.NiceName}</color></size>";
                    if (!player.HasModifier<VampireBittenModifier>() && player.Data.Role is VampireRole)
                    {
                        roleName += "<size=80%><color=#FFFFFF> (<color=#262626FF>OG</color>)</color></size>";
                    }

                    var cachedMod = player.GetModifiers<BaseModifier>().FirstOrDefault(x => x is ICachedRole);
                    if (cachedMod is ICachedRole cache && cache.Visible &&
                        player.Data.Role.GetType() != cache.CachedRole.GetType())
                    {
                        roleName = cache.ShowCurrentRoleFirst
                            ? $"<size=80%>{color.ToTextColor()}{player.Data.Role.NiceName}</color> ({cache.CachedRole.TeamColor.ToTextColor()}{cache.CachedRole.NiceName}</color>)</size>"
                            : $"<size=80%>{cache.CachedRole.TeamColor.ToTextColor()}{cache.CachedRole.NiceName}</color> ({color.ToTextColor()}{player.Data.Role.NiceName}</color>)</size>";
                    }

                    if (player.Data.IsDead && role is GuardianAngelRole gaRole)
                    {
                        roleName = $"<size=80%>{gaRole.TeamColor.ToTextColor()}{gaRole.NiceName}</color></size>";
                    }

                    if (SleuthModifier.SleuthVisibilityFlag(player) || (player.Data.IsDead &&
                                                                        role is not GuardianAngelRole))
                    {
                        var roleWhenAlive = player.GetRoleWhenAlive();
                        color = roleWhenAlive.TeamColor;

                        roleName = $"<size=80%>{color.ToTextColor()}{roleWhenAlive.NiceName}</color></size>";
                        if (!player.HasModifier<VampireBittenModifier>() && roleWhenAlive is VampireRole)
                        {
                            roleName += "<size=80%><color=#FFFFFF> (<color=#262626FF>OG</color>)</color></size>";
                        }
                    }
                    if (PlayerControl.LocalPlayer.HasDied() && isVisible && player.TryGetModifier<DeathHandlerModifier>(out var deathMod))
                    {
                        var deathReason =
                            $"<size=75%>『{Color.yellow.ToTextColor()}{deathMod.CauseOfDeath}</color>』</size>\n";

                        roleName = $"{deathReason}{roleName}";
                        canSeeDeathReason = true;
                    }
                }

                if (PlayerControl.LocalPlayer.HasDied() && taskOpt.ShowTaskDead && isVisible && (player.Is(Factions.Crewmate)))
                {
                    if (roleName != string.Empty)
                    {
                        roleName += " ";
                    }
                    roleName += $"<size=80%>{player.TaskInfo()}</size>";
                }

                if (canSeeDeathReason)
                {
                    roleName += $"\n<size=75%> </size>";
                }

                var revealedColorMod = revealMods.FirstOrDefault(x => x.Visible && x.NameColor != null);
                if (revealedColorMod != null)
                {
                    playerColor = (Color)revealedColorMod.NameColor!;
                    playerName = $"{playerColor.ToTextColor()}{playerName}</color>";
                }
                
                var addedRoleNameText = revealMods.FirstOrDefault(x => x.Visible && x.ExtraRoleText != string.Empty);
                if (addedRoleNameText != null)
                {
                    roleName += $"<size=80%>{addedRoleNameText.ExtraRoleText}</size>";
                }
                
                var addedPlayerNameText = revealMods.FirstOrDefault(x => x.Visible && x.ExtraNameText != string.Empty);
                if (addedPlayerNameText != null)
                {
                    playerName += addedPlayerNameText.ExtraNameText;
                }

                if (!string.IsNullOrEmpty(roleName))
                {
                    playerName = $"{color.ToTextColor()}{playerName}\n{roleName}</color>";
                }

                player.cosmetics.nameText.text = playerName;
                player.cosmetics.nameText.color = playerColor;
                player.cosmetics.nameText.transform.localPosition = new Vector3(0f, 0.15f, -0.5f);
            }
        }

        if (HudManager.Instance.TaskPanel != null)
        {
            var tabText = HudManager.Instance.TaskPanel.tab.transform.FindChild("TabText_TMP")
                .GetComponent<TextMeshPro>();
            tabText.SetText($"Tasks {PlayerControl.LocalPlayer.TaskInfo()}");
        }
    }

    public static void CreateZoomButton(HudManager instance)
    {
        var isChatButtonVisible = HudManager.Instance.Chat.isActiveAndEnabled;

        if (!ZoomButton)
        {
            ZoomButton = Object.Instantiate(instance.MapButton.gameObject, instance.MapButton.transform.parent);
            ZoomButton.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            ZoomButton.GetComponent<PassiveButton>().OnClick.AddListener(new Action(ButtonClickZoom));
            ZoomButton.name = "ZoomButton";
            ZoomButton.transform.Find("Background").localPosition = Vector3.zero;
            ZoomButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().sprite =
                TownOfSushiAssets.ZoomMinus.LoadAsset();
            ZoomButton.transform.Find("Active").GetComponent<SpriteRenderer>().sprite =
                TownOfSushiAssets.ZoomMinusActive.LoadAsset();
        }

        if (ZoomButton)
        {
            var aspectPosition = ZoomButton.GetComponentInChildren<AspectPosition>();
            var distanceFromEdge = aspectPosition.DistanceFromEdge;
            distanceFromEdge.x = isChatButtonVisible ? 2.73f : 2.15f;
            distanceFromEdge.y = 0.485f;
            aspectPosition.DistanceFromEdge = distanceFromEdge;
            aspectPosition.AdjustPosition();
        }
    }

    public static void PetsUpdate()
    {
        if (MeetingHud.Instance) return;

        foreach (var data in GameData.Instance.AllPlayers)
        {
            if (data == null || data.Disconnected) continue;
            var player = data.Object;
            if (player.AmOwner || player == null) continue;
            PetBehaviour pet = player.GetPet();

            bool cantsee = !PlayerControl.LocalPlayer.Data.IsDead && !player.petting && (player.Data.IsDead || player.inVent);
            if (pet != null)
            {
                pet?.Visible = !cantsee;
            }
        }
    }

    public static void CreateTeamChatButton(HudManager instance)
    {
        if (TeamChatButton)
        {
            return;
        }

        TeamChatButton = Object.Instantiate(instance.MapButton.gameObject, instance.MapButton.transform.parent);
        TeamChatButton.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
        TeamChatButton.GetComponent<PassiveButton>().OnClick.AddListener(new Action(TeamChatPatches.ToggleTeamChat));
        TeamChatButton.name = "FactionChat";
        TeamChatButton.transform.Find("Background").localPosition = Vector3.zero;
        TeamChatButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().sprite =
            TownOfSushiAssets.TeamChatInactive.LoadAsset();
        TeamChatButton.transform.Find("Active").GetComponent<SpriteRenderer>().sprite =
            TownOfSushiAssets.TeamChatActive.LoadAsset();
        TeamChatButton.transform.Find("Selected").GetComponent<SpriteRenderer>().sprite =
            TownOfSushiAssets.TeamChatSelected.LoadAsset();
    }

    public static void UpdateSubmergedButtons(HudManager instance)
    {
        if (ModCompatibility.IsSubmerged() && !SubmergedFloorButton)
        {
            SubmergedFloorButton = instance.MapButton.transform.parent.Find(instance.MapButton.name + "(Clone)")
                .gameObject;
        }
    }
    public static void UpdateColorblindText()
    {
        Vector3 colorBlindTextMeetingInitialLocalPos = new Vector3(0.3384f, -0.16666f, -0.01f);
        Vector3 colorBlindTextMeetingInitialLocalScale = new Vector3(0.9f, 1f, 1f);

        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
        {
            UpdateMeetingColorBlindText(player, colorBlindTextMeetingInitialLocalPos, colorBlindTextMeetingInitialLocalScale);
            UpdateRoundColorBlindText(player);
        }
    }

    private static void UpdateMeetingColorBlindText(PlayerControl player, Vector3 initialPos, Vector3 initialScale)
    {
        PlayerVoteArea playerVoteArea = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == player.PlayerId);
        if (playerVoteArea != null && playerVoteArea.ColorBlindName.gameObject.active)
        {
            playerVoteArea.ColorBlindName.transform.localPosition = initialPos + new Vector3(0f, 0.4f, 0f);
            playerVoteArea.ColorBlindName.transform.localScale = initialScale * 0.8f;
        }

        if (player == null || player.cosmetics.colorBlindText == null || playerVoteArea == null) return;
        var playerData = GameData.Instance.GetPlayerById(playerVoteArea.TargetPlayerId);
        Color playerColor = Palette.PlayerColors[playerData?.DefaultOutfit.ColorId ?? 0];
        playerVoteArea.ColorBlindName.color = playerColor;
    }

    private static void UpdateRoundColorBlindText(PlayerControl player)
    {
        if (player.cosmetics.colorBlindText != null && player.cosmetics.showColorBlindText && player.cosmetics.colorBlindText.gameObject.active)
        {
            player.cosmetics.colorBlindText.transform.localPosition = new Vector3(0, -1.4f, 0f);
        }
    }

    public static void CreateWikiButton(HudManager instance)
    {
        var isChatButtonVisible = HudManager.Instance.Chat.isActiveAndEnabled;

        if (!WikiButton)
        {
            WikiButton = Object.Instantiate(instance.MapButton.gameObject, instance.MapButton.transform.parent);
            WikiButton.name = "WikiButton";
            WikiButton.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            WikiButton.GetComponent<PassiveButton>().OnClick.AddListener((UnityAction)(() =>
            {
                if (Minigame.Instance)
                {
                    return;
                }

                IngameWikiMinigame.Create().Begin(null);
            }));

            WikiButton.transform.Find("Background").localPosition = Vector3.zero;
            WikiButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().sprite =
                TownOfSushiAssets.WikiButton.LoadAsset();
            WikiButton.transform.Find("Active").GetComponent<SpriteRenderer>().sprite =
                TownOfSushiAssets.WikiButtonActive.LoadAsset();
        }

        if (WikiButton)
        {
            var aspectPosition = WikiButton.GetComponentInChildren<AspectPosition>();
            var distanceFromEdge = aspectPosition.DistanceFromEdge;
            distanceFromEdge.x = isChatButtonVisible ? 2.73f : 2.15f;

            if (ZoomButton.active && !MeetingHud.Instance /*  && Minigame.Instance == null */ &&
                (PlayerJoinPatch.SentOnce || TutorialManager.InstanceExists))
            {
                distanceFromEdge.x += 0.84f;
            }

            if (TeamChatButton.active)
            {
                distanceFromEdge.x += 0.84f;
            }

            distanceFromEdge.y = 0.485f;
            WikiButton.SetActive(true);
            aspectPosition.DistanceFromEdge = distanceFromEdge;
            aspectPosition.AdjustPosition();
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPostfix]
    public static void HudManagerUpdatePatch(HudManager __instance)
    {
        Utils.Meetingtime -= Time.deltaTime;
        CreateZoomButton(__instance);
        CreateTeamChatButton(__instance);
        CreateWikiButton(__instance);
        UpdateColorblindText();

        if (PlayerControl.LocalPlayer == null ||
            PlayerControl.LocalPlayer.Data == null ||
            PlayerControl.LocalPlayer.Data.Role == null ||
            !ShipStatus.Instance ||
            (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started &&
             !TutorialManager.InstanceExists))
        {
            return;
        }

        // TERRIBLE FOR PERFORMANCE (FindObjectsOfType is very costly)
        var body = Object.FindObjectsOfType<DeadBody>()
            .FirstOrDefault(x => x.ParentId == PlayerControl.LocalPlayer.PlayerId);

        if (((PlayerControl.LocalPlayer.Data.IsDead && !body) || TutorialManager.InstanceExists)
            && Input.GetAxis("Mouse ScrollWheel") != 0 && !MeetingHud.Instance && Minigame.Instance == null &&
            !HudManager.Instance.Chat.IsOpenOrOpening)
        {
            CheckForScrollZoom();
        }

        UpdateTeamChat();
        UpdateRoleNameText();
        UpdateSubmergedButtons(__instance);
        PetsUpdate();
    }
}