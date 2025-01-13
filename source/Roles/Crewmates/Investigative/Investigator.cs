namespace TownOfSushi.Roles
{
    public class Investigator : Role
    {
        public readonly List<Footprint> AllPrints = new List<Footprint>();
        private KillButton _examineButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastExamined { get; set; }
        public DeadBody CurrentTarget;
        public bool ExamineMode = false;
        public PlayerControl DetectedKiller;
        public Investigator(PlayerControl player) : base(player)
        {
            Name = "Investigator";
            StartText = () => "Find all Impostors by examining footprints";
            TaskText = () => "Watch steps and examine players";
            RoleInfo = "As the Investigator, you are able to examine dead bodies to find information about the killer. You can also see footprints of other players. After examinating a body, you may examine an alive player in order to see if they have killed or not, if they did, you will get a red flash, otherwise will give you a green flash.";
            LoreText = "An experienced detective aboard the ship, you have honed your skills in tracking and deduction. By examining footprints, you piece together the movements of your crewmates, uncovering lies and identifying those who threaten the crew’s safety. It’s your mission to bring the truth to light and expose the Impostors hiding among you.";
            LastExamined = DateTime.UtcNow;
            Color = Colors.Investigator;
            RoleType = RoleEnum.Investigator;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewInvest;
        }
        public KillButton ExamineButton
        {
            get => _examineButton;
            set
            {
                _examineButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float ExamineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastExamined;
            var num = CustomGameOptions.ExamineCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    internal class BodyReportPatch
    {
        private static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo info)
        {
            if (info == null) return;
            var matches = Murder.KilledPlayers.Where(x => x.PlayerId == info.PlayerId).ToArray();
            DeadPlayer killer = null;

            if (matches.Length > 0)
                killer = matches[0];

            if (killer == null)
                return;

            var isInvestigatorAlive = __instance.Is(RoleEnum.Investigator);
            var areReportsEnabled = CustomGameOptions.InvestigatorReportOn;

            if (!isInvestigatorAlive || !areReportsEnabled)
                return;

            var isUserInvestigator = PlayerControl.LocalPlayer.Is(RoleEnum.Investigator);
            if (!isUserInvestigator)
                return;
            var br = new BodyReport
            {
                Killer = PlayerById(killer.KillerId),
                Reporter = __instance,
                Body = PlayerById(killer.PlayerId),
                KillAge = (float) (DateTime.UtcNow - killer.KillTime).TotalMilliseconds
            };

            var reportMsg = BodyReport.ParseBodyReport(br);

            if (string.IsNullOrWhiteSpace(reportMsg))
                return;

            if (DestroyableSingleton<HudManager>.Instance)
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, reportMsg);
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class AddPrints
    {
        private static float _time;

        public static bool GameStarted = false;
        private static float Interval => CustomGameOptions.FootprintInterval;
        private static bool Vent => CustomGameOptions.VentFootprintVisible;

        private static Vector2 Position(PlayerControl player)
        {
            return player.GetTruePosition() + new Vector2(0, 0.366667f);
        }


        public static void Postfix(HudManager __instance)
        {
            if ((GameManager.Instance && !GameManager.Instance.GameHasStarted) || !PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) return;
            if (MeetingHud.Instance) return;
            // New Footprint
            var investigator = GetRole<Investigator>(PlayerControl.LocalPlayer);

            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                Footprint.DestroyAll(investigator);
                return;
            }

            _time += Time.deltaTime;
            if (_time >= Interval)
            {
                _time -= Interval;
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player == null || player.Data.IsDead ||
                        player.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;
                    var canPlace = !investigator.AllPrints.Any(print =>
                        Vector3.Distance(print.Position, Position(player)) < 0.5f &&
                        print.Color.a > 0.5 &&
                        print.Player.PlayerId == player.PlayerId);

                    if (Vent && ShipStatus.Instance != null)
                        if (ShipStatus.Instance.AllVents.Any(vent =>
                            Vector2.Distance(vent.gameObject.transform.position, Position(player)) < 1f))
                            canPlace = false;

                    if (canPlace) new Footprint(player, investigator);
                }

                for (var i = 0; i < investigator.AllPrints.Count; i++)
                {
                    try
                    {
                        var footprint = investigator.AllPrints[i];
                        if (footprint.Update()) i--;
                    } catch
                    {
                        //assume footprint value is null and allow the loop to continue
                        continue;
                    }
                    
                }
            }
        }
    }

    public class BodyReport
    {
        public PlayerControl Killer { get; set; }
        public PlayerControl Reporter { get; set; }
        public PlayerControl Body { get; set; }
        public float KillAge { get; set; }

        public static string ParseBodyReport(BodyReport br)
        {
            if (br.KillAge > CustomGameOptions.InvestigatorFactionDuration * 1000)
                return
                    $"Body Report: The corpse is too old to gain information from. (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            if (br.Killer.PlayerId == br.Body.PlayerId)
                return
                    $"Body Report: The kill appears to have been a suicide! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            var role = GetPlayerRole(br.Killer);

            if (br.KillAge < CustomGameOptions.InvestigatorRoleDuration * 1000)
                return
                    $"Body Report: The killer appears to be a {role.Name}! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            if (br.Killer.Is(Faction.Crewmates))
                return
                    $"Body Report: The killer appears to be a Crewmate! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            else if (br.Killer.Is(Faction.Neutral))
                return
                    $"Body Report: The killer appears to be a Neutral Role! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            else
                return
                    $"Body Report: The killer appears to be an Impostor! (Killed {Math.Round(br.KillAge / 1000)}s ago)";
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class KillButtonTarget
    {
        public static byte DontRevive = byte.MaxValue;
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) return true;
            else
            {
                var detective = GetRole<Investigator>(PlayerControl.LocalPlayer);
                if (__instance == detective.ExamineButton) return true;
                else return false;
            }
        }

        public static void SetTarget(KillButton __instance, DeadBody target, Investigator role)
        {
            if (role.CurrentTarget && role.CurrentTarget != target)
            {
                foreach (var body in role.CurrentTarget.bodyRenderers) body.material.SetFloat("_Outline", 0f);
            }

            if (target != null && target.ParentId == DontRevive) target = null;
            role.CurrentTarget = target;
            if (role.CurrentTarget && __instance.enabled)
            {
                SpriteRenderer component = null;
                foreach (var body in role.CurrentTarget.bodyRenderers) component = body;
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", Color.red);
                __instance.graphic.color = Palette.EnabledColor;
                __instance.graphic.material.SetFloat("_Desat", 0f);
                return;
            }

            __instance.graphic.color = Palette.DisabledClear;
            __instance.graphic.material.SetFloat("_Desat", 1f);
        }
    }

    public class EndGame
    {
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
        [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]

        public static class EndGamePatch
        {
            public static void Prefix()
            {
                foreach (var role in GetRoles(RoleEnum.Investigator)) ((Investigator)role).AllPrints.Clear();
            }
        }
    }
    
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformInvestigate
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) return true;
            var role = GetRole<Investigator>(PlayerControl.LocalPlayer);
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (!__instance.enabled) return false;
            var maxDistance = KillDistance();

            if (__instance == role.ExamineButton)
            {
                var flag2 = role.ExamineTimer() == 0f;
                if (!flag2) return false;
                if (!role.ExamineMode) return false;
                if (role.ClosestPlayer == null) return false;
                if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                if (role.ClosestPlayer == null) return false;
                var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                if (interact[3] == true)
                {
                    if (role.ClosestPlayer == role.DetectedKiller) Flash(Color.red);
                    else Flash(Color.green);
                }
                if (interact[0] == true)
                {
                    role.LastExamined = DateTime.UtcNow;
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.LastExamined = DateTime.UtcNow;
                    role.LastExamined = role.LastExamined.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.ExamineCd);
                    return false;
                }
                else if (interact[2] == true) return false;
                return false;
            }
            else
            {
                if (role.CurrentTarget == null)
                    return false;
                if (Vector2.Distance(role.CurrentTarget.TruePosition,
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                var playerId = role.CurrentTarget.ParentId;
                var player = PlayerById(playerId);
                if (player.IsInfected() || role.Player.IsInfected())
                {
                    foreach (var pb in GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
                }
                foreach (var deadPlayer in Murder.KilledPlayers)
                {
                    if (deadPlayer.PlayerId == playerId)
                    {
                        role.DetectedKiller = PlayerById(deadPlayer.KillerId);
                        role.ExamineMode = true;
                    }
                }
                return false;
            }
        }
    }
    
    [HarmonyPatch(typeof(HudManager))]
    public class HudInvestigatorExamine
    {
        public static Sprite ExamineSprite => TownOfSushi.ExamineSprite;

        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateExamineButton(__instance);
        }

        public static void UpdateExamineButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) return;

            var role = GetRole<Investigator>(PlayerControl.LocalPlayer);

            if (role.ExamineButton == null)
            {
                role.ExamineButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ExamineButton.graphic.enabled = true;
                role.ExamineButton.gameObject.SetActive(false);
            }

            role.ExamineButton.graphic.sprite = ExamineSprite;
            role.ExamineButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            role.ExamineButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (role.ExamineMode)
            {
                role.ExamineButton.SetCoolDown(role.ExamineTimer(), CustomGameOptions.ExamineCd);
                SetTarget(ref role.ClosestPlayer, role.ExamineButton, float.NaN);

                var renderer = role.ExamineButton.graphic;
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
            else
            {
                role.ExamineButton.SetCoolDown(0f, 1f);
                var renderer = role.ExamineButton.graphic;
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = KillDistance();
            var flag = (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, KillDistance(),
                LayerMask.GetMask(new[] { "Players", "Ghost" }));

            var killButton = __instance.KillButton;
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;

            foreach (var collider2D in allocs)
            {
                if (!flag || isDead || collider2D.tag != "DeadBody") continue;
                var component = collider2D.GetComponent<DeadBody>();


                if (!(Vector2.Distance(truePosition, component.TruePosition) <=
                      maxDistance)) continue;

                var distance = Vector2.Distance(truePosition, component.TruePosition);
                if (!(distance < closestDistance)) continue;
                closestBody = component;
                closestDistance = distance;
            }

            KillButtonTarget.SetTarget(killButton, closestBody, role);
            killButton.SetCoolDown(0f, 1f);
        }
    }
}