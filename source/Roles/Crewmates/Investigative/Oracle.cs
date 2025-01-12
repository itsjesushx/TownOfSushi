namespace TownOfSushi.Roles
{
    public class Oracle : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl Confessor;
        public float Accuracy;
        public bool FirstMeetingDead;
        public RoleAlignment RevealedAlignment;
        public Faction RevealedFaction;
        public DateTime LastConfessed { get; set; }
        public Oracle(PlayerControl player) : base(player)
        {
            Name = "Oracle";
            StartText = () => "Get other payer's to confess their sins";
            TaskText = () => "Get another player to confess on your passing";
            RoleInfo = $"The Oracle can compel another player to confess their secrets upon death. The oracle will get information about 3 players being possibly evil each meeting. The Oracle can only make a player confess once per meeting. When the Oracle dies, the player they made confess will be reveal their faction with a probability of {CustomGameOptions.RevealAccuracy}% to be right.";
            LoreText = "A detective blessed with the power of foresight, you can guide the crew even in death. As the Oracle, your final moments are crucial, compelling another player to confess their secrets. Your revelations can sway the tide of suspicion and uncover the truth hidden within the shadows of the ship.";
            Color = Colors.Oracle;
            LastConfessed = DateTime.UtcNow;
            Accuracy = CustomGameOptions.RevealAccuracy;
            Faction = Faction.Crewmates;

            RoleAlignment = RoleAlignment.CrewInvest;
            FirstMeetingDead = true;
            FirstMeetingDead = false;
            RoleType = RoleEnum.Oracle;
        }
        public float ConfessTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastConfessed;
            var num = CustomGameOptions.ConfessCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HighlightConfessor
    {
        public static void UpdateMeeting(Oracle role, MeetingHud __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    if (player == role.Confessor)
                    {
                        if (role.RevealedFaction == Faction.Crewmates) state.NameText.text = "<color=#00FFFFFF>(Crew) </color>" + state.NameText.text;
                        else if (role.RevealedFaction == Faction.Impostors) state.NameText.text = "<color=#FF0000FF>(Imp) </color>" + state.NameText.text;
                        else state.NameText.text = "<color=#808080FF>(Neut) </color>" + state.NameText.text;
                    }
                }
            }
        }
        public static void Postfix(HudManager __instance)
        {
            if (!MeetingHud.Instance || PlayerControl.LocalPlayer.Data.IsDead) return;
            foreach (var oracle in GetRoles(RoleEnum.Oracle))
            {
                var role = GetRole<Oracle>(oracle.Player);
                if (!role.Player.Data.IsDead || role.Confessor == null) return;
                UpdateMeeting(role, MeetingHud.Instance);
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformConfess
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Oracle);
            if (!flag) return true;
            var role = GetRole<Oracle>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.ConfessTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = KillDistance();
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[3] == true)
            {
                role.Confessor = role.ClosestPlayer;
                bool showsCorrectFaction = true;
                int faction = 1;
                if (role.Accuracy == 0f) showsCorrectFaction = false;
                else
                {
                    var num = Random.RandomRangeInt(1, 101);
                    showsCorrectFaction = num <= role.Accuracy;
                }
                if (showsCorrectFaction)
                {
                    if (role.Confessor.Is(Faction.Crewmates)) faction = 0;
                    else if (role.Confessor.Is(Faction.Impostors)) faction = 2;
                }
                else
                {
                    var num = UnityEngine.Random.RandomRangeInt(0, 2);
                    if (role.Confessor.Is(Faction.Impostors)) faction = num;
                    else if (role.Confessor.Is(Faction.Crewmates)) faction = num + 1;
                    else if (num == 1) faction = 2;
                    else faction = 0;
                }
                if (faction == 0) role.RevealedFaction = Faction.Crewmates;
                else if (faction == 1) role.RevealedAlignment = RoleAlignment.NeutralEvil;
                else role.RevealedFaction = Faction.Impostors;
                StartRPC(CustomRPC.Confess, PlayerControl.LocalPlayer.PlayerId, role.Confessor.PlayerId, faction);
                
            }
            if (interact[0] == true)
            {
                role.LastConfessed = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastConfessed = DateTime.UtcNow;
                role.LastConfessed = role.LastConfessed.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.ConfessCd);
                return false;
            }
            else if (interact[2] == true) return false;
            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    public class HudConfess
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Oracle)) return;
            var confessButton = __instance.KillButton;

            var role = GetRole<Oracle>(PlayerControl.LocalPlayer);

            confessButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            confessButton.SetCoolDown(role.ConfessTimer(), CustomGameOptions.ConfessCd);

            var notConfessing = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => x != role.Confessor)
                .ToList();

            SetTarget(ref role.ClosestPlayer, confessButton, float.NaN, notConfessing);

            var renderer = confessButton.graphic;

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

     [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStartOracle
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Oracle)) return;
            var oracleRole = GetRole<Oracle>(PlayerControl.LocalPlayer);
            if (oracleRole.Confessor != null)
            {
                var playerResults = PlayerReportFeedback(oracleRole.Confessor);

                if (!string.IsNullOrWhiteSpace(playerResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, playerResults);
            }
        }

        public static string PlayerReportFeedback(PlayerControl player)
        {
            if (player.Data.IsDead || player.Data.Disconnected) return ColorString(Colors.Impostor, "Your confessor failed to survive so you received no confession");
            var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x != PlayerControl.LocalPlayer && x != player).ToList();
            if (allPlayers.Count < 2) return "Too few people alive to receive a confessional";
            var evilPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected &&
            (x.Is(Faction.Impostors) || (x.Is(RoleAlignment.NeutralKilling) && CustomGameOptions.NeutralKillingShowsEvil) ||
            (x.Is(RoleAlignment.NeutralEvil) && CustomGameOptions.NeutralEvilShowsEvil) || (x.Is(RoleAlignment.NeutralBenign) && CustomGameOptions.NeutralBenignShowsEvil) || (x.Is(RoleAlignment.NeutralBenign) && CustomGameOptions.NeutralBenignShowsEvil))).ToList();
            if (evilPlayers.Count == 0) return $"{player.GetDefaultOutfit().PlayerName} " + ColorString(Colors.Crewmate, "confesses to knowing that there are no more evil players!"); 
            allPlayers.Shuffle();
            evilPlayers.Shuffle();
            var secondPlayer = allPlayers[0];
            var firstTwoEvil = false;
            foreach (var evilPlayer in evilPlayers)
            {
                if (evilPlayer == player || evilPlayer == secondPlayer) firstTwoEvil = true;
            }
            if (firstTwoEvil)
            {
                var thirdPlayer = allPlayers[1];
                return $"{player.GetDefaultOutfit().PlayerName} confesses to knowing that they, {secondPlayer.GetDefaultOutfit().PlayerName} and/or {thirdPlayer.GetDefaultOutfit().PlayerName} is evil!";
            }
            else
            {
                var thirdPlayer = evilPlayers[0];
                return $"{player.GetDefaultOutfit().PlayerName} confesses to knowing that they, {secondPlayer.GetDefaultOutfit().PlayerName} and/or {thirdPlayer.GetDefaultOutfit().PlayerName} is evil!";
            }
        }
    }
}