using System.Collections;

namespace TownOfSushi.Roles
{
    public class Blackmailer : Role
    {
        public KillButton _blackmailButton;
        public PlayerControl ClosestPlayer;
        public PlayerControl Blackmailed;
        public DateTime LastBlackmailed { get; set; }
        public Blackmailer(PlayerControl player) : base(player)
        {
            Name = "Blackmailer";
            StartText = () => "Silence Crewmates During Meetings";
            TaskText = () => "Silence a player for the next meeting";
            RoleInfo = $"The Blackmailer can silence a Crewmate during meetings, preventing them from speaking and casting doubt on their credibility. The target will be unable to speak during the next meeting, allowing the Blackmailer to control the flow of information and sow confusion among the Crewmates.";
            LoreText = "A cunning manipulator, you thrive on controlling information. As the Blackmailer, you can silence a Crewmate during meetings, preventing them from speaking and casting doubt on their credibility. With your ability to shut down key voices, you can turn the tide of discussions and ensure that your allies remain undetected while others are left defenseless.";
            Color = ColorManager.ImpostorRed;
            LastBlackmailed = DateTime.UtcNow;
            RoleType = RoleEnum.Blackmailer;
            Faction = Faction.Impostors;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.ImpSupport;
        }

        public KillButton BlackmailButton
        {
            get => _blackmailButton;
            set
            {
                _blackmailButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public bool CanSeeBlackmailed(byte playerId)
        {
            return !CustomGameOptions.BlackmailInvisible || Blackmailed?.PlayerId == playerId || Player.PlayerId == playerId || PlayerById(playerId).Data.IsDead;
        }
        public float BlackmailTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBlackmailed;
            var num = CustomGameOptions.BlackmailCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }

    public class BlackmailMeetingUpdate
    {
        public static bool shookAlready = false;
        public static Sprite PrevXMark = null;
        public static Sprite PrevOverlay = null;
        public const float LetterXOffset = 0.22f;
        public const float LetterYOffset = -0.32f;

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public class MeetingHudStart
        {
            public static Sprite Letter => TownOfSushi.BlackmailLetterSprite;
            public static void Postfix(MeetingHud __instance)
            {
                shookAlready = false;

                var blackmailers = AllRoles.Where(x => x.RoleType == RoleEnum.Blackmailer && x.Player != null).Cast<Blackmailer>();

                foreach (var role in blackmailers)
                {
                    if (role.Blackmailed?.PlayerId == LocalPlayer().PlayerId && !role.Blackmailed.Data.IsDead)
                    {
                        Coroutines.Start(BlackmailShhh());
                    }
                    if (role.Blackmailed != null && !role.Blackmailed.Data.IsDead && role.CanSeeBlackmailed(LocalPlayer().PlayerId))
                    {
                        var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == role.Blackmailed.PlayerId);

                        playerState.XMark.gameObject.SetActive(true);
                        if (PrevXMark == null) PrevXMark = playerState.XMark.sprite;
                        playerState.XMark.sprite = Letter;
                        playerState.XMark.transform.localScale = playerState.XMark.transform.localScale * 0.75f;
                        playerState.XMark.transform.localPosition = new Vector3(
                            playerState.XMark.transform.localPosition.x + LetterXOffset,
                            playerState.XMark.transform.localPosition.y + LetterYOffset,
                            playerState.XMark.transform.localPosition.z);
                    }
                }
            }

            public static IEnumerator BlackmailShhh()
            {
                yield return HUDManager().CoFadeFullScreen(Color.clear, new Color(0f, 0f, 0f, 0.98f));
                var TempPosition = HUDManager().shhhEmblem.transform.localPosition;
                var TempDuration = HUDManager().shhhEmblem.HoldDuration;
                HUDManager().shhhEmblem.transform.localPosition = new Vector3(
                HUDManager().shhhEmblem.transform.localPosition.x,
                HUDManager().shhhEmblem.transform.localPosition.y,
                HUDManager().FullScreen.transform.position.z + 1f);
                HUDManager().shhhEmblem.TextImage.text = "YOU ARE BLACKMAILED";
                HUDManager().shhhEmblem.HoldDuration = 2.5f;
                yield return HUDManager().ShowEmblem(true);
                HUDManager().shhhEmblem.transform.localPosition = TempPosition;
                HUDManager().shhhEmblem.HoldDuration = TempDuration;
                yield return HUDManager().CoFadeFullScreen(new Color(0f, 0f, 0f, 0.98f), Color.clear);
                yield return null;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public class MeetingHud_UpdateBlackmailer
        {
            public static Sprite Overlay => TownOfSushi.BlackmailOverlaySprite;

            public static void Postfix(MeetingHud __instance)
            {
                var blackmailers = AllRoles.Where(x => x.RoleType == RoleEnum.Blackmailer && x.Player != null).Cast<Blackmailer>();

                foreach (var role in blackmailers)
                {
                    if (role.Blackmailed != null && !role.Blackmailed.Data.IsDead && role.CanSeeBlackmailed(LocalPlayer().PlayerId))
                    {
                        var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == role.Blackmailed.PlayerId);
                        playerState.Overlay.gameObject.SetActive(true);
                        if (PrevOverlay == null) PrevOverlay = playerState.Overlay.sprite;
                        playerState.Overlay.sprite = Overlay;
                        if (__instance.state != MeetingHud.VoteStates.Animating && shookAlready == false)
                        {
                            shookAlready = true;
                            (__instance as MonoBehaviour).StartCoroutine(Effects.SwayX(playerState.transform));
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.SetText))]
        public class StopChatting
        {
            public static bool Prefix(TextBoxTMP __instance)
            {
                var blackmailers = AllRoles.Where(x => x.RoleType == RoleEnum.Blackmailer && x.Player != null).Cast<Blackmailer>();
                foreach (var role in blackmailers)
                {
                    if (Meeting() && role.Blackmailed != null && !role.Blackmailed.Data.IsDead && role.Blackmailed.PlayerId == LocalPlayer().PlayerId)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudBlackmail
    {
        public static Sprite Blackmail => TownOfSushi.BlackmailSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (LocalPlayer()== null) return;
            if (LocalPlayer().Data == null) return;
            if (!LocalPlayer().Is(RoleEnum.Blackmailer)) return;
            var role = GetRole<Blackmailer>(LocalPlayer());
            if (role.BlackmailButton == null)
            {
                role.BlackmailButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.BlackmailButton.graphic.enabled = true;
                role.BlackmailButton.gameObject.SetActive(false);
            }

            if (IsDead()) role.BlackmailButton.SetTarget(null);

            role.BlackmailButton.graphic.sprite = Blackmail;
            role.BlackmailButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            var notBlackmailed = AllPlayers().Where(
                player => role.Blackmailed?.PlayerId != player.PlayerId
            ).ToList();

            role.BlackmailButton.SetCoolDown(role.BlackmailTimer(), CustomGameOptions.BlackmailCd);
            if (LocalPlayer().moveable) Utils.SetTarget(ref role.ClosestPlayer, role.BlackmailButton, float.NaN, notBlackmailed);
            else role.BlackmailButton.SetTarget(null);
            
                if (role.Blackmailed != null && !role.Blackmailed.Data.IsDead && !role.Blackmailed.Data.Disconnected)
                {
                    role.Blackmailed.nameText().color = Color.clear;
                }

                var imps = AllPlayers().Where(
                    player => player.Data.IsImpostor() && player != role.Blackmailed
                ).ToList();

                foreach (var imp in imps)
                {
                    if (imp.GetCustomOutfitType() == CustomPlayerOutfitType.Camouflage ||
                        imp.GetCustomOutfitType() == CustomPlayerOutfitType.Swooper) imp.nameText().color = Color.clear;
                    else if (imp.nameText().color == Color.clear ||
                        imp.nameText().color == new Color(0.3f, 0.0f, 0.0f)) imp.nameText().color = ColorManager.ImpostorRed;
                }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformBlackmail
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!LocalPlayer().Is(RoleEnum.Blackmailer)) return true;
            if (!LocalPlayer().CanMove) return false;
            if (IsDead()) return false;
            var role = GetRole<Blackmailer>(LocalPlayer());
            var target = role.ClosestPlayer;
            if (__instance == role.BlackmailButton)
            {
                if (role.Player.inVent) return false;
                if (!__instance.isActiveAndEnabled || role.ClosestPlayer == null) return false;
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.BlackmailTimer() != 0) return false;

                var interact = Interact(LocalPlayer(), target);
                if (interact[3] == true)
                {
                    role.Blackmailed?.myRend().material.SetFloat("_Outline", 0f);
                    if (role.Blackmailed != null && role.Blackmailed.Data.IsImpostor())
                    {
                        if (role.Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                            role.Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                            role.Blackmailed.nameText().color = ColorManager.ImpostorRed;
                        else role.Blackmailed.nameText().color = Color.clear;
                    }
                    role.Blackmailed = target;
                    StartRPC(CustomRPC.Blackmail, LocalPlayer().PlayerId, target.PlayerId);
                }
                role.BlackmailButton.SetCoolDown(0.01f, 1f);
                return false;
            }
            return true;
        }
    }
}