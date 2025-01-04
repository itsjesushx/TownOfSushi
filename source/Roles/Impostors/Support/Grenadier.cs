namespace TownOfSushi.Roles
{
    public class Grenadier : Role
    {
        public KillButton _flashButton;
        public bool Enabled;
        public DateTime LastFlashed;
        public float TimeRemaining;
        public static Il2CppSystem.Collections.Generic.List<PlayerControl> closestPlayers = null;

        static readonly Color normalVision = new Color(0.6f, 0.6f, 0.6f, 0f);
        static readonly Color dimVision = new Color(0.6f, 0.6f, 0.6f, 0.2f);
        static readonly Color blindVision = new Color(0.6f, 0.6f, 0.6f, 1f);
        public Il2CppSystem.Collections.Generic.List<PlayerControl> flashedPlayers = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

        public Grenadier(PlayerControl player) : base(player)
        {
            Name = "Grenadier";
            StartText = () => "Hinder The Crewmates' Vision";
            TaskText = () => "Blind the crewmates to get sneaky kills";
            RoleInfo = $"The Grenadier can make other players go blind for {CustomGameOptions.GrenadeDuration} seconds. blinding players make their screen go gray, but they can still use all of their abilities. The Grenadier can use this ability every {CustomGameOptions.GrenadeCd} seconds and this will not affect other Impostors or dead players.";
            LoreText = "A specialist in disruption, you excel at blinding the Crewmates and throwing them into confusion. As the Grenadier, you can use blinding grenades to obscure vision, making it easier to move unnoticed and take out your targets. Your ability to create chaos in critical moments gives the Impostors a tactical advantage, allowing you to strike while the crew is disoriented and vulnerable.";
            Color = Colors.Impostor;
            LastFlashed = DateTime.UtcNow;
            RoleType = RoleEnum.Grenadier;
            Faction = Faction.Impostors;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.ImpSupport;
        }

        public bool Flashed => TimeRemaining > 0f;


        public KillButton FlashButton
        {
            get => _flashButton;
            set
            {
                _flashButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float FlashTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastFlashed;
            ;
            var num = CustomGameOptions.GrenadeCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public void Flash()
        {
            if (Enabled != true)
            {
                closestPlayers = GetClosestPlayers(Player.GetTruePosition(), CustomGameOptions.FlashRadius);
                flashedPlayers = closestPlayers;
            }
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            //To stop the scenario where the flash and sabotage are called at the same time.
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var specials = system.specials.ToArray();
            var dummyActive = system.AnyActive;
            var sabActive = specials.Any(s => s.IsActive);

            foreach (var player in closestPlayers)
            {
                if (PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
                {
                    if (TimeRemaining > CustomGameOptions.GrenadeDuration - 0.5f && (!sabActive | dummyActive))
                    {
                        float fade = (TimeRemaining - CustomGameOptions.GrenadeDuration) * -2.0f;
                        if (ShouldPlayerBeBlinded(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color.Lerp(normalVision, blindVision, fade);
                        }
                        else if (ShouldPlayerBeDimmed(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color.Lerp(normalVision, dimVision, fade);
                            if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.sabSystem.Timer < 0.5f)
                            {
                                MapBehaviour.Instance.infectedOverlay.sabSystem.Timer = 0.5f;
                            }
                        }
                        else
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                        }
                    }
                    else if (TimeRemaining <= (CustomGameOptions.GrenadeDuration - 0.5f) && TimeRemaining >= 0.5f && (!sabActive))
                    {
                        if (ShouldPlayerBeBlinded(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = blindVision;
                        }
                        else if (ShouldPlayerBeDimmed(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = dimVision;
                            if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.sabSystem.Timer < 0.5f)
                            {
                                MapBehaviour.Instance.infectedOverlay.sabSystem.Timer = 0.5f;
                            }
                        }
                        else
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                        }
                    }
                    else if (TimeRemaining < 0.5f && (!sabActive | dummyActive))
                    {
                        float fade2 = (TimeRemaining * -2.0f) + 1.0f;
                        if (ShouldPlayerBeBlinded(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color.Lerp(blindVision, normalVision, fade2);
                        }
                        else if (ShouldPlayerBeDimmed(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color.Lerp(dimVision, normalVision, fade2);
                            if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.sabSystem.Timer < 0.5f)
                            {
                                MapBehaviour.Instance.infectedOverlay.sabSystem.Timer = 0.5f;
                            }
                        }
                        else
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                        }
                    }
                    else
                    {
                        ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                        ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                        DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                        TimeRemaining = 0.0f;
                    }
                }
            }

            if (TimeRemaining > 0.5f)
            {
                if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.sabSystem.Timer < 0.5f)
                {
                    MapBehaviour.Instance.infectedOverlay.sabSystem.Timer = 0.5f;
                }
            }
        }

        private static bool ShouldPlayerBeDimmed(PlayerControl player) {
            return (player.Data.IsImpostor() || player.Data.IsDead) && !MeetingHud.Instance;
        }

        private static bool ShouldPlayerBeBlinded(PlayerControl player) {
            return !player.Data.IsImpostor() && !player.Data.IsDead && !MeetingHud.Instance;
        }

        public void UnFlash()
        {
            Enabled = false;
            LastFlashed = DateTime.UtcNow;
            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
            flashedPlayers.Clear();
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class FlashUnFlash
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Grenadier))
            {
                var grenadier = (Grenadier) role;
                if (grenadier.Flashed)
                    grenadier.Flash();
                else if (grenadier.Enabled) grenadier.UnFlash();
            }
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class GrenadierHudManagerUpdatePatch
    {
        public static Sprite FlashSprite => TownOfSushi.FlashSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Grenadier)) return;
            var role = GetRole<Grenadier>(PlayerControl.LocalPlayer);
            if (role.FlashButton == null)
            {
                role.FlashButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.FlashButton.graphic.enabled = true;
                role.FlashButton.gameObject.SetActive(false);
            }

            if (CustomGameOptions.GrenadierIndicators) {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player != PlayerControl.LocalPlayer && !player.Data.IsImpostor()) {
                        var tempColour = player.nameText().color;
                        var data = player?.Data;
                        if (data == null || data.Disconnected || data.IsDead || PlayerControl.LocalPlayer.Data.IsDead)
                            continue;
                        if (role.flashedPlayers.Contains(player)) {
                            player.myRend().material.SetColor("_VisorColor", Color.black);
                            player.nameText().color = Color.black;
                        } else {
                            player.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                            player.nameText().color = tempColour;
                        }
                    }
                }
            }

            role.FlashButton.graphic.sprite = FlashSprite;
            role.FlashButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (role.Flashed)
            {
                role.FlashButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.GrenadeDuration);
                return;
            }

            role.FlashButton.SetCoolDown(role.FlashTimer(), CustomGameOptions.GrenadeCd);

            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var specials = system.specials.ToArray();
            var dummyActive = system.AnyActive;
            var sabActive = specials.Any(s => s.IsActive);

            if (sabActive & !dummyActive)
            {
                role.FlashButton.graphic.color = Palette.DisabledClear;
                role.FlashButton.graphic.material.SetFloat("_Desat", 1f);
                return;
            }

            role.FlashButton.graphic.color = Palette.EnabledColor;
            role.FlashButton.graphic.material.SetFloat("_Desat", 0f);
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformGrenadier
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Grenadier);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = GetRole<Grenadier>(PlayerControl.LocalPlayer);
            if (__instance == role.FlashButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
                var specials = system.specials.ToArray();
                var dummyActive = system.AnyActive;
                var sabActive = specials.Any(s => s.IsActive);
                if (sabActive) return false;
                if (role.FlashTimer() != 0) return false;
                var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;

                Rpc(CustomRPC.FlashGrenade, PlayerControl.LocalPlayer.PlayerId);
                role.TimeRemaining = CustomGameOptions.GrenadeDuration;
                role.Flash();
                return false;
            }

            return true;
        }
    }
}