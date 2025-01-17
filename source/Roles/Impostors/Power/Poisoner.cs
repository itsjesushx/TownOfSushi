namespace TownOfSushi.Roles
{
    public class Poisoner : Role

    {
        public KillButton _poisonButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastPoisoned;
        public PlayerControl PoisonedPlayer;
        public float TimeRemaining;
        public bool Enabled = false;
        public Poisoner(PlayerControl player) : base(player)
        {
            Name = "Poisoner";
            StartText = () => $"Poison a player to murder them after {CustomGameOptions.PoisonDelay} seconds";
            TaskText = () => "Poison the crewmates";
            RoleInfo = $"The Poisoner can poison a player every {CustomGameOptions.PoisonCd} seconds, after {CustomGameOptions.PoisonDelay} seconds the player die. Players with protection can't be killed by the poisoner. If you kill an Aftermath, you will suicide. If the poisoner is alive in the last 4, they will directly kill instead of poisoning.";
            LoreText = "A master of subtle and deadly arts, the Poisoner thrives in the shadows, silently sowing chaos among the crew. With a lethal touch, they ensure their victims never see their end coming, leaving only whispers of their presence behind.";
            Color = Palette.ImpostorRed;
            LastPoisoned = DateTime.UtcNow;
            RoleType = RoleEnum.Poisoner;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.ImpPower;
            Faction = Faction.Impostors;
            PoisonedPlayer = null;
        }
        public KillButton PoisonButton
        {
            get => _poisonButton;
            set
            {
                _poisonButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public bool Poisoned => TimeRemaining > 0f;
        public void Poison()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            if (MeetingHud.Instance)
            {
                TimeRemaining = 0;
            }
            if (PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count <= 4 && !Player.Data.IsDead)
            {
                TimeRemaining = 0;
            }
            if (TimeRemaining <= 0)
            {
                PoisonKill();
                StartRPC(CustomRPC.PoisonKill, PlayerControl.LocalPlayer.PlayerId);
            }
        }
        public void PoisonKill()
        {
            if (!PoisonedPlayer.IsShielded() && !PoisonedPlayer.IsFortified() && !PoisonedPlayer.Is(RoleEnum.Pestilence) && !PoisonedPlayer.IsProtected() && PoisonedPlayer != ShowRoundOneShield.FirstRoundShielded)
            {
                RpcMurderPlayerNoJump(Player, PoisonedPlayer);
                if (!PoisonedPlayer.Data.IsDead) SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.5f);
                GameHistory.CreateDeathReason(PoisonedPlayer, CustomDeathReason.Poisoned, Player);
            }
            else if (PoisonedPlayer.IsShielded())
            {
                var medic = PoisonedPlayer.GetMedic().Player.PlayerId;
                StartRPC(CustomRPC.AttemptSound, medic, PoisonedPlayer.PlayerId);
                MedicStopKill.BreakShield(medic, PoisonedPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
            }
            PoisonedPlayer = null;
            Enabled = false;
            LastPoisoned = DateTime.UtcNow;
        }
        public float PoisonTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastPoisoned;
            var num = CustomGameOptions.PoisonCd * 1000f;
            if (Player.Is(ModifierEnum.Underdog))
            {
                num = UnderdogPerformKill.LastImp() ? (CustomGameOptions.PoisonCd - CustomGameOptions.UnderdogKillBonus) * 1000f :
                    (UnderdogPerformKill.IncreasedKC() ? CustomGameOptions.PoisonCd * 1000 : (CustomGameOptions.PoisonCd + CustomGameOptions.UnderdogKillBonus) * 1000);
            }
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }

    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class PoisonerHUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner))
            {
                var role = GetRole<Poisoner>(PlayerControl.LocalPlayer);
                role.PoisonButton.graphic.sprite = TownOfSushi.PoisonSprite;
                role.LastPoisoned = DateTime.UtcNow;
            }
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class PoisonerHudManagerUpdate
    {
        public static Sprite PoisonSprite => TownOfSushi.PoisonSprite;
        public static Sprite PoisonedSprite => TownOfSushi.PoisonedSprite;

        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner)) return;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            var role = GetRole<Poisoner>(PlayerControl.LocalPlayer);
            if (role.PoisonButton == null)
            {
                role.PoisonButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.PoisonButton.graphic.enabled = true;
                role.PoisonButton.graphic.sprite = PoisonSprite;
            }

            role.PoisonButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            __instance.KillButton.Hide();

            var position = __instance.KillButton.transform.localPosition;
            role.PoisonButton.transform.localPosition = new Vector3(position.x,
                position.y, position.z);
            var notImp = PlayerControl.AllPlayerControls
                    .ToArray()
                    .Where(x => !x.Is(Faction.Impostors))
                    .ToList();

            if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) SetTarget(ref role.ClosestPlayer, role.PoisonButton);
            else SetTarget(ref role.ClosestPlayer, role.PoisonButton, float.NaN, notImp);

            if (role.ClosestPlayer != null)
            {
                role.ClosestPlayer.myRend().material.SetColor("_OutlineColor", Palette.Purple);
            }

            try
            {
                if (role.Poisoned)
                {
                    role.PoisonButton.graphic.sprite = PoisonedSprite;
                    role.Poison();
                    role.PoisonButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.PoisonDelay);
                }
                else
                {
                    role.PoisonButton.graphic.sprite = PoisonSprite;
                    if (role.PoisonedPlayer && role.PoisonedPlayer != PlayerControl.LocalPlayer)
                    {
                        role.PoisonKill();
                    }
                    if (role.ClosestPlayer != null)
                    {
                        role.PoisonButton.graphic.color = Palette.EnabledColor;
                        role.PoisonButton.graphic.material.SetFloat("_Desat", 0f);
                    }
                    else
                    {
                        role.PoisonButton.graphic.color = Palette.DisabledClear;
                        role.PoisonButton.graphic.material.SetFloat("_Desat", 1f);
                    }
                    role.PoisonButton.SetCoolDown(role.PoisonTimer(), CustomGameOptions.PoisonCd);
                    role.PoisonedPlayer = PlayerControl.LocalPlayer; //Only do this to stop repeatedly trying to re-kill poisoned player. null didn't work for some reason
                }
            }
            catch
            {

            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformPoisonKill
    {
        public static Sprite PoisonSprite => TownOfSushi.PoisonSprite;
        public static Sprite PoisonedSprite => TownOfSushi.PoisonedSprite;
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = GetRole<Poisoner>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;
            if (target == null) return false;
            if (!__instance.isActiveAndEnabled) return false;
            if (role.PoisonTimer() > 0) return false;
            if (role.Enabled == true) return false;
            if (role.Player.inVent)
            {
                role.PoisonButton.SetCoolDown(0.01f, 1f);
                return false;
            }
            if (PlayerControl.LocalPlayer.IsJailed()) return false;
            
            var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return false;
            var interact = Interact(PlayerControl.LocalPlayer, target);
            if (interact[3] == true)
            {
                role.PoisonedPlayer = target;
                role.PoisonButton.SetTarget(null);
                DestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
                role.TimeRemaining = CustomGameOptions.PoisonDelay;
                role.PoisonButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.PoisonDelay);
                StartRPC(CustomRPC.Poison, PlayerControl.LocalPlayer.PlayerId, target.PlayerId);
            }
            if (interact[0] == true)
            {
                role.LastPoisoned = DateTime.UtcNow;;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastPoisoned = DateTime.UtcNow;
                role.LastPoisoned = role.LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.PoisonCd);
                return false;
            }
            else if (interact[2] == true) return false;
            return false;
        }
    }
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    class PoisonStartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo meetingTarget)
        {
            if (__instance == null)
            {
                return;
            }
            var poisoners = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(RoleEnum.Poisoner)).ToList();
            foreach (var poisoner in poisoners)
            {
                var role = GetRole<Poisoner>(poisoner);
                if (poisoner != role.PoisonedPlayer && role.PoisonedPlayer != null)
                {
                    if (!role.PoisonedPlayer.Data.IsDead && !role.PoisonedPlayer.Is(RoleEnum.Pestilence))
                        RpcMurderPlayerNoJump(poisoner, role.PoisonedPlayer);
                }
                return;
            }
        }
    }
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class SetTargetPoisoner
    {
        public static void Postfix(KillButton __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner)) return;
            var role = Role.GetRole<Poisoner>(PlayerControl.LocalPlayer);
            if (target != null && __instance == DestroyableSingleton<HudManager>.Instance.KillButton)
                if (target.Data.IsImpostor())
                {
                    __instance.graphic.color = Palette.DisabledClear;
                    __instance.graphic.material.SetFloat("_Desat", 1f);
                }
        }
    }
}