

namespace TownOfSushi.Roles.Abilities
{
    public class ButtonBarry : Ability
    {
        public KillButton ButtonButton;
        public bool ButtonUsed;
        public DateTime StartingCooldown { get; set; }
        public ButtonBarry(PlayerControl player) : base(player)
        {
            Name = "Button Barry";
            TaskText = () => "Call a button from anywhere at any time";
            Color = ColorManager.ButtonBarry;
            StartingCooldown = DateTime.UtcNow;
            AbilityType = AbilityEnum.ButtonBarry;
        }
        public float StartTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - StartingCooldown;
            var num = 10000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformButtonBarry
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!LocalPlayer().Is(AbilityEnum.ButtonBarry)) return true;
            if (LocalPlayer().Is(RoleEnum.Glitch)) return true;
            if (LocalPlayer().Is(RoleEnum.Agent)) return true;
            if (LocalPlayer().Is(RoleEnum.Hitman)) return true;

            var role = GetAbility<ButtonBarry>(LocalPlayer());
            if (__instance != role.ButtonButton) return true;
            if (!LocalPlayer().CanMove) return false;
            if (IsDead()) return false;
            if (role.ButtonUsed) return false;
            if (role.StartTimer() > 0) return false;
            if (LocalPlayer().RemainingEmergencies <= 0) return false;
            if (!__instance.enabled) return false;

            role.ButtonUsed = true;

            StartRPC(CustomRPC.BarryButton, LocalPlayer().PlayerId);

            if (AmongUsClient.Instance.AmHost)
            {
                MeetingRoomManager.Instance.reporter = LocalPlayer();
                MeetingRoomManager.Instance.target = null;
                AmongUsClient.Instance.DisconnectHandlers.AddUnique(
                    MeetingRoomManager.Instance.Cast<IDisconnectHandler>());
                if (GameManager.Instance.CheckTaskCompletion()) return false;
                HUDManager().OpenMeetingRoom(LocalPlayer());
                LocalPlayer().RpcStartMeeting(null);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class UpdateButton
    {
        public static Sprite Button => TownOfSushi.ButtonSprite;

        public static void Postfix(HudManager __instance)
        {
            UpdateButtonBarry(__instance);
        }

        private static void UpdateButtonBarry(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (LocalPlayer()== null) return;
            if (LocalPlayer().Data == null) return;
            if (!LocalPlayer().Is(AbilityEnum.ButtonBarry)) return;
            if (LocalPlayer().Is(RoleEnum.Glitch)) return;
            if (LocalPlayer().Is(RoleEnum.Agent)) return;


            var role = GetAbility<ButtonBarry>(LocalPlayer());

            if (role.ButtonButton == null)
            {
                role.ButtonButton = Object.Instantiate(__instance.KillButton, __instance.transform.parent);
                role.ButtonButton.GetComponentsInChildren<TMPro.TextMeshPro>()[0].text = "";
                role.ButtonButton.graphic.enabled = true;
                role.ButtonButton.graphic.sprite = Button;
            }

            role.ButtonButton.graphic.sprite = Button;

            role.ButtonButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            role.ButtonButton.SetCoolDown(role.StartTimer(), 10f);
            var renderer = role.ButtonButton.graphic;

            if (__instance.UseButton != null)
            {
                var position1 = __instance.UseButton.transform.position;
                role.ButtonButton.transform.position = new Vector3(
                    Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position1.y,
                    position1.z);
            }
            else
            {
                var position1 = __instance.PetButton.transform.position;
                role.ButtonButton.transform.position = new Vector3(
                    Camera.main.ScreenToWorldPoint(new Vector3(0, 0)).x + 0.75f, position1.y,
                    position1.z);
            }

            if (!role.ButtonUsed && LocalPlayer().RemainingEmergencies > 0)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                return;
            }

            renderer.color = Palette.DisabledClear;
            renderer.material.SetFloat("_Desat", 1f);
        }
    }
}