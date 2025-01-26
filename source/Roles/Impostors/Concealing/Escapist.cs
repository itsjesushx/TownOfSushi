namespace TownOfSushi.Roles
{
    public class Escapist : Role
    {
        public KillButton _escapeButton;
        public DateTime LastEscape;
        public Vector3 EscapePoint = new();
        public Escapist(PlayerControl player) : base(player)
        {
            Name = "Escapist";
            StartText = () => "Get Away From Kills With Ease";
            TaskText = () => "Teleport to get away";
            RoleInfo = "The Escapist can mark a location for them to teleport back to, and can teleport back to that location at any time. They can only teleport back to that location once. After that they must mark a new location.";
            LoreText = "A master of escape, you can slip away from danger in the blink of an eye. As the Escapist, you harness the power of teleportation to evade death and slip past the eyes of those who might catch you in the act. Your ability to vanish at will makes you a dangerous foe, capable of leaving behind confusion and chaos as you retreat to safety.";
            Color = Colors.Impostor;
            RoleType = RoleEnum.Escapist;
            Faction = Faction.Impostors;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.ImpConcealing;
        }
        public KillButton EscapeButton
        {
            get => _escapeButton;
            set
            {
                _escapeButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float EscapeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastEscape;
            var num = CustomGameOptions.EscapeCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public static void Escape(PlayerControl escapist)
        {
            escapist.MyPhysics.ResetMoveState();
            var escapistRole = GetRole<Escapist>(escapist);
            if (escapistRole.EscapePoint == Vector3.zero) return;
            var position = escapistRole.EscapePoint;
            escapist.NetTransform.SnapTo(new Vector2(position.x, position.y));

            if (SubmergedCompatibility.isSubmerged())
            {
                if (PlayerControl.LocalPlayer.PlayerId == escapist.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(escapist.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }

            if (PlayerControl.LocalPlayer.PlayerId == escapist.PlayerId)
            {
                Flash(new Color(0.6f, 0.1f, 0.2f, 1f), 2.5f);
                if (Minigame.Instance) Minigame.Instance.Close();
            }
            escapist.moveable = true;
            escapist.Collider.enabled = true;
            escapist.NetTransform.enabled = true;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudEscape
    {
        public static Sprite MarkSprite => TownOfSushi.MarkSprite;
        public static Sprite EscapeSprite => TownOfSushi.EscapeSprite;
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Escapist)) return;
            var role = GetRole<Escapist>(PlayerControl.LocalPlayer);
            if (role.EscapeButton == null)
            {
                role.EscapeButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.EscapeButton.graphic.enabled = true;
                role.EscapeButton.graphic.sprite = MarkSprite;
                role.EscapeButton.gameObject.SetActive(false);

            }

            if (role.EscapeButton.graphic.sprite != MarkSprite && role.EscapeButton.graphic.sprite != EscapeSprite)
                role.EscapeButton.graphic.sprite = MarkSprite;

            role.EscapeButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.EscapeButton.graphic.color = Palette.EnabledColor;
            role.EscapeButton.graphic.material.SetFloat("_Desat", 0f);
            if (role.EscapeButton.graphic.sprite == MarkSprite) role.EscapeButton.SetCoolDown(0f, 1f);
            else role.EscapeButton.SetCoolDown(role.EscapeTimer(), CustomGameOptions.EscapeCd);
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformEscape
    {
        public static Sprite MarkSprite => TownOfSushi.MarkSprite;
        public static Sprite EscapeSprite => TownOfSushi.EscapeSprite;
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Escapist);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = GetRole<Escapist>(PlayerControl.LocalPlayer);
            if (__instance == role.EscapeButton)
            {
                if (role.Player.inVent) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.EscapeButton.graphic.sprite == MarkSprite)
                {
                    var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
                    if (!abilityUsed) return false;
                    role.EscapePoint = PlayerControl.LocalPlayer.transform.position;
                    role.EscapeButton.graphic.sprite = EscapeSprite;
                    DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
                    if (role.EscapeTimer() < 5f)
                        role.LastEscape = DateTime.UtcNow.AddSeconds(5 - CustomGameOptions.EscapeCd);
                }
                else
                {
                    if (__instance.isCoolingDown) return false;
                    if (role.EscapeTimer() != 0) return false;
                    var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
                    if (!abilityUsed) return false;
                    StartRPC(CustomRPC.Escape, PlayerControl.LocalPlayer.PlayerId, role.EscapePoint);
                    role.LastEscape = DateTime.UtcNow;
                    Escapist.Escape(role.Player);
                }

                return false;
            }

            return true;
        }
    }
}