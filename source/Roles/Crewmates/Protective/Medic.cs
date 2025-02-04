namespace TownOfSushi.Roles
{
    public class Medic : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public Dictionary<int, string> LightDarkColors = new Dictionary<int, string>();
        public DateTime StartingCooldown { get; set; }
        public Medic(PlayerControl player) : base(player)
        {
            var CanBreak = CustomGameOptions.ShieldBreaks ? "breaks if the shielded player gets a murder attempt" : "cannot break";
            Name = "Medic";
            StartText = () => "Create A Shield To Protect A Crewmate";
            TaskText = () => "Protect a crewmate with a shield";
            RoleInfo = $"As the Medic, you can protect a crewmate with a shield to prevent them from being killed by the impostor. The shield lasts for the whole game. The shield {CanBreak}. When the player has a kill attempt, the Medic gets a flash indicating it. \n - Note: Having a shield does not mean the player is good";
            LoreText = "A skilled protector, you specialize in keeping your fellow Crewmates safe. As the Medic, you can create shields to protect others from harm, ensuring that the most vulnerable stay safe from the deadly hands of the Impostors. Your quick thinking and ability to safeguard others make you a vital part of the crew's survival.";
            Color = ColorManager.Medic;
            StartingCooldown = DateTime.UtcNow;
            RoleType = RoleEnum.Medic;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewProtect;
            ShieldedPlayer = null;
            LightDarkColors.Add(0, "darker"); // Red
            LightDarkColors.Add(1, "darker"); // Blue
            LightDarkColors.Add(2, "darker"); // Green
            LightDarkColors.Add(3, "lighter"); // Pink
            LightDarkColors.Add(4, "lighter"); // Orange
            LightDarkColors.Add(5, "lighter"); // Yellow
            LightDarkColors.Add(6, "darker"); // Black
            LightDarkColors.Add(7, "lighter"); // White
            LightDarkColors.Add(8, "darker"); // Purple
            LightDarkColors.Add(9, "darker"); // Brown
            LightDarkColors.Add(10, "lighter"); // Cyan
            LightDarkColors.Add(11, "lighter"); // Lime
            LightDarkColors.Add(12, "darker"); // Maroon
            LightDarkColors.Add(13, "lighter"); // Rose
            LightDarkColors.Add(14, "lighter"); // Banana
            LightDarkColors.Add(15, "darker"); // Grey
            LightDarkColors.Add(16, "darker"); // Tan
            LightDarkColors.Add(17, "lighter"); // Coral
            LightDarkColors.Add(18, "darker"); // Watermelon
            LightDarkColors.Add(19, "darker"); // Chocolate
            LightDarkColors.Add(20, "lighter"); // Sky Blue
            LightDarkColors.Add(21, "lighter"); // Biege
            LightDarkColors.Add(22, "darker"); // Magenta
            LightDarkColors.Add(23, "lighter"); // Turquoise
            LightDarkColors.Add(24, "lighter"); // Lilac
            LightDarkColors.Add(25, "darker"); // Olive
            LightDarkColors.Add(26, "lighter"); // Azure
            LightDarkColors.Add(27, "darker"); // Plum
            LightDarkColors.Add(28, "darker"); // Jungle
            LightDarkColors.Add(29, "lighter"); // Mint
            LightDarkColors.Add(30, "lighter"); // Chartreuse
            LightDarkColors.Add(31, "darker"); // Macau
            LightDarkColors.Add(32, "darker"); // Tawny
            LightDarkColors.Add(33, "lighter"); // Gold
            LightDarkColors.Add(34, "lighter"); // Rainbow
            LightDarkColors.Add(35, "lighter"); // Monochrome
            LightDarkColors.Add(36, "lighter"); // Galaxy
            LightDarkColors.Add(37, "lighter"); // Ice
            LightDarkColors.Add(38, "lighter"); // Sunrise
            LightDarkColors.Add(39, "lighter"); // Peach
            LightDarkColors.Add(40, "lighter"); // Fire
            LightDarkColors.Add(41, "lighter"); // Water
        }
        public float StartTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - StartingCooldown;
            var num = 10000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public PlayerControl ClosestPlayer;
        public bool UsedAbility { get; set; } = false;
        public PlayerControl ShieldedPlayer { get; set; }
        public PlayerControl exShielded { get; set; }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public class Murder
    {
        public static List<DeadPlayer> KilledPlayers = new List<DeadPlayer>();
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            var deadBody = new DeadPlayer
            {
                PlayerId = target.PlayerId,
                KillerId = __instance.PlayerId,
                KillTime = DateTime.UtcNow
            };
            KilledPlayers.Add(deadBody);
        }
    }

    public class DeadPlayer
    {
        public byte KillerId { get; set; }
        public byte PlayerId { get; set; }
        public DateTime KillTime { get; set; }
    }
    public class MedicBodyReport
    {
        public PlayerControl Killer { get; set; }
        public PlayerControl Reporter { get; set; }
        public PlayerControl Body { get; set; }
        public float KillAge { get; set; }

        public static string ParseBodyReport(BodyReport br)
        {
            //System.Console.WriteLine(br.KillAge);
            if (br.KillAge > CustomGameOptions.MedicReportColorDuration * 1000)
                return
                    $"Body Report: The corpse is too old to gain information from. (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            if (br.Killer.PlayerId == br.Body.PlayerId)
                return
                    $"Body Report: The kill appears to have been a suicide! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            if (br.KillAge < CustomGameOptions.MedicReportNameDuration * 1000)
                return
                    $"Body Report: The killer appears to be {br.Killer.Data.PlayerName}! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            var colors = new Dictionary<int, string>
            {
                {0, "darker"},// red
                {1, "darker"},// blue
                {2, "darker"},// green
                {3, "lighter"},// pink
                {4, "lighter"},// orange
                {5, "lighter"},// yellow
                {6, "darker"},// black
                {7, "lighter"},// white
                {8, "darker"},// purple
                {9, "darker"},// brown
                {10, "lighter"},// cyan
                {11, "lighter"},// lime
                {12, "darker"},// maroon
                {13, "lighter"},// rose
                {14, "lighter"},// banana
                {15, "darker"},// gray
                {16, "darker"},// tan
                {17, "lighter"},// coral
                {18, "darker"},// watermelon
                {19, "darker"},// chocolate
                {20, "lighter"},// sky blue
                {21, "lighter"},// beige
                {22, "darker"},// magenta
                {23, "lighter"},// turquoise
                {24, "lighter"},// lilac
                {25, "darker"},// olive
                {26, "lighter"},// azure
                {27, "darker"},// plum
                {28, "darker"},// jungle
                {29, "lighter"},// mint
                {30, "lighter"},// chartreuse
                {31, "darker"},// macau
                {32, "darker"},// tawny
                {33, "lighter"},// gold
                {34, "lighter"},// rainbow
                {35, "lighter"},// monochrome
                {36, "lighter"},// Galaxy
                {37, "lighter"},// Ice
                {38, "lighter"},// Sunrise
                {39, "lighter"},// Peach
                {40, "lighter"},// Fire
                {41, "lighter"},// Water
            };
            var typeOfColor = colors[br.Killer.GetDefaultOutfit().ColorId];
            return
                $"Body Report: The killer appears to be a {typeOfColor} color. (Killed {Math.Round(br.KillAge / 1000)}s ago)";
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class MedicStopKill
    {
        public static void BreakShield(byte medicId, byte playerId, bool flag)
        {
            if (PlayerControl.LocalPlayer.PlayerId == playerId && CustomGameOptions.NotificationShield == NotificationOptions.Shielded) 
            {
                Flash(Color.red, 0.5f);
            }

            if (PlayerControl.LocalPlayer.PlayerId == medicId && CustomGameOptions.NotificationShield == NotificationOptions.Medic) 
            {
                Flash(Color.red, 0.5f);
            }

            if (CustomGameOptions.NotificationShield == NotificationOptions.Everyone) 
            {
                Flash(Color.red, 0.5f);
            }

            if (!flag)
                return;

            var player = PlayerById(playerId);
            foreach (var role in GetRoles(RoleEnum.Medic))
                if (((Medic) role).ShieldedPlayer.PlayerId == playerId)
                {
                    ((Medic) role).ShieldedPlayer = null;
                    ((Medic) role).exShielded = player;
                    System.Console.WriteLine(player.name + " Is Ex-Shielded");
                }

            player.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
            player.myRend().material.SetFloat("_Outline", 0f);
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class MedicShowShield
    {
        public static Color ProtectedColor = Color.cyan;

        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Medic))
            {
                var medic = (Medic) role;

                var exPlayer = medic.exShielded;
                if (exPlayer != null)
                {
                    System.Console.WriteLine(exPlayer.name + " is ex-Shielded and unvisored");
                    exPlayer.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                    exPlayer.myRend().material.SetFloat("_Outline", 0f);
                    medic.exShielded = null;
                    continue;
                }

                var player = medic.ShieldedPlayer;
                if (player == null) continue;

                if (player.Data.IsDead || medic.Player.Data.IsDead || medic.Player.Data.Disconnected)
                {
                    MedicStopKill.BreakShield(medic.Player.PlayerId, player.PlayerId, true);
                    medic.UsedAbility = false;
                    continue;
                }
            }
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    public class MedicHUDProtect
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Medic)) return;

            var protectButton = __instance.KillButton;
            var role = GetRole<Medic>(PlayerControl.LocalPlayer);

            protectButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            protectButton.SetCoolDown(role.StartTimer(), 10f);
            if (role.UsedAbility) return;
            SetTarget(ref role.ClosestPlayer, protectButton);
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class MedicProtect
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != HUDManager().KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Medic);
            if (!flag) return true;
            var role = GetRole<Medic>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (IsDead()) return false;
            if (!__instance.enabled) return false;
            if (role.UsedAbility || role.ClosestPlayer == null) return false;
            if (role.StartTimer() > 0) return false;

            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[3] == true)
            {
                StartRPC(CustomRPC.Protect, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);

                role.ShieldedPlayer = role.ClosestPlayer;
                role.UsedAbility = true;
                return false;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    internal class MedicBodyReportPatch
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

            var isMedicAlive = __instance.Is(RoleEnum.Medic);
            var areReportsEnabled = CustomGameOptions.ShowReports;

            if (!isMedicAlive || !areReportsEnabled)
                return;

            var isUserMedic = PlayerControl.LocalPlayer.Is(RoleEnum.Medic);
            if (!isUserMedic)
                return;
            //System.Console.WriteLine("RBTHREEF");
            var br = new BodyReport
            {
                Killer = PlayerById(killer.KillerId),
                Reporter = __instance,
                Body = PlayerById(killer.PlayerId),
                KillAge = (float) (DateTime.UtcNow - killer.KillTime).TotalMilliseconds
            };

            //System.Console.WriteLine("FIVEF");

            var reportMsg = BodyReport.ParseBodyReport(br);

            //System.Console.WriteLine("SIXTHF");

            if (string.IsNullOrWhiteSpace(reportMsg))
                return;

            //System.Console.WriteLine("SEFENFTH");

            if (HUDManager())
                // Send the message through chat only visible to the medic
                HUDManager().Chat.AddChat(PlayerControl.LocalPlayer, reportMsg);
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MedicAddButton
    {
        private static Sprite LighterSprite => TownOfSushi.LighterSprite;
        public static Sprite DarkerSprite => TownOfSushi.DarkerSprite;

        private static bool IsExempt(PlayerVoteArea voteArea)
        {
            var player = Utils.PlayerById(voteArea.TargetPlayerId);
            if (player == null || player.Data.Disconnected) {
                return true;
            } else {
                return false;
            }
        }

        public static void GenButton(Medic role, PlayerVoteArea voteArea)
        {
            
            var targetId = voteArea.TargetPlayerId;
            if (IsExempt(voteArea))
            {
                return;
            }
            var colorButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var newButton = Object.Instantiate(colorButton, voteArea.transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();

            PlayerControl playerControl = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => p.PlayerId == voteArea.TargetPlayerId);

            if (role.LightDarkColors[playerControl.GetDefaultOutfit().ColorId] == "lighter") {
                renderer.sprite = LighterSprite;
            } else {
                renderer.sprite = DarkerSprite;
            }
            newButton.transform.position = colorButton.transform.position - new Vector3(-0.8f, 0.2f, -2f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = colorButton.transform.parent.parent;
            var newButtonClickEvent = new UnityEngine.UI.Button.ButtonClickedEvent();
            newButtonClickEvent.AddListener(LighterDarkerHandler());
            newButton.GetComponent<PassiveButton>().OnClick = newButtonClickEvent;
            role.Buttons.Add(newButton);
        }

        private static Action LighterDarkerHandler()
        {
            //Used to avoid the Lighter/Darker Indicators causing any button problems by giving them their own Listener events.
            void Listener()
            {
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Medic))
            {
                var medic = (Medic) role;
                medic.Buttons.Clear();
            }
            if (CustomGameOptions.MedicReportColorDuration == 0) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Medic)) return;
            if (IsDead()) return;
            var medicrole = GetRole<Medic>(PlayerControl.LocalPlayer);
            foreach (var voteArea in __instance.playerStates)
            {
                try {
                    GenButton(medicrole, voteArea);
                } catch {
                }
            }
        }
    }
}