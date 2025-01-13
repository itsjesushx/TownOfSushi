using TMPro;
using TownOfSushi.Modules.CustomColors;

namespace TownOfSushi.Roles
{
    public class Tracker : Role
    {
        public Dictionary<byte, ArrowBehaviour> TrackerArrows = new Dictionary<byte, ArrowBehaviour>();
        public PlayerControl ClosestPlayer;
        public DateTime LastTracked { get; set; }
        public int MaxUses;
        public TextMeshPro UsesText;
        public bool ButtonUsable => MaxUses != 0;
        public Tracker(PlayerControl player) : base(player)
        {
            var ResetOnNewRound = CustomGameOptions.ResetOnNewRound ? "reset" : "not reset";
            var playerOrPlayers = CustomGameOptions.MaxTracks == 1 ? "player" : "players";
            Name = "Tracker";
            StartText = () => "Track Everyone's Movement";
            TaskText = () => "Track suspicious players";
            RoleInfo = $"The Tracker is able to track the movements of other players. The Arrow's color will be the tracked players color. The arrow will update the position of the player every {CustomGameOptions.UpdateInterval} seconds. The Tracker can track {CustomGameOptions.MaxTracks} {playerOrPlayers} every {CustomGameOptions.TrackCd} seconds. The Arrows will {ResetOnNewRound} after each meeting.";
            LoreText = "A master observer, you specialize in monitoring the movements of your crewmates. As the Tracker, you can follow suspicious players, uncovering patterns and identifying potential threats. Your vigilance and attention to detail are vital to exposing the Impostors lurking in the shadows.";
            Color = Colors.Tracker;
            LastTracked = DateTime.UtcNow;
            RoleType = RoleEnum.Tracker;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewInvest;
            MaxUses = CustomGameOptions.MaxTracks;
        }

        public float TrackerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastTracked;
            var num = CustomGameOptions.TrackCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public bool IsTracking(PlayerControl player)
        {
            return TrackerArrows.ContainsKey(player.PlayerId);
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = TrackerArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            TrackerArrows.Remove(arrow.Key);
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    public class HudTrack
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateTrackButton(__instance);
        }

        public static void UpdateTrackButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Tracker)) return;
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var trackButton = __instance.KillButton;

            var role = GetRole<Tracker>(PlayerControl.LocalPlayer);

            if (role.UsesText == null && role.MaxUses > 0)
            {
                role.UsesText = Object.Instantiate(trackButton.cooldownTimerText, trackButton.transform);
                role.UsesText.gameObject.SetActive(false);
                role.UsesText.transform.localPosition = new Vector3(
                    role.UsesText.transform.localPosition.x + 0.26f,
                    role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }
            if (role.UsesText != null)
            {
                role.UsesText.text = role.MaxUses + "";
            }
            trackButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (role.ButtonUsable) trackButton.SetCoolDown(role.TrackerTimer(), CustomGameOptions.TrackCd);
            else trackButton.SetCoolDown(0f, CustomGameOptions.TrackCd);
            if (role.MaxUses == 0) return;

            var notTracked = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !role.IsTracking(x))
                .ToList();

            SetTarget(ref role.ClosestPlayer, trackButton, float.NaN, notTracked);

            var renderer = trackButton.graphic;
            if (role.ClosestPlayer != null && role.ButtonUsable)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.UsesText.color = Palette.EnabledColor;
                role.UsesText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                role.UsesText.color = Palette.DisabledClear;
                role.UsesText.material.SetFloat("_Desat", 1f);
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformTrack
    {
        public static Sprite Sprite => TownOfSushi.Arrow;
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Tracker)) return true;
            var role = GetRole<Tracker>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.TrackerTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = KillDistance();
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;
            var target = role.ClosestPlayer;
            if (!role.ButtonUsable) return false;

            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[3] == true)
            {
                var gameObj = new GameObject();
                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Sprite;
                if (!CamouflageUnCamouflagePatch.IsCamouflaged)
                {
                    if (ColorUtils.IsRainbow(target.GetDefaultOutfit().ColorId))
                        renderer.color = ColorUtils.Rainbow;
                    
                    else if (ColorUtils.IsMonochrome(target.GetDefaultOutfit().ColorId))
                        renderer.color = ColorUtils.Monochrome;
                        
                    else if (ColorUtils.IsGalaxy(target.GetDefaultOutfit().ColorId))
                        renderer.color = ColorUtils.Galaxy;
                    else
                    {
                        renderer.color = Palette.PlayerColors[target.GetDefaultOutfit().ColorId];
                    }
                }
                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = target.transform.position;

                role.TrackerArrows.Add(target.PlayerId, arrow);
                role.MaxUses--;
            }
            if (interact[0] == true)
            {
                role.LastTracked = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastTracked = DateTime.UtcNow;
                role.LastTracked = role.LastTracked.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.TrackCd);
                return false;
            }
            else if (interact[2] == true) return false;
            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class UpdateTrackerArrows
    {
        public static Sprite Sprite => TownOfSushi.Arrow;
        private static DateTime _time = DateTime.UnixEpoch;
        private static float Interval => CustomGameOptions.UpdateInterval;
        public static bool CamoedLastTick = false;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Tracker)) return;

            var role = GetRole<Tracker>(PlayerControl.LocalPlayer);

            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                role.TrackerArrows.Values.DestroyAll();
                role.TrackerArrows.Clear();
                return;
            }

            foreach (var arrow in role.TrackerArrows)
            {
                var player = Utils.PlayerById(arrow.Key);
                if (player == null || player.Data == null || player.Data.IsDead || player.Data.Disconnected)
                {
                    role.DestroyArrow(arrow.Key);
                    continue;
                }

                if (!CamouflageUnCamouflagePatch.IsCamouflaged)
                {
                    if (ColorUtils.IsRainbow(player.GetDefaultOutfit().ColorId))
                        arrow.Value.image.color = ColorUtils.Rainbow;
                    
                    else if (ColorUtils.IsMonochrome(player.GetDefaultOutfit().ColorId))
                        arrow.Value.image.color = ColorUtils.Monochrome;

                    else if (ColorUtils.IsGalaxy(player.GetDefaultOutfit().ColorId))
                        arrow.Value.image.color = ColorUtils.Galaxy;
                        
                    else if (CamoedLastTick)
                    {
                        arrow.Value.image.color = Palette.PlayerColors[player.GetDefaultOutfit().ColorId];
                    }
                }
                else if (!CamoedLastTick)
                {
                    arrow.Value.image.color = Color.gray;
                }

                if (_time <= DateTime.UtcNow.AddSeconds(-Interval))
                    arrow.Value.target = player.transform.position;
            }

            CamoedLastTick = CamouflageUnCamouflagePatch.IsCamouflaged;
            if (_time <= DateTime.UtcNow.AddSeconds(-Interval))
                _time = DateTime.UtcNow;
        }
    }
}