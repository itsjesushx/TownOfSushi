using TownOfSushi.Extensions;

namespace TownOfSushi.Roles
{
    public class Swooper : Role
    {
        public KillButton _swoopButton;
        public bool Enabled;
        public DateTime LastSwooped;
        public float TimeRemaining;

        public Swooper(PlayerControl player) : base(player)
        {
            Name = "Swooper";
            StartText = () => "Turn Invisible Temporarily";
            TaskText = () => "Turn invisible and sneakily kill";
            RoleInfo = $"The Swooper is an Impostor that can turn invisible for {CustomGameOptions.SwoopDuration} seconds.";
            LoreText = "A shadow in the night, you can disappear from sight and strike without warning. As the Swooper, you can turn invisible for a brief moment, allowing you to sneak up on Crewmates and eliminate them undetected. Your ability to vanish makes you a terrifying presence, capable of taking down your targets without a trace before fading back into the shadows.";
            Color = Colors.Impostor;
            LastSwooped = DateTime.UtcNow;
            RoleType = RoleEnum.Swooper;
            Faction = Faction.Impostors;

            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.ImpSupport;
        }

        public bool IsSwooped => TimeRemaining > 0f;
        public KillButton SwoopButton
        {
            get => _swoopButton;
            set
            {
                _swoopButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float SwoopTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastSwooped;
            var num = CustomGameOptions.SwoopCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Swoop()
        {
            
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            if (Player.Data.IsDead)
            {
                TimeRemaining = 0f;
            }
            var color = Color.clear;
            if (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Data.IsDead) color.a = 0.1f;

            if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
            {
                Player.SetOutfit(CustomPlayerOutfitType.Swooper, new NetworkedPlayerInfo.PlayerOutfit()
                {
                    ColorId = Player.CurrentOutfit.ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    PlayerName = " ",
                    PetId = ""
                });
                Player.myRend().color = color;
                Player.nameText().color = Color.clear;
                Player.cosmetics.colorBlindText.color = Color.clear;
            }
        }


        public void UnSwoop()
        {
            Enabled = false;
            LastSwooped = DateTime.UtcNow;
            Unmorph(Player);
            Player.myRend().color = Color.white;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class SwoopUnswoop
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Swooper))
            {
                var swooper = (Swooper) role;
                if (swooper.IsSwooped)
                    swooper.Swoop();
                else if (MushroomSabotageActive()) swooper.UnSwoop();
                else if (swooper.Enabled) swooper.UnSwoop();
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformSwoop
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Swooper);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (MushroomSabotageActive()) return false;
            var role = GetRole<Swooper>(PlayerControl.LocalPlayer);
            if (__instance == role.SwoopButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.SwoopTimer() != 0) return false;
                var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;

                Rpc(CustomRPC.Swoop, PlayerControl.LocalPlayer.PlayerId);
                role.TimeRemaining = CustomGameOptions.SwoopDuration;
                role.Swoop();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudSwoop
    {
        public static Sprite SwoopSprite => TownOfSushi.SwoopSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Swooper)) return;
            var role = GetRole<Swooper>(PlayerControl.LocalPlayer);
            if (role.SwoopButton == null)
            {
                role.SwoopButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.SwoopButton.graphic.enabled = true;
                role.SwoopButton.gameObject.SetActive(false);
            }
            role.SwoopButton.graphic.sprite = SwoopSprite;
            role.SwoopButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (role.IsSwooped)
            {
                role.SwoopButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.SwoopDuration);
                return;
            }

            role.SwoopButton.SetCoolDown(role.SwoopTimer(), CustomGameOptions.SwoopCd);


            role.SwoopButton.graphic.color = Palette.EnabledColor;
            role.SwoopButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }
}