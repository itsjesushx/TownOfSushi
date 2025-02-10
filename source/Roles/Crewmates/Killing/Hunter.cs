namespace TownOfSushi.Roles
{
    public class Hunter : Role
    {
        public Hunter(PlayerControl player) : base(player)
        {
            var CanRetribute = CustomGameOptions.RetributionOnVote ? " If the Hunter is voted out, the last player that voted the hunter will die." : "";
            Name = "Hunter";
            StartText = () => "Stalk the <color=#FF0000FF>Killers</color>";
            TaskText = () => "Stalk and kill <color=#FF0000FF>Killers</color>";
            RoleInfo = $"As the Hunter, you can stalk players during rounds every {CustomGameOptions.HunterStalkCd} seconds. You can stalk a player for {CustomGameOptions.HunterStalkDuration} seconds. When the stalked players does something suspecious, you will get a flash indicating they did, after that you can aim to kill them, but careful, you can also murder innocent players.{CanRetribute}";
            LoreText = "A relentless predator, you specialize in tracking and eliminating the Impostors hiding among the crew. As the Hunter, your keen instincts guide you as you stalk your prey, ensuring no traitor escapes your sight. Your objective is clear: hunt down and eliminate the Impostors, but you can also harm the Crewmates.";
            Color = ColorManager.Hunter;
            LastStalked = DateTime.UtcNow;
            LastKilled = DateTime.UtcNow;
            RoleAlignment = RoleAlignment.CrewKilling;
            RoleType = RoleEnum.Hunter;
            AddToRoleHistory(RoleType);
            MaxUses = CustomGameOptions.HunterStalkUses;
        }

        private KillButton _stalkButton;
        public PlayerControl ClosestPlayer;
        public PlayerControl ClosestStalkPlayer;
        public PlayerControl StalkedPlayer;
        public PlayerControl LastVoted;
        public List<PlayerControl> CaughtPlayers = new List<PlayerControl>();
        public bool Enabled { get; set; }
        public DateTime LastStalked { get; set; }
        public float StalkDuration { get; set; }
        public DateTime LastKilled { get; set; }
        public int MaxUses { get; set; }
        public TMPro.TextMeshPro UsesText { get; set; }
        public KillButton StalkButton
        {
            get => _stalkButton;
            set
            {
                _stalkButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public bool Stalking => StalkDuration > 0f;
        public bool StalkUsable => MaxUses != 0;
        public float HunterKillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.HunterKillCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public float StalkTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastStalked;
            var num = CustomGameOptions.HunterStalkCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Stalk()
        {
            Enabled = true;
            StalkDuration -= Time.deltaTime;
        }

        public void StopStalking()
        {
            Enabled = false;
            LastStalked = DateTime.UtcNow;
            StalkedPlayer = null;
        }

        public void RpcCatchPlayer(PlayerControl stalked)
        {
            if (LocalPlayer().PlayerId == Player.PlayerId && !IsDead())
            {
                Flash(ColorManager.Hunter, 0.8f);
            }
            CaughtPlayers.Add(stalked);
            StalkDuration = 0;
            StopStalking();
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class HunterMeetingHudUpdate
    {
        public static void Postfix(MeetingHud __instance)
        {
            var _role = GetPlayerRole(LocalPlayer());
            if (_role?.RoleType != RoleEnum.Hunter) return;
            if (LocalPlayer().Data.IsDead) return;
            var role = (Hunter)_role;
            foreach (var state in __instance.playerStates)
            {
                var player = Utils.PlayerById(state.TargetPlayerId);
                var playerData = player?.Data;
                if (playerData == null || playerData.Disconnected)
                {
                    role.CaughtPlayers.Remove(player);
                    continue;
                }
                if (role.CaughtPlayers.Any(pc => pc.PlayerId == player.PlayerId)) state.NameText.color = Color.black;
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformStalk
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!LocalPlayer().Is(RoleEnum.Hunter)) return true;
            if (!LocalPlayer().CanMove) return false;
            if (IsDead()) return false;
            if (__instance.isCoolingDown) return false;
            if (!__instance.isActiveAndEnabled) return false;
            var role = GetRole<Hunter>(LocalPlayer());
            if (__instance == role.StalkButton)
            {
                if (role.ClosestStalkPlayer == null) return false;
                if (!role.StalkUsable) return false;
                if (role.StalkTimer() != 0) return false;
                var stalkInteract = Interact(LocalPlayer(), role.ClosestStalkPlayer, false);
                if (stalkInteract[3] == true)
                {
                    role.StalkDuration = CustomGameOptions.HunterStalkDuration;
                    role.StalkedPlayer = role.ClosestStalkPlayer;
                    role.MaxUses--;
                    role.Stalk();
                    StartRPC(CustomRPC.HunterStalk, LocalPlayer().PlayerId, role.ClosestStalkPlayer.PlayerId);
                }
                if (stalkInteract[0] == true)
                {
                    role.LastStalked = DateTime.UtcNow;
                }
                else if (stalkInteract[1] == true)
                {
                    role.LastStalked = DateTime.UtcNow;
                    role.LastStalked = role.LastKilled.AddSeconds(-CustomGameOptions.HunterKillCd + CustomGameOptions.ProtectKCReset);
                }
                return false;
            }

            if (role.ClosestPlayer == null) return false;
            if (!role.CaughtPlayers.Contains(role.ClosestPlayer)) return false;
            if (role.HunterKillTimer() != 0) return false;
            var distBetweenPlayers = GetDistBetweenPlayers(LocalPlayer(), role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[OptionsManager().currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            var interact = Interact(LocalPlayer(), role.ClosestPlayer, true);
            if (interact[0] == true)
            {
                role.LastKilled = DateTime.UtcNow;
            }
            else if (interact[1] == true)
            {
                role.LastKilled = DateTime.UtcNow;
                role.LastKilled = role.LastKilled.AddSeconds(-CustomGameOptions.HunterKillCd + CustomGameOptions.ProtectKCReset);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CastVote))]
    internal class CastVote
    {
        private static void Postfix(MeetingHud __instance, [HarmonyArgument(0)] byte srcPlayerId, [HarmonyArgument(1)] byte suspectPlayerId)
        {
            var votingPlayer = PlayerById(srcPlayerId);
            var suspectPlayer = PlayerById(suspectPlayerId);
            if (!suspectPlayer.Is(RoleEnum.Hunter)) return;
            var hunter = GetRole<Hunter>(suspectPlayer);
            hunter.LastVoted = votingPlayer;
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    internal class HunterMeetingExiledUpdate
    {
        private static void Postfix(ExileController __instance)
        {
            var exiled = __instance.initData.networkedPlayer;
            if (exiled == null) return;
            var player = exiled.Object;
            if (player.Is(RoleEnum.Hunter) && CustomGameOptions.RetributionOnVote)
            {
                var hunter = GetRole<Hunter>(player);
                if (hunter.LastVoted != null && hunter.LastVoted != player && !hunter.LastVoted.Is(RoleEnum.Pestilence))
                {
                    foreach (var role in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Executioner))
                    {
                        var exe = (Executioner)role;
                        if (exe.target == player) return;
                    }
                    Retribution.MurderPlayer(hunter, hunter.LastVoted);
                }
            }
            foreach (var role in AllRoles.Where(x => x.RoleType == RoleEnum.Hunter))
            {
                var hunter = (Hunter)role;
                hunter.LastVoted = null;
            }
        }
    }

    public class Retribution
    {
        public static void MurderPlayer(Hunter hunter, PlayerControl player)
        {
            if (player.Is(Faction.Crewmates)) hunter.IncorrectShots += 1;
            else hunter.CorrectKills += 1;
            MurderPlayer(player);
        }

        public static void MurderPlayer(
            PlayerControl player
        )
        {
            var hudManager = HUDManager();

            Sound().PlaySound(player.KillSfx, false, 0.8f);
            hudManager.KillOverlay.ShowKillAnimation(player.Data, player.Data);
            
            var amOwner = player.AmOwner;
            if (amOwner)
            {
                hudManager.ShadowQuad.gameObject.SetActive(false);
                player.nameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                player.RpcSetScanner(false);
                ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);
                if (!OptionsManager().currentNormalGameOptions.GhostsDoTasks)
                {
                    for (int i = 0; i < player.myTasks.Count; i++)
                    {
                        PlayerTask playerTask = player.myTasks.ToArray()[i];
                        playerTask.OnRemove();
                        Object.Destroy(playerTask.gameObject);
                    }

                    player.myTasks.Clear();
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                        StringNames.GhostIgnoreTasks,
                        new Il2CppReferenceArray<Il2CppSystem.Object>(0)
                    );
                }
                else
                {
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                        StringNames.GhostDoTasks,
                        new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                }

                player.myTasks.Insert(0, importantTextTask);
            }
            player.Die(DeathReason.Kill, false);

            var deadPlayer = new DeadPlayer
            {
                PlayerId = player.PlayerId,
                KillerId = player.PlayerId,
                KillTime = System.DateTime.UtcNow,
            };

            Murder.KilledPlayers.Add(deadPlayer);

            AssassinExileControllerPatch.AssassinatedPlayers.Add(player);
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class StalkUnstalk
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Hunter))
            {
                var hunter = (Hunter) role;
                if (hunter.Stalking)
                    hunter.Stalk();
                else if (hunter.Enabled) hunter.StopStalking();
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HunterCantReport
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.AmOwner) return;
            if (!__instance.CanMove) return;
            if (!__instance.Is(RoleEnum.Hunter)) return;
            if (CustomGameOptions.HunterBodyReport) return;
            var truePosition = __instance.GetTruePosition();

            var data = __instance.Data;
            var stuff = Physics2D.OverlapCircleAll(truePosition, __instance.MaxReportDistance, Constants.Usables);
            var flag = (OptionsManager().currentNormalGameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) && __instance.CanMove;
            var flag2 = false;

            foreach (var collider2D in stuff)
                if (flag && !data.IsDead && !flag2 && collider2D.tag == "DeadBody")
                {
                    var component = collider2D.GetComponent<DeadBody>();

                    if (Vector2.Distance(truePosition, component.TruePosition) <= __instance.MaxReportDistance)
                    {
                        var matches = Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == component.ParentId);
                        if (matches != null && matches.KillerId != LocalPlayer().PlayerId) { 
                            if (!PhysicsHelpers.AnythingBetween(__instance.Collider, truePosition, component.TruePosition, Constants.ShipOnlyMask, false)) flag2 = true; 
                        }
                    }
                }

            HUDManager().ReportButton.SetActive(flag2);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportClosest))]
    public static class HunterDontReport
    {
        public static bool Prefix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Hunter)) return true;
            if (CustomGameOptions.HunterBodyReport) return true;

            if (AmongUsClient.Instance.IsGameOver) return false;
            if (IsDead()) return false;
            foreach (var collider2D in Physics2D.OverlapCircleAll(__instance.GetTruePosition(),
                __instance.MaxReportDistance, Constants.PlayersOnlyMask))
                if (!(collider2D.tag != "DeadBody"))
                {
                    var component = collider2D.GetComponent<DeadBody>();
                    if (component && !component.Reported)
                    {
                        var matches = Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == component.ParentId);
                        if (matches != null && matches.KillerId != LocalPlayer().PlayerId)
                            component.OnClick();
                        if (component.Reported) break;
                    }
                }

            return false;
        }
    }

    [HarmonyPatch(typeof(DeadBody), nameof(DeadBody.OnClick))]
    public static class HunterDontClick
    {
        public static bool Prefix(DeadBody __instance)
        {
            if (!LocalPlayer().Is(RoleEnum.Hunter)) return true;
            if (CustomGameOptions.HunterBodyReport) return true;

            if (AmongUsClient.Instance.IsGameOver) return false;
            if (IsDead()) return false;

            var matches = Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == __instance.ParentId);
            if (matches != null && matches.KillerId != LocalPlayer().PlayerId) return true;
            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudStalk
    {
        public static Sprite StalkSprite => TownOfSushi.StalkSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (NullLocalPlayer()) return;
            if (NullLocalPlayerData()) return;
            if (!LocalPlayer().Is(RoleEnum.Hunter)) return;

            var role = GetRole<Hunter>(LocalPlayer());

                foreach (var player in role.CaughtPlayers)
                {
                    var data = player.Data;
                    if (data == null || data.Disconnected || data.IsDead || IsDead())
                        continue;

                    var colour = Color.black;
                    player.nameText().color = colour;
                }
            

            if (role.StalkButton == null)
            {
                role.StalkButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.StalkButton.graphic.enabled = true;
                role.StalkButton.gameObject.SetActive(false);
            }

            role.StalkButton.graphic.sprite = StalkSprite;
            role.StalkButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            if (role.UsesText == null && role.MaxUses > 0)
            {
                role.UsesText = Object.Instantiate(role.StalkButton.cooldownTimerText, role.StalkButton.transform);
                role.UsesText.gameObject.SetActive(false);
                role.UsesText.transform.localPosition = new Vector3(
                    role.UsesText.transform.localPosition.x + 0.26f,
                    role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }
            if (role.UsesText != null)
            {
                role.UsesText.text = role.MaxUses + "";
            }

            if (IsDead()) role.StalkButton.SetTarget(null);

            role.StalkButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (role.Stalking) role.StalkButton.SetCoolDown(role.StalkDuration, CustomGameOptions.HunterStalkDuration);
            else if (role.StalkUsable) role.StalkButton.SetCoolDown(role.StalkTimer(), CustomGameOptions.HunterStalkCd);
            else role.StalkButton.SetCoolDown(0f, CustomGameOptions.HunterStalkCd);

            var renderer = role.StalkButton.graphic;
            if (role.Stalking || role.MaxUses == 0 || !LocalPlayer().moveable) role.StalkButton.SetTarget(null);
            else
            {
                if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) Utils.SetTarget(ref role.ClosestStalkPlayer, role.StalkButton, float.NaN);
                else Utils.SetTarget(ref role.ClosestStalkPlayer, role.StalkButton, float.NaN);
            }

            if (role.Stalking || (role.StalkUsable && role.ClosestStalkPlayer != null && LocalPlayer().moveable))
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.UsesText.color = Palette.EnabledColor;
                role.UsesText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                role.UsesText.color = Palette.DisabledClear;
                role.UsesText.material.SetFloat("_Desat", 1f);
            }

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            __instance.KillButton.SetCoolDown(role.HunterKillTimer(), CustomGameOptions.HunterKillCd);
            if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, role.CaughtPlayers);
            else SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, role.CaughtPlayers);

            return;
        }
    }
}