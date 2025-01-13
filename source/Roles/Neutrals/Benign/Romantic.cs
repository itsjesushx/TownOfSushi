namespace TownOfSushi.Roles
{
    public class Romantic : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl Beloved;
        public DateTime LastPick;
        public bool SpawnedAs = true;
        public bool AlreadyPicked = false;
        public Romantic(PlayerControl player) : base(player)
        {
            var ChooseOrNew = CustomGameOptions.RomanticOnBelovedDeath == RomanticBecomeOptions.Repick ? "have to choose a new partner" : $"become {CustomGameOptions.RomanticOnBelovedDeath.ToString()} on your partner's death";
            Name = "Romantic";
            StartText = () => "Pick a beloved to win with them";
            TaskText = () => SpawnedAs ? "Protect and assist your beloved" : "Your beloved died. Pick a new one!";
            RoleInfo = "As the Romantic, you must pick a player to love, working together to ensure both of your survival. " + ChooseOrNew + ".";
            LoreText =$"A heart bound by love, you are driven by a deep connection to your chosen beloved. As the Romantic, you must pick a Crewmate to ally with, working together to ensure both of your survival. Your loyalty gives you strength, and you’ll do whatever it takes to protect and support your beloved. If they fall, you will {ChooseOrNew}.";
            Color = Colors.Romantic;
            RoleType = RoleEnum.Romantic;
            Faction = Faction.Neutral;
            LastPick = DateTime.UtcNow;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralBenign;
        }

        public float PickTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastPick;
            var num = CustomGameOptions.PickStartTimer * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformPickBeloved
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Romantic);
            if (!flag) return true;
            var role = GetRole<Romantic>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.PickTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = KillDistance();
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;
            if (role.AlreadyPicked) return false;

            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[3] == true)
            {
                role.Beloved = role.ClosestPlayer;
                role.AlreadyPicked = true;
                StartRPC(CustomRPC.SetRomanticTarget, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
            }
            if (interact[0] == true)
            {
                role.LastPick = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastPick = DateTime.UtcNow;
                role.LastPick = role.LastPick.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.PickStartTimer);
                return false;
            }
            else if (interact[2] == true) return false;
            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    public class HudPick
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Romantic)) return;
            var role = GetRole<Romantic>(PlayerControl.LocalPlayer);
            if (role.AlreadyPicked) return;
            
            var pickButton = __instance.KillButton;
            pickButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            pickButton.SetCoolDown(role.PickTimer(), CustomGameOptions.PickStartTimer);
            SetTarget(ref role.ClosestPlayer, pickButton, float.NaN);

            var renderer = pickButton.graphic;
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

    public static class LoversChat
    {
        private static DateTime MeetingStartTime = DateTime.MinValue;
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public class MeetingStart
        {
            public static void Prefix(MeetingHud __instance)
            {
                MeetingStartTime = DateTime.UtcNow;
            }
        }
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
        public static class AddChat
        {
            public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer)
            {
                if (__instance != HudManager.Instance.Chat) return true;

                var localPlayer = PlayerControl.LocalPlayer;
                if (localPlayer == null) return true;
                Boolean shouldSeeMessage = localPlayer.Data.IsDead || localPlayer.RomanticCoupleChat(sourcePlayer) ||
                    sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId;
                if (DateTime.UtcNow - MeetingStartTime < TimeSpan.FromSeconds(1))
                {
                    return shouldSeeMessage;
                }
                return MeetingHud.Instance != null || LobbyBehaviour.Instance != null || shouldSeeMessage;
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class EnableChat
        {
            public static void Postfix(HudManager __instance)
            {
                
                if (PlayerControl.LocalPlayer.HasRomanticCouple() & !__instance.Chat.isActiveAndEnabled)
                    __instance.Chat.SetVisible(true);
            }
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class RomanticChangeRolePatch
    {
        private static void UpdateMeeting(MeetingHud __instance, Romantic role)
        {
            if (CustomGameOptions.GAKnowsTargetRole) return;
            foreach (var player in __instance.playerStates)
                if (player.TargetPlayerId == role.Beloved.PlayerId)
                    player.NameText.text += "<color=#FF66CCFF> [♥]</color>";
        }

        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Romantic)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            var role = GetRole<Romantic>(PlayerControl.LocalPlayer);

            if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance, role);

            if (!CustomGameOptions.RomanticKnowsBelovedRole && !CamouflageUnCamouflagePatch.IsCamouflaged) role.Beloved.nameText().text += "<color=#FF66CCFF> ♥</color>";

            if (!role.Beloved.Data.IsDead && !role.Beloved.Data.Disconnected) return;

            StartRPC(CustomRPC.RomanticChangeRole, PlayerControl.LocalPlayer.PlayerId);
            DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);

            RomanticChangeRole(PlayerControl.LocalPlayer);
        }

        public static void RomanticChangeRole(PlayerControl player)
        {
            var ga = GetRole<Romantic>(player);
            player.myTasks.RemoveAt(0);
            RoleDictionary.Remove(player.PlayerId);

            if (CustomGameOptions.RomanticOnBelovedDeath == RomanticBecomeOptions.Jester)
            {
                var jester = new Jester(player);
                jester.SpawnedAs = false;
                jester.ReDoTaskText();
            }
            else if (CustomGameOptions.RomanticOnBelovedDeath == RomanticBecomeOptions.Amnesiac)
            {
                var amnesiac = new Amnesiac(player);
                amnesiac.SpawnedAs = false;
                amnesiac.ReDoTaskText();
            }
            else if (CustomGameOptions.RomanticOnBelovedDeath == RomanticBecomeOptions.Repick)
            {
                var amnesiac = new Romantic(player);
                amnesiac.SpawnedAs = false;
                amnesiac.ReDoTaskText();
            }
            else
            {
                new Crewmate(player);
            }
        }
    }
}