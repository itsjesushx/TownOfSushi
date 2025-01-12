namespace TownOfSushi.Roles
{
    public class Venerer : Role
    {
        public KillButton _abilityButton;
        public bool Enabled;
        public DateTime LastCamouflaged;
        public float TimeRemaining;
        public float KillsAtStartAbility;

        public Venerer(PlayerControl player) : base(player)
        {
            Name = "Venerer";
            StartText = () => "With Each Kill Your Ability Becomes Stronger";
            TaskText = () => "Kill players to unlock ability perks";
            RoleInfo = $"The Venerer has multiple abilities during the game, once they reach their first kill, they can camouflage everyone for {CustomGameOptions.AbilityDuration} seconds. After their second kill, they can sprint while camouflaged. After their third kill, they can freeze players in place while sprinting.";
            LoreText = "A relentless force, you grow stronger with every life you take. As the Venerer, each kill you make unlocks new, powerful abilities that enhance your deception and manipulation. The more you strike, the more dangerous you become, allowing you to further deceive, confuse, and overpower the crew. Your power only grows, making you an increasingly deadly threat as time goes on.";
            Color = Colors.Impostor;
            LastCamouflaged = DateTime.UtcNow;
            RoleType = RoleEnum.Venerer;
            Faction = Faction.Impostors;


            RoleAlignment = RoleAlignment.ImpDeception;
        }

        public bool IsCamouflaged => TimeRemaining > 0f;
        public KillButton AbilityButton
        {
            get => _abilityButton;
            set
            {
                _abilityButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float AbilityTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCamouflaged;
            var num = CustomGameOptions.AbilityCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Ability()
        {
            
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            if (Player.Data.IsDead)
            {
                TimeRemaining = 0f;
            }
            GroupCamouflage();
        }


        public void StopAbility()
        {
            Enabled = false;
            LastCamouflaged = DateTime.UtcNow;
            UnCamouflage();
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformVenerer
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Venerer);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (MushroomSabotageActive()) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = GetRole<Venerer>(PlayerControl.LocalPlayer);
            if (__instance == role.AbilityButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.AbilityTimer() != 0 || role.Kills < 1) return false;
                var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;
                StartRPC(CustomRPC.Camouflage, PlayerControl.LocalPlayer.PlayerId, role.Kills);
                role.TimeRemaining = CustomGameOptions.AbilityDuration;
                role.KillsAtStartAbility = role.Kills;
                role.Ability();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudVenerer
    {
        public static Sprite NoneSprite => TownOfSushi.NoAbilitySprite;
        public static Sprite CamoSprite => TownOfSushi.CamouflageSprite;
        public static Sprite CamoSprintSprite => TownOfSushi.CamoSprintSprite;
        public static Sprite CamoSprintFreezeSprite => TownOfSushi.CamoSprintFreezeSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Venerer)) return;
            var role = GetRole<Venerer>(PlayerControl.LocalPlayer);
            if (role.AbilityButton == null)
            {
                role.AbilityButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.AbilityButton.graphic.enabled = true;
                role.AbilityButton.gameObject.SetActive(false);
            }
            if (role.Kills == 0) role.AbilityButton.graphic.sprite = NoneSprite;
            else if (role.Kills == 1) role.AbilityButton.graphic.sprite = CamoSprite;
            else if (role.Kills == 2) role.AbilityButton.graphic.sprite = CamoSprintSprite;
            else role.AbilityButton.graphic.sprite = CamoSprintFreezeSprite;
            role.AbilityButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (role.IsCamouflaged)
            {
                role.AbilityButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.AbilityDuration);
                return;
            }

            if (role.Kills > 0)
            {
                role.AbilityButton.SetCoolDown(role.AbilityTimer(), CustomGameOptions.AbilityCd);
                role.AbilityButton.graphic.color = Palette.EnabledColor;
                role.AbilityButton.graphic.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.AbilityButton.SetCoolDown(0, CustomGameOptions.AbilityCd);
                role.AbilityButton.graphic.color = Palette.DisabledClear;
                role.AbilityButton.graphic.material.SetFloat("_Desat", 1f);
            }

        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class VenererAbility
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Venerer))
            {
                var venerer = (Venerer) role;
                if (venerer.IsCamouflaged)
                    venerer.Ability();
                else if (MushroomSabotageActive()) venerer.StopAbility();
                else if (venerer.Enabled) venerer.StopAbility();
            }
        }
    }
}