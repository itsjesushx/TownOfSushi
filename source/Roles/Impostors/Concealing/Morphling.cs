namespace TownOfSushi.Roles
{
    public class Morphling : Role, IVisualAlteration
    {
        public KillButton _morphButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastMorphed;
        public PlayerControl MorphedPlayer;
        public PlayerControl SampledPlayer;
        public float TimeRemaining;

        public Morphling(PlayerControl player) : base(player)
        {
            Name = "Morphling";
            StartText = () => "Transform into crewmates";
            TaskText = () => "Morph into crewmates";
            RoleInfo = $"The Morphling can morph into the form of their fellow Crewmates, morphing changes the Morphling's look to make them not look sus. The morphling can only morph into a crewmate once every {CustomGameOptions.MorphlingCd} seconds and lasts for {CustomGameOptions.MorphlingDuration} seconds.";
            LoreText = "A master of disguise, you possess the ability to transform into any Crewmate. As the Morphling, you can morph into the form of your fellow Crewmates, blending in with the innocent and deceiving your enemies. Your power of transformation allows you to infiltrate and manipulate, making you an elusive and dangerous Impostor who can strike without warning.";
            Color = ColorManager.ImpostorRed;
            LastMorphed = DateTime.UtcNow;
            RoleType = RoleEnum.Morphling;
            Faction = Faction.Impostors;

            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.ImpConcealing;
        }

        public KillButton MorphButton
        {
            get => _morphButton;
            set
            {
                _morphButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public bool Morphed => TimeRemaining > 0f;

        public void MorphlingMorph()
        {            
            TimeRemaining -= Time.deltaTime;
            Morph(Player, MorphedPlayer);
            if (Player.Data.IsDead)
            {
                TimeRemaining = 0f;
            }
        }

        public void MorphlingUnmorph()
        {
            MorphedPlayer = null;
            Unmorph(Player);
            LastMorphed = DateTime.UtcNow;
        }

        public float MorphTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMorphed;
            var num = CustomGameOptions.MorphlingCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (Morphed)
            {
                appearance = MorphedPlayer.GetDefaultAppearance();
                var modifier = GetModifier(MorphedPlayer);
                var ability = GetAbility(MorphedPlayer);
                if (modifier is IVisualAlteration alteration)
                    alteration.TryGetModifiedAppearance(out appearance);
                else if (ability is IVisualAlteration alteration2)
                    alteration2.TryGetModifiedAppearance(out appearance);
                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudMorph
    {
        public static Sprite SampleSprite => TownOfSushi.SampleSprite;
        public static Sprite MorphSprite => TownOfSushi.MorphSprite;


        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (LocalPlayer()== null) return;
            if (LocalPlayer().Data == null) return;
            if (!LocalPlayer().Is(RoleEnum.Morphling)) return;
            var role = GetRole<Morphling>(LocalPlayer());
            if (role.MorphButton == null)
            {
                role.MorphButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.MorphButton.graphic.enabled = true;
                role.MorphButton.graphic.sprite = SampleSprite;
                role.MorphButton.gameObject.SetActive(false);

            }

            if (role.MorphButton.graphic.sprite != SampleSprite && role.MorphButton.graphic.sprite != MorphSprite)
                role.MorphButton.graphic.sprite = SampleSprite;

            role.MorphButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (role.MorphButton.graphic.sprite == SampleSprite)
            {
                role.MorphButton.SetCoolDown(0f, 1f);
                SetTarget(ref role.ClosestPlayer, role.MorphButton);
            }
            else
            {
                if (role.Morphed)
                {
                    role.MorphButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.MorphlingDuration);
                    return;
                }

                role.MorphButton.SetCoolDown(role.MorphTimer(), CustomGameOptions.MorphlingCd);
                role.MorphButton.graphic.color = Palette.EnabledColor;
                role.MorphButton.graphic.material.SetFloat("_Desat", 0f);
            }
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class MorphUnmorph
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Morphling))
            {
                var morphling = (Morphling) role;
                if (morphling.Morphed)
                    morphling.MorphlingMorph();
                else if (MushroomSabotageActive()) morphling.MorphlingUnmorph();
                else if (morphling.MorphedPlayer) morphling.MorphlingUnmorph();
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformMorph
    {
        public static Sprite SampleSprite => TownOfSushi.SampleSprite;
        public static Sprite MorphSprite => TownOfSushi.MorphSprite;
        public static bool Prefix(KillButton __instance)
        {
            var flag = LocalPlayer().Is(RoleEnum.Morphling);
            if (!flag) return true;
            if (!LocalPlayer().CanMove) return false;
            if (MushroomSabotageActive()) return false;
            if (IsDead()) return false;
            var role = GetRole<Morphling>(LocalPlayer());
            var target = role.ClosestPlayer;
            if (__instance == role.MorphButton)
            {
                if (!__instance.isActiveAndEnabled) return false;
                if (role.MorphButton.graphic.sprite == SampleSprite)
                {
                    if (target == null) return false;
                    var abilityUsed = AbilityUsed(LocalPlayer());
                    if (!abilityUsed) return false;
                    role.SampledPlayer = target;
                    role.MorphButton.graphic.sprite = MorphSprite;
                    role.MorphButton.SetTarget(null);
                    HUDManager().KillButton.SetTarget(null);
                    if (role.MorphTimer() < 5f)
                        role.LastMorphed = DateTime.UtcNow.AddSeconds(5 - CustomGameOptions.MorphlingCd);
                }
                else
                {
                    if (__instance.isCoolingDown) return false;
                    if (role.MorphTimer() != 0) return false;
                    var abilityUsed = AbilityUsed(LocalPlayer());
                    if (!abilityUsed) return false;
                    StartRPC(CustomRPC.Morph, LocalPlayer().PlayerId, role.SampledPlayer.PlayerId);
                    role.TimeRemaining = CustomGameOptions.MorphlingDuration;
                    role.MorphedPlayer = role.SampledPlayer;
                    Morph(role.Player, role.SampledPlayer);
                }

                return false;
            }

            return true;
        }
    }
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class SetTargetMorphling
    {
        public static void Postfix(KillButton __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (LocalPlayer()== null) return;
            if (LocalPlayer().Data == null) return;
            if (!LocalPlayer().Is(RoleEnum.Morphling)) return;
            if (target != null && __instance == HUDManager().KillButton)
            if (target.Data.IsImpostor())
            {
                __instance.graphic.color = Palette.DisabledClear;
                __instance.graphic.material.SetFloat("_Desat", 1f);
            }
        }
    }
}
