namespace TownOfSushi.Roles
{
    public class Mystic : Role
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();
        public PlayerControl ClosestPlayer;
        public PlayerControl LastExaminedPlayer;
        public DateTime LastExamined { get; set; }
        public Mystic(PlayerControl player) : base(player)
        {
            Name = "Mystic";
            StartText = () => "Find out about the kills";
            TaskText = () => "Understand Kills & investigate about other players";
            RoleInfo = $"The mystic works similar to the Investigator, but with a twist. The Mystic can examine a player to see if they have killed someone recently. If the player has killed someone, the Mystic will be able to see the role/Faction of the killer. The Mystic can also examine a dead body to see who killed them and what role the killer is. The Mystic also gets a list of the possible roles that the examined player can be in meetings. Finally, the Mystic gets a flash and an Arrow pointing to dead bodies for {CustomGameOptions.MysticArrowDuration} seconds.";
            LoreText = "Gifted with an otherworldly sense, you can detect the echoes of violence and betrayal aboard the ship. As the Mystic, you unravel the mysteries behind the deaths of your crewmates, piecing together the truth to uncover the Impostors. Your intuition and insight are vital to the survival of the crew.";
            Color = Colors.Mystic;
            RoleType = RoleEnum.Mystic;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewInvest;
        }
        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            BodyArrows.Remove(arrow.Key);
        }
        public float ExamineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastExamined;
            var num = CustomGameOptions.MysticExamineCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mystic)) return true;
            var role = GetRole<Mystic>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.ExamineTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = KillDistance();
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, false);
            if (interact[3] == true)
            {
                var hasKilled = false;
                foreach (var player in Murder.KilledPlayers)
                {
                    if (player.KillerId == role.ClosestPlayer.PlayerId && (float)(DateTime.UtcNow - player.KillTime).TotalSeconds < CustomGameOptions.RecentKill)
                    {
                        hasKilled = true;
                    }
                }
                if (hasKilled) Flash(Color.red);
                else Flash(Color.green);
                role.LastExaminedPlayer = role.ClosestPlayer;
            }
            if (interact[0] == true)
            {
                role.LastExamined = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastExamined = DateTime.UtcNow;
                role.LastExamined = role.LastExamined.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.MysticExamineCd);
                return false;
            }
            else if (interact[2] == true) return false;
            return false;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MysticMeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mystic)) return;
            if (!CustomGameOptions.ExamineReportOn) return;
            var MysticRole = GetRole<Mystic>(PlayerControl.LocalPlayer);
            if (MysticRole.LastExaminedPlayer != null)
            {
                var playerResults = MysticBodyReport.PlayerReportFeedback(MysticRole.LastExaminedPlayer);
                var roleResults = MysticBodyReport.RoleReportFeedback(MysticRole.LastExaminedPlayer);

                if (!string.IsNullOrWhiteSpace(playerResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, playerResults);
                if (!string.IsNullOrWhiteSpace(roleResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, roleResults);
            }
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite Arrow => TownOfSushi.Arrow;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mystic)) return;
            var role = GetRole<Mystic>(PlayerControl.LocalPlayer);
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();

            if (!PlayerControl.LocalPlayer.Data.IsDead)
            {
                var validBodies = Object.FindObjectsOfType<DeadBody>().Where(x =>
                    Murder.KilledPlayers.Any(y => y.PlayerId == x.ParentId && y.KillTime.AddSeconds(CustomGameOptions.MysticArrowDuration) > System.DateTime.UtcNow));

                foreach (var bodyArrow in role.BodyArrows.Keys)
                {
                    if (!validBodies.Any(x => x.ParentId == bodyArrow))
                    {
                        role.DestroyArrow(bodyArrow);
                    }
                }

                foreach (var body in validBodies)
                {
                    if (!role.BodyArrows.ContainsKey(body.ParentId))
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = Arrow;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        role.BodyArrows.Add(body.ParentId, arrow);
                    }
                    role.BodyArrows.GetValueSafe(body.ParentId).target = body.TruePosition;
                }
            }
            else
            {
                if (role.BodyArrows.Count != 0)
                {
                    role.BodyArrows.Values.DestroyAll();
                    role.BodyArrows.Clear();
                }
            }
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    public class HudExamine
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateMysticExamineButton(__instance);
        }

        public static void UpdateMysticExamineButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mystic)) return;

            var examineButton = __instance.KillButton;
            var role = GetRole<Mystic>(PlayerControl.LocalPlayer);

            examineButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            examineButton.SetCoolDown(role.ExamineTimer(), CustomGameOptions.MysticExamineCd);
            SetTarget(ref role.ClosestPlayer, examineButton, float.NaN);

            var renderer = examineButton.graphic;
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

    public class MysticBodyReport
    {
        public PlayerControl Killer { get; set; }
        public PlayerControl Reporter { get; set; }
        public PlayerControl Body { get; set; }
        public float KillAge { get; set; }

        public static string ParseMysticBodyReport(MysticBodyReport br)
        {
            if (br.KillAge > CustomGameOptions.MysticFactionDuration * 1000)
                return
                    $"Body Report: The corpse is too old to gain information from. (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            if (br.Killer.PlayerId == br.Body.PlayerId)
                return
                    $"Body Report: The kill appears to have been a suicide! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            var role = GetPlayerRole(br.Killer);

            if (br.KillAge < CustomGameOptions.MysticRoleDuration * 1000)
                return
                    $"Body Report: The killer appears to be a {role.Name}! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            if (br.Killer.Is(Faction.Crewmates))
                return
                    $"Body Report: The killer appears to be a Crewmate! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            else if (br.Killer.Is(RoleAlignment.NeutralKilling) || br.Killer.Is(RoleAlignment.NeutralBenign))
                return
                    $"Body Report: The killer appears to be a Neutral Role! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            else
                return
                    $"Body Report: The killer appears to be an Impostor! (Killed {Math.Round(br.KillAge / 1000)}s ago)";
        }

        public static string PlayerReportFeedback(PlayerControl player)
        {
            if (player.Is(RoleEnum.Imitator) || StartImitate.ImitatingPlayer == player
                || player.Is(RoleEnum.Morphling) || player.Is(RoleEnum.Mystic)
                || player.Is(RoleEnum.Glitch))
                return "Your target has an altered perception of reality";
            else if (player.Is(RoleEnum.Blackmailer) || player.Is(RoleEnum.Witch) || player.Is(RoleEnum.Mystic) || player.Is(RoleEnum.Doomsayer)
                 || player.Is(RoleEnum.Oracle) || player.Is(RoleEnum.Agent) || player.Is(RoleEnum.Trapper))
                return "Your target has an insight for private information";
            else if (player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Janitor) || player.Is(RoleEnum.Vulture)
                 || player.Is(RoleEnum.Medium) ||  player.Is(RoleEnum.Hitman) || player.Is(RoleEnum.Undertaker) || player.Is(RoleEnum.Vampire))
                return "Your target has an unusual obsession with dead bodies";
            else if (player.Is(RoleEnum.Investigator) || player.Is(RoleEnum.Swooper) || player.Is(RoleEnum.Tracker)
                || player.Is(RoleEnum.Venerer) || player.Is(RoleEnum.Werewolf) || player.Is(RoleEnum.SerialKiller))
                return "Your target is well trained in hunting down prey";
            else if (player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Plaguebearer)
                  ||player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Transporter))
                return "Your target spreads fear amonst the group";
            else if (player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escapist) || player.Is(RoleEnum.Grenadier)
                || player.Is(RoleEnum.GuardianAngel) || player.Is(RoleEnum.Medic) || player.Is(RoleEnum.Romantic))
                return "Your target hides to guard themself or others";
            else if (player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Jester)
                 || player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Swapper))
                return "Your target has a trick up their sleeve";
            else if (player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Juggernaut) || player.Is(RoleEnum.Pestilence)
                  || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Jailor) ||  player.Is(RoleEnum.Warlock))
                return "Your target is capable of performing relentless attacks";
            else if (player.Is(RoleEnum.Crewmate) || player.Is(RoleEnum.Impostor))
                return "Your target appears to be roleless lol";
            else
                return "Error";
        }

        public static string RoleReportFeedback(PlayerControl player)
        {
            if (player.Is(RoleEnum.Imitator) || StartImitate.ImitatingPlayer == player
                || player.Is(RoleEnum.Morphling) || player.Is(RoleEnum.Mystic)
                || player.Is(RoleEnum.Glitch))
                return "(Imitator, Morphling, Mystic or The Glitch)";
            else if (player.Is(RoleEnum.Blackmailer) || player.Is(RoleEnum.Witch) || player.Is(RoleEnum.Mystic) || player.Is(RoleEnum.Doomsayer)
                 || player.Is(RoleEnum.Oracle) || player.Is(RoleEnum.Agent) || player.Is(RoleEnum.Trapper))
                return "(Blackmailer, Mystic, Witch, Doomsayer, Agent, Oracle or Trapper)";
            else if (player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Janitor) || player.Is(RoleEnum.Vulture)
                 || player.Is(RoleEnum.Medium) || player.Is(RoleEnum.Undertaker) || player.Is(RoleEnum.Hitman) || player.Is(RoleEnum.Vampire))
                return "(Amnesiac, Janitor, Medium, Hitman Undertaker, Vulture or Vampire)";
            else if (player.Is(RoleEnum.Investigator) || player.Is(RoleEnum.Swooper) || player.Is(RoleEnum.Tracker)
                || player.Is(RoleEnum.Venerer) || player.Is(RoleEnum.Werewolf) || player.Is(RoleEnum.SerialKiller))
                return "(Investigator, Swooper, Tracker, Vampire Hunter, Venerer, Serial Killer or Werewolf)";
            else if (player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Plaguebearer)
                  ||player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Transporter))
                return "(Arsonist, Miner, Plaguebearer, Seer or Transporter)";
            else if (player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escapist) || player.Is(RoleEnum.Grenadier)
                || player.Is(RoleEnum.GuardianAngel) || player.Is(RoleEnum.Medic) || player.Is(RoleEnum.Romantic))
                return "(Engineer, Escapist, Grenadier, Guardian Angel, Medic or Romantic)";
            else if (player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Jester)
                 || player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Swapper) || player.Is(RoleEnum.Veteran))
                return "(Executioner, Jester, Hunter, Swapper, Traitor or Veteran)";
            else if (player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Jailor) || player.Is(RoleEnum.Juggernaut) || player.Is(RoleEnum.Pestilence)
                 || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Warlock))
                return "(Bomber, Juggernaut, Pestilence, Vigilante, Jailor or Warlock)";
            else if (player.Is(RoleEnum.Crewmate) || player.Is(RoleEnum.Impostor))
                return "(Crewmate or Impostor)";
            else return "Error";
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    internal class MysticBodyReportPatch
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

            var isMysticAlive = __instance.Is(RoleEnum.Mystic);
            var areReportsEnabled = CustomGameOptions.MysticReportOn;

            if (!isMysticAlive || !areReportsEnabled)
                return;

            var isUserMystic = PlayerControl.LocalPlayer.Is(RoleEnum.Mystic);
            if (!isUserMystic)
                return;
            var br = new MysticBodyReport
            {
                Killer = PlayerById(killer.KillerId),
                Reporter = __instance,
                Body = PlayerById(killer.PlayerId),
                KillAge = (float) (DateTime.UtcNow - killer.KillTime).TotalMilliseconds
            };

            var reportMsg = MysticBodyReport.ParseMysticBodyReport(br);

            if (string.IsNullOrWhiteSpace(reportMsg))
                return;

            if (DestroyableSingleton<HudManager>.Instance)
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, reportMsg);
        }
    }
}