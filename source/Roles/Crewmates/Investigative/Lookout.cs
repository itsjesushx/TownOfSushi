using TMPro;

namespace TownOfSushi.Roles
{
    public class Lookout : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastWatched { get; set; }
        public int UsesLeft;
        public TextMeshPro UsesText;
        public bool ButtonUsable => UsesLeft != 0;
        public Dictionary<byte, List<RoleEnum>> Watching { get; set; } = new();

        public Lookout(PlayerControl player) : base(player)
        {
            Name = "Lookout";
            StartText = () => "Keep your eyes wide open";
            TaskText = () => "Watch other players";
            RoleInfo = "The Lookout is a Crewmate that can watch other players during rounds. During meetings they will see all roles who interact with each watched player.";
            LoreText = "With an unwavering gaze and a sharp eye for detail, the Lookout stands vigilant among the stars. As danger lurks in the shadows, their observations become the lifeline for the Crew. Though silent and unseen, their presence is felt in every corner of the ship, uncovering the truth one watchful moment at a time.";
            RoleAlignment = RoleAlignment.CrewInvest;
            Color = ColorManager.Lookout;
            LastWatched = DateTime.UtcNow;
            RoleType = RoleEnum.Lookout;
            AddToRoleHistory(RoleType);
            UsesLeft = CustomGameOptions.MaxWatches;
        }
        public float WatchTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastWatched;
            var num = CustomGameOptions.WatchCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformWatch
    {
        public static Sprite Sprite => TownOfSushi.Arrow;
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != HUDManager().KillButton) return true;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Lookout)) return true;
            var role = Role.GetRole<Lookout>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.WatchTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[VanillaOptions().currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;
            var target = role.ClosestPlayer;
            if (!role.ButtonUsable) return false;

            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[3] == true)
            {
                role.Watching.Add(role.ClosestPlayer.PlayerId, new List<RoleEnum>());
                role.UsesLeft--;
            }
            if (interact[0] == true)
            {
                role.LastWatched = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastWatched = DateTime.UtcNow;
                role.LastWatched = role.LastWatched.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.WatchCooldown);
                return false;
            }
            else if (interact[2] == true) return false;
            return false;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class LookoutMeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (IsDead()) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Lookout)) return;
            var loRole = GetRole<Lookout>(PlayerControl.LocalPlayer);
            foreach (var (key, value) in loRole.Watching)
            {
                var name = Utils.PlayerById(key).Data.PlayerName;
                if (value.Count == 0)
                {
                    if (HUDManager())
                        HUDManager().Chat.AddChat(PlayerControl.LocalPlayer, $"No players interacted with {name}");
                }
                else
                {
                    string message = $"Roles seen interacting with {name}:\n";
                    foreach (RoleEnum role in value.OrderBy(x => Guid.NewGuid()))
                    {
                        message += $" {role},";
                    }
                    message = message.Remove(message.Length - 1, 1);
                    if (HUDManager())
                        HUDManager().Chat.AddChat(PlayerControl.LocalPlayer, message);
                }
            }
        }
    }
    [HarmonyPatch(typeof(HudManager))]
    public class LookoutHudTrack
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Lookout)) return;
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var watchButton = __instance.KillButton;

            var role = GetRole<Lookout>(PlayerControl.LocalPlayer);

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(watchButton.cooldownTimerText, watchButton.transform);
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
                role.UsesText.text = role.UsesLeft + "";
            }
            watchButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (role.ButtonUsable) watchButton.SetCoolDown(role.WatchTimer(), CustomGameOptions.WatchCooldown);
            else watchButton.SetCoolDown(0f, CustomGameOptions.WatchCooldown);
            if (role.UsesLeft == 0) return;

            var notWatching = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !role.Watching.ContainsKey(x.PlayerId))
                .ToList();

            SetTarget(ref role.ClosestPlayer, watchButton, float.NaN, notWatching);

            var renderer = watchButton.graphic;
            if (role.ClosestPlayer != null && role.ButtonUsable && PlayerControl.LocalPlayer.moveable)
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
}