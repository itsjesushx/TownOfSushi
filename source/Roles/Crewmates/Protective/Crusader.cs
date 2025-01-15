namespace TownOfSushi.Roles
{
    public class Crusader : Role
    {
        public PlayerControl Fortified;
        public PlayerControl ClosestPlayer;
        public DateTime LastFortify;
        public Crusader (PlayerControl player) : base (player)
        {
            Name = "Crusader";
            StartText = () => "Protect a player from attacks";
            TaskText = () => "Fortify a player to protect them";
            RoleInfo = "As the Crusader, you can Fortify a crewmate with a spell to prevent them from being killed or interacted with by anyone. The spell lasts until next meeting. When the player has a kill attempt, the fortified player will murder the killer.";
            LoreText = "A devout guardian of the crew, the Crusader wields their protective spell to shield others from harm. Steadfast and unwavering, they stand ready to strike down any who dare threaten those under their protection.";
            Color = Colors.Crusader;
            RoleType = RoleEnum.Crusader;
            Faction = Faction.Crewmates;
            LastFortify = DateTime.UtcNow;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewProtect;
            Fortified = null;
        }
        public float FortifyTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastFortify;
            var num = CustomGameOptions.FortifyCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
    [HarmonyPatch(typeof(HudManager))]
    public class HudFortify
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Crusader)) return;
            var fortifyButton = __instance.KillButton;

            var role = GetRole<Crusader>(PlayerControl.LocalPlayer);

            fortifyButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            fortifyButton.SetCoolDown(role.FortifyTimer(), CustomGameOptions.FortifyCd);

            var notFortified = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => x != role.Fortified)
                .ToList();

            SetTarget(ref role.ClosestPlayer, fortifyButton, float.NaN, notFortified);

            var renderer = fortifyButton.graphic;

            if (role.ClosestPlayer != null)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformFortify
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Crusader);
            if (!flag) return true;
            var role = GetRole<Crusader>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.FortifyTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = KillDistance();
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[3] == true)
            {
                role.Fortified = role.ClosestPlayer;
                StartRPC(CustomRPC.Fortify, (byte)0, PlayerControl.LocalPlayer.PlayerId, role.Fortified.PlayerId);
            }
            if (interact[0] == true)
            {
                role.LastFortify = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastFortify = DateTime.UtcNow;
                role.LastFortify = role.LastFortify.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.FortifyCd);
                return false;
            }
            else if (interact[2] == true) return false;
            return false;
        }
    }
}